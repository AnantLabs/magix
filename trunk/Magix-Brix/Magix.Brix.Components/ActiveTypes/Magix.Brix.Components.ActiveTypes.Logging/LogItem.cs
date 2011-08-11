/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Text;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveTypes.Logging
{
    [ActiveType]
    public class LogItem : ActiveType<LogItem>
    {
        [ActiveField]
        public DateTime When { get; set; }

        [ActiveField]
        public string LogItemType { get; set; }

        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Message { get; set; }

        [ActiveField]
        public int ObjectID { get; set; }

        [ActiveField]
        public int ParentID { get; set; }

        [ActiveField]
        public string StackTrace { get; set; }

        [ActiveField]
        public string IPAddress { get; set; }

        [ActiveField(IsOwner = false)]
        public UserBase User { get; set; }

        [ActiveField]
        public string UserID { get; set; }
    }
}
