/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Components.ActiveTypes.Logging;

namespace Magix.Brix.Components.ActiveControllers.Logger
{
    [ActiveController]
    public class LoggerController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.Log")]
        protected void Magix_Core_Log(object sender, ActiveEventArgs e)
        {
            DateTime when = DateTime.Now;
            string logItemType = e.Params["LogItemType"].Get<string>("Generic-Type");
            string header = e.Params["Header"].Get<string>();
            string msg = e.Params["Message"].Get<string>();
            if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(msg))
            {
                throw new ArgumentException("Both Header and Message minimum needs to be set when Logging ...");
            }
            int objectID = e.Params["ObjectID"].Get<int>(-1);
            int parentID = e.Params["ParentID"].Get<int>(-1);
            string stackTrace = e.Params["StackTrace"].Get<string>();
            string ip = (HttpContext.Current.Handler as Page).Request.UserHostAddress;
            UserBase user = UserBase.Current;

            LogItem l = new LogItem();
            l.When = when;
            l.LogItemType = logItemType;
            l.Header = header;
            l.Message = msg;
            l.ObjectID = objectID;
            l.ParentID = parentID;
            l.StackTrace = stackTrace;
            l.IPAddress = ip;
            l.User = user;
            l.Save();
        }
    }
}
