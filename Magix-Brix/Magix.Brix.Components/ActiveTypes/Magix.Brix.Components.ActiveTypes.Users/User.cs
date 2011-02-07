/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Globalization;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Magix.Brix.Components.ActiveTypes.Users
{
    [ActiveType]
    internal class UserSettings : ActiveType<UserSettings>
    {
        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string Value { get; set; }
    }

    /**
     * Helper class for inheriting from User to allow addition of extra fields
     * according to domain model of your app. If you need to inherit from User
     * type, to add additional fields like for instance a link to Client, Email,
     * Address etc, then you must inherit directly from this class, and not
     * from the "shorthand" User class.
     */
    [ActiveType(TableName = "docWineTasting.CoreTypes.User")]
    public class UserBase : ActiveType<UserBase>
    {
        public UserBase()
        {
            Settings = new LazyList<UserSettings>();
            Roles = new LazyList<Role>();
        }

        [ActiveField]
        public string Username { get; set; }

        [ActiveField]
        public string Password { get; set; }

        [ActiveField(IsOwner = false)]
        public LazyList<Role> Roles { get; set; }

        [ActiveField]
        internal LazyList<UserSettings> Settings { get; set; }

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

        public void RemoveSetting(string name)
        {
            Settings.RemoveAll(
                delegate(UserSettings idx)
                {
                    return idx.Name == "ReportOnlyUser";
                });
            Save();
        }

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

        public override void Save()
        {
            if (Current == null)
                throw new ApplicationException(
                    @"Saving this object requires being logged in, you are not ...!
Please login to retry operation ...");
            if (!Current.InRole("Administrator") &&
                !Current.InRole("Superuser"))
            {
                if (Current != this)
                    throw new ApplicationException(
                        @"You do not have sufficient rights to save this object");
                // BUT! You ARE allowed to save *yourself* ... ;)
                // But only as long as you don't add yourself up to any new roles ...
                // This to support for changing ones one password and such ...
            }
            if (Username == "Administrator")
                throw new Exception("That was a very bad username suggestion my friend ...");
            if (string.IsNullOrEmpty(Password))
            {
                if (ID != 0)
                    throw new ApplicationException(
                        @"Sorry, but there's no way we'd allow you to change your Password to
and empty string ...");
                Password = "password123";
            }
            if (string.IsNullOrEmpty(Username))
            {
                if (ID != 0)
                    throw new ApplicationException(
                        @"Sorry, but there's no way we'd allow you to change your Username to an 
empty string ...");
                Username = "username" + Guid.NewGuid().ToString().Substring(0, 8);
            }
            UserBase other =
                UserBase.SelectFirst(
                    Criteria.Eq("Username", Username));
            if (other != null && other != this)
            {
                throw new ApplicationException(
                    @"Sorry, but that Username is already taken by another user, 
usernames must be unique within the application ...");
            }
            base.Save();
        }

        public static UserBase Current
        {
            get
            {
                if (HttpContext.Current.Session["Magix.Brix.Components.ActiveTypes.Users.User.Current"] == null)
                {
                    HttpCookie userCookie =
                        HttpContext.Current.Request.Cookies["Magix.Brix.Components.ActiveTypes.Users.User.Current"];
                    if (userCookie == null)
                        return null;
                    string[] entities = userCookie.Value.Split('|');
                    string cookieUsername = entities[0];
                    string cookiePassword = entities[1];
                    UserBase user =
                        UserBase.SelectFirst(
                            Criteria.Eq("Username", cookieUsername));
                    if (user == null)
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
                    HttpCookie cookie = new HttpCookie("Magix.Brix.Components.ActiveTypes.Users.User.Current");
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
                    HttpCookie cookie = new HttpCookie("Magix.Brix.Components.ActiveTypes.Users.User.Current");
                    cookie.Value = value.Username + "|" + passwordHashed;
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    HttpContext.Current.Session["Magix.Brix.Components.ActiveTypes.Users.User.Current"] = value.ID;
                }
            }
        }

        public bool InRole(string roleName)
        {
            return Roles.Exists(
                delegate(Role idx)
                {
                    return idx.Name == roleName;
                });
        }
    }
}
