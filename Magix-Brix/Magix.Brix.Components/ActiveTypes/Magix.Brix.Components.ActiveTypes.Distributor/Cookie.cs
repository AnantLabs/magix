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
     * Level2: Encapsulates a Cookie for a specific Domain. Used to be capable of 
     * transfering and storing state from one server to another when for instance
     * remoting events are being raised
     */
    [ActiveType]
    public class Cookie : ActiveType<Cookie>
    {
        /**
         * Level2: The Application Path the Cookie was created for, e.g. http://magixilluminate.com/
         * Must end with a /. Magix gives you additional granularity using this class, than the 
         * HTTP Cookie parts of the standard gives you by making you capable of separating 
         * different cookies according to different paths with a server. This is ofc ourse easy to 
         * override, and set back to its original logic. However, especially since Magix are for 
         * all practical concerns a single page web app framework [more or less]. This makes sense 
         * for Magix as a default behavior. Notice that there are no changes to the standard or 
         * Magix' usage of this standard, only the granularity of how Magix internally saves and
         * retrieves cookies for specific applications within the same domain. Change the Setting called
         * 'Magix.Core.MultiplAppsPerDomain' to reset this such that Magix will send its 
         * cookies according to how 'it's supposed to do it' according to the HTTP standard. Notice how
         * this makes sense for Magix, since by using Magix you can basically 100% transparently
         * have hundreds of hundreds of virtually overridden events, penetrating the network at all times, 
         * going back and forth between hundreds of applications, which again might call back into 
         * the original server, though to a different app path
         */
        [ActiveField]
        public string Domain { get; set; }

        /**
         * Level2: The name of the cookie
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Level2: The actual value of the cookie. Anything that can be described as a string
         * can in theory be stored here. 
         */
        [ActiveField]
        public string Value { get; set; }

        /**
         * Level2: Not really relevant for us, since we're storing cookies on 'application level' 
         * anyway, but we must be able to reproduce the Cookie exactly as it came back from the 
         * end-point, and hence need to store this value somehow
         */
        [ActiveField]
        public string ActualDomain { get; set; }

        /**
         * Level2: When the cookie expires. After this date, it will no longer 
         * be attached, but deleted
         */
        [ActiveField]
        public DateTime Expires { get; set; }

        /**
         * Level2: When the cookie was created
         */
        [ActiveField]
        public DateTime Created { get; set; }

        public override void Save()
        {
            if (ID == 0)
            {
                Created = DateTime.Now;

                Node node = new Node();

                if (Expires <= DateTime.Now.AddHours(1))
                    Expires = DateTime.Now.AddHours(1);

                node["LogItemType"].Value = "Magix.Core.RemotingCookieSaved";
                node["Header"].Value = "Cookie Saved for Domain [Application Actually]: " + Domain;
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
