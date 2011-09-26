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
        protected CheckBox chkAllowDragging;

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
                    SetPropertiesEventsVisible();
                    CreateSelectWidgetSelectList();

                    SetFormActive();

                    GetDragAndDropSettings();
                };
        }

        private void GetDragAndDropSettings()
        {
            Node node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.AllowDragAndDrop";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                if (node["Section"]["Allow"].Value != null &&
                    node["Section"]["Allow"].Value.ToString() == "True")
                {
                    DataSource["DragAndDrop"].Value = true;
                }
                else
                {
                    DataSource["DragAndDrop"].Value = false;
                }
                chkAllowDragging.Checked = DataSource["DragAndDrop"].Get<bool>();
            }
        }

        private void SetPropertiesEventsVisible()
        {
            Node node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.ShowOrHideEvents";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                if (node["Section"]["HideEvents"].Value != null &&
                    node["Section"]["HideEvents"].Value.ToString() == "True")
                {
                    if (!props.CssClass.Contains(" mux-hide-events"))
                        props.CssClass += " mux-hide-events";
                }
                else
                {
                    if (props.CssClass.Contains(" mux-hide-events"))
                        props.CssClass = props.CssClass.Replace(" mux-hide-events", "");
                }
            }

            node = new Node();

            node["SectionName"].Value = "Magix.MetaForms.ShowOrHideProperties";

            RaiseSafeEvent(
                "Magix.Core.LoadSettingsSection",
                node);

            if (node.Contains("Section"))
            {
                if (node["Section"]["HideProperties"].Value != null &&
                    node["Section"]["HideProperties"].Value.ToString() == "True")
                {
                    if (!props.CssClass.Contains(" mux-hide-props"))
                        props.CssClass += " mux-hide-props";
                }
                else
                {
                    if (props.CssClass.Contains(" mux-hide-props"))
                        props.CssClass = props.CssClass.Replace(" mux-hide-props", "");
                }
            }
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

                        string min = "";
                        Node tmpN = idx.Parent;
                        while (tmpN != null)
                        {
                            if (tmpN.Name != null &&
                                tmpN.Name.StartsWith("c-"))
                                min += "-";

                            tmpN = tmpN.Parent;
                        }

                        ListItem li = new ListItem();
                        li.Text += min + text;
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
                    tools.Style[Styles.zIndex] = "501";
                    props.Style[Styles.zIndex] = "500";
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
                    {
                        props.CssClass = props.CssClass.Replace(" mux-hide-events", "");

                        Node node = new Node();

                        node["Section"]["HideEvents"].Value = false.ToString();
                        node["SectionName"].Value = "Magix.MetaForms.ShowOrHideEvents";

                        RaiseSafeEvent(
                            "Magix.Core.SaveSettingsSection",
                            node);
                    }
                    else
                    {
                        props.CssClass += " mux-hide-events";

                        Node node = new Node();

                        node["Section"]["HideEvents"].Value = true.ToString();
                        node["SectionName"].Value = "Magix.MetaForms.ShowOrHideEvents";

                        RaiseSafeEvent(
                            "Magix.Core.SaveSettingsSection",
                            node);
                    }
                };

            propHeader.Click +=
                delegate
                {
                    if (props.CssClass.Contains(" mux-hide-props"))
                    {
                        props.CssClass = props.CssClass.Replace(" mux-hide-props", "");

                        Node node = new Node();

                        node["Section"]["HideProperties"].Value = false.ToString();
                        node["SectionName"].Value = "Magix.MetaForms.ShowOrHideProperties";

                        RaiseSafeEvent(
                            "Magix.Core.SaveSettingsSection",
                            node);
                    }
                    else
                    {
                        props.CssClass += " mux-hide-props";

                        Node node = new Node();

                        node["Section"]["HideProperties"].Value = true.ToString();
                        node["SectionName"].Value = "Magix.MetaForms.ShowOrHideProperties";

                        RaiseSafeEvent(
                            "Magix.Core.SaveSettingsSection",
                            node);
                    }
                };

            type.ClickEffect = new EffectToggle(desc, 250, true);
        }

        protected void chkAllowDragging_CheckedChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["Section"]["Allow"].Value = (sender as CheckBox).Checked.ToString();
            node["SectionName"].Value = "Magix.MetaForms.AllowDragAndDrop";

            RaiseSafeEvent(
                "Magix.Core.SaveSettingsSection",
                node);

            DataSource["DragAndDrop"].Value = (sender as CheckBox).Checked;

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
        }

        private void CreateFormControls()
        {
            if (DataSource.Contains("root") && 
                DataSource["root"].Contains("Surface"))
            {
                foreach (Node idx in DataSource["root"]["Surface"])
                {
                    if (idx.Name == "_ID")
                        continue;

                    CreateSingleControl(idx, ctrls);
                }
            }
        }

        private void CreateSingleControl(Node node, Control parent)
        {
            Node nn = new Node();

            nn["TypeName"].Value = node["TypeName"].Get<string>();
            nn["Preview"].Value = true;
            nn["ControlNode"].Value = node;
            nn["_ID"].Value = node["_ID"].Value;

            if (!string.IsNullOrEmpty(OldSelected))
                nn["OldSelected"].Value = OldSelected;

            RaiseSafeEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            nn["ControlNode"].UnTie(); // to be sure ....

            if (nn.Contains("Control"))
            {
                if (nn.Contains("HasSurface") && 
                    !node.Contains("Surface"))
                {
                    node.Add(new Node("Surface"));
                }
                Control ctrl = nn["Control"].Get<Control>();

                // Child controls
                if (node.Contains("Surface"))
                {
                    if (nn.Contains("CreateChildControlsEvent"))
                    {
                        Node tmp = new Node();

                        // Yup, looks stupidish, but feel very safe ... ;)
                        tmp["Controls"].Value = node["Surface"];
                        tmp["Control"].Value = ctrl;
                        tmp["Preview"].Value = true;
                        if (!string.IsNullOrEmpty(OldSelected))
                            tmp["OldSelected"].Value = OldSelected;

                        RaiseEvent( // No safe here, if this one fucks up, we're fucked ... !!
                            nn["CreateChildControlsEvent"].Get<string>(),
                            tmp);
                    }
                    else
                    {
                        foreach (Node idx in node["Surface"])
                        {
                            if (idx.Name == "_ID")
                                continue;

                            CreateSingleControl(idx, ctrl);
                        }
                    }
                }

                ctrl.Load +=
                    delegate
                    {
                        if (ctrl is BaseWebControl)
                        {
                            if ((ctrl as BaseWebControl).ClientID == OldSelected &&
                                !(ctrl as BaseWebControl).CssClass.Contains(" mux-wysiwyg-selected"))
                            {
                                AddSelectedCssClass(ctrl);
                            }
                        }
                        else if (ctrl.ClientID == OldSelected)
                        {
                            AddSelectedCssClass(ctrl);
                        }
                    };

                if (ctrl is BaseWebControl)
                {
                    (ctrl as BaseWebControl).Click +=
                        delegate
                        {
                            SetActiveControl(node);
                        };
                    (ctrl as BaseWebControl).Style[Styles.position] = "relative";

                    if (DataSource["DragAndDrop"].Get<bool>())
                    {
                        // Making draggable ...
                        AspectDraggable dragger = new AspectDraggable();
                        dragger.Dragged +=
                            delegate
                            {
                                int left = int.Parse((ctrl as BaseWebControl).Style[Styles.left].Replace("px", ""));
                                int top = int.Parse((ctrl as BaseWebControl).Style[Styles.top].Replace("px", ""));

                                AbsolutizeWidget(left, top, node, (ctrl as BaseWebControl));
                            };
                        (ctrl as BaseWebControl).Controls.Add(dragger);
                    }
                }

                // Making sure we're rendering the styles needed ...
                if (ctrl is BaseWebControl)
                    RenderStyles((ctrl as BaseWebControl), node);

                parent.Controls.Add(ctrl);
            }
        }

        private void AddSelectedCssClass(Control ctrlIn)
        {
            BaseWebControl ctrl = ctrlIn as BaseWebControl;

            if (ctrl != null && 
                !ctrl.CssClass.Contains(" mux-wysiwyg-selected"))
                ctrl.CssClass += " mux-wysiwyg-selected";

            Control idx = ctrlIn.Parent as Control;
            while (idx != null && idx != ctrls)
            {
                if (idx is BaseWebControl)
                {
                    if (!(idx as BaseWebControl).CssClass.Contains(" mux-wysiwyg-selected"))
                        (idx as BaseWebControl).CssClass += " mux-wysiwyg-selected";
                }
                idx = idx.Parent;
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
                Control c = Selector.FindControlClientID<Control>(ctrls, OldSelected);
                if (c != null)
                {
                    RemoveActiveCssClass(c);
                    if (c is BaseWebControl)
                        (c as BaseWebControl).ToolTip = "Click me to edit the Widget";
                }
                OldSelected = null;
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
            }
            selWidg.SetSelectedItemAccordingToValue(node["_ID"].Value.ToString());

            Control ctrl = Selector.FindControl<Control>(ctrls, 
                node.Contains("Properties") && node["Properties"].Contains("ID") ?
                    node["Properties"]["ID"].Value.ToString() : 
                    ("ID" + node["_ID"].Value.ToString()));

            if (ctrl != null)
            {
                OldSelected = ctrl.ClientID;
                AddSelectedCssClass(ctrl);
                if (ctrl is BaseWebControl)
                {
                    BaseWebControl ctrl2 = ctrl as BaseWebControl;
                    ctrl2.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                }
            }
        }

        private void RemoveActiveCssClass(Control cIn)
        {
            if (cIn == null)
                return;

            BaseWebControl c = cIn as BaseWebControl;

            if (c != null)
                c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");

            Control idx = cIn.Parent;
            while (idx != null && idx != ctrls)
            {
                if (idx is BaseWebControl)
                {
                    BaseWebControl idx1 = idx as BaseWebControl;
                    if (idx1.CssClass.Contains(" mux-wysiwyg-selected"))
                        idx1.CssClass = idx1.CssClass.Replace(" mux-wysiwyg-selected", "");
                }
                
                idx = idx.Parent;
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
                    Control c = Selector.FindControlClientID<Control>(ctrls, OldSelected);
                    RemoveActiveCssClass(c);
                    if (c is BaseWebControl)
                        (c as BaseWebControl).ToolTip = "Click me to edit the Widget";
                }
                OldSelected = "";
                ClearPropertyWindow();

                RaiseSafeEvent(
                    "Magix.MetaForms.GetControlsForForm",
                    DataSource);

                ctrls.Controls.Clear();
                CreateFormControls();
                ctrls.ReRender();
                CreateSelectWidgetSelectList();
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
                "Magix.MetaForms.ShowAllActionsAssociatedWithFormWidgetEvent",
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
            if (node.Contains("ReRender") &&
                node["ReRender"].Get<bool>())
            {
                // Oops ...!
                OldSelected = null;
                ClearPropertyWindow();
            }

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
            node["TypeName"].Value = b.Info;

            if (!string.IsNullOrEmpty(OldSelected))
            {
                int idid = int.Parse(type.Info);
                node["ParentControl"].Value = idid;
            }
            else
            {
                node["ParentControl"].Value = null; // Meaning the 'Root Node' will be the parent control
                node["HasSurface"].Value = true;
            }

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

            if(!string.IsNullOrEmpty(OldSelected))
            {
                RemoveActiveCssClass(Selector.FindControlClientID<Control>(ctrls, OldSelected));
            }

            OldSelected = Selector.FindControl<Control>(ctrls, "ID" + node["NewControlID"].Value.ToString()).ClientID;
            SetActiveControl(DataSource["root"]["Surface"].Find(
                delegate(Node idx)
                {
                    return idx.Name == "_ID" && 
                        idx.Get<int>() == node["NewControlID"].Get<int>();
                }).Parent);
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

        /**
         * Level2: Sets the currently active editing Widget to the given 'ID'
         */
        [ActiveEvent(Name = "Magix.MetaForms.SetActiveEditingMetaFormWidget")]
        protected void Magix_MetaForms_SetActiveEditingMetaFormWidget(object sender, ActiveEventArgs e)
        {
            SetActiveControl(DataSource["root"]["Surface"].Find(
                delegate(Node idx)
                {
                    return idx.Name == "_ID" &&
                        idx.Get<int>() == e.Params["ID"].Get<int>();
                }).Parent);
        }

        /**
         * Level2: Handled to make it possible to Paste Widget Hierarchies and similar 
         * onto the form
         */
        [ActiveEvent(Name = "Magix.ClipBoard.PasteItem")]
        protected void Magix_ClipBoard_PasteItem(object sender, ActiveEventArgs e)
        {
            Node paste = e.Params["PasteNode"].Value as Node;

            Node destination = null;

            if(!string.IsNullOrEmpty(OldSelected))
            {
                string selID = OldSelected.Substring(OldSelected.LastIndexOf("_ID") + 3);

                int slId = int.Parse(selID);

                destination = DataSource["root"]["Surface"].Find(
                    delegate(Node idx)
                    {
                        return idx.Name == "_ID" &&
                            idx.Get<int>() == slId;
                    });
            }

            Node ac = new Node();

            ac["ID"].Value = DataSource["ID"].Value;

            ac["PasteNode"].Value = paste;
            ac["MetaForm"].Value = DataSource["ID"].Value;
            if (destination != null)
                ac["ParentControl"].Value = destination.Value;

            RaiseSafeEvent(
                "Magix.MetaForms.PasteWidgetNodeIntoMetaForm",
                ac);

            // Refreshing form ...
            RaiseSafeEvent(
                "Magix.MetaForms.GetControlsForForm",
                DataSource);

            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();
            CreateSelectWidgetSelectList();
        }
    }
}
