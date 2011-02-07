/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.TalkBack;
using Magix.Brix.Components.ActiveTypes.Users;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveControllers.TalkBack
{
    [ActiveController]
    public class TalkBackController : ActiveController
    {
        [ActiveEvent(Name = "TalkBack.LaunchForum")]
        protected void Talkback_LaunchForum(object sender, ActiveEventArgs e)
        {
            Node header = new Node();
            header["Caption"].Value = "TalkBack ...";
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.SetFormCaption",
                header);

            Node node = e.Params;
            if (node == null)
                node = new Node();
            foreach (Posting idx in Posting.Select(
                Criteria.Range(1, 10, "When", false)))
            {
                node["Posts"]["P" + idx.ID]["Header"].Value = idx.Header;
                node["Posts"]["P" + idx.ID]["Content"].Value = idx.Content;
                node["Posts"]["P" + idx.ID]["User"].Value = idx.User == null ? "[null]" : idx.User.Username;
                node["Posts"]["P" + idx.ID]["Date"].Value = idx.When;
            }
            LoadModule(
                "Magix.Brix.Components.ActiveModules.TalkBack.Forum",
                "content3",
                node);
        }
    }
}













