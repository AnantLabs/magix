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
    /**
     * Level2: Implements logic for sending email using SMTP through the .Net classes
     */
    [ActiveController]
    public class Email_Controller : ActiveController
    {
        /**
         * Level2: Will send an Email, using the SMPT settings from web.config, with the given Header and Body
         * to the list in the EmailAddresses parameter from the AdminEmail and AdminEmailFrom parameters
         */
        [ActiveEvent(Name = "Magix.Core.SendEmail", Async = true)]
        protected void Magix_Core_SendEmail(object sender, ActiveEventArgs e)
        {
            try
            {
                string header = e.Params["Header"].Get<string>();
                string body = e.Params["Body"].Get<string>();
                string adminEmail = e.Params["AdminEmail"].Get<string>();
                string adminEmailFrom = e.Params["AdminEmailFrom"].Get<string>();

                // Building email
                MailMessage msg = CreateEmail(e, header, body, adminEmail, adminEmailFrom);

                // Sending email ...
                SmtpClient smtp = new SmtpClient();
                smtp.Send(msg);
            }
            catch (Exception err)
            {
                Node n = new Node();
                n["Message"].Value = 
                    "Something went wrong while trying to send email, message from server was; " + 
                    err.Message;

                // Even though we're on another thread, and the main thread highly likely has
                // ended and returned the response, we can actually 'delay' the message box
                // and have this work ... :)
                n["Delayed"].Value = true;

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    n);
            }
        }

        /*
         * Helper for above ...
         */
        private static MailMessage CreateEmail(ActiveEventArgs e, string header, string body, string adminEmail, string adminEmailFrom)
        {
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
            return msg;
        }
    }
}
