/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.IO;
using System.Web;
using System.Threading;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.Distributor;
using Magix.Brix.Components.ActiveTypes;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.Distributor
{
    /**
     * Level2: Helper class for serializing and transmitting back and forth 
     * between other servers
     */
    [ActiveController]
    public class Distributor_Controller : ActiveController
    {
        /**
         * Level2: Sink for Dashboard Plugin
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["EventRequests"].Value = true;
            e.Params["Type"]["Properties"]["EventRequests"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["EventRequests"]["Header"].Value = "Requests";
            e.Params["Type"]["Properties"]["EventRequests"]["ClickLabelEvent"].Value = "Magix.Publishing.ViewServerRequests";
            e.Params["Object"]["Properties"]["EventRequests"].Value = 
                Access.Event.CountWhere(Criteria.Eq("Authorized", false), Criteria.Eq("Ignored", false));
        }

        /**
         * Level2: Shows the Open and Pending Server Requests you have
         */
        [ActiveEvent(Name = "Magix.Publishing.ViewServerRequests")]
        protected void Magix_Publishing_ViewServerRequests(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Access.Event).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 11;
            node["Last"].Value = true;

            node["WhiteListColumns"]["UrlReferrer"].Value = true;
            node["WhiteListColumns"]["UrlReferrer"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["EventName"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Authorized"].Value = true;
            node["WhiteListColumns"]["Authorized"]["ForcedWidth"].Value = 3;

            node["NoIdColumn"].Value = true;
            node["IsDelete"].Value = false;
            node["CreateEventName"].Value = "Magix.Publishing.CreateAccessEvent";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Criteria"]["C2"]["Name"].Value = "Eq";
            node["Criteria"]["C2"]["Prop"].Value = "Ignored";
            node["Criteria"]["C2"]["Value"].Value = false;

            node["Criteria"]["C2"]["Name"].Value = "Eq";
            node["Criteria"]["C2"]["Prop"].Value = "Authorized";
            node["Criteria"]["C2"]["Value"].Value = false;

            node["Type"]["Properties"]["UrlReferrer"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["UrlReferrer"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["EventName"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Authorized"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Authorized"]["NoFilter"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);

            node = new Node();

            node["Caption"].Value = "Server Requests";

            RaiseEvent(
                "Magix.Core.SetFormCaption",
                node);
        }

        /**
         * Level2: Will check to see if this is a WebService call, if not, it 
         * will forward the event to the default impl.
         */
        [ActiveEvent(Name = "Magix.Core.PostHTTPRequest")]
        protected void Magix_Core_PostHTTPRequest(object sender, ActiveEventArgs e)
        {
            HttpCookie cookie = Page.Request.Cookies["Magix.Brix.Distributor.SecretAuthenticator"];

            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                cookie = new HttpCookie("Magix.Brix.Distributor.SecretAuthenticator");
                string id = Guid.NewGuid().ToString();

                using (Transaction tr = Adapter.Instance.BeginTransaction())
                {
                    Access a = new Access();
                    a.SecretKey = id;
                    a.IP = Page.Request.UserHostAddress;
                    if (Page.Request.UrlReferrer != null)
                        a.UrlReferrer = Page.Request.UrlReferrer.ToString();

                    Access.Event e2 = new Access.Event();
                    e2.Authorized = false;
                    e2.EventName = Page.Request["event"];
                    a.Events.Add(e2);
                    a.Save();

                    tr.Commit();
                }

                cookie.Value = id;
                cookie.Expires = DateTime.Now.AddYears(3);
                Page.Response.Clear();
                Page.Response.Cookies.Add(cookie);
                Page.Response.Write("no-access");
                try
                {
                    Page.Response.End(); // throws ...
                }
                catch (ThreadAbortException)
                {
                    ; // DO NOTHING ... ['by design' from MSFT ...]
                }
                return;
            }
            else
            {
                FireEvent(cookie.Value);
            }
        }

        private void FireEvent(string id)
        {
            Access a = Access.SelectFirst(Criteria.Eq("SecretKey", id));
            if(a == null)
            {
                try
                {
                    Page.Response.Clear();
                    Page.Response.Write("no-access");
                    Page.Response.End(); // throws ...
                }
                catch (ThreadAbortException)
                {
                    return; // DO NOTHING ... ['by design' from MSFT ...]
                }
            }
            string eventName = Page.Request["event"];

            if (a.Events.Exists(
                delegate(Access.Event idx)
                {
                    return idx.EventName == eventName && idx.Authorized;
                }))
            {
                Node node = null;

                if (!string.IsNullOrEmpty(Page.Request.Params["params"]))
                    node = Node.FromJSONString(HttpUtility.UrlDecode(Page.Request.Params["params"]));

                eventName = HttpUtility.UrlDecode(eventName);

                string returnValue = "";

                try
                {
                    Node l = new Node();

                    l["LogItemType"].Value = "Magix.Core.TransmitEventReceived";
                    l["Header"].Value = "EventName: " + eventName;
                    l["Body"].Value =
                        "End Point: " +
                        a.UrlReferrer +
                        ", params: " +
                        (node == null ? "" : node.ToJSONString());
                    l["ObjectID"].Value = -1;

                    RaiseEvent(
                        "Magix.Core.Log-HARDLINK",
                        l);

                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        RaiseEvent(
                            eventName,
                            node);

                        tr.Commit();
                    }

                    returnValue = "return:" + node.ToJSONString();
                }
                catch (Exception err)
                {
                    returnValue = "error:" + err.GetBaseException().Message;
                }
                try
                {
                    Page.Response.Clear();
                    Page.Response.Write(returnValue);
                    Page.Response.End(); // throws ...
                }
                catch (ThreadAbortException)
                {
                    ; // DO NOTHING ... ['by design' from MSFT ...]
                }
            }
            else
            {
                if (a.Events.Exists(
                    delegate(Access.Event idx)
                    {
                        return idx.EventName == eventName;
                    }))
                {
                    try
                    {
                        Page.Response.Clear();
                        Page.Response.Write("no-access");
                        Page.Response.End(); // throws ...
                    }
                    catch (ThreadAbortException)
                    {
                        ; // DO NOTHING ... ['by design' from MSFT ...]
                    }
                }
                else
                {
                    // Create a new request to admin of server to access resource ...
                    Access.Event e2 = new Access.Event();
                    e2.Authorized = false;
                    e2.EventName = eventName;
                    a.Events.Add(e2);
                    a.Save();
                }
            }
        }

        /**
         * Level2: Will raise an event at the externally given 'UrlEndPoint', which 
         * should be an at the very least an HTTP end point which can take as the 
         * input the WSDL in the WebService in this same project, one way or the other.
         * 'EventName' is the name of the event to raise externally, 'Node' will be
         * transmitted as its parameters
         */
        [ActiveEvent(Name = "Magix.Core.TransmitEventToExternalServer")]
        protected void Magix_Core_TransmitEventToExternalServer(object sender, ActiveEventArgs e)
        {
            string eventName = e.Params["EventName"].Get<string>();
            string urlEndPoint = e.Params["UrlEndPoint"].Get<string>();

            urlEndPoint = urlEndPoint.Trim().Trim('/') + "/";

            Node node = null;
            if (e.Params.Contains("Node"))
                node = e.Params["Node"];

            Node l = new Node();

            l["LogItemType"].Value = "Magix.Core.TransmitEventRaised";
            l["Header"].Value = "EventName: " + eventName;
            l["Body"].Value = 
                "End Point: " + 
                urlEndPoint +
                ", params: " +
                (node == null ? "" : node.ToJSONString());
            l["ObjectID"].Value = -1;

            RaiseEvent(
                "Magix.Core.Log-HARDLINK",
                l);

            // There can be only one ... ;)
            // We're updating cookies, which are globally used, by potentially multiple users remember ...
            lock (typeof(Distributor_Controller))
            {
                if (string.IsNullOrEmpty(urlEndPoint))
                    throw new ArgumentException("You must submit a URL to raise your remove event towards");

                string cookieDomain = urlEndPoint;

                if (!Settings.Instance.Get("Magix.Core.MultiplAppsPerDomain", true))
                {
                    cookieDomain = cookieDomain.Split(':')[1].Trim('/');
                    cookieDomain = cookieDomain.Substring(0, cookieDomain.IndexOf("/"));
                }

                if (!urlEndPoint.StartsWith("http"))
                    urlEndPoint = "http://" + urlEndPoint;

                if (string.IsNullOrEmpty(eventName))
                    throw new ArgumentException(
                        "You can't raise an event on another server without stating which event you'd like to raise");

                System.Net.HttpWebRequest req = System.Net.WebRequest.Create(urlEndPoint) as System.Net.HttpWebRequest;
                req.Method = "POST";
                req.Referer = GetApplicationBaseUrl();
                req.ContentType = "application/x-www-form-urlencoded";

                InitializeCookies(cookieDomain, req);

                using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
                {
                    writer.Write("event=" + HttpUtility.UrlEncode(eventName));
                    if (node != null)
                        writer.Write("&params=" + HttpUtility.UrlEncode(node.ToJSONString()));
                }
                using (System.Net.HttpWebResponse resp = req.GetResponse() as System.Net.HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300)
                        {
                            using (Transaction tr = Adapter.Instance.BeginTransaction())
                            {
                                foreach (System.Net.Cookie idx in resp.Cookies)
                                {
                                    foreach (Cookie idx2 in Cookie.Select(
                                        Criteria.Eq("Domain", urlEndPoint),
                                        Criteria.Eq("Name", idx.Name)))
                                    {
                                        idx2.Delete();
                                    }

                                    Cookie c = new Cookie();
                                    if (Settings.Instance.Get("Magix.Core.MultiplAppsPerDomain", true))
                                        c.Domain = urlEndPoint;
                                    else
                                        c.Domain = idx.Domain;
                                    c.Name = idx.Name;
                                    c.Expires = idx.Expires;
                                    c.ActualDomain = idx.Domain;
                                    c.Value = idx.Value;
                                    c.Save();
                                }

                                tr.Commit();
                            }
                            string val = reader.ReadToEnd();
                            if (!val.StartsWith("return:"))
                                throw new HttpException(
                                    "Something went wrong when connecting to '" +
                                    urlEndPoint +
                                    "'. Server responded with: " + val);

                            if (val.Length > 7)
                                e.Params["RetVal"].Value = Node.FromJSONString(val.Substring(7));
                        }
                    }
                }
            }
        }

        private void InitializeCookies(string urlEndPoint, System.Net.HttpWebRequest req)
        {
            if (req.CookieContainer == null)
            {
                req.CookieContainer = new System.Net.CookieContainer();
            }

            foreach (Cookie idx in Cookie.Select(Criteria.Eq("Domain", urlEndPoint)))
            {
                if (idx.Expires <= DateTime.Now)
                    idx.Delete();
                else
                    req.CookieContainer.Add(new System.Net.Cookie(idx.Name, idx.Value, "/", idx.ActualDomain));
            }
        }
    }
}
