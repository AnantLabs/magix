﻿/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;

namespace Magix.Brix.Publishing.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ModuleSettingAttribute : Attribute
    {
        public string ModuleEditorName = "";

        public string ModuleEditorEventName = "";

        public string DefaultValue = "Default";
    }
}
