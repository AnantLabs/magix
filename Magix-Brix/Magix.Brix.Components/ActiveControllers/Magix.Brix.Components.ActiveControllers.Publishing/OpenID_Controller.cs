/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
using System.Security;

namespace Magix.Brix.Components.ActiveControllers.Publishing
{
    /**
     * Level2: Controller implementing the OpenID logic for both Relying Party
     * and OpenID Provider logic. Every User in Magix can use the website
     * where Magix is installed as an OpenID provider by typing in the
     * root URL of the web application and append ?openID=username
     * In addition you can associate as many OpenID tokens as you wish to your User
     * and then use these other OpenID providers to log into Magix.
     */
    [ActiveController]
    public class OpenID_Controller : ActiveController
    {
        /**
         * Level2: Overriding our default Magix Login logic here to inject our own OpenID stuff ...
         * Overrides; 'Magix.Core.LogInUser' and 'Magix.Core.UserLoggedIn' which again
         * are being used by the Magix 'core' login logic, meaning we're basically
         * doing effectively 'AOP' here by overriding existing events like these ...
         * Where the 'Aspect' here would be the OpenID 'logic'
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.LogInUser",
                "Magix.Core.LogInUser-Override");

            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.LogInUser-Passover",
                "Magix.Core.LogInUser");

            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.UserLoggedIn",
                "Magix.Core.UserLoggedIn-Override");

            Magix.Brix.Loader.ActiveEvents.Instance.CreateEventMapping(
                "Magix.Core.UserLoggedIn-Passover",
                "Magix.Core.UserLoggedIn");
        }

        /**
         * Level2: Basically just checking if this is an OpenID Request, and doing
         * either the Provider or the Relying Party thing needed ...
         */
        [ActiveEvent(Name = "Magix.Core.InitialLoading")]
        protected void Magix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
            if (!string.IsNullOrEmpty(Page.Request.Params["openID"]))
            {
                InjectOpenIDHeaderHTMLMetaElements();
                ProcessAnyOpenIDProviderRequests();
            }
            else
            {
                ProcessAnyOpenIDRelyingPartyRequests();
            }
        }

        /**
         * Helper method used to inject OpenID Meta information into the header element of the 
         * Page.
         */
        private void InjectOpenIDHeaderHTMLMetaElements()
        {
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

        /**
         * Will try to extract DotNetOpenAuth OpenID requests being passed around, 
         * and process them, if any
         */
        private void ProcessAnyOpenIDProviderRequests()
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
        }

        /**
         * Will check to see if there's any OpenID Relying Party requests currently coming
         * in, or in the pipeline, and process them if necessary.
         */
        private void ProcessAnyOpenIDRelyingPartyRequests()
        {
            // Checking to see if we're getting return stuff from an OpenID Provider here ...
            OpenIdRelyingParty openid = new OpenIdRelyingParty();

            IAuthenticationResponse r = openid.GetResponse();
            if (r != null)
            {
                OpenIDProviderVerifiedToken(r);
            }
        }

        /**
         * OpenID Token was verified, can we log in directly, 
         * or are we supposed to create a new User object. As in
         * is this OpenID Token associated with an existing User here ...?
         */
        private void OpenIDProviderVerifiedToken(IAuthenticationResponse r)
        {
            // We're a Party ;) - [The Relying kind ...]
            // On our way back from our Provider ...
            switch (r.Status)
            {
                case AuthenticationStatus.Authenticated:
                    if (!AuthenticateUserFromIncomingOpenID(r))
                    {
                        // User was Authenticated as the rightful owner of his OpenID Token
                        // but so far has not had a User assigned to hime here.
                        CreateNewUserFromOpenIDToken(r);
                    }
                    break;

                case AuthenticationStatus.Canceled:
                    // Silently fall through ...?
                    Page.Response.Redirect(GetApplicationBaseUrl(), true);
                    break;

                case AuthenticationStatus.Failed:
                    throw new SecurityException("Failed to log you in on your chosen OpenID Provider ...");
            }
        }

        /**
         * Will either log in existing user from an OpenID Token and return true, or 
         * not be able to match him with an existing user and hence return false
         */
        private bool AuthenticateUserFromIncomingOpenID(IAuthenticationResponse r)
        {
            ClaimsResponse claimsResponse = r.GetExtension<ClaimsResponse>();
            OpenIDToken token =
                OpenIDToken.SelectFirst(Criteria.Eq("Name", r.ClaimedIdentifier));

            if (token != null)
            {
                // Logging is user through his OpenID/User association ...
                LogInExistingUserFromOpenIDToken(r, token);
                return true;
            }

            // Although User authenticated through his OpenID provider, doesn't
            // mean his Authorized for anything in particular on THIS website ...
            return false;
        }

        /**
         * Will log in User from an existing OpenID Token, and show a message box back
         * to the end user to show him we had success
         */
        private void LogInExistingUserFromOpenIDToken(IAuthenticationResponse r, OpenIDToken token)
        {
            User.Current = token.User;

            ClaimsResponse claim = r.GetExtension<ClaimsResponse>();
            if (claim != null)
            {
                if (claim.BirthDate.HasValue)
                    User.Current.BirthDate = claim.BirthDate.Value;
                if (!string.IsNullOrEmpty(claim.Country))
                    User.Current.Country = claim.Country;
                if (!string.IsNullOrEmpty(claim.Email))
                    User.Current.Email = claim.Email;
                if (!string.IsNullOrEmpty(claim.FullName))
                    User.Current.FullName = claim.FullName;
                if (claim.Gender.HasValue)
                    User.Current.Gender = claim.Gender.Value.ToString();
                if (!string.IsNullOrEmpty(claim.Language))
                    User.Current.Language = claim.Language;
                if (!string.IsNullOrEmpty(claim.Nickname))
                    User.Current.Nickname = claim.Nickname;
                if (!string.IsNullOrEmpty(claim.PostalCode))
                    User.Current.Zip = claim.PostalCode;
                if (!string.IsNullOrEmpty(claim.TimeZone))
                    User.Current.TimeZone = claim.TimeZone;
            }

            Node m = new Node();

            m["Message"].Value = string.Format("Yup, seems like you own '{0}'", r.ClaimedIdentifier);
            m["Delayed"].Value = true;

            RaiseEvent(
                "Magix.Core.ShowMessage",
                m);

            // User logged in, using his OpenID provider
            RaiseEvent("Magix.Core.UserLoggedIn");
        }

        /**
         * Will raise the 'Magix.Publishing.NewUserRegisteredThroughOpenID'
         * with all the data given from the OpenID Provider. And show the 
         * end user a dialog confirming he's successfully authenticated
         */
        private void CreateNewUserFromOpenIDToken(IAuthenticationResponse r)
        {
            // Weird, I know, but for circular cases, 
            // where you use yourself as both provider and relying party ...!!

            User.Current = null;

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

            Node m = new Node();

            m["Message"].Value = string.Format("Yup, seems like you own '{0}'", r.ClaimedIdentifier);
            m["Delayed"].Value = true;

            RaiseEvent(
                "Magix.Core.ShowMessage",
                m);

            // Redirecting back to root ...
            Page.Response.Redirect(GetApplicationBaseUrl(), true);
        }

        /**
         * Level2: Creates a default User object, and ads him into the
         * default role as set through the 
         * 'Magix.Publishing.OpenID.DefaultRoleName' setting.
         * Associates the given OpenID Token with the User [obviously]
         * Will also try to extract additional information, such as Address etc
         * from the OpenID Provider ...
         */
        [ActiveEvent(Name = "Magix.Publishing.NewUserRegisteredThroughOpenID")]
        protected void Magix_Publishing_NewUserRegisteredThroughOpenID(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                User user = InitializeNewOpenIDUser(e);
                AddNewOpenIDUserToDefaultRole(user);
                ExtractOpenIDExtensionData(e.Params, user);

                user.SaveNoVerification();

                tr.Commit();
            }
        }

        #region [ -- Helper methods to create a new user from a new OpenID Token -- ]

        /*
         * Helper method to initialize user
         */
        private static User InitializeNewOpenIDUser(ActiveEventArgs e)
        {
            User user = new User();
            user.Username = e.Params["OpenID"].Get<string>();
            user.Password = "pass" + Guid.NewGuid().ToString();

            OpenIDToken token = new OpenIDToken();
            token.Name = e.Params["OpenID"].Get<string>();
            user.OpenIDTokens.Add(token);
            return user;
        }

        /*
         * Extracts OpenID Extension Data from Provider
         */
        private static void ExtractOpenIDExtensionData(Node node, User user)
        {
            if (node.Contains("BirthDate"))
                user.BirthDate = node["BirthDate"].Get<DateTime>();

            if (node.Contains("Country"))
                user.Country = node["Country"].Get<string>();

            if (node.Contains("Email"))
                user.Email = node["Email"].Get<string>();

            if (node.Contains("FullName"))
                user.FullName = node["FullName"].Get<string>();

            if (node.Contains("Gender"))
                user.Gender = node["Gender"].Get<string>();

            if (node.Contains("Language"))
                user.Language = node["Language"].Get<string>();

            if (node.Contains("Nickname"))
                user.Nickname = node["Nickname"].Get<string>();

            if (node.Contains("PostalCode"))
                user.Zip = node["PostalCode"].Get<string>();

            if (node.Contains("TimeZone"))
                user.TimeZone = node["TimeZone"].Get<string>();
        }

        /*
         * Adds user to default role 
         */
        private static void AddNewOpenIDUserToDefaultRole(User user)
        {
            // Will attempt to find out if there's a 'Default Role' for new users setup at this
            // website, and if so, add the user to this role ...
            string defaultRole = Settings.Instance.Get("Magix.Publishing.OpenID.DefaultRoleName", "User");
            if (!string.IsNullOrEmpty(defaultRole))
            {
                Role r = Role.SelectFirst(Criteria.Eq("Name", defaultRole));
                if (r == null)
                    throw new ArgumentException("Your default role for OpenID users doesn't exists ...");
                user.Roles.Add(r);
            }
        }

        #endregion

        /**
         * Level2: Overridden to check if we're in OpenID Mode or in Username/Password mode
         * Will forward to overridden method is no OpenID value is given ...
         */
        [ActiveEvent(Name = "Magix.Core.LogInUser-Override")]
        protected void Magix_Core_LogInUser_Override(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string password = e.Params["Password"].Get<string>();
            string openID = e.Params["OpenID"].Get<string>();

            if (!string.IsNullOrEmpty(openID))
            {
                // We're now a Relying Party ...

                LogInWithOpenIDClaim(openID);
            }
            else
            {
                // Forwarding to 'base class'
                RaiseEvent(
                    "Magix.Core.LogInUser-Passover",
                    e.Params);
            }
        }

        /*
         * Helper method for 'Magix.Core.LogInUser-Override' to start an OpenID
         * Relying Party session with whatever was typed into our OpenID login box
         */
        private void LogInWithOpenIDClaim(string openIDToken)
        {
            openIDToken = openIDToken.Trim();

            if (string.IsNullOrEmpty(openIDToken))
            {
                // Yohoo ...!
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
            InitiateRelyingParty(openIDToken);
        }

        /*
         * Helper method for above
         */
        private void InitiateRelyingParty(string openIDToken)
        {
            using (OpenIdRelyingParty openId = new OpenIdRelyingParty())
            {
                //Identifier id = new 
                DotNetOpenAuth.OpenId.RelyingParty.IAuthenticationRequest request =
                    openId.CreateRequest(openIDToken, GetApplicationBaseUrl(), Page.Request.Url);

                ClaimsRequest claim = new ClaimsRequest();

                // Aiming for _everything_ ...
                // But demanding only Email ...
                claim.Email = DemandLevel.Require;

                claim.BirthDate = DemandLevel.Request;
                claim.Country = DemandLevel.Request;
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

        /**
         * Level2: Overridden to check to see if we're an OpenID Provider, and if so, redirect back 
         * to Relying Party accordingly. If we're not a Provider, it will forward the event to
         * the overridden logic
         */
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
                    "Magix.Core.UserLoggedIn-Passover",
                    e.Params);
            }
        }
    }
}







// NOT currently in use, but quite a useful construct. Kept for reference purposes ...

//[ActiveEvent(Name = "Magix.Publishing.GetSettings")]
//protected void Magix_Publishing_GetSettings(object sender, ActiveEventArgs e)
//{
//    e.Params["OpenID"]["Header"].Value = "OpenID";
//    e.Params["OpenID"]["DefaultRoleName"]["Name"].Value = "Default Role";
//    e.Params["OpenID"]["DefaultRoleName"]["SettingsValue"].Value = "Magix.Publishing.OpenID.DefaultRoleName";
//}
