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

        [ActiveEvent(Name = "Magix.Publishing.GetSettings")]
        protected void Magix_Publishing_GetSettings(object sender, ActiveEventArgs e)
        {
            e.Params["OpenID"]["Header"].Value = "OpenID";
            e.Params["OpenID"]["DefaultRoleName"]["Name"].Value = "Default Role";
            e.Params["OpenID"]["DefaultRoleName"]["SettingsValue"].Value = "Magix.Publishing.OpenID.DefaultRoleName";
        }

        [ActiveEvent(Name = "Magix.Publishing.NewUserRegisteredThroughOpenID")]
        protected void Magix_Publishing_NewUserRegisteredThroughOpenID(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                User user = new User();
                user.Username = e.Params["OpenID"].Get<string>();
                user.Password = "pass" + Guid.NewGuid().ToString();

                OpenIDToken token = new OpenIDToken();
                token.Name = e.Params["OpenID"].Get<string>();
                user.OpenIDTokens.Add(token);

                user.OpenIDTokens.Add(token);

                string defaultRole = Settings.Instance.Get("Magix.Publishing.OpenID.DefaultRoleName", "User");

                if (!string.IsNullOrEmpty(defaultRole))
                {
                    Role r = Role.SelectFirst(Criteria.Eq("Name", defaultRole));
                    if (r == null)
                        throw new ArgumentException("Your default role for OpenID users doesn't exists ...");
                    user.Roles.Add(r);
                }

                if (e.Params.Contains("BirthDate"))
                    user.BirthDate = e.Params["BirthDate"].Get<DateTime>();

                if (e.Params.Contains("Country"))
                    user.Country = e.Params["Country"].Get<string>();

                if (e.Params.Contains("Email"))
                    user.Email = e.Params["Email"].Get<string>();

                if (e.Params.Contains("FullName"))
                    user.FullName = e.Params["FullName"].Get<string>();

                if (e.Params.Contains("Gender"))
                    user.Gender = e.Params["Gender"].Get<string>();

                if (e.Params.Contains("Language"))
                    user.Language = e.Params["Language"].Get<string>();

                if (e.Params.Contains("Nickname"))
                    user.Nickname = e.Params["Nickname"].Get<string>();

                if (e.Params.Contains("PostalCode"))
                    user.Zip = e.Params["PostalCode"].Get<string>();

                if (e.Params.Contains("TimeZone"))
                    user.TimeZone = e.Params["TimeZone"].Get<string>();

                user.SaveNoVerification();

                tr.Commit();
            }
        }

        private void DoOpenProviderStuff()
        {
            OpenIdProvider provider = new OpenIdProvider();

            IRequest request = provider.GetRequest() as IRequest;

            if (request as DotNetOpenAuth.OpenId.Provider.IAuthenticationRequest != null)
            {
                // Need to store it till user is done with loggin in ...
                Page.Session["Magix.Publishing.OpenID.IAuthenticationRequest"] = request;
            }

            if (request != null && request.IsResponseReady)
            {
                Page.Session["Magix.Publishing.OpenID.IAuthenticationRequest"] = null;
                provider.SendResponse(request);
                Page.Response.End();
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

                            node["OpenID"].Value = r.ClaimedIdentifier.ToString();

                            ClaimsResponse claim = r.GetExtension<ClaimsResponse>();

                            if (claim != null)
                            {
                                if (claim.BirthDate.HasValue)
                                    node["BirthDate"].Value = claim.BirthDate.Value;

                                if (!string.IsNullOrEmpty(claim.Country))
                                    node["Country"].Value = claim.Country;

                                if (!string.IsNullOrEmpty(claim.Email))
                                    node["Email"].Value = claim.Email;

                                if (!string.IsNullOrEmpty(claim.FullName))
                                    node["FullName"].Value = claim.FullName;

                                if (claim.Gender.HasValue)
                                    node["Gender"].Value = claim.Gender.Value.ToString();

                                if (!string.IsNullOrEmpty(claim.Language))
                                    node["Language"].Value = claim.Language;

                                if (!string.IsNullOrEmpty(claim.Nickname))
                                    node["Nickname"].Value = claim.Nickname;

                                if (!string.IsNullOrEmpty(claim.PostalCode))
                                    node["PostalCode"].Value = claim.PostalCode;

                                if (!string.IsNullOrEmpty(claim.TimeZone))
                                    node["TimeZone"].Value = claim.TimeZone;
                            }

                            // New user came to site, through OpenID login ...
                            RaiseEvent(
                                "Magix.Publishing.NewUserRegisteredThroughOpenID",
                                node);

                            Page.Response.Redirect(GetApplicationBaseUrl(), true);
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

            if (string.IsNullOrEmpty(username))
            {
                Node node = new Node();
                node["Message"].Value = @"We could really need some help here, 
maybe a suggestion of what it might have been ...?";

                RaiseEvent(
                    "Magix.Core.ShowMessage",
                    node);
                return;
            }

            // We're now a 'Relying Party', meaning, we need an OpenID verification from
            // whatever provider was given ...

            using (OpenIdRelyingParty openId = new OpenIdRelyingParty())
            {
                //Identifier id = new 
                DotNetOpenAuth.OpenId.RelyingParty.IAuthenticationRequest request =
                    openId.CreateRequest(username, GetApplicationBaseUrl(), Page.Request.Url);

                ClaimsRequest claim = new ClaimsRequest();

                // Aiming for _everything_ ...
                claim.BirthDate = DemandLevel.Request;
                claim.Country = DemandLevel.Request;
                claim.Email = DemandLevel.Request;
                claim.FullName = DemandLevel.Request;
                claim.Gender = DemandLevel.Request;
                claim.Language = DemandLevel.Request;
                claim.Nickname = DemandLevel.Request;
                claim.PostalCode = DemandLevel.Request;
                claim.TimeZone = DemandLevel.Request;

                request.AddExtension(claim);

                request.RedirectToProvider();
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
