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
            string logItemType = e.Params["LogItemType"].Get<string>();
            string header = e.Params["Header"].Get<string>();
            string msg = e.Params["Message"].Get<string>();
            int objectID = e.Params["ObjectID"].Get<int>(-1);
            string stackTrace = e.Params["StackTrace"].Get<string>();
            string ip = (HttpContext.Current.Handler as Page).Request.UserHostAddress;
            UserBase user = UserBase.Current;

            LogItem l = new LogItem();
            l.When = when;
            l.LogItemType = logItemType;
            l.Header = header;
            l.Message = msg;
            l.ObjectID = objectID;
            l.StackTrace = stackTrace;
            l.IPAddress = ip;
            l.User = user;
            l.Save();
        }
    }
}
