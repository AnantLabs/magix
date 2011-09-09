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
using System.Collections.Generic;

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
                Control ctrl = nn["Control"].Get<Control>();

                if (node.Contains("Properties"))
                {
                    foreach (Node idx in node["Properties"])
                    {
                        // Skipping 'empty stuff' ...
                        if (idx.Value == null)
                            continue;

                        if (idx.Value is string && (idx.Value as string) == string.Empty)
                            continue;

                        PropertyInfo info = ctrl.GetType().GetProperty(
                            idx.Name,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Public);

                        if (info != null)
                        {
                            object tmp = idx.Value;

                            if (tmp.GetType() != info.GetGetMethod(true).ReturnType)
                            {
                                switch (info.GetGetMethod(true).ReturnType.FullName)
                                {
                                    case "System.Boolean":
                                        tmp = bool.Parse(tmp.ToString());
                                        break;
                                    case "System.DateTime":
                                        tmp = DateTime.ParseExact(tmp.ToString(), "yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Int32":
                                        tmp = int.Parse(tmp.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "System.Decimal":
                                        tmp = decimal.Parse(tmp.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    default:
                                        if (info.GetGetMethod(true).ReturnType.BaseType == typeof(Enum))
                                            tmp = Enum.Parse(info.GetGetMethod(true).ReturnType, tmp.ToString());
                                        else
                                            throw new ApplicationException("Unsupported type for serializing to Widget, type was: " + info.GetGetMethod(true).ReturnType.FullName);
                                        break;
                                }
                                info.GetSetMethod(true).Invoke(ctrl, new object[] { tmp });
                            }
                            else
                                info.GetSetMethod(true).Invoke(ctrl, new object[] { tmp });
                        }
                    }
                    foreach (Node idx in node["Actions"])
                    {
                        // Skipping 'empty stuff' ...
                        if (idx.Value == null)
                            continue;

                        if (idx.Value is string && (idx.Value as string) == string.Empty)
                            continue;

                        EventInfo info = ctrl.GetType().GetEvent(
                            idx.Name,
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Public);

                        if (info != null)
                        {
                            // Helper logic to keep event name for being able to do 
                            // lookup into action lists according to control and event name ...
                            ActionWrapper wrp = new ActionWrapper();
                            wrp.EventName = idx.Name;
                            wrp.Form = this;

                            MethodInfo method = 
                                typeof(ActionWrapper)
                                    .GetMethod(
                                        "RaiseActions", 
                                        BindingFlags.NonPublic | 
                                        BindingFlags.Instance |
                                        BindingFlags.FlattenHierarchy);

                            if (FirstLoad)
                            {
                                // We only do this once, since we're storing it 'cached' in the ViewState
                                // upon FirstLoad of Module ...
                                string evtName = idx.Name;
                                string evts = idx.Value.ToString();

                                ctrl.Init +=
                                    delegate
                                    {
                                        if (!ActionsForWidgets.ContainsKey(ctrl.ClientID))
                                            ActionsForWidgets[ctrl.ClientID] = new Dictionary<string, string>();
                                        ActionsForWidgets[ctrl.ClientID][evtName] = evts;
                                    };
                            }

                            Delegate del = Delegate.CreateDelegate(
                                info.EventHandlerType,
                                wrp,
                                method);

                            info.AddEventHandler(
                                ctrl,
                                del);
                        }
                    }
                }

                // Making sure we're rendering the styles needed ...
                RenderStyles(ctrl as BaseWebControl, node);

                parent.Controls.Add(ctrl);
            }
        }

        private void RenderStyles(BaseWebControl ctrl, Node node)
        {
            if (ctrl == null)
                return;

            if (node.Contains("Properties") &&
                node["Properties"].Contains("Style"))
            {
                foreach (Node idx in node["Properties"]["Style"])
                {
                    if (idx.Name == "_ID")
                        continue;

                    if (!string.IsNullOrEmpty(idx.Get<string>()))
                        ctrl.Style[idx.Name] = idx.Get<string>();
                }
            }
        }

        private class ActionWrapper
        {
            public string EventName;
            public MetaView_Form Form;

            protected void RaiseActions(object sender, EventArgs e)
            {
                Form.RaiseActionsForEvent(sender, e, EventName);
            }
        }

        internal void RaiseActionsForEvent(object sender, EventArgs e, string EventName)
        {
            string actions = ActionsForWidgets[(sender as Control).ClientID][EventName];

            DataSource["ActionsToExecute"].Value = actions;

            RaiseSafeEvent(
                "Magix.MetaForms.RaiseActionsFromActionString",
                DataSource);

            DataSource["ActionsToExecute"].UnTie();
        }

        private Dictionary<string, Dictionary<string, string>> ActionsForWidgets
        {
            get
            {
                if (ViewState["ActionsForWidgets"] == null)
                    ViewState["ActionsForWidgets"] = new Dictionary<string, Dictionary<string, string>>();
                return ViewState["ActionsForWidgets"] as Dictionary<string, Dictionary<string, string>>;
            }
        }

        /**
         * Level2: Which form to load up into the Meta Form View. Must be the unique Name to a Meta Form object
         */
        [ModuleSetting(ModuleEditorEventName = "Magix.MetaForms.MetaView_Form.GetTemplateColumnSelectForm")]
        public string MetaFormName
        {
            get { return ViewState["MetaFormName"] as string; }
            set { ViewState["MetaFormName"] = value; }
        }
    }
}
