/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX;
using Magix.UX.Widgets;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Magix.UX.Widgets.Core;
using Magix.Brix.Publishing.Common;

namespace Magix.Brix.Components.ActiveModules.MetaView
{
    /**
     * Level2: UI parts for showing a MetaView in 'SingleView Mode'. Basically shows a form, with items
     * dependent upon the look of the view. This is a Publisher Plugin module. This form expects
     * to be given a 'MetaViewName', which will serve as the foundation for raising the
     * 'Magix.MetaView.GetViewData' event, whos default implementation will populate the node
     * structure according to the views content in a Key/Value pair kind of relationship.
     * This will serv as the foundation for the module to know which types of controls it needs to load
     * up [TextBoxes, Buttons etc]
     * 
     * Handles the 'Magix.MetaView.SerializeSingleViewForm' event, which is the foundation for creating
     * new objects upon clicking Save buttons etc.
     * 
     * This is the PublisherPlugin you'd use if you'd like to have the end user being able to 
     * create a new MetaObject
     */
    [ActiveModule]
    [PublisherPlugin]
    public class MetaView_Single : ActiveModule
    {
        protected Panel ctrls;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    if (!node.Contains("MetaViewTypeName"))
                    {
                        // Probably in 'production mode' and hence need to get our data ...
                        node["MetaViewName"].Value = ViewName;

                        RaiseSafeEvent(
                            "Magix.MetaView.GetViewData",
                            node);
                    }
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindCtrls();

            Button b = Selector.SelectFirst<Button>(ctrls);
            if (b != null)
                ctrls.DefaultWidget = b.ID;
        }

        private void DataBindCtrls()
        {
            foreach (Node idx in DataSource["Properties"])
            {
                if (!string.IsNullOrEmpty(idx["Action"].Get<string>()))
                {
                    CreateActionControl(idx);
                }
                else if (idx["ReadOnly"].Get<bool>())
                {
                    CreateReadOnlyControl(idx, true);
                }
                else
                {
                    CreateReadWriteControl(idx, true);
                }
            }
        }

        private void CreateActionControl(Node idx)
        {
            Button b = new Button();
            b.Text = idx["Name"].Get<string>();
            b.ID = "b-" + idx["ID"].Get<int>();
            if (idx.Contains("ReadOnly"))
                b.Enabled = !idx["ReadOnly"].Get<bool>();
            b.CssClass = "action-button";
            b.Info = idx["Name"].Get<string>();
            b.ToolTip = idx["Description"].Get<string>();
            b.Click +=
                delegate
                {
                    // TODO: Out-factor into controller
                    foreach (string idxS in idx["Action"].Get<string>().Split('|'))
                    {
                        Node node = new Node();

                        node["ActionSenderName"].Value = b.Text;
                        node["MetaViewName"].Value = DataSource["MetaViewName"].Value;
                        node["MetaViewTypeName"].Value = DataSource["MetaViewTypeName"].Value;

                        GetPropertyValues(node["PropertyValues"], false);

                        // Settings Event Specific Features ...
                        node["ActionName"].Value = idxS;
                        node["OriginalWebPartID"].Value = DataSource["OriginalWebPartID"].Value;

                        RaiseSafeEvent(
                            "Magix.MetaAction.RaiseAction",
                            node);
                    }
                };
            ctrls.Controls.Add(b);
        }

        private void GetPropertyValues(Node node, bool flat)
        {
            foreach (Control idx in ctrls.Controls)
            {
                BaseWebControl w = idx as BaseWebControl;
                if (w != null)
                {
                    if (w is TextBox)
                    {
                        if (flat)
                        {
                            node[w.Info].Value = (w as TextBox).Text;
                        }
                        else
                        {
                            node[w.Info]["Value"].Value = (w as TextBox).Text;
                            node[w.Info]["Name"].Value = w.Info;
                        }
                    }
                }
            }
        }

        // TODO: Out-factor all of these to the controller ...
        // Make it more similar [hopefully shareable] between the 'MultiView' logic ...!!
        private void CreateReadOnlyControl(Node idx, bool shouldClear)
        {
            Label lbl = new Label();
            lbl.ToolTip = idx["Name"].Get<string>();
            lbl.Info = idx["Name"].Get<string>();
            lbl.Text = idx["Description"].Get<string>();
            lbl.CssClass = "meta-view-form-element meta-view-form-label";

            if (shouldClear)
                lbl.CssClass += " clear-both";

            ctrls.Controls.Add(lbl);
        }

        private void CreateReadWriteControl(Node idx, bool shouldClear)
        {
            TextBox b = new TextBox();
            b.PlaceHolder = idx["Description"].Get<string>();
            b.ToolTip = b.PlaceHolder;
            b.Info = idx["Name"].Get<string>();
            b.CssClass = "meta-view-form-element meta-view-form-textbox";

            if (shouldClear)
                b.CssClass += " clear-both";

            ctrls.Controls.Add(b);
        }

        /**
         * Level2: Will return the Container's ID back to caller [e.g. "content1"] if it's the
         * correct WebPartTemplate Container according to the requested 'PageObjectTemplateID'
         */
        [ActiveEvent(Name = "Magix.MetaView.GetWebPartsContainer")]
        protected void Magix_MetaView_GetWebPartsContainer(object sender, ActiveEventArgs e)
        {
            if (e.Params["OriginalWebPartID"].Get<int>() == DataSource["OriginalWebPartID"].Get<int>())
                e.Params["ID"].Value = this.Parent.ID;
        }

        /**
         * Level2: Will serialize the form into a key/value pair back to the caller. Basically the foundation
         * for this control's ability to create MetaObjects. Create an action, encapsulating this event,
         * instantiate it and raise it [somehow] when user is done, by attaching it to e.g. a Save button,
         * and have the form serialized into a brand new MetaObject of the given TypeName
         */
        [ActiveEvent(Name = "Magix.MetaView.SerializeSingleViewForm")]
        protected void Magix_MetaView_SerializeSingleViewForm(object sender, ActiveEventArgs e)
        {
            if (e.Params["OriginalWebPartID"].Get<int>() == DataSource["OriginalWebPartID"].Get<int>())
            {
                GetPropertyValues(e.Params, true);
            }
        }

        /**
         * Level2: Will 'empty' the current form. Useful in combination with Save or Clear button
         */
        [ActiveEvent(Name = "Magix.Meta.Actions.EmptyForm")]
        protected void Magix_Meta_Actions_EmptyForm(object sender, ActiveEventArgs e)
        {
            foreach (Control idx in ctrls.Controls)
            {
                BaseWebControl b = idx as BaseWebControl;
                if (b != null)
                {
                    TextBox t = b as TextBox;
                    if (t != null)
                    {
                        t.Text = "";
                    }
                }
            }
        }

        /**
         * Level2: The name of the MetaView to use as the foundation for this form
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.MetaView.MetaView_Single.GetTemplateColumnSelectView")]
        public string ViewName
        {
            get { return ViewState["ViewName"] as string; }
            set { ViewState["ViewName"] = value; }
        }
    }
}
