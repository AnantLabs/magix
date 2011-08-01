/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaViews;
using Magix.Brix.Data;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using System.Collections.Generic;
using Magix.Brix.Components.ActiveTypes.MetaTypes;

namespace Magix.Brix.Components.ActiveControllers.MetaViews
{
    [ActiveController]
    public class MetaViewController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Caption"].Value = "Meta Views ...";
            e.Params["Items"]["MetaType"]["Items"]["Views"]["Event"]["Name"].Value = "Magix.MetaView.ViewMetaViews";
        }

        [ActiveEvent(Name = "Magix.MetaView.ViewMetaViews")]
        protected void Magix_MetaView_ViewMetaViews(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "edit-views";

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 6;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["TypeName"]["MaxLength"].Value = 50;
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 3;

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaView.EditMetaView";
            node["CreateEventName"].Value = "Magix.MetaView.CreateMetaView";
            node["DeleteColumnEvent"].Value = "Magix.MetaView.DeleteMetaView";

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.Meta.GetCopyMetaViewTemplateColumn";

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.GetTemplateColumnSelectView")]
        protected void Magix_MetaView_GetTemplateColumnSelectView(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["Params"]["ID"].Value = e.Params["ID"].Value;
                    tx["Params"]["PropertyName"].Value = "Magix.Brix.Components.ActiveModules.MetaView.MetaView_SingleViewName";
                    tx["Params"]["PotID"].Value = e.Params["PotID"].Value;
                    tx["Text"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.SavePageObjectIDSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a MetaView ...", "");
            ls.Items.Add(item);

            foreach (MetaView idx in MetaView.Select())
            {
                ListItem it = new ListItem(idx.Name, idx.TypeName + idx.Name.ToString());
                if (idx.Name == e.Params["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.GetTemplateColumnSelectViewMultiple")]
        protected void Magix_MetaView_GetTemplateColumnSelectViewMultiple(object sender, ActiveEventArgs e)
        {
            SelectList ls = new SelectList();
            e.Params["Control"].Value = ls;

            ls.CssClass = "span-5";
            ls.Style[Styles.display] = "block";

            ls.SelectedIndexChanged +=
                delegate
                {
                    Node tx = new Node();

                    tx["Params"]["ID"].Value = e.Params["ID"].Value;
                    tx["Params"]["PropertyName"].Value = "Magix.Brix.Components.ActiveModules.MetaView.MetaView_MultipleViewName";
                    tx["Params"]["PotID"].Value = e.Params["PotID"].Value;
                    tx["Text"].Value = ls.SelectedItem.Text;

                    RaiseEvent(
                        "Magix.Publishing.SavePageObjectIDSetting",
                        tx);
                };

            ListItem item = new ListItem("Select a MetaView ...", "");
            ls.Items.Add(item);

            foreach (MetaView idx in MetaView.Select())
            {
                ListItem it = new ListItem(idx.Name, idx.TypeName + idx.Name.ToString());
                if (idx.Name == e.Params["Value"].Get<string>())
                    it.Selected = true;
                ls.Items.Add(it);
            }
        }

        [ActiveEvent(Name = "Magix.Meta.GetCopyMetaViewTemplateColumn")]
        protected void Magix_Meta_GetCopyMetaViewTemplateColumn(object sender, ActiveEventArgs e)
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
                        "Magix.Meta.CopyMetaView",
                        node);
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        [ActiveEvent(Name = "Magix.Meta.CopyMetaView")]
        protected void Magix_Meta_CopyMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView a = MetaView.SelectByID(e.Params["ID"].Get<int>());
                MetaView clone = a.Copy();

                clone.Save();

                tr.Commit();

                Node n = new Node();

                n["FullTypeName"].Value = typeof(MetaView).FullName;
                n["ID"].Value = clone.ID;

                RaiseEvent(
                    "DBAdmin.Grid.SetActiveRow",
                    n);

                n = new Node();
                n["FullTypeName"].Value = typeof(MetaView).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    n);

                n = new Node();
                n["ID"].Value = clone.ID;

                RaiseEvent(
                    "Magix.MetaView.EditMetaView",
                    n);
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.DeleteMetaView")]
        protected void Magix_MetaView_DeleteMetaView(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);
            Node node = e.Params;
            if (node == null)
            {
                node = new Node();
                node["ForcedSize"]["width"].Value = 550;
                node["WindowCssClass"].Value =
                    "mux-shaded mux-rounded push-5 down-2";
            }
            node["Caption"].Value = @"
Please confirm deletion of " + typeName + " with ID of " + id;
            node["Text"].Value = @"
<p>Are you sure you wish to delete this View? 
This View might be in use in several forms or other parts of your system. 
Deleting it may break these parts.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.MetaView.DeleteMetaView-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.DeleteMetaView-Confirmed")]
        protected void Magix_MetaView_DeleteMetaView_Confirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
            {
                // In case it's the one being edited ...
                Node n = new Node();

                n["Position"].Value = "content4";

                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ClearControls",
                    n);

                RaiseEvent(
                    "DBAdmin.Common.ComplexInstanceDeletedConfirmed",
                    e.Params);
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.CreateMetaView")]
        protected void Magix_MetaView_CreateMetaView(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView view = new MetaView();
                view.Name = "Default name";

                view.Save();

                tr.Commit();

                Node node = new Node();
                node["ID"].Value = view.ID;
                RaiseEvent(
                    "Magix.MetaView.EditMetaView",
                    node);
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.EditMetaView")]
        protected void Magix_MetaView_EditMetaView(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("content6");

            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            // MetaView Single View Editing ...
            node["FullTypeName"].Value = typeof(MetaView).FullName;
            node["ID"].Value = m.ID;

            // First filtering OUT columns ...!
            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["Properties"].Value = true;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["Name"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type Name";
            node["Type"]["Properties"]["Properties"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Properties"]["Header"].Value = "Props";

            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["Padding"].Value = 6;
            node["Container"].Value = "content4";
            node["MarginBottom"].Value = 10;

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);

            // Properties ...
            node = new Node();

            node["Width"].Value = 20;
            node["Padding"].Value = 4;
            node["Last"].Value = true;
            node["MarginBottom"].Value = 30;
            node["PullTop"].Value = 9;
            node["Container"].Value = "content5";

            node["PropertyName"].Value = "Properties";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(MetaView).FullName;

            node["WhiteListColumns"]["Name"].Value = true;
            node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["ReadOnly"].Value = true;
            node["WhiteListColumns"]["ReadOnly"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Description"].Value = true;
            node["WhiteListColumns"]["Description"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Action"].Value = true;
            node["WhiteListColumns"]["Action"]["ForcedWidth"].Value = 7;

            node["Type"]["Properties"]["Name"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["ReadOnly"]["Header"].Value = "Read On.";
            node["Type"]["Properties"]["ReadOnly"]["TemplateColumnEvent"].Value = "Magix.DataPlugins.GetTemplateColumns.CheckBox";
            node["Type"]["Properties"]["Description"]["Header"].Value = "Desc.";
            node["Type"]["Properties"]["Description"]["MaxLength"].Value = 50;
            node["Type"]["Properties"]["Action"]["Header"].Value = "Action";
            node["Type"]["Properties"]["Action"]["TemplateColumnEvent"].Value = "Magix.MetaView.GetMetaViewActionTemplateColumn";

            node["ID"].Value = e.Params["ID"].Value;
            node["NoIdColumn"].Value = true;
            node["ReUseNode"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);

            // Wysiwyg button ...
            node = new Node();

            node["Text"].Value = "Preview ...";
            node["ButtonCssClass"].Value = "span-4";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.MetaView.LoadWysiwyg";
            node["Event"]["ID"].Value = m.ID;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content5",
                node);
        }

        [ActiveEvent(Name = "Magix.DataPlugins.GetTemplateColumns.CheckBox")]
        protected void Magix_DataPlugins_GetTemplateColumns_CheckBox(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();

            Panel pnl = new Panel();

            CheckBox ch = new CheckBox();
            ch.Style[Styles.floating] = "left";
            ch.Style[Styles.width] = "15px";
            ch.Style[Styles.display] = "block";
            ch.Checked = bool.Parse(e.Params["Value"].Value.ToString());
            ch.CheckedChanged +=
                delegate
                {
                    Node node = new Node();
                    node["ID"].Value = e.Params["ID"].Value;
                    node["FullTypeName"].Value = e.Params["FullTypeName"].Value;
                    node["NewValue"].Value = ch.Checked.ToString();
                    node["PropertyName"].Value = e.Params["Name"].Value;

                    RaiseEvent(
                        "DBAdmin.Data.ChangeSimplePropertyValue",
                        node);
                };
            pnl.Controls.Add(ch);

            Label l = new Label();
            l.Text = "&nbsp;";
            l.CssClass += "span-2";
            l.Tag = "label";
            l.Load +=
                delegate
                {
                    l.For = ch.ClientID;
                };
            pnl.Controls.Add(l);

            e.Params["Control"].Value = pnl;
        }

        [ActiveEvent(Name = "Magix.MetaView.GetMetaViewActionTemplateColumn")]
        protected void Magix_MetaView_GetMetaViewActionTemplateColumn(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();

            MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ID"].Get<int>());

            Panel pnl = new Panel();
            pnl.CssClass = "action-wrapper";
            pnl.Click +=
                delegate
                {
                    pnl.CssClass += " action-wrapper-hover";

                    Node node = new Node();
                    node["ID"].Value = p.ID;

                    RaiseEvent(
                        "Magix.MetaView.AppendAction",
                        node);

                    node = new Node();
                    node["ID"].Value = p.ID;
                    node["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

                    RaiseEvent(
                        "DBAdmin.Grid.SetActiveRow",
                        node);
                };

            if (!string.IsNullOrEmpty(p.Action))
            {
                pnl.CssClass += " has-action";
            }
            else
            {
                pnl.CssClass += " has-no-action";
            }

            Panel grow = new Panel();
            grow.CssClass = "grower";
            grow.Click +=
                delegate
                {
                    pnl.CssClass = pnl.CssClass.Replace(" action-wrapper-hover", "");
                };

            string[] actions = 
                (p.Action ?? "")
                .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idxA in actions)
            {
                string actionName = idxA;
                if (actionName.Contains("("))
                    actionName = actionName.Substring(0, actionName.IndexOf('('));

                LinkButton btn = new LinkButton();
                btn.Text = "X";
                btn.CssClass = "clear-left span-1 delete-action";
                btn.Click +=
                    delegate
                    {
                        ActiveEvents.Instance.RaiseClearControls("content6");
                        using (Transaction tr = Adapter.Instance.BeginTransaction())
                        {
                            string na = p.Action.Substring(0, p.Action.IndexOf(actionName));
                            na += p.Action.Substring(p.Action.IndexOf(actionName) + actionName.Length);
                            p.Action = na.Replace("||", "|").Trim('|').Trim();
                            p.Save();

                            tr.Commit();
                        }

                        Node n = new Node();
                        n["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

                        RaiseEvent(
                            "Magix.Core.UpdateGrids",
                            n);

                    };
                grow.Controls.Add(btn);

                Label l = new Label();
                l.Text = actionName;
                l.Tag = "div";
                l.CssClass = "span-3 last";
                grow.Controls.Add(l);
            }

            Button bt = new Button();
            bt.Text = "Close";
            bt.CssClass = "bottom-right span-3";
            bt.Click +=
                delegate
                {
                    pnl.CssClass = pnl.CssClass.Replace(" action-wrapper-hover", "");
                    ActiveEvents.Instance.RaiseClearControls("content6");
                };
            grow.Controls.Add(bt);

            pnl.Controls.Add(grow);

            e.Params["Control"].Value = pnl;
        }

        [ActiveEvent(Name = "Magix.MetaView.AppendAction")]
        protected void Magix_MetaView_AppendAction(object sender, ActiveEventArgs e)
        {
            e.Params["SelectEvent"].Value = "Magix.MetaView.ActionWasChosenForAppending";
            e.Params["ParentID"].Value = e.Params["ID"].Value;
            RaiseEvent(
                "Magix.MetaActions.SearchActions",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.MetaView.ActionWasChosenForAppending")]
        protected void Magix_MetaView_ActionWasChosenForAppending(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("content6");

            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaView.MetaViewProperty p = MetaView.MetaViewProperty.SelectByID(e.Params["ParentID"].Get<int>());
                if (!string.IsNullOrEmpty(p.Action))
                    p.Action += "|";
                p.Action += e.Params["ActionName"].Get<string>();
                p.Save();

                tr.Commit();
            }

            Node n = new Node();
            n["FullTypeName"].Value = typeof(MetaView.MetaViewProperty).FullName;

            RaiseEvent(
                "Magix.Core.UpdateGrids",
                n);
        }

        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaView).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        [ActiveEvent(Name = "Magix.MetaType.RaiseViewMetaTypeFromMultipleView")]
        protected void Magix_MetaType_RaiseViewMetaTypeFromMultipleView(object sender, ActiveEventArgs e)
        {
            MetaView view = MetaView.SelectFirst(
                Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            e.Params["IsDelete"].Value = false;
            e.Params["NoIdColumn"].Value = true;

            foreach (MetaView.MetaViewProperty idx in view.Properties)
            {
                if (idx.Name == ":Delete")
                {
                    e.Params["IsDelete"].Value = true;
                    continue;
                }
                else if (idx.Name == ":Edit")
                {
                    e.Params["NoIdColumn"].Value = false;
                    continue;
                }

                // TODO: Somehow load BUTTONS for every 'Action Item' in the View, and append them
                // at the bottom of the grid, or something ...

                if (!string.IsNullOrEmpty(idx.Action))
                    continue; // Button thing ...

                string name = idx.Name;

                if (name.Contains(":"))
                {
                    string[] splits = name.Split(':');
                    name = splits[splits.Length - 1];
                    e.Params["Type"]["Properties"][name]["TemplateColumnEvent"].Value = 
                        "Magix.MetaView.MultiViewTemplateColumn";
                }

                e.Params["WhiteListColumns"][name].Value = true;
                e.Params["Type"]["Properties"][name]["ReadOnly"].Value = idx.ReadOnly;
                e.Params["Type"]["Properties"][name]["Header"].Value = name;
            }

            e.Params["MetaViewTypeName"].Value = view.TypeName;

            Page.Session["Magix.MetaView.EditingView"] = view;

            RaiseEvent(
                "Magix.MetaType.ViewMetaTypeFromTemplate",
                e.Params);
        }

        [ActiveEvent(Name = "Magix.MetaView.MultiViewTemplateColumn")]
        protected void Magix_MetaView_MultiViewTemplateColumn(object sender, ActiveEventArgs e)
        {
            MetaView v = Page.Session["Magix.MetaView.EditingView"] as MetaView;
            MetaView.MetaViewProperty p = v.Properties.Find(
                delegate(MetaView.MetaViewProperty idx)
                {
                    return idx.Name.Contains(":" + e.Params["Name"].Get<string>());
                });

            string typeOfControl = p.Name.Split(':')[0];

            int id = e.Params["ID"].Get<int>();

            switch (typeOfControl)
            {
                case "select":
                    {
                        SelectList ls = new SelectList();

                        ls.CssClass = "span-2 gridSelect";
                        ls.Style[Styles.display] = "block";

                        string typeProperty = p.Name.Split(':')[1];
                        string type = typeProperty.Split('.')[0];
                        string propertyName = typeProperty.Split('.')[1];
                        string gridPropertyName = p.Name.Split(':')[2];

                        ls.SelectedIndexChanged +=
                            delegate
                            {
                                using (Transaction tr = Adapter.Instance.BeginTransaction())
                                {
                                    MetaObject o = MetaObject.SelectByID(id);
                                    MetaObject.Value val = o.Values.Find(
                                        delegate(MetaObject.Value idxI)
                                        {
                                            return idxI.Name == gridPropertyName;
                                        });
                                    val.Val = ls.SelectedItem.Value;
                                    val.Save();

                                    tr.Commit();
                                }
                            };

                        ListItem i = new ListItem();
                        i.Value = "";
                        i.Text = "Please Select ...";
                        ls.Items.Add(i);

                        foreach (MetaObject idx in MetaObject.Select(Criteria.Eq("TypeName", type)))
                        {
                            ListItem it = new ListItem();
                            MetaObject.Value val = idx.Values.Find(
                                delegate(MetaObject.Value idxI)
                                {
                                    return idxI.Name == propertyName;
                                });
                            it.Text = val.Val;
                            it.Value = val.Val;
                            if (val.Val == e.Params["Value"].Get<string>())
                                it.Selected = true;
                            ls.Items.Add(it);
                        }

                        e.Params["Control"].Value = ls;
                    } break;
                case "date":
                    {
                        string gridPropertyName = p.Name.Split(':')[1];

                        MetaObject o = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                        string propertyName = e.Params["Name"].Get<string>();
                        MetaObject.Value val = o.Values.Find(
                            delegate(MetaObject.Value idx)
                            {
                                return idx.Name == propertyName;
                            });

                        Panel panel = new Panel();
                        panel.ID = "pnl" + id;
                        panel.CssClass = "calendar-wrapper";

                        Calendar c = new Calendar();
                        c.ID = "cal" + id;
                        c.CssClass += " mux-shaded mux-rounded";
                        if (val != null && 
                            !string.IsNullOrEmpty(val.Val))
                        {
                            c.Value = DateTime.ParseExact(val.Val, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        c.Style[Styles.display] = "none";
                        c.Style[Styles.position] = "absolute";
                        c.Style[Styles.top] = "0";
                        c.Style[Styles.left] = "0";
                        c.Style[Styles.zIndex] = "100";
                        panel.Controls.Add(c);

                        LinkButton but = new LinkButton();

                        c.DateSelected +=
                            delegate(object sender2, EventArgs e2)
                            {
                                new EffectFadeOut(c, 250).Render();
                                using (Transaction tr = Adapter.Instance.BeginTransaction())
                                {
                                    if (val == null)
                                    {
                                        val = new MetaObject.Value();
                                        val.Name = propertyName;
                                        o.Values.Add(val);
                                        o.Save();
                                    }
                                    val.Val = c.Value.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

                                    val.Save();

                                    tr.Commit();

                                    but.Text = DateTime.ParseExact(
                                        val.Val, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture)
                                        .ToString("ddd d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                            };

                        but.Text = "???";
                        if (val != null &&
                            !string.IsNullOrEmpty(val.Val))
                            but.Text = DateTime.ParseExact(
                                val.Val, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture)
                                .ToString("ddd d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        but.ID = "but" + id;
                        but.Click +=
                            delegate
                            {
                                new EffectFadeIn(c, 250).Render();
                            };
                        panel.Controls.Add(but);

                        e.Params["Control"].Value = panel;
                    } break;
                case "choice":
                    {
                        string typeProperty = p.Name.Split(':')[1];
                        string type = typeProperty.Split('.')[0];
                        string propertyName = typeProperty.Split('.')[1].Split(',')[0];
                        string propertyCss = typeProperty.Split('.')[1].Split(',')[1];
                        string gridPropertyName = p.Name.Split(':')[2];

                        LinkButton b = new LinkButton();

                        MetaObject o = MetaObject.SelectByID(id);
                        MetaObject.Value val = o.Values.Find(
                            delegate(MetaObject.Value idxI)
                            {
                                return idxI.Name == gridPropertyName;
                            });
                        MetaObject o4 = null;
                        string cssClass = "status-unknown";
                        if (val != null)
                        {
                            foreach (MetaObject idx in MetaObject.Select(Criteria.Eq("TypeName", type)))
                            {
                                MetaObject.Value val2 = idx.Values.Find(
                                    delegate(MetaObject.Value idxI)
                                    {
                                        return idxI.Name == propertyName && idxI.Val == val.Val;
                                    });
                                if (val2 != null)
                                {
                                    o4 = idx;
                                    break;
                                }
                            }
                            MetaObject.Value propCss = o4.Values.Find(
                                delegate(MetaObject.Value idxI)
                                {
                                    return idxI.Name == propertyCss;
                                });
                            cssClass = propCss.Val;
                        }

                        b.Text = "&nbsp;";
                        b.CssClass = "multi-choice " + cssClass;
                        string choiceVal = val == null ? "" : val.Val;

                        b.Text = choiceVal;

                        b.Click +=
                            delegate
                            {
                                MetaObject next = null;
                                bool found = false;
                                foreach (MetaObject idx in MetaObject.Select(Criteria.Eq("TypeName", type)))
                                {
                                    if (found)
                                    {
                                        next = idx;
                                        break;
                                    }
                                    MetaObject.Value val2 = idx.Values.Find(
                                        delegate(MetaObject.Value idxI)
                                        {
                                            return idxI.Name == propertyName && 
                                                choiceVal == idxI.Val;
                                        });
                                    if (val2 != null && val2.Val == choiceVal)
                                        found = true;
                                }
                                if (next == null)
                                    next = MetaObject.SelectFirst(Criteria.Eq("TypeName", type));
                                b.CssClass = "multi-choice " + next.Values.Find(
                                    delegate(MetaObject.Value idxI)
                                    {
                                        return idxI.Name == propertyCss;
                                    }).Val;
                                b.Text = next.Values.Find(
                                    delegate(MetaObject.Value idxI)
                                    {
                                        return idxI.Name == propertyName;
                                    }).Val;
                                using (Transaction tr = Adapter.Instance.BeginTransaction())
                                {
                                    MetaObject o2 = MetaObject.SelectByID(id);
                                    MetaObject.Value val3 = o.Values.Find(
                                        delegate(MetaObject.Value idxI)
                                        {
                                            return idxI.Name == gridPropertyName;
                                        });
                                    if (val3 == null)
                                    {
                                        val3 = new MetaObject.Value();
                                        val3.Name = gridPropertyName;
                                        o2.Values.Add(val3);
                                        o2.Save();
                                    }
                                    val3.Val = next.Values.Find(
                                        delegate(MetaObject.Value idxI)
                                        {
                                            return idxI.Name == propertyName;
                                        }).Val;
                                    val3.Save();

                                    tr.Commit();
                                }
                            };

                        e.Params["Control"].Value = b;
                    } break;
            }
        }

        [ActiveEvent(Name = "Magix.MetaView.LoadWysiwyg")]
        protected void Magix_MetaView_LoadWysiwyg(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            node["Width"].Value = 24;
            node["Last"].Value = true;
            node["Top"].Value = 2;
            node["MarginBottom"].Value = 20;
            node["PullTop"].Value = 28;
            node["CssClass"].Value = "yellow-background";
            node["MetaViewTypeName"].Value = m.TypeName;
            node["MetaViewName"].Value = m.Name;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                node["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                node["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                node["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                node["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                node["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.MetaView.MetaView_Single",
                "content6",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaView.GetViewData")]
        protected void Magix_MetaView_GetViewData(object sender, ActiveEventArgs e)
        {
            MetaView m = MetaView.SelectFirst(Criteria.Eq("Name", e.Params["MetaViewName"].Get<string>()));

            e.Params["MetaViewTypeName"].Value = m.TypeName;

            foreach (MetaView.MetaViewProperty idx in m.Properties)
            {
                e.Params["Properties"]["p-" + idx.ID]["ID"].Value = idx.ID;
                e.Params["Properties"]["p-" + idx.ID]["Name"].Value = idx.Name;
                e.Params["Properties"]["p-" + idx.ID]["ReadOnly"].Value = idx.ReadOnly;
                e.Params["Properties"]["p-" + idx.ID]["Description"].Value = idx.Description;
                e.Params["Properties"]["p-" + idx.ID]["Action"].Value = idx.Action;
            }
        }

        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaViewCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaViewCount"]["Header"].Value = "Views";
            e.Params["Type"]["Properties"]["MetaViewCount"]["ClickLabelEvent"].Value = "Magix.MetaView.ViewMetaViews";
            e.Params["Object"]["Properties"]["MetaViewCount"].Value = MetaView.Count.ToString();
        }
    }
}






















