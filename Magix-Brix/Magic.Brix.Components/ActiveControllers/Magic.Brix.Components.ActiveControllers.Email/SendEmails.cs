/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System.Net;
using System.Net.Mail;
using Magic.Brix.Types;
using Magic.Brix.Loader;
using Magic.Brix.Components.ActiveTypes;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Magic.Brix.Components.ActiveControllers.Email
{
    [ActiveController]
    public class SendEmails : ActiveController
    {
        [ActiveEvent(Name = "SendEmailLocally", Async = true)]
        protected void SendEmail(object sender, ActiveEventArgs e)
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
    }
}
