/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using System.Globalization;

namespace Magix.Brix.Components.ActiveModules.FileExplorer
{
    [ActiveModule]
    public class Explorer : UserControl, IModule
    {
        protected Panel pnl;
        protected Panel prop;
        protected Label header;
        protected Label extension;
        protected Label size;
        protected LinkButton imageLink;
        protected Label imageSize;
        protected Label imageWarning;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    prop.Visible = false;
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBindFiles();
        }

        private void DataBindFiles()
        {
            DataBindExplorer();
        }

        private void DataBindExplorer()
        {
            int start = 0;
            int end = 18;
            int idxNo = 0;
            if (DataSource["Folder"].Get<string>().Length
                > DataSource["RootAccessFolder"].Get<string>().Length)
            {
                string name = "&nbsp;";
                Label btn = new Label();
                btn.Text = name;
                Panel p = new Panel();
                p.Info = "../";
                p.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        Panel pp = sender as Panel;
                        string folderName = pp.Info;
                        DataSource["FolderToOpen"].Value = folderName;
                        DataSource["Directories"].UnTie();
                        DataSource["Files"].UnTie();
                        RaiseSafeEvent(
                            "FileExplorer.GetFilesFromFolder",
                            DataSource);
                        DataSource["FolderToOpen"].UnTie();
                        ReDataBind();
                    };
                p.CssClass = "folderUpIcon";
                p.Controls.Add(btn);
                pnl.Controls.Add(p);
                idxNo += 1;
            }
            if (DataSource.Contains("Directories"))
            {
                foreach (Node idx in DataSource["Directories"])
                {
                    if (idxNo >= end)
                        break;
                    if (idxNo >= start)
                    {
                        string name = idx["Name"].Get<string>();
                        Label btn = new Label();
                        btn.Text = name;
                        Panel p = new Panel();
                        p.Info = name;
                        p.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                Panel pp = sender as Panel;
                                string folderName = pp.Info;
                                DataSource["FolderToOpen"].Value = folderName;
                                DataSource["Directories"].UnTie();
                                DataSource["Files"].UnTie();
                                RaiseSafeEvent(
                                    "FileExplorer.GetFilesFromFolder",
                                    DataSource);
                                ReDataBind();
                            };
                        p.CssClass = "folderIcon";
                        if (((idxNo + 1) - start) % 6 == 0)
                            p.CssClass += " lastImage";
                        if (DataSource.Contains("SelectedFile") &&
                            DataSource["SelectedFile"].Get<string>().ToLower() == name.ToLower())
                            p.CssClass += " selected";
                        p.ToolTip = name.Substring(name.LastIndexOf("\\") + 1);
                        p.Controls.Add(btn);
                        pnl.Controls.Add(p);
                    }
                    idxNo += 1;
                }
            }
            if (DataSource.Contains("Files"))
            {
                foreach (Node idx in DataSource["Files"])
                {
                    if (idxNo >= end)
                        break;
                    if (idxNo >= start)
                    {
                        if (idx.Contains("IsImage") && idx["IsImage"].Get<bool>())
                        {
                            string name = idx["Name"].Get<string>();
                            Image btn = new Image();
                            btn.AlternateText = name;
                            btn.ImageUrl =
                                DataSource["Folder"].Get<string>() +
                                name;

                            Panel p = new Panel();
                            if (idx.Contains("Wide") && idx["Wide"].Get<bool>())
                            {
                                p.CssClass = "imageIcon wide";
                            }
                            else
                                p.CssClass = "imageIcon";
                            if (((idxNo + 1) - start) % 6 == 0)
                                p.CssClass += " lastImage";
                            if (DataSource.Contains("SelectedFile") &&
                                DataSource["SelectedFile"].Get<string>().ToLower() == name.ToLower())
                                p.CssClass += " selected";
                            p.Click +=
                                delegate(object sender, EventArgs e)
                                {
                                    Panel pp = sender as Panel;
                                    string folderName = pp.Info;
                                    DataSource["File"].Value = folderName;
                                    RaiseSafeEvent(
                                        "FileExplorer.FileSelectedInExplorer",
                                        DataSource);
                                    UpdateSelectedFile();
                                };
                            p.Info = name;
                            p.ToolTip = name + " - click for more options/info ...";
                            p.Controls.Add(btn);
                            pnl.Controls.Add(p);
                        }
                    }
                    idxNo += 1;
                }
            }
        }

        private void UpdateSelectedFile()
        {
            new EffectFadeIn(prop, 500)
                .Render();
            prop.Visible = true;
            prop.Style[Styles.display] = "none";
            header.Text = "Name: " + DataSource["File"]["Name"].Get<string>();
            extension.Text = "Extension: " + DataSource["File"]["Extension"].Get<string>();
            size.Text = "Size: " +
                (DataSource["File"]["Size"].Get<long>() / 1024)
                .ToString("###.###.###.###", CultureInfo.InvariantCulture) +
                "KB";
            if (DataSource["File"].Contains("IsImage") && DataSource["File"]["IsImage"].Get<bool>())
            {
                imageLink.Visible = true;
                imageSize.Visible = true;
                imageLink.Text = "Click to view full size...";
                imageLink.Info = 
                    DataSource["Folder"].Get<string>() + 
                    DataSource["File"]["FullName"].Get<string>();

                int width = DataSource["File"]["ImageWidth"].Get<int>();
                int height = DataSource["File"]["ImageHeight"].Get<int>();

                string imageSizeText =
                    width +
                    "px - " +
                    height +
                    "px";
                imageSize.Text = imageSizeText;

                if ((width + 10) % 40 != 0 || height % 18 != 0 || width > 950)
                {
                    imageWarning.Visible = true;
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectHighlight(imageWarning, 500),
                            new EffectTimeout(500),
                            new EffectHighlight(imageWarning, 500),
                            new EffectTimeout(500),
                            new EffectHighlight(imageWarning, 500))
                        .Render();
                    imageWarning.Text = string.Format(
                        @"Images should be 30, 70, 110, 150 etc till 950, and some multiplication 
of 18 in height. Your image seems to be {0}x{1}, which means it's {2} pixels too wide
and {3} pixels to tall to show up beautifully in your design.",
                        width,
                        height,
                        width < 30 ? -(30 - width) : ((width + 10) % 40),
                        height < 18 ? -(18 - height) : height % 18);
                }
                else
                {
                    imageWarning.Visible = false;
                }
            }
            else
            {
                imageLink.Visible = false;
                imageSize.Visible = false;
            }
        }

        protected void imageLink_Click(object sender, EventArgs e)
        {
            LinkButton b = sender as LinkButton;
            string file = b.Info;
            Node node = new Node();
            node["ForcedSize"]["width"].Value = DataSource["File"]["ImageWidth"].Get<int>() + 80;
            node["ForcedSize"]["height"].Value = DataSource["File"]["ImageHeight"].Get<int>() + 81;
            node["ImageUrl"].Value = file;
            node["SetFocus"].Value = true;
            node["styles"]["border"].Value = "solid 1px Blue";
            node["AlternateText"].Value = "Preview of image in full size...";
            node["Caption"].Value = 
                "Full size of: " + 
                DataSource["Folder"].Get<string>() +
                DataSource["File"].Get<string>();
            ActiveEvents.Instance.RaiseLoadControl(
                "Magix.Brix.Components.ActiveModules.CommonModules.ImageModule",
                "child",
                node);
        }

        private void ReDataBind()
        {
            prop.Visible = false;
            pnl.Controls.Clear();
            DataBindExplorer();
            pnl.ReRender();
            new EffectHighlight(pnl, 500)
                .Render();
            if (Parent.Parent.Parent is Window)
            {
                (Parent.Parent.Parent as Window).Caption = 
                    DataSource["Caption"].Get<string>();
            }
            else
            {
                Node node = new Node();
                node["Caption"].Value = DataSource["Caption"].Get<string>();
                RaiseSafeEvent(
                    "Magix.Core.SetFormCaption",
                    node);
            }
        }

        protected bool RaiseSafeEvent(string eventName, Node node)
        {
            try
            {
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    eventName,
                    node);
                return true;
            }
            catch (Exception err)
            {
                Node n = new Node();
                while (err.InnerException != null)
                    err = err.InnerException;
                n["Message"].Value = err.Message;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    n);
                return false;
            }
        }

        private Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set { ViewState["DataSource"] = value; }
        }
    }
}

