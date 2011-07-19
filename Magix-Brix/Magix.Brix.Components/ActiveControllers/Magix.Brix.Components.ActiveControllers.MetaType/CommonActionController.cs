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
            int po = e.Params["PageObjectTemplateID"].Get<int>();
            node["PageObjectTemplateID"].Value = po;

            if (e.Params.Contains("NoIdColumn"))
                node["NoIdColumn"].Value = e.Params["NoIdColumn"].Value;

            if (e.Params.Contains("IsDelete"))
                node["IsDelete"].Value = e.Params["IsDelete"].Value;

            if (e.Params.Contains("MetaTemplateObjectID"))
            {
                node["MetaTemplateObjectID"].Value = e.Params["MetaTemplateObjectID"].Value;
            }
            else
            {
                // Finding first Meta Object of type
                MetaObject t = MetaObject.SelectFirst(
                    Criteria.Eq("Name", e.Params["MetaTypeName"].Get<string>()));
                node["MetaTemplateObjectID"].Value = t.ID;
            }

            node["FreezeContainer"].Value = true;
            node["FullTypeName"].Value = typeof(MetaObject).FullName + "-META";

            if (e.Params.Contains("WhiteListColumns"))
            {
                node["WhiteListColumns"] = e.Params["WhiteListColumns"];
            }
            else
            {
                node["WhiteListColumns"]["Name"].Value = true;
                node["WhiteListColumns"]["Name"]["ForcedWidth"].Value = 3;
                node["WhiteListColumns"]["Reference"].Value = true;
                node["WhiteListColumns"]["Reference"]["ForcedWidth"].Value = 5;

                node["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
                node["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
                node["Type"]["Properties"]["Reference"]["ReadOnly"].Value = true;
            }

            if (e.Params.Contains("Type"))
            {
                node["Type"] = e.Params["Type"];
            }

            if (!node.Contains("Container"))
            {
                Node xx = new Node();

                xx["PageObjectTemplateID"].Value = po;
                
                RaiseEvent(
                    "Magix.Meta.GetContainerIDOfApplicationWebPart",
                    xx);
                node["Container"].Value = xx["ID"].Get<string>();
            }

            node["FilterOnId"].Value = false;
            node["IDColumnName"].Value = "Edit";
            node["IDColumnValue"].Value = "Edit";
            node["IDColumnEvent"].Value = "Magix.Meta.EditMetaObject";

            node["IsCreate"].Value = false;

            node["ReuseNode"].Value = true;

            MetaObject t2 = MetaObject.SelectByID(node["MetaTemplateObjectID"].Get<int>());

            node["SetCount"].Value = MetaObject.CountWhere(
                Criteria.Eq("Name", t2.Name));
            node["LockSetCount"].Value = true;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DBAdmin.Form.ViewClass",
                node);
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
                        e.Params["Type"]["Properties"][idx.Name]["Header"].Value = idx.Name;
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
                    Criteria.Eq("Name", templ.Name));
                e.Params["LockSetCount"].Value = true;

                foreach (MetaObject idxO in MetaObject.Select(
                    Criteria.Eq("Name", templ.Name),
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

        [ActiveEvent(Name = "Magix.Meta.ChangeMetaObjectValue")]
        protected void Magix_Meta_ChangeMetaObjectValue(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = MetaObject.SelectByID(e.Params["ID"].Get<int>());
                MetaObject.Value val = t.Values.Find(
                    delegate(MetaObject.Value idx)
                    {
                        return t.Name == e.Params["PropertyName"].Get<string>();
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





















