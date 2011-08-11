/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Magix.UX.Builder;
using Magix.UX.Widgets.Core;

namespace Magix.UX.Widgets
{
    /**
     * A DropDown list type of control, although it can also be set into a non-drop 
     * down mode. Basically multiple choices type of widget. To some extent, it 
     * overlaps the logical functionality of the RadioButton widget, although the
     * SelectList is more useful for cases where you have a massive number of choices,
     * like for instance choose one out of 300 different languages, while the RadioButton 
     * is more useful for cases where you have fewer choices, such as choose 'coffee, tea
     * or water'. Add up your choices by adding up ListItems inside of your SelectList.
     */
    [ParseChildren(true, "Items")]
    public class SelectList : BaseWebControlFormElement
    {
        private ListItemCollection _listItems;
        private string _selectedItemValue;

        /**
         * Raised whenever the selected value of your select list is changed.
         * You can also set the selected value in code through using for instance
         * the SelectedIndex property. Whenever a user changes the actively selected
         * item of your select list, this event will be raised.
         */
        public event EventHandler SelectedIndexChanged;

        public SelectList()
        {
            _listItems = new ListItemCollection(this);
        }

        /**
         * If this property is anything higher than 1, which is the default value,
         * the select list will no longer be a 'drop down' type of multiple
         * choices widget, but will show as a 'panel' showing all the choices and making
         * it possible to scroll, if needed, to see more items further down.
         * This property is the number of items that the select list will show
         * at one time. Anything higher than 1, will change its appearence completely
         * and make it into a non-drop-down type of widget.
         */
        public int Size
        {
            get { return ViewState["Size"] == null ? 1 : (int)ViewState["Size"]; }
            set
            {
                if (value != Size)
                    SetJsonGeneric("size", value.ToString());
                ViewState["Size"] = value;
            }
        }

        /**
         * A collection of all the items you have inside of your select list.
         * This will be automatically parse through your .ASPX syntax if you
         * declare items inside of the .ASPX file, or you can also programmatically 
         * add items in your codebehind file.
         */
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ListItemCollection Items
        {
            get
            {
                return _listItems;
            }
        }

        /**
         * Will return the currently selected item or set the currently selected item.
         * There are multiple duplicates of this property, like for instance the 
         * SelectedIndex property. The default SelectedItem will always be the
         * first (zero'th) element, regardless of which property you're using to
         * retrieve it.
         */
        public ListItem SelectedItem
        {
            get
            {
                if (_selectedItemValue == null)
                {
                    if (Items.Count == 0)
                        return null;
                    return Items[0];
                }
                return Items.Find(
                    delegate(ListItem idx)
                    {
                        return idx.Value == _selectedItemValue;
                    });
            }
            set
            {
                _selectedItemValue = value.Value;
                if (IsTrackingViewState)
                {
                    this.SetJsonValue("Value", value.Value);
                }
                SelectedIndex = Items.IndexOf(value);
            }
        }

        /**
         * Will return the index of the currently selected item or set the currently 
         * selected item based on its index.
         * There are multiple duplicates of this property, like for instance the 
         * SelectedItem property. The default SelectedItem will always be the
         * first (zero'th) element, regardless of which property you're using to
         * retrieve it. Meaning that the default value of this property will always
         * be '0'.
         */
        public int SelectedIndex
        {
            get
            {
                if (Items == null || Items.Count == 0)
                    return -1;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
                        return i;
                }
                return 0;
            }
            set
            {
                if (value == SelectedIndex)
                    return;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == value)
                        _selectedItemValue = Items[i].Value;
                }
                if (IsTrackingViewState)
                {
                    SetJsonGeneric("value", _selectedItemValue);
                }
            }
        }

        protected override void TrackViewState()
        {
            Items.TrackViewState();
            base.TrackViewState();
        }

        protected override object SaveViewState()
        {
            object[] retVal = new object[2];
            retVal[0] = Items.SaveViewState();
            ViewState["_selectedItemValue"] = _selectedItemValue;
            retVal[1] = base.SaveViewState();
            return retVal;
        }

        protected override void LoadViewState(object savedState)
        {
            object[] content = savedState as object[];
            Items.LoadViewState(content[0]);
            base.LoadViewState(content[1]);
            if (_selectedItemValue == null)
                _selectedItemValue = (string)ViewState["_selectedItemValue"];
        }

        protected override void SetValue()
        {
            string newVal = Page.Request.Params[ClientID];
            if (newVal != null)
                _selectedItemValue = newVal;
        }

        public override void RaiseEvent(string name)
        {
            switch (name)
            {
                case "change":
                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged(this, new EventArgs());
                    break;
                default:
                    base.RaiseEvent(name);
                    break;
            }
        }

        protected override string GetEventsRegisterScript()
        {
            string evts = base.GetEventsRegisterScript();
            if (SelectedIndexChanged != null)
            {
                if (evts.Length != 0)
                    evts += ",";
                evts += "['change']";
            }
			return evts;
        }

        protected override void RenderMuxControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("select"))
            {
                AddAttributes(el);
                foreach (ListItem idx in Items)
                {
                    using (Element l = builder.CreateElement("option"))
                    {
                        l.AddAttribute("value", idx.Value);
                        if (!idx.Enabled)
                            l.AddAttribute("disabled", "disabled");
                        if (idx.Selected)
                            l.AddAttribute("selected", "selected");
                        l.Write(idx.Text);
                    }
                }
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("name", ClientID);
            if (Size != -1)
                el.AddAttribute("size", Size.ToString());
            base.AddAttributes(el);
        }
    }
}
