/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System.Net;
using System.Net.Mail;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace Magix.Brix.Components.ActiveControllers.Email
{
    [ActiveController]
    public class SendEmailController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.SendEmailLocally", Async = true)]
        protected void Magix_Core_SendEmailLocally(object sender, ActiveEventArgs e)
        {
            try
            {
                string header = e.Params["Header"].Get<string>();
                string body = e.Params["Body"].Get<string>();
                string adminEmail = e.Params["AdminEmail"].Get<string>();
                string adminEmailFrom = e.Params["AdminEmailFrom"].Get<string>();

                MailMessage msg = new MailMessage();
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Subject = header;
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                    Regex.Replace(body, @"<(.|\n)*?>", string.Empty),
                    null,
                    "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                    body,
                    null,
                    "text/html");

                msg.AlternateViews.Add(plainView);
                msg.AlternateViews.Add(htmlView);
                foreach (Node idxEmail in e.Params["EmailAddresses"])
                {
                    msg.To.Add(new MailAddress(idxEmail.Get<string>()));
                }
                if (!string.IsNullOrEmpty(adminEmailFrom))
                {
                    string emailFrom = "\"" + adminEmailFrom + "\" <" + adminEmail + ">";
                    msg.From = new MailAddress(emailFrom);
                }
                else
                {
                    msg.From = new MailAddress(adminEmail);
                }
                SmtpClient smtp = new SmtpClient();
                smtp.Send(msg);
            }
            catch (Exception)
            {
                //Node n = new Node();
                //n["Message"].Value = "Something went wrong while trying to send email, message from server was; " + err.Message;
                //ActiveEvents.Instance.RaiseActiveEvent(
                //    this,
                //    "Magix.Core.ShowMessage",
                //    n);
                // CAN'T DO, ANOTHER THREAD ...!
            }
        }
    }
}
