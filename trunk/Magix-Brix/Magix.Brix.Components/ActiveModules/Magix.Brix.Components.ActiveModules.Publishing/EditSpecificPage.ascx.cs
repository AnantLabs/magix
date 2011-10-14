/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using System.Web;

namespace Magix.Brix.Components.ActiveModules.Publishing
{
    /**
     * Level2: Allows for editing of one single specific WebPage in the system. Contains most of the UI
     * which you're probably daily using while adding and creating new pages and such
     */
    [ActiveModule]
    public class EditSpecificPage : ActiveModule
    {
        protected InPlaceEdit header;
        protected InPlaceEdit url;
        protected SelectList sel;
        protected Panel parts;
        protected Panel roles;

        public override void InitialLoading(Node node)
        {
            base.InitialLoading(node);

            Load +=
                delegate
                {
                    header.Text = node["Header"].Get<string>();
                    url.Text = node["URL"].Get<string>();
                    foreach (Node idx in node["Templates"])
                    {
                        ListItem i = new ListItem(idx["Name"].Get<string>(), idx["ID"].Get<int>().ToString());
                        if (idx["ID"].Get<int>() == DataSource["TemplateID"].Get<int>())
                            i.Selected = true;
                        sel.Items.Add(i);
                    }
                };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
            {
                DataBindRoles();
                DataBindWebParts();
            }
        }

        private void DataBindRoles()
        {
            foreach (Node idx in DataSource["Roles"])
            {
                Button b = new Button();
                b.Text = idx["Name"].Get<string>();
                b.Info = idx["ID"].Get<int>().ToString();
                b.CssClass = "span-6";
                b.Click +=
                    delegate
                    {
                        Node node = new Node();

                        node["ID"].Value = DataSource["ID"].Get<int>();
                        node["RoleID"].Value = int.Parse(b.Info);

                        bool createAccess = false;

                        if (b.CssClass.Contains(" unchecked") || b.CssClass.Contains(" neutral"))
                            createAccess = true;

                        node["Access"].Value = createAccess;

                        RaiseSafeEvent(
                            "Magix.Publishing.ChangePageAccess",
                            node);

                        // Signalizing that Pages has been changed, in case other modules are
                        // dependent upon knowing ...
                        RaiseEvent("Magix.Publishing.PageWasUpdated");
                    };
                b.ToolTip = idx["Name"].Get<string>();

                if (DataSource.Contains("ActiveRoles"))
                {
                    foreach (Node idxA in DataSource["ActiveRoles"])
                    {
                        if (idx["ID"].Get<int>() == idxA["ID"].Get<int>())
                        {
                            if (!idxA.Contains("HasAccess"))
                            {
                                b.CssClass += " neutral";
                                b.ToolTip = "Can implicitly access object";
                            }
                            else if (idxA["HasAccess"].Get<bool>())
                            {
                                if (idxA["Explicitly"].Get<bool>())
                                {
                                    b.ToolTip = "Can Explicitly Access the Object, click this button to Remove Explicit Access";
                                    b.CssClass += " checked-expl";
                                }
                                else
                                {
                                    b.ToolTip = "Can Implicitly Access the Object, click this button to Create Explicit Access";
                                    b.CssClass += " checked";
                                }
                            }
                            else
                            {
                                b.ToolTip = "Implicitly have no Access to the Object, click here to Explicitly Grant Access";
                                b.CssClass += " unchecked";
                            }
                            break;
                        }
                    }
                }
                else
                {
                    b.CssClass += " neutral";
                    b.ToolTip = "Implicitly have Access to the Object, click here to Explicitly grant it access";
                }
                roles.Controls.Add(b);
            }
        }

        private void DataBindWebParts()
        {
            int current = int.Parse(sel.SelectedItem.Value);
            foreach (Node idx in DataSource["Templates"])
            {
                if (idx.Name == "t-" + current)
                {
                    foreach (Node idxC in idx["Containers"])
                    {
                        int width = idxC["Width"].Get<int>();
                        int height = idxC["Height"].Get<int>();
                        bool last = idxC["Last"].Get<bool>();
                        bool overflow = idxC["Overflow"].Get<bool>();
                        string name = idxC["Name"].Get<string>() ?? "[Unknown]";
                        int id = idxC["ID"].Get<int>();
                        int padding = idxC["Padding"].Get<int>();
                        int push = idxC["Push"].Get<int>();
                        int top = idxC["Top"].Get<int>();
                        int bottomMargin = idxC["BottomMargin"].Get<int>();
                        string moduleName = idxC["ModuleName"].Get<string>();

                        // Creating Window ...
                        Window w = new Window();
                        w.ToolTip = string.Format("Module is of type '{0}'", moduleName);
                        w.CssClass += " mux-shaded mux-rounded";
                        if (width != 0)
                            w.CssClass += " span-" + width;
                        if (height != 0)
                            w.CssClass += " height-" + height;
                        if (last)
                            w.CssClass += " last";
                        if (overflow)
                            w.CssClass += " mux-overflow-design";
                        w.Caption = name;
                        w.Info = id.ToString();
                        if (padding != 0)
                            w.CssClass += " right-" + padding;
                        if (push != 0)
                            w.CssClass += " push-" + push;
                        if (top != 0)
                            w.CssClass += " down-" + top;
                        if (bottomMargin != 0)
                            w.CssClass += " bottom-" + bottomMargin;

                        // TODO: Should we allow dragging and dropping, 'override positioning' for pages ...?
                        w.Draggable = false;
                        w.Closable = false;

                        // Creating InPlaceEdits [or other types of controls] for every Setting we've got ...
                        if (DataSource.Contains("ObjectTemplates"))
                        {
                            foreach (Node idxT in DataSource["ObjectTemplates"])
                            {
                                if (idxT.Contains("i-" + id))
                                {
                                    foreach (Node idxII in idxT["i-" + id])
                                    {
                                        string propName = idxII.Name;
                                        string value = idxII.Get<string>();
                                        string editor = idxII["Editor"].Get<string>();
                                        string editorEvent = idxII["EditorEvent"].Get<string>();
                                        int webPartId = idxII["ID"].Get<int>();
                                        if (!string.IsNullOrEmpty(editorEvent))
                                        {
                                            Node node = new Node();
                                            node["WebPartID"].Value = webPartId;
                                            node["Value"].Value = value;

                                            RaiseEvent(
                                                editorEvent,
                                                node);
                                            w.Content.Controls.Add(node["Control"].Value as Control);
                                        }
                                        else if (!string.IsNullOrEmpty(editor))
                                        {
                                            Label ed = new Label();
                                            ed.ToolTip =
                                                string.Format("Click to edit '{0}'",
                                                    propName);
                                            ed.Text = value;
                                            ed.Tag = "div";
                                            ed.CssClass += "magix-publishing-wysiwyg";
                                            ed.Info = id.ToString() + "|" + propName + "|" + editor;
                                            ed.Click +=
                                                delegate(object sender2, EventArgs e2)
                                                {
                                                    Label ed2 = sender2 as Label;
                                                    int id2 = int.Parse(ed2.Info.Split('|')[0]);
                                                    string propName2 = ed2.Info.Split('|')[1];

                                                    Node tx = new Node();

                                                    tx["SaveEvent"].Value = "Magix.Publishing.ChangeWebPartSetting";
                                                    tx["SaveEvent"]["WebPartID"].Value = webPartId;
                                                    tx["Text"].Value = ed2.Text;
                                                    tx["Width"].Value = 18;
                                                    tx["Height"].Value = 30;
                                                    tx["Last"].Value = true;
                                                    tx["Padding"].Value = 6;
                                                    tx["Push"].Value = 0;
                                                    tx["Top"].Value = 0;
                                                    tx["BottomMargin"].Value = 15;

                                                    ActiveEvents.Instance.RaiseLoadControl(
                                                        ed2.Info.Split('|')[2],
                                                        "content5",
                                                        tx);
                                                };
                                            w.Content.Controls.Add(ed);
                                        }
                                        else
                                        {
                                            CreateInPlaceEdit(id, w, propName, value, webPartId);
                                        }
                                    }
                                }
                            }
                        }

                        parts.Controls.Add(w);
                    }
                }
            }
        }

        private void CreateInPlaceEdit(int id, Window w, string propName, string value, int webPartId)
        {
            InPlaceEdit ed = new InPlaceEdit();
            ed.CssClass = "span-5 mux-in-place-edit";
            ed.Style[Styles.position] = "relative";
            ed.Style[Styles.height] = "18px";
            ed.ToolTip = propName;
            ed.Text = value;
            ed.Info = id.ToString() + "|" + propName;
            ed.TextChanged +=
                delegate(object sender2, EventArgs e2)
                {
                    InPlaceEdit ed2 = sender2 as InPlaceEdit;
                    int id2 = int.Parse(ed2.Info.Split('|')[0]);
                    string propName2 = ed2.Info.Split('|')[1];

                    Node tx = new Node();

                    tx["WebPartID"].Value = webPartId;
                    tx["Value"].Value = ed2.Text;

                    RaiseSafeEvent(
                        "Magix.Publishing.ChangeWebPartSetting",
                        tx);
                };
            w.Content.Controls.Add(ed);
        }

        protected void header_TextChanged(object sende, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["Text"].Value = header.Text;

            RaiseSafeEvent(
                "Magix.Publishing.ChangePageProperty",
                node);
        }

        protected void url_TextChanged(object sende, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["URL"].Value = url.Text;

            RaiseSafeEvent(
                "Magix.Publishing.ChangePageProperty",
                node);

            url.Text = node["URL"].Get<string>();
        }

        protected void createChild_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();

            RaiseEvent(
                "Magix.Publishing.CreateChildWebPage",
                node);
        }

        /**
         * Level2: Needs to be handled here, in case it was 'this page'
         */
        [ActiveEvent(Name = "Magix.Publishing.PageWasDeleted")]
        protected void Magix_Publishing_PageWasDeleted(object sender, ActiveEventArgs e)
        {
            if (e.Params["ID"].Get<int>() == DataSource["ID"].Get<int>())
            {
                ActiveEvents.Instance.RaiseClearControls(Parent.ID);
            }
        }

        protected void delete_Click(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();

            RaiseSafeEvent(
                "Magix.Publishing.DeletePageObject",
                node);
        }

        /**
         * Page was somehow updated, and we need to reload the currently editing page
         */
        [ActiveEvent(Name = "Magix.Publishing.PageWasUpdated")]
        protected void Magix_Publishing_PageWasUpdated(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Value;

            RaiseSafeEvent(
                "Magix.Publishing.EditSpecificPage",
                node);
        }

        protected void sel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Node node = new Node();

            node["ID"].Value = DataSource["ID"].Get<int>();
            node["PageTemplateID"].Value = int.Parse(sel.SelectedItem.Value);

            RaiseSafeEvent(
                "Magix.Publishing.ChangePageProperty",
                node);

            foreach (ListItem idx in sel.Items)
            {
                if (idx.Value == node["PageTemplateID"].Get<int>().ToString())
                {
                    idx.Selected = true;
                    break;
                }
            }

            // Signalizing that Pages has been changed, in case other modules are
            // dependent upon knowing ...
            RaiseEvent("Magix.Publishing.PageWasUpdated");
        }
    }
}
