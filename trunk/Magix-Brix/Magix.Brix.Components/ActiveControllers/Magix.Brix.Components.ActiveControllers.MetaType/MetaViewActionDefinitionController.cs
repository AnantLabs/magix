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
        #region [ -- Application Startup. Creation of default, 'built-in' events ... -- ]

        [ActiveEvent(Name = "Magix.Core.ApplicationStartup")]
        protected static void Magix_Core_ApplicationStartup(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.SaveActiveForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.SaveActiveForm";
                    a.EventName = "Magix.Meta.Actions.SaveObject";
                    a.Description = @"Will save the currently active Single-View Form.
Will determine which form raised the event originally, and explicitly save the field values
from that Form into a new Meta Object with the TypeName from the View ...";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.EmptyActiveForm")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.EmptyActiveForm";
                    a.EventName = "Magix.Meta.Actions.EmptyForm";
                    a.Description = @"Will empty the currrently active Editable Form. 
Will determine which form raised the event originally, and explicitly empty that 
form only. Useful for things such as 'Clear Buttons' and such ...";
                    a.StripInput = false;
                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.ShowDefaultMessage")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.ShowDefaultMessage";
                    a.EventName = "Magix.Core.ShowMessage";
                    a.Description = @"Will show a default message to the User. 
Mostly here for Reference Reasons so that you can have an Example Action to 
copy for your own messages. For your convenience ... :)";

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Message";
                    m.Value = "Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?Who's there ...?";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = "Knock, knock ...";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Milliseconds";
                    m.Value = "1500";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.TurnOnDebugging")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.Meta.Actions.TurnOnDebugging";
                    a.EventName = "Magix.Meta.Actions.SetSessionVariable";
                    a.Description = @"Will turn on 'Debugging', meaning you'll have a wire-grid
covering your screen to see the 40x18 pixel 'grid-lock', plus you'll also get to see every single Action 
ever raised on the server shown in an 'Action Stack Trace' Window. This only affects your session, 
meaning it should be safe to do in production to track down errors and such in live software ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "Magix.Core.IsDebug";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.TurnOffDebugging")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.Meta.Actions.TurnOffDebugging";
                    a.EventName = "Magix.Meta.Actions.SetSessionVariable";
                    a.Description = @"Will turn _OFF_ 'Debugging', meaning you'll no longer 
have a wire-grid covering your screen, plus the stack tracing of actions on the 
server will disappear. Only affects your session, and no other logged on users ability to 
see debugging information ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Name";
                    m.Value = "Magix.Core.IsDebug";
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Value";
                    m.Value = "False";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.ViewMetaType")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Meta.Actions.ViewMetaType";
                    a.EventName = "Magix.MetaType.ViewMetaType";
                    a.Description = @"Will load a grid of all Meta Objects of type already loaded
in current activating WebPart. If you want to load a specific type, then you can override the 
type being loaded by adding 'MetaViewTypeName' as a parameter, containing the name of the view. 
There are many other properties you can override...";
                    a.StripInput = false;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "NoIdColumn";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "IsDelete";
                    m.Value = "False";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                tr.Commit();
            }
        }

        #endregion

        [ActiveEvent(Name = "Magix.Meta.Actions.SetSessionVariable")]
        protected void Magix_Meta_Actions_SetSessionVariable(object sender, ActiveEventArgs e)
        {
            Page.Session[e.Params["Name"].Get<string>()] = e.Params["Value"].Value;
        }

        [ActiveEvent(Name = "Magix.Meta.Actions.SaveObject")]
        protected void Magix_Meta_Actions_SaveObject(object sender, ActiveEventArgs e)
        {
            using (Transaction tr = Adapter.Instance.BeginTransaction())
            {
                MetaObject t = new MetaObject();

                t.TypeName = e.Params["MetaViewTypeName"].Get<string>();
                t.Reference = 
                    e.Params["MetaViewName"].Get<string>() +
                    "|" + 
                    e.Params["ActionSenderName"].Get<string>();
                t.Created = DateTime.Now;

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






















