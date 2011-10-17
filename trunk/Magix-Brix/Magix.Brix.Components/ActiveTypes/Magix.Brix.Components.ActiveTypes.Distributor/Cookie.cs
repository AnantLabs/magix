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
     * Level2: 
     */
    public class Cookie : ActiveType<Cookie>
    {
        /**
         */
        [ActiveField]
        public string Domain { get; set; }

        /**
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         */
        [ActiveField]
        public string Value { get; set; }

        /**
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         */
        public override void Save()
        {
            if (ID == 0)
            {
                Created = DateTime.Now;

                Node node = new Node();

                node["LogItemType"].Value = "Magix.Core.RemotingCookieSaved";
                node["Header"].Value = "Cookie Saved for Domain: " + Domain;
                node["ObjectID"].Value = -1;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.Log",
                    node);
            }
            base.Save();
        }
    }
}
