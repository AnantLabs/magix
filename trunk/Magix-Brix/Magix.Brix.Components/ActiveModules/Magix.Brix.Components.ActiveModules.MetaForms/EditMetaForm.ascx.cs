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

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     */
    [ActiveModule]
    [PublisherPlugin]
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
                }
                ctrl.Load +=
                    delegate
                    {
                        if (ctrl.ClientID == OldSelected && 
                            !ctrl.CssClass.Contains(" mux-wysiwyg-selected"))
                            ctrl.CssClass += " mux-wysiwyg-selected";
                    };
                ctrl.Click +=
                    delegate
                    {
                        if (!string.IsNullOrEmpty(OldSelected))
                        {
                            BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                            c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
                        }
                        SetActiveControl(node);
                        OldSelected = ctrl.ClientID;
                        ctrl.CssClass += " mux-wysiwyg-selected";
                    };
                parent.Controls.Add(ctrl);
            }
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
            }
            else
            {
                desc.Style[Styles.height] = "18px";
            }
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
        }

        protected void ctrls_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(OldSelected))
            {
                BaseWebControl c = Selector.FindControlClientID<BaseWebControl>(ctrls, OldSelected);
                c.CssClass = c.CssClass.Replace(" mux-wysiwyg-selected", "");
            }
            OldSelected = null;
            ClearPropertyWindow();
        }

        protected string GetPropertyValue(object inpNode)
        {
            string retVal = null;
            string nodeName = inpNode as string;
            Node tmp = DataSource["Surface"].Find(
                delegate(Node idx)
                {
                    if (idx.Contains("_ID") &&
                        idx["_ID"].Get<int>().ToString() == type.Info)
                    {
                        return true;
                    }
                    return false;
                });
            if (tmp != null && tmp.Contains("Properties"))
            {
                foreach (Node idx in tmp["Properties"])
                {
                    if (idx.Name == nodeName)
                    {
                        if (idx.Value != null)
                        {
                            switch (idx.Value.GetType().ToString())
                            {
                                case "System.String":
                                    retVal = idx.Value.ToString();
                                    break;
                                case "System.Boolean":
                                    retVal = idx.Value.ToString();
                                    break;
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        protected bool GetPropertyValueBool(object inpNode)
        {
            bool retVal = false;
            string nodeName = inpNode as string;
            Node tmp = DataSource["Surface"].Find(
                delegate(Node idx)
                {
                    if (idx.Contains("_ID") &&
                        idx["_ID"].Get<int>().ToString() == type.Info)
                    {
                        return true;
                    }
                    return false;
                });
            if (tmp != null && tmp.Contains("Properties"))
            {
                foreach (Node idx in tmp["Properties"])
                {
                    if (idx.Name == nodeName)
                    {
                        if (idx.Value != null)
                        {
                            switch (idx.Value.GetType().ToString())
                            {
                                case "System.Boolean":
                                    retVal = bool.Parse(idx.Value.ToString());
                                    break;
                            }
                        }
                    }
                }
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
