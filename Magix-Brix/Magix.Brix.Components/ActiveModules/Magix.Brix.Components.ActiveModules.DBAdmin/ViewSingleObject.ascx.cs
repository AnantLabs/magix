/*
 * Magix - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Effects;
using Magix.UX.Widgets.Core;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    [ActiveModule]
    public class ViewSingleObject : Module, IModule
    {
        protected Panel pnl;
        protected Button change;
        protected Button remove;
        protected Panel changePnl;
        protected Panel removePnl;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);
            Load +=
                delegate
                {
                    if (node.Contains("ChildCssClass"))
                    {
                        pnl.CssClass = node["ChildCssClass"].Get<string>();
                    }
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBindObjects();
        }

        protected void change_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            node["ParentID"].Value = DataSource["ParentID"].Value;
            node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
            node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;
            RaiseSafeEvent(
                "DBAdmin.Form.ChangeObject",
                node);
        }

        protected void remove_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            node["ParentID"].Value = DataSource["ParentID"].Value;
            node["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
            node["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;

            RaiseSafeEvent(
                "DBAdmin.Data.RemoveObject",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");
        }

        protected void DataBindObjects()
        {
            if (DataSource.Contains("Object") && 
                DataSource["Object"]["ID"].Get<int>() != 0)
            {
                Label tb = new Label();
                tb.Tag = "table";
                tb.CssClass = "viewObjects singleInstance";

                // Header rows
                tb.Controls.Add(CreateHeaderRow());

                // Creating property rows
                foreach (Node idxProp in DataSource["Type"]["Properties"])
                {
                    if (!DataSource.Contains("WhiteListColumns") ||
                        (DataSource["WhiteListColumns"].Contains(idxProp.Name) &&
                        DataSource["WhiteListColumns"][idxProp.Name].Get<bool>()))
                    {
                        tb.Controls.Add(CreateRow(DataSource["Object"]["Properties"][idxProp.Name]));
                    }
                }
                pnl.Controls.Add(tb);
            }
            DataBindDone();
        }

        protected void DataBindDone()
        {
            changePnl.Visible = DataSource["IsChange"].Get<bool>();
            removePnl.Visible = DataSource["IsRemove"].Get<bool>() &&
                DataSource.Contains("Object");
            string parentTypeName = 
                !DataSource.Contains("ParentFullTypeName") ? 
                "" : 
                DataSource["ParentFullTypeName"].Get<string>();

            string caption = "";
            if (DataSource.Contains("Caption"))
            {
                caption = DataSource["Caption"].Get<string>();
            }
            else
            {
                if (DataSource["ParentID"].Get<int>() > 0)
                {
                    parentTypeName =
                        parentTypeName.Substring(
                            parentTypeName.LastIndexOf(".") + 1);
                    if (DataSource.Contains("Object"))
                    {
                        caption = string.Format(
                            "{0}[{1}] of {2}[{3}]/{4}",
                            DataSource["TypeName"].Get<string>(),
                            DataSource["Object"]["ID"].Get<int>(),
                            parentTypeName.Substring(parentTypeName.LastIndexOf(".") + 1),
                            DataSource["ParentID"].Value,
                            DataSource["ParentPropertyName"].Value);
                    }
                }
                else
                {
                    if (DataSource.Contains("Object"))
                    {
                        caption = string.Format(
                            "{0}[{1}]",
                            DataSource["TypeName"].Get<string>(),
                            DataSource["Object"]["ID"].Get<int>());
                    }
                    else
                    {
                        caption = "Viewing an object...";
                    }
                }
            }
            if (Parent.Parent.Parent is Window)
            {
                (Parent.Parent.Parent as Window).Caption = caption;
            }
            else
            {
                Node node = new Node();
                node["Caption"].Value = caption;
                RaiseSafeEvent(
                    "Magix.Core.SetFormCaption",
                    node);
            }
        }

        private Label CreateRow(Node node)
        {
            Label row = new Label();
            row.Tag = "tr";

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Name") &&
                DataSource["WhiteListProperties"]["Name"].Get<bool>()))
            {
                if (DataSource["Type"]["Properties"][node.Name].Contains("ClickLabelEvent")
                    && !string.IsNullOrEmpty(DataSource["Type"]["Properties"][node.Name]["ClickLabelEvent"].Get<string>()))
                {
                    string evtName = DataSource["Type"]["Properties"][node.Name]["ClickLabelEvent"].Get<string>();

                    Panel c1 = new Panel();
                    if (DataSource.Contains("WhiteListProperties") &&
                        DataSource["WhiteListProperties"]["Name"].Contains("ForcedWidth"))
                    {
                        c1.CssClass += "wide-" +
                            DataSource["WhiteListProperties"]["Name"]["ForcedWidth"].Get<int>();
                    }
                    bool bold = DataSource["Type"]["Properties"][node.Name].Contains("Bold") &&
                        DataSource["Type"]["Properties"][node.Name]["Bold"].Get<bool>();
                    if (bold)
                        c1.Style[Styles.fontWeight] = "bold";
                    c1.Tag = "td";
                    c1.CssClass = "columnName";

                    LinkButton clicker = new LinkButton();
                    if (DataSource["Type"]["Properties"][node.Name].Contains("Header"))
                    {
                        clicker.Text = DataSource["Type"]["Properties"][node.Name]["Header"].Get<string>();
                    }
                    else
                    {
                        clicker.Text = node.Name;
                    }
                    clicker.Click +=
                        delegate
                        {
                            RaiseSafeEvent(
                                evtName,
                                DataSource["Type"]["Properties"][node.Name]["ClickLabelEvent"]);
                        };
                    c1.Controls.Add(clicker);
                    row.Controls.Add(c1);
                }
                else
                {
                    if (DataSource["Type"]["Properties"][node.Name].Contains("TemplateColumnHeaderEvent") &&
                        !string.IsNullOrEmpty(
                            DataSource["Type"]["Properties"][node.Name]["TemplateColumnHeaderEvent"].Get<string>()))
                    {
                        Label c1 = new Label();
                        c1.Tag = "td";
                        c1.Info = node.Name;

                        if (DataSource.Contains("WhiteListProperties") &&
                            DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                        {
                            c1.CssClass += "wide-" +
                                DataSource["WhiteListProperties"]["Name"]["ForcedWidth"].Get<int>();
                        }

                        string eventName =
                            DataSource["Type"]["Properties"][node.Name]["TemplateColumnHeaderEvent"].Get<string>();

                        Node colNode = new Node();

                        colNode["FullTypeName"].Value = DataSource["FullTypeName"].Get<string>(); ;
                        colNode["Name"].Value = node.Name;
                        colNode["Value"].Value = node.Get<string>();
                        colNode["ID"].Value = DataSource["Object"]["ID"].Get<int>();
                        if (DataSource.Contains("Container"))
                            colNode["Container"].Value = DataSource["Container"].Value;

                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            eventName,
                            colNode);

                        c1.Controls.Add(colNode["Control"].Get<Control>());
                        row.Controls.Add(c1);
                    }
                    else
                    {
                        Label c1 = new Label();
                        if (DataSource.Contains("WhiteListProperties") &&
                            DataSource["WhiteListProperties"]["Name"].Contains("ForcedWidth"))
                        {
                            c1.CssClass += "wide-" +
                                DataSource["WhiteListProperties"]["Name"]["ForcedWidth"].Get<int>();
                        }
                        bool bold = DataSource["Type"]["Properties"][node.Name].Contains("Bold") &&
                            DataSource["Type"]["Properties"][node.Name]["Bold"].Get<bool>();
                        if (bold)
                            c1.Style[Styles.fontWeight] = "bold";
                        c1.Tag = "td";
                        c1.CssClass = "columnName";
                        if (DataSource["Type"]["Properties"][node.Name].Contains("Header"))
                        {
                            c1.Text = DataSource["Type"]["Properties"][node.Name]["Header"].Get<string>();
                        }
                        else
                        {
                            c1.Text = node.Name;
                        }
                        row.Controls.Add(c1);
                    }
                }
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Type") &&
                DataSource["WhiteListProperties"]["Type"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                c1.CssClass = "columnType";
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"]["Type"].Contains("ForcedWidth"))
                {
                    c1.CssClass += "wide-" +
                        DataSource["WhiteListProperties"]["Type"]["ForcedWidth"].Get<int>();
                }
                c1.Text =
                    DataSource["Type"]["Properties"][node.Name]["TypeName"].Get<string>()
                        .Replace("<", "&lt;").Replace(">", "&gt;");
                row.Controls.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Attributes") &&
                DataSource["WhiteListProperties"]["Attributes"].Get<bool>()))
            {
                Label c1 = new Label();
                c1.Tag = "td";
                c1.CssClass = "columnType";
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"]["Attributes"].Contains("ForcedWidth"))
                {
                    c1.CssClass += "wide-" +
                        DataSource["WhiteListProperties"]["Attributes"]["ForcedWidth"].Get<int>();
                }
                string text = "";
                if (!DataSource["Type"]["Properties"][node.Name]["IsOwner"].Get<bool>())
                    text += "IsNotOwner ";
                if (DataSource["Type"]["Properties"][node.Name]["BelongsTo"].Get<bool>())
                    text += "BelongsTo ";
                if (!string.IsNullOrEmpty(DataSource["Type"]["Properties"][node.Name]["RelationName"].Get<string>()))
                    text += "'" + DataSource["Type"]["Properties"][node.Name]["RelationName"].Get<string>() + "'";
                c1.Text = text;
                row.Controls.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Value") &&
                DataSource["WhiteListProperties"]["Value"].Get<bool>()))
            {
                if (DataSource["Type"]["Properties"][node.Name].Contains("TemplateColumnEvent") &&
                    !string.IsNullOrEmpty(
                        DataSource["Type"]["Properties"][node.Name]["TemplateColumnEvent"].Get<string>()))
                {
                    Label c1 = new Label();
                    c1.Tag = "td";
                    c1.Info = node.Name;

                    if (DataSource.Contains("WhiteListProperties") &&
                        DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                    {
                        c1.CssClass += "wide-" +
                            DataSource["WhiteListProperties"]["Value"]["ForcedWidth"].Get<int>();
                    }

                    string eventName = 
                        DataSource["Type"]["Properties"][node.Name]["TemplateColumnEvent"].Get<string>();

                    Node colNode = new Node();
                    colNode["FullTypeName"].Value = DataSource["FullTypeName"].Get<string>(); ;
                    colNode["MetaViewName"].Value = DataSource["MetaViewName"].Get<string>(); ;
                    colNode["Name"].Value = node.Name;
                    colNode["Value"].Value = node.Get<string>();
                    colNode["ID"].Value = DataSource["Object"]["ID"].Get<int>();
                    colNode["PageObjectTemplateID"].Value = DataSource["PageObjectTemplateID"].Value;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        eventName,
                        colNode);

                    c1.Controls.Add(colNode["Control"].Get<Control>());
                    row.Controls.Add(c1);
                }
                else
                {
                    bool bold = DataSource["Type"]["Properties"][node.Name].Contains("Bold") &&
                        DataSource["Type"]["Properties"][node.Name]["Bold"].Get<bool>();
                    Label c1 = new Label();
                    if (bold)
                        c1.Style[Styles.fontWeight] = "bold";
                    if (DataSource.Contains("WhiteListProperties") &&
                        DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                    {
                        c1.CssClass += "wide-" +
                            DataSource["WhiteListProperties"]["Value"]["ForcedWidth"].Get<int>();
                    }
                    c1.Tag = "td";
                    if (DataSource["Type"]["Properties"][node.Name]["IsComplex"].Get<bool>())
                    {
                        if (DataSource["Type"]["Properties"][node.Name].Contains("ReadOnly") &&
                            DataSource["Type"]["Properties"][node.Name]["ReadOnly"].Get<bool>())
                        {
                            Label ed = new Label();
                            ed.Text = node.Value.ToString();
                            if (DataSource["Type"]["Properties"][node.Name].Contains("MaxLength"))
                            {
                                if (ed.Text.Length > DataSource["Type"]["Properties"][node.Name]["MaxLength"].Get<int>())
                                {
                                    ed.Text = ed.Text.Substring(0, DataSource["Type"]["Properties"][node.Name]["MaxLength"].Get<int>()) + " ...";
                                }
                            }
                            if (DataSource.Contains("WhiteListProperties") &&
                                DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                            {
                                c1.CssClass += "span-" + 
                                    DataSource["WhiteListProperties"]["Value"]["ForcedWidth"].Get<int>();
                            }
                            c1.Controls.Add(ed);
                        }
                        else
                        {
                            LinkButton ed = new LinkButton();
                            ed.Text = node.Value.ToString();
                            ed.Info = node.Name;
                            if (DataSource["Type"]["Properties"][node.Name]["BelongsTo"].Get<bool>())
                                ed.CssClass = "belongsTo";
                            ed.Click +=
                                delegate(object sender, EventArgs e)
                                {
                                    LinkButton lb = sender as LinkButton;
                                    Label ctrlOld = Magix.UX.Selector.SelectFirst<Label>(lb.Parent.Parent.Parent,
                                        delegate(Control idxCtrl)
                                        {
                                            BaseWebControl ctrl = idxCtrl as BaseWebControl;
                                            if (ctrl != null)
                                                return ctrl.CssClass == "grid-selected";
                                            return false;
                                        });
                                    if (ctrlOld != null)
                                        ctrlOld.CssClass = "";
                                    (lb.Parent.Parent as Label).CssClass = "grid-selected";
                                    int id = DataSource["Object"]["ID"].Get<int>();
                                    string column = lb.Info;
                                    Node n = new Node();
                                    n["ID"].Value = id;
                                    n["PropertyName"].Value = column;
                                    n["IsList"].Value = DataSource["Type"]["Properties"][column]["IsList"].Value;
                                    n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                                    RaiseSafeEvent(
                                        "DBAdmin.Form.ViewListOrComplexPropertyValue",
                                        n);
                                };
                            c1.Controls.Add(ed);
                        }
                    }
                    else
                    {
                        if (DataSource["Type"]["Properties"][node.Name].Contains("ReadOnly") &&
                            DataSource["Type"]["Properties"][node.Name]["ReadOnly"].Get<bool>())
                        {
                            Label ed = new Label();
                            ed.Text = node.Value as string;
                            if (DataSource.Contains("WhiteListProperties") &&
                                DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                            {
                                ed.CssClass += "span-" +
                                    DataSource["WhiteListProperties"]["Value"]["ForcedWidth"].Get<int>();
                            }
                            c1.Controls.Add(ed);
                        }
                        else
                        {
                            switch (DataSource["Type"]["Properties"][node.Name]["FullTypeName"].Get<string>())
                            {
                                default:
                                    {
                                        TextAreaEdit ed = new TextAreaEdit();
                                        ed.TextLength = 500;
                                        ed.Text = node.Value as string;
                                        ed.CssClass += " larger";
                                        ed.Info = node.Name;
                                        ed.TextChanged +=
                                            delegate(object sender, EventArgs e)
                                            {
                                                TextAreaEdit edit = sender as TextAreaEdit;
                                                int id = DataSource["Object"]["ID"].Get<int>();
                                                string column = edit.Info;
                                                Node n = new Node();
                                                n["ID"].Value = id;
                                                n["PropertyName"].Value = column;
                                                n["NewValue"].Value = edit.Text;
                                                n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                                                RaiseSafeEvent(
                                                    DataSource.Contains("ChangeSimplePropertyValue") ? 
                                                        DataSource["ChangeSimplePropertyValue"].Get<string>() :
                                                        "DBAdmin.Data.ChangeSimplePropertyValue",
                                                    n);
                                            };
                                        c1.Controls.Add(ed);
                                    } break;
                            }
                        }
                    }
                    row.Controls.Add(c1);
                }
            }
            return row;
        }

        private HtmlTableRow CreateHeaderRow()
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("class", "header");

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Name") &&
                DataSource["WhiteListProperties"]["Name"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"].Contains("Name") &&
                    DataSource["WhiteListProperties"]["Name"].Contains("Header"))
                {
                    c1.InnerHtml = 
                        DataSource["WhiteListProperties"]["Name"]["Header"].Get<string>();
                }
                else
                {
                    c1.InnerHtml = "Name";
                }
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"]["Name"].Contains("ForcedWidth"))
                    c1.Attributes.Add(
                        "class",
                        "wide-" +
                        DataSource["WhiteListProperties"]["Name"]["ForcedWidth"].Get<int>());
                else
                    c1.Attributes.Add("class", "wide-4");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Type") &&
                DataSource["WhiteListProperties"]["Type"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Type";
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"]["Type"].Contains("ForcedWidth"))
                    c1.Attributes.Add(
                        "class",
                        "wide-" +
                        DataSource["WhiteListProperties"]["Type"]["ForcedWidth"].Get<int>());
                else
                    c1.Attributes.Add("class", "wide-4");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Attributes") &&
                DataSource["WhiteListProperties"]["Attributes"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                c1.InnerHtml = "Attributes";
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"]["Attributes"].Contains("ForcedWidth"))
                    c1.Attributes.Add(
                        "class",
                        "wide-" +
                        DataSource["WhiteListProperties"]["Attributes"]["ForcedWidth"].Get<int>());
                else
                    c1.Attributes.Add("class", "wide-5");
                row.Cells.Add(c1);
            }

            if (!DataSource.Contains("WhiteListProperties") ||
                (DataSource["WhiteListProperties"].Contains("Value") &&
                DataSource["WhiteListProperties"]["Value"].Get<bool>()))
            {
                HtmlTableCell c1 = new HtmlTableCell();
                if (DataSource.Contains("WhiteListProperties") &&
                    DataSource["WhiteListProperties"].Contains("Value") &&
                    DataSource["WhiteListProperties"]["Value"].Contains("Header"))
                {
                    c1.InnerHtml =
                        DataSource["WhiteListProperties"]["Value"]["Header"].Get<string>();
                }
                else
                {
                    c1.InnerHtml = "Value";
                }
                if (DataSource.Contains("WhiteListProperties") && 
                    DataSource["WhiteListProperties"]["Value"].Contains("ForcedWidth"))
                    c1.Attributes.Add(
                        "class", 
                        "wide-" + 
                        DataSource["WhiteListProperties"]["Value"]["ForcedWidth"].Get<int>());
                else
                    c1.Attributes.Add("class", "wide-7");
                row.Cells.Add(c1);
            }
            return row;
        }

        [ActiveEvent(Name = "Magix.Core.CheckIfIDIsBeingSingleEdited")]
        protected void Magix_Core_CheckIfIDIsBeingSingleEdited(object sender, ActiveEventArgs e)
        {
            if (e.Params["ID"].Get<int>() == DataSource["ID"].Get<int>())
                e.Params["Yes"].Value = true;
        }

        [ActiveEvent(Name = "Magix.Core.ChangeCssClassOfModule")]
        protected void Magix_Core_ChangeCssClassOfModule(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == 
                DataSource["FullTypeName"].Get<string>())
            {
                if (!e.Params.Contains("ID") || 
                    e.Params["ID"].Get<int>() == DataSource["ID"].Get<int>())
                {
                    if (e.Params.Contains("Replace"))
                    {
                        pnl.CssClass =
                            pnl.CssClass.Replace(
                                e.Params["Replace"].Get<string>(),
                                e.Params["CssClass"].Get<string>());
                    }
                    else
                    {
                        pnl.CssClass += e.Params["CssClass"].Get<string>();
                    }
                }
            }
        }

        protected override void ReDataBind()
        {
            if (DataSource.Contains("DoNotRebind") &&
                DataSource["DoNotRebind"].Get<bool>())
                return;
            if (DataSource.Contains("ParentID") && 
                DataSource["ParentID"].Get<int>() > 0)
            {
                DataSource["Object"].UnTie();
                //DataSource["Type"].UnTie();
                if (RaiseSafeEvent(
                    "DBAdmin.Data.GetObjectFromParentProperty",
                    DataSource))
                {
                    pnl.Controls.Clear();
                    DataBindObjects();
                    pnl.ReRender();
                }
            }
            else
            {
                if (!DataSource.Contains("Object"))
                    return;
                DataSource["ID"].Value = DataSource["Object"]["ID"].Get<int>();
                DataSource["Object"].UnTie();
                //DataSource["Type"].UnTie(); // TODO; Remove ALL of these UnTies [Type unties] since they destroy architecture by not allowing DRY ...
                if (RaiseSafeEvent(
                    DataSource.Contains("GetObjectEvent") ?
                        DataSource["GetObjectEvent"].Get<string>() :
                        "DBAdmin.Data.GetObject",
                    DataSource))
                {
                    pnl.Controls.Clear();
                    DataBindObjects();
                    pnl.ReRender();
                }

                // Checking to see if our object has 'vanished' ...
                if (!DataSource.Contains("Object"))
                {
                    // We are looking at one object, with no parent 'select logic' included
                    // This one object is NOT EXISTING
                    // Hence we can safely close this particular window ...
                    ActiveEvents.Instance.RaiseClearControls(Parent.ID);
                    return;
                }
            }
            FlashPanel(pnl);
        }
    }
}




















