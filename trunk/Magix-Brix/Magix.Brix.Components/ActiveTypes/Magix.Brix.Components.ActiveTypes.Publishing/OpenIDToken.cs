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
    /**
     * Encapsulates an OpenID Token. Meaning an OpenID Claim. Magix has support for serving
     * as both an OpenID Relying Party and an OpenID Provider. This class serializes the
     * OpenID Claims for users when using Magix as an OpenID Relying Party, and associating
     * these claims with specific users. This means you can associate an OpenID claim to your
     * user and then use the OpenID Token to log into Magix later
     */
    [ActiveType]
    public class OpenIDToken : ActiveType<OpenIDToken>
    {
        /**
         * OpenID Claim or Token. E.g.; myGoogleUsername.blogspot.com
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Which user the claim belongs to, if any
         */
        [ActiveField(BelongsTo = true)]
        public User User { get; set; }

        /**
         * Overridden to make sure all Claims are unique within Magix
         */
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
