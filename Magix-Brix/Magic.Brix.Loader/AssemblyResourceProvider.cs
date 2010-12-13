/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Web.Hosting;
using System.Web.Caching;
using System.Collections;

/**
 * Namespace containing all the helpers for dynamically loading UserControls 
 * from Embedded Resources.
 */
namespace Magix.Brix.Loader
{
    /**
     * Helper class to make it possible to load controls (and more importantly) UserControls
     * which are embedded as resources in DLLs.
     */
    public class AssemblyResourceProvider : VirtualPathProvider
    {
        // Returns true if the path to the control is a Module.
        private static bool IsAppResourcePath(string virtualPath)
        {
            string absolutePath = VirtualPathUtility.ToAppRelative(virtualPath);

            // Notice a Virtual Path might be either a path containing Magix.Brix.Module (in which case
            // it's a DLL in the bin folder) or be an absolute path in addition to containing
            // a name of a DLL residing on disc (in which case it's a DLL in some other parts of 
            // the file system)
            // And since we want to make it possible to both load everything in the bin older in addition
            // to files in another physical directory, we must check for this...
            return absolutePath.Contains("/Magix.Brix.Module/") || 
                (absolutePath.Contains(":") && absolutePath.ToLower().Contains(".dll"));
        }

        // Overridden from base class to verify that file exists
        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) || base.FileExists(virtualPath));
        }

        // Overridden to return a file, either from base class or from our other helper
        // class; AssemblyResourceVirtualFile
        public override VirtualFile GetFile(string virtualPath)
        {
            return IsAppResourcePath(virtualPath) ? 
                new AssemblyResourceVirtualFile(virtualPath) : 
                base.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath, 
            IEnumerable virtualPathDependencies, 
            DateTime utcStart)
        {
            return IsAppResourcePath(virtualPath) ? 
                null : 
                base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }
}


