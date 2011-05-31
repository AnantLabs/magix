/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
        public string StackTrace { get; set; }

        [ActiveField]
        public string IPAddress { get; set; }

        [ActiveField(IsOwner = false)]
        public UserBase User { get; set; }
    }
}
