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
         * Level2: Will check to see if this is a WebService call, if not, it 
         * will forward the event to the default impl.
         */
        [ActiveEvent(Name = "Magix.Core.PostHTTPRequest")]
        protected void Magix_Core_PostHTTPRequest(object sender, ActiveEventArgs e)
        {
            string eventName = Page.Request["event"];
            if (!string.IsNullOrEmpty(eventName))
            {
                Node node = null;

                if (!string.IsNullOrEmpty(Page.Request.Params["params"]))
                    node = Node.FromJSONString(HttpUtility.UrlDecode(Page.Request.Params["params"]));

                eventName = HttpUtility.UrlDecode(eventName);

                RaiseEvent(
                    eventName,
                    node);

                try
                {
                    Page.Response.Clear();
                    Page.Response.Write("return:");
                    Page.Response.End(); // throws ...
                }
                catch (ThreadAbortException)
                {
                    ; // DO NOTHING ... ['by design' from MSFT ...]
                }
            }
            else
            {
                RaiseEvent(
                    "Magix.Core.InitialLoading-Passover",
                    e.Params);
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

            if (!urlEndPoint.StartsWith("http"))
                urlEndPoint = "http://" + urlEndPoint;

            Node node = null;
            if (e.Params.Contains("Node"))
                node = e.Params["Node"];

            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentException(
                    "You can't raise an event on another server without stating which event you'd like to raise");

            HttpWebRequest req = WebRequest.Create(urlEndPoint) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            InitializeCookies(urlEndPoint, req);
            using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write("event=" + HttpUtility.UrlEncode(eventName));
                if (node != null)
                    writer.Write("&params=" + HttpUtility.UrlEncode(node.ToJSONString()));
            }
            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                string val = reader.ReadToEnd();
                if (!val.StartsWith("return:"))
                    throw new HttpException("Something went wrong when connecting to " + urlEndPoint + " Server responded with: " + val);

                if (val.Length > 7)
                    e.Params["RetVal"].Value = Node.FromJSONString(val.Substring(7));
            }
        }

        private void InitializeCookies(string urlEndPoint, HttpWebRequest req)
        {
            if (urlEndPoint.Contains("/"))
                urlEndPoint = urlEndPoint.Substring(0, urlEndPoint.IndexOf("/"));

            foreach (Cookie idx in Cookie.Select(Criteria.Eq("Domain", urlEndPoint)))
            {
            }
        }
    }
}
