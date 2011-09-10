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
                };
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

            RaiseSafeEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            if (nn.Contains("Control"))
            {
                BaseWebControl ctrl = nn["Control"].Get<BaseWebControl>();
                ctrl.ToolTip = "Click me to edit the Widget";
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
                        if (!string.IsNullOrEmpty(OldSelected))
                        {
                            BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                            c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
                            c.ToolTip = "Click me to edit the Widget";
                        }
                        SetActiveControl(node);
                        OldSelected = ctrl.ClientID;
                        ctrl.CssClass += " mux-wysiwyg-selected";
                        ctrl.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                    };
                ctrl.Style[Styles.position] = "relative";

                // Making draggable ...
                AspectDraggable dragger = new AspectDraggable();
                dragger.Dragged +=
                    delegate
                    {
                        int left = int.Parse(ctrl.Style[Styles.left].Replace("px", ""));
                        int top = int.Parse(ctrl.Style[Styles.top].Replace("px", ""));

                        AbsolutizeWidget(left, top, node);
                    };
                ctrl.Controls.Add(dragger);

                // Making sure we're rendering the styles needed ...
                RenderStyles(ctrl, node);

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

        private void AbsolutizeWidget(int left, int top, Node node)
        {
            if (left == 0 && top == 0)
                return;

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
            ClearPropertyWindow();

            if (node == null || 
                !node.Contains("TypeName"))
                return;

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
        }
    }
}
