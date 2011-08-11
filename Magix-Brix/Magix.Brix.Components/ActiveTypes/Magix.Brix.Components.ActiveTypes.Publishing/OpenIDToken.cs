/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using U = Magix.Brix.Components.ActiveTypes.Users;
using System.Collections.Generic;
using System.Web;

namespace Magix.Brix.Components.ActiveTypes.Publishing
{
    [ActiveType]
    public class OpenIDToken : ActiveType<OpenIDToken>
    {
        [ActiveField]
        public string Name { get; set; }

        [ActiveField(BelongsTo = true)]
        public User User { get; set; }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(Name) &&
                Name.Length > 0)
            {
                if (CountWhere(
                    Criteria.Eq("Name", Name),
                    Criteria.NotId(ID)) > 0)
                {
                    throw new ArgumentException(
                        @"Sorry, but that OpenID is already registered on this 
site. Try to log in using it ...");
                }
            }
            base.Save();
        }
    }
}
