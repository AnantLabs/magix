/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
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
