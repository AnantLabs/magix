/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

namespace Magix.Brix.Data
{
    public interface IPersistViewState
    {
        void Save(string sessionId, string pageUrl, string content);
        string Load(string sessionId, string pageUrl);
    }
}
