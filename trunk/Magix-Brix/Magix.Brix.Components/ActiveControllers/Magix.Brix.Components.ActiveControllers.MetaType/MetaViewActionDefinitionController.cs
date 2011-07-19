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
using System.Globalization;

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
                if (Action.CountWhere(
                    Criteria.Eq("EventName", "Magix.Meta.Actions.SaveObject")) == 0)
                {
                    Action a = new Action();
                    a.Name = "SaveObject";
                    a.EventName = "Magix.Meta.Actions.SaveObject";
                    a.Description = "Will save the currently active Single-View form";
                    a.Save();
                }
                if (Action.CountWhere(
                    Criteria.Eq("EventName", "Magix.Meta.Actions.EmptyForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "EmptyForm";
                    a.EventName = "Magix.Meta.Actions.EmptyForm";
                    a.Description = "Will empty the currrently active Single-View form";
                    a.Save();
                }
                if (Action.CountWhere(
                    Criteria.Eq("EventName", "Magix.Core.ShowMessage")) == 0)
                {
                    Action a = new Action();
                    a.Name = "ShowMessage";
                    a.EventName = "Magix.Core.ShowMessage";
                    a.Description = "Will show a default message to the User";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Message";
                    m.Value = "Hello World 2.0 ...";
                    a.Params.Add(m);

                    a.Save();
                }

                tr.Commit();
            }
        }

        [ActiveEvent(Name = "Magix.Meta.RaiseEvent")]
        protected void Magix_Meta_RaiseEvent(object sender, ActiveEventArgs e)
        {
            Action action = Action.SelectByID(e.Params["ActionID"].Get<int>());

            Node node = e.Params;
            if (action.StripInput)
                node = new Node();

            foreach (Action.ActionParams idx in action.Params)
            {
                GetActionParameters(node, idx);
            }

            RaiseEvent(
                action.EventName,
                node);
        }

        private static void GetActionParameters(Node node, Action.ActionParams a)
        {
            switch (a.TypeName)
            {
                case "System.String":
                    node[a.Name].Value = a.Value;
                    break;
                case "System.Int32":
                    node[a.Name].Value = int.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.Decimal":
                    node[a.Name].Value = decimal.Parse(a.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    node[a.Name].Value = DateTime.ParseExact(a.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    node[a.Name].Value = bool.Parse(a.Value);
                    break;
                default:
                    node[a.Name].Value = a.Value;
                    break;
            }
            foreach (Action.ActionParams idx in a.Children)
            {
                GetActionParameters(node[a.Name], idx);
            }
        }

        [ActiveEvent(Name = "Magix.Meta.Actions.SaveObject")]
        protected void Magix_Meta_Actions_SaveObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = new MetaObject();

                t.Name = e.Params["MetaViewTypeName"].Get<string>();
                t.Reference = e.Params["EventReference"].Get<string>();
                t.Created = e.Params["EventTime"].Get<DateTime>();

                foreach (Node idx in e.Params["PropertyValues"])
                {
                    MetaObject.Value v = new MetaObject.Value();
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






















