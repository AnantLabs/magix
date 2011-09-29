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
using Magix.UX.Widgets.Core;
using System.Collections.Generic;
using Magix.UX;
using Magix.Brix.Components.ActiveTypes;

namespace Magix.Brix.Components
{
    /**
     * Level4: Implements most of the logic for our DBAdmin module and basically
     * every single Grid we have in Magix. This class is the foundation for probably
     * most Magix screens you've ever seen, since every Grid within the system, virtually,
     * is built using this class. Have so many bells and whistles, it could probably need
     * its own book, not to mention some serious refactoring. Till at least the refactoring
     * parts are over, I think I'll resist the temptation of going 'over the board' in 
     * documenting this, since first of all everything will change later, and you shouldn't
     * fiddle too much directly with this bugger yourself. There's also PLENTY code samples
     * around ANYWHERE in magix of usage of this class, so have fun :)
     */
    public abstract class ListModule : Module
    {
        private LinkButton rc;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    rc.Visible = node["IsFilterColumns"].Get<bool>();
                };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        /**
         * Level4: The control being our 'table element'
         */
        protected abstract Control TableParent { get; }

        /**
         * Level4: Called when databinding are done
         */
        protected abstract void DataBindDone();

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Remove Columns button
            rc = new LinkButton();
            rc.ID = "rc";
            rc.Click += rc_Click;
            rc.ToolTip = "Configure visible columns...";
            rc.Text = "&nbsp;";
            this.Controls.Add(rc);
        }

        protected virtual Control Root
        {
            get { return this; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindGrid();
        }

        protected void rc_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            string fullTypeName = DataSource["FullTypeName"].Get<string>();
            node["FullTypeName"].Value = fullTypeName;

            if (DataSource.Contains("WhiteListColumns"))
                node["WhiteListColumns"] = DataSource["WhiteListColumns"];

            RaiseSafeEvent(
                "DBAdmin.Form.ShowAddRemoveColumns",
                node);
        }

        protected void DataBindGrid()
        {
            Label table = new Label();
            table.Tag = "table";
            table.CssClass = "mux-grid-objects";

            table.Controls.Add(CreateHeaderForTable());

            foreach (Node idx in DataSource["Objects"])
            {
                table.Controls.Add(CreateRow(idx));
            }

            TableParent.Controls.Add(table);
            DataBindDone();

            // TODO: Refactor ...!
            if (Parent.Parent.Parent is Window)
            {
                if (HasFilteredColumns())
                {
                    rc.CssClass = "mux-remove-columns";
                    rc.ToolTip = "Remove columns";
                }
                else
                {
                    rc.CssClass = "mux-restore-columns";
                    rc.ToolTip = "Add removed columns, or remove more columns";
                }
            }
            else
            {
                if (HasFilteredColumns())
                {
                    rc.CssClass = "mux-no-remove-columns";
                    rc.ToolTip = "Remove columns";
                }
                else
                {
                    rc.CssClass = "mux-no-restore-columns";
                    rc.ToolTip = "Add removed columns, or remove more columns";
                }
            }
        }

        private bool HasFilteredColumns()
        {
            bool green = true;
            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                if (!GetColumnVisibility(idx.Name))
                {
                    green = false;
                    break;
                }
            }
            return green;
        }

        protected void FilterMethod(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            Node node = new Node();
            node["PropertyName"].Value = btn.Info;
            node["FullTypeName"].Value = DataSource["FullTypeName"].Get<string>();
            if (DataSource.Contains("WhiteListColumns"))
                node["WhiteListColumns"] = DataSource["WhiteListColumns"];

            RaiseSafeEvent(
                DataSource.Contains("ConfigureFiltersEvent") ? 
                    DataSource["ConfigureFiltersEvent"].Get<string>() : 
                    "DBAdmin.Form.ConfigureFilterForColumn",
                node);
        }

        private Control CreateHeaderForTable()
        {
            Label row = new Label();
            row.Tag = "tr";
            row.CssClass = "mux-grid-header";

            if (DataSource["IsSelect"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                cS.Text = "Select";
                cS.CssClass = "wide-2 mux-no-filter";
                row.Controls.Add(cS);
            }
            bool hasIdFilter = false;
            if (!(DataSource.Contains("NoIdColumn")
                && DataSource["NoIdColumn"].Get<bool>()))
            {
                Label li = new Label();
                li.Tag = "td";
                li.CssClass = "wide-2";
                if (!DataSource["IsFilter"].Get<bool>() ||
                    (DataSource.Contains("FilterOnId") &&
                    !DataSource["FilterOnId"].Get<bool>()))
                {
                    if (DataSource.Contains("IDColumnName"))
                    {
                        li.Text = DataSource["IDColumnName"].Get<string>();
                    }
                    else
                    {
                        li.Text = "ID";
                    }
                    li.CssClass = "wide-2 mux-no-filter";
                }
                else
                {
                    LinkButton b = new LinkButton();
                    b.Text = "ID";
                    b.ToolTip = "Click to filter ";
                    Node fNode = new Node();
                    fNode["Key"].Value =
                        "DBAdmin.Filter." +
                        DataSource["FullTypeName"].Get<string>() +
                        ":ID";
                    fNode["Default"].Value = "";
                    RaiseSafeEvent(
                        "DBAdmin.Data.GetFilter",
                        fNode);
                    string idFilterString = fNode["Filter"].Get<string>();
                    hasIdFilter = !string.IsNullOrEmpty(idFilterString);
                    b.ToolTip += idFilterString.Replace("|", " on ");
                    b.CssClass =
                        string.IsNullOrEmpty(
                            idFilterString) ?
                            "" :
                            "mux-filtered overridden";
                    bool isFilterOnId = !string.IsNullOrEmpty(idFilterString);
                    b.Click += FilterMethod;
                    b.Info = "ID";
                    li.Controls.Add(b);
                }
                row.Controls.Add(li);
            }
            if (DataSource["IsRemove"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                cS.Text = "Rem.";
                cS.CssClass = "wide-2 mux-no-filter";
                row.Controls.Add(cS);
            }
            if (DataSource["IsDelete"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                string header = "Delete";
                if (DataSource.Contains("DeleteHeader"))
                    header = DataSource["DeleteHeader"].Get<string>();
                cS.Text = header;
                cS.CssClass = "wide-2 mux-no-filter";
                row.Controls.Add(cS);
            }

            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                bool columnVisible = GetColumnVisibility(idx.Name);
                if (!columnVisible)
                    continue;
                Label l = new Label();
                l.Tag = "td";
                string idxTypeName = idx["TypeName"].Get<string>();
                if (DataSource.Contains("WhiteListColumns") &&
                    DataSource["WhiteListColumns"][idx.Name].Contains("ForcedWidth"))
                {
                    l.CssClass = "wide-" + 
                        DataSource["WhiteListColumns"][idx.Name]["ForcedWidth"].Get<int>();
                }
                else
                {
                    int wide = 3;
                    switch (idxTypeName)
                    {
                        case "Int32":
                        case "Boolean":
                            break;
                        case "DateTime":
                            wide = 4;
                            break;
                        case "String":
                        default:
                            wide = 4;
                            break;
                    }
                    wide = Math.Max((int)(((double)idx.Name.Length) / 2.5), wide);
                    l.CssClass = "wide-" + wide;
                }
                string captionOfColumn = idx.Name;
                if (idx.Contains("Header") &&
                    !string.IsNullOrEmpty(idx["Header"].Get<string>()))
                {
                    captionOfColumn = idx["Header"].Get<string>();
                }
                if (!DataSource["IsFilter"].Get<bool>() || 
                    (idx.Contains("NoFilter") && idx["NoFilter"].Get<bool>()))
                {
                    l.Text = captionOfColumn;
                    l.CssClass += " mux-no-filter";
                    string toolTip = "";
                    if (idx["BelongsTo"].Get<bool>())
                        toolTip += "BelongsTo ";
                    if (!string.IsNullOrEmpty(idx["RelationName"].Get<string>()))
                        toolTip += "'" + idx["RelationName"].Get<string>() + "' ";
                    l.ToolTip = toolTip;
                }
                else
                {
                    LinkButton b = new LinkButton();
                    b.Text = captionOfColumn;
                    Node fNode = new Node();
                    fNode["Key"].Value = 
                        "DBAdmin.Filter." +
                        DataSource["FullTypeName"].Get<string>() + ":" + 
                        idx.Name;
                    fNode["Default"].Value = "";
                    RaiseSafeEvent(
                        "DBAdmin.Data.GetFilter",
                        fNode);
                    string filterString = fNode["Filter"].Get<string>();
                    b.ToolTip = "Click to filter. ";
                    if (idx["BelongsTo"].Get<bool>())
                        b.ToolTip += "BelongsTo ";
                    if (!string.IsNullOrEmpty(idx["RelationName"].Get<string>()))
                        b.ToolTip += "'" + idx["RelationName"].Get<string>() + "' ";
                    if (!string.IsNullOrEmpty(filterString))
                        b.ToolTip += filterString.Replace("|", " on ");
                    if (hasIdFilter)
                        b.ToolTip += " - Filter overridden by filter on ID column ...";
                    b.CssClass =
                        string.IsNullOrEmpty(
                            filterString) ?
                            "" :
                            (hasIdFilter ? "mux-filtered-overridden" : "mux-filtered");
                    b.Click += FilterMethod;
                    b.Info = idx.Name;
                    l.Controls.Add(b);
                }
                row.Controls.Add(l);
            }
            return row;
        }

        private bool GetColumnVisibility(string colName)
        {
            if (ViewState["ColVisible" + colName] == null)
            {
                bool value = Settings.Instance.Get(
                    "DBAdmin.VisibleColumns." +
                    DataSource["FullTypeName"].Get<string>() + ":" + colName,
                    true);
                ViewState["ColVisible" + colName] = value;
            }
            return (bool)ViewState["ColVisible" + colName];
        }

        protected void ResetColumnsVisibility()
        {
            List<string> keys = new List<string>();
            foreach (string idxKey in ViewState.Keys)
            {
                if (idxKey.StartsWith("ColVisible"))
                    keys.Add(idxKey);
            }
            foreach (string idxKey in keys)
            {
                ViewState.Remove(idxKey);
            }
        }

        protected string SelectedID
        {
            get { return ViewState["SelectedID"] == null ? (string)"-1" : (string)ViewState["SelectedID"]; }
            set { ViewState["SelectedID"] = value; }
        }

        /**
         * Level4: Will 'page' back to 'start' [whatever that is]
         */
        [ActiveEvent(Name = "Magix.Core.SetGridPageStart")]
        public void Magix_Core_SetGridPageStart(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == DataSource["FullTypeName"].Get<string>())
            {
                DataSource["Start"].Value = e.Params["Start"].Get<int>();
                DataSource["End"].Value = e.Params["End"].Get<int>();
                ReDataBind();
            }
        }

        /**
         * Level4: Will set the Active Row of the grid, if the 'FullTypeName' is correct
         */
        [ActiveEvent(Name = "DBAdmin.Grid.SetActiveRow")]
        public void DBAdmin_Grid_SetActiveRow(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == DataSource["FullTypeName"].Get<string>())
            {
                if (!SelectedID.Equals("-1"))
                {
                    if (!e.Params["ID"].Value.Equals(SelectedID))
                    {
                        Label l = Selector.SelectFirst<Label>(this,
                            delegate(Control idxCtrl)
                            {
                                return idxCtrl is Label && 
                                    (idxCtrl as Label).Info == SelectedID.ToString();
                            });
                        if (l != null)
                            l.CssClass = "";
                    }
                }
                SelectedID = e.Params["ID"].Value.ToString();
                if (!SelectedID.Equals("-1"))
                {
                    Label l = Selector.SelectFirst<Label>(this,
                        delegate(Control idxCtrl)
                        {
                            return idxCtrl is Label &&
                                (idxCtrl as Label).Info == SelectedID.ToString();
                        });
                    if (l != null)
                        l.CssClass = "mux-grid-selected";
                }
            }
        }

        private Control CreateRow(Node node)
        {
            Label row = new Label();
            row.Tag = "tr";
            if (node["ID"].Value.Equals(SelectedID))
                row.CssClass = "mux-grid-selected";

            row.Info = node["ID"].Value.ToString();

            if (DataSource["IsSelect"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                LinkButton lb2 = new LinkButton();
                lb2.Text = "Select";
                lb2.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        LinkButton b2 = sender as LinkButton;

                        Node n = new Node();

                        n["ID"].Value = GetId((b2.Parent.Parent as Label).Info);
                        n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                        n["ParentID"].Value = DataSource["ParentID"].Value;
                        n["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
                        n["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;

                        if (DataSource["IsList"].Get<bool>())
                        {
                            RaiseSafeEvent(
                                (DataSource.Contains("SelectEvent") ? 
                                    DataSource["SelectEvent"].Get<string>() : 
                                    "DBAdmin.Data.AppendObjectToParentPropertyList"),
                                n);
                        }
                        else
                        {
                            RaiseSafeEvent(
                                (DataSource.Contains("SelectEvent") ?
                                    DataSource["SelectEvent"].Get<string>() :
                                    "DBAdmin.Data.AppendObjectToParentPropertyList"),
                                n);
                        }
                    };
                cS.Controls.Add(lb2);
                row.Controls.Add(cS);
            }
            if (!(DataSource.Contains("NoIdColumn") &&
                DataSource["NoIdColumn"].Get<bool>()))
            {
                Label li = new Label();
                li.Tag = "td";

                if (DataSource.Contains("IdColumnNotClickable") &&
                    DataSource["IdColumnNotClickable"].Get<bool>())
                {
                    Label lb = new Label();
                    lb.Text = node["ID"].Value.ToString();
                    li.Controls.Add(lb);
                }
                else
                {
                    LinkButton lb = new LinkButton();

                    if (DataSource.Contains("IDColumnValue"))
                    {
                        lb.Text = DataSource["IDColumnValue"].Get<string>();
                    }
                    else
                    {
                        lb.Text = node["ID"].Value.ToString();
                    }

                    lb.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            LinkButton b = sender as LinkButton;
                            Node n = new Node();
                            string id = (b.Parent.Parent as Label).Info;
                            (b.Parent.Parent as Label).CssClass = "mux-grid-selected";
                            if (!SelectedID.Equals("-1"))
                            {
                                if (id != SelectedID)
                                {
                                    Label l = Selector.SelectFirst<Label>(this,
                                        delegate(Control idxCtrl)
                                        {
                                            return idxCtrl is Label && (idxCtrl as Label).Info == SelectedID.ToString();
                                        });
                                    if (l != null)
                                        l.CssClass = "";
                                }
                            }
                            SelectedID = id;

                            n["ID"].Value = GetId(id);
                            n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                            n["DataSource"] = DataSource;

                            RaiseSafeEvent(
                                DataSource.Contains("IDColumnEvent") ?
                                    DataSource["IDColumnEvent"].Get<string>() :
                                    "DBAdmin.Form.ViewComplexObject",
                                n);

                            n["DataSource"].UnTie();
                        };
                    li.Controls.Add(lb);
                }
                row.Controls.Add(li);
            }
            if (DataSource["IsRemove"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                LinkButton lb2 = new LinkButton();
                lb2.Text = "Remove";
                lb2.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        LinkButton b2 = sender as LinkButton;
                        Node n = new Node();
                        string id = (b2.Parent.Parent as Label).Info;
                        (b2.Parent.Parent as Label).CssClass = "mux-grid-selected";
                        if (!SelectedID.Equals("-1"))
                        {
                            if (id != SelectedID)
                            {
                                Label l = Selector.SelectFirst<Label>(this,
                                    delegate(Control idxCtrl)
                                    {
                                        return idxCtrl is Label && (idxCtrl as Label).Info == SelectedID.ToString();
                                    });
                                if (l != null)
                                    l.CssClass = "";
                            }
                        }
                        SelectedID = id;

                        n["ID"].Value = GetId((b2.Parent.Parent as Label).Info);
                        n["FullTypeName"].Value = DataSource["FullTypeName"].Value;
                        n["ParentID"].Value = DataSource["ParentID"].Value;
                        n["ParentPropertyName"].Value = DataSource["ParentPropertyName"].Value;
                        n["ParentFullTypeName"].Value = DataSource["ParentFullTypeName"].Value;

                        RaiseSafeEvent(
                            DataSource.Contains("RemoveEvent") ? 
                                DataSource["RemoveEvent"].Get<string>() :
                                "DBAdmin.Form.RemoveObjectFromParentPropertyList", 
                            n);
                    };
                cS.Controls.Add(lb2);
                row.Controls.Add(cS);
            }
            if (DataSource["IsDelete"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                LinkButton lb2 = new LinkButton();
                string lblTxt = "Delete";
                if (DataSource.Contains("DeleteText"))
                    lblTxt = DataSource["DeleteText"].Get<string>();
                lb2.Text = lblTxt;
                lb2.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        LinkButton b = sender as LinkButton;
                        Node n = new Node();
                        string id = (b.Parent.Parent as Label).Info;
                        (b.Parent.Parent as Label).CssClass = "mux-grid-selected";
                        if (!SelectedID.Equals("-1"))
                        {
                            if (id != SelectedID)
                            {
                                Label l = Selector.SelectFirst<Label>(this,
                                    delegate(Control idxCtrl)
                                    {
                                        return idxCtrl is Label && 
                                            (idxCtrl as Label).Info == 
                                            SelectedID.ToString();
                                    });
                                if (l != null)
                                    l.CssClass = "";
                            }
                        }
                        SelectedID = id;

                        n["ID"].Value = GetId((b.Parent.Parent as Label).Info);
                        n["FullTypeName"].Value = DataSource["FullTypeName"].Value;

                        if (RaiseSafeEvent(
                            DataSource.Contains("DeleteColumnEvent") ? 
                                DataSource["DeleteColumnEvent"].Get<string>() : 
                                "DBAdmin.Data.DeleteObject", 
                            n))
                            ReDataBind();
                    };
                cS.Controls.Add(lb2);
                row.Controls.Add(cS);
            }

            foreach (Node idxType in DataSource["Type"]["Properties"])
            {
                Node idx = node["Properties"][idxType.Name];
                bool columnVisible = GetColumnVisibility(idx.Name);
                if (!columnVisible)
                    continue;

                Label l = new Label();
                l.Tag = "td";
                l.Info = idx.Name;

                if (DataSource["Type"]["Properties"][idx.Name].Contains("TemplateColumnEvent") &&
                    !string.IsNullOrEmpty(
                        DataSource["Type"]["Properties"][idx.Name]["TemplateColumnEvent"].Get<string>()))
                {
                    string eventName = DataSource["Type"]["Properties"][idx.Name]["TemplateColumnEvent"].Get<string>();

                    Node colNode = new Node();
                    colNode["FullTypeName"].Value = DataSource["FullTypeName"].Get<string>(); ;
                    colNode["Name"].Value = idx.Name;
                    colNode["Value"].Value = idx.Get<string>();
                    colNode["MetaViewName"].Value = DataSource["MetaViewName"].Get<string>();
                    colNode["ID"].Value = GetId(node["ID"].Value.ToString());
                    colNode["OriginalWebPartID"].Value = DataSource["OriginalWebPartID"].Value;

                    RaiseSafeEvent(
                        eventName,
                        colNode);

                    if (colNode.Contains("Control"))
                        l.Controls.Add(colNode["Control"].Get<Control>());
                }
                else if (DataSource["Type"]["Properties"][idx.Name]["IsComplex"].Get<bool>())
                {
                    if (DataSource["Type"]["Properties"][idx.Name].Contains("ReadOnly") &&
                        DataSource["Type"]["Properties"][idx.Name]["ReadOnly"].Get<bool>())
                    {
                        l.CssClass += "read-only";
                        Label ll = new Label();
                        ll.Text = idx.Get<string>();
                        l.Controls.Add(ll);
                    }
                    else
                    {
                        LinkButton btn = new LinkButton();
                        btn.Text = idx.Get<string>();

                        if (DataSource["Type"]["Properties"][idx.Name]["BelongsTo"].Get<bool>())
                            btn.CssClass = "mux-grid-belongs-to";
                        btn.Info
                            = DataSource["Type"]["Properties"][idx.Name]["IsList"].Get<bool>().ToString();

                        btn.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton ed = sender as LinkButton;
                                (ed.Parent.Parent as Label).CssClass = "mux-grid-selected";
                                string id = (ed.Parent.Parent as Label).Info;
                                if (!SelectedID.Equals("-1"))
                                {
                                    if (!id.Equals(SelectedID))
                                    {
                                        Label lblx = Selector.SelectFirst<Label>(this,
                                            delegate(Control idxCtrl)
                                            {
                                                return idxCtrl is Label && (idxCtrl as Label).Info == SelectedID.ToString();
                                            });
                                        if (lblx != null)
                                            lblx.CssClass = "";
                                    }
                                }
                                SelectedID = id;
                                string column = (ed.Parent as Label).Info;

                                Node n = new Node();

                                n["ID"].Value = GetId(id);
                                n["PropertyName"].Value = column;
                                n["IsList"].Value = bool.Parse(ed.Info);
                                n["FullTypeName"].Value = DataSource["FullTypeName"].Value;

                                RaiseSafeEvent(
                                    "DBAdmin.Form.ViewListOrComplexPropertyValue",
                                    n);
                            };
                        l.Controls.Add(btn);
                    }
                }
                else
                {
                    if (DataSource["Type"]["Properties"][idx.Name].Contains("ReadOnly") &&
                        DataSource["Type"]["Properties"][idx.Name]["ReadOnly"].Get<bool>())
                    {
                        l.CssClass += "read-only";

                        string txt = idx.Get<string>() ?? "";

                        Label ll = new Label();

                        if (idx.Contains("ToolTip"))
                            ll.ToolTip = idx["ToolTip"].Get<string>();
                        else
                            ll.ToolTip = txt; // Setting ToolTip before we shrink text ...

                        if (DataSource["Type"]["Properties"][idx.Name].Contains("MaxLength"))
                        {
                            int maxLength = DataSource["Type"]["Properties"][idx.Name]["MaxLength"].Get<int>();
                            if (txt.Length > maxLength)
                            {
                                txt = txt.Substring(0, maxLength) + "...";
                            }
                        }
                        ll.Text = txt;
                        l.Controls.Add(ll);
                    }
                    else
                    {
                        string ctrlType = typeof(TextAreaEdit).FullName;

                        if (DataSource["Type"]["Properties"][idx.Name].Contains("ControlType"))
                        {
                            ctrlType = DataSource["Type"]["Properties"][idx.Name]["ControlType"].Get<string>();
                        }
                        switch (ctrlType)
                        {
                            case "Magix.Brix.Components.TextAreaEdit":
                                {
                                    TextAreaEdit edit = new TextAreaEdit();
                                    edit.ToolTip = idx.Get<string>();

                                    if (DataSource["Type"]["Properties"][idx.Name].Contains("MaxLength"))
                                    {
                                        int maxLength = DataSource["Type"]["Properties"][idx.Name]["MaxLength"].Get<int>();
                                        edit.TextLength = maxLength;
                                    }
                                    else
                                        edit.TextLength = 20;

                                    edit.DisplayTextBox +=
                                        delegate(object sender, EventArgs e)
                                        {
                                            TextAreaEdit ed = sender as TextAreaEdit;
                                            string id = (ed.Parent.Parent as Label).Info;
                                            (ed.Parent.Parent as Label).CssClass = "mux-grid-selected";
                                            if (!SelectedID.Equals("-1"))
                                            {
                                                if (!id.Equals(SelectedID))
                                                {
                                                    Label l2 = Selector.SelectFirst<Label>(this,
                                                        delegate(Control idxCtrl)
                                                        {
                                                            return idxCtrl is Label && (idxCtrl as Label).Info == SelectedID.ToString();
                                                        });
                                                    if (l2 != null)
                                                        l2.CssClass = "";
                                                }
                                            }
                                            SelectedID = id;
                                        };
                                    edit.TextChanged +=
                                        delegate(object sender, EventArgs e)
                                        {
                                            TextAreaEdit ed = sender as TextAreaEdit;
                                            string id = (ed.Parent.Parent as Label).Info;
                                            string column = (ed.Parent as Label).Info;

                                            Node n = new Node();

                                            n["ID"].Value = GetId(id);
                                            n["PropertyName"].Value = column;
                                            n["NewValue"].Value = ed.Text;
                                            n["FullTypeName"].Value = DataSource["FullTypeName"].Value;

                                            RaiseSafeEvent(
                                                DataSource.Contains("ChangeSimplePropertyValue") ?
                                                    DataSource["ChangeSimplePropertyValue"].Get<string>() :
                                                    "DBAdmin.Data.ChangeSimplePropertyValue",
                                                n);

                                        };
                                    edit.Text = idx.Get<string>();
                                    l.Controls.Add(edit);
                                } break;
                            case "Magix.UX.Widgets.InPlaceEdit":
                                {
                                    InPlaceEdit edit = new InPlaceEdit();
                                    edit.ToolTip = idx.Get<string>();

                                    edit.TextChanged +=
                                        delegate(object sender, EventArgs e)
                                        {
                                            InPlaceEdit ed = sender as InPlaceEdit;
                                            string id = (ed.Parent.Parent as Label).Info;
                                            string column = (ed.Parent as Label).Info;

                                            Node n = new Node();

                                            n["ID"].Value = GetId(id);
                                            n["PropertyName"].Value = column;
                                            n["NewValue"].Value = ed.Text;
                                            n["FullTypeName"].Value = DataSource["FullTypeName"].Value;

                                            RaiseSafeEvent(
                                                DataSource.Contains("ChangeSimplePropertyValue") ?
                                                    DataSource["ChangeSimplePropertyValue"].Get<string>() :
                                                    "DBAdmin.Data.ChangeSimplePropertyValue",
                                                n);

                                        };
                                    edit.Text = idx.Get<string>();
                                    l.Controls.Add(edit);
                                } break;
                        }
                    }
                }
                row.Controls.Add(l);
            }
            return row;
        }

        private object GetId(string p)
        {
            int tmp = 0;
            if (int.TryParse(p, out tmp))
                return tmp;
            return p;
        }
    }
}
