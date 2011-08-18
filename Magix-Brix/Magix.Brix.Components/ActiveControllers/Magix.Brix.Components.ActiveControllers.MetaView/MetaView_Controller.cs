/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaViews;
using Magix.Brix.Data;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using System.Collections.Generic;
using Magix.Brix.Components.ActiveTypes.MetaTypes;

namespace Magix.Brix.Components.ActiveControllers.MetaViews
{
    /**
     * Level2: Contains helper logic for viewing and maintaing MetaViews, and related subjects. MetaViews are the
     * foundation for the whole viewing parts of the Meta Application system. A MetaView is imperativ for
     * both being able to collect new data and also for viewing existing data. The MetaView defines which
     * parts of the object you can see at any time too, which means you can use it to filter access according
     * to which Grid the user is having access to, for instance. This controller contains logic for editing
     * and maintaining MetaViews, plus also direct usage of MetaViews
     */
    [ActiveController]
    public class MetaView_Controller : ActiveController
    {
        /**
         * Level2: Will return one item back to caller which hopefully will function as the basis
         * of loading the ViewMetaViews logic
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Caption"].Value = "Meta Views ...";
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Event"]["Name"].Value = "Magix.MetaView.ViewMetaViews";
        }

        /**
         * Level2: Will show the given MetaView ['MetaViewName'] in MultiView mode. 
         * As in, the end user will see a Grid of all MetaObjects of the TypeName of the MetaView
         */
        [ActiveEvent(Name = "Magix.MetaType.ViewMetaViewMultiMode")]
        protected void Magix_MetaType_ViewMetaViewMultiMode(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            // To help our Publishing Module to refresh ...
            // TODO: Refactor ...
            node["OriginalWebPartID"].Value = e.Params["OriginalWebPartID"].Get<int>();

            if (e.Params.Contains("NoIdColumn"))
                node["NoIdColumn"].Value = e.Params["NoIdColumn"].Value;

            if (e.Params.Contains("IsDelete"))
                node["IsDelete"].Value = e.Params["IsDelete"].Value;

            if (e.Params.Contains("IsInlineEdit"))
                node["IsInlineEdit"].Value = e.Params["IsInlineEdit"].Value;

            if (e.Params.Contains("Container"))
                node["Container"].Value = e.Params["Container"].Value;

            node["FreezeContainer"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject).FullName + "-META";

            if (e.Params.Contains("WhiteListColumns"))
            {
                node["WhiteListColumns"] = e.Params["WhiteListColumns"];
            }

            if (e.Params.Contains("Type"))
            {
                node["Type"] = e.Params["Type"];
            }

            if (!node.Contains("Container"))
            {
                Node xx = new Node();

                xx["OriginalWebPartID"].Value = e.Params["OriginalWebPartID"].Get<int>();

                RaiseEvent(
                    "Magix.MetaView.GetWebPartsContainer",
                    xx);
                node["Container"].Value = xx["ID"].Get<string>();
            }

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditMetaObject";
            node["DeleteColumnEvent"].Value = "Magix.Meta.DeleteMetaObject";
            node["ChangeSimplePropertyValue"].Value = "Magix.Meta.ChangeMetaObjectValue";

            node["IsCreate"].Value = false;

            node["ReuseNode"].Value = true;
            if (e.Params.Contains("MetaViewName"))
                node["MetaViewName"].Value = e.Params["MetaViewName"].Value;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will show a Grid with all the MetaViews to the end user
         */
        [ActiveEvent(Name = "Magix.MetaView.ViewMetaViews")]
        protected void Magix_MetaView_ViewMetaViews(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "edit-views";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 6;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["TypeName"]["MaxLength"].Value = 50;
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 3;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaView.EditMetaView";
            node["CreateEventName"].Value = "Magix.MetaView.CreateMetaViewAndEdit";
            node["DeleteColumnEvent"].Value = "Magix.MetaView.DeleteMetaView";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.Meta.GetCopyMetaViewTemplateColumn";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        // TODO: Get Generic SelectList Control
        /**
         * Level3: Will return a SelectList with all the MetaViews back to caller
         */
        [ActiveEvent(Name = "Magix.MetaView.MetaView_Single.GetTemplateColumnSelectView")]
        protected void Magix_MetaView_MetaView_Single_GetTemplateColumnSelectView(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a MetaView ...", "");
            ls.Items.Add(item);

            foreach (MetaView idx in MetaView.Select())
            {
                ListItem it = new ListItem(idx.Name, idx.TypeName + idx.Name.ToString());
                if (idx.Name == e.Params["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }
        }

        /**
         * Level3: Will return a SelectList with all the MetaViews back to caller
         */
        [ActiveEvent(Name = "Magix.MetaView.MetaView_Multiple.GetTemplateColumnSelectView")]
        protected void Magix_MetaView_MetaView_Multiple_GetTemplateColumnSelectView(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a MetaView ...", "");
            ls.Items.Add(item);

            foreach (MetaView idx in MetaView.Select())
            {
                ListItem it = new ListItem(idx.Name, idx.TypeName + idx.Name.ToString());
                if (idx.Name == e.Params["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }
        }

        /**
         * Level3: Creates a Copy MetaView LinkButton
         */
        [ActiveEvent(Name = "Magix.Meta.GetCopyMetaViewTemplateColumn")]
        protected void Magix_Meta_GetCopyMetaViewTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Creating our SelectList
            LinkButton ls = new LinkButton();
            ls.Text = "Copy";
            ls.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = id;
                    RaiseEvent(
                        "Magix.Meta.CopyMetaViewAndEditCopy",
                        node);
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Will copy [deep clone] the incoming 'ID' MetaView and return the new copy's
         * ID as 'NewID'
         */
        [ActiveEvent(Name = "Magix.Meta.CopyMetaView")]
        protected void Magix_Meta_CopyMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView a = MetaView.SelectByID(e.Params["ID"].Get<int>());
                MetaView clone = a.Copy();

                clone.Save();

                tr.Commit();
                e.Params["NewID"].Value = clone.ID;
            }
        }

        /**
         * Level2: Will copy [deep clone] the incoming 'ID' MetaView and edit it immediately
         */
        [ActiveEvent(Name = "Magix.Meta.CopyMetaViewAndEditCopy")]
        protected void Magix_Meta_CopyMetaViewAndEditCopy(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.Meta.CopyMetaView",
                e.Params);

            Node n = new Node();

            n["FullTypeName"].Value = typeof(MetaView).FullName;
            n["ID"].Value = e.Params["NewID"].Value;

            RaiseEvent(
                "DBAdmin.Grid.SetActiveRow",
                n);

            n = new Node();
            n["FullTypeName"].Value = typeof(MetaView).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);

            n = new Node();
            n["ID"].Value = e.Params["NewID"].Value;

            RaiseEvent(
                "Magix.MetaView.EditMetaView",
                n);
        }

        /**
         * Level2: Will ask the user if he really wish to delete the MetaView given through 'ID',
         * and if the user says yes, delete it
         */
        [ActiveEvent(Name = "Magix.MetaView.DeleteMetaView")]
        protected void Magix_MetaView_DeleteMetaView(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);
            Node node = e.Params;
            if (node == null)
            {
                node = new Node();
                node["ForcedSize"]["width"].Value = 550;
                node["WindowCssClass"].Value =
                    "mux-shaded mux-rounded push-5 down-2";
            }
            node["Caption"].Value = @"
Please confirm deletion of " + typeName + " with ID of " + id;
            node["Text"].Value = @"
<p>Are you sure you wish to delete this View? 
This View might be in use in several forms or other parts of your system. 
Deleting it may break these parts.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.MetaView.DeleteMetaView-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Level2: End user confirmed he wishes to delete the MetaView
         */
        [ActiveEvent(Name = "Magix.MetaView.DeleteMetaView-Confirmed")]
        protected void Magix_MetaView_DeleteMetaView_Confirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
            {
                // In case it's the one being edited ...
                Node n = new Node();

                n["Position"].Value = "content4";

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ClearControls",
                    n);

                RaiseEvent(
                    "DBAdmin.Common.ComplexInstanceDeletedConfirmed",
                    e.Params);
            }
        }

        /**
         * Level2: Will create a new MetaView with some default settings and return ID of new view 
         * as 'NewID'
         */
        [ActiveEvent(Name = "Magix.MetaView.CreateMetaView")]
        protected void Magix_MetaView_CreateMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView view = new MetaView();
                view.Name = "Default name";

                view.Save();

                tr.Commit();

                e.Params["NewID"].Value = view.ID;
            }
        }

        /**
         * Level2: Will create and Edit immediately a new MetaView
         */
        [ActiveEvent(Name = "Magix.MetaView.CreateMetaViewAndEdit")]
        protected void Magix_MetaView_CreateMetaViewAndEdit(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.MetaView.CreateMetaView",
                e.Params);

            Node node = new Node();
            node["ID"].Value = e.Params["NewID"].Value;

            RaiseEvent(
                "Magix.MetaView.EditMetaView",
                node);
        }

        /**
         * Level2: Will edit the given ['ID'] MetaView with its properties and some other
         * controls. Such as for instance a 'View WYSIWYG' button
         */
        [ActiveEvent(Name = "Magix.MetaView.EditMetaView")]
        protected void Magix_MetaView_EditMetaView(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("content6");

            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());
            ViewMetaViewDirectPropertiesGrid(m);
            ViewMetaViewINDirectPropertiesGrid(m);
            ViewWysiwygButton(m);
        }

        /*
         * Helper for above ...
         */
        private void ViewMetaViewDirectPropertiesGrid(MetaView m)
        {
            Node node = new Node();

            // MetaView Single View Editing ...
            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["ID"].Value = m.ID;

            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["TypeName"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["Container"].Value = "content4";
            node["MarginBottom"].Value = 10;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        /*
         * Helper for above ...
         */
        private void ViewMetaViewINDirectPropertiesGrid(MetaView m)
        {
            // Properties ...
            Node node = new Node();

            node["Width"].Value = 20;
            node["Padding"].Value = 4;
            node["Last"].Value = true;
            node["MarginBottom"].Value = 30;
            node["PullTop"].Value = 9;
            node["Container"].Value = "content5";

            node["PropertyName"].Value = "Properties";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(MetaView).FullName;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["ReadOnly"].Value = true;
            node["WhiteListColumns"]["ReadOnly"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Description"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Action"].Value = true;
            node["WhiteListColumns"]["Action"]["ForcedWidth"].Value = 2;

            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["ReadOnly"]["Header"].Value = "Read On.";
            node["Type"]["Properties"]["ReadOnly"]["TemplateColumnEvent"].Value = "Magix.DataPlugins.GetTemplateColumns.CheckBox";
            node["Type"]["Properties"]["Description"]["Header"].Value = "Desc.";
            node["Type"]["Properties"]["Description"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Action"]["Header"].Value = "Action";
            node["Type"]["Properties"]["Action"]["TemplateColumnEvent"].Value = "Magix.MetaView.GetMetaViewActionTemplateColumn";

            node["ID"].Value = m.ID;
            node["NoIdColumn"].Value = true;
            node["ReUseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);
        }

        /*
         * Helper for above ...
         */
        private void ViewWysiwygButton(MetaView m)
        {
            // Wysiwyg button ...
            Node node = new Node();

            node["Text"].Value = "Preview ...";
            node["ButtonCssClass"].Value = "span-4";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.MetaView.LoadWysiwyg";
            node["Event"]["ID"].Value = m.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /**
         * Level3: Will return a clickable and drop-down-able Panel that the end user can click
         * to append and remove events that triggers the MetaView Property if clicked
         */
        [ActiveEvent(Name = "Magix.MetaView.GetMetaViewActionTemplateColumn")]
        protected void Magix_MetaView_GetMetaViewActionTemplateColumn(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();

            MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());

            Panel pnl = new Panel();
            pnl.CssClass = "action-wrapper";
            pnl.ToolTip = "Click to append an Action to this Property of the Form. Effectively rendering the contro a button, that'll raise whatever action(s) you choose to give it ...";
            pnl.Click +=
                delegate
                {
                    pnl.CssClass += " action-wrapper-hover";

                    Node node = new Node();
                    node["ID"].Value = p.ID;

                    RaiseEvent(
                        "Magix.MetaView.AppendAction",
                        node);

                    node = new Node();
                    node["ID"].Value = p.ID;
                    node["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

                    RaiseEvent(
                        "DBAdmin.Grid.SetActiveRow",
                        node);
                };

            if (!string.IsNullOrEmpty(p.Action))
            {
                pnl.CssClass += " has-action";
            }
            else
            {
                pnl.CssClass += " has-no-action";
            }

            Panel grow = new Panel();
            grow.CssClass = "grower";
            grow.ToolTip = "These are the Actions you've already assigned to this Property. Click the 'x' to remove any of these actions.";
            grow.Click +=
                delegate
                {
                    pnl.CssClass = pnl.CssClass.Replace(" action-wrapper-hover", "");
                };

            string[] actions = 
                (p.Action ?? "")
                .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idxA in actions)
            {
                string actionName = idxA;
                if (actionName.Contains("("))
                    actionName = actionName.Substring(0, actionName.IndexOf('('));

                LinkButton btn = new LinkButton();
                btn.Text = "X";
                btn.ToolTip = "Click to remove Action from the property";
                btn.CssClass = "clear-left span-1 delete-action";
                btn.Click +=
                    delegate
                    {
                        ActiveEvents.Instance.RaiseClearControls("content6");
                        using (Transaction tr = Adapter.Instance.BeginTransaction())
                        {
                            string na = p.Action.Substring(0, p.Action.IndexOf(actionName));
                            na += p.Action.Substring(p.Action.IndexOf(actionName) + actionName.Length);
                            p.Action = na.Replace("||", "|").Trim('|').Trim();
                            p.Save();

                            tr.Commit();
                        }

                        Node n = new Node();
                        n["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

                        RaiseEvent(
                            "Magix.Core.UpdateGrids",
                            n);

                    };
                grow.Controls.Add(btn);

                Label l = new Label();
                l.Text = actionName;
                l.ToolTip = "Name of Action";
                l.Tag = "div";
                l.CssClass = "span-3 last";
                grow.Controls.Add(l);
            }

            Button bt = new Button();
            bt.Text = "Close";
            bt.ToolTip = "Close this Action Popup Window";
            bt.CssClass = "bottom-right span-3";
            bt.Click +=
                delegate
                {
                    pnl.CssClass = pnl.CssClass.Replace(" action-wrapper-hover", "");
                    ActiveEvents.Instance.RaiseClearControls("content6");
                };
            grow.Controls.Add(bt);

            pnl.Controls.Add(grow);

            e.Params["Control"].Value = pnl;
        }

        /**
         * Level2: Will show a Search box, from which the end user can search for a specific action
         * to append to the list of actions already associated with the MetaView Property
         */
        [ActiveEvent(Name = "Magix.MetaView.AppendAction")]
        protected void Magix_MetaView_AppendAction(object sender, ActiveEventArgs e)
        {
            e.Params["SelectEvent"].Value = "Magix.MetaView.ActionWasChosenForAppending";
            e.Params["ParentID"].Value = e.Params["ID"].Value;

            RaiseEvent(
                "Magix.MetaActions.SearchActions",
                e.Params);
        }

        /**
         * Level2: Will append the specific 'ActionName' into the 'ParentID' MetaView Property and
         * call for an update of the grids
         */
        [ActiveEvent(Name = "Magix.MetaView.ActionWasChosenForAppending")]
        protected void Magix_MetaView_ActionWasChosenForAppending(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("content6");

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ParentID"].Get<int>());
                if (!string.IsNullOrEmpty(p.Action))
                    p.Action += "|";
                p.Action += e.Params["ActionName"].Get<string>();
                p.Save();

                tr.Commit();
            }

            Node n = new Node();
            n["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);
        }

        // TODO: Implement checking against SPECIFIC MetaView before we 'flush containers' since
        // user might very well delete _another_ view than the one being edited ...
        /**
         * Level2: Handled to make sure we clear content4 and out if a MetaView s deleted.
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        // TODO: Implement Wrapper Action around this bugger ...
        /**
         * Level2: Will show the 'MetaViewName' MetaView within the 'current container'
         */
        [ActiveEvent(Name = "Magix.MetaType.ShowMetaViewMultipleInCurrentContainer")]
        protected void Magix_MetaType_ShowMetaViewMultipleInCurrentContainer(object sender, ActiveEventArgs e)
        {
            MetaView view = MetaView.SelectFirst(
                Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            // Defaults ...
            e.Params["IsDelete"].Value = false;
            e.Params["NoIdColumn"].Value = true;
            e.Params["IsFilter"].Value = false;

            e.Params["MetaViewTypeName"].Value = view.TypeName;
            e.Params["MetaViewName"].Value = view.Name;

            RaiseEvent(
                "Magix.MetaType.ViewMetaViewMultiMode",
                e.Params);
        }

        /**
         * Level2: Overridden to handle MetaView 'dynamically displayed'. Meaning in "-META" 'mode',
         * front-web style
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META" &&
                e.Params.Contains("MetaViewName"))
            {
                MetaView view = MetaView.SelectFirst(
                    Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

                foreach (MetaView.MetaViewProperty idx in view.Properties)
                {
                    if (idx.Name == ":Delete")
                    {
                        e.Params["IsDelete"].Value = true;
                        continue;
                    }
                    else if (idx.Name == ":Edit")
                    {
                        e.Params["NoIdColumn"].Value = false;
                        continue;
                    }
                    else if (idx.Name == ":Create")
                    {
                        e.Params["IsCreate"].Value = true;
                        continue;
                    }

                    string name = idx.Name;

                    if (name.Contains(":"))
                    {
                        string[] splits = name.Split(':');
                        name = splits[splits.Length - 1];
                        e.Params["Type"]["Properties"][name]["TemplateColumnEvent"].Value =
                            "Magix.MetaView.MetaView_Multiple_GetColonTemplateColumn";
                    }

                    if (!string.IsNullOrEmpty(idx.Action))
                    {
                        e.Params["WhiteListColumns"][name].Value = true;
                        e.Params["Type"]["Properties"][name]["ReadOnly"].Value = idx.ReadOnly;
                        e.Params["Type"]["Properties"][name]["Header"].Value = name;
                        e.Params["Type"]["Properties"][name]["NoFilter"].Value = true;
                        e.Params["Type"]["Properties"][name]["TemplateColumnEvent"].Value =
                            "Magix.MetaView.MultiViewActionItemTemplateColumn";
                    }

                    e.Params["WhiteListColumns"][name].Value = true;
                    e.Params["Type"]["Properties"][name]["ReadOnly"].Value = idx.ReadOnly;
                    e.Params["Type"]["Properties"][name]["Header"].Value = name;
                    e.Params["Type"]["Properties"][name]["NoFilter"].Value = true;
                }
            }
        }

        /**
         * Level2: Overridden to handle MetaView 'dynamically displayed'. Meaning in "-META" 'mode'
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.CreateObject")]
        protected void DBAdmin_DynamicType_CreateObject(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META")
            {
                MetaObject obj = new MetaObject();
                MetaView n = MetaView.SelectFirst(
                    Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));
                obj.TypeName = n.TypeName;
                obj.Save();
                e.Params["ID"].Value = obj.ID;
            }
        }

        /**
         * Level2: Overridden to handle MetaView 'dynamically displayed'. Meaning in "-META" 'mode'
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META" &&
                e.Params.Contains("MetaViewName"))
            {
                MetaView templ = MetaView.SelectFirst(
                    Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

                e.Params["SetCount"].Value = MetaObject.CountWhere(
                    Criteria.Eq("TypeName", templ.TypeName));

                e.Params["LockSetCount"].Value = true;

                foreach (MetaObject idxO in MetaObject.Select(
                    Criteria.Eq("TypeName", templ.TypeName),
                    Criteria.Range(
                        e.Params["Start"].Get<int>(),
                        e.Params["End"].Get<int>(),
                        "Created",
                        false)))
                {
                    e.Params["Objects"]["o-" + idxO.ID]["ID"].Value = idxO.ID;
                    foreach (MetaObject.Property idx in idxO.Values)
                    {
                        string propertyName = idx.Name ?? "";
                        if (propertyName.IndexOf(":") != -1)
                        {
                            string[] splits = propertyName.Split(':');
                            propertyName = splits[splits.Length - 1];
                        }
                        if (idxO.Values.Exists(
                            delegate(MetaObject.Property ixx)
                            {
                                return ixx.Name == propertyName;
                            }))
                            e.Params["Objects"]["o-" + idxO.ID]["Properties"][propertyName].Value = idx.Value;
                    }

                    // Looping through, 'touching' all the items with no values ...
                    foreach (MetaView.MetaViewProperty idx in templ.Properties.FindAll(
                        delegate(MetaView.MetaViewProperty idxI)
                        {
                            return !idxO.Values.Exists(
                                delegate(MetaObject.Property idxI2)
                                {
                                    return idxI2.Name == idxI.Name;
                                });
                        }
                        ))
                    {
                        e.Params["Objects"]["o-" + idxO.ID]["Properties"][idx.Name].Value = "";
                    }
                }
            }
        }

        /**
         * Level3: Will return a Button control to caller, upon which clicked, will raise the named actions in its
         * settings according to which MetaViewProperty it belongs to. Front-web stuff, basically a MetaView
         * form field with 'Actions' associated with it
         */
        [ActiveEvent(Name = "Magix.MetaView.MultiViewActionItemTemplateColumn")]
        protected void Magix_MetaView_MultiViewActionItemTemplateColumn(object sender, ActiveEventArgs e)
        {
            MetaView v = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            MetaView.MetaViewProperty p = v.Properties.Find(
                delegate(MetaView.MetaViewProperty idx)
                {
                    return idx.Name.Contains(":" + e.Params["Name"].Get<string>());
                });

            int id = e.Params["ID"].Get<int>();
            string text = e.Params["Name"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            int pageObj = e.Params["OriginalWebPartID"].Get<int>();
            string metaViewName = e.Params["MetaViewName"].Get<string>();

            LinkButton b = new LinkButton();
            b.Text = text;
            if (!string.IsNullOrEmpty(e.Params["Value"].Get<string>()))
            {
                b.CssClass += "has-value";
            }
            else
            {
                b.CssClass += "has-no-value";
            }
            b.Click +=
                delegate
                {
                    Node node = new Node();

                    node["ID"].Value = id;
                    node["Name"].Value = text;
                    node["Value"].Value = value;
                    node["OriginalWebPartID"].Value = pageObj;
                    node["MetaViewName"].Value = metaViewName;

                    RaiseEvent(
                        "Magix.MetaView.RunActionsForMetaViewProperty",
                        node);
                };
            e.Params["Control"].Value = b;
        }

        /**
         * Level2: Will run the Actions associated with the MetaViewProperty given through 'MetaViewName' [MetaView - Name],
         * 'Name' [of property within MetaObject] and expects to be raised from within a WebPart, since it
         * will pass along the 'current container' onwards
         */
        [ActiveEvent(Name = "Magix.MetaView.RunActionsForMetaViewProperty")]
        protected void Magix_MetaView_RunActionsForMetaViewProperty(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string name = e.Params["Name"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            int pageObj = e.Params["OriginalWebPartID"].Get<int>();

            MetaView v = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            MetaView.MetaViewProperty p = v.Properties.Find(
                delegate(MetaView.MetaViewProperty idx)
                {
                    return idx.Name == e.Params["Name"].Get<string>();
                });

            foreach (string idxS in p.Action.Split('|'))
            {
                Node node = new Node();

                node["ActionSenderName"].Value = name;
                node["Value"].Value = value;
                node["MetaViewName"].Value = v.Name;
                node["MetaViewTypeName"].Value = v.TypeName;
                node["ActionName"].Value = idxS;
                node["MetaObjectID"].Value = id;

                // Settings Event Specific Features ...
                node["ActionName"].Value = idxS;
                node["OriginalWebPartID"].Value = pageObj;

                RaiseEvent(
                    "Magix.MetaAction.RaiseAction",
                    node);
            }
        }

        /**
         *Level3:  Will create a control type depending upon the colon-prefix of the column. For instance, if given
         * date:When it will create a Calendar, putting the Value selected into the 'When' property.
         * If given select:xx.yy:zz it will create a select list, enumerating into
         * the ObjectTypes given with the Property de-referenced. E.g. select:Gender.Sex:Male-Female will enumerate
         * every 'Sex' value of every object of TypeName 'Gender' and put it into the property with
         * the name of 'Male-Female'. choice:Gender.Sex,Css:Male-Female would function identically, except
         * it also expects to find a Css property, whos value will be used as the CSS class for a small 
         * 'clickable enumerating' type of control which would allow for 'single-choice' selection of
         * status for instance. If you want to create plugin types for the MultiView Grid system, this
         * event is what you'd have to handle to inject your own Column types
         */
        [ActiveEvent(Name = "Magix.MetaView.MetaView_Multiple_GetColonTemplateColumn")]
        protected void Magix_MetaView_MetaView_Multiple_GetColonTemplateColumn(object sender, ActiveEventArgs e)
        {
            MetaView v = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            MetaView.MetaViewProperty p = v.Properties.Find(
                delegate(MetaView.MetaViewProperty idx)
                {
                    return idx.Name.Contains(":" + e.Params["Name"].Get<string>());
                });

            string typeOfControl = p.Name.Split(':')[0];

            int id = e.Params["ID"].Get<int>();

            switch (typeOfControl)
            {
                case "select":
                    CreateMetaView_MultiView_SelectList(id, p, e.Params);
                    break;
                case "date":
                    CreateMetaView_MultiView_Calendar(id, p, e.Params);
                    break;
                case "choice":
                    CreateMetaView_MultiView_ChoiceEnum(id, p, e.Params);
                    break;
                default:
                    // Assuming some other bugger will handle this guy ...
                    break;
            }
        }

        /*
         * Helper for above ...
         */
        private void CreateMetaView_MultiView_SelectList(int id, MetaView.MetaViewProperty p, Node node)
        {
            SelectList ls = new SelectList();

            ls.CssClass = "span-2 gridSelect";
            ls.Style[Styles.display] = "block";

            string typeProperty = p.Name.Split(':')[1];
            string type = typeProperty.Split('.')[0];
            string propertyName = typeProperty.Split('.')[1];
            string gridPropertyName = p.Name.Split(':')[2];

            ls.SelectedIndexChanged +=
                delegate
                {
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        MetaObject o = MetaObject.SelectByID(id);
                        MetaObject.Property val = o.Values.Find(
                            delegate(MetaObject.Property idxI)
                            {
                                return idxI.Name == gridPropertyName;
                            });
                        if (val == null)
                        {
                            val = new MetaObject.Property();
                            val.Name = gridPropertyName;
                            o.Values.Add(val);
                            o.Save();
                        }
                        val.Value = ls.SelectedItem.Value;
                        val.Save();

                        tr.Commit();
                    }
                };

            ListItem i = new ListItem();
            i.Value = "";
            i.Text = "Please Select ...";
            ls.Items.Add(i);

            foreach (MetaObject idx in Cache<IEnumerable<MetaObject>>(
                "Magix.MetaObjects.OfTypeName-" + type,
                delegate
                {
                    return MetaObject.Select(Criteria.Eq("TypeName", type));
                }))
            {
                ListItem it = new ListItem();
                MetaObject.Property val = idx.Values.Find(
                    delegate(MetaObject.Property idxI)
                    {
                        return idxI.Name == propertyName;
                    });
                it.Text = val.Value;
                it.Value = val.Value;
                if (val.Value == node["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }

            node["Control"].Value = ls;
        }

        /*
         * Helper for above ...
         */
        private void CreateMetaView_MultiView_Calendar(int id, MetaView.MetaViewProperty p, Node node)
        {
            string gridPropertyName = p.Name.Split(':')[1];

            MetaObject o = MetaObject.SelectByID(id);
            string propertyName = node["Name"].Get<string>();
            MetaObject.Property val = o.Values.Find(
                delegate(MetaObject.Property idx)
                {
                    return idx.Name == propertyName;
                });

            Panel panel = new Panel();
            panel.ID = "pnl" + id;
            panel.CssClass = "calendar-wrapper";

            Calendar c = new Calendar();
            c.ID = "cal" + id;
            c.CssClass += " mux-shaded mux-rounded";
            if (val != null &&
                !string.IsNullOrEmpty(val.Value))
            {
                c.Value = DateTime.ParseExact(val.Value, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            c.Style[Styles.display] = "none";
            c.Style[Styles.position] = "absolute";
            c.Style[Styles.top] = "0";
            c.Style[Styles.left] = "0";
            c.Style[Styles.zIndex] = "100";
            panel.Controls.Add(c);

            LinkButton but = new LinkButton();

            c.DateSelected +=
                delegate(object sender2, EventArgs e2)
                {
                    new EffectFadeOut(c, 250).Render();
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        if (val == null)
                        {
                            val = new MetaObject.Property();
                            val.Name = propertyName;
                            o.Values.Add(val);
                            o.Save();
                        }
                        val.Value = c.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

                        val.Save();

                        tr.Commit();

                        but.Text = DateTime.ParseExact(
                            val.Value, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture)
                            .ToString("ddd d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                };

            but.Text = "???";
            if (val != null &&
                !string.IsNullOrEmpty(val.Value))
                but.Text = DateTime.ParseExact(
                    val.Value, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture)
                    .ToString("ddd d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
            but.ID = "but" + id;
            but.Click +=
                delegate
                {
                    new EffectFadeIn(c, 250).Render();
                };
            panel.Controls.Add(but);

            node["Control"].Value = panel;
        }

        /*
         * Helper for above ...
         */
        private void CreateMetaView_MultiView_ChoiceEnum(int id, MetaView.MetaViewProperty p, Node node)
        {
            string typeProperty = p.Name.Split(':')[1];
            string type = typeProperty.Split('.')[0];
            string propertyName = typeProperty.Split('.')[1].Split(',')[0];
            string propertyCss = typeProperty.Split('.')[1].Split(',')[1];
            string gridPropertyName = p.Name.Split(':')[2];

            LinkButton b = new LinkButton();

            MetaObject o = MetaObject.SelectByID(id);
            MetaObject.Property val = o.Values.Find(
                delegate(MetaObject.Property idxI)
                {
                    return idxI.Name == gridPropertyName;
                });
            MetaObject o4 = null;
            string cssClass = "status-unknown";
            if (val != null)
            {
                foreach (MetaObject idx in Cache<IEnumerable<MetaObject>>(
                    "Magix.MetaObjects.OfTypeName-" + type,
                    delegate
                    {
                        return MetaObject.Select(Criteria.Eq("TypeName", type));
                    }))
                {
                    MetaObject.Property val2 = idx.Values.Find(
                        delegate(MetaObject.Property idxI)
                        {
                            return idxI.Name == propertyName && idxI.Value == val.Value;
                        });
                    if (val2 != null)
                    {
                        o4 = idx;
                        break;
                    }
                }
                if (o4 != null)
                {
                    MetaObject.Property propCss = o4.Values.Find(
                        delegate(MetaObject.Property idxI)
                        {
                            return idxI.Name == propertyCss;
                        });
                    if (propCss != null)
                        cssClass = propCss.Value;
                }
            }

            b.Text = "&nbsp;";
            b.CssClass = "multi-choice " + cssClass;
            string choiceVal = val == null ? "" : val.Value;

            b.Text = choiceVal;

            b.Click +=
                delegate
                {
                    MetaObject next = null;
                    bool found = false;
                    foreach (MetaObject idx in MetaObject.Select(Criteria.Eq("TypeName", type)))
                    {
                        if (found)
                        {
                            next = idx;
                            break;
                        }
                        MetaObject.Property val2 = idx.Values.Find(
                            delegate(MetaObject.Property idxI)
                            {
                                return idxI.Name == propertyName &&
                                    choiceVal == idxI.Value;
                            });
                        if (val2 != null && val2.Value == choiceVal)
                            found = true;
                    }
                    if (next == null)
                        next = MetaObject.SelectFirst(Criteria.Eq("TypeName", type));
                    b.CssClass = "multi-choice " + next.Values.Find(
                        delegate(MetaObject.Property idxI)
                        {
                            return idxI.Name == propertyCss;
                        }).Value;
                    b.Text = next.Values.Find(
                        delegate(MetaObject.Property idxI)
                        {
                            return idxI.Name == propertyName;
                        }).Value;
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        MetaObject o2 = MetaObject.SelectByID(id);
                        MetaObject.Property val3 = o.Values.Find(
                            delegate(MetaObject.Property idxI)
                            {
                                return idxI.Name == gridPropertyName;
                            });
                        if (val3 == null)
                        {
                            val3 = new MetaObject.Property();
                            val3.Name = gridPropertyName;
                            o2.Values.Add(val3);
                            o2.Save();
                        }
                        val3.Value = next.Values.Find(
                            delegate(MetaObject.Property idxI)
                            {
                                return idxI.Name == propertyName;
                            }).Value;
                        val3.Save();

                        tr.Commit();
                    }
                };

            node["Control"].Value = b;
        }

        /**
         * Level2: Loads up WYSIWYG editor for MetaView in 'SingleView Mode'
         */
        [ActiveEvent(Name = "Magix.MetaView.LoadWysiwyg")]
        protected void Magix_MetaView_LoadWysiwyg(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["Top"].Value = 2;
            node["MarginBottom"].Value = 20;
            node["PullTop"].Value = 28;
            node["CssClass"].Value = "yellow-background";
            node["MetaViewTypeName"].Value = m.TypeName;
            node["MetaViewName"].Value = m.Name;

            // TODO: Refactor and make shareable with MultiView and non-WYSIWYG mode ...
            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                node["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                node["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                node["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                node["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                node["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaView.MetaView_Single",
                "content6",
                node);
        }

        /**
         * Level2: Returns the properties for the MetaView back to caller
         */
        [ActiveEvent(Name = "Magix.MetaView.GetViewData")]
        protected void Magix_MetaView_GetViewData(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            e.Params["MetaViewTypeName"].Value = m.TypeName;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                e.Params["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                e.Params["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                e.Params["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                e.Params["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }
        }

        /**
         * Level2: Returns the number of MetaViews and an event for viewing all MetaViews back to caller
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaViewCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["Header"].Value = "Views";
            e.Params["Type"]["Properties"]["MetaViewCount"]["ClickLabelEvent"].Value = "Magix.MetaView.ViewMetaViews";
            e.Params["Object"]["Properties"]["MetaViewCount"].Value = MetaView.Count.ToString();
        }

        /**
         * Level2: Will create a new MetaObject according to the values given from the MetaView_SingleView form.
         * 'PropertyValues' is expected to contain a Name/Value list-pair, and the ViewName is
         * supposed to be the unique name to the specific MetaView being used.
         */
        [ActiveEvent(Name = "Magix.MetaView.CreateSingleViewMetaObject")]
        protected void Magix_MetaView_CreateSingleViewMetaObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView view = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

                MetaObject t = new MetaObject();

                t.TypeName = view.TypeName;
                t.Reference =
                    e.Params["MetaViewName"].Get<string>() +
                    "|" +
                    e.Params["ActionSenderName"].Get<string>();
                t.Created = DateTime.Now;

                foreach (Node idx in e.Params["PropertyValues"])
                {
                    MetaObject.Property v = new MetaObject.Property();
                    v.Name = idx["Name"].Get<string>();
                    v.Value = idx["Value"].Get<string>();
                    t.Values.Add(v);
                }

                t.Save();

                tr.Commit();
            }
        }
    }
}






















