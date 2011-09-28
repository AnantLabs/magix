/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using System.Net;
using System.IO;
using Magix.UX;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.Messaging;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Here mostly for Editing User, and also to some extent creating default 
     * users and roles upon startup if none exists
     */
    [ActiveController]
    public class EditUser_Controller : ActiveController
    {
        /**
         * Level2: Makes sure we have Administrator and User Roles, in addition creates a default
         * user [admin/admin] if no user exists
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.SaveSettingsSection",
                "Magix.Core.SaveSettingsSection-Override");
            ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.LoadSettingsSection",
                "Magix.Core.LoadSettingsSection-Override");
            ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.SaveSettingsSection-Passover",
                "Magix.Core.SaveSettingsSection");
            ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.LoadSettingsSection-Passover",
                "Magix.Core.LoadSettingsSection");
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // Creating a couple of default User/Role objects, so our CMS users
                // can log in ...
                if (Role.CountWhere(Criteria.Eq("Name", "Administrator")) == 0)
                {
                    Role role = new Role();
                    role.Name = "Administrator";
                    role.Save();
                }
                if (Role.CountWhere(Criteria.Eq("Name", "User")) == 0)
                {
                    Role role = new Role();
                    role.Name = "User";
                    role.Save();
                }
                if (User.Count == 0)
                {
                    // Oops, creating our default user [admin/admin]
                    User admin = new User();

                    admin.Username = "admin";
                    admin.Password = "admin";
                    admin.AvatarURL = "media/images/avatars/marvin-headshot.png";
                    admin.Roles.Add(Role.SelectFirst(Criteria.Eq("Name", "Administrator")));

                    admin.SaveNoVerification();
                }
                tr.Commit();
            }
        }

        /**
         * Level2: Overridden to make sure setting sections are saved and loaded per user
         */
        [ActiveEvent(Name = "Magix.Core.SaveSettingsSection-Override")]
        protected void Magix_Core_SaveSettingsSection_Override(object sender, ActiveEventArgs e)
        {
            if (User.Current != null)
                e.Params["SectionName"].Value = User.Current.Username + "-" + 
                    e.Params["SectionName"].Get<string>();
            RaiseEvent(
                "Magix.Core.SaveSettingsSection-Passover",
                e.Params);
        }

        /**
         * Level2: Overridden to make sure setting sections are saved and loaded per user
         */
        [ActiveEvent(Name = "Magix.Core.LoadSettingsSection-Override")]
        protected void Magix_Core_LoadSettingsSection_Override(object sender, ActiveEventArgs e)
        {
            if (User.Current != null)
                e.Params["SectionName"].Value = User.Current.Username + "-" +
                    e.Params["SectionName"].Get<string>();
            RaiseEvent(
                "Magix.Core.LoadSettingsSection-Passover",
                e.Params);
        }

        /**
         * Level2: Allows editing of all users, and searching and changing properties and such. Shows
         * all Users in a Grid
         */
        [ActiveEvent(Name = "Magix.Publishing.EditUsers")]
        protected void Magix_Publishing_EditUsers(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(User).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Username"].Value = true;
            node["WhiteListColumns"]["Username"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Password"].Value = true;
            node["WhiteListColumns"]["Password"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Roles"].Value = true;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Publishing.EditUser";
            node["CreateEventName"].Value = "Magix.Publishing.CreateUser";

            node["Type"]["Properties"]["Username"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Password"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Roles"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Roles"]["Header"].Value = "Role"; // Singular form ...
            node["Type"]["Properties"]["Roles"]["TemplateColumnEvent"].Value = 
                "Magix.Publishing.GetRoleTemplateColumn";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level3: Get 'select Role' template column for our Grid. Creates a SelectList and returns
         * back to caller.
         * PS!
         * We only allow for editing the user such that he or she can belong to only one Role. If you'd
         * like to extend this logic to allow for more than one Role to be added, this is where you'd want to
         * do such, probably by overriding this event, and choose some sort of 'multiple choice' control
         * to return instead
         */
        [ActiveEvent(Name = "Magix.Publishing.GetRoleTemplateColumn")]
        private void Magix_Publishing_GetRoleTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Fetching specific user
            User user = User.SelectByID(id);

            // Creating our SelectList
            SelectList ls = new SelectList();
            ls.CssClass = "mux-grid-select";
            ls.Info = id.ToString();
            ls.Items.Add(new ListItem("None", "-1"));

            foreach (Role idx in Role.Select())
            {
                ListItem li = new ListItem(idx.Name, idx.ID.ToString());
                if (user.InRole(idx.Name))
                    li.Selected = true;
                ls.Items.Add(li);
            }

            // Supplying our Event Handler for the Changed Event ...
            ls.SelectedIndexChanged += ls_SelectedIndexChanged;

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        #region [ -- Event handler for SelectList SelectedIndexChanged from Template column -- ]

        // TODO: Refactor to 'generalized Select List Control thingie ...'

        protected void ls_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectList sl = sender as SelectList;
            int id = int.Parse(sl.Info.Split('|')[0]);

            // Getting our user
            User user2 = User.SelectByID(id);

            // Changing our user's role belonging ...
            user2.Roles.Clear();
            int idOfRole = int.Parse(sl.SelectedItem.Value);
            if (idOfRole != -1)
            {
                user2.Roles.Add(Role.SelectByID(idOfRole));
            }

            // Saving ...
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                user2.Save();
                tr.Commit();
            }

            // Signalizing to Grids that they need to update ...
            Node n = new Node();
            n["ID"].Value = id;
            n["PropertyName"].Value = "Roles";
            n["NewValue"].Value = sl.SelectedItem.Value;
            n["FullTypeName"].Value = typeof(User).FullName;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.UpdateGrids",
                n);
        }

        #endregion

        /**
         * Level2: Edit one specific User, and all of his properties
         */
        [ActiveEvent(Name = "Magix.Publishing.EditUser")]
        protected void Magix_Publishing_EditUser(object sender, ActiveEventArgs e)
        {
            // Getting the requested User ...
            User user = User.SelectByID(e.Params["ID"].Get<int>());

            // Loading Avatar for User
            LoadEditUserImage(user);

            // Loading the Edit Users properties ... [name, username etc]
            LoadEditUserObject(sender, e, user);

            // Pops up the Edit User Settings grid ...
            LoadEditUserSettings(user.ID);

            // Pops up the Edit User OpenID Tokens Grid
            LoadEditUserOpenIDs(user.ID);
        }

        /*
         * Used by EditUser logic
         */
        private static void LoadEditUserImage(User user)
        {
            Node node = new Node();

            node["Append"].Value = false;
            node["Events"]["Click"].Value = "Magix.Publishing.ChangeAvatarForUser";
            node["Events"]["Click"]["ID"].Value = user.ID;
            node["ChildCssClass"].Value = "span-4 height-20 blockImage clear-both";
            node["Seed"].Value = user.ID;
            node["Padding"].Value = 6;
            node["Top"].Value = 1;
            node["MarginBottom"].Value = 18;
            node["Width"].Value = 18;
            node["Container"].Value = "content4";
            node["AlternateText"].Value = "Avatar of User";
            node["Description"].Value = "Click image to change it ...";
            node["ImageURL"].Value = user.AvatarURL;

            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.CommonModules.ImageModule",
                "content4",
                node);
        }

        /*
         * Used by EditUser logic
         */
        private void LoadEditUserObject(object sender, ActiveEventArgs e, User user)
        {
            // First filtering OUT columns ...!
            e.Params["WhiteListColumns"]["FullName"].Value = true;
            e.Params["WhiteListColumns"]["Username"].Value = true;
            e.Params["WhiteListColumns"]["Password"].Value = true;
            e.Params["WhiteListColumns"]["Roles"].Value = true;
            e.Params["WhiteListColumns"]["Email"].Value = true;

            e.Params["WhiteListColumns"]["Phone"].Value = true;
            e.Params["WhiteListColumns"]["Address"].Value = true;
            e.Params["WhiteListColumns"]["City"].Value = true;
            e.Params["WhiteListColumns"]["Zip"].Value = true;
            e.Params["WhiteListColumns"]["State"].Value = true;
            e.Params["WhiteListColumns"]["Twitter"].Value = true;
            e.Params["WhiteListColumns"]["Facebook"].Value = true;

            e.Params["WhiteListColumns"]["BirthDate"].Value = true;
            e.Params["WhiteListColumns"]["Country"].Value = true;
            e.Params["WhiteListColumns"]["Gender"].Value = true;
            e.Params["WhiteListColumns"]["Language"].Value = true;
            e.Params["WhiteListColumns"]["Nickname"].Value = true;
            e.Params["WhiteListColumns"]["TimeZone"].Value = true;

            e.Params["WhiteListProperties"]["Name"].Value = true;
            e.Params["WhiteListProperties"]["Value"].Value = true;

            e.Params["Type"]["Properties"]["FullName"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["FullName"]["Bold"].Value = true;
            e.Params["Type"]["Properties"]["Username"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Password"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Roles"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Roles"]["TemplateColumnEvent"].Value =
                "Magix.Publishing.GetRoleTemplateColumn";
            e.Params["Type"]["Properties"]["Email"]["ReadOnly"].Value = false;

            e.Params["Type"]["Properties"]["Phone"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Address"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["City"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Zip"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["State"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Twitter"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Facebook"]["ReadOnly"].Value = false;

            e.Params["Type"]["Properties"]["BirthDate"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Country"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Gender"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Language"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["Nickname"]["ReadOnly"].Value = false;
            e.Params["Type"]["Properties"]["TimeZone"]["ReadOnly"].Value = false;

            e.Params["Padding"].Value = 6;
            e.Params["Width"].Value = 18;
            e.Params["Append"].Value = true;
            e.Params["Container"].Value = "content4";

            e.Params["Caption"].Value =
                string.Format(
                    "Editing User: {0}",
                    user.Username);

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                e.Params);
        }

        /*
         * Used by EditUser logic
         */
        protected void LoadEditUserSettings(int id)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["MarginBottom"].Value = 20;
            node["PullTop"].Value = 18;
            node["Container"].Value = "content5";

            node["PropertyName"].Value = "Settings";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(User).FullName;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Value"].Value = true;
            node["WhiteListColumns"]["Value"]["ForcedWidth"].Value = 9;

            node["Type"]["Properties"]["Name"]["Header"].Value = "Name";
            node["Type"]["Properties"]["Value"]["Header"].Value = "Setting Value";
            node["Type"]["Properties"]["Value"]["MaxLength"].Value = 40;

            node["ID"].Value = id;
            node["NoIdColumn"].Value = true;
            node["ReUseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);
        }

        /*
         * Used by EditUser logic
         */
        private void LoadEditUserOpenIDs(int id)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["MarginBottom"].Value = 20;
            node["PullTop"].Value = 18;
            node["Container"].Value = "content6";

            node["PropertyName"].Value = "OpenIDTokens";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(User).FullName;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Name"]["Header"].Value = "OpenID";

            node["ID"].Value = id;
            node["NoIdColumn"].Value = true;
            node["ReUseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);
        }

        /**
         * Level2: Callback for creating a New User from Dashboard that pops up editing
         * the specific user after his initial creation
         */
        [ActiveEvent(Name = "Magix.Publishing.CreateUser")]
        protected void Magix_Publishing_CreateUser(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                User u = new User();
                u.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = u.ID;
                node["FullTypeName"].Value = typeof(User).FullName;

                RaiseEvent(
                    "Magix.Publishing.EditUser",
                    node);
            }
        }

        /**
         * Level2: User has requested a change of Avatar for either himself, or another user.
         * Loads up the FileExplorer Selector, and allows for selecting a new avatar 
         * for the admin user
         */
        [ActiveEvent(Name = "Magix.Publishing.ChangeAvatarForUser")]
        protected void Magix_Publishing_ChangeAvatarForUser(object sender, ActiveEventArgs e)
        {
            User u = User.SelectByID(e.Params["ID"].Get<int>());

            string clientImageFolder = "media/images/avatars/";

            Node node = new Node();

            node["Top"].Value = 2;
            node["IsSelect"].Value = true;
            node["Push"].Value = 1;
            node["Width"].Value = 23;
            node["Folder"].Value = clientImageFolder;
            node["Filter"].Value = "*.png;*.gif;*.jpg;*.jpeg;";
            node["SelectEvent"].Value = "Magix.Publishing.SelectImageForUser";
            node["SelectEvent"]["Params"]["UserID"].Value = u.ID;
            node["Seed"].Value = u.ID;

            RaiseEvent(
                "Magix.FileExplorer.LaunchExplorer",
                node);
        }

        /**
         * Level2: New image was selected in our FileExplorer as a new Avatar for our User. Will
         * change the User's Avatar and save it and update 
         */
        [ActiveEvent(Name = "Magix.Publishing.SelectImageForUser")]
        protected void Magix_Publishing_SelectImageForUser(object sender, ActiveEventArgs e)
        {
            User u = User.SelectByID(e.Params["Params"]["UserID"].Get<int>());
            u.AvatarURL = e.Params["Folder"].Get<string>() + e.Params["FileName"].Get<string>();
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                u.Save();
                tr.Commit();
            }
            ActiveEvents.Instance.RaiseClearControls("child");

            // To update our Edit User / Image Module UI parts ...
            Node node = new Node();

            node["ImageURL"].Value = u.AvatarURL;
            node["Seed"].Value = u.ID;

            // Update our Image Avatar Control
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.ChangeImage",
                node);
        }

        /**
         * Level2: Callback for supplying the Default Avatar for a User. Override
         * this one if you wish to change the default image for a new user, which might
         * be useful in e.g. OpenID scenarios and profiling efforts
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDefaultAvatarURL")]
        protected void Magix_Publishing_GetDefaultAvatarURL(object sender, ActiveEventArgs e)
        {
            e.Params["URL"].Value = "media/images/avatars/marvin-headshot.png";
        }
    }
}
