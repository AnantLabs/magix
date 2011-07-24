/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.UX.Widgets;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    [ActiveController]
    public class CommonActionController : ActiveController
    {
        [ActiveEvent(Name = "Magix.MetaType.ViewMetaType")]
        protected void Magix_MetaType_ViewMetaType(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            // To help our Publishing Module to refresh ...
            // TODO: Refactor ...
            node["PageObjectTemplateID"].Value = e.Params["PageObjectTemplateID"].Get<int>();

            if (e.Params.Contains("NoIdColumn"))
                node["NoIdColumn"].Value = e.Params["NoIdColumn"].Value;

            if (e.Params.Contains("IsDelete"))
                node["IsDelete"].Value = e.Params["IsDelete"].Value;

            if (e.Params.Contains("IsInlineEdit"))
                node["IsInlineEdit"].Value = e.Params["IsInlineEdit"].Value;

            if (e.Params.Contains("Container"))
                node["Container"].Value = e.Params["Container"].Value;

            if (e.Params.Contains("MetaTemplateObjectID"))
            {
                MetaObject t = MetaObject.SelectByID(e.Params["MetaTemplateObjectID"].Get<int>());
                if (t == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            @"Seems like some wize-guy have deleted the Meta Object ['{0}'] being 
used as a Template for this Meta View ...", 
                            e.Params["MetaTemplateObjectID"].Get<int>()));
                }
                node["MetaTemplateObjectID"].Value = t.ID;
            }
            else
            {
                // Finding first Meta Object of type
                MetaObject t = MetaObject.SelectFirst(
                    Criteria.Eq(
                        "TypeName", 
                        e.Params["MetaViewTypeName"].Get<string>()));
                if (t == null)
                {
                    throw new ArgumentException(
                        string.Format(@"You don't have <em>any Objects with the TypeName</em> you'd like to show. 
This poses a problem for the grid system since it can't know anything about the object, not
even which columns to show for it. Create at least <em>One Object</em> by hand to inform the Grid 
System about how your objects actually looks like. Create at least on item of type '{0}'",
                        e.Params["MetaViewTypeName"].Get<string>()));
                }
                node["MetaTemplateObjectID"].Value = t.ID;
            }

            node["FreezeContainer"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject).FullName + "-META";

            if (e.Params.Contains("WhiteListColumns"))
            {
                node["WhiteListColumns"] = e.Params["WhiteListColumns"];
            }

            if (e.Params.Contains("Type"))
            {
                node["Type"] = e.Params["Type"];
            }

            if (!node.Contains("Container"))
            {
                Node xx = new Node();

                xx["PageObjectTemplateID"].Value = e.Params["PageObjectTemplateID"].Get<int>();
                
                RaiseEvent(
                    "Magix.Meta.GetContainerIDOfApplicationWebPart",
                    xx);
                node["Container"].Value = xx["ID"].Get<string>();
            }

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditMetaObject";
            node["DeleteColumnEvent"].Value = "Magix.Meta.DeleteMetaObject";
            node["ChangeSimplePropertyValue"].Value = "Magix.Meta.ChangeMetaObjectValue";

            node["IsCreate"].Value = false;

            node["ReuseNode"].Value = true;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
        }

        [ActiveEvent(Name = "Magix.MetaType.ViewMetaTypeFromTemplate")]
        protected void Magix_MetaType_ViewMetaTypeFromTemplate(object sender, ActiveEventArgs e)
        {
            RaiseEvent(
                "Magix.MetaType.ViewMetaType",
                e.Params);
        }

        [ActiveEvent(Name = "DBAdmin.Data.ChangeSimplePropertyValue")]
        protected void DBAdmin_Data_ChangeSimplePropertyValue(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META")
            {
                using (Transaction tr = Adapter.Instance.BeginTransaction())
                {
                    MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

                    MetaObject.Value v = t.Values.Find(
                        delegate(MetaObject.Value idx)
                        {
                            return idx.Name == e.Params["PropertyName"].Get<string>();
                        });
                    if (v == null)
                    {
                        v = new MetaObject.Value();
                        v.Name = e.Params["PropertyName"].Get<string>();
                        v.Val = e.Params["NewValue"].Get<string>();
                        t.Values.Add(v);
                        t.Save();
                    }
                    else
                    {
                        v.Val = e.Params["NewValue"].Get<string>();
                        v.Save();
                    }

                    tr.Commit();
                }
            }
        }

        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META")
            {
                MetaObject t = MetaObject.SelectByID(e.Params["MetaTemplateObjectID"].Get<int>());

                foreach (MetaObject.Value idx in t.Values)
                {
                    if (!e.Params.Contains("WhiteListColumns") ||
                        (e.Params["WhiteListColumns"].Contains(idx.Name)) &&
                        e.Params["WhiteListColumns"][idx.Name].Get<bool>())
                    {
                        e.Params["Type"]["Properties"][idx.Name]["Header"].Value = idx.Name;
                        e.Params["Type"]["Properties"][idx.Name]["NoFilter"].Value = true;
                        if (e.Params.Contains("IsInlineEdit") &&
                            !e.Params["IsInlineEdit"].Get<bool>())
                            e.Params["Type"]["Properties"][idx.Name]["ReadOnly"].Value = true;
                    }
                }
            }
        }

        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == typeof(MetaObject).FullName + "-META")
            {
                MetaObject templ = MetaObject.SelectByID(e.Params["MetaTemplateObjectID"].Get<int>());

                e.Params["SetCount"].Value = MetaObject.CountWhere(
                    Criteria.Eq("TypeName", templ.TypeName));

                e.Params["LockSetCount"].Value = true;

                foreach (MetaObject idxO in MetaObject.Select(
                    Criteria.Eq("TypeName", templ.TypeName),
                    Criteria.Range(
                        e.Params["Start"].Get<int>(),
                        e.Params["End"].Get<int>(),
                        "Created",
                        false)))
                {
                    e.Params["Objects"]["o-" + idxO.ID]["ID"].Value = idxO.ID;
                    foreach (MetaObject.Value idx in idxO.Values)
                    {
                        if (idxO.Values.Exists(
                            delegate(MetaObject.Value ixx)
                            {
                                return ixx.Name == idx.Name;
                            }))
                            e.Params["Objects"]["o-" + idxO.ID]["Properties"][idx.Name].Value = idx.Val;
                    }

                    // Looping through, 'touching' all the items with no values ...
                    foreach (MetaObject.Value idx in templ.Values.FindAll(
                        delegate(MetaObject.Value idxI)
                        {
                            return !idxO.Values.Exists(
                                delegate(MetaObject.Value idxI2)
                                {
                                    return idxI2.Name == idxI.Name;
                                });
                        }
                        ))
                    {
                        e.Params["Objects"]["o-" + idxO.ID]["Properties"][idx.Name].Value = "";
                    }
                }
            }
        }

        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObject")]
        protected void DBAdmin_DynamicType_GetObject(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() != typeof(MetaObject).FullName + "-META")
                return;
            MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

            e.Params["Object"]["ID"].Value = t.ID;

            MetaObject templ = MetaObject.SelectByID(e.Params["MetaTemplateObjectID"].Get<int>());

            foreach (MetaObject.Value idx in t.Values)
            {
                if (templ.Values.Exists(
                    delegate(MetaObject.Value ixx)
                    {
                        return ixx.Name == idx.Name;
                    }))
                {
                    e.Params["Object"]["Properties"][idx.Name].Value = idx.Val;
                }
            }

            // Looping through, 'touching' all the items with no values ...
            foreach (MetaObject.Value idx in templ.Values.FindAll(
                delegate(MetaObject.Value idxI)
                {
                    return !t.Values.Exists(
                        delegate(MetaObject.Value idxI2)
                        {
                            return idxI2.Name == idxI.Name;
                        });
                }
                ))
            {
                e.Params["Object"]["Properties"][idx.Name].Value = "";
            }
        }

        [ActiveEvent(Name = "Magix.Meta.EditMetaObject")]
        protected void Magix_Meta_EditMetaObject(object sender, ActiveEventArgs e)
        {
            MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());

            string container = e.Params["Parameters"]["Container"].Get<string>();

            Node node = new Node();

            node["Container"].Value = container;
            node["MetaTemplateObjectID"].Value = e.Params["Parameters"]["MetaTemplateObjectID"].Value;
            node["FreezeContainer"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject) + "-META";
            node["ReuseNode"].Value = true;
            node["ID"].Value = t.ID;

            if (!node.Contains("WhiteListColumns"))
            {
                MetaObject templ = MetaObject.SelectByID(node["MetaTemplateObjectID"].Get<int>());
                foreach (MetaObject.Value idx in templ.Values)
                {
                    node["WhiteListColumns"][idx.Name].Value = true;
                }
            }

            node["WhiteListProperties"]["Name"].Value = true;
            node["WhiteListProperties"]["Name"]["ForcedWidth"].Value = 3;
            node["WhiteListProperties"]["Value"].Value = true;
            node["WhiteListProperties"]["Value"]["ForcedWidth"].Value = 5;
            node["ChangeSimplePropertyValue"].Value = "Magix.Meta.ChangeMetaObjectValue";

            RaiseEvent(
                "DBAdmin.Form.ViewComplexObject",
                node);
        }

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

        [ActiveEvent(Name = "Magix.Meta.ChangeMetaObjectValue")]
        protected void Magix_Meta_ChangeMetaObjectValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                MetaObject.Value val = t.Values.Find(
                    delegate(MetaObject.Value idx)
                    {
                        return idx.Name == e.Params["PropertyName"].Get<string>();
                    });
                if (val == null)
                {
                    val = new MetaObject.Value();
                    val.Name = e.Params["PropertyName"].Get<string>();
                    val.Val = e.Params["NewValue"].Get<string>();
                    t.Values.Add(val);
                    t.Save();
                }
                else
                {
                    val.Val = e.Params["NewValue"].Get<string>();
                    val.Save();
                }

                tr.Commit();
            }
        }

        // TODO: Kep this ont, and then implement support for "Close WebPart Button"
        // But for now, we've changed the ID column event handler to point to another 
        // Event Handler...
        [ActiveEvent(Name = "Magix.Meta.ReloadWebPart")]
        protected void Magix_Meta_ReloadWebPart(object sender, ActiveEventArgs e)
        {
            int po = e.Params["Parameters"]["PageObjectTemplateID"].Get<int>();

            Node node = new Node();

            node["PageObjectTemplateID"].Value = po;

            RaiseEvent(
                "Magix.Publishing.ReloadWebPart",
                node);
        }
    }
}





















