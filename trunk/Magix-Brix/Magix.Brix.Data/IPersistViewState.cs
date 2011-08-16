/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

namespace Magix.Brix.Data
{
    /**
     * Level4: Interface for classes able to implement persistent viewstate support for Magix. Normally this
     * would be a service of the Application Pool, meaning the Default.aspx.cs would implement this
     * somehow
     */
    public interface IPersistViewState
    {
        /**
         * Level4: Called by Magix when it's time to save the ViewState. Normally expected to
         * dump the stuff into a DB somewhere with the key of sessionId+pageUrl or something
         */
        void Save(string sessionId, string pageUrl, string content);

        /**
         * Level4: Called by Magix when it's time to reload the ViewState. Normally expected to
         * dump the stuff out of a DB somewhere with the key of sessionId+pageUrl or something
         */
        string Load(string sessionId, string pageUrl);
    }
}
