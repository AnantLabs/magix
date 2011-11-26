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
using System.Collections.Generic;
using Magix.Brix.Components.ActiveTypes.Publishing;
using System.Reflection;

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
         * Level2: Will initialize all the Actions which are Overriding existing Active Events
         */
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            CreateActionOverrides();
        }

        /*
         * Helper for above ...
         */
        private static void CreateActionOverrides()
        {
            foreach (Action idx in Action.Select(Criteria.Sort("Overrides", true)))
            {
                if (idx.Overrides.EndsWith("!"))
                {
                    // If the Event Name ends with a '!', the original event handlers are 'punctured' ...
                    string name = idx.Overrides.Trim('!').Trim('.');
                    ActiveEvents.Instance.CreateEventMapping(name, "Magix.MetaAction.RaiseOverriddenAction");
                    ActiveEvents.Instance.CreateEventMapping(name + "-Action-Overridden", name);
                }
                else if (idx.Overrides.IndexOf("...") == 0)
                {
                    // If the Event Name does NOT end with a '!', the original event handlers must also be 
                    // invoked. FIRST if it starts with a '...'
                    string name = idx.Overrides.Trim('.');
                    ActiveEvents.Instance.CreateEventMapping(name, "Magix.MetaAction.RaiseOverriddenActionCallBaseFirst");
                    ActiveEvents.Instance.CreateEventMapping(name + "-Action-Overridden", name);
                }
                else
                {
                    // If the Event Name does NOT end with a '!', the original event handlers must also be 
                    // invoked LAST unless is starts with '...'
                    string name = idx.Overrides;
                    ActiveEvents.Instance.CreateEventMapping(name, "Magix.MetaAction.RaiseOverriddenActionCallBase");
                    ActiveEvents.Instance.CreateEventMapping(name + "-Action-Overridden", name);
                }
            }
        }

        /**
         * Level2: Will Reinitialize all the Overridden Actions
         */
        [ActiveEvent(Name = "Magix.MetaAction.ReInitializeOverriddenActions")]
        protected void Magix_Meta_ReInitializeOverriddenActions(object sender, ActiveEventArgs e)
        {
            RaiseEvent("Magix.MetaAction.ClearOverriddenActions");
            CreateActionOverrides();
        }

        /**
         * Level2: Will Clear all the Overridden Actions
         */
        [ActiveEvent(Name = "Magix.MetaAction.ClearOverriddenActions")]
        protected void Magix_Meta_ClearOverriddenActions(object sender, ActiveEventArgs e)
        {
            List<string> lst = new List<string>();

            foreach (string idx in ActiveEvents.Instance.EventMappingKeys)
            {
                if (idx.EndsWith("-Action-Overridden"))
                {
                    lst.Add(idx);
                }
                else if (ActiveEvents.Instance.GetEventMappingValue(idx) == "Magix.MetaAction.RaiseOverriddenAction")
                {
                    lst.Add(idx);
                }
                else if (ActiveEvents.Instance.GetEventMappingValue(idx) == "Magix.MetaAction.RaiseOverriddenActionCallBase")
                {
                    lst.Add(idx);
                }
                else if (ActiveEvents.Instance.GetEventMappingValue(idx) == "Magix.MetaAction.RaiseOverriddenActionCallBaseFirst")
                {
                    lst.Add(idx);
                }
            }

            foreach (string idx in lst)
            {
                ActiveEvents.Instance.RemoveMapping(idx);
            }
        }

        /**
         * Level2: Helper to make sure our Overriding Actions are being raised
         */
        [ActiveEvent(Name = "Magix.MetaAction.RaiseOverriddenAction")]
        protected void Magix_Meta_RaiseOverriddenAction(object sender, ActiveEventArgs e)
        {
            Action action = Action.SelectFirst(Criteria.Eq("Overrides", e.Name.Trim('.') + "!"));

            e.Params["ActionID"].Value = action.ID;

            RaiseEvent(
                "Magix.MetaAction.RaiseAction",
                e.Params);
        }

        /**
         * Level2: Helper to make sure our Overriding Actions are being raised
         */
        [ActiveEvent(Name = "Magix.MetaAction.RaiseOverriddenActionCallBase")]
        protected void Magix_Meta_RaiseOverriddenActionCallBase(object sender, ActiveEventArgs e)
        {
            Action action = Action.SelectFirst(Criteria.Eq("Overrides", e.Name));

            e.Params["ActionID"].Value = action.ID;

            RaiseEvent(
                "Magix.MetaAction.RaiseAction",
                e.Params);

            RaiseEvent(
                action.Overrides + "-Action-Overridden",
                e.Params);
        }

        /**
         * Level2: Helper to make sure our Overriding Actions are being raised
         */
        [ActiveEvent(Name = "Magix.MetaAction.RaiseOverriddenActionCallBaseFirst")]
        protected void Magix_Meta_RaiseOverriddenActionCallBaseFirst(object sender, ActiveEventArgs e)
        {
            Action action = Action.SelectFirst(Criteria.Eq("Overrides", "..." + e.Name));

            RaiseEvent(
                action.Overrides.Trim('.').Trim('!') + "-Action-Overridden",
                e.Params);

            e.Params["ActionID"].Value = action.ID;

            RaiseEvent(
                "Magix.MetaAction.RaiseAction",
                e.Params);
        }

        /**
         * Level 2: Returns the Desktop Icon for launching Actions back to caller
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Actions"]["Image"].Value = "media/images/desktop-icons/lightning-icon.png";
            e.Params["Items"]["Actions"]["Shortcut"].Value = "c";
            e.Params["Items"]["Actions"]["Text"].Value = "Click to launch Actions [Key A]";
            e.Params["Items"]["Actions"]["CSS"].Value = "mux-desktop-icon mux-actions";
            e.Params["Items"]["Actions"]["Event"].Value = "Magix.MetaType.ViewActions";
        }

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
            node["CssClass"].Value = "mux-edit-actions";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["IsFind"].Value = true;
            node["IsCreate"].Value = true;
            node["GetContentsEventName"].Value = "DBAdmin.Data.GetContentsOfClass-Filter-Override";
            node["SetFocus"].Value = true;

            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 2;
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 10;
            node["WhiteListColumns"]["Params"].Value = true;
            node["WhiteListColumns"]["Params"]["ForcedWidth"].Value = 2;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditAction";
            node["CreateEventName"].Value = "Magix.Meta.CreateActionAndEdit";
            node["DeleteColumnEvent"].Value = "Magix.MetaAction.DeleteMetaAction";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.MetaAction.GetCopyActionTemplateColumn";
            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);

            node = new Node();

            node["Caption"].Value = "Actions";

            RaiseEvent(
                "Magix.Core.SetFormCaption",
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
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = false;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";

            RaiseEvent(
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
            if (e.Params.Contains("Filter") && 
                !string.IsNullOrEmpty(e.Params["Filter"].Get<string>()))
            {
                e.Params["Criteria"]["C3"]["Name"].Value = "Like";
                e.Params["Criteria"]["C3"]["Value"].Value = "%" + e.Params["Filter"].Get<string>() + "%";
                e.Params["Criteria"]["C3"]["Column"].Value = "Name";
            }
            else if (e.Params.Contains("Criteria") &&
                e.Params["Criteria"].Contains("C3"))
            {
                e.Params["Criteria"]["C3"].UnTie();
            }

            RaiseEvent(
                "DBAdmin.Data.GetContentsOfClass",
                e.Params);

            if (e.Params.Contains("Objects") && 
                e.Params["Objects"].Count == 1)
            {
                // TODO: Doesn't work, since it fucks up while searching for actions 'all over the places'. Fix ...
                // Automatically editing bugger ...
                //Node n = new Node();

                //n["ID"].Value = e.Params["Objects"][0]["ID"].Value;

                //RaiseEvent(
                //    "Magix.Meta.EditAction",
                //    n);

                //n = new Node();
                //n["FullTypeName"].Value = typeof(Action).FullName;
                //n["ID"].Value = e.Params["Objects"][0]["ID"].Value;

                //RaiseEvent(
                //    "DBAdmin.Grid.SetActiveRow",
                //    n);
            }
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

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);

            n = new Node();
            n["FullTypeName"].Value = typeof(Action).FullName;
            n["ID"].Value = cloneID;

            RaiseEvent(
                "DBAdmin.Grid.SetActiveRow",
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

            Node n = new Node();

            n["FullTypeName"].Value = typeof(Action).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);

            n = new Node();
            n["FullTypeName"].Value = typeof(Action).FullName;
            n["ID"].Value = e.Params["NewID"].Value;

            RaiseEvent(
                "DBAdmin.Grid.SetActiveRow",
                n);

            n = new Node();

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

            CreateRunParamButton(a);

            if (!a.Name.StartsWith("Magix."))
                CreateCreateParamButton(a);

            if (!a.Name.StartsWith("Magix."))
                CreateCreateParamButton2(a);

            if (!a.Name.StartsWith("Magix."))
                CreateDeleteParamButton(a);

            if (!a.Name.StartsWith("Magix."))
                CreateFindOverrideButton(a);

            EditActionItem(a, node);

            ActiveEvents.Instance.RaiseClearControls("content6");
        }

        /*
         * Helper for above ...
         */
        private void EditActionItemParams(Action a, Node e)
        {
            Node node = new Node();

            node["Width"].Value = 6;
            node["CssClass"].Value = "clear-both";
            node["MarginBottom"].Value = 10;
            node["Top"].Value = 1;
            node["TreeCssClass"].Value = "mux-parameters";
            node["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            node["ActionItemID"].Value = a.ID;

            if (a.Name.StartsWith("Magix."))
                node["ItemSelectedEvent"].Value = "Magix.MetaAction.EditParamReadOnly";
            else
                node["ItemSelectedEvent"].Value = "Magix.MetaAction.EditParam";

            node["GetItemsEvent"].Value = "Magix.MetaAction.GetActionItemTree";
            node["Header"].Value = "Params";

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
            node["WhiteListColumns"]["Overrides"].Value = true;
            node["WhiteListColumns"]["StripInput"].Value = true;
            node["WhiteListColumns"]["Description"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            // Making sure all Magix Actions are READ-ONLY for the user, so he doesn't start
            // changing their names and screwing up things ...
            if (a.Name.StartsWith("Magix."))
            {
                // These are 'template actions', not intended for being edited at all
                node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["Overrides"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["StripInput"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["Description"]["ReadOnly"].Value = true;
            }
            else
            {
                node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Name"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
                node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["EventName"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
                node["Type"]["Properties"]["Overrides"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Overrides"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
                node["Type"]["Properties"]["StripInput"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Description"]["ReadOnly"].Value = false;
                node["Type"]["Properties"]["Description"]["MaxLength"].Value = 4000;
            }

            node["Type"]["Properties"]["EventName"]["Header"].Value = "Event Name";
            node["Type"]["Properties"]["EventName"]["Bold"].Value = true;
            node["Type"]["Properties"]["StripInput"]["Header"].Value = "Strip Input Node";
            node["Type"]["Properties"]["StripInput"]["TemplateColumnEvent"].Value = "Magix.DataPlugins.GetTemplateColumns.CheckBox";

            node["Append"].Value = true;
            node["Container"].Value = "content5";
            node["ChildCssClass"].Value = "clear-both";

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        /*
         * Helper for above ...
         */
        private void CreateFindOverrideButton(Action a)
        {
            Node node = new Node();

            node["Append"].Value = true;

            node["Text"].Value = "Search ...";
            node["ButtonCssClass"].Value = "span-4 down-1";
            node["Event"].Value = "Magix.MetaAction.FindOverrideActiveEvent";
            node["Event"]["ActionID"].Value = a.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /**
         * Level2: Will show a Search Modal Window which will allow you to find any 
         * Active Event handler within the system
         */
        [ActiveEvent(Name = "Magix.MetaAction.FindOverrideActiveEvent")]
        protected void Magix_MetaAction_FindOverrideActiveEvent(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(ActiveEvents).FullName;
            node["Container"].Value = "child";
            node["Width"].Value = 24;
            node["Top"].Value = 23;
            node["Last"].Value = true;
            node["IsFind"].Value = true;
            node["ReUseNode"].Value = true;
            node["IsDelete"].Value = false;
            node["SetFocus"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 22;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Select";
            node["IDColumnValue"].Value = "Select";
            node["IDColumnEvent"].Value = "Magix.Meta.SelectActiveEvent";
            node["IDColumnEvent"]["ActionID"].Value = e.Params["ActionID"].Value;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = false;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Sink for selecting an Active Event to encapsulate within an Action
         */
        [ActiveEvent(Name = "Magix.Meta.SelectActiveEvent")]
        protected void Magix_Meta_SelectActiveEvent(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                int id = e.Params["Parameters"]["IDColumnEvent"]["ActionID"].Get<int>();
                Action a = Action.SelectByID(id);
                a.EventName = _activeEvents[e.Params["ID"].Get<int>()].Left;

                if (string.IsNullOrEmpty(a.Description))
                {
                    a.Description = _activeEvents[e.Params["ID"].Get<int>()].Right;
                    ShowMessage("Description was fetched from all Event Handlers handling this Active Event within your Application Pool ...", "Info", 5000);
                }
                else
                {
                    ShowMessage("No description was copied since your Action had a description from before. To allow for pasting of the default description, make sure your Action has no description before you select an Active Event to wrap ...", "Info", 7000);
                }

                a.Save();

                tr.Commit();

                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }

        private static List<Tuple<string, string>> _activeEvents = new List<Tuple<string, string>>();

        /**
         * Level2: Sink for finding all Active Events within the system Application Pool
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode_3(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(ActiveEvents).FullName)
            {
                if (_activeEvents.Count == 0)
                {
                    foreach (Assembly idxA in Loader.PluginLoader.PluginAssemblies)
                    {
                        foreach (Type idxT in idxA.GetTypes())
                        {
                            foreach (MethodInfo idxM in idxT.GetMethods(
                                BindingFlags.Public | 
                                BindingFlags.NonPublic | 
                                BindingFlags.Static | 
                                BindingFlags.Instance))
                            {
                                ActiveEventAttribute[] atrs =
                                    idxM.GetCustomAttributes(typeof(ActiveEventAttribute), true)
                                    as ActiveEventAttribute[];
                                if (atrs != null && atrs.Length > 0)
                                {
                                    if (string.IsNullOrEmpty(atrs[0].Name))
                                        continue;

                                    if (_activeEvents.Exists(
                                        delegate(Tuple<string, string> idxTT)
                                        {
                                            return idxTT.Left == atrs[0].Name;
                                        }))
                                        continue;

                                    _activeEvents.Add(new Tuple<string, string>(
                                        atrs[0].Name, 
                                        GetDocumentation(atrs[0].Name)));
                                }
                            }
                        }
                    }
                }

                int start = e.Params["Start"].Get<int>();
                int end = e.Params["End"].Get<int>();
                string filter = e.Params.Contains("Filter") ?
                    e.Params["Filter"].Get<string>() :
                    "";
                string[] filters = filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int idxNo = 0;
                int idxIDNo = 0;
                foreach (Tuple<string, string> idxKey in _activeEvents)
                {
                    int found = 0;
                    foreach (string idxF in filters)
                    {
                        if (idxKey.Left.ToLower().Contains(idxF))
                            found += 1;
                    }
                    if (found == filters.Length)
                    {
                        if (idxNo >= start && idxNo < end)
                        {
                            e.Params["Objects"]["o-" + idxNo]["ID"].Value = idxIDNo;
                            e.Params["Objects"]["o-" + idxNo]["Properties"]["Name"].Value = idxKey.Left;
                            e.Params["Objects"]["o-" + idxNo]["Properties"]["Name"]["ToolTip"].Value = idxKey.Right;
                        }
                        idxNo += 1;
                    }
                    idxIDNo += 1;
                }
                e.Params["SetCount"].Value = idxNo;
                e.Params["LockSetCount"].Value = true;
            }
        }

        private string GetDocumentation(string actEvtName)
        {
            Node node = new Node();

            node["ActiveEventName"].Value = actEvtName;

            RaiseEvent(
                "Magix.Core.GetDocumentationForActiveEvent",
                node);

            return node["Result"].Get<string>("");
        }

        /*
         * Helper for above ...
         */
        private void CreateRunParamButton(Action a)
        {
            Node node = new Node();

            node["Width"].Value = 18;
            node["Top"].Value = 1;
            node["Last"].Value = true;
            node["MarginBottom"].Value = 10;

            node["Text"].Value = "Run!";
            node["AccessKey"].Value = "m";
            node["ButtonCssClass"].Value = "span-4";
            node["Event"].Value = "Magix.MetaAction.RaiseAction-FromActionScreen";
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

            node["Text"].Value = "New Root Param ...";
            node["ButtonCssClass"].Value = "span-5";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.Meta.CreateParameter";
            node["Event"]["ID"].Value = a.ID;
            node["Event"]["IsRoot"].Value = true;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        /*
         * Helper for above ...
         */
        private void CreateCreateParamButton2(Action a)
        {
            Node node = new Node();

            node["Seed"].Value = "create-child-button";
            node["Text"].Value = "New Child Param ...";
            node["ButtonCssClass"].Value = "span-5";
            node["Append"].Value = true;
            node["Enabled"].Value = false;
            node["Event"].Value = "Magix.Meta.CreateParameter";
            node["Event"]["ID"].Value = a.ID;
            node["Event"]["IsRoot"].Value = false;

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
         * Level2: Will forward to RaiseAction, and show a Message to user ...
         */
        [ActiveEvent(Name = "Magix.MetaAction.RaiseAction-FromActionScreen")]
        protected void Magix_MetaAction_RaiseAction_FromActionScreen(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            if (e.Params.Contains("ActionName"))
                node["ActionName"].Value = e.Params["ActionName"].Get<string>();

            if (e.Params.Contains("ActionID"))
                node["ActionID"].Value = e.Params["ActionID"].Get<string>();

            RaiseEvent("Magix.MetaAction.RaiseAction", node);
            ShowMessage("Action Successfully Executed ...");
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

            node = new Node();

            node["Seed"].Value = "create-child-button";
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

                if (e.Params.Contains("IsRoot") && e.Params["IsRoot"].Get<bool>() || !tmp.Contains("ID"))
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

            Action a = Action.SelectByID(id);
            if (a.Name.StartsWith("Magix."))
                throw new ArgumentException("Sorry, those are 'System Actions', and in general terms 'off limits'. If you _really_ know what you're doing, you can delete these through the DBAdmin interface ...");


            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);
            Node node = e.Params;
            if (node == null)
            {
                node = new Node();
                node["ForcedSize"]["width"].Value = 550;
                node["CssClass"].Value = "mux-shaded mux-rounded push-5 down-2";
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
                    "Magix.Core.ClearViewportContainer",
                    n);
                
                RaiseEvent(
                    "DBAdmin.Common.ComplexInstanceDeletedConfirmed",
                    e.Params);
            }
        }

        /**
         * Level2: Will initiate editing of Parameter for Action unless it's already being edited, at
         * which point it'll be 'brought to front'
         */
        [ActiveEvent(Name = "Magix.MetaAction.EditParam")]
        private void Magix_MetaAction_EditParam(object sender, ActiveEventArgs e)
        {
            ViewParameter(e, false);
        }

        /**
         * Level2: Will initiate viewing of Parameter for Action unless it's already being viewed, at
         * which point it'll be 'brought to front'
         */
        [ActiveEvent(Name = "Magix.MetaAction.EditParamReadOnly")]
        private void Magix_MetaAction_EditParamReadOnly(object sender, ActiveEventArgs e)
        {
            ViewParameter(e, true);
        }

        private void ViewParameter(ActiveEventArgs e, bool readOnly)
        {
            Action.ActionParams p =
                Action.ActionParams.SelectByID(int.Parse(e.Params["SelectedItemID"].Value.ToString()));

            string content = "content6";

            if (e.Params.Contains("ReUseNode") &&
                e.Params["ReUseNode"].Get<bool>() &&
                e.Params.Contains("Container"))
            {
                content = e.Params["Container"].Get<string>();
            }

            Node ch = new Node();

            ch["ID"].Value = p.ID;

            // Checks to see if Item is already being edited ...
            RaiseEvent(
                "DBAdmin.Form.CheckIfActiveTypeIsBeingSingleEdited",
                ch);

            if (ch["Yes"].Get<bool>())
            {
                // Assuming, by default, if the User clicks the same twice, he wants to start a 'new stack' ...
                ActiveEvents.Instance.RaiseClearControls(content);

                if (e.Params.Contains("ChildCssClass"))
                {
                    e.Params["ChildCssClass"].Value = e.Params["ChildCssClass"].Get<string>().Replace(" last", "");
                }
            }

            if (!e.Params.Contains("ReUseNode") ||
                !e.Params["ReUseNode"].Get<bool>())
            {
                Node xx = new Node();

                xx["Container"].Value = content;

                RaiseEvent(
                    "Magix.Core.GetNumberOfChildrenOfContainer",
                    xx);

                // Because of our little CSS Animation Tricks, we always
                // must have 3 div elements in our container ...
                for (int idx = xx["Count"].Get<int>(); idx < 2; idx++)
                {
                    Node tt = new Node();

                    if (xx["Count"].Get<int>() == idx)
                    {
                        tt["Width"].Value = 18;
                        tt["Last"].Value = true;
                        tt["CssClass"].Value = "mux-small-editer down--10";
                    }
                    tt["Tag"].Value = "div";
                    tt["Append"].Value = true;
                    tt["Text"].Value = "&nbsp;";
                    tt["ModuleID"].Value = "filler" + idx;

                    LoadModule(
                        "Magix.Brix.Components.ActiveModules.CommonModules.Text",
                        content,
                        tt);
                }
            }

            // Editing it, since it's not being edited from before ...
            Node node = new Node();

            if (e.Params.Contains("ReUseNode") &&
                    e.Params["ReUseNode"].Get<bool>())
            {
                node = e.Params;
            }

            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Value"].Value = true;
            node["WhiteListColumns"]["TypeName"].Value = true;

            if (!node.Contains("WhiteListProperties"))
            {
                node["WhiteListProperties"]["Name"].Value = true;
                node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 2;
                node["WhiteListProperties"]["Value"].Value = true;
                node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 7;
            }

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = readOnly;
            node["Type"]["Properties"]["Name"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
            node["Type"]["Properties"]["Value"]["ReadOnly"].Value = readOnly;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = readOnly;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";
            node["Type"]["Properties"]["TypeName"]["TemplateColumnEvent"].Value = "Magix.MetaAction.GetMetaActionParameterTypeNameTemplateColumn";

            if (!node.Contains("Container"))
                node["Container"].Value = "content6";
            node["IsList"].Value = false;
            node["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            node["ID"].Value = p.ID;
            node["ModuleID"].Value = "am" + p.ID;

            if (!e.Params.Contains("ReUseNode") ||
                !e.Params["ReUseNode"].Get<bool>())
            {
                node["Append"].Value = true;
                node["AppendMaxCount"].Value = 3;
            }

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);

            if (!e.Params.Contains("ReUseNode") ||
                !e.Params["ReUseNode"].Get<bool>())
            {
                Node cc = new Node();

                cc["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
                cc["Replace"].Value = " mux-selected-action-param";

                RaiseEvent(
                    "DBAdmin.Form.ChangeCssClassOfModule",
                    cc);

                cc = new Node();

                cc["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
                cc["ID"].Value = p.ID;
                cc["CssClass"].Value = " mux-selected-action-param";

                RaiseEvent(
                    "DBAdmin.Form.ChangeCssClassOfModule",
                    cc);
            }

            node = new Node();

            node["Seed"].Value = "delete-button";
            node["Enabled"].Value = true;

            RaiseEvent(
                "Magix.Core.EnabledClickable",
                node);

            node = new Node();

            node["Seed"].Value = "create-child-button";
            node["Enabled"].Value = true;

            RaiseEvent(
                "Magix.Core.EnabledClickable",
                node);
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
            ls.CssClass = "mux-grid-select";

            if (e.Params.Contains("ReadOnly"))
                ls.Enabled = !e.Params["ReadOnly"].Get<bool>();

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
                FillOneActionParam(lazyList, idx);
            }
        }

        private void FillOneActionParam(LazyList<Action.ActionParams> lazyList, Node idx)
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
                    a.Value = null;
                    break;
            }
            lazyList.Add(a);
            foreach (Node idx2 in idx)
            {
                if (!string.IsNullOrEmpty(idx.Name))
                    FillOneActionParam(a.Children, idx2);
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

            string eventName = action.EventName;

            if (eventName.Contains("("))
            {
                string expr = eventName.Split('(')[1].Split(')')[0].Trim();

                eventName = eventName.Substring(0, eventName.IndexOf("(")).Trim();

                bool isInside = false;
                string nodePath = "";
                for (int idx = 0; idx < expr.Length; idx++)
                {
                    if (isInside)
                    {
                        if (expr[idx] == ']')
                        {
                            node = node[nodePath];
                            nodePath = "";
                            isInside = false;
                            continue;
                        }
                        nodePath += expr[idx];
                    }
                    else
                    {
                        if (expr[idx] == '[')
                        {
                            isInside = true;
                            continue;
                        }
                    }
                }
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
                eventName,
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
            node.AddForceNew(new Node(a.Name));
            switch (a.TypeName)
            {
                case "System.String":
                    node[node.Count - 1].Value = a.Value;
                    break;
                case "System.Int32":
                    node[node.Count - 1].Value = int.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.Decimal":
                    node[node.Count - 1].Value = decimal.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    node[node.Count - 1].Value = DateTime.ParseExact(a.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    node[node.Count - 1].Value = bool.Parse(a.Value);
                    break;
                default:
                    node[node.Count - 1].Value = a.Value;
                    break;
            }
            foreach (Action.ActionParams idx in a.Children)
            {
                GetActionParameters(node[node.Count - 1], idx);
            }
        }

        /**
         * Level2: Sink for selecting Actions for Modules
         */
        [ActiveEvent(Name = "Magix.Publishing.GetSelectActionForModuleControl")]
        private void Magix_Publishing_GetSelectActionForModuleControl(object sender, ActiveEventArgs e)
        {
            LinkButton b = new LinkButton();
            b.Text = "Actions ...";

            string id = e.Params["WebPartID"].Value.ToString();
            string actions = e.Params["Value"].Value.ToString();

            b.CssClass = "clear-both mux-darken";
            b.Style[Styles.width] = "294px";
            b.Style[Styles.floating] = "left";

            if (!string.IsNullOrEmpty(actions))
                b.CssClass += " mux-has-actions";
            else
                b.CssClass += " mux-no-actions";

            b.Click +=
                delegate
                {
                    Node node = new Node();

                    node["IsDelete"].Value = true;
                    node["IsCreate"].Value = true;
                    node["Container"].Value = "child";
                    node["Width"].Value = 20;
                    node["Top"].Value = 33;
                    node["FullTypeName"].Value = typeof(Action).FullName + "-META3";
                    node["GetObjectsEvent"].Value = "DBAdmin.DynamicType.GetObjectsNode";
                    node["ChangeSimplePropertyValue"].Value = "Magix.MetaActions.ChangeSimplePropertyValue3";

                    node["WebPartID"].Value = id;
                    node["Header"].Value = "Actions for Module";

                    node["EventName"].Value = e.Params["EventName"].Value;
                    node["CreateEventName"].Value = "Magix.MetaForms.OpenAppendNewActionDialogue3";

                    node["WhiteListColumns"]["Up"].Value = true;
                    node["WhiteListColumns"]["Up"]["ForcedWidth"].Value = 1;
                    node["WhiteListColumns"]["Down"].Value = true;
                    node["WhiteListColumns"]["Down"]["ForcedWidth"].Value = 1;
                    node["WhiteListColumns"]["Name"].Value = true;
                    node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 14;

                    node["NoIdColumn"].Value = true;
                    node["DeleteColumnEvent"].Value = "Magix.MetaForms.RemoveActionFromActionList3";

                    node["ReuseNode"].Value = true;

                    RaiseEvent(
                        "DBAdmin.Form.ViewClass",
                        node);
                };

            e.Params["Control"].Value = b;
        }

        /**
         * Level2: Sink for deleting Meta Form Action from Event
         */
        [ActiveEvent(Name = "Magix.MetaForms.RemoveActionFromActionList3")]
        protected void Magix_MetaForms_RemoveActionFromActionList3(object sender, ActiveEventArgs e)
        {
            string id = e.Params["ID"].Get<string>();

            Node node = new Node();

            node["CssClass"].Value = "mux-shaded mux-rounded down-33 span-10";
            node["Caption"].Value = @"Please confirm removing of Action";
            node["Text"].Value = @"
<p>Are you sure you wish to remove this Action Reference?</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["Event"].Value = "Magix.MetaForms.RemoveActionFromActionList3-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Level2: Will actually remove the Action from the Action list
         */
        [ActiveEvent(Name = "Magix.MetaForms.RemoveActionFromActionList3-Confirmed")]
        protected void Magix_MetaForms_RemoveActionFromActionList3_Confirmed(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPart.WebPartSetting n = WebPart.WebPartSetting.SelectByID(int.Parse(e.Params["ID"].Get<string>().Split('|')[0]));
                int idxNo = 0;
                int toRemove = int.Parse(e.Params["ID"].Get<string>().Split('|')[1]);
                string result = "";
                foreach (string idx in n.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (idxNo != toRemove)
                        result += idx + "|";
                    idxNo += 1;
                }
                result = result.Trim('|');
                n.Value = result;
                n.Save();

                tr.Commit();
            }

            Node node = new Node();
            node["FullTypeName"].Value = typeof(Action).FullName + "-META3";

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                node);

            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Level2: Will show the 'List of Actions form' for appending and editing and deleting
         * actions associated with the specific Action on the specific Widget
         */
        [ActiveEvent(Name = "Magix.MetaForms.OpenAppendNewActionDialogue3")]
        protected void Magix_MetaForms_OpenAppendNewActionDialogue3(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "child";
            node["Width"].Value = 15;
            node["Top"].Value = 33;

            WebPart.WebPartSetting f = WebPart.WebPartSetting.SelectByID(e.Params["WebPartID"].Get<int>());

            node["ParentID"].Value = f.ID;

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
            node["SelectEvent"].Value = "Magix.MetaForms.ActionWasSelected3";
            node["SelectEvent"]["NodeID"].Value = f.ID;
            node["ConfigureFiltersEvent"].Value = "Magix.MetaForms.ConfigureFilterForColumns";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["Name"]["TemplateColumnEvent"].Value = "Magix.Forms.GetActionSelectActionTemplateColumn";
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = false;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";
            node["Start"].Value = 0;
            node["End"].Value = 8;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will append the given 'ID' Action into the given 'ParentPropertyName' Event name
         * and save the MetaForm.Node
         */
        [ActiveEvent(Name = "Magix.MetaForms.ActionWasSelected3")]
        protected void Magix_MetaForms_ActionWasSelected3(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPart.WebPartSetting n = WebPart.WebPartSetting.SelectByID(e.Params["ParentID"].Get<int>());
                n.Value += "|" +
                    Action.SelectByID(e.Params["ID"].Get<int>()).Name;
                n.Save();

                tr.Commit();
            }

            ActiveEvents.Instance.RaiseClearControls("child");
        }

        /**
         * Level2: Allows user to change name of Action Reference in WebParts
         */
        [ActiveEvent(Name = "Magix.MetaActions.ChangeSimplePropertyValue3")]
        protected void Magix_MetaActions_ChangeSimplePropertyValue3(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                string id = e.Params["ID"].Value.ToString();
                WebPart.WebPartSetting n = WebPart.WebPartSetting.SelectByID(int.Parse(id.Split('|')[0]));
                string oldVal = n.Value;

                string newVal = "";

                if (Action.CountWhere(Criteria.Eq("Name", e.Params["NewValue"].Get<string>())) == 0)
                {
                    ShowMessage("That action doesn't exist ...");
                }

                for (int idx = 0; idx < oldVal.Split('|').Length; idx++)
                {
                    if (idx == int.Parse(id.Split('|')[1]))
                    {
                        newVal += e.Params["NewValue"].Get<string>() + "|";
                    }
                    else
                    {
                        newVal += oldVal.Split('|')[idx] + "|";
                    }
                }

                n.Value = newVal.Trim('|').Replace("||", "|");

                n.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Sink for getting the type information for editing Actions for form element on
         * grid system
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(Action).FullName + "-META3")
            {
                e.Params["Type"]["Properties"]["Up"]["NoFilter"].Value = true;
                e.Params["Type"]["Properties"]["Up"]["Header"].Value = "Up";
                e.Params["Type"]["Properties"]["Up"]["TemplateColumnEvent"].Value = "Magix.MetaForms.GetPushActionUpTemplateColumn3";
                e.Params["Type"]["Properties"]["Down"]["NoFilter"].Value = true;
                e.Params["Type"]["Properties"]["Down"]["Header"].Value = "Dwn";
                e.Params["Type"]["Properties"]["Down"]["TemplateColumnEvent"].Value = "Magix.MetaForms.GetPushActionDownTemplateColumn3";
                e.Params["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
                e.Params["Type"]["Properties"]["Name"]["MaxLength"].Value = 100;
                e.Params["Type"]["Properties"]["Name"]["ControlType"].Value = typeof(InPlaceEdit).FullName;
                e.Params["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
            }
        }

        /**
         * Level2: Returns the Up column for the Action list
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetPushActionUpTemplateColumn3")]
        protected void Magix_MetaForms_GetPushActionUpTemplateColumn3(object sender, ActiveEventArgs e)
        {
            string id = e.Params["ID"].Value.ToString();

            LinkButton b = new LinkButton();
            b.Text = "&uArr;";
            b.CssClass = "span-1 last";

            b.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = id;

                    RaiseEvent(
                        "Magix.MetaForms.PushActionUp3",
                        node);
                };

            e.Params["Control"].Value = b;
        }

        /**
         * Level2: Returns the Down column for the Action list
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetPushActionDownTemplateColumn3")]
        protected void Magix_MetaForms_GetPushActionDownTemplateColumn3(object sender, ActiveEventArgs e)
        {
            string id = e.Params["ID"].Value.ToString();

            LinkButton b = new LinkButton();
            b.Text = "&dArr;";
            b.CssClass = "span-1 last";

            b.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = id;

                    RaiseEvent(
                        "Magix.MetaForms.PushActionDown3",
                        node);
                };

            e.Params["Control"].Value = b;
        }

        /**
         * Level2: Moves an Action up or down depending upon the event name
         */
        [ActiveEvent(Name = "Magix.MetaForms.PushActionUp3")]
        [ActiveEvent(Name = "Magix.MetaForms.PushActionDown3")]
        protected void Magix_MetaForms_PushActionUp3_Down3(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                WebPart.WebPartSetting n = WebPart.WebPartSetting.SelectByID(int.Parse(e.Params["ID"].Get<string>().Split('|')[0]));
                string oldValue = n.Value;

                List<string> tmp = new List<string>(oldValue.Split('|'));
                int pos = int.Parse(e.Params["ID"].Get<string>().Split('|')[1]);
                string act = tmp[pos];

                if (e.Name == "Magix.MetaForms.PushActionUp3")
                {
                    if (pos == 0)
                    {
                        ShowMessage("You can't move that action further up");
                        return;
                    }
                    tmp.RemoveAt(pos);
                    tmp.Insert(pos - 1, act);
                }
                else
                {
                    if (pos == tmp.Count - 1)
                    {
                        ShowMessage("You can't move that action further down");
                        return;
                    }
                    tmp.RemoveAt(pos);
                    tmp.Insert(pos + 1, act);
                }

                string nVal = "";
                foreach (string idx in tmp)
                {
                    nVal += idx + "|";
                }
                n.Value = nVal.Trim('|').Replace("||", "|");
                n.Save();

                tr.Commit();

                Node node = new Node();
                node["FullTypeName"].Value = typeof(Action).FullName + "-META3";

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        /**
         * Level2: Sink for getting the list data for editing Actions for WebPart element
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() != typeof(Action).FullName + "-META3")
                return;

            string actionString = "";
            WebPart.WebPartSetting p = WebPart.WebPartSetting.SelectByID(e.Params["WebPartID"].Get<int>());

            actionString = p.Value;

            int idxNo = 0;
            foreach (string idx in actionString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (idxNo >= e.Params["Start"].Get<int>() &&
                    idxNo < e.Params["End"].Get<int>())
                {
                    e.Params["Objects"]["o-" + idxNo]["ID"].Value = p.ID +
                        "|" +
                        idxNo;
                    e.Params["Objects"]["o-" + idxNo]["Properties"]["Name"].Value = idx;
                }
                idxNo += 1;
            }
            e.Params["SetCount"].Value = idxNo;
            e.Params["LockSetCount"].Value = true;
        }
    }
}
