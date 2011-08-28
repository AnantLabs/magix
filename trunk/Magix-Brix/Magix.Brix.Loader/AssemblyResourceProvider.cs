/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Web.Hosting;
using System.Web.Caching;
using System.Collections;

namespace Magix.Brix.Loader
{
    /**
     * Level4: Helper class to make it possible to load controls (and more importantly) UserControls
     * which are embedded as resources in DLLs. Not intended for direct usage, but will be in 
     * 'the background' and making sure you can load ActiveModules as resources from your DLLs
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

        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) || base.FileExists(virtualPath));
        }

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
