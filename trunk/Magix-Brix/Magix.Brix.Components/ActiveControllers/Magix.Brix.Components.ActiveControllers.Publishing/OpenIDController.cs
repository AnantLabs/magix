/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes.Publishing;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using System.Net;
using System.IO;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using Magix.UX;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Provider;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.Messaging;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    [ActiveController]
    public class OpenIDController : ActiveController
    {
        [ActiveEvent(Name = "Brix.Core.InitialLoading")]
        protected void Brix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
            if (!string.IsNullOrEmpty(Page.Request.Params["openID"]))
                DoOpenProviderStuff();
            if (string.IsNullOrEmpty(Page.Request.Params["openID"]))
                DoRelyingPartyStuff();
        }

        private void DoOpenProviderStuff()
        {
            OpenIdProvider provider = new OpenIdProvider();

            DotNetOpenAuth.OpenId.Provider.IAuthenticationRequest request =
                provider.GetRequest() as
                DotNetOpenAuth.OpenId.Provider.IAuthenticationRequest;

            if (request != null)
                Page.Session["Magix.Publishing.OpenID.IAuthenticationRequest"] = request;

            DotNetOpenAuth.OpenId.Provider.IRequest req = provider.GetRequest();
            if (req != null && req.IsResponseReady)
            {
                provider.SendResponse(req);
            }

            // Since this is a requet for a specific user's OpenID page [maybe]
            // We'll have to inject a couple of <link tags onto our HTML, to show
            // the way to the OpenID.server, and other properties ...

            Node node = new Node();

            node["rel"].Value = "openid.server";
            node["href"].Value = GetApplicationBaseUrl() + "?openID=" + Page.Request.Params["openID"];

            RaiseEvent(
                "Magix.Core.AddLinkInHeader",
                node);
        }

        private void DoRelyingPartyStuff()
        {
            // Checking to see if we're getting return stuff from an OpenID Provider here ...
            OpenIdRelyingParty openid = new OpenIdRelyingParty();

            IAuthenticationResponse r = openid.GetResponse();
            if (r != null)
            {
                // We're a Party ;) - [The Relying kind ...]
                // On our way back from our Provider ...
                switch (r.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        ClaimsResponse claimsResponse = r.GetExtension<ClaimsResponse>();
                        OpenIDToken token = 
                            OpenIDToken.SelectFirst(Criteria.Eq("Name", r.ClaimedIdentifier));

                        Node m = new Node();
                        m["Message"].Value = string.Format("Yup, seems like you own '{0}'", r.ClaimedIdentifier);
                        m["Delayed"].Value = true;
                        RaiseEvent(
                            "Magix.Core.ShowMessage",
                            m);

                        if (token == null)
                        {
                            User.Current = null; // Weird, I know, but for circular cases, where you use yourself as both provider and relying party ...

                            Node node = new Node();

                            node["OpenID"].Value = r.ClaimedIdentifier;

                            // New user came to site, through OpenID login ...
                            RaiseEvent(
                                "Magix.Publishing.NewUserRegisteredThroughOpenID",
                                node);

                            AjaxManager.Instance.Redirect(GetApplicationBaseUrl());
                        }
                        else
                        {
                            User.Current = token.User;

                            // User logged in, using his OpenID provider
                            RaiseEvent("Magix.Core.UserLoggedIn");
                        } break;
                    case AuthenticationStatus.Canceled:
                        break; // Silently fall through ...?
                    case AuthenticationStatus.Failed:
                        throw new ArgumentException("Failed to log you in on your chosen OpenID Provider ...");
                }
            }
        }

        // TODO: Override ...
        [ActiveEvent(Name = "Magix.Core.LogInUser")]
        protected void Magix_Core_LogInUser(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string password = e.Params["Password"].Get<string>();

            if (string.IsNullOrEmpty(password))
            {
                // Since no password was given when trying to log in user, we're assuming
                // it's an OpenID, and hence re-directing user to his OpenID provider ...

                // We're now a Relying Party ...

                LogInWithOpenIDClaim(username);
            }
        }

        private void LogInWithOpenIDClaim(string username)
        {
            username = username.Trim();

            // We're now a 'Relying Party', meaning, we need an OpenID verification from
            // whatever provider was given ...

            using (OpenIdRelyingParty openId = new OpenIdRelyingParty())
            {
                //Identifier id = new 
                DotNetOpenAuth.OpenId.RelyingParty.IAuthenticationRequest request =
                    openId.CreateRequest(username, GetApplicationBaseUrl(), Page.Request.Url);

                // Some 'slightly messy' tricking to get our URL ... ;)
                string oUrl = request.RedirectingResponse.Headers["Location"];

                // Re-directing to OpenID Provider ...
                AjaxManager.Instance.Redirect(oUrl);
            }
        }

        // TODO: Override ...
        [ActiveEvent(Name = "Magix.Core.UserLoggedIn-Override")]
        protected void Magix_Core_UserLoggedIn_Override(object sender, ActiveEventArgs e)
        {
            DotNetOpenAuth.OpenId.Provider.IAuthenticationRequest request = 
                Page.Session["Magix.Publishing.OpenID.IAuthenticationRequest"] as 
                DotNetOpenAuth.OpenId.Provider.IAuthenticationRequest;
            if(request != null)
            {
                // We're a PROVIDER ... :)

                // Resetting to make sure ...
                Page.Session["Magix.Publishing.OpenID.IAuthenticationRequest"] = null;

                request.IsAuthenticated = true;

                request.ClaimedIdentifier = 
                    GetApplicationBaseUrl() + 
                    "?openID=" + 
                    User.Current.Username;

                request.LocalIdentifier = 
                    GetApplicationBaseUrl() + 
                    "?openID=" + 
                    User.Current.Username;

                OpenIdProvider p = new OpenIdProvider();
                p.SendResponse(request); // Will re-direct back to caller and abort thread ...
            }
            else
            {
                RaiseEvent(
                    "Magix.Core.UserLoggedIn",
                    e.Params);
            }
        }
    }
}
