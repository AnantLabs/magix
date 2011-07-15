/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes.MetaTypes;

namespace Magix.Brix.Components.ActiveControllers.MetaTypes
{
    [ActiveController]
    public class MetaViewActionDefinitionController : ActiveController
    {
        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                Action a = new Action();
                a.Name = "SaveObject";
                a.EventName = "Magix.Meta.Actions.SaveObject";
                a.Description = "Will save the active form";
                a.Save();

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Meta.GetActionItems")]
        protected void Magix_Meta_GetActionItems(object sender, ActiveEventArgs e)
        {
            foreach (Action idx in Action.Select())
            {
                e.Params["Items"][idx.Name]["Name"].Value = idx.Name;
                e.Params["Items"][idx.Name]["Action"].Value = idx.EventName;
                e.Params["Items"][idx.Name]["Description"].Value = idx.Description;
            }
        }

        [ActiveEvent(Name = "Magix.Meta.Actions.SaveObject")]
        protected void Magix_Meta_Actions_SaveObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaType t = new MetaType();

                t.Name = e.Params["MetaViewTypeName"].Get<string>();
                t.Reference = e.Params["MetaViewName"].Get<string>() + 
                    "|" + 
                    e.Params["ActionEventName"].Get<string>();

                foreach (Node idx in e.Params["PropertyValues"])
                {
                    MetaType.Value v = new MetaType.Value();
                    v.Name = idx["Name"].Get<string>();
                    v.Val = idx["Value"].Get<string>();
                    t.Values.Add(v);
                }

                t.Save();

                tr.Commit();
            }
        }
    }
}






















