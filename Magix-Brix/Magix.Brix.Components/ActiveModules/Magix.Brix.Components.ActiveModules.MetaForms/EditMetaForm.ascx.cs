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
using System.Drawing;
using Magix.UX.Effects;
using Magix.UX.Aspects;

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     * Level2: Helper for editing and creating Meta Form objects
     */
    [ActiveModule]
    public class EditMetaForm : ActiveModule
    {
        protected Window tools;
        protected Window props;
        protected System.Web.UI.WebControls.Repeater tlsRep;
        protected Panel ctrls;
        protected Label type;
        protected System.Web.UI.WebControls.Repeater propRep;
        protected Panel propWrp;
        protected Label desc;
        protected Label propHeader;
        protected System.Web.UI.WebControls.Repeater eventRep;
        protected Panel eventWrp;
        protected Label eventHeader;
        protected Panel shortCutWrp;
        protected System.Web.UI.WebControls.Repeater shortCutRep;
        protected SelectList selWidg;
        protected LinkButton formInitActions;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    RaiseSafeEvent(
                        "Magix.MetaForms.GetControlTypes",
                        DataSource);

                    tlsRep.DataSource = DataSource["Controls"];
                    tlsRep.DataBind();

                    SetWindowPropertiesPositionsAccordingToSettings();
                    SetWindowToolsPositionsAccordingToSettings();
                    SetTopWindow();
                    CreateSelectWidgetSelectList();

                    SetFormActive();
                };
        }

        private void CreateSelectWidgetSelectList()
        {
            selWidg.Items.Clear();
            selWidg.Items.Add(new ListItem("Meta Form", ""));
            DataSource["root"]["Surface"].Find(
                delegate(Node idx)
                {
                    if (idx.Name.IndexOf("c-") == 0 && 
                        idx.Contains("TypeName"))
                    {
                        string text = idx["TypeName"].Get<string>();
                        if (idx.Contains("Properties") &&
                            idx["Properties"].Contains("ID"))
                            text += "[" + idx["Properties"]["ID"].Value.ToString() + "]";

                        ListItem li = new ListItem();
                        li.Text = text;
                        li.Value = idx["_ID"].Value.ToString();
                        selWidg.Items.Add(li);
                    }
                    return false;
                });
        }

        private void SetTopWindow()
        {
            Node node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.TopWindow";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                if (node["Section"]["Window"].Get<string>() == "tools")
                {
                    tools.Style[Styles.zIndex] = "501";
                    props.Style[Styles.zIndex] = "500";
                }
                else
                {
                    tools.Style[Styles.zIndex] = "500";
                    props.Style[Styles.zIndex] = "501";
                }
            }
        }

        private void SetWindowToolsPositionsAccordingToSettings()
        {
            Node node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.ToolsWindowPosition";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                tools.Style[Styles.left] = int.Parse(node["Section"]["X"].Get<string>()).ToString() + "px";
                tools.Style[Styles.top] = int.Parse(node["Section"]["Y"].Get<string>()).ToString() + "px";
            }
        }

        private void SetWindowPropertiesPositionsAccordingToSettings()
        {
            Node node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.PropertyWindowPosition";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                props.Style[Styles.left] = int.Parse(node["Section"]["X"].Get<string>()).ToString() + "px";
                props.Style[Styles.top] = int.Parse(node["Section"]["Y"].Get<string>()).ToString() + "px";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CreateFormControls();

            eventHeader.Click +=
                delegate
                {
                    if (props.CssClass.Contains(" mux-hide-events"))
                        props.CssClass = props.CssClass.Replace(" mux-hide-events", "");
                    else
                        props.CssClass += " mux-hide-events";
                };

            propHeader.Click +=
                delegate
                {
                    if (props.CssClass.Contains(" mux-hide-props"))
                        props.CssClass = props.CssClass.Replace(" mux-hide-props", "");
                    else
                        props.CssClass += " mux-hide-props";
                };

            type.ClickEffect = new EffectToggle(desc, 250, false);

            tools.Dragger.Bounds = new Rectangle(-150, 0, 990, 450);
            props.Dragger.Bounds = new Rectangle(-150, 0, 990, 450);
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
            nn["Preview"].Value = true;

            RaiseSafeEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            if (nn.Contains("Control"))
            {
                BaseWebControl ctrl = nn["Control"].Get<BaseWebControl>();
                if (string.IsNullOrEmpty(ctrl.ToolTip))
                    ctrl.ToolTip = "Click me to edit the Widget";
                if (node.Contains("Properties"))
                {
                    foreach (Node idx in node["Properties"])
                    {
                        // Skipping 'empty stuff' ...
                        if (idx.Value == null)
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
                }
                ctrl.Load +=
                    delegate
                    {
                        if (ctrl.ClientID == OldSelected &&
                            !ctrl.CssClass.Contains(" mux-wysiwyg-selected"))
                        {
                            ctrl.CssClass += " mux-wysiwyg-selected";
                            ctrl.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                        }
                    };
                ctrl.Click +=
                    delegate
                    {
                        Control tmp = ctrl;
                        SetActiveControl(node);
                    };
                ctrl.Style[Styles.position] = "relative";

                // Making draggable ...
                AspectDraggable dragger = new AspectDraggable();
                dragger.Dragged +=
                    delegate
                    {
                        int left = int.Parse(ctrl.Style[Styles.left].Replace("px", ""));
                        int top = int.Parse(ctrl.Style[Styles.top].Replace("px", ""));

                        AbsolutizeWidget(left, top, node, ctrl);
                    };
                ctrl.Controls.Add(dragger);

                // Making sure we're rendering the styles needed ...
                RenderStyles(ctrl, node);

                if (string.IsNullOrEmpty(ctrl.ID))
                    ctrl.ID = "ID" + node["_ID"].Value.ToString();

                parent.Controls.Add(ctrl);
            }
        }

        private void RenderStyles(BaseWebControl ctrl, Node node)
        {
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

        private void AbsolutizeWidget(int left, int top, Node node, BaseWebControl ctrl)
        {
            if (left == 0 && top == 0)
                return;

            ctrl.Style[Styles.position] = "absolute";

            Node n = new Node();

            n["ID"].Value = node["_ID"].Value;
            n["Left"].Value = left;
            n["Top"].Value = top;

            RaiseSafeEvent(
                "Magix.MetaForms.AbsolutizeWidget",
                n);
        }

        private string OldSelected
        {
            get { return ViewState["OldSelected"] as string; }
            set { ViewState["OldSelected"] = value; }
        }

        private void SetActiveControl(Node node)
        {
            if (!string.IsNullOrEmpty(OldSelected))
            {
                BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
                c.ToolTip = "Click me to edit the Widget";
                OldSelected = null;
                if (ctrls.CssClass.IndexOf(" mux-control-selected") != -1)
                    ctrls.CssClass = ctrls.CssClass.Replace(" mux-control-selected", "");
            }

            ClearPropertyWindow();

            if (node == null ||
                !node.Contains("TypeName"))
            {
                SetFormActive();
                return;
            }

            string typeName = node["TypeName"].Get<string>();

            Node ctrlType = null;
            if (DataSource.Contains("Controls"))
            {
                foreach (Node idx in DataSource["Controls"])
                {
                    if (idx.Contains("TypeName") &&
                        idx["TypeName"].Get<string>() == typeName)
                    {
                        ctrlType = idx;
                        break;
                    }
                }
            }

            if (ctrlType != null)
            {
                type.Text = ctrlType["Name"].Get<string>();
                type.Info = node["_ID"].Value.ToString();
                desc.Text = ctrlType["ToolTip"].Get<string>();

                propRep.DataSource = ctrlType["Properties"];
                propRep.DataBind();
                propWrp.ReRender();

                eventRep.DataSource = ctrlType["Events"];
                eventRep.DataBind();
                eventWrp.ReRender();

                propHeader.Visible = ctrlType["Properties"].Count > 0;
                eventHeader.Visible = ctrlType["Events"].Count > 0;
                if (ctrls.CssClass.IndexOf(" mux-control-selected") == -1)
                    ctrls.CssClass += " mux-control-selected";

                // Shortcut buttons ...
                if (ctrlType.Contains("ShortCuts"))
                {
                    shortCutRep.DataSource = ctrlType["ShortCuts"];
                    shortCutRep.DataBind();
                    shortCutWrp.ReRender();
                }
            }
            else
            {
                desc.Style[Styles.height] = "0";
                if (ctrls.CssClass.IndexOf(" mux-control-selected") != -1)
                    ctrls.CssClass = ctrls.CssClass.Replace(" mux-control-selected", "");
            }
            selWidg.SetSelectedItemAccordingToValue(node["_ID"].Value.ToString());

            Control ctrl = Selector.FindControl<Control>(ctrls, 
                node.Contains("Properties") && node["Properties"].Contains("ID") ?
                    node["Properties"]["ID"].Value.ToString() : 
                    ("ID" + node["_ID"].Value.ToString()));

            if (ctrl != null)
            {
                OldSelected = ctrl.ClientID;
                if (ctrl is BaseWebControl)
                {
                    BaseWebControl ctrl2 = ctrl as BaseWebControl;
                    ctrl2.CssClass += " mux-wysiwyg-selected";
                    ctrl2.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                }
            }
        }

        private void SetFormActive()
        {
            formInitActions.CssClass = GetCssClassInitActionsForm();
        }

        protected void ShortCutButtonClicked(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = int.Parse(type.Info);

            RaiseSafeEvent(
                (sender as BaseWebControl).Info,
                node);

            if (node.Contains("Refresh"))
            {
                if (!string.IsNullOrEmpty(OldSelected))
                {
                    BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                    c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
                    c.ToolTip = "Click me to edit the Widget";
                }
                OldSelected = "";
                ClearPropertyWindow();

                RaiseSafeEvent(
                    "Magix.MetaForms.GetControlsForForm",
                    DataSource);

                ctrls.Controls.Clear();
                CreateFormControls();
                ctrls.ReRender();

                if (ctrls.CssClass.IndexOf(" mux-control-selected") != -1)
                    ctrls.CssClass = ctrls.CssClass.Replace(" mux-control-selected", "");
            }
        }

        protected void ActionsClicked(object sender, EventArgs e)
        {
            LinkButton b = sender as LinkButton;
            string eventName = b.Info;

            Node node = new Node();

            node["EventName"].Value = eventName;
            node["ID"].Value = int.Parse(type.Info);

            RaiseSafeEvent(
                "Magix.MetaForms.ShowAllActionsAssociatedWithFormEvent",
                node);
        }

        protected string GetCssClassInitActionsForm()
        {
            if (DataSource["root"].Contains("Actions") &&
                DataSource["root"]["Actions"].Contains("InitialLoading"))
            {
                if (!string.IsNullOrEmpty(DataSource["root"]["Actions"]["InitialLoading"].Get<string>()))
                {
                    return "span-2 last mux-has-actions";
                }
            }
            return "span-2 last mux-no-actions";
        }

        protected string GetCssClass(object tmp)
        {
            Node n = DataSource.Find(
                delegate(Node idx)
                {
                    if (idx.Contains("_ID") && idx["_ID"].Value.ToString() == type.Info)
                        return true;
                    return false;
                });
            if (n.Contains("Actions") && 
                n["Actions"].Contains((tmp as Node).Parent.Name) &&
                !string.IsNullOrEmpty(n["Actions"][(tmp as Node).Parent.Name].Get<string>()))
                return "mux-has-actions";
            return "mux-no-actions";
        }

        private void ClearPropertyWindow()
        {
            selWidg.SelectedIndex = 0;

            propHeader.Visible = false;
            eventHeader.Visible = false;
            type.Text = "";
            type.Info = "";
            desc.Text = "";
            propRep.DataSource = null;
            propRep.DataBind();
            propWrp.ReRender();
            eventRep.DataSource = null;
            eventRep.DataBind();
            eventWrp.ReRender();
            shortCutRep.DataSource = null;
            shortCutRep.DataBind();
            shortCutWrp.ReRender();
        }

        protected void formInitActions_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["EventName"].Value = "InitialLoading";

            RaiseSafeEvent(
                "Magix.MetaForms.ShowAllActionsAssociatedWithMainFormEvent",
                node);
        }

        protected void ctrls_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(OldSelected))
            {
                BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
                c.ToolTip = "Click me to edit the Widget";
            }
            OldSelected = null;
            ClearPropertyWindow();
            if (ctrls.CssClass.IndexOf(" mux-control-selected") != -1)
                ctrls.CssClass = ctrls.CssClass.Replace(" mux-control-selected", "");

            SetFormActive();
        }

        protected string GetPropertyValue(object inpNode)
        {
            string retVal = null;
            string nodeName = inpNode as string;
            Node tmp = DataSource["root"].Find(
                delegate(Node idx)
                {
                    if (idx.Contains("_ID") &&
                        idx["_ID"].Get<int>().ToString() == type.Info)
                    {
                        return true;
                    }
                    return false;
                });
            bool found = false;
            if (tmp != null && tmp.Contains("Properties"))
            {
                foreach (Node idx in tmp["Properties"])
                {
                    if (idx.Name == nodeName)
                    {
                        if (idx.Value != null)
                        {
                            retVal = idx.Value.ToString();
                            found = true;
                            break;
                        }
                    }
                }
            }
            if (!found)
            {
                // Looking for default value ...
                Node typeNode = DataSource["Controls"].Find(
                    delegate(Node idx)
                    {
                        if (idx.Contains("TypeName") &&
                            idx["TypeName"].Get<string>() == tmp["TypeName"].Get<string>())
                            return true;
                        return false;
                    });
                if (DataSource["Controls"][typeNode.Name]["Properties"][nodeName].Contains("Default"))
                    retVal = DataSource["Controls"][typeNode.Name]["Properties"][nodeName]["Default"].Value.ToString();
            }
            return retVal;
        }

        protected bool GetPropertyValueBool(object inpNode)
        {
            bool retVal = false;
            string nodeName = inpNode as string;
            Node tmp = DataSource["root"].Find(
                delegate(Node idx)
                {
                    if (idx.Contains("_ID") &&
                        idx["_ID"].Get<int>().ToString() == type.Info)
                    {
                        return true;
                    }
                    return false;
                });
            bool found = false;
            if (tmp != null && tmp.Contains("Properties"))
            {
                foreach (Node idx in tmp["Properties"])
                {
                    if (idx.Name == nodeName)
                    {
                        if (idx.Value != null && 
                            idx.Value is bool)
                        {
                            retVal = (bool)idx.Value;
                            found = true;
                            break;
                        }
                    }
                }
            }
            if (!found)
            {
                // Looking for default value ...
                Node typeNode = DataSource["Controls"].Find(
                    delegate(Node idx)
                    {
                        if (idx.Contains("TypeName") &&
                            idx["TypeName"].Get<string>() == tmp["TypeName"].Get<string>())
                            return true;
                        return false;
                    });
                if (DataSource["Controls"][typeNode.Name]["Properties"][nodeName].Contains("Default") &&
                    DataSource["Controls"][typeNode.Name]["Properties"][nodeName]["Default"].Value is bool)
                    retVal = DataSource["Controls"][typeNode.Name]["Properties"][nodeName]["Default"].Get<bool>();
            }
            return retVal;
        }

        protected void PropertyValueChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["Value"].Value = (sender as TextAreaEdit).Text;
            node["ControlID"].Value = int.Parse(type.Info);
            node["PropertyName"].Value = (sender as TextAreaEdit).Info;

            RaiseSafeEvent(
                "Magix.MetaForms.ChangeFormPropertyValue",
                node);

            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();
        }

        protected void PropertyValueIntChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["Value"].Value = (sender as InPlaceEdit).Text;
            node["ControlID"].Value = int.Parse(type.Info);
            node["PropertyName"].Value = (sender as InPlaceEdit).Info;

            RaiseSafeEvent(
                "Magix.MetaForms.ChangeFormPropertyValue",
                node);

            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();
        }

        protected void PropertyValueBoolChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["Value"].Value = (sender as CheckBox).Checked;
            node["ControlID"].Value = int.Parse(type.Info);
            node["PropertyName"].Value = (sender as CheckBox).Info;

            RaiseSafeEvent(
                "Magix.MetaForms.ChangeFormPropertyValue",
                node);

            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();
        }

        protected void AddControlToPage(object sender, EventArgs e)
        {
            LinkButton b = sender as LinkButton;
            string typeName = b.Info;

            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;
            node["ParentControl"].Value = null; // Meaning the 'Root Node' will be the parent control
            node["TypeName"].Value = b.Info;

            RaiseSafeEvent(
                "Magix.MetaForms.AppendControlToForm",
                node);

            // Refreshing form ...
            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();
        }

        protected void selWidg_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = selWidg.SelectedItem.Value;
            ClearPropertyWindow();
            if (string.IsNullOrEmpty(sel))
            {
                SetActiveControl(null);
                return; // No controls selected ...
            }


            Node ct = DataSource["root"]["Surface"].Find(
                delegate(Node idx)
                {
                    if (idx.Name == "_ID" &&
                        idx.Value.ToString() == sel)
                    {
                        return true;
                    }
                    return false;
                });
            if (ct != null)
                SetActiveControl(ct.Parent);
        }

        protected void tools_Dragged(object sender, EventArgs e)
        {
            int x = int.Parse(tools.Style[Styles.left].Replace("px", ""));
            int y = int.Parse(tools.Style[Styles.top].Replace("px", ""));

            Node node = new Node();

            node["Section"]["X"].Value = x.ToString();
            node["Section"]["Y"].Value = y.ToString();
            node["SectionName"].Value = "Magix.MetaForms.ToolsWindowPosition";

            RaiseSafeEvent(
                "Magix.Core.SaveSettingsSection",
                node);
        }

        protected void props_Dragged(object sender, EventArgs e)
        {
            int x = int.Parse(props.Style[Styles.left].Replace("px", ""));
            int y = int.Parse(props.Style[Styles.top].Replace("px", ""));

            Node node = new Node();

            node["Section"]["X"].Value = x.ToString();
            node["Section"]["Y"].Value = y.ToString();
            node["SectionName"].Value = "Magix.MetaForms.PropertyWindowPosition";

            RaiseSafeEvent(
                "Magix.Core.SaveSettingsSection",
                node);
        }

        protected void tools_Click(object sender, EventArgs e)
        {
            if (props.Style[Styles.zIndex] != "500")
            {
                props.Style[Styles.zIndex] = "500";
                tools.Style[Styles.zIndex] = "501";

                Node node = new Node();

                node["SectionName"].Value = "Magix.MetaForms.TopWindow";
                node["Section"]["Window"].Value = "tools";

                RaiseSafeEvent(
                    "Magix.Core.SaveSettingsSection",
                    node);
            }
        }

        protected void props_Click(object sender, EventArgs e)
        {
            if (tools.Style[Styles.zIndex] != "500")
            {
                tools.Style[Styles.zIndex] = "500";
                props.Style[Styles.zIndex] = "501";

                Node node = new Node();

                node["SectionName"].Value = "Magix.MetaForms.TopWindow";
                node["Section"]["Window"].Value = "props";

                RaiseSafeEvent(
                    "Magix.Core.SaveSettingsSection",
                    node);
            }
        }

        /**
         * Level2: Refreshes and re-databinds the currently edited Meta Form. Useful if you've 
         * got wizards and similar types of Action Buttons that modifies the form/widgets somehow, 
         * and you need to re-render them with their new settings
         */
        [ActiveEvent(Name = "Magix.MetaForms.RefreshEditableMetaForm")]
        protected void Magix_MetaForms_RefreshEditableMetaForm(object sender, ActiveEventArgs e)
        {
            // Refreshing form ...
            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();

            formInitActions.CssClass = GetCssClassInitActionsForm();

            if (string.IsNullOrEmpty(selWidg.SelectedItem.Value))
                return; // Might be our Form ...

            Node ctrlType = null;
            Node typeNode = DataSource["root"]["Surface"].Find(
                delegate(Node idx)
                {
                    return idx.Name == "_ID" &&
                        idx.Value.ToString() == selWidg.SelectedItem.Value;
                }).Parent;
            string typeName = typeNode["TypeName"].Get<string>();
            if (DataSource.Contains("Controls"))
            {
                foreach (Node idx in DataSource["Controls"])
                {
                    if (idx.Contains("TypeName") &&
                        idx["TypeName"].Get<string>() == typeName)
                    {
                        ctrlType = idx;
                        break;
                    }
                }
            }

            if (ctrlType != null)
            {
                propRep.DataSource = ctrlType["Properties"];
                propRep.DataBind();
                propWrp.ReRender();

                eventRep.DataSource = ctrlType["Events"];
                eventRep.DataBind();
                eventWrp.ReRender();
            }
        }
    }
}
