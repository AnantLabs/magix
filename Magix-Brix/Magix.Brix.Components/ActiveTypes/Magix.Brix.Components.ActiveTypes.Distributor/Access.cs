/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.Distributor
{
    /**
     * Level2: Encapsulates access right for one single other server in regards to whether or not it
     * can legally raise events, and which events it has requested to raise
     */
    [ActiveType]
    public class Access : ActiveType<Access>
    {
        /**
         * Level2: Encapsualtes one single event one other server can legally raise on yours
         */
        [ActiveType]
        public class Event : ActiveType<Event>
        {
            /**
             * Level2: The name of the event the other side can legally raise on yours
             */
            [ActiveField]
            public string EventName { get; set; }

            /**
             * Level2: IP address of requesting party
             */
            [ActiveField]
            public string IP { get; set; }

            /**
             * Level2: Whether or not the other side is Authorized to raise that specific event or not
             */
            [ActiveField]
            public bool Authorized { get; set; }

            /**
             * Level2: Whether or not the other side is ignored in regards to this specific event or not
             */
            [ActiveField]
            public bool Ignored { get; set; }

            /**
             * Level2: When this event was first attempted invoked
             */
            [ActiveField]
            public DateTime Created { get; set; }

            /**
             * Level2: Not to be trusted, but really just a friendly name for the app. Magix 
             * will automatically use the base URL for where it's installed as the referrer
             */
            [ActiveField]
            public string UrlReferrer { get; set; }

            public override void Save()
            {
                if (ID == 0)
                {
                    Created = DateTime.Now;
                }
                base.Save();
            }
        }

        public Access()
        {
            Events = new LazyList<Event>();
        }

        /**
         * Level2: Secret Key which basically is the communication point up to the end point server
         * to supply. Keep this one secret, since it basically authenticates you to that other 
         * specific server, and hence encapsulates your access rights on the other party's behalf.
         * Loosing this key, might not only be seen as stupid, but also directly irresponsible on
         * behalf of the server to which this key authenticates you towards
         */
        [ActiveField]
        public string SecretKey { get; set; }

        /**
         * Level2: Not to be trusted, but really just a friendly name for the app. Magix 
         * will automatically use the base URL for where it's installed as the referrer
         */
        [ActiveField]
        public string UrlReferrer { get; set; }

        /**
         * Level2: IP address of requesting party
         */
        [ActiveField]
        public string IP { get; set; }

        /**
         * Level2: When request was initially initiated
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * Level2: If true, will be ignored every time other side tries to initiate contact
         */
        [ActiveField]
        public bool Ignored { get; set; }

        /**
         * Level2: List of events the other party can legally raise on our server
         */
        [ActiveField]
        public LazyList<Event> Events { get; set; }

        public override void Save()
        {
            if (ID == 0)
            {
                Created = DateTime.Now;
            }
            if (Ignored)
            {
                foreach (Event idx in Events)
                {
                    Ignored = true;
                }
            }
            foreach (Event idx in Events)
            {
                if (string.IsNullOrEmpty(idx.UrlReferrer) && !string.IsNullOrEmpty(UrlReferrer))
                {
                    idx.UrlReferrer = UrlReferrer;
                }
            }
            foreach (Event idx in Events)
            {
                if (string.IsNullOrEmpty(idx.IP) && !string.IsNullOrEmpty(IP))
                {
                    idx.IP = IP;
                }
            }
            base.Save();
        }
    }
}
