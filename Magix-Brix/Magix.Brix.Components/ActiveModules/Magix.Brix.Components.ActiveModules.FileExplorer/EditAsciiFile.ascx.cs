/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using System.IO;

namespace Magix.Brix.Components.ActiveModules.FileExplorer
{
    [ActiveModule]
    public class EditAsciiFile : UserControl, IModule
    {
        protected TextArea txt;

        void IModule.InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    // Loading file ...
                    using (TextReader reader = File.OpenText(DataSource["File"].Get<string>()))
                    {
                        txt.Text = reader.ReadToEnd().Replace("\t", "   ");
                    }

                    // Focusing our textarea ...
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(txt, true))
                        .Render();
                };
        }

        protected void save_Click(object sender, EventArgs e)
        {
            using (TextWriter writer = File.CreateText(DataSource["File"].Get<string>()))
            {
                writer.Write(txt.Text);
            }

            // Feedback to user ...
            Node n = new Node();
            n["Message"].Value = "File was successfully saved ...";
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Magix.Core.ShowMessage",
                n);
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}

