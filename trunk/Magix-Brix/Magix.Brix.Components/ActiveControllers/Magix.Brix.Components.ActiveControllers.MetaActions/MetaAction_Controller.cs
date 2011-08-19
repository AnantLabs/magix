/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.UX.Widgets;
using System.Globalization;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    /**
     * Level2: Contains logic for Actions which are wrappers around ActiveEvents for the end user
     * to be able to raise his own events, with his own data in the Node structure. The 
     * MetaAction system is at the core of the Meta Application system wince without it
     * the end user cannot create his own types of events or Actions
     */
    [ActiveController]
    public class MetaAction_Controller : ActiveController
    {
        /**
         * Level2: Will return the menu items needed to fire up 'View Meta Actions' forms 
         * for Administrator
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Caption"].Value = "Meta Types";

            e.Params["Items"]["MetaType"]["Items"]["Actions"]["Caption"].Value = "Meta Actions ...";
            e.Params["Items"]["MetaType"]["Items"]["Actions"]["Event"]["Name"].Value = "Magix.MetaType.ViewActions";
        }

        /**
         * Level2: Will show a Grid containing all Meta Actions within the system
         */
        [ActiveEvent(Name = "Magix.MetaType.ViewActions")]
        protected void Magix_MetaType_ViewActions(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "content3";
            node["CssClass"].Value = "edit-actions";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 9;
            node["WhiteListColumns"]["Params"].Value = true;
            node["WhiteListColumns"]["Params"]["ForcedWidth"].Value = 2;
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 3;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditAction";
            node["CreateEventName"].Value = "Magix.Meta.CreateActionAndEdit";
            node["DeleteColumnEvent"].Value = "Magix.MetaAction.DeleteMetaAction";


            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.MetaAction.GetCopyActionTemplateColumn";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will show a 'search for Action and select' type of Grid to the end user so that
         * he can select and append an action into whatever collection wants to contain one
         */
        [ActiveEvent(Name = "Magix.MetaActions.SearchActions")]
        protected void Magix_MetaActions_SearchActions(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "content6";
            node["Width"].Value = 18;
            node["Padding"].Value = 6;
            node["PullTop"].Value = 28;
            node["MarginBottom"].Value = 30;
            node["Last"].Value = true;
            if (e.Params.Contains("ParentID"))
                node["ParentID"].Value = e.Params["ParentID"].Value;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 9;
            node["WhiteListColumns"]["Params"].Value = true;
            node["WhiteListColumns"]["Params"]["ForcedWidth"].Value = 2;

            node["NoIdColumn"].Value = true;
            node["IsCreate"].Value = false;
            node["IsDelete"].Value = false;
            node["IsFind"].Value = true;
            node["IsSelect"].Value = true;
            node["GetContentsEventName"].Value = "DBAdmin.Data.GetContentsOfClass-Filter-Override";
            node["SetFocus"].Value = true;
            node["SelectEvent"].Value = "Magix.MetaAction.ActionWasSelected";
            node["ParentPropertyName"].Value = e.Params["SelectEvent"].Value;


            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Raise 'ParentPropertyName', first setting the ActionName to the Action found in 'ID'.
         * Helper method for editing Actions in Views
         */
        [ActiveEvent(Name = "Magix.MetaAction.ActionWasSelected")]
        protected void Magix_MetaAction_ActionWasSelected(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ID"].Get<int>());
            e.Params["ActionName"].Value = a.Name;

            RaiseEvent(
                e.Params["ParentPropertyName"].Get<string>(),
                e.Params);
        }

        /**
         * Level2: Basically just overrides 'DBAdmin.Data.GetContentsOfClass' to allow
         * for adding some custom Criterias for our Search Box. Puts 'Filter'
         * into a Like expression on the Name column before calling 'base class'
         */
        [ActiveEvent(Name = "DBAdmin.Data.GetContentsOfClass-Filter-Override")]
        protected void DBAdmin_Data_GetContentsOfClass_Filter_Override(object sender, ActiveEventArgs e)
        {
            // First in Header ...
            if (e.Params.Contains("Filter"))
            {
                e.Params["Criteria"]["C3"]["Name"].Value = "Like";
                e.Params["Criteria"]["C3"]["Value"].Value = "%" + e.Params["Filter"].Get<string>() + "%";
                e.Params["Criteria"]["C3"]["Column"].Value = "Name";
            }
            else if (e.Params.Contains("Criteria") &&
                e.Params["Criteria"].Contains("C3"))
                e.Params["Criteria"]["C3"].UnTie();

            RaiseEvent(
                "DBAdmin.Data.GetContentsOfClass",
                e.Params);
        }

        /**
         * Level3: Returns a LinkButton that will allow for Deep-Copying the selected Action
         */
        [ActiveEvent(Name = "Magix.MetaAction.GetCopyActionTemplateColumn")]
        protected void Magix_MetaAction_GetCopyActionTemplateColumn(object sender, ActiveEventArgs e)
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
                        "Magix.MetaAction.CopyActionAndEdit",
                        node);
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Performs a Deep-Copy of the Action and returns the ID of the new Action as 'NewID'
         */
        [ActiveEvent(Name = "Magix.MetaAction.CopyAction")]
        protected void Magix_MetaAction_CopyAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = Action.SelectByID(e.Params["ID"].Get<int>());
                Action clone = a.Copy();

                clone.Save();

                tr.Commit();

                e.Params["NewID"].Value = clone.ID;
            }
        }

        /**
         * Level2: Performs a Deep-Copy of the Action and start editing the Action immediately
         */
        [ActiveEvent(Name = "Magix.MetaAction.CopyActionAndEdit")]
        protected void Magix_MetaAction_CopyActionAndEdit(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.MetaAction.CopyAction",
                e.Params);

            object cloneID = e.Params["NewID"].Value;

            Node n = new Node();

            n["FullTypeName"].Value = typeof(Action).FullName;
            n["ID"].Value = cloneID;

            RaiseEvent(
                "DBAdmin.Grid.SetActiveRow",
                n);

            n = new Node();
            n["FullTypeName"].Value = typeof(Action).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);

            n = new Node();
            n["ID"].Value = cloneID;

            RaiseEvent(
                "Magix.Meta.EditAction",
                n);
        }

        /**
         * Level2: Creates a new Default Action and returns the ID of the new Action as 'NewID'
         */
        [ActiveEvent(Name = "Magix.Meta.CreateAction")]
        protected void Magix_Meta_CreateAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = new Action();
                a.Name = "Default, please change";
                a.Save();

                tr.Commit();

                e.Params["NewID"].Value = a.ID;
            }
        }

        /**
         * Level2: Creates a new Default Action and starts editing it immediately
         */
        [ActiveEvent(Name = "Magix.Meta.CreateActionAndEdit")]
        protected void Magix_Meta_CreateActionAndEdit(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.Meta.CreateAction",
                e.Params);

            Node node = new Node();

            node["Start"].Value = 0;
            node["End"].Value = 10;
            node["FullTypeName"].Value = typeof(Action).FullName;

            RaiseEvent(
                "Magix.Core.SetGridPageStart",
                node);

            Node n = new Node();
            n["ID"].Value = e.Params["NewID"].Value;

            RaiseEvent(
                "Magix.Meta.EditAction",
                n);
        }

        /**
         * Level2: Edits the given Action ['ID'], with all its properties, parameters and so on. Also
         * creates a 'Run' button which the end user can click to run the action
         */
        [ActiveEvent(Name = "Magix.Meta.EditAction")]
        protected void Magix_Meta_EditAction(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ID"].Get<int>());
            
            Node node = new Node();
            node["ID"].Value = e.Params["ID"].Value;
            node["FullTypeName"].Value = typeof(Action).FullName;

            EditActionItemParams(a, node);
            EditActionItem(a, node);

            CreateRunParamButton(a);

            CreateCreateParamButton(a);

            CreateDeleteParamButton(a);

            ActiveEvents.Instance.RaiseClearControls("content6");
        }

        /*
         * Helper for above ...
         */
        private void EditActionItemParams(Action a, Node e)
        {
            Node node = new Node();

            node["CssClass"].Value = "clear-left";
            node["Width"].Value = 6;
            node["MarginBottom"].Value = 10;
            node["Top"].Value = 1;
            node["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            node["ActionItemID"].Value = a.ID;
            node["ItemSelectedEvent"].Value = "Magix.MetaAction.EditParam";
            node["GetItemsEvent"].Value = "Magix.MetaAction.GetActionItemTree";
            node["NoClose"].Value = true;

            RaiseEvent(
                "Magix.MetaAction.GetActionItemTree",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Tree",
                "content4",
                node);
        }

        /*
         * Helper for above ...
         */
        private void EditActionItem(Action a, Node node)
        {
            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["StripInput"].Value = true;
            node["WhiteListColumns"]["Description"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["Header"].Value = "Event Name";
            node["Type"]["Properties"]["EventName"]["Bold"].Value = true;
            node["Type"]["Properties"]["StripInput"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["StripInput"]["Header"].Value = "Strip Input Node";
            node["Type"]["Properties"]["StripInput"]["TemplateColumnEvent"].Value = "Magix.DataPlugins.GetTemplateColumns.CheckBox";
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = false;

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Container"].Value = "content5";
            node["MarginBottom"].Value = 20;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        /*
         * Helper for above ...
         */
        private void CreateRunParamButton(Action a)
        {
            Node node = new Node();

            node["Text"].Value = "Run!";
            node["ButtonCssClass"].Value = "span-4";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.MetaAction.RaiseAction";
            node["Event"]["ActionID"].Value = a.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /*
         * Helper for above ...
         */
        private void CreateCreateParamButton(Action a)
        {
            Node node = new Node();

            node["Text"].Value = "New Param ...";
            node["ButtonCssClass"].Value = "span-5";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.Meta.CreateParameter";
            node["Event"]["ID"].Value = a.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /*
         * Helper for above ...
         */
        private void CreateDeleteParamButton(Action a)
        {
            Node node = new Node();

            node["Text"].Value = "Delete";
            node["Seed"].Value = "delete-button";
            node["ButtonCssClass"].Value = "span-4 last";
            node["Append"].Value = true;
            node["Enabled"].Value = false;
            node["Event"].Value = "Magix.Meta.DeleteParameter";
            node["Event"]["ID"].Value = a.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /**
         * Level2: Deletes the Action.ActionParams given ['ID'] and updates a lot of UI properties.
         * Raises 'Magix.Core.GetSelectedTreeItem' to get the ID of which ActionParams to
         * actually delete
         */
        [ActiveEvent(Name = "Magix.Meta.DeleteParameter")]
        protected void Magix_Meta_DeleteParameter(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Node node2 = new Node();

                RaiseEvent(
                    "Magix.Core.GetSelectedTreeItem",
                    node2);

                Action.ActionParams p = Action.ActionParams.SelectByID(node2["ID"].Get<int>());
                p.Delete();

                tr.Commit();
            }

            ActiveEvents.Instance.RaiseClearControls("content6");

            Node node = new Node();
            node["Seed"].Value = "delete-button";
            node["Enabled"].Value = false;

            RaiseEvent(
                "Magix.Core.EnabledClickable",
                node);

            // Signalizing to Grids that they need to update ...
            Node n = new Node();
            n["FullTypeName"].Value = typeof(Action).FullName +
                "|" +
                typeof(Action.ActionParams).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);
        }

        /**
         * Level2: Creates a new Parameter and attaches to either the Selected Parameter, or if
         * none selected, the Action directly on root level. Depends upon the 
         * 'Magix.Core.GetSelectedTreeItem' event to get to know whether or not
         * it should add the action to a specific ActionParam as a Child or directly
         * upon the root level of the Action given through 'ID'
         */
        [ActiveEvent(Name = "Magix.Meta.CreateParameter")]
        protected void Magix_Meta_CreateParameter(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = Action.SelectByID(e.Params["ID"].Get<int>());

                Node tmp = new Node();

                RaiseEvent(
                    "Magix.Core.GetSelectedTreeItem",
                    tmp);

                RaiseEvent(
                    "Magix.Core.ExpandTreeSelectedID");

                Action.ActionParams p = new Action.ActionParams();
                p.Value = "Default, please change ...";
                p.Name = "Default Name";

                if (!tmp.Contains("ID"))
                {
                    a.Params.Add(p);
                    a.Save();
                }
                else
                {
                    Action.ActionParams p2 = Action.ActionParams.SelectByID(tmp["ID"].Get<int>());
                    p2.Children.Add(p);
                    p2.Save();
                }

                tr.Commit();

                Node node = new Node();

                // Signalizing to Grids that they need to update ...
                Node n = new Node();
                n["FullTypeName"].Value = typeof(Action).FullName +
                    "|" +
                    typeof(Action.ActionParams).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    n);
            }
        }

        /**
         * Level2: Will ask the user for confirmation about deleting the given Action ['ID'], and if
         * the end user confirms, the Action will be deleted
         */
        [ActiveEvent(Name = "Magix.MetaAction.DeleteMetaAction")]
        protected void Magix_MetaAction_DeleteMetaAction(object sender, ActiveEventArgs e)
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
<p>Are you sure you wish to delete this Action? 
This action might be in use in several forms or other parts of your system. 
Deleting it may break these parts.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.MetaAction.DeleteMetaAction-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Level2: Will call 'DBAdmin.Common.ComplexInstanceDeletedConfirmed' which again [hopefully]
         * will delete the given Action
         */
        [ActiveEvent(Name = "Magix.MetaAction.DeleteMetaAction-Confirmed")]
        protected void Magix_MetaAction_DeleteMetaAction_Confirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(Action).FullName)
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

        // TODO: Too long ...!! [refactor]
        /**
         * Level2: Will initiate editing of Parameter for Action unless it's already being edited, at
         * which point it'll be 'brought to front'
         */
        [ActiveEvent(Name = "Magix.MetaAction.EditParam")]
        private void Magix_Meta_EditParam(object sender, ActiveEventArgs e)
        {
            Action.ActionParams p = 
                Action.ActionParams.SelectByID(int.Parse(e.Params["SelectedItemID"].Value.ToString()));

            Node ch = new Node();
            ch["ID"].Value = p.ID;

            // Checks to see if Item is already being edited ...
            RaiseEvent(
                "DBAdmin.Form.CheckIfActiveTypeIsBeingSingleEdited",
                ch);

            if (!ch.Contains("Yes"))
            {
                // Editing it, since it's not being edited from before ...
                Node node = new Node();

                // First filtering OUT columns ...!
                node["WhiteListColumns"]["Name"].Value = true;
                node["WhiteListColumns"]["Value"].Value = true;
                node["WhiteListColumns"]["TypeName"].Value = true;

                node["WhiteListProperties"]["Name"].Value = true;
                node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 2;
                node["WhiteListProperties"]["Value"].Value = true;
                node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 4;

                node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Value"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["TypeName"]["TemplateColumnEvent"].Value = "Magix.MetaAction.GetMetaActionParameterTypeNameTemplateColumn";

                node["Width"].Value = 18;
                node["Last"].Value = true;
                node["Padding"].Value = 6;
                node["Container"].Value = "content6";
                node["IsList"].Value = false;
                node["CssClass"].Value = "small-editer";
                node["PullTop"].Value = 18;
                node["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
                node["ID"].Value = p.ID;

                Node xx = new Node();
                xx["Container"].Value = "content6";

                RaiseEvent(
                    "Magix.Core.GetNumberOfChildrenOfContainer",
                    xx);

                node["Append"].Value = xx["Count"].Get<int>() < 3;

                RaiseEvent(
                    "DBAdmin.Form.ViewComplexObject",
                    node);

                node = new Node();
                node["Seed"].Value = "delete-button";
                node["Enabled"].Value = true;
                RaiseEvent(
                    "Magix.Core.EnabledClickable",
                    node);
            }

            Node cc = new Node();

            cc["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            cc["CssClass"].Value = "";
            cc["Replace"].Value = " selected-action";

            RaiseEvent(
                "DBAdmin.Form.ChangeCssClassOfModule",
                cc);

            cc = new Node();

            cc["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            cc["ID"].Value = p.ID;
            cc["CssClass"].Value = " selected-action";

            RaiseEvent(
                "DBAdmin.Form.ChangeCssClassOfModule",
                cc);
        }

        /**
         * Level3: Returns a SelectList with the opportunity for the end user
         * to select which type [system-type, native] the specific parameter should be
         * converted to before Action is being ran
         */
        [ActiveEvent(Name = "Magix.MetaAction.GetMetaActionParameterTypeNameTemplateColumn")]
        private void Magix_MetaAction_GetMetaActionParameterTypeNameTemplateColumn(object sender, ActiveEventArgs e)
        {
            Action.ActionParams p = Action.ActionParams.SelectByID(e.Params["ID"].Get<int>());

            SelectList ls = new SelectList();
            ls.CssClass = "gridSelect";

            ls.SelectedIndexChanged +=
                delegate
                {
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        p.TypeName = ls.SelectedItem.Value;

                        p.Save();

                        tr.Commit();
                    }
                };


            ls.Items.Add(new ListItem("String", "System.String"));
            ls.Items.Add(new ListItem("Integer", "System.Int32"));
            ls.Items.Add(new ListItem("Decimal", "System.Decimal"));
            ls.Items.Add(new ListItem("DateTime", "System.DateTime"));
            ls.Items.Add(new ListItem("Bool", "System.Boolean"));

            switch (p.TypeName)
            {
                case "System.String":
                    ls.SelectedIndex = 0;
                    break;
                case "System.Int32":
                    ls.SelectedIndex = 1;
                    break;
                case "System.Decimal":
                    ls.SelectedIndex = 2;
                    break;
                case "System.DateTime":
                    ls.SelectedIndex = 3;
                    break;
                case "System.Boolean":
                    ls.SelectedIndex = 4;
                    break;
                default:
                    ls.Enabled = false;
                    ls.ToolTip = "Some Custom type I cannot infer ...";
                    break;
            }

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Returns a tree structure containing all the Action's Parameters to the caller
         */
        [ActiveEvent(Name = "Magix.MetaAction.GetActionItemTree")]
        private void Magix_Meta_GetActionItemTree(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ActionItemID"].Get<int>());
            foreach (Action.ActionParams idx in a.Params)
            {
                AddParamToNode(idx, e.Params);
            }
        }

        /*
         * Helper for above
         */
        private void AddParamToNode(Action.ActionParams par, Node node)
        {
            node["Items"]["i-" + par.ID]["ID"].Value = par.ID;
            node["Items"]["i-" + par.ID]["Name"].Value = par.Name;
            node["Items"]["i-" + par.ID]["ToolTip"].Value = par.Value;

            foreach (Action.ActionParams idx in par.Children)
            {
                AddParamToNode(idx, node["Items"]["i-" + par.ID]);
            }
        }

        /**
         * Level2: Returns menu items for dashboard functionality to be able to click and view Actions from Dashboard
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaActionCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaActionCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaActionCount"]["Header"].Value = "Actions";
            e.Params["Type"]["Properties"]["MetaActionCount"]["ClickLabelEvent"].Value = "Magix.MetaType.ViewActions";
            e.Params["Object"]["Properties"]["MetaActionCount"].Value = Action.Count.ToString();
        }

        /**
         * Level2: Will take an incoming 'EventName' with an optionally attached 'EventNode' structure
         * and create an Action out of it
         */
        [ActiveEvent(Name = "Magix.Core.EventClickedWhileDebugging")]
        protected void Magix_Core_EventClickedWhileDebugging(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                string eventName = e.Params["EventName"].Get<string>();
                Node eventNode = new Node();

                if(e.Params.Contains("EventNode"))
                    eventNode = e.Params["EventNode"].Value as Node;
                
                Action a = new Action();
                a.EventName = eventName;
                a.Name = "Debug-Copy-" + eventName;

                FillActionParams(eventNode, a.Params);

                a.Save();

                tr.Commit();

                
                Node node = new Node();
                node["ID"].Value = a.ID;

                RaiseEvent("Magix.MetaType.ViewActions");

                RaiseEvent(
                    "Magix.Meta.EditAction",
                    node);

                node = new Node();
                node["ID"].Value = a.ID;
                node["FullTypeName"].Value = typeof(Action).FullName;

                RaiseEvent(
                    "DBAdmin.Grid.SetActiveRow",
                    node);
            }
        }

        // TODO: There are two almost similar methods in this file which the underneath is one of. Remove one of them
        /*
         * Helper for above ...
         */
        private void FillActionParams(
            Node eventNode, 
            LazyList<Action.ActionParams> lazyList)
        {
            if (eventNode == null)
                return;
            foreach (Node idx in eventNode)
            {
                Action.ActionParams a = new Action.ActionParams();
                a.Name = idx.Name;
                switch ((idx.Value == null ? "" : idx.Value.GetType().FullName))
                {
                    case "System.Int32":
                        a.TypeName = idx.Value.GetType().FullName;
                        a.Value = idx.Value.ToString();
                        break;
                    case "System.DateTime":
                        a.TypeName = idx.Value.GetType().FullName;
                        a.Value = ((DateTime)idx.Value).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                        break;
                    case "System.Boolean":
                        a.TypeName = idx.Value.GetType().FullName;
                        a.Value = idx.Value.ToString();
                        break;
                    case "System.Decimal":
                        a.TypeName = idx.Value.GetType().FullName;
                        a.Value = ((decimal)idx.Value).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "System.String":
                        a.TypeName = idx.Value.GetType().FullName;
                        a.Value = idx.Value.ToString();
                        break;
                    default:
                        a.TypeName = "[Anonymous-Coward]";
                        a.Value = (idx.Value ?? "").ToString();
                        break;
                }
                lazyList.Add(a);
                if (eventNode.Contains("Children"))
                    FillActionParams(eventNode["Children"], a.Children);
            }
        }

        /**
         * Level2: Will take an incoming Action ['ActionID' OR 'ActionName'] and run it. Will merge
         * the incoming parameters with the Params of the Action, giving the 'incoming Parameters'
         * preference over the Params associated with Action
         */
        [ActiveEvent(Name = "Magix.MetaAction.RaiseAction")]
        protected void Magix_Meta_RaiseEvent(object sender, ActiveEventArgs e)
        {
            Action action = null;
            if (e.Params.Contains("ActionName"))
                action = Action.SelectFirst(
                    Criteria.Eq("Name", e.Params["ActionName"].Get<string>()));
            else
                action = Action.SelectByID(e.Params["ActionID"].Get<int>());

            if (action == null)
            {
                throw new NotImplementedException(@"Sorry dude, we couldn't find that Action. 
The 'Magix.MetaAction.RaiseAction' Action needs to reference another existing Action 
within the system, either through the 'ActionName' parameter, or the 'ActionID' parameter. 
Name, obviously, pointing to the exact name of an Action, while ActionID obviously
referring to the exact ID of an action, and needing to be an integer ...");
            }

            Node node = e.Params;
            if (action.StripInput)
            {
                node = new Node();
            }

            foreach (Action.ActionParams idx in action.Params)
            {
                GetActionParameters(node, idx);
            }

            if (e.Params.Contains("Params"))
            {
                foreach (Node idx in e.Params["Params"])
                {
                    EmbedNodeIntoNode(idx, node);
                }
            }

            RaiseEvent(
                action.EventName,
                node);
        }

        /*
         * Helper for above ...
         */
        private void EmbedNodeIntoNode(Node source, Node destination)
        {
            if (source.Value != null)
            {
                destination[source.Name].Value = source.Value;
                foreach (Node idx in source)
                {
                    EmbedNodeIntoNode(idx, destination[source.Name]);
                }
            }
        }

        // TODO: There are two almost similar methods in this file which the underneath is one of. Remove one of them
        /*
         * Helper for above ...
         */
        private static void GetActionParameters(Node node, Action.ActionParams a)
        {
            switch (a.TypeName)
            {
                case "System.String":
                    node[a.Name].Value = a.Value;
                    break;
                case "System.Int32":
                    node[a.Name].Value = int.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.Decimal":
                    node[a.Name].Value = decimal.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    node[a.Name].Value = DateTime.ParseExact(a.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    node[a.Name].Value = bool.Parse(a.Value);
                    break;
                default:
                    node[a.Name].Value = a.Value;
                    break;
            }
            foreach (Action.ActionParams idx in a.Children)
            {
                GetActionParameters(node[a.Name], idx);
            }
        }
    }
}





















