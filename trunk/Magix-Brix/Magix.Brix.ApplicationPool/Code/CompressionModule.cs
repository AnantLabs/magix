/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.IO.Compression;

namespace Magix.Brix.ApplicationPool
{
    /**
     * Level3: Helper class for implementing GZIP-Compression of resources and pages. PS! Thanx 
     * and credits goes to Mads Kristiansen for sharing this on his blog. Google 'CompressionModule 
     * Mads Kristiansen' if more background information is needed about this piece of code
     */
    public sealed class CompressionModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += new EventHandler(context_PostReleaseRequestState);
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        private const string GZIP = "gzip";
        private const string DEFLATE = "deflate";

        void context_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if (app.Context.CurrentHandler is System.Web.UI.Page && app.Request.RequestType == "GET")
            {
                if (IsEncodingAccepted(DEFLATE))
                {
                    app.Response.Filter = new DeflateStream(app.Response.Filter, CompressionMode.Compress);
                    SetEncoding(DEFLATE);
                }
                else if (IsEncodingAccepted(GZIP))
                {
                    app.Response.Filter = new GZipStream(app.Response.Filter, CompressionMode.Compress);
                    SetEncoding(GZIP);
                }
            }
        }

        private static bool IsEncodingAccepted(string encoding)
        {
            HttpContext context = HttpContext.Current;
            return context.Request.Headers["Accept-encoding"] != null &&
                context.Request.Headers["Accept-encoding"].Contains(encoding);
        }

        private static void SetEncoding(string encoding)
        {
            HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if (app.Request.Path.Contains("WebResource.axd"))
            {
                SetCachingHeaders(app);

                if (IsBrowserSupported() &&
                    app.Context.Request.QueryString["c"] == null &&
                    (IsEncodingAccepted(DEFLATE) || IsEncodingAccepted(GZIP)))
                    app.CompleteRequest();
            }
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
            if (!IsBrowserSupported() || (!IsEncodingAccepted(DEFLATE) && !IsEncodingAccepted(GZIP)))
                return;

            HttpApplication app = (HttpApplication)sender;
            string key = app.Request.QueryString.ToString();

            if (app.Request.Path.Contains("WebResource.axd") &&
                app.Context.Request.QueryString["c"] == null)
            {
                if (app.Application[key] == null)
                {
                    AddCompressedBytesToCache(app, key);
                }

                SetEncoding((string)app.Application[key + "enc"]);
                app.Context.Response.ContentType = "text/javascript";
                app.Context.Response.BinaryWrite((byte[])app.Application[key]);
            }
        }

        private static void SetCachingHeaders(HttpApplication app)
        {
            string etag = "\"" + app.Context.Request.QueryString.ToString().GetHashCode().ToString() + "\"";
            string incomingEtag = app.Request.Headers["If-None-Match"];

            app.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
            app.Response.Cache.SetExpires(DateTime.Now.AddDays(30));
            app.Response.Cache.SetCacheability(HttpCacheability.Public);
            app.Response.Cache.SetLastModified(DateTime.Now.AddDays(-30));
            app.Response.Cache.SetETag(etag);

            if (String.Compare(incomingEtag, etag) == 0)
            {
                app.Response.StatusCode = (int)HttpStatusCode.NotModified;
                app.Response.End();
            }
        }

        private static bool IsBrowserSupported()
        {
            // Because of bug in Internet Explorer 6
            HttpContext context = HttpContext.Current;
            return !(context.Request.UserAgent != null && context.Request.UserAgent.Contains("MSIE 6"));
        }

        private static void AddCompressedBytesToCache(HttpApplication app, string key)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(app.Context.Request.Url.OriginalString + "&c=1");
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Stream responseStream = response.GetResponseStream();

                using (MemoryStream ms = CompressResponse(responseStream, app, key))
                {
                    app.Application.Add(key, ms.ToArray());
                }
            }
        }

        private static MemoryStream CompressResponse(Stream responseStream, HttpApplication app, string key)
        {
            MemoryStream dataStream = new MemoryStream();
            StreamCopy(responseStream, dataStream);
            responseStream.Dispose();

            byte[] buffer = dataStream.ToArray();
            dataStream.Dispose();

            MemoryStream ms = new MemoryStream();
            Stream compress = null;

            if (IsEncodingAccepted(DEFLATE))
            {
                compress = new DeflateStream(ms, CompressionMode.Compress);
                app.Application.Add(key + "enc", DEFLATE);
            }
            else if (IsEncodingAccepted(GZIP))
            {
                compress = new GZipStream(ms, CompressionMode.Compress);
                app.Application.Add(key + "enc", DEFLATE);
            }

            compress.Write(buffer, 0, buffer.Length);
            compress.Dispose();
            return ms;
        }

        private static void StreamCopy(Stream input, Stream output)
        {
            byte[] buffer = new byte[2048];
            int read;
            do
            {
                read = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, read);
            } while (read > 0);
        }
    }
}