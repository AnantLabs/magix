/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    [ActiveController]
    public class MetaActionController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Caption"].Value = "MetaTypes";

            e.Params["Items"]["MetaType"]["Items"]["Types"]["Caption"].Value = "View Objects ...";
            e.Params["Items"]["MetaType"]["Items"]["Types"]["Event"]["Name"].Value = "Magix.MetaType.OpenMetaTypeDashboard";

            e.Params["Items"]["MetaType"]["Items"]["Actions"]["Caption"].Value = "View Actions ...";
            e.Params["Items"]["MetaType"]["Items"]["Actions"]["Event"]["Name"].Value = "Magix.MetaType.ViewActions";
        }

        [ActiveEvent(Name = "Magix.MetaType.ViewActions")]
        protected void Magix_MetaType_ViewActions(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Description"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["EventName"]["ForcedWidth"].Value = 4;
            node["WhiteListColumns"]["Params"].Value = true;
            node["WhiteListColumns"]["Params"]["ForcedWidth"].Value = 2;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditAction";
            node["CreateEventName"].Value = "Magix.Meta.CreateAction";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 10;
            node["Type"]["Properties"]["EventName"]["Header"].Value = "Action";
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Description"]["MaxLength"].Value = 20;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.Meta.CreateAction")]
        protected void Magix_Meta_CreateAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = new Action();
                a.Name = "Default, please change";
                a.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Meta.EditAction")]
        protected void Magix_Meta_EditAction(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ID"].Get<int>());
            EditActionItemParams(a, e.Params);
            EditActionItem(a, e.Params);

            CreateRunParamButton(a);

            CreateCreateParamButton(a);

            CreateDeleteParamButton(a);

            ActiveEvents.Instance.RaiseClearControls("content6");
        }

        private void CreateRunParamButton(Action a)
        {
            Node node = new Node();

            node["Text"].Value = "Run!";
            node["ButtonCssClass"].Value = "span-4";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.Meta.RaiseEvent";
            node["Event"]["ActionID"].Value = a.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

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

        [ActiveEvent(Name = "Magix.Meta.CreateParameter")]
        protected void Magix_Meta_CreateParameter(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = Action.SelectByID(e.Params["ID"].Get<int>());

                Node tmp = new Node();

                RaiseEvent(
                    "Magix.Core.GetTreeSelectedID",
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
            }

            // Signalizing to Grids that they need to update ...
            Node n = new Node();
            n["FullTypeName"].Value = typeof(Action).FullName + 
                "|" +
                typeof(Action.ActionParams).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);
        }

        private void EditActionItem(Action a, Node node)
        {
            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["EventName"].Value = true;
            node["WhiteListColumns"]["StripInput"].Value = true;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Params"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["EventName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["StripInput"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Description"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Container"].Value = "content5";
            node["Top"].Value = 2;
            node["MarginBottom"].Value = 10;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        private void EditActionItemParams(Action a, Node e)
        {
            Node node = new Node();

            node["CssClass"].Value = "clear-left";
            node["Width"].Value = 6;
            node["MarginBottom"].Value = 10;
            node["Top"].Value = 1;
            node["FullTypeName"].Value = typeof(Action.ActionParams).FullName;
            node["ActionItemID"].Value = a.ID;
            node["ItemSelectedEvent"].Value = "Magix.Meta.EditParam";

            RaiseEvent(
                "Magix.Meta.GetActionItemTree",
                node);

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Tree",
                "content4",
                node);
        }

        [ActiveEvent(Name = "Magix.Meta.EditParam")]
        private void Magix_Meta_EditParam(object sender, ActiveEventArgs e)
        {
            Action.ActionParams p = Action.ActionParams.SelectByID(e.Params["SelectedItemID"].Get<int>());

            Node ch = new Node();
            ch["ID"].Value = p.ID;
            RaiseEvent(
                "Magix.Core.CheckIfIDIsBeingSingleEdited",
                ch);
            if (ch.Contains("Yes"))
                return;

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
            node["Type"]["Properties"]["TypeName"]["TemplateColumnEvent"].Value = "Magix.Publishing.GetMetaTypeName";

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Container"].Value = "content6";
            node["IsList"].Value = false;
            node["CssClass"].Value = "small-editer";
            node["PullTop"].Value = 8;
            node["MarginBottom"].Value = 10;
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

        [ActiveEvent(Name = "Magix.Publishing.GetMetaTypeName")]
        private void Magix_Publishing_GetMetaTypeName(object sender, ActiveEventArgs e)
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

        [ActiveEvent(Name = "Magix.Meta.GetActionItemTree")]
        private void Magix_Meta_GetActionItemTree(object sender, ActiveEventArgs e)
        {
            Action a = Action.SelectByID(e.Params["ActionItemID"].Get<int>());
            foreach (Action.ActionParams idx in a.Params)
            {
                AddParamToNode(idx, e.Params);
            }
        }

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
    }
}





















