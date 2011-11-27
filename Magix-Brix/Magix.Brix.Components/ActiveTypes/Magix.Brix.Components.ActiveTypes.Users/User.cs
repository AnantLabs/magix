/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Globalization;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Generic;
using Magix.Brix.Loader;
using Magix.UX;
using System.Security;

namespace Magix.Brix.Components.ActiveTypes.Users
{
    /**
     * Level3: Settings stored on a 'per user level'. Useful for storing simple fact
     * on a per user level
     */
    [ActiveType]
    internal class UserSettings : ActiveType<UserSettings>
    {
        /**
         * Level3: The Name of the settings. Must be unique within the user. Normally
         * you should prefix these kinds of 'glogal storages' with your company
         * name or something, to avoid nameclashings
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Level3: The Value, stored as string. Convert as you wish yourself
         */
        [ActiveField]
        public string Value { get; set; }
    }

    /**
     * Level3: A User is a single person using your website, being registered 
     * with a username and a password, and hence no longer
     * 'anonymous'. A user is normally recognized through his 'Username', and he has a
     * password which he can use to log into the system, normally. This class
     * encapsulates that logic. PS! This class supports inheriting [though it's not
     * really entirely stable quite yet ...! ]
     */
    [ActiveType(TableName = "docMagix.Brix.Components.ActiveTypes.Users.UserBase")]
    public class UserBase : ActiveTypeCached<UserBase>
    {
        public UserBase()
        {
            Settings = new LazyList<UserSettings>();
            Roles = new LazyList<Role>();
        }

        /**
         * Level3: Username. Normally used to login to the system with, in combination with the user's
         * password
         */
        [ActiveField]
        public string Username { get; set; }

        // TODO: Stored HASHED ....!
        /**
         * Level3: Password. Unique word, character combinations or a sentence which only the user
         * knows about himself, and which is used to authenticate the user upon logging in
         */
        [ActiveField]
        public string Password { get; set; }

        /**
         * Level3: Email address of user
         */
        [ActiveField]
        public string Email { get; set; }

        /**
         * Level3: List of roles. Please notice that every user in Magix might belong to more than one role,
         * though there's not UI currently for adding a user into more than one role, meaning defacto
         * there's only one role per user today
         */
        [ActiveField(IsOwner = false)]
        public LazyList<Role> Roles { get; set; }

        /**
         * Level3: Settings for user instance
         */
        [ActiveField]
        internal LazyList<UserSettings> Settings { get; set; }

        /**
         * Level3: Will return a string containing all roles user belongs to
         */
        public string RolesString
        {
            get
            {
                string retVal = "";
                foreach (Role idx in Roles)
                {
                    retVal += idx.Name + ",";
                }
                return retVal.Trim(',');
            }
        }

        /**
         * Level3: Returns the given setting for the user
         */
        public T GetSetting<T>(string name, T defaultValue)
        {
            foreach (UserSettings idx in Settings)
            {
                if (idx.Name == name)
                    return (T)Convert.ChangeType(
                        idx.Value, 
                        typeof(T), 
                        CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        /**
         * Level3: Removes the given setting for the user
         */
        public void RemoveSetting(string name)
        {
            Settings.RemoveAll(
                delegate(UserSettings idx)
                {
                    return idx.Name == name;
                });
            Save();
        }

        /**
         * Level3: Updates the given setting for the user with the given value
         */
        public void SetSetting<T>(string name, T val)
        {
            foreach (UserSettings idx in Settings)
            {
                if (idx.Name == name)
                {
                    idx.Value = (string)Convert.ChangeType(
                        val, 
                        typeof(string), 
                        CultureInfo.InvariantCulture);
                    idx.Save();
                    return;
                }
            }
            UserSettings u = new UserSettings();
            u.Name = name;
            u.Value = (string)Convert.ChangeType(
                val, 
                typeof(string), 
                CultureInfo.InvariantCulture);
            Settings.Add(u);
            Save();
        }

        /**
         * Level3: Overridden to make sure username is unique etc.
         */
        public override void Save()
        {
            SaveImpl(true);
        }

        /**
         * Level3: Will save the user 'raw' running none of the verification logic to make sure
         * the data is not violating some logic or something
         */
        public void SaveNoVerification()
        {
            SaveImpl(false);
        }

        /**
         * Level3: Implementation of our two other Save methods
         */
        protected void SaveImpl(bool verify)
        {
            if (verify)
            {
                if (Current == null)
                    throw new SecurityException(
                        @"Saving this object requires being logged in, you are not ...!
Please login to retry operation ...");
                if (!Current.InRole("Administrator") &&
                    !Current.InRole("Superuser"))
                {
                    if (Current != this)
                        throw new SecurityException(
                            @"You do not have sufficient rights to save this object");
                    // BUT! You ARE allowed to save *yourself* ... ;)
                    // But only as long as you don't add yourself up to any new roles ...
                    // This to support for changing ones one password and such ...
                }
                if (Username == "Administrator")
                    throw new ArgumentException("That was a very bad username suggestion my friend ...");
                if (string.IsNullOrEmpty(Password))
                {
                    if (ID != 0)
                        throw new ArgumentException(
                            @"Sorry, but there's no way we'd allow you to change your Password to
and empty string ...");
                    Password = "password123";
                }
                if (string.IsNullOrEmpty(Username))
                {
                    if (ID != 0)
                        throw new ArgumentException(
                            @"Sorry, but there's no way we'd allow you to change your Username to an 
empty string ...");
                    Username = "username" + Guid.NewGuid().ToString().Substring(0, 8);
                }
                UserBase other =
                    UserBase.SelectFirst(
                        Criteria.Eq("Username", Username));
                if (other != null && other != this)
                {
                    throw new ArgumentException(
                        @"Sorry, but that Username is already taken by another user, 
usernames must be unique within the application ...");
                }
            }
            if (ID == 0)
            {
                Node node = new Node();

                node["LogItemType"].Value = "Magix.Core.UserCreated";
                node["Header"].Value = "Username: " + Username;
                node["Message"].Value = "New user was created ...";

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.Log",
                    node);
            }
            base.Save();
        }

        /**
         * Level3: Overridden to make sure we log this
         */
        public override void Delete()
        {
            Node node = new Node();
            node["LogItemType"].Value = "Magix.Core.UserDeleted";
            node["Header"].Value = "Username: " + Username;
            node["Message"].Value = "Existing user was deleted ...";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.Log",
                node);

            base.Delete();
        }

        /*
         * Level3: Helper for creating unique cookie values, in case you've got more than one website at the same
         * URL
         */
        private static string GetUniqueAppString()
        {
            return HttpContext.Current.Request.ApplicationPath.Trim('/');
        }

        /**
         * Level3: Will return or change the currently logged in user object. Notice that Users are in Magix
         * stored in cookies on the harddiscs of the browsers that clients visits your website with.
         * These cookies are currently being stored as non-persistive, and HttpOnly, but will make 
         * sure that as long as user doesn't close his browser, he will become automatically logged in 
         * again upon visiting the site, if he had visited before without closing his browser
         */
        public static UserBase Current
        {
            get
            {
                if (HttpContext.Current.Session["Magix.Brix.Components.ActiveTypes.Users.User.Current"] == null)
                {
                    HttpCookie userCookie =
                        HttpContext.Current.Request.Cookies[
                            GetUniqueAppString() +
                            "Magix.Brix.Components.ActiveTypes.Users.User.Current"];
                    if (userCookie == null)
                        return null;
                    string[] entities = userCookie.Value.Split('|');
                    if (entities.Length < 2)
                        return null;
                    string cookieUsername = entities[0];
                    string cookiePassword = entities[1];

                    UserBase user =
                        UserBase.SelectFirst(
                            Criteria.Eq("Username", cookieUsername));
                    if (user == null || string.IsNullOrEmpty(user.Password))
                        return null;

                    StringBuilder passwordHashBuffer = new StringBuilder();
                    MD5 md5 = MD5.Create();
                    byte[] passwordBuffer = Encoding.ASCII.GetBytes(user.Password);
                    byte[] hash = md5.ComputeHash(passwordBuffer);
                    foreach (byte idxByte in hash)
                    {
                        passwordHashBuffer.Append(idxByte);
                    }
                    string passwordHashed = passwordHashBuffer.ToString();
                    if (passwordHashed == cookiePassword)
                    {
                        HttpContext.Current.Session["Magix.Brix.Components.ActiveTypes.Users.User.Current"] = user.ID;
                    }
                }
                return UserBase.SelectByID(
                    (int)HttpContext.Current.Session[
                        "Magix.Brix.Components.ActiveTypes.Users.User.Current"]);
            }
            set
            {
                if (value == null)
                {
                    HttpContext.Current.Request.Cookies.Remove(
                        GetUniqueAppString() +
                        "Magix.Brix.Components.ActiveTypes.Users.User.Current");
                    HttpCookie cookie = new HttpCookie(
                        GetUniqueAppString() + 
                        "Magix.Brix.Components.ActiveTypes.Users.User.Current");
                    cookie.Value = "mumboJumbo|mumboJumbo";
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    HttpContext.Current.Session.Remove("Magix.Brix.Components.ActiveTypes.Users.User.Current");
                }
                else
                {
                    StringBuilder passwordHashBuffer = new StringBuilder();
                    MD5 md5 = MD5.Create();
                    byte[] passwordBuffer = Encoding.ASCII.GetBytes(value.Password);
                    byte[] hash = md5.ComputeHash(passwordBuffer);
                    foreach (byte idxByte in hash)
                    {
                        passwordHashBuffer.Append(idxByte);
                    }
                    string passwordHashed = passwordHashBuffer.ToString();
                    HttpCookie cookie = new HttpCookie(
                        GetUniqueAppString() + 
                        "Magix.Brix.Components.ActiveTypes.Users.User.Current");
                    cookie.Value = value.Username + "|" + passwordHashed;
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    HttpContext.Current.Session["Magix.Brix.Components.ActiveTypes.Users.User.Current"] = value.ID;
                }
            }
        }

        /**
         * Level3: Returns true if user belongs to a role with the given name
         * [case-in-sensitive search]
         */
        public bool InRole(string roleName)
        {
            return Roles.Exists(
                delegate(Role idx)
                {
                    return idx.Name.ToLowerInvariant() == roleName.ToLowerInvariant();
                });
        }
    }
}
