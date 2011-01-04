/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.UX.Effects;
using Magix.UX.Widgets;
using Magix.UX.Aspects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.Brix.Components.ActiveTypes;
using System.Web.UI.HtmlControls;
using Magix.UX;

namespace Magix.Brix.Components.ActiveModules.DBAdmin
{
    public abstract class DataBindableListControl : NestedDynamic, IModule
    {
        private LinkButton rc;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Remove Columns button
            rc = new LinkButton();
            rc.ID = "rc";
            rc.Click += rc_Click;
            rc.ToolTip = "Configure visible columns...";
            rc.Text = "&nbsp;";
            Controls.Add(rc);
        }

        protected void rc_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                node["Columns"][idx.Name]["Name"].Value = idx["Name"].Get<string>();
                node["Columns"][idx.Name]["TypeName"].Value = idx["TypeName"].Get<string>();
            }
            node["FullTypeName"].Value = DataSource["FullTypeName"].Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.ConfigureColumns",
                node);
        }

        protected int Start
        {
            get { return DataSource["Start"].Get<int>(); }
            set { DataSource["Start"].Value = value; ; }
        }

        protected int End
        {
            get { return DataSource["End"].Get<int>(); }
            set { DataSource["End"].Value = value; ; }
        }

        protected int Count
        {
            get { return DataSource["Objects"].Count; }
        }

        protected int MaxItems
        {
            get { return Settings.Instance.Get("DBAdmin.MaxItemsToShow", 20); }
        }

        protected virtual void UpdateCaption()
        {
            (Parent.Parent.Parent as Window).Caption = string.Format(
@"{0}-{1}/{2}({4}) of {3}",
                Start,
                End,
                TotalCount,
                TypeName,DataSource["TotalTypeCount"].Get<int>());
        }

        protected Label CreateTable()
        {
            bool green = true;
            string fullTypeName = FullTypeName;
            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                if (!Settings.Instance.Get(
                    "DBAdmin.VisibleColumns." +
                    fullTypeName + ":" + idx["Name"].Value,
                    true))
                {
                    green = false;
                    break;
                }
            }
            if (green)
            {
                rc.CssClass = "window-left-buttons window-remove-columns";
                rc.ToolTip = "Remove columns";
            }
            else
            {
                rc.CssClass = "window-left-buttons window-restore-columns";
                rc.ToolTip = "Add removed columns, or remove more columns";
            }
            Label table = new Label();
            table.Tag = "table";
            table.CssClass = "viewObjects";
            return table;
        }

        protected void FilterMethod(object sender, EventArgs e)
        {
            Node node = new Node();
            node["PropertyName"].Value = (sender as LinkButton).Info.Split('|')[0];
            node["FullTypeName"].Value = FullTypeName;
            node["PropertyTypeName"].Value = (sender as LinkButton).Info.Split('|')[1];
            node["ParentFullType"].Value = ParentFullType;
            node["TypeName"].Value = TypeName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.GetFilterForColumn",
                node);
        }

        protected Label CreateHeader(Control table)
        {
            Label row = new Label();
            row.Tag = "tr";
            row.CssClass = "header";
            if (DataSource["IsSelect"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                cS.Text = "Select";
                row.Controls.Add(cS);
            }
            if (DataSource["IsRemove"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                cS.Text = "Remove";
                row.Controls.Add(cS);
            }
            if (DataSource["IsDelete"].Get<bool>())
            {
                Label cS = new Label();
                cS.Tag = "td";
                cS.Text = "Delete";
                row.Controls.Add(cS);
            }
            Label cId = new Label();
            if (DataSource["IsFilter"].Get<bool>())
            {
                LinkButton b = new LinkButton();
                b.Text = "ID";
                string filterString = Settings.Instance.Get(FullTypeName + ":ID", "");
                b.ToolTip = filterString.Replace("|", " on ");
                b.CssClass = 
                    string.IsNullOrEmpty(
                        filterString) ? 
                        "" : 
                        "filtered";
                b.Click += FilterMethod;
                b.Info = "ID|Int32";
                cId.Controls.Add(b);
            }
            else
            {
                cId.Text = "ID";
            }
            cId.Tag = "td";
            row.Controls.Add(cId);
            string fullTypeName = DataSource["FullTypeName"].Get<string>();
            foreach (Node idx in DataSource["Type"]["Properties"])
            {
                string propertyName = idx["Name"].Get<string>();
                string idxSettingVisibility = 
                    "DBAdmin.VisibleColumns." + fullTypeName + ":" + propertyName;
                if (!GetVisibilityForColumn(idxSettingVisibility))
                    continue;
                bool isList = idx["IsList"].Get<bool>();
                string typeName = idx["TypeName"].Get<string>();
                bool belongsTo = idx["BelongsTo"].Get<bool>();
                bool isOwner = idx["IsOwner"].Get<bool>();
                string relationName = idx["RelationName"].Get<string>();
                HtmlTableCell cell = new HtmlTableCell();
                bool shouldAddFilter = false;
                switch (typeName)
                {
                    case "String":
                    case "Int32":
                    case "Boolean":
                    case "Decimal":
                    case "DateTime":
                        shouldAddFilter = true;
                        break;
                }
                if (shouldAddFilter && DataSource["IsFilter"].Get<bool>())
                {
                    LinkButton b = new LinkButton();
                    string filterString = Settings.Instance.Get(FullTypeName + ":" + propertyName, "");
                    b.ToolTip = filterString.Replace("|", " on ");
                    b.Info = propertyName + "|" + typeName;
                    b.Click += FilterMethod;
                    b.CssClass =
                        string.IsNullOrEmpty(
                            Settings.Instance.Get(
                                FullTypeName + 
                                ":" + 
                                propertyName, "")) ?
                            "" :
                            "filtered";
                    b.Text = string.Format(
    @"{0} ({1}{2}{3}{4})",
                        propertyName,
                        typeName,
                        ((!isOwner) ? " IsNotOwner" : ""),
                        (belongsTo ? " BelongsTo" : ""),
                        !string.IsNullOrEmpty(relationName) ? (" " + relationName) : "");
                    cell.Controls.Add(b);
                }
                else
                {
                    cell.InnerHtml = string.Format(
    @"{0} ({1}{2}{3}{4})",
                        propertyName,
                        typeName,
                        ((!isOwner) ? " IsNotOwner" : ""),
                        (belongsTo ? " BelongsTo" : ""),
                        !string.IsNullOrEmpty(relationName) ? (" " + relationName) : "");
                }
                row.Controls.Add(cell);
            }
            return row;
        }

        private bool GetVisibilityForColumn(string idxSettingVisibility)
        {
            if (ViewState[idxSettingVisibility] != null)
                return (bool)ViewState[idxSettingVisibility];
            bool value = Settings.Instance.Get(idxSettingVisibility, true);
            ViewState[idxSettingVisibility] = value;
            return value;
        }

        protected int SelectedItem
        {
            get { return ViewState["SelectedItem"] == null ? -1 : (int)ViewState["SelectedItem"]; }
            set { ViewState["SelectedItem"] = value; }
        }


        protected void CreateCells(Control tbl)
        {
            foreach (Node idxObj in DataSource["Objects"])
            {
                Label row = new Label();
                row.Tag = "tr";
                if (idxObj["ID"].Get<int>() == SelectedItem)
                {
                    row.CssClass = "grid-selected";
                    row.Info = "sel";
                }
                if (DataSource["IsSelect"].Get<bool>())
                {
                    Label cS = new Label();
                    cS.Tag = "td";
                    LinkButton btn = new LinkButton();
                    btn.Text = "Select";
                    btn.Info = idxObj["ID"].Value.ToString();
                    btn.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                Node node = new Node();
                                node["IsListAppend"].Value = DataSource["IsListAppend"].Value;
                                node["ParentID"].Value = ParentID;
                                node["ParentPropertyName"].Value = ParentPropertyName;
                                node["ParentType"].Value = ParentType;
                                node["NewObjectID"].Value = int.Parse((sender as LinkButton).Info);
                                node["NewObjectType"].Value = DataSource["FullTypeName"].Value;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin." +
                                        DataSource["SelectEventToFire"].Get<string>(),
                                    node);
                                int closingWindowID = int.Parse((Parent.Parent.Parent as Window).ID.Replace("wd", ""));
                                if (closingWindowID != 0)
                                {
                                    int otherClosingWindowID = closingWindowID - 1;
                                    string idOfOtherClosingWindow =
                                        (Parent.Parent.Parent as Window).ID.Replace(
                                            "wd" + closingWindowID,
                                            "wd" + otherClosingWindowID);
                                    Node node2 = new Node();
                                    node2["WindowID"].Value = idOfOtherClosingWindow;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "DBAdmin.InstanceWasSelected",
                                        node2);
                                    (Parent.Parent.Parent as Window).CloseWindow();
                                }
                            }
                            catch (Exception err)
                            {
                                Node node2 = new Node();
                                while (err.InnerException != null)
                                    err = err.InnerException;
                                node2["Message"].Value = err.Message;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "ShowMessage",
                                    node2);
                            }
                        };
                    cS.Controls.Add(btn);
                    row.Controls.Add(cS);
                }
                if (DataSource["IsRemove"].Get<bool>())
                {
                    Label cS = new Label();
                    cS.Tag = "td";
                    LinkButton btn = new LinkButton();
                    btn.Text = "Remove";
                    btn.Info = idxObj["ID"].Value.ToString();
                    btn.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                Node node = new Node();
                                node["ParentID"].Value = ParentID;
                                node["ParentPropertyName"].Value = ParentPropertyName;
                                node["ParentType"].Value = ParentFullType;
                                node["ObjectToRemoveID"].Value = int.Parse((sender as LinkButton).Info);
                                node["ObjectToRemoveType"].Value = DataSource["FullTypeName"].Value;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin.ComplexInstanceRemoved",
                                    node);
                                ReDataBind();
                            }
                            catch (Exception err)
                            {
                                Node node2 = new Node();
                                while (err.InnerException != null)
                                    err = err.InnerException;
                                node2["Message"].Value = err.Message;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "ShowMessage",
                                    node2);
                            }
                        };
                    cS.Controls.Add(btn);
                    row.Controls.Add(cS);
                }
                if (DataSource["IsDelete"].Get<bool>())
                {
                    Label cS = new Label();
                    cS.Tag = "td";
                    LinkButton btn = new LinkButton();
                    btn.Text = "Delete";
                    btn.Info = idxObj["ID"].Value.ToString();
                    btn.Click +=
                        delegate(object sender, EventArgs e)
                        {
                            try
                            {
                                Node node = new Node();
                                node["ObjectToDeleteID"].Value =
                                    int.Parse((sender as LinkButton).Info);
                                node["ObjectToDeleteType"].Value = DataSource["FullTypeName"].Value;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "DBAdmin.ComplexInstanceDeleted",
                                    node);
                                ReDataBind();

                            }
                            catch (Exception err)
                            {
                                Node node2 = new Node();
                                while (err.InnerException != null)
                                    err = err.InnerException;
                                node2["Message"].Value = err.Message;
                                ActiveEvents.Instance.RaiseActiveEvent(
                                    this,
                                    "ShowMessage",
                                    node2);
                            }
                        };
                    cS.Controls.Add(btn);
                    row.Controls.Add(cS);
                }
                int id = idxObj["ID"].Get<int>();
                Label idC = new Label();
                idC.Tag = "td";
                idC.Text = id.ToString();
                row.Controls.Add(idC);
                string fullTypeName = DataSource["FullTypeName"].Get<string>();
                foreach (Node idxProp in idxObj["Properties"])
                {
                    string propertyName = idxProp["PropertyName"].Get<string>();
                    string idxSettingVisibility =
                        "DBAdmin.VisibleColumns." + fullTypeName + ":" + propertyName;
                    if (!GetVisibilityForColumn(idxSettingVisibility))
                        continue;
                    Label c = new Label();
                    c.Tag = "td";
                    if (idxProp["IsList"].Get<bool>())
                    {
                        LinkButton b = new LinkButton();
                        b.Text = idxProp["Value"].Get<string>();
                        b.Info = 
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" + 
                            idxProp["PropertyName"].Get<string>() + "|" +
                            idxProp["BelongsTo"].Value;
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                try
                                {
                                    LinkButton bt = sender as LinkButton;
                                    string[] infos = bt.Info.Split('|');
                                    string parentPropertyName = infos[1];
                                    int parentId = int.Parse(infos[0]);
                                    Label l = Selector.SelectFirst<Label>(this,
                                        delegate(Control idx)
                                        {
                                            return idx.ID == "sel";
                                        });
                                    if (l != null)
                                        l.CssClass = "";
                                    (bt.Parent as Label).CssClass = "grid-selected-cell";
                                    (bt.Parent.Parent as Label).CssClass = "grid-selected";
                                    SelectedItem = parentId;
                                    bool belongsTo = bool.Parse(infos[2]);
                                    string parentType = DataSource["FullTypeName"].Get<string>();
                                    Node node = new Node();
                                    node["FullTypeName"].Value = parentType;
                                    node["ID"].Value = parentId;
                                    node["PropertyName"].Value = parentPropertyName;
                                    node["BelongsTo"].Value = belongsTo;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "DBAdmin.ViewList",
                                        node);

                                }
                                catch (Exception err)
                                {
                                    Node node2 = new Node();
                                    while (err.InnerException != null)
                                        err = err.InnerException;
                                    node2["Message"].Value = err.Message;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "ShowMessage",
                                        node2);
                                }
                            };
                        c.Controls.Add(b);
                    }
                    else if (idxProp["IsComplex"].Get<bool>())
                    {
                        LinkButton b = new LinkButton();
                        b.Text = idxProp["Value"].Get<string>();
                        b.Info =
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" +
                            idxProp["PropertyName"].Get<string>() + "|" +
                            idxProp["BelongsTo"].Value;
                        b.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                try
                                {
                                    LinkButton bt = sender as LinkButton;
                                    string[] infos = bt.Info.Split('|');
                                    string parentPropertyName = infos[1];
                                    int parentId = int.Parse(infos[0]);
                                    (bt.Parent as Label).CssClass = "grid-selected-cell";
                                    Label l = Selector.SelectFirst<Label>(this,
                                        delegate(Control idx)
                                        {
                                            return idx.ID == "sel";
                                        });
                                    if (l != null)
                                        l.CssClass = "";
                                    (bt.Parent.Parent as Label).CssClass = "grid-selected";
                                    SelectedItem = parentId;
                                    string parentType = DataSource["FullTypeName"].Get<string>();
                                    bool belongsTo = bool.Parse(infos[2]);
                                    Node node = new Node();
                                    node["FullTypeName"].Value = parentType;
                                    node["ID"].Value = parentId;
                                    node["PropertyName"].Value = parentPropertyName;
                                    node["BelongsTo"].Value = belongsTo;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "DBAdmin.ViewSingleInstance",
                                        node);

                                }
                                catch (Exception err)
                                {
                                    Node node2 = new Node();
                                    while (err.InnerException != null)
                                        err = err.InnerException;
                                    node2["Message"].Value = err.Message;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "ShowMessage",
                                        node2);
                                }
                            };
                        c.Controls.Add(b);
                    }
                    else
                    {
                        TextAreaEdit ed = new TextAreaEdit();
                        ed.Text = idxProp["Value"].Get<string>();
                        ed.Info =
                            idxProp.Parent.Parent["ID"].Value.ToString() + "|" +
                            idxProp["PropertyName"].Get<string>();
                        ed.TextChanged +=
                            delegate(object sender, EventArgs e)
                            {
                                try
                                {
                                    TextAreaEdit edit = sender as TextAreaEdit;
                                    Node node = new Node();
                                    string[] infos = edit.Info.Split('|');
                                    string parentPropertyName = infos[1];
                                    int parentId = int.Parse(infos[0]);
                                    node["ID"].Value = parentId;
                                    node["FullTypeName"].Value = FullTypeName;
                                    node["PropertyName"].Value = parentPropertyName;
                                    node["Value"].Value = edit.Text;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "DBAdmin.ChangeValue",
                                        node);

                                }
                                catch (Exception err)
                                {
                                    Node node2 = new Node();
                                    while (err.InnerException != null)
                                        err = err.InnerException;
                                    node2["Message"].Value = err.Message;
                                    ActiveEvents.Instance.RaiseActiveEvent(
                                        this,
                                        "ShowMessage",
                                        node2);
                                }
                            };
                        c.Controls.Add(ed);
                    }
                    row.Controls.Add(c);
                }
                tbl.Controls.Add(row);
            }
        }
    }
}
