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
    class Person : ActiveType<Person>
    {
        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public DateTime Born { get; set; }

        protected void foo()
        {
            Person per = Person.SelectFirst(Criteria.Eq("Name", "Thomas Hansen"));

            foreach (Person idxx in Person.Select())
            {
                idxx.Name = "Something else";
                idxx.Save();
            }
        }
    }

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
