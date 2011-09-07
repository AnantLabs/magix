/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaForms;
using Magix.Brix.Data;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.MetaForms
{
    /**
     * Level2: Contains logic to help build Meta Forms. Meta Forms are forms where you
     * can almost in its entirety decide everything about how a View can be built
     */
    [ActiveController]
    public class MetaForms_Controller : ActiveController
    {
        /**
         * Level2: Will return the menu items needed to fire up 'View Meta Forms' forms 
         * for Administrator
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Caption"].Value = "Meta Types";
            e.Params["Items"]["MetaType"]["Items"]["MetaForms"]["Caption"].Value = "Meta Forms ...";
            e.Params["Items"]["MetaType"]["Items"]["MetaForms"]["Event"]["Name"].Value = "Magix.MetaForms.ViewForms";
        }

        /**
         * Level2: Returns the MetaForm count back to dashboard
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaForms"].Value = true;
            e.Params["Type"]["Properties"]["MetaForms"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaForms"]["Header"].Value = "Meta Forms";
            e.Params["Type"]["Properties"]["MetaForms"]["ClickLabelEvent"].Value = "Magix.MetaForms.ViewForms";
            e.Params["Object"]["Properties"]["MetaForms"].Value = MetaForm.Count.ToString();
        }

        /**
         * Level2: Will inject desktop icons for the Meta Form shortcut
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Forms"]["Image"].Value = "media/images/rosetta.png";
            e.Params["Items"]["Forms"]["Shortcut"].Value = "O";
            e.Params["Items"]["Forms"]["Text"].Value = "Click to launch Meta Forms [Key O]";
            e.Params["Items"]["Forms"]["CSS"].Value = "mux-desktop-icon";
            e.Params["Items"]["Forms"]["Event"].Value = "Magix.MetaForms.ViewForms";
        }

        /**
         * Level2: Will show all Meta Forms for admin
         */
        [ActiveEvent(Name = "Magix.MetaForms.ViewForms")]
        protected void Magix_MetaForms_ViewForms(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaForm).FullName;
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "large-bottom-margin";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 4;

            node["WhiteListColumns"]["Created"].Value = true;
            node["WhiteListColumns"]["Created"]["ForcedWidth"].Value = 4;

            node["ReuseNode"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaForms.EditForm";

            node["Container"].Value = "content3";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will show the Edit Meta Forms form for editing a specific 
         * MetaForm
         */
        [ActiveEvent(Name = "Magix.MetaForms.EditForm")]
        protected void Magix_MetaForms_EditForm(object sender, ActiveEventArgs e)
        {
            MetaForm f = MetaForm.SelectByID(e.Params["ID"].Get<int>());

            Node node = f.FormNode;
            node["ID"].Value = f.ID;

            RaiseEvent(
                "Magix.MetaForms.GetControlsForForm",
                node);

            node["Last"].Value = true;
            node["Width"].Value = 24;
            node["Overflowized"].Value = true;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaForms.EditMetaForm",
                "content4",
                node);
        }

        /**
         * Level2: Will return the controls that the Meta Form builder has by default, such as Button,
         * Label, CheckBox etc
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetControlTypes")]
        protected void Magix_MetaForms_GetControlTypes(object sender, ActiveEventArgs e)
        {
            // Button type
            GetButton(e);

            // Label type
            GetLabel(e);

            // CheckBox type
            CreateCheckBox(e);

            // CheckBox type
            CreateTextBox(e);
        }

        private void CreateTextBox(ActiveEventArgs e)
        {
            e.Params["Controls"]["TextBox"]["Name"].Value = "Magix TextBox";
            e.Params["Controls"]["TextBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.TextBox";
            e.Params["Controls"]["TextBox"]["ToolTip"].Value = @"Creates a TextBox type of 
control, which you can assign Text to and Change Event Handlers";

            e.Params["Controls"]["TextBox"]["Properties"]["CssClass"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control";

            e.Params["Controls"]["TextBox"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["TextBox"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["Text"]["Description"].Value = @"The 
visible text for the end user and also the text fragment the user can change by editing the text box value";

            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"]["Description"].Value = @"The 
'watermark' text of your textbox. Will show when textbox is empty, as a 'cue' to the end user for what to 
type into it";

            e.Params["Controls"]["TextBox"]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["Info"]["Description"].Value = @"Additional information 
which can be stored within your object, which is not visible for the end user in any ways, but still 
will follow your Widget around as a small piece of 'information storage'. Mostly used for figuring out 
which field your widget has been Data Bound towards within your Meta Object, or what Property 
it is supposed to create upon creation of a new Meta Object";

            e.Params["Controls"]["TextBox"]["Events"]["Change"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["Change"]["Description"].Value = @"Raised when 
the text has changed, and the user chooses to 'leave the field' and move to another field on the form by 
e.g. clicking with his mouse or using TAB such that the textbox looses focus";
        }

        private static void CreateCheckBox(ActiveEventArgs e)
        {
            e.Params["Controls"]["CheckBox"]["Name"].Value = "Magix CheckBox";
            e.Params["Controls"]["CheckBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.CheckBox";
            e.Params["Controls"]["CheckBox"]["ToolTip"].Value = @"Creates a CheckBox type of 
control, which you can assign Text to and Change Event Handlers";

            e.Params["Controls"]["CheckBox"]["Properties"]["CssClass"].Value = typeof(string).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control";

            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"].Value = typeof(bool).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"]["Description"].Value = @"The 
Checked state of your CheckBox, true will 'tag it off' as checked, while false [the default] will 
keep it 'open'";

            e.Params["Controls"]["CheckBox"]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Info"]["Description"].Value = @"Additional information 
which can be stored within your object, which is not visible for the end user in any ways, but still 
will follow your Widget around as a small piece of 'information storage'. Mostly used for figuring out 
which field your widget has been Data Bound towards within your Meta Object, or what Property 
it is supposed to create upon creation of a new Meta Object";

            e.Params["Controls"]["CheckBox"]["Events"]["Change"].Value = true;
            e.Params["Controls"]["CheckBox"]["Events"]["Change"]["Description"].Value = @"Raised when 
the checked state has changed, either by clicking or through some other user interaction";
        }

        private static void GetLabel(ActiveEventArgs e)
        {
            e.Params["Controls"]["Label"]["Name"].Value = "Magix Label";
            e.Params["Controls"]["Label"]["TypeName"].Value = "Magix.MetaForms.Plugins.Label";
            e.Params["Controls"]["Label"]["ToolTip"].Value = @"Creates a Label type of 
control, which you can assign Text to. Basically serves as a read-only textual fragment on your page. 
Change which HTML tag it's being rendered with by setting its 'Tag' property";

            e.Params["Controls"]["Label"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Text"]["Description"].Value = @"The text visible to 
the end user in his browser";

            e.Params["Controls"]["Label"]["Properties"]["CssClass"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control";

            e.Params["Controls"]["Label"]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Info"]["Description"].Value = @"Additional information 
which can be stored within your object, which is not visible for the end user in any ways, but still 
will follow your Widget around as a small piece of 'information storage'. Mostly used for figuring out 
which field your widget has been Data Bound towards within your Meta Object, or what Property 
it is supposed to create upon creation of a new Meta Object";

            e.Params["Controls"]["Label"]["Properties"]["Tag"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Tag"]["Description"].Value = @"Which HTML tag 
will be rendered by the control. There are many legal values for this property, some of them are 'p', 
'div', 'span', 'label', 'li' [use panel for 'ul'] and 'address'. But also many more. Check up the 
standard for HTML5 if you'd like to wish all its legal values. All 'normal HTML elements', which doesn't 
need special attributes or child elements can really be described by modifying this property accordingly";
        }

        private static void GetButton(ActiveEventArgs e)
        {
            e.Params["Controls"]["Button"]["Name"].Value = "Magix Button";
            e.Params["Controls"]["Button"]["TypeName"].Value = "Magix.MetaForms.Plugins.Button";
            e.Params["Controls"]["Button"]["ToolTip"].Value = @"Creates a Button type of 
control, which you can assign Click actions to, from which when the user clicks, will raise 
the actions you've associated with the button. You can have several buttons per form, and they 
can have different Text values to differentiate them for the user";

            e.Params["Controls"]["Button"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["Text"]["Description"].Value = @"The text displayed 
to the end user on top of the button";

            e.Params["Controls"]["Button"]["Properties"]["CssClass"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control";

            e.Params["Controls"]["Button"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["Button"]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["Info"]["Description"].Value = @"Additional information 
which can be stored within your object, which is not visible for the end user in any ways, but still 
will follow your Widget around as a small piece of 'information storage'. Mostly used for figuring out 
which field your widget has been Data Bound towards within your Meta Object, or what Property 
it is supposed to create upon creation of a new Meta Object";

            e.Params["Controls"]["Button"]["Events"]["Click"].Value = true;
            e.Params["Controls"]["Button"]["Events"]["Click"]["Description"].Value = @"Raised when 
the button is clicked";
        }

        /**
         * Level2: Will return the Control tree hierarchy for the Meta Form
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetControlsForForm")]
        protected void Magix_MetaForms_GetControlsForForm(object sender, ActiveEventArgs e)
        {
            MetaForm f = MetaForm.SelectByID(e.Params["ID"].Get<int>());

            if (e.Params.Contains("root"))
                e.Params["root"].UnTie();

            e.Params.Add(f.FormNode);
        }

        /**
         * Level2: Will create one of the default internally installed types for the 
         * Meta Form system, which includes e.g. Button, CheckBox and Label etc
         */
        [ActiveEvent(Name = "Magix.MetaForms.CreateControl")]
        protected void Magix_MetaForms_CreateControl(object sender, ActiveEventArgs e)
        {
            switch (e.Params["TypeName"].Get<string>())
            {
                case "Magix.MetaForms.Plugins.Button":
                    {
                        Button btn = new Button();
                        btn.Text = "[null]";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.Label":
                    {
                        Label btn = new Label();
                        btn.Text = "[null]";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.CheckBox":
                    {
                        CheckBox btn = new CheckBox();
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.TextBox":
                    {
                        TextBox btn = new TextBox();
                        e.Params["Control"].Value = btn;
                    } break;
                default:
                    // DO NOTHING. Others might handle ...
                    break;
            }
        }

        /**
         * Level2: Appends a new Control of type 'TypeName' to the given Meta Form 'ID' within
         * its control with ID of 'ParentControl'. Let ParentControl be null or not defined 
         * to use the Root Form object of the Meta Form
         */
        [ActiveEvent(Name = "Magix.MetaForms.AppendControlToForm")]
        protected void Magix_MetaForms_AppendControlToForm(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm f = MetaForm.SelectByID(e.Params["ID"].Get<int>());
                Magix.Brix.Components.ActiveTypes.MetaForms.MetaForm.Node parent = null;
                if (e.Params.Contains("ParentControl") &&
                    e.Params["ParentControl"].Value != null)
                {
                    parent = f.Form.Find(
                        delegate(MetaForm.Node idx)
                        {
                            return idx.Name == "ID" && (e.Params["ParentControl"]).Equals(idx.Value);
                        });
                }
                else
                    parent = f.Form;
                if (parent == null)
                    throw new ArgumentException("That parent doesn't exist");

                int count = parent["Surface"].Children.Count;

                parent["Surface"]["c-" + count]["TypeName"].Value = e.Params["TypeName"].Get<string>();

                parent.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Changes the 'ID' MetaForm's 'PropertyName' into 'Value' and saves 
         * the MetaForm property
         */
        [ActiveEvent(Name = "Magix.MetaForms.ChangeFormPropertyValue")]
        protected void Magix_MetaForms_ChangeFormPropertyValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm f = MetaForm.SelectByID(e.Params["ID"].Get<int>());
                MetaForm.Node nn = FindNodeByID(f.Form, e.Params["ControlID"].Get<int>());
                string val = null;
                string typeName = null;
                switch (e.Params["Value"].Value.GetType().ToString())
                {
                    case "System.String":
                        val = e.Params["Value"].Value.ToString();
                        typeName = typeof(string).FullName;
                        break;
                    case "System.Boolean":
                        val = e.Params["Value"].Value.ToString();
                        typeName = typeof(bool).FullName;
                        break;
                }
                nn["Properties"][e.Params["PropertyName"].Get<string>()].Value = val;
                if (!string.IsNullOrEmpty(typeName))
                    nn["Properties"][e.Params["PropertyName"].Get<string>()].TypeName = typeName;

                f.Form.Save();

                tr.Commit();
            }
        }

        private MetaForm.Node FindNodeByID(MetaForm.Node node, int id)
        {
            MetaForm.Node retVal = node.Find(
                delegate(MetaForm.Node idx)
                {
                    return idx.ID == id;
                });
            if (retVal != null)
                return retVal;
            foreach (MetaForm.Node idx in node.Children)
            {
                retVal = FindNodeByID(idx, id);
                if (retVal != null)
                    return retVal;
            }
            return null;
        }
    }
}
