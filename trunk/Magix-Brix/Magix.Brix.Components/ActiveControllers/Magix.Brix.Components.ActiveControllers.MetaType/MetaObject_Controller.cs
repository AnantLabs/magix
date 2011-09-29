/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    /**
     * Level2: Contains logic for editing, maintaining and viewing MetaObjects. MetaObjects are at the 
     * heart of the Meta Application System since they serve as the 'storage' for everything a
     * view updates or creates through interaction with the end user
     */
    [ActiveController]
    public class MetaObject_Controller : ActiveController
    {
        /**
         * Level 2: Returns the Desktop Icon for launching Actions back to caller
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Objects"]["Image"].Value = "media/images/desktop-icons/objects.png";
            e.Params["Items"]["Objects"]["Shortcut"].Value = "B";
            e.Params["Items"]["Objects"]["Text"].Value = "Click to view Meta Objects [Key B]";
            e.Params["Items"]["Objects"]["CSS"].Value = "mux-desktop-icon";
            e.Params["Items"]["Objects"]["Event"].Value = "Magix.MetaType.EditMetaObjects_UnFiltered";
        }

        /**
         * Level2: Returns menu event handlers for viewing MetaObjects
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["MetaType"]["Caption"].Value = "Meta Types";
            e.Params["Items"]["MetaType"]["Items"]["Types"]["Caption"].Value = "Meta Objects ...";
            e.Params["Items"]["MetaType"]["Items"]["Types"]["Event"]["Name"].Value = "Magix.MetaType.EditMetaObjects_UnFiltered";
        }

        /**
         * Level2: Will open up editing of the MetaObject directly, without any 'views' or other interferences.
         * Mostly meant for administrators to edit objects in 'raw mode'
         */
        [ActiveEvent(Name = "Magix.MetaType.EditMetaObjects_UnFiltered")]
        protected void Magix_MetaType_EditMetaObjects_UnFiltered(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["FullTypeName"].Value = typeof(MetaObject).FullName; // NO '-META' here ... !!!
            node["Container"].Value = "content3";
            node["Width"].Value = 18;
            node["Last"].Value = true;
            node["CssClass"].Value = "mux-edit-objects";

            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Reference"].Value = true;
            node["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Created"].Value = true;
            node["WhiteListColumns"]["Created"]["ForcedWidth"].Value = 3;
            node["WhiteListColumns"]["Copy"].Value = true;
            node["WhiteListColumns"]["Copy"]["ForcedWidth"].Value = 2;
            node["FilterOnId"].Value = true;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaType.EditONEMetaObject_UnFiltered";
            node["DeleteColumnEvent"].Value = "Magix.MetaType.DeleteMetaObject";

            node["ReuseNode"].Value = true;
            node["CreateEventName"].Value = "Magix.MetaType.CreateMetaObjectAndEdit";

            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type";
            node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Copy"]["NoFilter"].Value = true;
            node["Type"]["Properties"]["Copy"]["TemplateColumnEvent"].Value = "Magix.MetaType.GetCopyMetaObjectTemplateColumn";

            node["Criteria"]["C1"]["Name"].Value = "Sort";
            node["Criteria"]["C1"]["Value"].Value = "Created";
            node["Criteria"]["C1"]["Ascending"].Value = false;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        /**
         * Level3: Will return a 'Copy Template LinkButton' back to caller
         */
        [ActiveEvent(Name = "Magix.MetaType.GetCopyMetaObjectTemplateColumn")]
        protected void Magix_MetaType_GetCopyMetaObjectTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Fetching specific user
            // Creating our SelectList
            LinkButton ls = new LinkButton();
            ls.Text = "Copy";
            ls.Click +=
                delegate
                {
                    Node tmp = new Node();
                    tmp["ID"].Value = id;

                    RaiseEvent(
                        "Magix.MetaType.CopyMetaObject",
                        tmp);

                    ActiveEvents.Instance.RaiseClearControls("content5");

                    Node node = new Node();
                    node["FullTypeName"].Value = typeof(MetaObject).FullName;

                    RaiseEvent(
                        "Magix.Core.UpdateGrids",
                        node);

                    Node n = new Node();
                    n["FullTypeName"].Value = typeof(MetaObject).FullName;
                    n["ID"].Value = tmp["NewID"].Value;

                    RaiseEvent(
                        "DBAdmin.Grid.SetActiveRow",
                        n);

                    node = new Node();
                    node["ID"].Value = tmp["NewID"].Value;

                    RaiseEvent(
                        "Magix.MetaType.EditONEMetaObject_UnFiltered",
                        node);
                };


            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Will copy the incoming MetaObject ['ID']
         */
        [ActiveEvent(Name = "Magix.MetaType.CopyMetaObject")]
        private void Magix_MetaType_CopyMetaObject(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject n = MetaObject.SelectByID(id).CloneAndSave();
                tr.Commit();
                e.Params["NewID"].Value = n.ID;
            }
        }

        /**
         * Level2: Creates a new MetaObject with some default values and returns the ID of the new MetaObject
         * as 'NewID'
         */
        [ActiveEvent(Name = "Magix.MetaType.CreateMetaObject")]
        protected void Magix_MetaType_CreateMetaObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject m = new MetaObject();
                m.TypeName = "[Anonymous-Coward]";
                m.Reference = e.Name;

                MetaObject.Property val = new MetaObject.Property();
                val.Name = "Default Name";
                val.Value = "Default Value";
                m.Values.Add(val);

                m.Save();

                tr.Commit();
                e.Params["NewID"].Value = m.ID;
            }
        }

        /**
         * Level2: Creates a new MetaObject with some default values, and lets the end 
         * user edit it immediately
         */
        [ActiveEvent(Name = "Magix.MetaType.CreateMetaObjectAndEdit")]
        protected void Magix_MetaType_CreateMetaObjectAndEdit(object sender, ActiveEventArgs e)
        {
            Node x = new Node();
            RaiseEvent(
                "Magix.MetaType.CreateMetaObject",
                x);
 
            Node node = new Node();
            node["Start"].Value = 0;
            node["End"].Value = 10;
            node["FullTypeName"].Value = typeof(MetaObject).FullName;

            RaiseEvent(
                "Magix.Core.SetGridPageStart",
                node);

            node = new Node();
            node["ID"].Value = x["NewID"].Value;

            RaiseEvent(
                "Magix.MetaType.EditONEMetaObject_UnFiltered",
                node);
        }

        // TODO: Break further up. Too long ...
        /**
         * Level2: Allows for editing the MetaObject directly without any Views filtering 
         * out anything
         */
        [ActiveEvent(Name = "Magix.MetaType.EditONEMetaObject_UnFiltered")]
        protected void Magix_MetaType_EditONEMetaObject_UnFiltered(object sender, ActiveEventArgs e)
        {
            MetaObject m = MetaObject.SelectByID(e.Params["ID"].Get<int>());

            Node node = new Node();

            string cssClass = "";

            if (string.IsNullOrEmpty(m.TypeName))
            {
                cssClass = "mux-no-typename";
            }
            else
            {
                int rnd = Math.Abs(m.TypeName.GetHashCode());
                switch (rnd % 7) // TODO: Improve statistical probability by 'smoothening' it ...
                {
                    case 0:
                        cssClass = "mux-grid-bg-type-1";
                        break;
                    case 1:
                        cssClass = "mux-grid-bg-type-2";
                        break;
                    case 2:
                        cssClass = "mux-grid-bg-type-3";
                        break;
                    case 3:
                        cssClass = "mux-grid-bg-type-4";
                        break;
                    case 4:
                        cssClass = "mux-grid-bg-type-5";
                        break;
                    case 5:
                        cssClass = "mux-grid-bg-type-6";
                        break;
                    case 6:
                        cssClass = "mux-grid-bg-type-7";
                        break;
                }
            }

            node["CssClass"].Value = cssClass;

            node["FullTypeName"].Value = typeof(MetaObject).FullName;
            node["ID"].Value = m.ID;

            // First filtering OUT columns ...!
            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["Reference"].Value = true;
            node["WhiteListColumns"]["Created"].Value = true;
            node["WhiteListColumns"]["Children"].Value = true;

            foreach (var idx in m.Values)
            {
                node["WhiteListColumns"][idx.Name].Value = true;
            }

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 4;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 10;

            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type";
            node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = false;
            node["Type"]["Properties"]["Created"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["Children"]["Header"].Value = "Children";
            node["Type"]["Properties"]["Children"]["TemplateColumnEvent"].Value = "Magix.MetaType.GetMetaObjectChildrenTemplateColumn";

            foreach (var idx in m.Values)
            {
                node["Type"]["Properties"][idx.Name]["TemplateColumnHeaderEvent"].Value = "Magix.MetaType.GetMetaObjectValuesNAMETemplateColumn";
                node["Type"]["Properties"][idx.Name]["TemplateColumnEvent"].Value = "Magix.MetaType.GetMetaObjectValuesTemplateColumn";
                node["Type"]["Properties"][idx.Name]["ReadOnly"].Value = false;
                node["Object"]["Properties"][idx.Name].Value = idx.Value;
            }

            node["Width"].Value = 16;
            node["Last"].Value = true;
            node["Padding"].Value = 8;
            node["MarginBottom"].Value = 20;
            if (e.Params != null && 
                e.Params.Contains("Container") &&
                e.Params["Container"].Get<string>() != "content4")
            {
                node["Container"].Value = e.Params["Container"].Value;
                node["PullTop"].Value = 18;
            }
            else
            {
                node["Container"].Value = "content4";
                ActiveEvents.Instance.RaiseClearControls("content5");
            }

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);

            string container = node["Container"].Get<string>();

            // Must append a 'New Property Button' after our object editing view
            node = new Node();

            node["Text"].Value = "+";
            node["ToolTip"].Value = "Click to create a New Property for your Object ...";
            node["ButtonCssClass"].Value = "span-2 clear-both";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.MetaType.CreateNewMetaObject-Value-AndEdit";
            node["Event"]["ObjectID"].Value = m.ID;
            node["Event"]["Container"].Value = container;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                container,
                node);
        }

        /**
         * Level2: Will Append an existing MetaObject [ID] to another existing 
         * MetaObject [ParentID] as a child
         */
        [ActiveEvent(Name = "Magix.MetaType.AppendChildMetaObjectToMetaObject")]
        protected void Magix_MetaType_AppendChildMetaObjectToMetaObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject parent = MetaObject.SelectByID(e.Params["ParentID"].Get<int>());
                MetaObject child = MetaObject.SelectByID(e.Params["ID"].Get<int>());

                parent.Children.Add(child);

                parent.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Will Append an existing MetaObject [ID] to another existing MetaObject [ParentID] as a child
         * and immediately Edit the Parent MetaObject
         */
        [ActiveEvent(Name = "Magix.MetaType.AppendChildMetaObjectToMetaObjectAndEditParent")]
        protected void Magix_MetaType_AppendChildMetaObjectToMetaObjectAndEditParent(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.MetaType.AppendChildMetaObjectToMetaObject",
                e.Params);

            ActiveEvents.Instance.RaiseClearControls("content6");

            Node node = new Node();

            node["ID"].Value = e.Params["ParentID"].Value;

            RaiseEvent(
                "Magix.MetaType.EditONEMetaObject_UnFiltered",
                node);
        }

        /**
         * Level2: Calls DBAdmin.Form.AppendObject after overriding some 'visual properties' for the Grid
         * system
         */
        [ActiveEvent(Name = "DBAdmin.Form.AppendObject-OverriddenForVisualReasons")]
        protected void DBAdmin_Form_AppendObject_OverriddenForVisualReasons(object sender, ActiveEventArgs e)
        {
            e.Params["Container"].Value = "content6";
            e.Params["ReUseNode"].Value = true;
            e.Params["Padding"].Value = 8;
            e.Params["Width"].Value = 16;
            e.Params["PullTop"].Value = 18;
            e.Params["Last"].Value = true;
            e.Params["SelectEvent"].Value = "Magix.MetaType.AppendChildMetaObjectToMetaObjectAndEditParent";
            e.Params["IdColumnNotClickable"].Value = true;
            e.Params["IsFilter"].Value = false;
            e.Params["IsDelete"].Value = false;

            e.Params["WhiteListColumns"]["TypeName"].Value = true;
            e.Params["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 4;
            e.Params["WhiteListColumns"]["Reference"].Value = true;
            e.Params["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 6;

            e.Params["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["TypeName"]["Header"].Value = "Type";
            e.Params["Type"]["Properties"]["Reference"]["ReadOnly"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.AppendObject",
                e.Params);
        }

        /**
         * Level2: Creates a new 'Value Row' for our MetaObject ['ID']. Returns the ID of the new Value object
         * as 'NewID'
         */
        [ActiveEvent(Name = "Magix.MetaType.CreateNewMetaObject-Value")]
        protected void Magix_MetaType_CreateNewMetaObject_Value(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject o = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                MetaObject.Property v = new MetaObject.Property();
                o.Values.Add(v);

                o.Save();

                tr.Commit();

                e.Params["NewID"].Value = v.ID;
            }
        }

        /**
         * Level2: Creates a new 'Value Row' for our MetaObject and Edits the MetaObject immediately
         */
        [ActiveEvent(Name = "Magix.MetaType.CreateNewMetaObject-Value-AndEdit")]
        protected void Magix_MetaType_CreateNewMetaObject_Value_AndEdit(object sender, ActiveEventArgs e)
        {
            e.Params["ID"].Value = e.Params["ObjectID"].Value;

            RaiseEvent(
                "Magix.MetaType.CreateNewMetaObject-Value",
                e.Params);

            Node node = new Node();

            node["ID"].Value = e.Params["ID"].Value;
            node["Container"].Value = e.Params["Container"].Value;

            // Easy out ...
            RaiseEvent(
                "Magix.MetaType.EditONEMetaObject_UnFiltered",
                node);
        }

        /**
         * Level2: Calls 'Magix.MetaType.EditONEMetaObject_UnFiltered' after changing the Container
         * to display the module within. Allows editging of Child MetaObjects
         */
        [ActiveEvent(Name = "Magix.MetaType.EditONEMetaObject_UnFiltered-ChildMetaObject")]
        protected void Magix_MetaType_EditONEMetaObject_UnFiltered_ChildMetaObject(object sender, ActiveEventArgs e)
        {
            e.Params["Container"].Value = "content6";

            RaiseEvent(
                "Magix.MetaType.EditONEMetaObject_UnFiltered",
                e.Params);
        }

        /**
         * Level3: Returns a LinkButton with no Children back to caller upon which clicked will start editing
         * the Children collection of objects within the MetaObject
         */
        [ActiveEvent(Name = "Magix.MetaType.GetMetaObjectChildrenTemplateColumn")]
        protected void Magix_MetaType_GetMetaObjectChildrenTemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Creating our SelectList
            LinkButton ls = new LinkButton();
            ls.Text = value;

            // Supplying our Event Handler for the Changed Event ...
            ls.Click +=
                delegate
                {
                    ViewMetaObjectChildrenCollection(id);
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /*
         * Helper for the above ...
         */
        private void ViewMetaObjectChildrenCollection(int id)
        {
            Node node = new Node();

            node["Container"].Value = "content5";
            node["Top"].Value = 1;
            node["Width"].Value = 16;
            node["Padding"].Value = 8;
            node["Last"].Value = true;
            node["PullTop"].Value = 18;
            node["MarginBottom"].Value = 20;

            node["ID"].Value = id;
            node["PropertyName"].Value = "Children";
            node["IsList"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject).FullName;
            node["ReUseNode"].Value = true;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.MetaType.EditONEMetaObject_UnFiltered-ChildMetaObject";
            node["AppendEventName"].Value = "DBAdmin.Form.AppendObject-OverriddenForVisualReasons";
            node["RemoveEvent"].Value = "Magix.Publishing.RemoveChildObjectAndEdit";

            node["WhiteListColumns"]["TypeName"].Value = true;
            node["WhiteListColumns"]["TypeName"]["ForcedWidth"].Value = 5;
            node["WhiteListColumns"]["Reference"].Value = true;
            node["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 6;

            node["Type"]["Properties"]["TypeName"]["ReadOnly"].Value = true;
            node["Type"]["Properties"]["TypeName"]["Header"].Value = "Type";
            node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = true;

            RaiseEvent(
                "DBAdmin.Form.ViewListOrComplexPropertyValue",
                node);

            ActiveEvents.Instance.RaiseClearControls("content6");
        }

        /**
         * Level2: Will remove the Child MetaObject ['ID'] from the Parent MetaObject ['ParentID']
         * collection of children. Notice the child object will NOT be deleted, only
         * 'unreferenced out of' the parent MetaObject
         */
        [ActiveEvent(Name = "Magix.Publishing.RemoveChildObject")]
        protected void Magix_Publishing_RemoveChildObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject parent = MetaObject.SelectByID(e.Params["ParentID"].Get<int>());
                MetaObject child = MetaObject.SelectByID(e.Params["ID"].Get<int>());

                parent.Children.Remove(child);

                child.ParentMetaObject = null;

                parent.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Will remove the Child MetaObject ['ID'] from the Parent MetaObject ['ParentID']
         * collection of children. Notice the child object will NOT be deleted, only
         * 'unreferenced out of' the parent MetaObject. Will instantly edit the Parent
         * MetaObject.
         */
        [ActiveEvent(Name = "Magix.Publishing.RemoveChildObjectAndEdit")]
        protected void Magix_Publishing_RemoveChildObjectAndEdit(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.Publishing.RemoveChildObject",
                e.Params);

            Node node = new Node();
            node["ID"].Value = e.Params["ParentID"].Value;

            // Easy out ...
            RaiseEvent(
                "Magix.MetaType.EditONEMetaObject_UnFiltered",
                node);
        }

        /**
         * Level3: Will return a TextAreaEdit, from which the Value of the Value object belonging to
         * the MetaObject can be edited, and a LinkButton, from which the entire object can
         * be deleted, back to caller
         */
        [ActiveEvent(Name = "Magix.MetaType.GetMetaObjectValuesTemplateColumn")]
        protected void Magix_MetaType_GetMetaObjectValuesTemplateColumn(object sender, ActiveEventArgs e)
        {
            // TODO: Break up ....
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();

            // Creating our Edit Value
            TextAreaEdit inplace = new TextAreaEdit();
            inplace.Text = value;
            inplace.CssClass = "mux-in-place-edit mux-larger-edit mux-left-float";

            // Supplying our Event Handler for the Changed Event ...
            inplace.TextChanged +=
                delegate
                {
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        MetaObject o = MetaObject.SelectByID(id);
                        MetaObject.Property va = o.Values.Find(
                            delegate(MetaObject.Property idxS)
                            {
                                return idxS.Name == name;
                            });
                        va.Value = inplace.Text;

                        va.Save();

                        tr.Commit();
                    }
                };

            System.Web.UI.WebControls.PlaceHolder p = new System.Web.UI.WebControls.PlaceHolder();
            p.Controls.Add(inplace);

            LinkButton lb = new LinkButton();
            lb.Text = "&nbsp;";
            lb.CssClass = "mux-remove-property";
            lb.ToolTip = "Click to Permanently Remove Property ...";
            lb.Click +=
                delegate
                {
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        MetaObject o = MetaObject.SelectByID(id);
                        MetaObject.Property va = o.Values.Find(
                            delegate(MetaObject.Property idxS)
                            {
                                return idxS.Name == name;
                            });
                        va.Delete();

                        o.Values.Remove(va);

                        tr.Commit();

                        // Since the property was deleted, we will
                        // re-databind the whole grid for simplicity ...

                        Node node = new Node();
                        node["ID"].Value = id;

                        RaiseEvent(
                            "Magix.MetaType.EditONEMetaObject_UnFiltered",
                            node);
                    }
                };
            p.Controls.Add(lb);

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = p;
        }

        /**
         * Level3: Will return an InPlaceEdit back to caller, since having Carriage Returns in
         * a Property Name would only serve to be ridiculous
         */
        [ActiveEvent(Name = "Magix.MetaType.GetMetaObjectValuesNAMETemplateColumn")]
        protected void Magix_MetaType_GetMetaObjectValuesNAMETemplateColumn(object sender, ActiveEventArgs e)
        {
            // Extracting necessary variables ...
            string name = e.Params["Name"].Get<string>();
            string fullTypeName = e.Params["FullTypeName"].Get<string>();
            int id = e.Params["ID"].Get<int>();
            string value = e.Params["Value"].Get<string>();
            string container = e.Params["Container"].Get<string>();

            // Creating our Edit Name of property editer ...
            InPlaceEdit ls = new InPlaceEdit();
            ls.Text = name;
            ls.CssClass = "mux-in-place-edit mux-larger-edit";

            // Supplying our Event Handler for the Changed Event ...
            ls.TextChanged +=
                delegate
                {
                    using (Transaction tr = Adapter.Instance.BeginTransaction())
                    {
                        MetaObject o = MetaObject.SelectByID(id);
                        MetaObject.Property va = o.Values.Find(
                            delegate(MetaObject.Property idxS)
                            {
                                return idxS.Name == name;
                            });
                        if (ls.Text == va.Name)
                            return;
                        va.Name = ls.Text;

                        va.Save();

                        tr.Commit();

                        // Since the Name of the Property has changed, we
                        // re-databind the whole grid for simplicity ...

                        Node node = new Node();
                        node["ID"].Value = id;
                        node["Container"].Value = container;

                        RaiseEvent(
                            "Magix.MetaType.EditONEMetaObject_UnFiltered",
                            node);
                    }
                };

            // Stuffing our newly created control into the return parameters, so
            // our Grid control can put it where it feels for it ... :)
            e.Params["Control"].Value = ls;
        }

        /**
         * Level2: Will ask the user for confirmation to assure he really wants to delete the specific
         * MetaObject ['ID'], and if the user confirms will delete that object
         */
        [ActiveEvent(Name = "Magix.MetaType.DeleteMetaObject")]
        protected void Magix_MetaType_DeleteMetaObject(object sender, ActiveEventArgs e)
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
<p>Are you sure you wish to delete this object? 
Deletion is permanent, and cannot be undone! 
Deletion of this object <span style=""color:Red;font-weight:bold;"">will also trigger 
deletion of several other objects</span>, since it may 
have relationships towards other instances in your database.</p>";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.MetaType.DeleteObjectRaw-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Level2: Implementation of deletion of MetaObject after user has confirmed he really 
         * wants to delete it
         */
        [ActiveEvent(Name = "Magix.MetaType.DeleteObjectRaw-Confirmed")]
        protected void Magix_MetaType_DeleteObjectRaw_Confirmed(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject m = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                m.Delete();

                tr.Commit();

                ActiveEvents.Instance.RaiseClearControls("content4");
                ActiveEvents.Instance.RaiseClearControls("child");

                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        /**
         * Level2: Here only to make sure Grids are updated if we're adding a child MetaObject 
         * to another MetaObject
         */
        [ActiveEvent(Name = "DBAdmin.Common.CreateObjectAsChild")]
        protected void DBAdmin_Common_CreateObjectAsChild(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject.Property).FullName)
            {
                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName;

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);
            }
        }

        /**
         * Level2: Clears from content4 and out
         */
        [ActiveEvent(Name = "DBAdmin.Common.ComplexInstanceDeletedConfirmed")]
        protected void DBAdmin_Common_ComplexInstanceDeletedConfirmed(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName)
            {
                ActiveEvents.Instance.RaiseClearControls("content4");
            }
        }

        /**
         * Level2: Returns the number of MetaObjects in the system back to caller and the 
         * name of the Event needed to show all MetaObjects in the system
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDataForAdministratorDashboard")]
        protected void Magix_Publishing_GetDataForAdministratorDashboard(object sender, ActiveEventArgs e)
        {
            e.Params["WhiteListColumns"]["MetaTypesCount"].Value = true;
            e.Params["Type"]["Properties"]["MetaTypesCount"]["ReadOnly"].Value = true;
            e.Params["Type"]["Properties"]["MetaTypesCount"]["Header"].Value = "Objects";
            e.Params["Type"]["Properties"]["MetaTypesCount"]["ClickLabelEvent"].Value = "Magix.MetaType.EditMetaObjects_UnFiltered";
            e.Params["Object"]["Properties"]["MetaTypesCount"].Value = MetaObject.Count.ToString();
        }

        /**
         * Level2: Will either update the existing Value or create a new Value with the given 'Name'
         * and make sure exists within the MetaObject ['ID']
         */
        [ActiveEvent(Name = "Magix.MetaType.SetMetaObjectValue")]
        protected void Magix_MetaType_SetMetaObjectValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (!e.Params.Contains("ID") ||
                    e.Params["ID"].Get<int>() == -1)
                    throw new ArgumentException("Oops, that object doesn't exist ...");

                MetaObject o = MetaObject.SelectByID(e.Params["ID"].Get<int>());

                if (o == null)
                    throw new ArgumentException("Oops, that object doesn't exist2 ...");

                MetaObject.Property val = o.Values.Find(
                    delegate(MetaObject.Property idx)
                    {
                        return idx.Name == e.Params["Name"].Get<string>();
                    });

                if (val == null)
                {
                    val = new MetaObject.Property();
                    val.Name = e.Params["Name"].Get<string>();
                    o.Values.Add(val);

                    o.Save();
                }
                val.Value = e.Params["Value"].Get<string>();

                val.Save();

                tr.Commit();
            }
        }

        /**
         * Level2: Handled to make sure we can traverse our MetaObjects in META mode [front-web, showing grids and views 
         * of Meta Objects]
         */
        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META")
            {
                using (Transaction tr = Adapter.Instance.BeginTransaction())
                {
                    MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

                    MetaObject.Property v = t.Values.Find(
                        delegate(MetaObject.Property idx)
                        {
                            return idx.Name == e.Params["PropertyName"].Get<string>();
                        });
                    if (v == null)
                    {
                        v = new MetaObject.Property();
                        v.Name = e.Params["PropertyName"].Get<string>();
                        v.Value = e.Params["NewValue"].Get<string>();
                        t.Values.Add(v);
                        t.Save();
                    }
                    else
                    {
                        v.Value = e.Params["NewValue"].Get<string>();
                        v.Save();
                    }

                    tr.Commit();
                }
            }
        }

        /**
         * Level2: Handled to make sure "META mode" MetaObjects can be seen 'front-web'
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObject")]
        protected void DBAdmin_DynamicType_GetObject(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() != typeof(MetaObject).FullName + "-META")
                return;

            MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

            e.Params["Object"]["ID"].Value = t.ID;

            foreach (MetaObject.Property idx in t.Values)
            {
                e.Params["Object"]["Properties"][idx.Name].Value = idx.Value;
            }
        }

        /**
         * Level2: Handled to make sure "META mode" MetaObjects can be edited 'front-web'
         */
        [ActiveEvent(Name = "Magix.Meta.EditMetaObject")]
        protected void Magix_Meta_EditMetaObject(object sender, ActiveEventArgs e)
        {
            MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

            string container = e.Params["Parameters"]["Container"].Get<string>();

            Node node = new Node();

            node["Container"].Value = container;
            node["FreezeContainer"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject) + "-META";
            node["ReuseNode"].Value = true;
            node["ID"].Value = t.ID;
            node["MetaViewName"].Value = e.Params["Parameters"]["MetaViewName"].Get<string>();
            node["OriginalWebPartID"].Value = e.Params["Parameters"]["OriginalWebPartID"].Value;

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 5;
            node["ChangeSimplePropertyValue"].Value = "Magix.Meta.ChangeMetaObjectValue";

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

        /**
         * Level2: Handled to make sure "META mode" MetaObjects can be deleted 'front-web'
         */
        [ActiveEvent(Name = "Magix.Meta.DeleteMetaObject")]
        protected void Magix_Meta_DeleteMetaObject(object sender, ActiveEventArgs e)
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
            node["Caption"].Value = @"Please confirm!";
            node["Text"].Value = @"I hope you know what you're doing when deleting this object,
many things can go wrong if it's in use in other places, such as being used as a template 
for your views and such. Please confirm that you really know what you're doing, and that 
you'd still like to have this object deleted ...";
            node["OK"]["ID"].Value = id;
            node["OK"]["FullTypeName"].Value = fullTypeName;
            node["OK"]["Event"].Value = "Magix.Meta.DeleteMetaObject-Confirmed";
            node["Cancel"]["Event"].Value = "DBAdmin.Common.ComplexInstanceDeletedNotConfirmed";
            node["Cancel"]["FullTypeName"].Value = fullTypeName;
            node["Width"].Value = 15;

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.MessageBox",
                "child",
                node);
        }

        /**
         * Level2: Handled to make sure "META mode" MetaObjects can be seen 'front-web'. Confirmation, actual
         * deletion
         */
        [ActiveEvent(Name = "Magix.Meta.DeleteMetaObject-Confirmed")]
        protected void Magix_Meta_DeleteMetaObject_Confirmed(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                t.Delete();

                tr.Commit();

                Node node = new Node();
                node["FullTypeName"].Value = typeof(MetaObject).FullName + "-META";

                RaiseEvent(
                    "Magix.Core.UpdateGrids",
                    node);

                ActiveEvents.Instance.RaiseClearControls("child"); // Assuming message box still visible ...
            }
        }

        /**
         * Level2: Handled to make sure "META mode" MetaObjects can have their values changed 'front-web'
         */
        [ActiveEvent(Name = "Magix.Meta.ChangeMetaObjectValue")]
        protected void Magix_Meta_ChangeMetaObjectValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                MetaObject.Property val = t.Values.Find(
                    delegate(MetaObject.Property idx)
                    {
                        return idx.Name == e.Params["PropertyName"].Get<string>();
                    });
                if (val == null)
                {
                    val = new MetaObject.Property();
                    val.Name = e.Params["PropertyName"].Get<string>();
                    val.Value = e.Params["NewValue"].Get<string>();
                    t.Values.Add(val);
                    t.Save();
                }
                else
                {
                    val.Value = e.Params["NewValue"].Get<string>();
                    val.Save();
                }

                tr.Commit();
            }
        }
    }
}
