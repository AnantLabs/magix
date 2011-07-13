/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using System.Web;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    [ActiveModule]
    public class EditSpecificTemplate : ActiveModule, IModule
    {
        protected Panel parts;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindWebParts();
        }

        private void DataBindWebParts()
        {
            int current = DataSource["ID"].Get<int>();
            foreach (Node idx in DataSource["Templates"])
            {
                if (idx.Name == "t-" + current)
                {
                    foreach (Node idxC in idx["Containers"])
                    {
                        int width = idxC["Width"].Get<int>();
                        int height = idxC["Height"].Get<int>();
                        bool last = idxC["Last"].Get<bool>();
                        string name = idxC["Name"].Get<string>() ?? "[Unknown]";
                        int id = idxC["ID"].Get<int>();
                        int padding = idxC["Padding"].Get<int>();
                        int push = idxC["Push"].Get<int>();
                        int top = idxC["Top"].Get<int>();
                        int bottomMargin = idxC["BottomMargin"].Get<int>();
                        string moduleName = idxC["ModuleName"].Get<string>();
                        string cssClass = idxC["CssClass"].Get<string>();

                        // Creating Window ...
                        Window w = new Window();
                        w.CssClass += " web-part";
                        SetCommonWebPartProperties(
                            width, 
                            height, 
                            last, 
                            name, 
                            id, 
                            padding, 
                            push, 
                            top, 
                            bottomMargin, 
                            moduleName, 
                            w);

                        CreateActionButtons(id, w);

                        CreateModuleSelectListForWebPart(id, moduleName, w);

                        CreateNameInPlaceEdit(name, id, w);

                        CreateChangeCssClassInPlaceEdit(id, cssClass, w);

                        CheckBox ch = new CheckBox();
                        ch.Checked = last;
                        ch.CheckedChanged +=
                            delegate
                            {
                                Node nx = new Node();

                                nx["ID"].Value = id;
                                nx["Action"].Value = "ChangeLast";
                                nx["Value"].Value = ch.Checked;

                                RaiseSafeEvent(
                                    "Magix.Publishing.ChangeTemplateProperty",
                                    nx);

                                ReDataBind();
                            };

                        Label lbl = new Label();
                        lbl.Text = "Last";
                        lbl.Load
                            +=
                            delegate
                            {
                                lbl.For = ch.ClientID;
                            };
                        lbl.Tag = "label";
                        
                        Panel pnl = new Panel();
                        pnl.CssClass = "span-3";
                        pnl.Controls.Add(ch);
                        pnl.Controls.Add(lbl);

                        w.Content.Controls.Add(pnl);

                        parts.Controls.Add(w);
                    }
                }
            }
        }

        private void CreateChangeCssClassInPlaceEdit(int id, string cssClass, Window w)
        {
            InPlaceTextAreaEdit tx = new InPlaceTextAreaEdit();
            tx.Text = cssClass;
            tx.CssClass += " span-5  editer";
            tx.Info = id.ToString();
            tx.TextChanged +=
                delegate(object sender, EventArgs e)
                {
                    InPlaceTextAreaEdit t = sender as InPlaceTextAreaEdit;

                    Node node = new Node();

                    node["ID"].Value = int.Parse(t.Info);
                    node["Action"].Value = "ChangeCssClass";
                    node["Value"].Value = t.Text;

                    RaiseSafeEvent(
                        "Magix.Publishing.ChangeTemplateProperty",
                        node);
                };
            tx.ToolTip = "Css Class of WebPart";
            w.Content.Controls.Add(tx);
        }

        private void CreateNameInPlaceEdit(string name, int id, Window w)
        {
            InPlaceTextAreaEdit nameI = new InPlaceTextAreaEdit();
            nameI.Text = name;
            nameI.Info = id.ToString();
            nameI.CssClass += " span-5 editer";
            nameI.ToolTip = "Name of WebPart";
            nameI.TextChanged +=
                delegate(object sender, EventArgs e)
                {
                    InPlaceTextAreaEdit xed = sender as InPlaceTextAreaEdit;
                    int idPart = int.Parse(xed.Info);

                    Node node = new Node();

                    node["ID"].Value = idPart;
                    node["Action"].Value = "ChangeName";
                    node["Action"]["Value"].Value = xed.Text;

                    RaiseSafeEvent(
                        "Magix.Publishing.ChangeTemplateProperty",
                        node);

                    DataSource["Templates"].UnTie();
                    DataSource["AllModules"].UnTie();

                    RaiseEvent(
                        "Magix.Publishing.GetTemplates",
                        DataSource);

                    parts.Controls.Clear();
                    DataBindWebParts();
                    parts.ReRender();
                };
            w.Content.Controls.Add(nameI);
        }

        private void CreateActionButtons(int id, Window w)
        {
            LinkButton incWidth = new LinkButton();
            incWidth.Text = "&nbsp;";
            incWidth.ToolTip = "Increase Width";
            incWidth.CssClass = "magix-publishing-increase-width";
            incWidth.Info = id.ToString();
            incWidth.Click += incWidth_Click;
            w.Content.Controls.Add(incWidth);

            LinkButton decWidth = new LinkButton();
            decWidth.Text = "&nbsp;";
            decWidth.ToolTip = "Decrease Width";
            decWidth.CssClass = "magix-publishing-decrease-width";
            decWidth.Info = id.ToString();
            decWidth.Click += decWidth_Click;
            w.Content.Controls.Add(decWidth);

            LinkButton incHeight = new LinkButton();
            incHeight.Text = "&nbsp;";
            incHeight.ToolTip = "Increase Height";
            incHeight.CssClass = "magix-publishing-increase-height";
            incHeight.Info = id.ToString();
            incHeight.Click += incHeight_Click;
            w.Content.Controls.Add(incHeight);

            LinkButton decHeight = new LinkButton();
            decHeight.Text = "&nbsp;";
            decHeight.ToolTip = "Decrease Height";
            decHeight.CssClass = "magix-publishing-decrease-height";
            decHeight.Info = id.ToString();
            decHeight.Click += decHeight_Click;
            w.Content.Controls.Add(decHeight);

            LinkButton incDown = new LinkButton();
            incDown.Text = "&nbsp;";
            incDown.ToolTip = "Increase Top Margin";
            incDown.CssClass = "magix-publishing-increase-down";
            incDown.Info = id.ToString();
            incDown.Click += incDown_Click;
            w.Content.Controls.Add(incDown);

            LinkButton decDown = new LinkButton();
            decDown.Text = "&nbsp;";
            decDown.ToolTip = "Decrease Top Margin";
            decDown.CssClass = "magix-publishing-decrease-down";
            decDown.Info = id.ToString();
            decDown.Click += decDown_Click;
            w.Content.Controls.Add(decDown);

            LinkButton incBottom = new LinkButton();
            incBottom.Text = "&nbsp;";
            incBottom.ToolTip = "Increase Bottom Margin";
            incBottom.CssClass = "magix-publishing-increase-bottom";
            incBottom.Info = id.ToString();
            incBottom.Click += incBottom_Click;
            w.Content.Controls.Add(incBottom);

            LinkButton decBottom = new LinkButton();
            decBottom.Text = "&nbsp;";
            decBottom.ToolTip = "Decrease Bottom Margin";
            decBottom.CssClass = "magix-publishing-decrease-bottom";
            decBottom.Info = id.ToString();
            decBottom.Click += decBottom_Click;
            w.Content.Controls.Add(decBottom);

            LinkButton incLeft = new LinkButton();
            incLeft.Text = "&nbsp;";
            incLeft.ToolTip = "Increase Left Margin";
            incLeft.CssClass = "magix-publishing-increase-left";
            incLeft.Info = id.ToString();
            incLeft.Click += incLeft_Click;
            w.Content.Controls.Add(incLeft);

            LinkButton decLeft = new LinkButton();
            decLeft.Text = "&nbsp;";
            decLeft.ToolTip = "Decrease Left Margin";
            decLeft.CssClass = "magix-publishing-decrease-left";
            decLeft.Info = id.ToString();
            decLeft.Click += decLeft_Click;
            w.Content.Controls.Add(decLeft);

            LinkButton incPadding = new LinkButton();
            incPadding.Text = "&nbsp;";
            incPadding.ToolTip = "Increase Right Margin";
            incPadding.CssClass = "magix-publishing-increase-padding";
            incPadding.Info = id.ToString();
            incPadding.Click += incPadding_Click;
            w.Content.Controls.Add(incPadding);

            LinkButton decPadding = new LinkButton();
            decPadding.Text = "&nbsp;";
            decPadding.ToolTip = "Decrease Right Margin";
            decPadding.CssClass = "magix-publishing-decrease-padding";
            decPadding.Info = id.ToString();
            decPadding.Click += decPadding_Click;
            w.Content.Controls.Add(decPadding);
        }

        void incWidth_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseWidth");
        }

        void decWidth_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseWidth");
        }

        void incHeight_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseHeight");
        }

        void decHeight_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseHeight");
        }

        void incDown_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseDown");
        }

        void decDown_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseDown");
        }

        void incBottom_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseBottom");
        }

        void decBottom_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseBottom");
        }

        void incLeft_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseLeft");
        }

        void decLeft_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseLeft");
        }

        void incPadding_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreasePadding");
        }

        void decPadding_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreasePadding");
        }

        private void ChangeProperty(object sender, string action)
        {
            LinkButton lbn = sender as LinkButton;
            int idPart = int.Parse(lbn.Info);

            Node node = new Node();

            node["ID"].Value = idPart;
            node["Action"].Value = action;

            RaiseSafeEvent(
                "Magix.Publishing.ChangeTemplateProperty",
                node);

            ReDataBind();
        }

        private void ReDataBind()
        {
            DataSource["Templates"].UnTie();
            DataSource["AllModules"].UnTie();

            RaiseEvent(
                "Magix.Publishing.GetTemplates",
                DataSource);

            parts.Controls.Clear();
            DataBindWebParts();
            parts.ReRender();
        }

        private static void SetCommonWebPartProperties(int width, int height, bool last, string name, int id, int padding, int push, int top, int bottomMargin, string moduleName, Window w)
        {
            w.ToolTip = string.Format("Module is of type '{0}'", moduleName);
            w.CssClass += " mux-shaded mux-rounded";
            if (width > 0)
                w.CssClass += " span-" + width;
            if (height > 0)
                w.CssClass += " height-" + height;
            if (last)
                w.CssClass += " last";
            w.Caption = name;
            w.Info = id.ToString();
            if (padding > 0)
                w.CssClass += " pushRight-" + padding;
            if (push > 0)
                w.CssClass += " pushLeft-" + push;
            if (top > 0)
                w.CssClass += " down-" + top;
            if (bottomMargin > 0)
                w.CssClass += " spcBottom-" + bottomMargin;
            w.Draggable = false;
            w.Closable = false;
        }

        private void CreateModuleSelectListForWebPart(int id, string moduleName, Window w)
        {
            if (DataSource.Contains("AllModules"))
            {
                SelectList sel = new SelectList();
                sel.CssClass = "span-3";
                sel.Info = id.ToString();
                sel.ToolTip = "Module Type of WebPart";
                sel.SelectedIndexChanged +=
                    delegate(object sender, EventArgs e)
                    {
                        SelectList sel2 = sender as SelectList;
                        int id2 = int.Parse(sel2.Info);

                        Node nn = new Node();

                        nn["ID"].Value = id2;
                        nn["ModuleName"].Value = sel.SelectedItem.Value;

                        RaiseSafeEvent(
                            "Magix.Publishing.ChangeModuleForTemplate",
                            nn);
                    };
                foreach (Node idxN1 in DataSource["AllModules"])
                {
                    ListItem li =
                        new ListItem(
                            idxN1["ShortName"].Get<string>(),
                            idxN1["ModuleName"].Get<string>());
                    if (idxN1["ModuleName"].Get<string>() == moduleName)
                        li.Selected = true;
                    sel.Items.Add(li);
                }
                w.Content.Controls.Add(sel);
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.TemplateWasModified")]
        protected void Magix_Publishing_TemplateWasModified(object sender, EventArgs e)
        {
            ReDataBind();
        }
    }
}



