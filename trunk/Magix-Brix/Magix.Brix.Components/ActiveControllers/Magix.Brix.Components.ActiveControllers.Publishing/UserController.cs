/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class UserController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                // Creating a couple of default User/Role objects, so our CMS users
                // can log in ...
                if (Role.Count == 0)
                {
                    Role role = new Role();
                    role.Name = "Administrator";
                    role.Save();
                }
                if (User.Count == 0)
                {
                    User admin = new User();
                    admin.Username = "admin";
                    admin.Password = "admin";
                    admin.Roles.Add(Role.SelectFirst(Criteria.Eq("Name", "Administrator")));
                    admin.SaveNoVerification();
                }
                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Brix.Core.InitialLoading")]
        protected void Page_Init_InitialLoading(object sender, ActiveEventArgs e)
        {
            if (User.Current == null && ShouldShowLoginBox())
            {
                // Loading login module ...
                Node node = new Node();

                node["Container"].Value = "content4";
                node["Width"].Value = 8;
                node["Push"].Value = 8;
                node["Last"].Value = true;
                node["Top"].Value = 5;

                RaiseEvent(
                    "Magix.Core.LoadLoginModule",
                    node);
            }
            else if (User.Current != null)
            {
                RaiseEvent("Magix.Publishing.LoadDashboard");
            }
        }

        [ActiveEvent(Name = "Magix.Core.UserLoggedIn")]
        protected void Magix_Core_UserLoggedIn(object sender, ActiveEventArgs e)
        {
            RaiseEvent("Magix.Publishing.LoadDashboard");
        }

        [ActiveEvent(Name = "Magix.Core.LogInUser")]
        protected void Magix_Core_LogInUser(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string password = e.Params["Password"].Get<string>();

            User u = User.SelectFirst(
                Criteria.Eq("Username", username),
                Criteria.Eq("Password", password));

            if (u != null)
            {
                e.Params["Success"].Value = true;
                User.Current = u;

                RaiseEvent("Magix.Core.UserLoggedIn");
            }
        }

        private bool ShouldShowLoginBox()
        {
            return Page.Request.Params["login"] == "true" ||
                Settings.Instance.Get("ShouldShowLogin", true);
        }
    }
}
