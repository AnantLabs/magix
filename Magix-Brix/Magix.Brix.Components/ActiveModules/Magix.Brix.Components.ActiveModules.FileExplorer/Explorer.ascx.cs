/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.IO;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using System.Globalization;
using Magix.UX;
using Magix.UX.Widgets.Core;

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
        protected Label imageSize;
        protected Label imageWarning;
        protected Label fullUrl;
        protected Button previous;
        protected Button next;
        protected InPlaceEdit name;
        protected Image preview;
        protected Button delete;
        protected System.Web.UI.WebControls.FileUpload file;
        protected TextBox fileReal;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    DataSource = node;
                    prop.Visible = false;
                    delete.Enabled = false;
                    Start = 0;
                    End = 18;
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

        private int Start
        {
            get { return (int)ViewState["Start"]; }
            set { ViewState["Start"] = value; }
        }

        private int End
        {
            get { return (int)ViewState["End"]; }
            set { ViewState["End"] = value; }
        }

        private void DataBindExplorer()
        {
            int start = Start;
            int end = End;
            previous.Visible = DataSource["Directories"].Count + DataSource["Files"].Count > 17;
            next.Visible = DataSource["Directories"].Count + DataSource["Files"].Count > 17;
            previous.Enabled = Start > 0;
            next.Enabled = End < (DataSource["Directories"].Count + DataSource["Files"].Count + (Start == 0 ? 1 : 0));
            int idxNo = 0;
            if (start == 0)
            {
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
                            Start = 0;
                            End = 18;
                            ReDataBind();
                        };
                    p.CssClass = "folderUpIcon";
                    p.Controls.Add(btn);
                    pnl.Controls.Add(p);
                    idxNo += 1;
                }
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
                            p.Style[Styles.position] = "relative";
                            Label l = new Label();
                            l.Style[Styles.position] = "absolute";
                            l.Style[Styles.bottom] = "0";
                            l.Style[Styles.left] = "0";
                            l.Style[Styles.backgroundColor] = "rgba(0, 0, 0, 0.2)";
                            l.Style[Styles.color] = "#fff";
                            l.CssClass = "small";
                            p.Controls.Add(l);
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
                                    pp.CssClass += " viewing";
                                    Panel old = Selector.SelectFirst<Panel>(pp.Parent,
                                        delegate(System.Web.UI.Control idx3)
                                        {
                                            return (idx3 is BaseWebControl) &&
                                                (idx3 as BaseWebControl).Info == SelectedPanelID;
                                        });
                                    if (old != null)
                                        old.CssClass = old.CssClass.Replace(" viewing", "");
                                    SelectedPanelID = pp.Info;
                                    string folderName = pp.Info;
                                    DataSource["File"].Value = folderName;
                                    RaiseSafeEvent(
                                        "FileExplorer.FileSelectedInExplorer",
                                        DataSource);
                                    UpdateSelectedFile();
                                    new EffectFadeIn(prop, 500)
                                        .Render();
                                    prop.Visible = true;
                                    prop.Style[Styles.display] = "none";
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

        protected void preview_Click(object sender, EventArgs e)
        {
            OpenFullPreviewOfImage(
                DataSource["Folder"].Get<string>() + 
                DataSource["File"]["FullName"].Get<string>());
        }

        protected void delete_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["Folder"].Value = DataSource["Folder"].Value;
            node["File"].Value = DataSource["File"]["FullName"].Value;
            DataSource["Directories"].UnTie();
            DataSource["Files"].UnTie();
            RaiseSafeEvent(
                "FileExplorer.DeleteFile",
                node);
            DataSource["FolderToOpen"].Value = "";
            RaiseSafeEvent(
                "FileExplorer.GetFilesFromFolder",
                DataSource);
            ReDataBind();
        }

        protected void submitFile_Click(object sender, EventArgs e)
        {
            if (!file.HasFile)
            {
                Node node = new Node();
                node["Message"].Value = "You have to specify a file ...! ";
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "Magix.Core.ShowMessage",
                    node);
            }
            else
            {
                fileReal.Text = "";
                string fileName = file.FileName;
                string webServerApp = Server.MapPath("~/");
                file.SaveAs(
                    webServerApp +
                    DataSource["Folder"].Get<string>().Replace("/", "\\") +
                    fileName);

                DataSource["FolderToOpen"].Value = "";
                DataSource["File"].UnTie();
                DataSource["Files"].UnTie();
                RaiseSafeEvent(
                    "FileExplorer.GetFilesFromFolder",
                    DataSource);
                ReDataBind();

                SelectedPanelID = fileName;
                DataSource["File"].Value = fileName;
                RaiseSafeEvent(
                    "FileExplorer.FileSelectedInExplorer",
                    DataSource);
                UpdateSelectedFile();
                new EffectFadeIn(prop, 500)
                    .Render();
                prop.Visible = true;
                prop.Style[Styles.display] = "none";

                Panel pl = Selector.SelectFirst<Panel>(this,
                    delegate(Control idx)
                    {
                        return (idx is BaseWebControl) &&
                            (idx as BaseWebControl).Info == SelectedPanelID;
                    });
                if (pl != null)
                    pl.CssClass += " viewing";
            }
        }

        private void UpdateSelectedFile()
        {
            delete.Enabled = true;
            header.Text = "Name: " + DataSource["File"]["Name"].Get<string>();
            extension.Text = "Extension: " + DataSource["File"]["Extension"].Get<string>();
            name.Text = DataSource["File"]["Name"].Get<string>();
            name.Info = DataSource["File"]["FullName"].Get<string>();
            preview.AlternateText = DataSource["File"]["Name"].Get<string>();
            preview.ImageUrl = 
                DataSource["Folder"].Get<string>() + 
                DataSource["File"]["FullName"].Get<string>();
            preview.CssClass =
                DataSource["Files"][DataSource["File"]["FullName"].Get<string>()]
                .Contains("Wide") &&
                DataSource["Files"][DataSource["File"]["FullName"].Get<string>()]["Wide"]
                .Get<bool>() ? 
                    "span-4 preview wide" : 
                    "span-4 preview";
            size.Text = "Size: " +
                (((double)DataSource["File"]["Size"].Get<long>()) / 1024D)
                .ToString("###,###,###,##0.0", CultureInfo.InvariantCulture) +
                "KB";
            fullUrl.Text = " Full URL: " +
                DataSource["Folder"].Get<string>() +
                DataSource["File"]["FullName"].Get<string>();
            if (DataSource["File"].Contains("IsImage") && DataSource["File"]["IsImage"].Get<bool>())
            {
                imageSize.Visible = true;
                int width = DataSource["File"]["ImageWidth"].Get<int>();
                int height = DataSource["File"]["ImageHeight"].Get<int>();
                int optimalWidth = width - (width < 30 ? -(30 - width) : ((width + 10) % 40));
                int optimalHeight = height - (height < 18 ? -(18 - height) : height % 18);

                if ((width + 10) % 40 != 0 || height % 18 != 0 || width > 950)
                {
                    string imageSizeText =
                        width +
                        "px - " +
                        height +
                        "px - optimal " +
                        optimalWidth +
                        "x" +
                        optimalHeight;
                    imageSize.Text = imageSizeText;
                    imageWarning.Visible = true;
                    imageWarning.Style[Styles.display] = "none";
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectRollDown(imageWarning, 500),
                            new EffectHighlight(imageWarning, 500),
                            new EffectTimeout(500),
                            new EffectHighlight(imageWarning, 500),
                            new EffectTimeout(500),
                            new EffectHighlight(imageWarning, 500))
                        .Render();
                    imageWarning.Text = string.Format(
@"Images should be 30, 70, 110, 150, etc wide, and some multiplication 
of 18 in height. Your image seems to be {0}x{1}. If you scale it down to
{2}x{3} pixels, it will show up more beautifully in the design of your website 
flow ...",
                        width,
                        height,
                        optimalWidth,
                        optimalHeight);
                }
                else
                {
                    string imageSizeText =
                        width +
                        "px - " +
                        height +
                        "px - grids " + 
                        (width + 10) / 40 +
                        "x" + 
                        height / 18;
                    imageSize.Text = imageSizeText;
                    imageWarning.Visible = false;
                }
            }
            else
            {
                imageSize.Visible = false;
            }
        }

        protected void name_TextChanged(object sender, EventArgs e)
        {
            string newName = name.Text;
            string oldName = (sender as InPlaceEdit).Info;
            Node node = DataSource;
            node["Directories"].UnTie();
            node["Files"].UnTie();
            node["NewName"].Value = newName;
            node["OldName"].Value = oldName;
            RaiseSafeEvent(
                "FileExplorer.ChangeFileName",
                node);
            node["NewName"].UnTie();
            node["OldName"].UnTie();
            SelectedPanelID = newName + oldName.Substring(oldName.LastIndexOf("."));
            ReDataBind();
            Panel pl = Selector.SelectFirst<Panel>(this,
                delegate(Control idx)
                {
                    return (idx is BaseWebControl) &&
                        (idx as BaseWebControl).Info == SelectedPanelID;
                });
            if (pl != null)
                pl.CssClass += " viewing";
            UpdateSelectedFile();
            prop.Visible = true;
        }

        private void OpenFullPreviewOfImage(string file)
        {
            Node node = new Node();
            node["ForcedSize"]["width"].Value = DataSource["File"]["ImageWidth"].Get<int>() + 80;
            node["ForcedSize"]["height"].Value = DataSource["File"]["ImageHeight"].Get<int>() + 90;
            node["ImageUrl"].Value = file;
            node["SetFocus"].Value = true;
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

        protected void previous_Click(object sender, EventArgs e)
        {
            int delta = End - Start;
            if (Start == 17)
                delta -= 1;
            Start -= delta;
            End -= delta;
            pnl.Controls.Clear();
            DataBindExplorer();
            pnl.ReRender();
            prop.Visible = false;
            delete.Enabled = false;
        }

        protected void next_Click(object sender, EventArgs e)
        {
            int delta = End - Start;
            if (Start == 0)
                delta -= 1;
            Start += delta;
            End += delta;
            pnl.Controls.Clear();
            DataBindExplorer();
            pnl.ReRender();
            prop.Visible = false;
            delete.Enabled = false;
        }

        private void ReDataBind()
        {
            prop.Visible = false;
            delete.Enabled = false;
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

        private string SelectedPanelID
        {
            get { return ViewState["SelectedPanelID"] as string; }
            set { ViewState["SelectedPanelID"] = value; }
        }
    }
}

