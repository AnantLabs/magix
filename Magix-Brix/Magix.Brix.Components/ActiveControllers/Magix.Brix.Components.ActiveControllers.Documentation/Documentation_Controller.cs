/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Types;
using Magix.Brix.Loader;
using Doxygen.NET;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using Magix.Brix.Components.ActiveTypes.MetaTypes;
using Magix.Brix.Data;
using Magix.Brix.Components.ActiveTypes;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveControllers.Documentation
{
    /**
     * Level2: Contains the logic for our 'Class Browser' which can browse all the 
     * classes in the system and make some changes to them by enabling them and
     * disabling them by either overriding specific events or by disabling entire
     * controllers or modules all together
     */
    [ActiveController]
    public class Documentation_Controller : ActiveController
    {
        /**
         * Level2: Will populate the Desktop
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Help"]["Image"].Value = "media/images/desktop-icons/marvin-help.png";
            e.Params["Items"]["Help"]["Shortcut"].Value = "x";
            e.Params["Items"]["Help"]["Text"].Value = "Click to have Marvin Magix Rescue you ... [Key H]";
            e.Params["Items"]["Help"]["CSS"].Value = "mux-desktop-icon mux-help";
            e.Params["Items"]["Help"]["Event"].Value = "Magix.Documentation.LaunchHeka";
        }

        /**
         * Level2: Will return the menu items needed to start the class browser
         */
        [ActiveEvent(Name = "Magix.Documentation.LaunchHeka")]
        protected void Magix_Documentation_LaunchHeka(object sender, ActiveEventArgs e)
        {
            LoadTipOfToday();
        }

        /*
         * Helper for above
         */
        private void LoadTipOfToday()
        {
            Node node = new Node();
            if (Page.Session["HasLoadedTooltipOfToday"] == null)
            {
                node["Text"].Value = TipOfToday.Instance.Next(UserBase.Current.Username);
                Page.Session["HasLoadedTooltipOfToday"] = true;
            }
            else
            {
                node["Text"].Value = TipOfToday.Instance.Current(UserBase.Current.Username);
            }
            node["CssClass"].Value = "mux-tooltip-module";
            node["ChildCssClass"].Value = "mux-tool-tip";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.TipOfToday",
                "floater",
                node);
        }

        /**
         * Level2: Will return the menu items needed to start the class browser
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Admin"]["Caption"].Value = "Admin";

            Node tmp = e.Params["Items"]["Admin"]["Items"]["System"]["Items"]["DoxDotNet"];
            tmp["Caption"].Value = "Class Browser ...";
            tmp["Event"]["Name"].Value = "Magix.MetaType.ViewDoxygenFiles";
        }

        /**
         * Level2: Sink for returning documentation for Active Event. Pass in 'ActiveEventName' 
         * and 'Result' will contain the documentation, concatenated from all its Event Handlers
         */
        [ActiveEvent(Name = "Magix.Core.GetDocumentationForActiveEvent")]
        protected void Magix_Core_GetDocumentationForActiveEvent(object sender, ActiveEventArgs e)
        {
            string n = e.Params["ActiveEventName"].Get<string>();
            if (!string.IsNullOrEmpty(n))
            {
                n = n.Replace(".", "_").Replace("-", "_");
                foreach (Class idx in Docs.GetAllClasses())
                {
                    foreach (Method idxM in idx.GetMethods(2))
                    {
                        if (idxM.Name.StartsWith(n))
                        {
                            e.Params["Result"].Value = 
                                e.Params["Result"].Get<string>("") + 
                                idx.Namespace.FullName + "." + idx.Name + "." + idxM.Name + ":" +
                                idxM.Description + 
                                "|";
                        }
                    }
                    if (e.Params.Contains("Result"))
                        e.Params["Result"].Value = e.Params["Result"].Get<string>().Trim('|');
                }
            }
        }

        /**
         * Level2: Gets every namespace and class into a tree hierarchy and loads up the Tree module
         * to display it
         */
        [ActiveEvent(Name = "Magix.MetaType.ViewDoxygenFiles")]
        protected void Magix_MetaType_ViewDoxygenFiles(object sender, ActiveEventArgs e)
        {
            // Getting setting for 'Level of Documentation' [easy, intermediate or advanced or 'super']
            Node xx = new Node();

            xx["Name"].Value = "Magix.Brix.Components.ActiveControllers.Documentation.CurrentLevel";
            xx["Default"].Value = "1";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                xx);

            int level = int.Parse(xx["Value"].Get<string>(), CultureInfo.InvariantCulture);

            Node node = new Node();
            foreach (Namespace idx in Docs.GetNamespaces(level))
            {
                Node outerMostNamespace = AddNamespaceToNode(idx, node);
                foreach (Class idxC in idx.GetClasses(level))
                {
                    AddClassToNode(idxC, outerMostNamespace);
                }
            }

            node["Width"].Value = 9;
            node["MarginBottom"].Value = 10;
            node["Last"].Value = false;
            node["Header"].Value = "Documentation for Magix";
            node["FullTypeName"].Value = typeof(Namespace).FullName;
            node["ItemSelectedEvent"].Value = "Magix.Doxygen.ViewNamespaceOrClass";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Tree",
                "content3",
                node);

            LoadToggleDoxLevelButton(level);
            LoadGiveMeMyDarnBookDamnit(level);
        }

        private void LoadGiveMeMyDarnBookDamnit(int level)
        {
            // Loading 'change level button' ...
            Node node = new Node();

            node["ButtonCssClass"].Value = "span-8 clear-both down-1";
            node["Append"].Value = true;

            // TODO: Marketing campaign around E@ster E9g ... :P
            // PS!
            // I bet the guy who finds it, will find it through SVN/History, and now read this text,
            // admit to himself that he's cheating [maybe], don't really give a damn, and
            // move on with his life, claiming the Easter Egg reward anyway ... !!! ;)
            // Still he needs to explain why it's an E@ster E9g, and what's 'wrong with it' ...
            // And he also needs to explain the probem with _ONE_ word ...!
            // In an override for a Serialized Action which changes the text of the button
            // to that one word which explains it, and make that Action Publicly Available 
            // for Download ... ;)
            node["Event"].Value = "Magix.Doxygen.Nei!Det_E_Boka_Mi ...!";
            node["Text"].Value = GetTextForGiveMeMyBookManButton();

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content3",
                node);
        }

        /**
         * Will generate the PDF containing the entire documentation, plus potential
         * plugins documentation features, such as the MetaAction and MetaView system etc.
         * Will redirect the client to download the PDF file once generated.
         */
        [ActiveEvent(Name = "Magix.Doxygen.Nei!Det_E_Boka_Mi ...!")]
        protected void Magix_Doxygen_Give_Me_My_Bok_Mann(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            int level = GetDoxLevel();

            SetTitleAndCoverImage(level, node);

            // Index ...
            CreateIntroduction_TipOfToday_IndexItem(node);
            CreateIndexItemForMetaActions(node);
            CreateIndexItemsForClasses(node, level);

            // Pages [linked to from index - hopefully ... ! :O]
            GetTipOfTodayPagesForDox(node);
            GetAllActionsForPages(node);
            GetAllClassesForPages(node, level);

            RaiseEvent(
                "Magix.PDF.CreatePDF-Book",
                node);
        }

        private void GetAllActionsForPages(Node node)
        {
            string allActionsHtml = "<h1>Meta Actions</h1>";
            allActionsHtml += @"
<p>Actions is one of the more important concepts in Magix. Actions wraps Active Events, which again wraps verbs, and hence becomes all your Business Logic. Combined with the Meta Form Designer, Actions are the by far most important concepts you'll need to understand before you can achieve productivity in Magix.</p>
<img src=""media/images/screenshots/actions-screen.png"" />
<em>While logged in as an Administrator in your Dashboard, click either 'Actions' or use the menu to open up your Action list.</em>
<p>Meta Actions, or 'Actions' for short, are dynamically created actions within the system, 
either created by the system itself during installation, or Actions created 
by other software packages, or by users, such as you.</p>
<p>System actions cannot be changed or modified through the Action screen. To change a system action, copy it and change the copy and consume the copy, or use the Data Explorer, which will allow you to edit your System Actions - NOT RECOMMENDED!</p>
<p>Actions can be invoked by some sort of event, for instance a user 
clicking a button, or something similar, in for instance a Meta Form or a Meta View. Basically, if it can be described as a verb, it's an Action.</p>
<p>Actions are wrappers around Active Events, which again are sinks within your code, which you can reach from the outer 
most environment as you wish, depending upon what you wish to allow for, as an admin of your server. The Action System is the soul of Magix, since it's the wrapper around the most important concept in the O2 Process; Active Events.</p>
<p>Actions can override Active Events, and polymorphistically transfer execution to another server of your choice, 
and such create perfect scaling out capabilities, 100% transparently. To use this feature search for the 'transmit' action in your Actions list. The Transmit Action shows how you can transfer polymorphistically an action from one server to another using HTTP POST and serializing the Active Event Name and Node structure down to JSON and HTTP POST parameters such that the event is 100% transparently transfered to another server of your choice, and invoked at your endpoint, if permissions are granted, and then returning the result back to the invoker of the Action.</p>
<p>By combining Transmit with Execute actions, you can create logical entities such as 'Customer Server)s)' or 'Logging Server(s)' etc, simply by polymorphistically raise your Active Events on another end point of your choice.</p>
<p>Actions can override one Active Event with another and such exchange one piece of logic with another piece of logic. To use this feature make sure the 'Override' property of your Action points to the Action you wish to override, then run the ReInitializeOverridden system Action. No changes are being done by Overrides before ReInitialize is executed. This is such that you can make changes in your system, then later when you're done, you reinitialize your actions overrides, and your system updates with the new logic.</p>
<p>Actions can execute dynamically created Executions trees to create Turing Complete systems. Search for 'execute' to see an example of this. Execute allows you to create a Node structure with control mechanisms such as 'if', 'else if', 'raise', 'throw', 'foreach' etc. The intent of the Magix Turing Executor is to allow for the system to become 100% Turing Complete without needing to resort to code, while still keeping enough structure to be capable of completely abstracting it away through visual WYSIWYG type 
of constructs. This combined with the serialization support of Magix for parameters and function 
invocations and results and such, combined with Magix' extreme meta support, makes it relatively easy to create really interesting stuff such as Virtual Reality construction of executable code, which is 'recorded' as the individual opens and closes doors which represents your legal execution logic flow, etc, etc, etc.</p>
<p>All Actions you create will have their Description become an integral part of the documentation.</p>
<p>To summarize, such that you can more easily remember and understand Actions conceptually, Actions are partially serialized function invocations, which can be invoked again and again and again. Parts of their Node structure will automatically be transfered by the invoker, and carry over stuff such as the DataSource of the Active Module etc, while other parts are statically defined through the static Node parameters of your Actions. Any amount of runtime polymorphism can later at any point be added, either by an automatic process, or by the administrator of the server during configuration.</p>
<p>This extremely flexible late binding polymorphism mechanisms of Magix is really what makes Magix Magic.</p>
<p>Extremely easy to understand conceptually, after some initial 'aha' time. And insanely flexible, after the first wave of 'ahas' have passed. Realize though that yours truly is still doing 'ahas' in regards to Actions. Actions will, and should, largely replace your functions and methods. Actions basically replaces every single OOP mechanism you have used in conventional OO languages. You can still use OOP constructs as you wish in Magix, but Actions and Active Events will most of the time become your preferred weapon of choice. Simply because of their superiority to old-style functions and methods.</p>
<p>Yes, they're also extremely fast.</p>
<h4>The different properties of your actions are</h4>
<ul>
<li>Name - Serves as a unique reference for later being able to invoke the Action. The Action's function name, if you wish</li>
<li>Event Name - The Active Event the Action will raise upon invocation</li>
<li>Overrides - If not empty, will become the polymorphistic overridden implementation for the given Active Event, with the same name as the Overrides property</li>
<li>Strip Input Node - If true, will strip the incoming input node from the place which raised the Action</li>
</ul>
<p>If you're not editing a System Action, then you can use the Search button to search for Active Events which you want to wrap your Actions around.</p>
<hr />
<h2>Creating and Invoking Active Events from C#</h2>
<p>When you create an Active Event, you associate it with a name, which is a string literal, which later becomes its 'address'.</p>
<pre>// This code should either be in an Active
// Controller somwehere, or in an Active Module
// If the code is in an Active Controller then 
// it will handle the Active Event every single 
// time it is invoked. If it is in an Active 
// Module it will only be handled in that Active
// Module if the module is loaded, unless you make
// the method which handles your Active Event
// static

[ActiveEvent(Name = ""YourNamespace.PatPitty"")]
protected void YourNamespace_PatPitty(object sender, ActiveEventArgs e)
{
   /* do stuff here. e.Params will contain 
   * all the static Nodes you've got in 
   * your Action in your visual interface, 
   * in addition to that if Strip Input 
   * Node is false, the entire
   * Node structure from the caller, normally 
   * the DataSource or previous Action
   * will be passed on into here too, 
   * with the static parameters overwriting the 
   * dynamic ones if needed.
   */
   PetAKitten(e.Params[""Kitten""].Get&lt;string&gt;());
   e.Params[""Result""].Value = ""Moaw!"";
}

protected void PetAKitten(string kittenName)
{
   /* Kitty, kitty, kitty ... :) */
}


[ ... some other file somewhere, which doesn't 
need to know anything about the component 
implementing the Active Event handler ... ]


// From some Controller somewhere, inheriting from ActiveControll base class ...
// Or any other place referencing Magix.Core, if you'd like to type out the 
// ActiveEvents.Instance.RaiseActiveEvent instead of the shorthands ...
protected void PittyPatty()
{
   Node n = new Node();
   n[""Kitten""].Value = ""My Kitten"";
   RaiseEvent(
      ""YourNamespace.PatPitty"",
      n);
   ShowMessage(e.Params[""Result""].Get&lt;string&gt;());
}

</pre>
<p>The above code is an example of how to create an Active Event, in C#, which again can be serialized to an Action which wraps it.</p>
<p>Simply by realizing that Active Event address units are nothing but strings, you realize you can pass references to methods around, almost like you'd do with a dynamic programming language with lambda support. Except with Magix, you don't need to pass around your Lambda objects, but you can cross-interfere and override at any level, without needing to do changes to code. The entirety of the polymorphism mechanisms are completely runtime configurable, basically ...</p>
<p>By decoupling our polymorphism mechanisms away from our types and objects this way, we achieve exponential productivity growth after some initial learning. Because everything becomes reusable. Just for God's sake, do NOT pass complex types around. If it's not in the .Net System namespace, it probably doesn't belong in the Node structure!</p>
<p>It is considered smart to namespace your Active Event such that they're unique for your company/department/project etc - To avoid namespace clashing.</p>
<hr />
";
            foreach (Action idx in Action.Select())
            {
                string name = idx.Name;
                string evt = idx.EventName;
                string description = idx.Description.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "").Replace("\n", "");
                string shortName = name;
                if (shortName.IndexOf('.') != -1)
                {
                    shortName = shortName.Substring(shortName.LastIndexOf('.') + 1);
                }

                allActionsHtml += "<h2>" + shortName + "</h2>";
                allActionsHtml += "<em>" + name + "</em>";
                allActionsHtml += "<em>" + evt + "</em>";
                allActionsHtml += "<p>" + description + "</p>";
            }
            node["Pages"]["Actions"].Value = allActionsHtml;
        }

        private void GetAllClassesForPages(Node node, int level)
        {
            foreach (Namespace idx in Docs.GetNamespaces(level))
            {
                foreach (Class idxC in idx.GetClasses(level))
                {
                    if (!string.IsNullOrEmpty(idxC.Description))
                    {
                        // Classes each have their own 'section' ...
                        AddClassToNodeForBookDistroPages(idxC, node);
                    }
                }
            }
        }

        private void AddClassToNodeForBookDistroPages(Class cls, Node node)
        {
            string allHtml = "<h1>" + cls.Name + "</h1>";

            System.Type am = Adapter.ActiveModules.Find(
                delegate(System.Type idx)
                {
                    return idx.FullName == cls.FullName;
                });
            if (am != null)
            {
                allHtml += "<codebig>[ActiveModule]</codebig>";
            }

            System.Type ac = PluginLoader.Instance.ActiveControllers.Find(
                delegate(System.Type idx)
                {
                    return idx.FullName == cls.FullName;
                });
            if (ac != null)
            {
                allHtml += "<codebig>[ActiveController]</codebig>";
            }

            System.Type at = Adapter.ActiveTypes.Find(
                delegate(System.Type idx)
                {
                    return idx.FullName == cls.FullName;
                });
            if (at != null)
            {
                allHtml += "<codebig>[ActiveType";
                ActiveTypeAttribute[] atrs =
                    at.GetCustomAttributes(typeof(ActiveTypeAttribute), true) as
                    ActiveTypeAttribute[];
                if (atrs != null && atrs.Length > 0)
                {
                    if (!string.IsNullOrEmpty(atrs[0].TableName))
                    {
                        allHtml += "(";
                        allHtml += "TableName=\"" + atrs[0].TableName + "\"";
                        allHtml += ")";
                    }
                }
                allHtml += "]</codebig>";
            }
            allHtml += "<codebig>" + cls.FullName + "</codebig>";

            allHtml += "<p2>Description: </p2><p>" + cls.Description + "</p>";

            if (cls.BaseTypes != null && cls.BaseTypes.Count > 0)
            {
                allHtml += "<p2>Basetypes</p2>";
                foreach (string idx in cls.BaseTypes)
                {
                    allHtml += "<codesmall>" + Docs.GetTypeByID(idx).FullName + "</codesmall>";
                }
            }

            allHtml += DoClassCTORsForPDF(cls);
            allHtml += DoClassMethodsForPDF(cls);
            allHtml += DoClassEventsForPDF(cls);
            allHtml += DoClassPropertiesForPDF(cls);
            node["Pages"][cls.FullName].Value = allHtml;
        }

        private string DoClassCTORsForPDF(Class cls)
        {
            string allHtml = "";
            foreach (Constructor idx in cls.Constructors)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue;
                allHtml += "<code>";
                allHtml +=
                    idx.AccessModifier + "\t" +
                    idx.ReturnType.Replace("<", "&lt;").Replace(">", "&gt;") + "\t" +
                    idx.Name;
                allHtml += "(";

                bool first = true;
                foreach (Parameter idxP in idx.Parameters)
                {
                    if (first)
                        first = false;
                    else
                        allHtml += ", ";
                    allHtml += idxP.Type.Replace("<", "(").Replace(">", ")") + " " + idxP.Name;
                }
                allHtml += ")";
                allHtml += "</code>";
                allHtml += "<p>" + idx.Description + "</p>";
            }
            if (!string.IsNullOrEmpty(allHtml))
                allHtml = "<sec>Constructors</sec>" + allHtml;
            return allHtml;
        }

        private string DoClassPropertiesForPDF(Class cls)
        {
            string allHtml = "";
            foreach (Property idx in cls.Properties)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue;
                allHtml += "<code>";

                System.Type at = Adapter.ActiveTypes.Find(
                    delegate(System.Type idx2)
                    {
                        return idx2.FullName == cls.FullName;
                    });
                if (at != null)
                {
                    ActiveTypeAttribute[] atrs =
                        at.GetCustomAttributes(typeof(ActiveTypeAttribute), true) as
                        ActiveTypeAttribute[];
                    if (atrs != null && atrs.Length > 0)
                    {
                        PropertyInfo prop = at.GetProperty(
                            idx.Name, 
                            BindingFlags.Instance | 
                            BindingFlags.Public | 
                            BindingFlags.NonPublic);
                        if (prop != null)
                        {
                            ActiveFieldAttribute[] at2 =
                                prop.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as
                                ActiveFieldAttribute[];
                            if (at2 != null && at2.Length > 0)
                            {
                                allHtml += "[ActiveField";
                                allHtml += "(";
                                bool found = false;

                                if (!string.IsNullOrEmpty(at2[0].RelationName))
                                {
                                    allHtml += "RelationName=\"" + at2[0].RelationName + "\"";
                                    found = true;
                                }
                                if (!at2[0].IsOwner)
                                {
                                    if (found)
                                        allHtml += ", ";
                                    allHtml += "IsOwner=false";
                                    found = true;
                                }
                                if (at2[0].BelongsTo)
                                {
                                    if (found)
                                        allHtml += ", ";
                                    allHtml += "BelongsTo=true";
                                    found = true;
                                }
                                allHtml += ")";
                                allHtml += "]\r\n";
                            }
                        }
                    }
                }
                allHtml += " </code><codenomargs>";
                allHtml +=
                    idx.AccessModifier + "\t" +
                    idx.ReturnType.Replace("<", "&lt;").Replace(">", "&gt;") + "\t" +
                    idx.Name +
                    "</codenomargs>";
                allHtml += "<p>" + idx.Description + "</p>";
            }
            if (!string.IsNullOrEmpty(allHtml))
                allHtml = "<sec>Properties</sec>" + allHtml;
            return allHtml;
        }

        private string DoClassEventsForPDF(Class cls)
        {
            string allHtml = "";
            foreach (Event idx in cls.Events)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue;
                allHtml += "<code>";
                allHtml +=
                    idx.AccessModifier + "\t" +
                    idx.ReturnType.Replace("<", "&lt;").Replace(">", "&gt;") + "\t" +
                    idx.Name;
                allHtml += "</code>";
                allHtml += "<p>" + idx.Description + "</p>";
            }
            if (!string.IsNullOrEmpty(allHtml))
                allHtml = "<p2>Events</p2>" + allHtml;
            return allHtml;
        }

        private static string DoClassMethodsForPDF(Class cls)
        {
            string allHtml = "";
            foreach (Method idx in cls.Methods)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue;

                System.Type eh = Adapter.ActiveModules.Find(
                    delegate(System.Type idx2)
                    {
                        return idx2.FullName == cls.FullName;
                    });
                if (eh == null)
                    eh = Adapter.ActiveTypes.Find(
                    delegate(System.Type idx2)
                    {
                        return idx2.FullName == cls.FullName;
                    });
                if (eh == null)
                    eh = PluginLoader.Instance.ActiveControllers.Find(
                    delegate(System.Type idx2)
                    {
                        return idx2.FullName == cls.FullName;
                    });
                if (eh == null)
                {
                    foreach (Assembly idxA in Loader.PluginLoader.PluginAssemblies)
                    {
                        try
                        {
                            foreach (System.Type idxT in idxA.GetTypes())
                            {
                                if (idxT.FullName == cls.FullName)
                                {
                                    eh = idxT;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            ; // Some throws ...
                        }
                    }
                }
                if (eh != null)
                {
                    try
                    {
                        MethodInfo prop = eh.GetMethod(
                                idx.Name,
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.NonPublic | 
                                BindingFlags.Static);
                        if (prop != null)
                        {
                            ActiveEventAttribute[] at2 =
                                prop.GetCustomAttributes(typeof(ActiveEventAttribute), true) as
                                ActiveEventAttribute[];
                            if (at2 != null && at2.Length > 0)
                            {
                                allHtml += "<codenomargsbold>[ActiveEvent(";
                                if (at2[0].Async)
                                {
                                    allHtml += "Async = true, ";
                                }
                                allHtml += "Name = \"" + at2[0].Name + "\")]</codenomargsbold>";
                            }
                        }
                    }
                    catch
                    {
                        // silently catching since some methods might have the same names
                        // at which point they're NOT Event Handlers. That's for sure ... ;)
                    }
                }
                allHtml += "<codenomargs>";
                allHtml +=
                    idx.AccessModifier + " " +
                    idx.ReturnType.Replace("<", "(").Replace(">", ")") + " " +
                    idx.Name.Replace("<", "(").Replace(">", ")");
                allHtml += "(";

                bool first = true;
                foreach (Parameter idxP in idx.Parameters)
                {
                    if (first)
                        first = false;
                    else
                        allHtml += ", ";
                    allHtml += idxP.Type.Replace("<", "(").Replace(">", ")") + " " + idxP.Name;
                }
                allHtml += ")";
                allHtml += "</codenomargs><br/>";
                allHtml += "<p>" + idx.Description + "</p><br /><br />";
            }
            if (!string.IsNullOrEmpty(allHtml))
                allHtml = "<sec>Methods</sec>" + allHtml;
            return allHtml;
        }

        private void GetTipOfTodayPagesForDox(Node node)
        {
            Node xx = new Node();

            RaiseEvent(
                "Magix.Core.GetAllToolTips",
                xx);

            string allTipOfToday = "";
            foreach (Node idx in xx)
            {
                // TipOfToday stores its stuff as HTML ...
                string html = idx.Get<string>();
                allTipOfToday += html;
            }
            node["Pages"]["Introduction"].Value = allTipOfToday;
        }

        private static void CreateIndexItemForMetaActions(Node node)
        {
            node["Index"]["Actions"]["Header"].Value = "Meta Actions";
            node["Index"]["Actions"]["Description"].Value = "Reference documentation about the " +
                "MetaActions that was present, and had documentation within your installation upon " +
                "the generation of this document";
        }

        private void CreateIndexItemsForClasses(Node node, int level)
        {
            foreach (Namespace idx in Docs.GetNamespaces(level))
            {
                if (new List<Doxygen.NET.Type>(idx.GetClasses(level)).Count > 0)
                {
                    foreach (Class idxC in idx.GetClasses(level))
                    {
                        if (!string.IsNullOrEmpty(idxC.Description))
                        {
                            AddClassToNodeForBookDistro(idxC, node);
                        }
                    }
                }
            }
        }

        private static void CreateIntroduction_TipOfToday_IndexItem(Node node)
        {
            node["Index"]["Introduction"]["Header"].Value = "Welcome to Magix!";
            node["Index"]["Introduction"]["Description"].Value = "A short introduction to Magix " +
"and what you can expect from it. Explains the terms and introduces Magix using a Tutorial-Like " +
"communication form. This is the main content parts of the book, while the other parts are more " +
"reference like in structure. Start out here to get an introduction of the different terms if you " +
"are new to Magix, and use the other parts as a Reference Guide later when wondering about specific subjects. " +
"Also remember that there are tons of stuff about Magix on YouTube and on the internet in general in case you're " +
"stuck with a very specific subject";
        }

        private static void SetTitleAndCoverImage(int level, Node node)
        {
            switch (level)
            {
                case 1:
                    node["Title"].Value = "Magix for Noobs!";
                    node["File"].Value = "Tmp/Magix-For-Noobs.pdf";
                    node["FrontPage"].Value = "media/images/book-front-page-noob.png";
                    break;

                case 2:
                    node["Title"].Value = "Magix for 'The Believers'";
                    node["File"].Value = "Tmp/Magix-For-Believers.pdf";
                    node["FrontPage"].Value = "media/images/book-front-page-believer.png";
                    break;

                case 3:
                    node["Title"].Value = "Magix for C#-People";
                    node["File"].Value = "Tmp/Magix-For-CSharp-People.pdf";
                    node["FrontPage"].Value = "media/images/book-front-page-CSharp.png";
                    break;

                case 4:
                    node["Title"].Value = "Magix for 'Believing C#-People'";
                    node["File"].Value = "Tmp/Magix-For-Believing-CSharp-People.pdf";
                    node["FrontPage"].Value = "media/images/book-front-page-CSharp-Believers.png";
                    break;
            }
        }

        private int GetDoxLevel()
        {
            // Getting setting for 'Level of Documentation' [easy, intermediate or advanced or 'super']
            Node doxLevel = new Node();

            doxLevel["Name"].Value = "Magix.Brix.Components.ActiveControllers.Documentation.CurrentLevel";
            doxLevel["Default"].Value = "1";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                doxLevel);
            return int.Parse(doxLevel["Value"].Value.ToString());
        }

        private void AddClassToNodeForBookDistro(Class cls, Node node)
        {
            node["Index"][cls.FullName]["Header"].Value = cls.Name;
            node["Index"][cls.FullName]["Description"].Value = cls.Description;
        }

        private string GetTextForGiveMeMyBookManButton()
        {
            Node node = new Node();

            RaiseEvent(
                "Magix.EasterEgg.GiveMeTextForMyMyBookDarnitButton",
                node);
            return node["Text"].Value.ToString();
        }

        static string[] _bookRandomRamblingsForButton = {
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Give me my Book Man!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Book please ...?",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "Dox in Books [form]",
            "Download Book!",
            "Download Book!",
            "Download Book!",
            "BOOK!",
            "Download Book!",
            "Download Book!",
            "Dox against Forrest!",
            "Download Book!",
            "Dead Tree Distribution!",
            "Dox took the Trees!",
            "Doxygen stuff...",
            "Trees Unite!",
            "Dead K[tr]e[a]nnedies",
            "I Eat at McDonalds",
            "YES! Its the Easter Egg!"
        };

        [ActiveEvent(Name = "Magix.EasterEgg.GiveMeTextForMyMyBookDarnitButton")]
        protected void Magix_EasterEgg_GiveMeTextForMyMyBookDarnitButton(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["Name"].Value = "Magix.Dox.NameForBookButtonIndex";
            node["Default"].Value = "0";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                node);

            int index = int.Parse(node["Value"].Value.ToString());

            e.Params["Text"].Value = _bookRandomRamblingsForButton[index];

            index += 1;
            if (index >= _bookRandomRamblingsForButton.Length)
                index = 0;

            node["Value"].Value = index.ToString();

            RaiseEvent(
                "Magix.Common.SetUserSetting",
                node);
        }

        /*
         * Helper for above ...
         */
        private void LoadToggleDoxLevelButton(int level)
        {
            // Loading 'change level button' ...
            Node node = new Node();

            switch (level)
            {
                case 1:
                    node["Text"].Value = "Intermediary";
                    break;
                case 2:
                    node["Text"].Value = "Code!";
                    break;
                case 3:
                    node["Text"].Value = "Advanced Coding!";
                    break;
                case 4:
                    node["Text"].Value = "Back to [Noob] ...!";
                    break;
            }
            node["ButtonCssClass"].Value = "span-6 down-1";
            node["Append"].Value = true;
            node["Event"].Value = "Magix.Doxygen.RollLevel";

            LoadModule(
                "Magix.Brix.Components.ActiveModules.CommonModules.Clickable",
                "content3",
                node);
        }

        #region [ -- Helper methods for viewing doxygen files -- ]

        /*
         * Helper for above
         */
        private Node AddNamespaceToNode(Namespace idx, Node node)
        {
            string fullName = idx.FullName;
            string[] splits = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Node tmp = node;
            string tmpFullNs = "";
            while (splits.Length > 0)
            {
                string currentNamespace = splits[0];
                if (!string.IsNullOrEmpty(tmpFullNs))
                    tmpFullNs += ".";
                tmpFullNs += currentNamespace;

                tmp = tmp["Items"][currentNamespace];

                AddMicroNamespaceToNode(
                    idx,
                    currentNamespace,
                    tmp,
                    tmpFullNs,
                    fullName.Substring(currentNamespace.Length).Length == 0);

                fullName = fullName.Substring(currentNamespace.Length);

                if (fullName.Length == 0)
                    break;
                else
                    fullName = fullName.Substring(1);

                splits = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return tmp;
        }

        /*
         * Helper for above
         */
        private void AddMicroNamespaceToNode(
            Namespace nameSp,
            string currentNamespace,
            Node node,
            string fullName,
            bool last)
        {
            node["ID"].Value = fullName.Replace(".", "_XX_").Replace("+", "_XQ_");
            node["Name"].Value = currentNamespace;
            if (last)
            {
                if (!string.IsNullOrEmpty(nameSp.Description))
                    node["ToolTip"].Value = nameSp.Description;
                else
                    node["ToolTip"].Value = fullName;
            }
            else
                node["ToolTip"].Value = fullName;
            node["CssClass"].Value = "mux-tree-item-namespace";
        }

        /*
         * Helper for above
         */
        private void AddClassToNode(Class cls, Node outerMostNamespace)
        {
            outerMostNamespace["Items"][cls.FullName]["ID"].Value = cls.FullName.Replace(".", "_XX_").Replace("+", "_XQ_");
            outerMostNamespace["Items"][cls.FullName]["Name"].Value = cls.Name;
            if (!string.IsNullOrEmpty(cls.Description))
                outerMostNamespace["Items"][cls.FullName]["ToolTip"].Value = cls.Description;
            else
                outerMostNamespace["Items"][cls.FullName]["ToolTip"].Value = cls.FullName;
            outerMostNamespace["Items"][cls.FullName]["CssClass"].Value = "mux-tree-item-class";
        }

        #endregion

        /**
         * Rolls level on documentation advanceness one up, til 4, and then back to zero.
         * Reloads Doxygen Documentation class/namespace Tree afterwards
         */
        [ActiveEvent(Name = "Magix.Doxygen.RollLevel")]
        protected void Magix_Doxygen_RollLevel(object sender, ActiveEventArgs e)
        {
            // Getting setting for 'Level of Documentation' [easy, intermediate or advanced or 'super']
            Node xx = new Node();
            xx["Name"].Value = "Magix.Brix.Components.ActiveControllers.Documentation.CurrentLevel";
            xx["Default"].Value = "1";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                xx);

            int level = int.Parse(xx["Value"].Get<string>());

            level += 1;
            if (level >= 5)
                level = 1;

            xx["Value"].Value = level.ToString();

            RaiseEvent(
                "Magix.Common.SetUserSetting",
                xx);

            // Chickening out ... ;)
            RaiseEvent("Magix.MetaType.ViewDoxygenFiles");
        }

        /**
         * Level2: Expects a SelectedItemID which should point to either a Namespace or
         * a Class, and will show this namespace/class features. Such as which
         * dll(s) implements the namespace/class, if a class, which methods it has etc
         */
        [ActiveEvent(Name = "Magix.Doxygen.ViewNamespaceOrClass")]
        protected void Magix_Doxygen_ViewNamespaceOrClass(object sender, ActiveEventArgs e)
        {
            string fullName = e.Params["SelectedItemID"].Get<string>().Replace("_XX_", ".").Replace("_XQ_", "+");

            // Getting setting for 'Level of Documentation' [easy, intermediate or advanced or 'super']
            Node xx = new Node();
            xx["Name"].Value = "Magix.Brix.Components.ActiveControllers.Documentation.CurrentLevel";
            xx["Default"].Value = "1";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                xx);

            int level = int.Parse(xx["Value"].Get<string>(), CultureInfo.InvariantCulture);

            Namespace nSpc = Docs.Namespaces.Find(
                delegate(Namespace idx)
                {
                    return idx.FullName == fullName;
                });
            if(nSpc != null)
            {
                // Namespace
                ViewNamespaceDlls(nSpc);
            }
            else
            {
                // Class 
                Class cls = Docs.GetAllClasses().Find(
                    delegate(Class idx)
                    {
                        return idx.FullName == fullName;
                    });
                if (cls != null)
                    ShowClassDllsAndMethods(cls, level);
            }
        }

        #region [ -- Helper methods for Viewing specific class or Namespace -- ]

        /*
         * Helper for above
         */
        private void ViewNamespaceDlls(Namespace nSpc)
        {
            Node node = new Node();

            node["IsDelete"].Value = false;
            node["NoIdColumn"].Value = true;
            node["Container"].Value = "content4";
            node["Width"].Value = 9;
            node["MarginBottom"].Value = 10;
            node["Last"].Value = true;
            node["IsCreate"].Value = false;
            node["ReuseNode"].Value = true;

            node["FullTypeName"].Value = typeof(Documentation_Controller).FullName + "-META";

            node["Namespace"].Value = nSpc.FullName;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);

            ActiveEvents.Instance.RaiseClearControls("content5");
        }

        /*
         * Helper for above
         */
        private void ShowClassDllsAndMethods(Class cls, int level)
        {
            ViewClassDlls(cls);
            ViewClassMembers(cls, level);
        }

        /*
         * Shows all DLLs that implements class
         */
        private void ViewClassDlls(Class cls)
        {
            // Showing file(s) that contain the class
            Node node = new Node();

            node["IsDelete"].Value = false;
            node["NoIdColumn"].Value = true;
            node["Container"].Value = "content4";
            node["Width"].Value = 9;
            node["MarginBottom"].Value = 10;
            node["Last"].Value = true;
            node["IsCreate"].Value = false;
            node["ReuseNode"].Value = true;

            node["FullTypeName"].Value = typeof(Documentation_Controller).FullName + "-META";

            node["Class"].Value = cls.FullName;

            RaiseEvent(
                "DBAdmin.Form.ViewClass",
                node);
        }

        /*
         * Shows the members of the Class in a Grid plus all documentation in regards to the class
         */
        private void ViewClassMembers(Class cls, int level)
        {
            // Loading 'View Class Details' Module ...
            Node node = new Node();

            node["Width"].Value = 18;
            node["Padding"].Value = 6;
            node["MarginBottom"].Value = 10;
            node["PullTop"].Value = 8;
            node["Last"].Value = true;
            node["Description"].Value = cls.Description;
            if (cls.FullName.IndexOf('.') > 0)
                node["Name"].Value = cls.FullName.Substring(cls.FullName.LastIndexOf('.') + 1);
            else
                node["Name"].Value = cls.FullName;
            node["FullName"].Value = cls.FullName;

            // Showing all Methods of class
            foreach (Method idx in cls.GetMethods(level))
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue; // Don't do methods not described ...

                node["Methods"][idx.ID]["Name"].Value = idx.Name;
                node["Methods"][idx.ID]["Access"].Value = idx.AccessModifier;
                node["Methods"][idx.ID]["Returns"].Value = idx.ReturnType;
                node["Methods"][idx.ID]["Description"].Value =
                    idx.Description
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
                node["Methods"][idx.ID]["DescriptionHTML"].Value =
                    idx.Description
                    .Replace("<para", "<p")
                    .Replace("</para>", "</p>");

                foreach (Parameter idxP in idx.Parameters)
                {
                    node["Methods"][idx.ID]["Pars"][idxP.Name]["Name"].Value = idxP.Name;
                    node["Methods"][idx.ID]["Pars"][idxP.Name]["Type"].Value = idxP.Type;
                }
            }

            LoadModule(
                "Magix.Brix.Components.ActiveModules.Documentation.ShowClassDetails",
                "content5",
                node);
        }

        #endregion

        /**
         * Level2: Handled to make sure we handle the "Documentation_Controller-META" type for the Grid system
         * so that we can see our DLLs
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectTypeNode")]
        protected void DBAdmin_DynamicType_GetObjectTypeNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == 
                typeof(Documentation_Controller).FullName + "-META")
            {
                e.Params["WhiteListColumns"]["Name"].Value = true;
                e.Params["WhiteListColumns"]["ForcedWidth"].Value = 9;
                e.Params["Type"]["Properties"]["Name"]["ReadOnly"].Value = true;
                e.Params["Type"]["Properties"]["Name"]["Header"].Value = "File";
                e.Params["Type"]["Properties"]["Name"]["NoFilter"].Value = true;
            }
        }

        /**
         * Level2: Handled to make sure we handle the "Documentation_Controller-META" type for the Grid system
         * so that we can see our DLLs
         */
        [ActiveEvent(Name = "DBAdmin.DynamicType.GetObjectsNode")]
        protected void DBAdmin_DynamicType_GetObjectsNode(object sender, ActiveEventArgs e)
        {
            if (e.Params["FullTypeName"].Get<string>() == 
                typeof(Documentation_Controller).FullName + "-META")
            {
                List<Assembly> asm = new List<Assembly>();
                foreach (Assembly idx in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (idx.FullName.StartsWith("System") ||
                            idx.FullName.Contains("mscorlib") ||
                            idx.FullName.StartsWith("Microsoft") ||
                            idx.FullName.StartsWith("DotNetOpenAuth"))
                            continue;
                        foreach (System.Type idxT in idx.GetTypes())
                        {
                            if (idxT.FullName.StartsWith("Microsoft"))
                                continue;
                            if (idxT.Namespace == null)
                                continue;
                            if (e.Params.Contains("Namespace"))
                            {
                                if (idxT.Namespace.Contains(e.Params["Namespace"].Get<string>()))
                                {
                                    if (!asm.Exists(
                                        delegate(Assembly idxA)
                                        {
                                            return idxA.FullName == idxT.Namespace;
                                        }))
                                        asm.Add(idx);
                                }
                            }
                            else if (e.Params.Contains("Class"))
                            {
                                if (idxT.FullName == e.Params["Class"].Get<string>())
                                {
                                    if (!asm.Exists(
                                        delegate(Assembly idxA)
                                        {
                                            return idxA.FullName == idxT.Namespace;
                                        }))
                                        asm.Add(idx);
                                }
                            }
                        }
                    }
                    catch
                    {
                        ; // Some will throw ...
                    }
                }
                int idxNo = 1;
                foreach (Assembly idx in asm)
                {
                    e.Params["Objects"]["o-" + idx.FullName]["ID"].Value = idxNo;
                    e.Params["Objects"]["o-" + idx.FullName]["Properties"]["Name"].Value = idx.FullName.Split(',')[0] + ".dll";
                    e.Params["Objects"]["o-" + idx.FullName]["Properties"]["Name"]["ToolTip"].Value = idx.FullName;
                    idxNo += 1;
                }
            }
        }

        /*
         * Will cache our Doxygen.NET objects, such that they're faster available
         */
        private Docs Docs
        {
            get
            {
                // Loading it ONCE and caching it for ALL users. VERY expensive operation ...!
                if (Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] == null)
                {
                    lock (typeof(Documentation_Controller))
                    {
                        if (Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] == null)
                        {
                            Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"] =
                                new Docs(Page.MapPath("~/DoxFiles"));
                        }
                    }
                }
                return (Docs)Page.Application["Magix.Brix.Components.ActiveControllers.Documentation.Doc"];
            }
        }
    }
}
