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
using System.Globalization;
using Magix.UX.Effects;

namespace Magix.Brix.Components.ActiveModules.MetaForms
{
    /**
     * Level2: Encapsulates a MetaForm through a PublisherPlugin which you can inject into your sites
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

            if (FirstLoad)
                ExecuteInitActions();
        }

        private void ExecuteInitActions()
        {
            // Doing init actions for Widgets ...
            List<BaseControl> tmp = new List<BaseControl>();
            foreach (BaseControl idx in Selector.Select<BaseControl>(this))
            {
                tmp.Add(idx);
            }
            foreach (BaseControl idx in tmp)
            {
                if (Selector.FindControlClientID<BaseControl>(this, idx.ClientID) != null)
                {
                    // Yup, still around ... ;)
                    idx.RaiseInitiallyLoaded();
                }
            }

            // Form Init Actions ...
            if (!DataSource["root"].Contains("Actions") ||
                !DataSource["root"]["Actions"].Contains("InitialLoading") ||
                string.IsNullOrEmpty(DataSource["root"]["Actions"]["InitialLoading"].Get<string>()))
                return;

            string actions = DataSource["root"]["Actions"]["InitialLoading"].Get<string>();

            DataSource["ActionsToExecute"].Value = actions;

            ExecuteSafely(
                delegate
                {
                    RaiseEvent(
                        "Magix.MetaForms.RaiseActionsFromActionString",
                        DataSource);
                },
                "Something went wrong while running your InitialLoading actions ...");

            DataSource["ActionsToExecute"].UnTie();
        }

        private void CreateFormControls()
        {
            if (DataSource.Contains("root") &&
                DataSource["root"].Contains("Surface"))
            {
                ExecuteSafely(
                    delegate
                    {
                        foreach (Node idx in DataSource["root"]["Surface"])
                        {
                            if (idx.Name == "_ID")
                                continue;

                            CreateSingleControl(idx, ctrls);
                        }
                    },
                    "Something went wrong while trying to create one of your Form Widgets, hence entire operation was aborted");
            }
        }

        private void CreateSingleControl(Node node, Control parent)
        {
            Node nn = new Node();

            nn["TypeName"].Value = node["TypeName"].Get<string>();
            nn["ControlNode"].Value = node;
            nn["_ID"].Value = node["_ID"].Value;
            nn["DataSourceRootNode"].Value = DataSource;

            RaiseEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            if (nn.Contains("Control"))
            {
                Control ctrl = nn["Control"].Get<Control>();

                // Child controls
                if (node.Contains("Surface"))
                {
                    if (nn.Contains("CreateChildControlsEvent"))
                    {
                        // Listable control type ...
                        Node tmp = new Node();

                        // Yup, looks stupidish, but feel very safe ... ;)
                        tmp["Controls"].Value = node["Surface"];
                        tmp["Control"].Value = ctrl;
                        tmp["DataSourceRootNode"].Value = DataSource;
                        if (node.Contains("Properties") &&
                            node["Properties"].Contains("Info"))
                        {
                            tmp["DataSource"].Value = GetObjectFromExpression(node["Properties"]["Info"].Get<string>(), false);
                        }

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
                parent.Controls.Add(ctrl);
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

        /**
         * Level2: Expects a 'Container' parameter, and if it matches the Container of this specific module, 
         * it will change the MetaForm, load that specific MetaForm into itself, and run the Init Actions
         * of that specific MetaForm as if it was just loaded normally
         */
        [ActiveEvent(Name = "Magix.MetaForms.LoadNewMetaForm")]
        protected void Magix_MetaForms_LoadNewMetaForm(object sender, ActiveEventArgs e)
        {
            if (this.Parent.ID == e.Params["Container"].Get<string>())
            {
                MetaFormName = e.Params["MetaFormName"].Get<string>();

                DataSource["MetaFormName"].Value = MetaFormName;
                DataSource["root"].UnTie();
                DataSource["Controls"].UnTie();

                RaiseSafeEvent(
                    "Magix.MetaForms.GetControlsForForm",
                    DataSource);

                ctrls.Controls.Clear();
                CreateFormControls();
                ExecuteInitActions();
                ctrls.ReRender();
            }
        }

        /**
         * Level2: Will set Focus to the first TextBox in the form if raised from 'within' the 
         * current Meta Form, or explicitly has its 'OriginalWebPartID' overridden to 
         * reflect another WebPart ID on the page
         */
        [ActiveEvent(Name = "Magix.MetaView.SetFocusToFirstTextBox")]
        protected void Magix_MetaView_SetFocusToFirstTextBox(object sender, ActiveEventArgs e)
        {
            if (e.Params["OriginalWebPartID"].Get<int>() == DataSource["OriginalWebPartID"].Get<int>())
            {
                TextBox b = Selector.SelectFirst<TextBox>(ctrls);
                if (b != null)
                {
                    new EffectTimeout(500)
                        .ChainThese(
                            new EffectFocusAndSelect(b))
                        .Render();
                }
            }
        }

        /**
         * Level2: Will empty the form, and set all of its values back to their defaults, but only the ones 
         * with a valid Info field, and which are editable for the end user
         */
        [ActiveEvent(Name = "Magix.MetaView.EmptyForm")]
        protected void Magix_MetaView_EmptyForm(object sender, ActiveEventArgs e)
        {
            foreach (BaseControl idx in Selector.Select<BaseControl>(
                ctrls,
                delegate(Control idxI)
                {
                    return idxI is BaseWebControl &&
                        !string.IsNullOrEmpty((idxI as BaseWebControl).Info);
                }))
            {
                string dataFieldName = idx.Info;
                if (dataFieldName.Contains(":"))
                {
                    dataFieldName = dataFieldName.Split(':')[1];
                }
                if (string.IsNullOrEmpty(dataFieldName))
                    continue;

                EmptyControlValues(idx);
            }
        }

        /*
         * Helper for above ...
         */
        private void EmptyControlValues(BaseControl ctrl)
        {
            Node typeNode = GetTypeNode(ctrl);

            Node node = DataSource;

            node["Control"].Value = ctrl;
            node["TypeNode"].Value = typeNode;

            RaiseEvent(
                "Magix.MetaForms.EmptySingleWidgetInstance",
                node);

            DataSource["Control"].UnTie();
            DataSource["TypeNode"].UnTie();
        }

        /**
         * Level2: Will serialize all Form Widgets with a valid Info field value such that a Node structure 
         * is created according to the form widget values
         */
        [ActiveEvent(Name = "Magix.MetaForms.CreateNodeFromMetaForm")]
        protected void Magix_MetaForms_CreateNodeFromMetaForm(object sender, ActiveEventArgs e)
        {
            // Removing any previous saved objects ...
            DataSource["Object"].UnTie();

            foreach (BaseControl idx in Selector.Select<BaseControl>(
                ctrls,
                delegate(Control idxI)
                {
                    return idxI is BaseControl &&
                        !string.IsNullOrEmpty((idxI as BaseControl).Info);
                }))
            {
                string dataFieldName = idx.Info;
                if (dataFieldName.Contains(":"))
                {
                    dataFieldName = dataFieldName.Split(':')[1];
                }
                if (string.IsNullOrEmpty(dataFieldName))
                    continue;

                SerializeControlValueIntoDataSourceNode(idx, dataFieldName);
            }
        }

        private void SerializeControlValueIntoDataSourceNode(BaseControl ctrl, string dataFieldName)
        {
            Node typeNode = GetTypeNode(ctrl);

            if (typeNode == null)
                return; //Oops ...!

            Node node = DataSource;

            node["Control"].Value = ctrl;
            node["TypeNode"].Value = typeNode;
            node["DataFieldName"].Value = dataFieldName;

            RaiseEvent(
                "Magix.MetaForms.SerializeControlIntoNode",
                node);

            DataSource["Control"].UnTie();
            DataSource["TypeNode"].UnTie();
            DataSource["DataFieldName"].UnTie();
        }

        private Node GetTypeNode(BaseControl idx)
        {
            Node typeNode = null;
            if (idx.ID.StartsWith("ID"))
            {
                int id = -1;
                if (idx.ID.Contains("x"))
                {
                    string[] t = idx.ID.Substring(2).Split('x');
                    id = int.Parse(t[t.Length - 1]);
                }
                else if (idx.ID.StartsWith("ID"))
                    id = int.Parse(idx.ID.Substring(2));

                if (id != -1)
                {
                    typeNode = DataSource["root"]["Surface"].Find(
                        delegate(Node idxN)
                        {
                            if (idxN.Name == "_ID")
                            {
                                if (idx.ID.Contains("x") &&
                                    idxN.Value != null &&
                                    idxN.Value.ToString().Contains("x"))
                                {
                                    string[] h = idxN.Value.ToString().Split('x');
                                    return h[h.Length - 1] == id.ToString();
                                }
                                else
                                    return idxN.Value.ToString() == id.ToString();
                            }
                            return false;
                        });
                    if (typeNode != null)
                        typeNode = typeNode.Parent;
                }
            }
            else
            {
                string id = idx.ID;
                typeNode = DataSource["root"]["Surface"].Find(
                    delegate(Node idxN)
                    {
                        return idxN.Name == "ID" &&
                            idxN.Parent.Name == "Properties" &&
                            idxN.Value.ToString() == id;
                    });
            }
            return typeNode;
        }

        /**
         * Level2: Will DataBind every Widget of your MetaForm such that every Widget property which 
         * starts with 'DataSource' and then any number of square brackets, e.g. 
         * DataSource["Objects"][0]["ID"] will
         * then fetch the first Node child of 'Objects' node from your DataSource and databind whatever 
         * property you are putting this value into
         */
        [ActiveEvent(Name = "Magix.MetaForms.DataBindForm")]
        protected void Magix_MetaForms_DataBindForm(object sender, ActiveEventArgs e)
        {
            ctrls.Controls.Clear();
            CreateFormControls();
            ctrls.ReRender();

            DataBindHardLinkedProperties();
        }

        /*
         * Helper for above, will databind all expressions starting with 'DataSource', as in all the 
         * non-relative ones, or 'static ones' ...
         */
        private void DataBindHardLinkedProperties()
        {
            // First the 'hard linked' ones ...
            DataSource["root"].Find(
                delegate(Node idx)
                {
                    if (idx.Parent.Name == "Properties" &&
                        idx.Name != "_ID" &&
                        idx.Value != null &&
                        idx.Value.ToString().StartsWith("{DataSource"))
                    {
                        // Only 'non-relative' data bindings ... [relative ones starts with a '[' ... ]
                        int id = (int)idx.Parent.Parent["_ID"].Value;
                        Control c = Selector.FindControl<Control>(ctrls, "ID" + id);

                        if (c != null)
                        {
                            PropertyInfo prop = c.GetType().GetProperty(idx.Name,
                                BindingFlags.Instance |
                                BindingFlags.NonPublic |
                                BindingFlags.Public);

                            if (prop != null)
                            {
                                Type toConvert = prop.PropertyType;
                                object val = GetExpression(idx.Value as string);
                                if (val != null)
                                {
                                    switch (toConvert.FullName)
                                    {
                                        case "System.String":
                                            val = val.ToString();
                                            break;
                                        case "System.Boolean":
                                            val = bool.Parse(val.ToString());
                                            break;
                                        case "System.DateTime":
                                            val = DateTime.ParseExact(val.ToString(), "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                                            break;
                                        case "System.Decimal":
                                            val = Decimal.Parse(val.ToString(), CultureInfo.InvariantCulture);
                                            break;
                                        case "System.Int":
                                            val = int.Parse(val.ToString(), CultureInfo.InvariantCulture);
                                            break;
                                    }
                                    if (val != null)
                                    {
                                        prop.GetSetMethod(true).Invoke(c, new object[] { val });
                                    }
                                }
                            }
                        }
                    }
                    return false;
                });
        }

        /*
         * Takes in e.g. DataSource[Object].Name or DataSource[Object][0].Value and returns a
         * string representation of that expression [invariant culture, ISO standardish ...]
         */
        private string GetExpression(string expr)
        {
            if (expr == null)
                return null;

            object p = GetObjectFromExpression(expr, true);

            if (p == null)
                return null;

            switch (p.GetType().FullName)
            {
                case "System.String":
                    return p.ToString();
                case "System.Boolean":
                    return p.ToString();
                case "System.DateTime":
                    return ((DateTime)p).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                case "System.Decimal":
                    return ((decimal)p).ToString(CultureInfo.InvariantCulture);
                case "System.Int32":
                    return ((int)p).ToString(CultureInfo.InvariantCulture);
                default:
                    return expr; // 
            }
        }

        /*
         * Helper for above, will return either the Name, Value or Node itself as an object to caller [above]
         * according to how the expressions looks like, e.g. DataSource[Objects].Value
         */
        private object GetObjectFromExpression(string expr, bool doThrow)
        {
            if (expr.StartsWith("{DataSource"))
            {
                expr = expr.Substring(expr.IndexOf("{") + 1);
                expr = expr.Substring(0, expr.LastIndexOf("}"));

                // 'Static' value, not 'relative' ...
                // Scanning forwards from after 'DataSource' ...
                Node x = DataSource;
                bool isInside = false;
                string bufferNodeName = null;
                string lastEntity = null;
                for (int idx = 10; idx < expr.Length; idx++)
                {
                    char tmp = expr[idx];
                    if (isInside)
                    {
                        if (tmp == ']')
                        {
                            lastEntity = "";

                            if (string.IsNullOrEmpty(bufferNodeName))
                                throw new ArgumentException("Opps, empty node name/index ...");

                            bool allNumber = true;
                            foreach (char idxC in bufferNodeName)
                            {
                                if (("0123456789").IndexOf(idxC) == -1)
                                {
                                    allNumber = false;
                                    break;
                                }
                            }
                            if (allNumber)
                            {
                                int intIdx = int.Parse(bufferNodeName);
                                if (x.Count > intIdx)
                                    x = x[intIdx];
                                else
                                    return null;
                            }
                            else
                            {
                                if (!x.Contains(bufferNodeName))
                                {
                                    return null;
                                }
                                x = x[bufferNodeName];
                            }
                            bufferNodeName = "";
                            isInside = false;
                            continue;
                        }
                        bufferNodeName += tmp;
                    }
                    else
                    {
                        if (tmp == '[')
                        {
                            bufferNodeName = "";
                            isInside = true;
                            continue;
                        }
                        lastEntity += tmp;
                    }
                }
                if (lastEntity == ".Value")
                    return x.Value;
                else if (lastEntity == ".Name")
                    return x.Name;
                else if (lastEntity == "")
                    return x;
            }
            return null;
        }
    }
}
