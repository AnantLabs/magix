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
                        bool overflow = idxC["Overflow"].Get<bool>();
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
                        if (overflow)
                            w.CssClass += " overflow-design";
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
                        w.CssClass += " ";

                        CreateActionButtons(id, w);

                        CreateNameInPlaceEdit(name, id, w);

                        InPlaceTextAreaEdit tx = CreateChangeCssClassInPlaceEdit(id, cssClass, w);

                        CreateChangeCssClassTemplate(id, name, w, cssClass, tx);

                        CreateModuleSelectListForWebPart(id, moduleName, w);

                        // Last CheckBox
                        CheckBox ch = new CheckBox();
                        ch.Checked = last;
                        ch.ID = "lch-" + id;
                        ch.CheckedChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                ch = sender as CheckBox;
                                Node nx = new Node();

                                nx["ID"].Value = id;
                                nx["Action"].Value = "ChangeLast";
                                nx["Value"].Value = ch.Checked;

                                RaiseSafeEvent(
                                    "Magix.Publishing.ChangeTemplateProperty",
                                    nx);

                                if (ch.Checked)
                                    w.CssClass += " last";
                                else
                                    w.CssClass = w.CssClass.Replace(" last", "");
                            };

                        Label lbl = new Label();
                        lbl.ID = "llbl-" + id;
                        lbl.Text = "Last";
                        lbl.Tag = "label";
                        lbl.Load
                            +=
                            delegate
                            {
                                lbl.For = ch.ClientID;
                            };

                        Panel pnl = new Panel();
                        ch.ID = "lpnl-" + id;
                        pnl.CssClass = "span-2";
                        pnl.Controls.Add(ch);
                        pnl.Controls.Add(lbl);
                        pnl.ToolTip = "Whether or not this WebPart is to the very far right or not ... [Try experiementing if you experience unwanted 'jumping' while moving WebPart around or resizing it ...]";

                        w.Content.Controls.Add(pnl);

                        // Overflow CheckBox
                        CheckBox ch1 = new CheckBox();
                        ch1.Checked = overflow;
                        ch1.ID = "och-" + id;
                        ch1.CheckedChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                Node nx = new Node();

                                nx["ID"].Value = id;
                                nx["Action"].Value = "ChangeOverflow";
                                nx["Value"].Value = ch.Checked;

                                RaiseSafeEvent(
                                    "Magix.Publishing.ChangeTemplateProperty",
                                    nx);

                                if (ch.Checked)
                                    w.CssClass += " overflow-design";
                                else
                                    w.CssClass = w.CssClass.Replace(" overflow-design", "");
                            };

                        Label lbl1 = new Label();
                        lbl1.ID = "olbl-" + id;
                        lbl1.Text = "Overflow";
                        lbl1.Load
                            +=
                            delegate
                            {
                                lbl1.For = ch1.ClientID;
                            };
                        lbl1.Tag = "label";

                        Panel pnl1 = new Panel();
                        ch1.ID = "opnl-" + id;
                        pnl1.CssClass = "span-3";
                        pnl1.Controls.Add(ch1);
                        pnl1.Controls.Add(lbl1);
                        pnl1.ToolTip = "Whether or not this will allow its content to overflow in the vertical direction";

                        w.Content.Controls.Add(pnl1);

                        // Delete 'this' button
                        LinkButton b = new LinkButton();
                        b.Text = "Delete";
                        b.CssClass = "span-2 last";
                        b.Style[Styles.display] = "block";
                        b.Click +=
                            delegate
                            {
                                Node dl = new Node();
                                dl["ID"].Value = id;

                                RaiseSafeEvent(
                                    "Magix.Publishing.DeleteWebPartTemplateFromWebPageTemplate",
                                    dl);

                                ReDataBind();
                            };
                        w.Content.Controls.Add(b);

                        parts.Controls.Add(w);
                    }
                }
            }
        }

        private void CreateChangeCssClassTemplate(int id, string name, Window w, string cssClass, InPlaceTextAreaEdit tx)
        {
            Node node = new Node();
            node["ID"].Value = id;
            node["Name"].Value = name;

            RaiseSafeEvent(
                "Magix.Publishing.GetCssTemplatesForWebPartTemplate",
                node);

            if (node.Contains("Classes"))
            {
                SelectList ls = new SelectList();
                ls.ID = "selTem-" + id;
                ls.CssClass = "span-3";
                ls.ToolTip = "Changes CSS class according to which templates the system have found in 'media/modules/web-part-templates.css' file ...";
                ls.SelectedIndexChanged +=
                    delegate
                    {
                        Node nx = new Node();
                        nx["ID"].Value = id;
                        nx["Value"].Value = ls.SelectedItem.Value;

                        RaiseSafeEvent(
                            "Magix.Publishing.ChangeTemplateOfWebPartTemplate",
                            nx);

                        tx.Text = ls.SelectedItem.Value;
                    };
                ls.Items.Add(new ListItem("Choose Template ...", "-1"));

                foreach (Node idx in node["Classes"])
                {
                    ListItem li = new ListItem(idx["Name"].Get<string>(), idx["Value"].Get<string>());
                    if (li.Value == cssClass)
                        li.Selected = true;
                    ls.Items.Add(li);
                }
                w.Content.Controls.Add(ls);
            }
        }

        private InPlaceTextAreaEdit CreateChangeCssClassInPlaceEdit(int id, string cssClass, Window w)
        {
            InPlaceTextAreaEdit tx = new InPlaceTextAreaEdit();
            tx.Text = cssClass;
            tx.CssClass += " span-4 editer";
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
            tx.ToolTip = "Css Class of WebPart [better changed with 'Template Change DropBox, unless you _know_ what you do ...!]";
            w.Content.Controls.Add(tx);

            return tx;
        }

        private void CreateNameInPlaceEdit(string name, int id, Window w)
        {
            InPlaceTextAreaEdit nameI = new InPlaceTextAreaEdit();
            nameI.Text = name;
            nameI.Info = id.ToString();
            nameI.CssClass += " span-3 editer";
            nameI.ToolTip = "Name of WebPart [also helps determine which CSS Templates to use ...]";
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
            incWidth.DblClick += incWidth_DblClick;
            w.Content.Controls.Add(incWidth);

            LinkButton decWidth = new LinkButton();
            decWidth.Text = "&nbsp;";
            decWidth.ToolTip = "Decrease Width";
            decWidth.CssClass = "magix-publishing-decrease-width";
            decWidth.Info = id.ToString();
            decWidth.Click += decWidth_Click;
            decWidth.DblClick += decWidth_DblClick;
            w.Content.Controls.Add(decWidth);

            LinkButton incHeight = new LinkButton();
            incHeight.Text = "&nbsp;";
            incHeight.ToolTip = "Increase Height";
            incHeight.CssClass = "magix-publishing-increase-height";
            incHeight.Info = id.ToString();
            incHeight.Click += incHeight_Click;
            incHeight.DblClick += incHeight_DblClick;
            w.Content.Controls.Add(incHeight);

            LinkButton decHeight = new LinkButton();
            decHeight.Text = "&nbsp;";
            decHeight.ToolTip = "Decrease Height";
            decHeight.CssClass = "magix-publishing-decrease-height";
            decHeight.Info = id.ToString();
            decHeight.Click += decHeight_Click;
            decHeight.DblClick += decHeight_DblClick;
            w.Content.Controls.Add(decHeight);

            LinkButton incDown = new LinkButton();
            incDown.Text = "&nbsp;";
            incDown.ToolTip = "Increase Top Margin";
            incDown.CssClass = "magix-publishing-increase-down";
            incDown.Info = id.ToString();
            incDown.Click += incDown_Click;
            incDown.DblClick += incDown_DblClick;
            w.Content.Controls.Add(incDown);

            LinkButton decDown = new LinkButton();
            decDown.Text = "&nbsp;";
            decDown.ToolTip = "Decrease Top Margin";
            decDown.CssClass = "magix-publishing-decrease-down";
            decDown.Info = id.ToString();
            decDown.Click += decDown_Click;
            decDown.DblClick += decDown_DblClick;
            w.Content.Controls.Add(decDown);

            LinkButton incBottom = new LinkButton();
            incBottom.Text = "&nbsp;";
            incBottom.ToolTip = "Increase Bottom Margin";
            incBottom.CssClass = "magix-publishing-increase-bottom";
            incBottom.Info = id.ToString();
            incBottom.Click += incBottom_Click;
            incBottom.DblClick += incBottom_DblClick;
            w.Content.Controls.Add(incBottom);

            LinkButton decBottom = new LinkButton();
            decBottom.Text = "&nbsp;";
            decBottom.ToolTip = "Decrease Bottom Margin";
            decBottom.CssClass = "magix-publishing-decrease-bottom";
            decBottom.Info = id.ToString();
            decBottom.Click += decBottom_Click;
            decBottom.DblClick += decBottom_DblClick;
            w.Content.Controls.Add(decBottom);

            LinkButton incLeft = new LinkButton();
            incLeft.Text = "&nbsp;";
            incLeft.ToolTip = "Increase Left Margin";
            incLeft.CssClass = "magix-publishing-increase-left";
            incLeft.Info = id.ToString();
            incLeft.Click += incLeft_Click;
            incLeft.DblClick += incLeft_DblClick;
            w.Content.Controls.Add(incLeft);

            LinkButton decLeft = new LinkButton();
            decLeft.Text = "&nbsp;";
            decLeft.ToolTip = "Decrease Left Margin";
            decLeft.CssClass = "magix-publishing-decrease-left";
            decLeft.Info = id.ToString();
            decLeft.Click += decLeft_Click;
            decLeft.DblClick += decLeft_DblClick;
            w.Content.Controls.Add(decLeft);

            LinkButton incPadding = new LinkButton();
            incPadding.Text = "&nbsp;";
            incPadding.ToolTip = "Increase Right Margin";
            incPadding.CssClass = "magix-publishing-increase-padding";
            incPadding.Info = id.ToString();
            incPadding.Click += incPadding_Click;
            incPadding.DblClick += incPadding_DblClick;
            w.Content.Controls.Add(incPadding);

            LinkButton decPadding = new LinkButton();
            decPadding.Text = "&nbsp;";
            decPadding.ToolTip = "Decrease Right Margin";
            decPadding.CssClass = "magix-publishing-decrease-padding";
            decPadding.Info = id.ToString();
            decPadding.Click += decPadding_Click;
            decPadding.DblClick += decPadding_DblClick;
            w.Content.Controls.Add(decPadding);
        }

        void incWidth_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseWidth", -1);
        }

        void decWidth_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseWidth", -1);
        }

        void incHeight_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseHeight", -1);
        }

        void decHeight_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseHeight", -1);
        }

        void incDown_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseDown", -1);
        }

        void decDown_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseDown", -1);
        }

        void incBottom_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseBottom", -1);
        }

        void decBottom_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseBottom", -1);
        }

        void incLeft_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseLeft", -1);
        }

        void decLeft_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseLeft", -1);
        }

        void incPadding_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreasePadding", -1);
        }

        void decPadding_Click(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreasePadding", -1);
        }

        private void ChangeProperty(object sender, string action, int value)
        {
            LinkButton lbn = sender as LinkButton;
            int idPart = int.Parse(lbn.Info);

            Node node = new Node();

            node["ID"].Value = idPart;
            node["Action"].Value = action;

            if (value != -1)
            {
                node["NewValue"].Value = value;
            }

            RaiseSafeEvent(
                "Magix.Publishing.ChangeTemplateProperty",
                node);

            Panel p = lbn.Parent.Parent as Window;

            if (node.Contains("NewWidth"))
                p.CssClass = p.CssClass.Replace(
                    " span-" +
                    node["OldWidth"].Get<int>(), "") +
                    " span-" +
                    node["NewWidth"].Get<int>();

            if (node.Contains("NewHeight"))
                p.CssClass = p.CssClass.Replace(
                    " height-" +
                    node["OldHeight"].Get<int>(), "") +
                    " height-" +
                    node["NewHeight"].Get<int>();

            if (node.Contains("NewTop"))
                p.CssClass = p.CssClass.Replace(
                    " down-" +
                    node["OldTop"].Get<int>(), "") +
                    " down-" +
                    node["NewTop"].Get<int>();

            if (node.Contains("NewPadding"))
                p.CssClass = p.CssClass.Replace(
                    " pushRight-" +
                    node["OldPadding"].Get<int>(), "") +
                    " pushRight-" +
                    node["NewPadding"].Get<int>();

            if (node.Contains("NewPush"))
                p.CssClass = p.CssClass.Replace(
                    " pushLeft-" +
                    node["OldPush"].Get<int>(), "") +
                    " pushLeft-" +
                    node["NewPush"].Get<int>();

            if (node.Contains("NewMarginBottom"))
                p.CssClass = p.CssClass.Replace(
                    " spcBottom-" +
                    node["OldMarginBottom"].Get<int>(), "") +
                    " spcBottom-" +
                    node["NewMarginBottom"].Get<int>();
        }

        void incWidth_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseWidth", 100);
        }

        void decWidth_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseWidth", 0);
        }

        void incHeight_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseHeight", 100);
        }

        void decHeight_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseHeight", 0);
        }

        void incDown_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseDown", 100);
        }

        void decDown_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseDown", 0);
        }

        void incBottom_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseBottom", 100);
        }

        void decBottom_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseBottom", 0);
        }

        void incLeft_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreaseLeft", 100);
        }

        void decLeft_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreaseLeft", 0);
        }

        void incPadding_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "IncreasePadding", 100);
        }

        void decPadding_DblClick(object sender, EventArgs e)
        {
            ChangeProperty(sender, "DecreasePadding", 0);
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
                sel.ID = "selMod-" + id;
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

        [ActiveEvent(Name = "Magix.Publishing.WebPartTemplateWasModified")]
        protected void Magix_Publishing_WebPartTemplateWasModified(object sender, EventArgs e)
        {
            ReDataBind();
        }
    }
}



