/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using Magix.Brix.Types;

namespace Magix.Brix.Loader
{
    // TODO: Remove this interface and exchange with logic in ActiveModule base class
    /**
     * Level3: Optional interface you can mark your Modules with. If you do your Modules will be called the
     * first time they load through the InitialLoading with whatever object you choose to RaiseYour 
     * events with.
     */
    public interface IModule
    {
        /**
         * Level3: Will be called when the Module is initially loaded with the initializationObject
         * parameter you pass into your LoadModule - if any.
         */
        void InitialLoading(Node node);
    }
}
