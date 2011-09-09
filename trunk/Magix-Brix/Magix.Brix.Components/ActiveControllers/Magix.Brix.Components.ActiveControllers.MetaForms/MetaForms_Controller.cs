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
using Magix.Brix.Components.ActiveTypes.MetaTypes;

namespace Magix.Brix.Components.ActiveControllers.MetaForms
{
    /**
     * Level2: Contains logic to help build Meta Forms. Meta Forms are forms where you
     * can almost in its entirety decide everything about how a View can be built
     */
    [ActiveController]
    public class MetaForms_Controller : ActiveController
    {
        #region [ -- 'Global' plugin event sinks ... -- ]
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

        #endregion

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

            Node node = new Node();
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

        #region [ -- Plugins for Meta Form -- ]

        /**
         * Level2: Will return the controls that the Meta Form builder has by default, such as Button,
         * Label, CheckBox etc
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetControlTypes")]
        protected void Magix_MetaForms_GetControlTypes(object sender, ActiveEventArgs e)
        {
            // Button type
            CreateButton(e);

            // Label type
            CreateLabel(e);

            // CheckBox type
            CreateCheckBox(e);

            // CheckBox type
            CreateTextBox(e);
        }

        /*
         * Helper for above ...
         */
        private static void GetCommonEventsAndProperties(ActiveEventArgs e, string typeName)
        {
            e.Params["Controls"][typeName]["Properties"]["ID"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["ID"]["Description"].Value = @"String ID used for 
uniquely identifying a widget later, for extracting its value or changing its state somehow. Must be unique 
per Meta Form. Should _not_ be set for widgets inside of Repeaters or other listable types of controls. Can only 
contain alphanumerical characters [a-z|A-Z|0-9], and it should start with an a-z|A-Z character";

            e.Params["Controls"][typeName]["Properties"]["CssClass"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control";

            e.Params["Controls"][typeName]["Properties"]["Dir"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["Dir"]["Description"].Value = @"Direction of text 
on the widget. If you set it to 'rtl', it will display characters from right to left, as in the way 
it's being done in among other things in Arabic";

            e.Params["Controls"][typeName]["Properties"]["ToolTip"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["ToolTip"]["Description"].Value = @"Tooltip hint 
given to user when he moves cursor over widget. Obviously useless, for apparent reasons, on tablets, iPhones 
and such";

            e.Params["Controls"][typeName]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["Info"]["Description"].Value = @"Additional information 
which can be stored within your object, which is not visible for the end user in any ways, but still 
will follow your Widget around as a small piece of 'information storage'. Mostly used for figuring out 
which field your widget has been Data Bound towards within your Meta Object, or what Property 
it is supposed to create upon creation of a new Meta Object";

            e.Params["Controls"][typeName]["Properties"]["TabIndex"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["TabIndex"]["Description"].Value = @"Numerical 
value indicating which order the widget is within the 'tab hierarchy', meaning which control is next if 
the user clicks the TAB key on the keyboard. A control which has TabIndex of 2 will gain focus when 
the user hits TAB and the widget that has currently focus has 1 as TabIndex, and so on";

            // Events ...

            e.Params["Controls"][typeName]["Events"]["Click"].Value = true;
            e.Params["Controls"][typeName]["Events"]["Click"]["Description"].Value = @"Raised when 
the widget is clicked. Most useful for buttons and such, since if used on other types of elements it will 
partially destroy accessibility for your application";

            e.Params["Controls"][typeName]["Events"]["DblClick"].Value = true;
            e.Params["Controls"][typeName]["Events"]["DblClick"]["Description"].Value = @"Raised when 
the widget is double clicked. Most useful for buttons and such, since if used on other types of elements it will 
partially destroy accessibility for your application";

            e.Params["Controls"][typeName]["Events"]["MouseDown"].Value = true;
            e.Params["Controls"][typeName]["Events"]["MouseDown"]["Description"].Value = @"Raised when 
the left mouse button is squeezed, but before it's released, on top of the specific widget. Obviously, for apparent 
reasons, not very useful for phones, iPads, Droids and Tablet development";

            e.Params["Controls"][typeName]["Events"]["MouseUp"].Value = true;
            e.Params["Controls"][typeName]["Events"]["MouseUp"]["Description"].Value = @"Raised when 
the left mouse button is squeezed and released, on top of the specific widget. Obviously, for apparent 
reasons, not very useful for phones, iPads, Droids and Tablet development";

            e.Params["Controls"][typeName]["Events"]["MouseOver"].Value = true;
            e.Params["Controls"][typeName]["Events"]["MouseOver"]["Description"].Value = @"Raised when 
the mouse cursor is brought over the widget";

            e.Params["Controls"][typeName]["Events"]["MouseOut"].Value = true;
            e.Params["Controls"][typeName]["Events"]["MouseOut"]["Description"].Value = @"Raised when 
the mouse cursor leaves the element, meaning the cursor is moved outside of the boundaries of the 
widget";

            e.Params["Controls"][typeName]["Events"]["KeyPress"].Value = true;
            e.Params["Controls"][typeName]["Events"]["KeyPress"]["Description"].Value = @"Raised when 
a key is pressed and typed on the widget";

            e.Params["Controls"][typeName]["Events"]["EscKey"].Value = true;
            e.Params["Controls"][typeName]["Events"]["EscKey"]["Description"].Value = @"Raised when 
the ESC key is clicked and released on the widget";
        }

        /*
         * Helper for above ...
         */
        private void CreateTextBox(ActiveEventArgs e)
        {
            e.Params["Controls"]["TextBox"]["Name"].Value = "Magix TextBox";
            e.Params["Controls"]["TextBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.TextBox";
            e.Params["Controls"]["TextBox"]["ToolTip"].Value = @"Creates a TextBox type of 
control, which you can assign Text to and TextChanged Event Handlers. TextBoxes are the by far most 
common type of input control, since it can take any types of input values from the end user";

            GetCommonEventsAndProperties(e, "TextBox");

            e.Params["Controls"]["TextBox"]["Properties"]["TextMode"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["TextMode"]["Description"].Value = @"Can be 
either 'Normal' [default], 'Password', 'Email', 'Number' or 'Phone'. Changes the input type accordingly 
such that modern browsers can help the end user with getting the input correctly";

            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"]["Description"].Value = @"The 
'watermark' text of your textbox. Will show when textbox is empty, as a 'cue' to the end user for what to 
type into it";

            e.Params["Controls"]["TextBox"]["Properties"]["AutoCapitalize"].Value = typeof(bool).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoCapitalize"]["Default"].Value = true;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoCapitalize"]["Description"].Value = @"If true, 
will automatically capitalize the first letter of the TextBox as the user is typing";

            e.Params["Controls"]["TextBox"]["Properties"]["AutoCorrect"].Value = typeof(bool).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoCorrect"]["Default"].Value = true;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoCorrect"]["Description"].Value = @"If true, 
will automatically correct spelling mistakes, if possible, for the end user as he is typing";

            e.Params["Controls"]["TextBox"]["Properties"]["AutoComplete"].Value = typeof(bool).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoComplete"]["Default"].Value = true;
            e.Params["Controls"]["TextBox"]["Properties"]["AutoComplete"]["Description"].Value = @"If true, 
will attempt at automatically completing the field according to the user's settings within the browser. Doesn't 
always work since it tries to parse the field type of information according to its ID, which you have little or 
no 'absolute' control over";

            e.Params["Controls"]["TextBox"]["Properties"]["MaxLength"].Value = typeof(int).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["MaxLength"]["Default"].Value = 0;
            e.Params["Controls"]["TextBox"]["Properties"]["MaxLength"]["Description"].Value = @"Maximum 
length of text accepted within widget";

            e.Params["Controls"]["TextBox"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["Text"]["Description"].Value = @"The 
visible text for the end user and also the text fragment the user can change by editing the text box value";

            e.Params["Controls"]["TextBox"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["TextBox"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["TextBox"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the control's value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["TextBox"]["Events"]["TextChanged"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["TextChanged"]["Description"].Value = @"Raised when 
the text has changed, and the user chooses to 'leave the field' and move to another field on the form by 
e.g. clicking with his mouse or using TAB such that the textbox looses focus";

            e.Params["Controls"]["TextBox"]["Events"]["EscPressed"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["EscPressed"]["Description"].Value = @"Raised when 
the ESC key was clicked while the TextBox had focus, and was taking input";

            e.Params["Controls"]["TextBox"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["TextBox"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";
        }

        /*
         * Helper for above ...
         */
        private static void CreateCheckBox(ActiveEventArgs e)
        {
            e.Params["Controls"]["CheckBox"]["Name"].Value = "Magix CheckBox";
            e.Params["Controls"]["CheckBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.CheckBox";
            e.Params["Controls"]["CheckBox"]["ToolTip"].Value = @"Creates a CheckBox type of 
control, which you can assign Text to and CheckedChanged Event Handlers. Useful for things that black and 
white in nature, such as yes and no questions";

            GetCommonEventsAndProperties(e, "CheckBox");

            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the control's value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"].Value = typeof(bool).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"]["Description"].Value = @"The 
Checked state of your CheckBox, true will 'tag it off' as checked, while false [the default] will 
keep it 'open'";

            e.Params["Controls"]["CheckBox"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["CheckBox"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["CheckBox"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["CheckBox"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";

            e.Params["Controls"]["CheckBox"]["Events"]["CheckedChanged"].Value = true;
            e.Params["Controls"]["CheckBox"]["Events"]["CheckedChanged"]["Description"].Value = @"Raised when 
the checked state has changed, either by clicking or through some other user interaction";
        }

        /*
         * Helper for above ...
         */
        private static void CreateLabel(ActiveEventArgs e)
        {
            e.Params["Controls"]["Label"]["Name"].Value = "Magix Label";
            e.Params["Controls"]["Label"]["TypeName"].Value = "Magix.MetaForms.Plugins.Label";
            e.Params["Controls"]["Label"]["ToolTip"].Value = @"Creates a Label type of 
control, which you can assign Text to. Basically serves as a read-only textual fragment on your page. 
Change which HTML tag it's being rendered with by setting its 'Tag' property";

            GetCommonEventsAndProperties(e, "Label");

            e.Params["Controls"]["Label"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Text"]["Description"].Value = @"The text visible to 
the end user in his browser";

            e.Params["Controls"]["Label"]["Properties"]["Tag"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Tag"]["Description"].Value = @"Which HTML tag 
will be rendered by the control. There are many legal values for this property, some of them are 'p', 
'div', 'span', 'label', 'li' [use panel for 'ul'] and 'address'. But also many more. Check up the 
standard for HTML5 if you'd like to wish all its legal values. All 'normal HTML elements', which doesn't 
need special attributes or child elements can really be described by modifying this property accordingly";

            e.Params["Controls"]["Label"]["Properties"]["For"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["For"]["Description"].Value = @"Will couple 
the control with an existing CheckBox or RadioButton on the form. Set this to the 'ID' property 
of whatever CheckBox or RadioButton you wish to associate this Label with. It probably won't work 
in your browser unless you also set the 'Tag' property to 'label'";
        }

        /*
         * Helper for above ...
         */
        private static void CreateButton(ActiveEventArgs e)
        {
            e.Params["Controls"]["Button"]["Name"].Value = "Magix Button";
            e.Params["Controls"]["Button"]["TypeName"].Value = "Magix.MetaForms.Plugins.Button";
            e.Params["Controls"]["Button"]["ToolTip"].Value = @"Creates a Button type of 
control, which you can assign Click actions to, from which when the user clicks, will raise 
the actions you've associated with the button. You can have several buttons per form, and they 
can have different Text values to differentiate them for the user";

            GetCommonEventsAndProperties(e, "Button");

            e.Params["Controls"]["Button"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["Text"]["Description"].Value = @"The text displayed 
to the end user on top of the button";

            e.Params["Controls"]["Button"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["Button"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["Button"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the widget cannot be clicked in 
any ways by the end user";

            e.Params["Controls"]["Button"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["Button"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["Button"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["Button"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["Button"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["Button"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";
        }

        #endregion

        /**
         * Level2: Will return the Control tree hierarchy for the Meta Form
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetControlsForForm")]
        protected void Magix_MetaForms_GetControlsForForm(object sender, ActiveEventArgs e)
        {
            MetaForm f = null;
            if (e.Params.Contains("MetaFormName"))
                f = MetaForm.SelectFirst(Criteria.Eq("Name", e.Params["MetaFormName"].Get<string>()));
            else if (e.Params.Contains("ID"))
                f = MetaForm.SelectByID(e.Params["ID"].Get<int>());

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

        /**
         * Level2: Will show the Actions associated with the Event 'EventName' on the 
         * MetaForm.Node with the ID of 'ID'
         */
        [ActiveEvent(Name = "Magix.MetaForms.ShowAllActionsAssociatedWithFormEvent")]
        protected void Magix_MetaForms_ShowAllActionsAssociatedWithFormEvent(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["IsDelete"].Value = true;
            node["IsCreate"].Value = true;
            node["IsInlineEdit"].Value = false;
            node["Container"].Value = "child";
            node["Width"].Value = 16;
            node["Top"].Value = 20;
            node["FullTypeName"].Value = typeof(Action).FullName + "-META";
            node["GetObjectsEvent"].Value = "DBAdmin.DynamicType.GetObjectsNode";

            node["MetaFormNodeID"].Value = e.Params["ID"].Value;
            node["Header"].Value = string.Format(@"Actions for '{0}'",
                e.Params["EventName"].Get<string>());

            node["EventName"].Value = e.Params["EventName"].Value;
            node["CreateEventName"].Value = "Magix.MetaForms.OpenAppendNewActionDialogue";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 12;

            node["NoIdColumn"].Value = true;
            node["DeleteColumnEvent"].Value = "Magix.MetaForms.RemoveActionFromActionList";

            node["ReuseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Sink for getting the type information for editing Actions for form element on
         * grid system
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(Action).FullName + "-META")
            {
                e.Params["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
                e.Params["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
            }
        }

        /**
         * Level2: Sink for getting the list data for editing Actions for form element on
         * grid system
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() != typeof(Action).FullName + "-META")
                return;

            MetaForm.Node n = MetaForm.Node.SelectByID(e.Params["MetaFormNodeID"].Get<int>());
            string actionString = "";
            if (n.Contains("Actions"))
            {
                if (n["Actions"].Contains(e.Params["EventName"].Get<string>()))
                    actionString = n["Actions"][e.Params["EventName"].Get<string>()].Value;
            }

            int idxNo = 0;
            foreach (string idx in actionString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (idxNo >= e.Params["Start"].Get<int>() &&
                    idxNo < e.Params["End"].Get<int>())
                {
                    e.Params["Objects"]["o-" + idxNo]["ID"].Value = e.Params["MetaFormNodeID"].Value.ToString() + 
                        "|" + 
                        idxNo + 
                        "|" +
                        e.Params["EventName"].Get<string>();
                    e.Params["Objects"]["o-" + idxNo]["Properties"]["Name"].Value = idx;
                }
                idxNo += 1;
            }
            e.Params["SetCount"].Value = idxNo;
            e.Params["LockSetCount"].Value = true;
        }

        /**
         * Level2: Sink for deleting Meta Form Action from Event
         */
        [ActiveEvent(Name = "Magix.MetaForms.RemoveActionFromActionList")]
        protected void Magix_MetaForms_RemoveActionFromActionList(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(Action).FullName + "-META")
            {
                using (Transaction tr = Adapter.Instance.BeginTransaction())
                {
                    MetaForm.Node n = MetaForm.Node.SelectByID(int.Parse(e.Params["ID"].Get<string>().Split('|')[0]));
                    int idxNo = 0;
                    int toRemove = int.Parse(e.Params["ID"].Get<string>().Split('|')[1]);
                    string result = "";
                    foreach (string idx in n["Actions"][e.Params["ID"].Get<string>().Split('|')[2]].Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (idxNo != toRemove)
                            result += idx + "|";
                        idxNo += 1;
                    }
                    result = result.Trim('|');
                    n["Actions"][e.Params["ID"].Get<string>().Split('|')[2]].Value = result;
                    n.Save();
                    tr.Commit();
                }

                Node node = new Node();
                node["FullTypeName"].Value = typeof(Action).FullName + "-META";

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        /**
         * Level2: Will show the 'List of Actions form' for appending and editing and deleting
         * actions associated with the specific Action on the specific Widget
         */
        [ActiveEvent(Name = "Magix.MetaForms.OpenAppendNewActionDialogue")]
        protected void Magix_MetaForms_OpenAppendNewActionDialogue(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(Action).FullName;
            node["Container"].Value = "child";
            node["Width"].Value = 15;
            node["Top"].Value = 20;
            node["ParentID"].Value = e.Params["MetaFormNodeID"].Value;
            node["ParentPropertyName"].Value = e.Params["EventName"].Value;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 9;
            node["WhiteListColumns"]["Params"].Value = true;
            node["WhiteListColumns"]["Params"]["ForcedWidth"].Value = 2;

            node["NoIdColumn"].Value = true;
            node["IsCreate"].Value = false;
            node["IsDelete"].Value = false;
            node["IsFind"].Value = true;
            node["IsSelect"].Value = true;
            node["GetContentsEventName"].Value = "DBAdmin.Data.GetContentsOfClass-Filter-Override";
            node["SetFocus"].Value = true;
            node["SelectEvent"].Value = "Magix.MetaForms.ActionWasSelected";
            node["SelectEvent"]["NodeID"].Value = e.Params["PropertyID"].Get<int>();

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Name"]["NoFilter"].Value = false;
            node["Type"]["Properties"]["Params"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Params"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Params"]["Header"].Value = "Pars.";

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaForms.ActionWasSelected")]
        protected void Magix_MetaForms_ActionWasSelected(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm.Node n = MetaForm.Node.SelectByID(e.Params["ParentID"].Get<int>());
                n["Actions"][e.Params["ParentPropertyName"].Get<string>()].Value += "|" +
                    Action.SelectByID(e.Params["ID"].Get<int>()).Name;
                n["Actions"][e.Params["ParentPropertyName"].Get<string>()].Value =
                    n["Actions"][e.Params["ParentPropertyName"].Get<string>()].Value.Trim('|');
                n.Save();

                tr.Commit();

                ActiveEvents.Instance.RaiseClearControls("child");
            }
        }

        /**
         * Level2: Returns the select list for selecting a Meta Form back to caller
         */
        [ActiveEvent(Name = "Magix.MetaForms.MetaView_Form.GetTemplateColumnSelectForm")]
        protected void Magix_MetaForms_MetaView_Form_GetTemplateColumnSelectForm(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["WebPartID"].Value = e.Params["WebPartID"].Value;
                    tx["Value"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a Meta Form ...", "");
            ls.Items.Add(item);

            foreach (MetaForm idx in MetaForm.Select())
            {
                ListItem it = new ListItem(idx.Name, idx.Name);
                if (idx.Name == e.Params["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }
        }

        /**
         * Level2: Expects to get an action-list [named actions, separated by pipes [|]] which
         * it will execute sequentially, in the order they are in. Expects to 
         * be raised from a WebPart, and needs 'ActionsToExecute' which is the piped 
         * list of Actions to execute
         */
        [ActiveEvent(Name = "Magix.MetaForms.RaiseActionsFromActionString")]
        protected void Magix_MetaForms_RaiseActionsFromActionString(object sender, ActiveEventArgs e)
        {
            string actions = e.Params["ActionsToExecute"].Get<string>();

            foreach (string idx in actions.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                e.Params["ActionName"].Value = idx;

                RaiseEvent(
                    "Magix.MetaAction.RaiseAction",
                    e.Params);

                e.Params["ActionName"].UnTie();
            }
        }
    }
}
