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
    /**
     * Class encapsulating log items in Magix. Magix has automated logging components which
     * will log certain aspects about your system and its behavior. These log items are stored
     * in your database through using this active type
     */
    [ActiveType]
    public class LogItem : ActiveType<LogItem>
    {
        /**
         * Automatically generated date of when log item was created
         */
        [ActiveField]
        public DateTime When { get; set; }

        /**
         * A friendly 'type string' meant to easily allow filtering different
         * categories of log items according to how the end user wants it
         */
        [ActiveField]
        public string LogItemType { get; set; }

        /**
         * Friendly [and SHORT!] description of log item
         */
        [ActiveField]
        public string Header { get; set; }

        /**
         * More thorough description if feasable
         */
        [ActiveField]
        public string Message { get; set; }

        /**
         * And integer value pointing to the ID of the 'relevant object', whatever that is
         */
        [ActiveField]
        public int ObjectID { get; set; }

        /**
         * Similar to above. Just 'another container' for similar type of data
         */
        [ActiveField]
        public int ParentID { get; set; }

        /**
         * StackTrace, if we're in an exception
         */
        [ActiveField]
        public string StackTrace { get; set; }

        /**
         * IP address of user triggering log item. Saved for security reasons
         */
        [ActiveField]
        public string IPAddress { get; set; }

        /**
         * Logged in user triggering log item
         */
        [ActiveField(IsOwner = false)]
        public UserBase User { get; set; }

        /**
         * ID of cookie on disc for user, if any. Every request to Magix
         * will automatically create a 'tracking cookie', upon from which the user
         * will be automatically associated with, till he either uses a different client,
         * or empties his cache somehow. This is the 'ID' of that tracking cookie. Which
         * makes it possible to 'identify' users, even though they're not logged in
         */
        [ActiveField]
        public string UserID { get; set; }
    }
}
