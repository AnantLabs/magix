﻿/*
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
        #region [ -- Application Startup. Creation of default, 'built-in' Actions ... -- ]

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

                    m = new Action.ActionParams();
                    m.Name = "IsInlineEdit";
                    m.Value = "True";
                    m.TypeName = typeof(bool).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.SendEmail")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Meta.Actions.SendEmail";
                    a.EventName = "Magix.MetaType.SendEmail";
                    a.Description = @"Will send yourself an email to the Email address you've associated
with your user. The email will contain a default header and a default body. Override the 
settings if you wish to send other emails, to other recipes, with another subject and/or body. 
PS! This Action is dependent upon that you've configured your web.config to point towards a valid 'mailSettings'.
You _CANNOT_ use rasoftwarefactory.com for this! You might however be able to use for instance your 
Google Account if you do some 'Googling' ... ;)";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Header";
                    m.Value = null; // Intentionally - Template Action ...!
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Body";
                    m.Value = null; // Intentionally - Template Action ...!
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "Email";
                    m.Value = null; // Intentionally - Template Action ...!
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "From";
                    m.Value = null; // Intentionally - Template Action ...!
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "To";
                    m.Value = "[not here silly ...!]";

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "email-adr-1";
                    m2.Value = null; // Intentionally - Template Action ...!
                    m2.TypeName = typeof(string).FullName;
                    m.Children.Add(m2);

                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.ReplaceStringValue")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Meta.Actions.ReplaceStringValue";
                    a.EventName = "Magix.MetaType.ReplaceStringValue";
                    a.Description = @"Will transform every entity of 'OldString' 
found in 'Source' into the contents of 'NewString' and return as a 'Result', output node ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Source";
                    m.Value = "Howdy doodie woodie, my name is [0]";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "OldString";
                    m.Value = "[0]";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    m = new Action.ActionParams();
                    m.Name = "NewString";
                    m.Value = "Marvin";
                    m.TypeName = typeof(string).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.MultiAction")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Meta.Actions.MultiAction";
                    a.EventName = "Magix.MetaType.MultiAction";
                    a.Description = @"Will raise several Actions consecutively, in the order they're defined
in the 'Actions' node. Each Action needs a 'Name' and its own set of parameters through its 'Params' node.
All 'Params' nodes will be copied into the root node before every event is raised. This means that your
Root node will become VERY large after subsequent actions. Be warned ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "Actions";
                    a.Params.Add(m);

                    Action.ActionParams ar = new Action.ActionParams();
                    ar.Name = "act-1";
                    m.Children.Add(ar);

                    Action.ActionParams m2 = new Action.ActionParams();
                    m2.Name = "Name";
                    m2.Value = "Magix.MetaType.ReplaceStringValue";
                    m2.TypeName = typeof(string).FullName;
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    Action.ActionParams m3 = new Action.ActionParams();
                    m3.Name = "Source";
                    m3.Value = @"Hello there world, this is [0] talking 
to you. [0] would like to use this opportunity to thank you for trusting him 
to run your website ... :)";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    m3 = new Action.ActionParams();
                    m3.Name = "OldString";
                    m3.Value = "[0]";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    m3 = new Action.ActionParams();
                    m3.Name = "NewString";
                    m3.Value = "Marvin";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    ar = new Action.ActionParams();
                    ar.Name = "act-2";
                    m.Children.Add(ar);

                    m2 = new Action.ActionParams();
                    m2.Name = "Name";
                    m2.Value = "Magix.MetaType.RenameNode";
                    m2.TypeName = typeof(string).FullName;
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    m3 = new Action.ActionParams();
                    m3.Name = "FromName";
                    m3.Value = "Result";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    m3 = new Action.ActionParams();
                    m3.Name = "ToName";
                    m3.Value = "Message";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    ar = new Action.ActionParams();
                    ar.Name = "act-3";
                    m.Children.Add(ar);

                    m2 = new Action.ActionParams();
                    m2.Name = "Name";
                    m2.Value = "Magix.MetaType.StripEverythingBut";
                    m2.TypeName = typeof(string).FullName;
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    m3 = new Action.ActionParams();
                    m3.Name = "But";
                    m3.Value = "Message";
                    m3.TypeName = typeof(string).FullName;
                    m2.Children.Add(m3);

                    ar = new Action.ActionParams();
                    ar.Name = "act-4";
                    m.Children.Add(ar);

                    m2 = new Action.ActionParams();
                    m2.Name = "Name";
                    m2.Value = "Magix.Core.ShowMessage";
                    m2.TypeName = typeof(string).FullName;
                    ar.Children.Add(m2);

                    m2 = new Action.ActionParams();
                    m2.Name = "Params";
                    ar.Children.Add(m2);

                    m3 = new Action.ActionParams();
                    m3.Name = "Milliseconds";
                    m3.Value = "3000";
                    m3.TypeName = typeof(int).FullName;
                    m2.Children.Add(m3);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.Meta.Actions.GetObjectIntoNode")) == 0)
                {
                    Action a = new Action();

                    a.Name = "Magix.Meta.Actions.GetObjectIntoNode";
                    a.EventName = "Magix.MetaType.GetObjectIntoNode";
                    a.Description = @"Will put every property from the given Meta Object, 
into the given Node, with the name/value pair as the node name/value parts, 
assuming they're all strings. Copy this Action, and make sure you _CHANGE_ its MetaObjectID
towards pointing to the ID of a real existing Meta Object, to fill in values from one of
your Meta Objects into a Node, maybe before sending the node into another even, using
MultiActions or something ...";
                    a.StripInput = true;

                    Action.ActionParams m = new Action.ActionParams();
                    m.Name = "MetaObjectID";
                    m.Value = "-1";
                    m.TypeName = typeof(int).FullName;
                    a.Params.Add(m);

                    a.Save();
                }

                if (Action.CountWhere(
                    Criteria.Eq("Name", "Magix.DynamicEvent.GetActiveFormData")) == 0)
                {
                    Action a = new Action();
                    a.Name = "Magix.DynamicEvent.GetActiveFormData";
                    a.EventName = "Magix.Meta.GetActiveFormData";
                    a.Description = @"Will find the Form that raised the current event
chain, and query it to put all its data flat out into the current Node structure
with the Name/Value as the Node Name/Value pair.";
                    a.StripInput = false;
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






















