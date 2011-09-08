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
using System.Reflection;

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     */
    [ActiveModule]
    [PublisherPlugin]
    public class MetaView_Form : ActiveModule
    {
        protected Panel ctrls;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    DataSource["MetaFormName"].Value = MetaFormName;
                    RaiseSafeEvent(
                        "Magix.MetaForms.GetControlsForForm",
                        DataSource);
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateFormControls();
        }

        private void CreateFormControls()
        {
            if (DataSource.Contains("root") &&
                DataSource["root"].Contains("Surface"))
            {
                foreach (Node idx in DataSource["root"]["Surface"])
                {
                    CreateSingleControl(idx, ctrls);
                }
            }
        }

        private void CreateSingleControl(Node node, BaseWebControl parent)
        {
            Node nn = new Node();

            nn["TypeName"].Value = node["TypeName"].Get<string>();

            RaiseSafeEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            if (nn.Contains("Control"))
            {
                BaseWebControl ctrl = nn["Control"].Get<BaseWebControl>();
                if (node.Contains("Properties"))
                {
                    foreach (Node idx in node["Properties"])
                    {
                        PropertyInfo info = ctrl.GetType().GetProperty(
                            idx.Name,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Public);
                        if (info != null)
                            info.GetSetMethod(true).Invoke(ctrl, new object[] { idx.Value });
                    }
                    foreach (Node idx in node["Actions"])
                    {
                        EventInfo info = ctrl.GetType().GetEvent(
                            idx.Name,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Public);
                        if (info != null)
                        {
                            MethodInfo method = 
                                this.GetType()
                                    .GetMethod(
                                        "RaiseActions", 
                                        BindingFlags.NonPublic | 
                                        BindingFlags.Instance |
                                        BindingFlags.FlattenHierarchy);
                            Delegate del = Delegate.CreateDelegate(
                                info.EventHandlerType,
                                this,
                                method);
                            info.AddEventHandler(
                                ctrl,
                                del);
                        }
                    }
                }
                parent.Controls.Add(ctrl);
            }
        }

        protected void RaiseActions(object sender, EventArgs e)
        {
            BaseWebControl webCtrl = sender as BaseWebControl;
            if (webCtrl != null)
            {
            }
            else
            {
                // Might still be other types of MUX controls, such as Timers etc ...
                BaseControl ctrl = sender as BaseControl;
                if (ctrl == null)
                {
                    throw new ApplicationException("Sorry, but we only currently support MUX controls. However, these are easy to create yourself by making sure your own Ajax Controls inherits from BaseControl or BaseWebControl [preferably]");
                }
            }
        }

        /**
         * Level2: Which form to load up into the Meta Form View
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.MetaForms.MetaView_Form.GetTemplateColumnSelectForm")]
        public string MetaFormName
        {
            get { return ViewState["MetaFormName"] as string; }
            set { ViewState["MetaFormName"] = value; }
        }
    }
}
