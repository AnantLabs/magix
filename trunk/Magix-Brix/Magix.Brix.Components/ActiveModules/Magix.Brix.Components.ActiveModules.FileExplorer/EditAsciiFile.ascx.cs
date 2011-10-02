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
    // TODO: Implement Controller logic here for saving. Looks ugly, and not possible to override
    /**
     * Level2: Kind of like Magix' version of 'Notepad'. Allows for editing of textfiles or text fragments, 
     * and allow for saving them. Pass in either 'File' [pointing to an absolute file name] or 'Text' which
     * should be textual fragments. If 'NoSave' is True, it won't allow for saving of the document
     */
    [ActiveModule]
    public class EditAsciiFile : ActiveModule
    {
        protected TextArea txt;
        protected Button save;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    DataSource = node;

                    // Loading file ...
                    if (DataSource.Contains("File"))
                    {
                        using (TextReader reader = File.OpenText(DataSource["File"].Get<string>()))
                        {
                            txt.Text = reader.ReadToEnd().Replace("\t", "   ").Replace("\r\n", "\n");
                        }
                    }
                    else
                    {
                        txt.Text = DataSource["Text"].Get<string>().Replace("\t", "   ").Replace("\r\n", "\n");
                        save.Visible = false;
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

            RaiseSafeEvent(
                "Magix.Core.ShowMessage",
                n);
        }
    }
}
