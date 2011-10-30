/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using System.Web;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    /**
     * Level2: Allows for editing of WebPageTemplate objects. Contains most of the UI
     * which you're probably daily using while adding and creating new templates and such
     */
    [ActiveModule]
    public class EditSpecificTemplate : ActiveModule
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
                        w.CssClass = " mux-web-part";
                        if (overflow)
                            w.CssClass += " mux-overflow-design";

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

                        Label lbl3 = new Label();
                        lbl3.Text = name;
                        lbl3.CssClass = "mux-webpart-widget-info";
                        lbl3.ToolTip = "Click for Settings ...";
                        w.Content.Controls.Add(lbl3);

                        lbl3.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                bool remove =
                                    (((Control)sender).Parent.Parent as Window).CssClass.Contains(" mux-edit-web-part");
                                foreach (Window idxW in Selector.Select<Window>(parts))
                                {
                                    idxW.CssClass = idxW.CssClass.Replace(" mux-edit-web-part", "");
                                    idxW.Style[Styles.zIndex] = "1";
                                }
                                (((Control)sender).Parent.Parent as Window).Style[Styles.zIndex] = "2";
                                if (remove)
                                    (((Control)sender).Parent.Parent as Window).CssClass =
                                        (((Control)sender).Parent.Parent as Window).CssClass.Replace(" mux-edit-web-part", "");
                                else
                                    (((Control)sender).Parent.Parent as Window).CssClass += " mux-edit-web-part";
                            };

                        CreateActionButtons(id, w);

                        CreateNameInPlaceEdit(name, id, w);

                        Label lbl2 = new Label();
                        lbl2.Text = "WebPart CSS Class";
                        lbl2.Tag = "label";
                        lbl2.CssClass = " span-5 down-1 mux-webpart-property-edit-widget";
                        w.Content.Controls.Add(lbl2);

                        InPlaceEdit tx = CreateChangeCssClassInPlaceEdit(id, cssClass, w);

                        CreateChangeCssClassTemplate(id, name, w, cssClass, tx);

                        CreateModuleSelectListForWebPart(id, moduleName, w);

                        // Last CheckBox
                        CheckBox ch = new CheckBox();
                        ch.Checked = last;
                        ch.ID = "lch-" + id;
                        ch.CssClass = "span-1";
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
                        lbl.CssClass = "span-4 last";
                        lbl.Tag = "label";
                        lbl.Load
                            +=
                            delegate
                            {
                                lbl.For = ch.ClientID;
                            };

                        Panel pnl = new Panel();
                        ch.ID = "lpnl-" + id;
                        pnl.CssClass = "span-5 down-1 mux-webpart-property-edit-widget";
                        pnl.Controls.Add(ch);
                        pnl.Controls.Add(lbl);

                        w.Content.Controls.Add(pnl);

                        // Overflow CheckBox
                        CheckBox ch1 = new CheckBox();
                        ch1.Checked = overflow;
                        ch1.ID = "och-" + id;
                        ch1.CssClass = "span-1";
                        ch1.CheckedChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                Node nx = new Node();

                                nx["ID"].Value = id;
                                nx["Action"].Value = "ChangeOverflow";
                                nx["Value"].Value = ch1.Checked;

                                RaiseSafeEvent(
                                    "Magix.Publishing.ChangeTemplateProperty",
                                    nx);

                                if (ch1.Checked)
                                    w.CssClass += " mux-overflow-design";
                                else
                                    w.CssClass = w.CssClass.Replace(" mux-overflow-design", "");
                            };

                        Label lbl1 = new Label();
                        lbl1.ID = "olbl-" + id;
                        lbl1.Text = "Overflow";
                        lbl1.CssClass = "span-4 last";
                        lbl1.Load
                            +=
                            delegate
                            {
                                lbl1.For = ch1.ClientID;
                            };
                        lbl1.Tag = "label";

                        Panel pnl1 = new Panel();
                        pnl1.ID = "opnl-" + id;
                        pnl1.CssClass = "span-5 down-1 mux-webpart-property-edit-widget";
                        pnl1.Controls.Add(ch1);
                        pnl1.Controls.Add(lbl1);

                        w.Content.Controls.Add(pnl1);

                        // Delete 'this' button
                        LinkButton b = new LinkButton();
                        b.Text = "Delete";
                        b.CssClass = "span-5 down-1 mux-webpart-property-edit-widget";
                        b.Style[Styles.display] = "block";
                        b.Click +=
                            delegate
                            {
                                Node dl = new Node();
                                dl["ID"].Value = id;

                                RaiseSafeEvent(
                                    "Magix.Publishing.DeleteWebPartTemplate",
                                    dl);

                                ReDataBind();
                            };
                        w.Content.Controls.Add(b);

                        parts.Controls.Add(w);
                    }
                }
            }
        }

        private void CreateChangeCssClassTemplate(int id, string name, Window w, string cssClass, InPlaceEdit tx)
        {
            Node node = new Node();
            node["ID"].Value = id;
            node["Name"].Value = name;

            RaiseSafeEvent(
                "Magix.Publishing.GetCssTemplatesForWebPartTemplate",
                node);

            if (node.Contains("Classes"))
            {
                Label lbl2 = new Label();
                lbl2.Text = "CSS Template";
                lbl2.Tag = "label";
                lbl2.CssClass = " span-5 down-1 mux-webpart-property-edit-widget";
                w.Content.Controls.Add(lbl2);

                SelectList ls = new SelectList();
                ls.ID = "selTem-" + id;
                ls.CssClass = "span-5 mux-webpart-property-edit-widget";
                ls.SelectedIndexChanged +=
                    delegate
                    {
                        Node nx = new Node();
                        nx["ID"].Value = id;
                        nx["Value"].Value = ls.SelectedItem.Value;

                        RaiseSafeEvent(
                            "Magix.Publishing.ChangeCssForWebPartTemplateFromCssTemplate",
                            nx);

                        tx.Text = ls.SelectedItem.Value;
                    };
                ls.Items.Add(new ListItem("Choose Template ...", ""));

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

        private InPlaceEdit CreateChangeCssClassInPlaceEdit(int id, string cssClass, Window w)
        {
            InPlaceEdit tx = new InPlaceEdit();
            tx.Text = cssClass;
            tx.CssClass += " span-5 mux-in-place-edit-loose mux-webpart-property-edit-widget";
            tx.Info = id.ToString();
            tx.TextChanged +=
                delegate(object sender, EventArgs e)
                {
                    InPlaceEdit t = sender as InPlaceEdit;

                    Node node = new Node();

                    node["ID"].Value = int.Parse(t.Info);
                    node["Action"].Value = "ChangeCssClass";
                    node["Value"].Value = t.Text;

                    RaiseSafeEvent(
                        "Magix.Publishing.ChangeTemplateProperty",
                        node);
                };
            w.Content.Controls.Add(tx);

            return tx;
        }

        private void CreateNameInPlaceEdit(string name, int id, Window w)
        {
            Label lbl = new Label();
            lbl.Text = "WebPart Name";
            lbl.Tag = "label";
            lbl.CssClass = " span-5 down-1 mux-webpart-property-edit-widget";
            w.Content.Controls.Add(lbl);

            InPlaceEdit nameI = new InPlaceEdit();
            nameI.Text = name;
            nameI.Info = id.ToString();
            nameI.CssClass += " span-5 mux-in-place-edit-loose mux-webpart-property-edit-widget";
            nameI.TextChanged +=
                delegate(object sender, EventArgs e)
                {
                    InPlaceEdit xed = sender as InPlaceEdit;
                    int idPart = int.Parse(xed.Info);

                    Node node = new Node();

                    node["ID"].Value = idPart;
                    node["Action"].Value = "ChangeName";
                    node["Action"]["Value"].Value = xed.Text;

                    if (RaiseSafeEvent(
                        "Magix.Publishing.ChangeTemplateProperty",
                        node))
                    {
                        Selector.SelectFirst<Label>(
                            xed.Parent,
                            delegate(Control idx)
                            {
                                return (idx is Label) &&
                                    (idx as Label).CssClass.Contains("mux-webpart-widget-info");
                            }).Text = xed.Text;
                    }
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
                    " right-" +
                    node["OldPadding"].Get<int>(), "") +
                    " right-" +
                    node["NewPadding"].Get<int>();

            if (node.Contains("NewPush"))
                p.CssClass = p.CssClass.Replace(
                    " push-" +
                    node["OldPush"].Get<int>(), "") +
                    " push-" +
                    node["NewPush"].Get<int>();

            if (node.Contains("NewMarginBottom"))
                p.CssClass = p.CssClass.Replace(
                    " bottom-" +
                    node["OldMarginBottom"].Get<int>(), "") +
                    " bottom-" +
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
                "Magix.Publishing.GetWebPageTemplates",
                DataSource);

            RaiseEvent(
                "Magix.Publishing.GetPublisherPlugins",
                DataSource);

            parts.Controls.Clear();
            DataBindWebParts();
            parts.ReRender();
        }

        private static void SetCommonWebPartProperties(int width, int height, bool last, string name, int id, int padding, int push, int top, int bottomMargin, string moduleName, Window w)
        {
            w.CssClass += " mux-shaded mux-rounded";
            if (width != 0)
                w.CssClass += " span-" + width;
            if (height != 0)
                w.CssClass += " height-" + height;

            if (last)
                w.CssClass += " last";

            w.Caption = name;
            w.Info = id.ToString();
            if (padding != 0)
                w.CssClass += " right-" + padding;
            if (push != 0)
                w.CssClass += " push-" + push;
            if (top != 0)
                w.CssClass += " down-" + top;
            if (bottomMargin != 0)
                w.CssClass += " bottom-" + bottomMargin;
            w.Draggable = false;
            w.Closable = false;
        }

        private void CreateModuleSelectListForWebPart(int id, string moduleName, Window w)
        {
            if (DataSource.Contains("AllModules"))
            {
                Label lbl2 = new Label();
                lbl2.Text = "Module";
                lbl2.Tag = "label";
                lbl2.CssClass = " span-5 down-1 mux-webpart-property-edit-widget";
                w.Content.Controls.Add(lbl2);

                SelectList sel = new SelectList();
                sel.CssClass = "span-5 mux-webpart-property-edit-widget";
                sel.Info = id.ToString();
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
                            "Magix.Publishing.ChangeModuleTypeForWebPartTemplate",
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

        [ActiveEvent(Name = "Magix.Publishing.WebPageTemplateWasModified")]
        protected void Magix_Publishing_MultiHandler(object sender, EventArgs e)
        {
            ReDataBind();
        }
    }
}
