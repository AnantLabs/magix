/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.UX.Helpers
{
    public static class StringHelper
    {
        public static string ConditionalAdd(
            string original, 
            string ifEmpty, 
            string ifNotEmpty, 
            string atEnd)
        {
            if (string.IsNullOrEmpty(original))
                return ifEmpty + atEnd;
            if (string.IsNullOrEmpty(atEnd))
                return original + ifEmpty;
            return original + ifNotEmpty + atEnd;
        }
    }
}
