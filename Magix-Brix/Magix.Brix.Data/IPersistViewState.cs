/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

namespace Magix.Brix.Data
{
    public interface IPersistViewState
    {
        void Save(string sessionId, string pageUrl, string content);
        string Load(string sessionId, string pageUrl);
    }
}
