/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveControllers.Editor
{
    [ActiveController]
    public class RichEditController : ActiveController
    {
        [ActiveEvent(Name = "RichEditor.Form.Launch")]
        protected void LaunchRichEditor(object sender, ActiveEventArgs e)
        {
            Node node = e.Params;
            if (node == null)
            {
                node = new Node();
                node["Text"].Value = @"
<p>Hello world ...!</p>
";
                node["Container"].Value = "content1";
            }
            if (node["Container"].Get<string>("content1") != "child")
                ActiveEvents.Instance.RaiseClearControls(
                    node["Container"].Get<string>("content1"));
            LoadModule(
                "Magix.Brix.Components.ActiveModules.Editor.RichEdit",
                node["Container"].Get<string>("content1"),
                node);
        }
    }
}
