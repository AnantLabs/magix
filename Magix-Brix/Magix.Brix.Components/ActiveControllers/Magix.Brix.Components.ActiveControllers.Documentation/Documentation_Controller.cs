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
         * Level2: Will populate the Desktop with the 'Help from Marvin' icon
         */
        [ActiveEvent(Name = "Magix.Publishing.GetDashBoardDesktopPlugins")]
        protected void Magix_Publishing_GetDashBoardDesktopPlugins(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Help"]["Image"].Value = "media/images/desktop-icons/marvin-help.png";
            e.Params["Items"]["Help"]["Shortcut"].Value = "x";
            e.Params["Items"]["Help"]["Text"].Value = "Click to have Marvin Magix Rescue you ... [Key H]";
            e.Params["Items"]["Help"]["CSS"].Value = "mux-desktop-icon mux-help";
            e.Params["Items"]["Help"]["Event"].Value = "Magix.Documentation.LaunchMarvin";
        }

        /**
         * Level2: Will return the menu items needed to start the class browser
         */
        [ActiveEvent(Name = "Magix.Documentation.LaunchMarvin")]
        protected void Magix_Documentation_LaunchMarvin(object sender, ActiveEventArgs e)
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
<p>Meta Actions, or 'Actions' for short, are dynamically created actions within the system, 
either created by the system itself during installation or something, or Actions created
by users, such as you.</p>
<p>These Actions can then later be invoked by some sort of event, for instance a user 
clicking a button, or something similar</p>
<p>Basically, if it can be described as a 'verb', it's probably an Action ... ;)</p>
";
            foreach (Action idx in Action.Select())
            {
                string name = idx.Name;
                string evt = idx.EventName;
                string description = idx.Description;
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

                allHtml += "<code>";
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
                if (eh != null)
                {
                    try
                    {
                        MethodInfo prop = eh.GetMethod(
                                idx.Name,
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.NonPublic);
                        if (prop != null)
                        {
                            ActiveEventAttribute[] at2 =
                                prop.GetCustomAttributes(typeof(ActiveEventAttribute), true) as
                                ActiveEventAttribute[];
                            if (at2 != null && at2.Length > 0)
                            {
                                allHtml += "[ActiveEvent(";
                                if (at2[0].Async)
                                {
                                    allHtml += "Async=true, ";
                                }
                                allHtml += "Name=\"" + at2[0].Name + "\")]";
                            }
                        }
                    }
                    catch
                    {
                        // silently catching since some methods might have the same names
                        // at which point they're NOT Event Handlers. That's for sure ... ;)
                    }
                }
                allHtml += "</code>";

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
                allHtml += "<p>" + idx.Description + "</p>";
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

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
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
                    catch (Exception err)
                    {
                        Node xx = new Node();

                        xx["Message"].Value =
                            "<p>File: " + idx.FullName + "</p>" + 
                            "<p>Message: " + err.Message + "</p>" +
                            "<p class='mux-err-stack-trace'>" + err.StackTrace + "</p>";

                        xx["IsError"].Value = true;

                        RaiseEvent(
                            "Magix.Core.ShowMessage",
                            xx);
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

        /**
         * Will cache our Doxygen.NET objects, such that they're faster available
         */
        public Docs Docs
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
