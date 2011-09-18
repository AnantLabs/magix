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
using System.IO;
using Magix.UX.Widgets.Core;
using System.Reflection;

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
            e.Params["Items"]["Forms"]["Image"].Value = "media/images/desktop-icons/rosetta.png";
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
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 3;

            node["ReuseNode"].Value = true;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Created"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.MetaForms.GetCopyMetaFormTemplateColumn";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaForms.EditForm";
            node["CreateEventName"].Value = "Magix.MetaForms.CreateNewMetaForm";

            node["Container"].Value = "content3";

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level2: Will return a LinkButton, which once clicked will copy and edit
         * a specific MetaForm
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetCopyMetaFormTemplateColumn")]
        protected void Magix_MetaForms_GetCopyMetaFormTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Creating our SelectList
            LinkButton ls = new LinkButton();
            ls.Text = "Copy";
            ls.Click +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = id;

                    RaiseEvent(
                        "Magix.MetaForms.CopyFormAndEdit",
                        node);
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Performs a Deep-Copy of the MetaForm and start editing it immediately
         */
        [ActiveEvent(Name = "Magix.MetaForms.CopyFormAndEdit")]
        protected void Magix_MetaForms_CopyFormAndEdit(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.MetaForms.CopyForm",
                e.Params);

            object cloneID = e.Params["NewID"].Value;

            Node n = new Node();

            n["FullTypeName"].Value = typeof(MetaForm).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);

            n = new Node();
            n["FullTypeName"].Value = typeof(MetaForm).FullName;
            n["ID"].Value = cloneID;

            RaiseEvent(
                "DBAdmin.Grid.SetActiveRow",
                n);

            n = new Node();
            n["ID"].Value = cloneID;

            RaiseEvent(
                "Magix.MetaForms.EditForm",
                n);
        }

        /**
         * Level2: Performs a Deep-Copy of the Form and returns the ID of the new MetaForm as 'NewID'
         */
        [ActiveEvent(Name = "Magix.MetaForms.CopyForm")]
        protected void Magix_MetaAction_CopyAction(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm a = MetaForm.SelectByID(e.Params["ID"].Get<int>());
                MetaForm clone = a.Clone();

                clone.Save();

                tr.Commit();

                e.Params["NewID"].Value = clone.ID;
            }
        }

        /**
         * Level2: Creates a new MetaForm and edits it in content4
         */
        [ActiveEvent(Name = "Magix.MetaForms.CreateNewMetaForm")]
        protected void Magix_MetaForms_CreateNewMetaForm(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm f = new MetaForm();
                f.Name = "Default";
                f.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = f.ID;

                RaiseEvent(
                    "Magix.MetaForms.EditForm",
                    node);
            }
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
            CreateButton(e);
            CreateLabel(e);
            CreateCheckBox(e);
            CreateTextBox(e);
            CreateHiddenField(e);
            CreateHyperLink(e);
            CreateImage(e);
            CreateLinkButton(e);
            CreatTextArea(e);
            CreateRadioButton(e);
            CreateCalendar(e);
            CreatePanel(e);
            CreateRepeater(e);
        }

        private void CreateRepeater(ActiveEventArgs e)
        {
            e.Params["Controls"]["Repeater"]["Name"].Value = "Repeater";
            e.Params["Controls"]["Repeater"]["TypeName"].Value = "Magix.MetaForms.Plugins.Repeater";
            e.Params["Controls"]["Repeater"]["HasSurface"].Value = true;
            e.Params["Controls"]["Repeater"]["List"].Value = true;
            e.Params["Controls"]["Repeater"]["ToolTip"].Value = @"Creates a Repeater type of 
control, which you can use for conveying information which are lists or similar. Utilizing the 
repeater, you can for instance create your own Data Grids to display lists of information or objects. The 
Widgets you add to a Repeater should first of all _NOT_ have ID values, secondly they will not render 
as one widget, but in fact rather be used as a template for repeating whatever DataSource you have 
databinded your repeater towards such that the number of items in your node structure for 
the node you DataBind your repeater towards, will define how many times the repeater will repeat itself, 
rendering every single control you have asked it to to n times. To understand Repeaters, it is crucial that 
you understand the Eval DataBind method of Magix";

            GetCommonEventsAndProperties(e, "Repeater", true);

            // The Repeater has a 'unique' type of Info property logic, and hence need its own
            // more thorough explanation ...
            e.Params["Controls"]["Repeater"]["Properties"]["Info"]["Description"].Value = @"The Info 
property is unique when it comes to all List controls, including the Repeater, because it defines 
which DataSource expression it should use. Valid values includes; DataSource[Objects], 
DataSource[2][Properties], or relative paths [Actions][3] etc if the Repeater is itself 
within another Listable type of control. However, the DataBinding expression must 
return a listable type of collection. Whenever you are dealing with Listable types of controls, then 
the databinding expressions can become relative. Meaning if they do not start with DataSource, the 
databinding will occur towards the current index property node of the first listable control upwards in 
the hierarchy from the control currently being databinded. Example, you are databinding a Repeater 
with DataSource[Objects] and then have a button databinding its Text value with [Properties][Name].Value. 
Then the first button, which is the one in your first Repeater row will render with whatever value is 
in the DataSource[Objects][0][Properties][Name].Value, converted into Text, since the Text property of 
the Button is of type Text. The second Repeater row will contain the value of DataSource[Objects][1] ... 
etc ... And you will get one repeater row for every item within DataSource[Objects]. 
This is true for all Listable Widgets, such as the Repeater";

            
            e.Params["Controls"]["Repeater"]["Properties"]["Tag"].Value = typeof(string).FullName;
            e.Params["Controls"]["Repeater"]["Properties"]["Tag"]["Description"].Value = @"Which HTML tag 
will be rendered by the widget. There are many legal values for this property, some of them are p, 
div, span, label, li, ul. But also many more. Check up the 
standard for HTML5 if you would like to wish all its legal values. All normal HTML elements, which does not 
need special attributes or child elements can really be described by modifying this property accordingly. 
Also HTML5 types of elements, such as address and section. However, realize that since this is a Repeater, 
it is expected to repeat its value several times according to how many items you DataBind your Repeater 
towards. Just thinking out loud here ... ;)";
        }

        private void CreatePanel(ActiveEventArgs e)
        {
            e.Params["Controls"]["Panel"]["Name"].Value = "Panel";
            e.Params["Controls"]["Panel"]["TypeName"].Value = "Magix.MetaForms.Plugins.Panel";
            e.Params["Controls"]["Panel"]["HasSurface"].Value = true;
            e.Params["Controls"]["Panel"]["ToolTip"].Value = @"Creates a Panel type of 
control, which you can use as a Panel for hosting other types of Controls. Renders as a div by default, 
which is highly useful in regards to Pinch Zooming and such for dividing your page up into 
zoomable elements";

            GetCommonEventsAndProperties(e, "Panel", true);

            e.Params["Controls"]["Panel"]["Properties"]["Tag"].Value = typeof(string).FullName;
            e.Params["Controls"]["Panel"]["Properties"]["Tag"]["Description"].Value = @"Which HTML tag 
will be rendered by the widget. There are many legal values for this property, some of them are p, 
div, span, label, li, ul and address. But also many more. Check up the 
standard for HTML5 if you would like to wish all its legal values. All normal HTML elements 
can really be described by modifying this property accordingly. 
Also HTML5 types of elements, such as address and section";
        }

        /*
         * Helper for above ...
         */
        private void CreateCalendar(ActiveEventArgs e)
        {
            e.Params["Controls"]["Calendar"]["Name"].Value = "Calendar";
            e.Params["Controls"]["Calendar"]["TypeName"].Value = "Magix.MetaForms.Plugins.Calendar";
            e.Params["Controls"]["Calendar"]["ToolTip"].Value = @"Creates a Calendar type of 
control, which you can use if you need the end user to pick a specific date";

            GetCommonEventsAndProperties(e, "Calendar", true);

            e.Params["Controls"]["Calendar"]["Properties"]["Value"].Value = typeof(DateTime).FullName;
            e.Params["Controls"]["Calendar"]["Properties"]["Value"]["Description"].Value = @"The 
DateTime selected date of the Calendar";

            e.Params["Controls"]["Calendar"]["Events"]["DateChanged"].Value = true;
            e.Params["Controls"]["Calendar"]["Events"]["DateChanged"]["Description"].Value = @"Raised when 
the DateTime Value has changed";

            e.Params["Controls"]["Calendar"]["Events"]["DateSelected"].Value = true;
            e.Params["Controls"]["Calendar"]["Events"]["DateSelected"]["Description"].Value = @"Raised when 
the DateTime Value has changed, byt the end user clicking a specific date, versus just browsing around in years as 
the DateChanged event does";
        }

        /*
         * Helper for above ...
         */
        private void CreateRadioButton(ActiveEventArgs e)
        {
            e.Params["Controls"]["RadioButton"]["Name"].Value = "RadioButton";
            e.Params["Controls"]["RadioButton"]["TypeName"].Value = "Magix.MetaForms.Plugins.RadioButton";
            e.Params["Controls"]["RadioButton"]["ToolTip"].Value = @"Creates a RadioButton type of 
control, which you can assign Text to and CheckedChanged Event Handlers. Useful for multiple 
choice types of input data, such as; Milk, Beer or Water";

            GetCommonEventsAndProperties(e, "RadioButton", true);

            e.Params["Controls"]["RadioButton"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["RadioButton"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["RadioButton"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["RadioButton"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["RadioButton"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the control value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["RadioButton"]["Properties"]["Checked"].Value = typeof(bool).FullName;
            e.Params["Controls"]["RadioButton"]["Properties"]["Checked"]["Description"].Value = @"The 
Checked state of your RadioButton, true will tag it off as checked, while false [the default] will 
keep it open";

            e.Params["Controls"]["RadioButton"]["Properties"]["GroupName"].Value = typeof(string).FullName;
            e.Params["Controls"]["RadioButton"]["Properties"]["GroupName"]["Description"].Value = @"The 
group of RadioButtons the RadioButton will belong to. If you have got three RadioButtons, and they are 
all a part of the same single-selection from multiple choices, then only one of them can be 
checked at the same time. So checking off one, will uncheck off any previously checked RadioButtons within 
the same GrouName value";

            e.Params["Controls"]["RadioButton"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["RadioButton"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["RadioButton"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["RadioButton"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";

            e.Params["Controls"]["RadioButton"]["Events"]["CheckedChanged"].Value = true;
            e.Params["Controls"]["RadioButton"]["Events"]["CheckedChanged"]["Description"].Value = @"Raised when 
the checked state has changed, either by clicking or through some other user interaction";
        }

        /*
         * Helper for above ...
         */
        private void CreatTextArea(ActiveEventArgs e)
        {
            e.Params["Controls"]["TextArea"]["Name"].Value = "TextArea";
            e.Params["Controls"]["TextArea"]["TypeName"].Value = "Magix.MetaForms.Plugins.TextArea";
            e.Params["Controls"]["TextArea"]["ToolTip"].Value = @"Creates a TextArea type of 
control, which you can assign Text to and TextChanged Event Handlers";

            GetCommonEventsAndProperties(e, "TextArea", true);

            e.Params["Controls"]["TextArea"]["Properties"]["PlaceHolder"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextArea"]["Properties"]["PlaceHolder"]["Description"].Value = @"The 
watermark text of your TextArea. Will show when TextArea is empty, as a cue to the end user for what to 
type into it";

            e.Params["Controls"]["TextArea"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextArea"]["Properties"]["Text"]["Description"].Value = @"The 
visible text for the end user and also the text fragment the user can change by editing the text box value";

            e.Params["Controls"]["TextArea"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextArea"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["TextArea"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["TextArea"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["TextArea"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the control value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["TextArea"]["Events"]["TextChanged"].Value = true;
            e.Params["Controls"]["TextArea"]["Events"]["TextChanged"]["Description"].Value = @"Raised when 
the text has changed, and the user chooses to leave the field and move to another field on the form by 
e.g. clicking with his mouse or using TAB such that the TextArea looses focus";

            e.Params["Controls"]["TextArea"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["TextArea"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["TextArea"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["TextArea"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";
        }

        /*
         * Helper for above ...
         */
        private void CreateLinkButton(ActiveEventArgs e)
        {
            e.Params["Controls"]["LinkButton"]["Name"].Value = "LinkButton";
            e.Params["Controls"]["LinkButton"]["TypeName"].Value = "Magix.MetaForms.Plugins.LinkButton";
            e.Params["Controls"]["LinkButton"]["ToolTip"].Value = @"Creates a LinkButton type of 
control, which you can assign Click actions to, from which when the user clicks, will raise 
the actions you have associated with the LinkButton. You can have multiple LinkButtons per form, and they 
can have different Text values to differentiate them for the user";

            GetCommonEventsAndProperties(e, "LinkButton", true);

            e.Params["Controls"]["LinkButton"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["LinkButton"]["Properties"]["Text"]["Description"].Value = @"The text displayed 
to the end user on top of the LinkButton";

            e.Params["Controls"]["LinkButton"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["LinkButton"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["LinkButton"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the widget cannot be clicked in 
any ways by the end user";

            e.Params["Controls"]["LinkButton"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["LinkButton"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["LinkButton"]["Events"]["Blur"].Value = true;
            e.Params["Controls"]["LinkButton"]["Events"]["Blur"]["Description"].Value = @"Raised when 
the user moves focus away from the Widget by for instance clicking the TAB key while the Widget has 
focus, or clicking another place with his mouse or touch screen while the widget has focus";

            e.Params["Controls"]["LinkButton"]["Events"]["Focused"].Value = true;
            e.Params["Controls"]["LinkButton"]["Events"]["Focused"]["Description"].Value = @"Raised when 
the user moves focus ONTO the Widget by for instance clicking the TAB key such that the Widget gets 
focus, or clicking the widget with his mouse or touch screen";
        }

        /*
         * Helper for above ...
         */
        private void CreateImage(ActiveEventArgs e)
        {
            e.Params["Controls"]["Image"]["Name"].Value = "Image";
            e.Params["Controls"]["Image"]["TypeName"].Value = "Magix.MetaForms.Plugins.Image";
            e.Params["Controls"]["Image"]["ToolTip"].Value = @"Creates an Image type of 
control, which you can assign an Image URL to";

            GetCommonEventsAndProperties(e, "Image", true);

            e.Params["Controls"]["Image"]["Properties"]["AlternateText"].Value = typeof(string).FullName;
            e.Params["Controls"]["Image"]["Properties"]["AlternateText"]["Description"].Value = @"The 
text shown if the image is broken. Also highly useful for conveying information about the image 
to people who have challenged eye sight and such. _SET THIS PROPERTY_ to something intelligent, since 
it is rude not to";

            e.Params["Controls"]["Image"]["Properties"]["ImageUrl"].Value = typeof(string).FullName;
            e.Params["Controls"]["Image"]["Properties"]["ImageUrl"]["Description"].Value = @"The 
actual URL to the Image used. Can be relative or absolute [starting with http://]";

            e.Params["Controls"]["Image"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["Image"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox 
for instance";
        }

        /*
         * Helper for above ...
         */
        private void CreateHyperLink(ActiveEventArgs e)
        {
            e.Params["Controls"]["HyperLink"]["Name"].Value = "HyperLink";
            e.Params["Controls"]["HyperLink"]["TypeName"].Value = "Magix.MetaForms.Plugins.HyperLink";
            e.Params["Controls"]["HyperLink"]["ToolTip"].Value = @"Creates a HyperLink type of 
control, which you can assign a URL to, which once clicked will bring the user to that URL";

            GetCommonEventsAndProperties(e, "HyperLink", true);

            e.Params["Controls"]["HyperLink"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["HyperLink"]["Properties"]["Text"]["Description"].Value = @"The 
visible text for the end user";

            e.Params["Controls"]["HyperLink"]["Properties"]["URL"].Value = typeof(string).FullName;
            e.Params["Controls"]["HyperLink"]["Properties"]["URL"]["Description"].Value = @"The 
actual URL the user will be brought to when he clicks the hyperlink";

            e.Params["Controls"]["HyperLink"]["Properties"]["Target"].Value = typeof(string).FullName;
            e.Params["Controls"]["HyperLink"]["Properties"]["Target"]["Description"].Value = @"The 
named browser window you wish to use, unless you want the current window to be replaced. Or _blank 
to make sure it is always a new window";

            e.Params["Controls"]["HyperLink"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["HyperLink"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox 
for instance";
        }

        /*
         * Helper for above ...
         */
        private void CreateHiddenField(ActiveEventArgs e)
        {
            e.Params["Controls"]["HiddenField"]["Name"].Value = "HiddenField";
            e.Params["Controls"]["HiddenField"]["TypeName"].Value = "Magix.MetaForms.Plugins.HiddenField";
            e.Params["Controls"]["HiddenField"]["ToolTip"].Value = @"Creates a HiddenField type of 
control, which you can assign Values to. A hidden field widget is not visible for the end user, 
but is still a value control, and can be databinded to hold values the user shouldn not see, but must still 
somehow follow him as he proceeds through the flow of your process";

            GetCommonEventsAndProperties(e, "HiddenField", false);

            e.Params["Controls"]["HiddenField"]["Properties"]["Value"].Value = typeof(string).FullName;
            e.Params["Controls"]["HiddenField"]["Properties"]["Value"]["Description"].Value = @"The 
visible text for the end user and also the text fragment the user can change by editing the text box value";
        }

        /*
         * Helper for above ...
         */
        private static void GetCommonEventsAndProperties(ActiveEventArgs e, string typeName, bool isWebControl)
        {
            e.Params["Controls"][typeName]["Properties"]["ID"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["ID"]["Description"].Value = @"String ID used for 
uniquely identifying a widget later, for extracting its value or changing its state somehow. Must be unique 
per Meta Form. Should _not_ be set for widgets inside of Repeaters or other listable types of controls. Can only 
contain alphanumerical characters [a-z|A-Z|0-9], and it should start with an a-z|A-Z character. In fact, unless 
you are certain about what you are doing, and what you want to achieve, you probably should _NOT_ mess 
with this property";

            if (isWebControl)
            {
                e.Params["Controls"][typeName]["Properties"]["CssClass"].Value = typeof(string).FullName;
                e.Params["Controls"][typeName]["Properties"]["CssClass"]["Default"].Value = "span-2";
                e.Params["Controls"][typeName]["Properties"]["CssClass"]["Description"].Value = @"The CSS 
class of the control. CSS classes can be concatenated by adding spaces between them if you wish to 
use multiple CSS classes for the same control. If you do not know what a CSS class is, or all of this is 
rubbish to you, you would probably be better of not using this property, but instead indirectly through 
the Builder Short Cut Button create your animations and styles for your Widgets";

                e.Params["Controls"][typeName]["Properties"]["Dir"].Value = typeof(string).FullName;
                e.Params["Controls"][typeName]["Properties"]["Dir"]["Description"].Value = @"Direction of text 
on the widget. If you set it to rtl, it will display characters from right to left, as in the way 
it is being done in among other things in Arabic";

                e.Params["Controls"][typeName]["Properties"]["ToolTip"].Value = typeof(string).FullName;
                e.Params["Controls"][typeName]["Properties"]["ToolTip"]["Description"].Value = @"Tooltip hint 
given to user when he moves cursor over widget. Obviously useless, for apparent reasons, on tablets, iPhones 
and such";

                e.Params["Controls"][typeName]["Properties"]["TabIndex"].Value = typeof(string).FullName;
                e.Params["Controls"][typeName]["Properties"]["TabIndex"]["Description"].Value = @"Numerical 
value indicating which order the widget is within the tab hierarchy, meaning which control is next if 
the user clicks the TAB key on the keyboard. A control which has TabIndex of 2 will gain focus when 
the user hits TAB and the widget that has currently focus has 1 as TabIndex, and so on";

                // events ...

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
the left mouse button is squeezed, but before it is released, on top of the specific widget. Obviously, for apparent 
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

                e.Params["Controls"][typeName]["ShortCuts"]["Builder"]["Text"].Value = "Builder ...";
                e.Params["Controls"][typeName]["ShortCuts"]["Builder"]["ToolTip"].Value = "Opens up the Style Builder such that you can create Animations and Styles for your Widget as you please. Add some Candy to your widgets by changing the way they Animate, Behave and Look like. Fonts, colors, etc, etc, etc. It is all done from here ... ;)";
                e.Params["Controls"][typeName]["ShortCuts"]["Builder"]["CssClass"].Value = "mux-shortcut-builder";
                e.Params["Controls"][typeName]["ShortCuts"]["Builder"]["Event"].Value = "Magix.MetaForms.OpenStyleBuilderForWidget";
            }

            e.Params["Controls"][typeName]["Properties"]["Info"].Value = typeof(string).FullName;
            e.Params["Controls"][typeName]["Properties"]["Info"]["Description"].Value = @"Additional 
information which can be stored within your object, which is not visible for the end user in any 
ways, but still will follow your Widget around as a small piece of information storage";

            // Shortcut buttons ...
            e.Params["Controls"][typeName]["ShortCuts"]["Delete"]["Text"].Value = "Delete!";
            e.Params["Controls"][typeName]["ShortCuts"]["Delete"]["ToolTip"].Value = "Deletes the currently selected widget. Be careful, this operation _cannot_ be undone ... !!";
            e.Params["Controls"][typeName]["ShortCuts"]["Delete"]["CssClass"].Value = "mux-shortcut-delete";
            e.Params["Controls"][typeName]["ShortCuts"]["Delete"]["Event"].Value = "Magix.MetaForms.DeleteMetaFormWidgetFromForm";
        }

        /*
         * Helper for above ...
         */
        private void CreateTextBox(ActiveEventArgs e)
        {
            e.Params["Controls"]["TextBox"]["Name"].Value = "TextBox";
            e.Params["Controls"]["TextBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.TextBox";
            e.Params["Controls"]["TextBox"]["ToolTip"].Value = @"Creates a TextBox type of 
control, which you can assign Text to and TextChanged Event Handlers. TextBoxes are the by far most 
common type of input control, since it can take any types of input values from the end user";

            GetCommonEventsAndProperties(e, "TextBox", true);

            e.Params["Controls"]["TextBox"]["Properties"]["TextMode"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["TextMode"]["Description"].Value = @"Can be 
either Normal [default], Password, Email, Number or Phone. Changes the input type accordingly 
such that modern browsers can help the end user with getting the input correctly";

            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"].Value = typeof(string).FullName;
            e.Params["Controls"]["TextBox"]["Properties"]["PlaceHolder"]["Description"].Value = @"The 
watermark text of your textbox. Will show when textbox is empty, as a cue to the end user for what to 
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
will attempt at automatically completing the field according to the user settings within the browser. Doesn not 
always work since it tries to parse the field type of information according to its ID, which you have little or 
no absolute control over";

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
will enable the widget [true is its default value]. If it is false, the control value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["TextBox"]["Events"]["TextChanged"].Value = true;
            e.Params["Controls"]["TextBox"]["Events"]["TextChanged"]["Description"].Value = @"Raised when 
the text has changed, and the user chooses to leave the field and move to another field on the form by 
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
            e.Params["Controls"]["CheckBox"]["Name"].Value = "CheckBox";
            e.Params["Controls"]["CheckBox"]["TypeName"].Value = "Magix.MetaForms.Plugins.CheckBox";
            e.Params["Controls"]["CheckBox"]["ToolTip"].Value = @"Creates a CheckBox type of 
control, which you can assign Text to and CheckedChanged Event Handlers. Useful for things that black and 
white in nature, such as yes and no questions";

            GetCommonEventsAndProperties(e, "CheckBox", true);

            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"].Value = typeof(string).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["AccessKey"]["Description"].Value = @"The keyboard 
shortcut key, often combined with e.g. ALT+SHIFT+x where x is any single key which can legally serve 
as a shortcut, which depends upon your platform of choice. ALT+SHIFT+X is for Windows and FireFox for instance";

            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"].Value = typeof(bool).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"]["Default"].Value = true;
            e.Params["Controls"]["CheckBox"]["Properties"]["Enabled"]["Description"].Value = @"If true, 
will enable the widget [true is its default value]. If it is false, the control value cannot be changed in 
any ways by the end user";

            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"].Value = typeof(bool).FullName;
            e.Params["Controls"]["CheckBox"]["Properties"]["Checked"]["Description"].Value = @"The 
Checked state of your CheckBox, true will tag it off as checked, while false [the default] will 
keep it open";

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
            e.Params["Controls"]["Label"]["Name"].Value = "Label";
            e.Params["Controls"]["Label"]["TypeName"].Value = "Magix.MetaForms.Plugins.Label";
            e.Params["Controls"]["Label"]["ToolTip"].Value = @"Creates a Label type of 
control, which you can assign Text to. Basically serves as a read-only textual fragment on your page. 
Change which HTML tag it is being rendered with by setting its Tag property";

            GetCommonEventsAndProperties(e, "Label", true);

            e.Params["Controls"]["Label"]["Properties"]["Text"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Text"]["Description"].Value = @"The text visible to 
the end user in his browser";

            e.Params["Controls"]["Label"]["Properties"]["Tag"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["Tag"]["Description"].Value = @"Which HTML tag 
will be rendered by the control. There are many legal values for this property, some of them are p, 
div, span, label, li [use panel for ul] and address. But also many more. Check up the 
standard for HTML5 if you would like to wish all its legal values. All normal HTML elements, which doesn not 
need special attributes or child elements can really be described by modifying this property accordingly";

            e.Params["Controls"]["Label"]["Properties"]["For"].Value = typeof(string).FullName;
            e.Params["Controls"]["Label"]["Properties"]["For"]["Description"].Value = @"Will couple 
the control with an existing CheckBox or RadioButton on the form. Set this to the ID property 
of whatever CheckBox or RadioButton you wish to associate this Label with. It probably will not work 
in your browser unless you also set the Tag property to label";
        }

        /*
         * Helper for above ...
         */
        private static void CreateButton(ActiveEventArgs e)
        {
            e.Params["Controls"]["Button"]["Name"].Value = "Button";
            e.Params["Controls"]["Button"]["TypeName"].Value = "Magix.MetaForms.Plugins.Button";
            e.Params["Controls"]["Button"]["ToolTip"].Value = @"Creates a Button type of 
control, which you can assign Click actions to, from which when the user clicks, will raise 
the actions you have associated with the button. You can have several buttons per form, and they 
can have different Text values to differentiate them for the user";

            GetCommonEventsAndProperties(e, "Button", true);

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
            // Creating our Control, if its our duty, and setting its default values ...
            switch (e.Params["TypeName"].Get<string>())
            {
                case "Magix.MetaForms.Plugins.Button":
                    {
                        Button btn = new Button();
                        btn.Text = "[null]";
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.Label":
                    {
                        Label btn = new Label();
                        btn.Text = "[null]";
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.CheckBox":
                    {
                        CheckBox btn = new CheckBox();
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.TextBox":
                    {
                        TextBox btn = new TextBox();
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.HiddenField":
                    {
                        if (e.Params.Contains("Preview") &&
                            e.Params["Preview"].Get<bool>())
                        {
                            TextBox btn = new TextBox();
                            btn.Style[Styles.position] = "absolute";
                            btn.CssClass = "span-2";
                            btn.ToolTip = "Will render as a HiddenField in non-preview modes ... However, we cannot render it as a hidden field while in WYSIWYG mode since those are invisible, and you wouldn not have any things to select to be able to modify its properties ...";
                            e.Params["Control"].Value = btn;
                        }
                        else
                        {
                            HiddenField btn = new HiddenField();
                            e.Params["Control"].Value = btn;
                        }
                    } break;
                case "Magix.MetaForms.Plugins.Image":
                    {
                        Image btn = new Image();
                        btn.AlternateText = "Anonymous Coward Image";
                        btn.Style[Styles.floating] = "left";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.HyperLink":
                    {
                        HyperLink btn = new HyperLink();
                        btn.CssClass = "span-2";
                        btn.Text = "Anonymous Coward Anchor Text";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.LinkButton":
                    {
                        LinkButton btn = new LinkButton();
                        btn.CssClass = "span-2";
                        btn.Text = "Anonymous Coward Anchor Text";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.TextArea":
                    {
                        TextArea btn = new TextArea();
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.RadioButton":
                    {
                        RadioButton btn = new RadioButton();
                        btn.CssClass = "span-2";
                        e.Params["Control"].Value = btn;
                    } break;
                case "Magix.MetaForms.Plugins.Calendar":
                    {
                        if (e.Params.Contains("Preview") &&
                            e.Params["Preview"].Get<bool>())
                        {
                            Label btn = new Label();
                            btn.CssClass = "mux-calendar mux-rounded mux-shaded";
                            btn.Tag = "div";
                            btn.Text = "Will render as a Calendar in front-web";
                            e.Params["Control"].Value = btn;
                        }
                        else
                        {
                            Calendar btn = new Calendar();
                            btn.CssClass = "mux-calendar mux-rounded mux-shaded";
                            e.Params["Control"].Value = btn;
                        }
                    } break;
                case "Magix.MetaForms.Plugins.Panel":
                    {
                        Panel btn = new Panel();
                        btn.CssClass = "span-2 height-2";

                        if (e.Params.Contains("Preview") &&
                            e.Params["Preview"].Get<bool>())
                        {
                            btn.Style[Styles.border] = "dashed 1px rgba(0,0,0,.2)";
                        }

                        e.Params["Control"].Value = btn;
                        e.Params["HasSurface"].Value = true;
                    } break;
                case "Magix.MetaForms.Plugins.Repeater":
                    {
                        Panel btn = new Panel();
                        btn.CssClass = "span-2 height-2";

                        if (e.Params.Contains("Preview") &&
                            e.Params["Preview"].Get<bool>())
                        {
                            e.Params["List"].Value = true;
                            btn.Style[Styles.border] = "dashed 1px rgba(0,0,0,.2)";
                            if (e.Params.Contains("ControlNode"))
                            {
                                Node c = e.Params["ControlNode"].Value as Node;
                                if (c.Contains("Properties") &&
                                    c["Properties"].Contains("Info") &&
                                    ((c["Properties"]["Info"].Value as string ?? "").StartsWith("DataSource") ||
                                    (c["Properties"]["Info"].Value as string ?? "").StartsWith("[")))
                                {
                                    Label d = new Label();
                                    d.Style[Styles.position] = "absolute";
                                    d.Style[Styles.bottom] = "0";
                                    d.Style[Styles.right] = "0";
                                    d.Style[Styles.opacity] = ".5";
                                    d.Text = "data:" + c["Properties"]["Info"].Value as string;
                                    btn.Controls.Add(d);
                                }
                            }
                        }

                        e.Params["Control"].Value = btn;
                        e.Params["HasSurface"].Value = true;
                        e.Params["CreateChildControlsEvent"].Value = "Magix.MetaForms.CreateRepeaterChildControlCollection";
                    } break;
                default:
                    // DO NOTHING. Others might handle ...
                    break;
            }

            SetProperties(e.Params);

            // Making sure we're rendering the styles needed ...
            RenderStyles(e.Params["Control"].Value as BaseWebControl, e.Params);
        }

        /*
         * Helper for above ...
         */
        private void RenderStyles(BaseWebControl ctrl, Node node)
        {
            if (ctrl == null)
                return;

            if (!node.Contains("ControlNode"))
                return;

            node = node["ControlNode"].Value as Node;

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

        private void SetProperties(Node node)
        {
            System.Web.UI.Control ctrl = node["Control"].Value as System.Web.UI.Control;

            Node dataSource = node.Contains("DataSource") ? node["DataSource"].Value as Node : null;

            if (node.Contains("ControlNode") &&
                node["ControlNode"].Value != null &&
                (node["ControlNode"].Value as Node).Contains("Properties"))
            {
                foreach (Node idx in (node["ControlNode"].Value as Node)["Properties"])
                {
                    if (idx.Name == "_ID")
                        continue;

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

                        if (dataSource != null && tmp is string && (tmp as string).StartsWith("["))
                            tmp = GetExpression(tmp as string, dataSource);

                        if (tmp.GetType() != info.GetGetMethod(true).ReturnType)
                        {
                            switch (info.GetGetMethod(true).ReturnType.FullName)
                            {
                                case "System.String":
                                    tmp = tmp.ToString();
                                    break;
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

            // Only setting ID if it's undefined in properties ...
            if (string.IsNullOrEmpty(ctrl.ID))
                ctrl.ID = "ID" + node["_ID"].Value.ToString();
        }

        private string GetExpression(string expr, Node dataSource)
        {
            if (expr == null)
                return null;

            if (dataSource == null)
                return null;

            object p = GetObjectFromExpression(expr, dataSource);

            if (p == null)
                return null;

            switch (p.GetType().FullName)
            {
                case "System.String":
                    return p.ToString();
                case "System.Boolean":
                    return p.ToString();
                case "System.DateTime":
                    return ((DateTime)p).ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                case "System.Decimal":
                    return ((decimal)p).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "System.Int32":
                    return ((int)p).ToString(System.Globalization.CultureInfo.InvariantCulture);
                default:
                    return expr; // 
            }
        }

        private object GetObjectFromExpression(string expr, Node dataSource)
        {
            if (expr.StartsWith("["))
            {
                // 'Static' value, not 'relative' ...
                // Scanning forwards from after 'DataSource' ...
                Node x = dataSource;
                bool isInside = false;
                string bufferNodeName = null;
                string lastEntity = null;
                for (int idx = 0; idx < expr.Length; idx++)
                {
                    char tmp = expr[idx];
                    if (isInside)
                    {
                        if (tmp == ']')
                        {
                            lastEntity = "";
                            if (!x.Contains(bufferNodeName))
                            {
                                return null;
                            }

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
                                if (x.Count >= intIdx)
                                    x = x[intIdx];
                                return null;
                            }
                            else
                            {
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

        /**
         * Level2: Will databind a Repeater towards the incoming Node structure according to 
         * how the Repeater is supposed to be DataBinded, which again is according to its Info field.
         * Will also Databind [obviously!] every Child Widget of your Repeater, and basically 
         * create as many rows as there are items in your Databinded Node.
         */
        [ActiveEvent(Name = "Magix.MetaForms.CreateRepeaterChildControlCollection")]
        protected void Magix_MetaForms_CreateRepeaterChildControlCollection(object sender, ActiveEventArgs e)
        {
            // Yet again, looks stupid, but feels safish ...
            if (e.Params.Contains("Preview") &&
                    e.Params["Preview"].Get<bool>())
            {
                CreateSingleRepeaterControl(
                    true,
                    e.Params["Controls"].Value as Node,
                    e.Params["Control"].Value as System.Web.UI.Control,
                    e.Params.Contains("OldSelected") ? 
                        e.Params["OldSelected"].Get<string>() : 
                        null,
                        null);
            }
            else
            {
                CreateSingleRepeaterControl(
                    false,
                    e.Params["Controls"].Value as Node,
                    e.Params["Control"].Value as System.Web.UI.Control,
                    null,
                    e.Params.Contains("DataSource") ? e.Params["DataSource"].Value as Node : null);
            }
        }

        /*
         * Helper for above ...
         */
        private void CreateSingleRepeaterControl(
            bool preview, 
            Node ctrls, 
            System.Web.UI.Control ctrl, 
            string oldSelected,
            Node dataSource)
        {
            int rows = 0;
            if (preview)
                rows = 1;
            else if (dataSource != null)
                rows = dataSource.Count;

            for (int idxNo = 0; idxNo < rows; idxNo++)
            {
                foreach (Node idx in ctrls)
                {
                    if (idx.Name == "_ID")
                        continue;

                    Node nn = new Node();

                    nn["TypeName"].Value = idx["TypeName"].Get<string>();

                    if (preview)
                        nn["Preview"].Value = preview;

                    nn["ControlNode"].Value = idx;
                    nn["_ID"].Value = idx["_ID"].Value;

                    if (dataSource != null)
                        nn["DataSource"].Value = dataSource[idxNo];

                    RaiseEvent(
                        "Magix.MetaForms.CreateControl",
                        nn);

                    nn["ControlNode"].UnTie(); // to be sure ...

                    if (nn.Contains("Control"))
                    {
                        System.Web.UI.Control ct = nn["Control"].Value as System.Web.UI.Control;

                        if (preview && ct is BaseWebControl)
                        {
                            BaseWebControl ctr = ct as BaseWebControl;

                            object id = idx["_ID"].Value;

                            ctr.Click +=
                                delegate
                                {
                                    Node t = new Node();
                                    t["ID"].Value = id;

                                    RaiseEvent(
                                        "Magix.MetaForms.SetActiveEditingMetaFormWidget",
                                        t);
                                };
                            ctr.Load +=
                                delegate
                                {
                                    if (ctr.ClientID == oldSelected &&
                                        !ctr.CssClass.Contains(" mux-wysiwyg-selected"))
                                    {
                                        AddSelectedCssClass(ctr);
                                        ctr.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                                    }
                                };
                            ctr.Style[Styles.position] = "relative";
                        }

                        ctrl.Controls.Add(ct);

                        if (idx.Contains("Surface"))
                        {
                            if (idx.Contains("CreateChildControlsEvent"))
                            {
                                // Listable control type ...
                                Node tmp = new Node();

                                // Yup, looks stupidish, but feel very safe ... ;)
                                tmp["Controls"].Value = idx["Surface"];
                                tmp["Control"].Value = ct;
                                if (idx.Contains("Properties") &&
                                    idx["Properties"].Contains("Info"))
                                {
                                    tmp["DataSource"].Value = GetObjectFromExpression(idx["Properties"]["Info"].Get<string>(), dataSource);
                                }

                                RaiseEvent( // No safe here, if this one fucks up, we're fucked ... !!
                                    nn["CreateChildControlsEvent"].Get<string>(),
                                    tmp);
                            }
                            else
                            {
                                foreach (Node idx3 in idx["Surface"])
                                {
                                    if (idx3.Name == "_ID")
                                        continue;

                                    CreateSingleControl(
                                        preview, 
                                        idx3, 
                                        ct, 
                                        (dataSource == null || dataSource.Count <= idxNo ? null : dataSource[idxNo]), 
                                        oldSelected);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateSingleControl(
            bool preview, 
            Node node, 
            System.Web.UI.Control parent, 
            Node dataSource, 
            string oldSelected)
        {
            Node nn = new Node();

            nn["TypeName"].Value = node["TypeName"].Get<string>();
            nn["ControlNode"].Value = node;
            nn["_ID"].Value = node["_ID"].Value;
            if (dataSource != null)
                nn["DataSource"].Value = dataSource;
            if (preview)
                nn["Preview"].Value = true;
            if (!string.IsNullOrEmpty(oldSelected))
                nn["OldSelected"].Value = oldSelected;

            RaiseEvent(
                "Magix.MetaForms.CreateControl",
                nn);

            if (nn.Contains("Control"))
            {
                System.Web.UI.Control ctrl = nn["Control"].Get<System.Web.UI.Control>();

                if (preview && ctrl is BaseWebControl)
                {
                    BaseWebControl ctr = ctrl as BaseWebControl;

                    object id = nn["_ID"].Value;

                    ctr.Click +=
                        delegate
                        {
                            Node t = new Node();
                            t["ID"].Value = id;

                            RaiseEvent(
                                "Magix.MetaForms.SetActiveEditingMetaFormWidget",
                                t);
                        };
                    ctr.Load +=
                        delegate
                        {
                            if (ctr.ClientID == oldSelected &&
                                !ctr.CssClass.Contains(" mux-wysiwyg-selected"))
                            {
                                AddSelectedCssClass(ctr);
                                ctr.ToolTip = "Drag and Drop me to position me absolutely [which is _not_ a generally good idea BTW]";
                            }
                        };
                }


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
                        if (node.Contains("Properties") &&
                            node["Properties"].Contains("Info"))
                        {
                            tmp["DataSource"].Value = GetObjectFromExpression(node["Properties"]["Info"].Get<string>(), dataSource);
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

                            CreateSingleControl(preview, idx, ctrl, dataSource, oldSelected);
                        }
                    }
                }

                // Making sure we're rendering the styles needed ...
                RenderStyles(ctrl as BaseWebControl, node);

                parent.Controls.Add(ctrl);
            }
        }

        /*
         * Helper for above ...
         */
        private void AddSelectedCssClass(BaseWebControl ctrl)
        {
            if (!ctrl.CssClass.Contains(" mux-wysiwyg-selected"))
                ctrl.CssClass += " mux-wysiwyg-selected";

            BaseWebControl idx = ctrl.Parent as BaseWebControl;
            while (idx != null && idx.ID != "ctrls")
            {
                if (!idx.CssClass.Contains(" mux-wysiwyg-selected"))
                    idx.CssClass += " mux-wysiwyg-selected";

                idx = idx.Parent as BaseWebControl;
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
                    int id = int.Parse(e.Params["ParentControl"].Value.ToString());
                    parent = f.Form.Find(
                        delegate(MetaForm.Node idx)
                        {
                            return idx.ID == id;
                        });
                }
                else
                    parent = f.Form;

                if (parent == null)
                    throw new ArgumentException("That parent doesn't exist");

                if (!e.Params.Contains("HasSurface") ||
                    !e.Params["HasSurface"].Get<bool>())
                    throw new ArgumentException("That control cannot have Child Controls. De-select it, and select another control, or select the form itself before you try to add controls to your Meta Form ...");

                int count = parent["Surface"].Children.Count;

                foreach (MetaForm.Node idx in parent["Surface"].Children)
                {
                    if (int.Parse(idx.Name.Substring(2)) >= count)
                        count = int.Parse(idx.Name.Substring(2)) + 1;
                }

                parent["Surface"]["c-" + count]["TypeName"].Value = e.Params["TypeName"].Get<string>();

                parent.Save();

                tr.Commit();

                e.Params["NewControlID"].Value = parent["Surface"]["c-" + count].ID;
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
                if (e.Params["PropertyName"].Get<string>() == "ID")
                {
                    // Need to signalize a re-render, and de-select need back to caller ...
                    e.Params["ReRender"].Value = true;
                }
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
        [ActiveEvent(Name = "Magix.MetaForms.ShowAllActionsAssociatedWithFormWidgetEvent")]
        protected void Magix_MetaForms_ShowAllActionsAssociatedWithFormWidgetEvent(object sender, ActiveEventArgs e)
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

        [ActiveEvent(Name = "Magix.MetaForms.ShowAllActionsAssociatedWithMainFormEvent")]
        protected void Magix_MetaForms_ShowAllActionsAssociatedWithMainFormEvent(object sender, ActiveEventArgs e)
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

            node["MetaFormID"].Value = e.Params["ID"].Value;
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

            string actionString = "";
            MetaForm.Node n = null;
            if (e.Params.Contains("MetaFormNodeID"))
            {
                n = MetaForm.Node.SelectByID(e.Params["MetaFormNodeID"].Get<int>());
            }
            else if (e.Params.Contains("MetaFormID"))
            {
                MetaForm f = MetaForm.SelectByID(e.Params["MetaFormID"].Get<int>());
                n = f.Form;
            }
            else
            {
                throw new ArgumentException("Sorry, I don't know how to retrieve those objects without either a MetaFormNodeID or a MetaFormID");
            }

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
                    e.Params["Objects"]["o-" + idxNo]["ID"].Value = n.ID + 
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

            // Mking sure we refresh our properties in our UI ...
            RaiseEvent("Magix.MetaForms.RefreshEditableMetaForm");
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
            if (e.Params.Contains("MetaFormNodeID"))
                node["ParentID"].Value = e.Params["MetaFormNodeID"].Value;
            else
            {
                MetaForm f = MetaForm.SelectByID(e.Params["MetaFormID"].Get<int>());
                node["ParentID"].Value = f.Form.ID;
            }
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
            node["ConfigureFiltersEvent"].Value = "Magix.MetaForms.ConfigureFilterForColumns";

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

        /**
         * Level2: Ads up Top=20 and nothing more before it forwards to
         * 'DBAdmin.Form.ConfigureFilterForColumn'
         */
        [ActiveEvent(Name = "Magix.MetaForms.ConfigureFilterForColumns")]
        protected void Magix_MetaForms_ConfigureFilterForColumns(object sender, ActiveEventArgs e)
        {
            e.Params["Top"].Value = 20;

            RaiseEvent(
                "DBAdmin.Form.ConfigureFilterForColumn",
                e.Params);
        }

        /**
         * Level2: Will append the given 'ID' Action into the given 'ParentPropertyName' Event name
         * and save the MetaForm.Node
         */
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
            }

            ActiveEvents.Instance.RaiseClearControls("child");
            RaiseEvent("Magix.MetaForms.RefreshEditableMetaForm");
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

            foreach (MetaForm idx in MetaForm.Select(Criteria.Sort("Created", false)))
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

        /**
         * Level2: Will make sure the widget becomes absolutized and stored as such. 
         * Expects ID of MetaForm.Node through 'ID' and 'Left' and 'Top' pixel numbers 
         * defining where on the local coordinate system they're supposed to be displayed
         */
        [ActiveEvent(Name = "Magix.MetaForms.AbsolutizeWidget")]
        protected void Magix_MetaForms_AbsolutizeWidget(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm.Node n = MetaForm.Node.SelectByID(e.Params["ID"].Get<int>());

                // The node stores style values exactly as they would be rendered out
                // while rendered, meaning no CAPitalization of names ...
                n["Properties"]["Style"]["left"].Value = e.Params["Left"].Value.ToString() + "px";
                n["Properties"]["Style"]["top"].Value = e.Params["Top"].Value.ToString() + "px";
                n["Properties"]["Style"]["position"].Value = "absolute";

                n.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Will delete the given 'ID' MetaForm.Node element from the MetaForm
         */
        [ActiveEvent(Name = "Magix.MetaForms.DeleteMetaFormWidgetFromForm")]
        protected void Magix_MetaForms_DeleteMetaFormWidgetFromForm(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm.Node n = MetaForm.Node.SelectByID(e.Params["ID"].Get<int>());

                n.Delete();

                tr.Commit();
            }

            // Signalizing to caller that he needs to refresh his view ...
            e.Params["Refresh"].Value = true;
        }

        /**
         * Level2: Will open up the Style Builder for creating Animations and Styles for your widget.
         * The Event Handler needs an 'ID' parameter which defines which MetaForm.Node object 
         * to actually modify the style of upon saving of your style
         */
        [ActiveEvent(Name = "Magix.MetaForms.OpenStyleBuilderForWidget")]
        protected void Magix_MetaForms_OpenStyleBuilderForWidget(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            RaiseEvent(
                "Magix.MetaForms.GetAnimations",
                node);

            MetaForm.Node nx = MetaForm.Node.SelectByID(e.Params["ID"].Get<int>());

            node["ID"].Value = e.Params["ID"].Value;
            node["TypeName"].Value = nx["TypeName"].Value;

            node["CSSClass"].Value =
                nx.Contains("Properties") && nx["Properties"].Contains("CssClass") ? 
                    nx["Properties"]["CssClass"].Value : 
                    "";

            if (nx.Contains("Properties"))
            {
                foreach (MetaForm.Node idx in nx["Properties"].Children)
                {
                    switch (idx.Name)
                    {
                        case "CssClass":
                        case "ID":
                            break;
                        default:
                            node["Properties"][idx.Name].Value = idx.Value;
                            break;
                    }
                }
            }

            if (nx.Contains("Properties") &&
                nx["Properties"].Contains("Style"))
            {
                foreach (MetaForm.Node idx in nx["Properties"]["Style"].Children)
                {
                    node["Properties"]["Style"][idx.Name].Value = idx.Value;
                }
            }

            node["Caption"].Value = "Magix Style Builder for Widget ...";
            node["Width"].Value = 24;
            node["Top"].Value = 20;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaForms.StyleBuilder",
                "child",
                node);
        }

        /**
         * Level2: Returns all existing Animations within 'Animations' as a 
         * list of 'Name'/'CSSClass' items
         */
        [ActiveEvent(Name = "Magix.MetaForms.GetAnimations")]
        protected void Magix_MetaForms_GetAnimations(object sender, ActiveEventArgs e)
        {
            string fullAnimationCssFilePath = Page.Server.MapPath("~/media/modules/animations.css");
            using (TextReader reader = new StreamReader(File.OpenRead(fullAnimationCssFilePath)))
            {
                string wholeContent = reader.ReadToEnd();
                wholeContent = wholeContent.Replace("\r\n", "\n");
                int idxNo = 0;
                foreach (string idx in wholeContent.Split(
                    new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (idx.StartsWith(".") &&
                        !idx.Contains(" ") &&
                        !idx.Contains(":"))
                    {
                        e.Params["Animations"]["a-" + idxNo]["Name"].Value = idx.Substring(1);
                        e.Params["Animations"]["a-" + idxNo]["CSSClass"].Value = idx.Trim('.').Trim(',');
                        idxNo += 1;
                    }
                }
            }
        }

        /**
         * Level2: Includes the animation.css file onto ever single first loading
         */
        [ActiveEvent(Name = "Magix.Core.InitialLoading")]
        protected void Magix_Core_InitialLoading(object sender, ActiveEventArgs e)
        {
            IncludeCssFile("media/modules/animations.css");
        }

        /**
         * Level2: Updates and save all the incoming 'Style' values into the MetaForms.Node given
         * through ID which should point to a Widget, with a 'Properties'/'Style' node
         */
        [ActiveEvent(Name = "Magix.MetaForms.SetWidgetStyles")]
        protected void Magix_MetaForms_SetWidgetStylesMagix_MetaForms_GetAnimations(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaForm.Node n = MetaForm.Node.SelectByID(e.Params["ID"].Get<int>());

                n["Properties"]["Style"].Children.Clear();

                foreach (Node idx in e.Params["Style"])
                {
                    n["Properties"]["Style"][idx.Name].Value = idx.Get<string>();
                }
                n["Properties"]["CssClass"].Value = e.Params["CssClass"].Get<string>();
                n.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Overridden to make sure we can clean out our Content 4++ when deletion of MetaForms
         * occurs
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaForm).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }
    }
}
