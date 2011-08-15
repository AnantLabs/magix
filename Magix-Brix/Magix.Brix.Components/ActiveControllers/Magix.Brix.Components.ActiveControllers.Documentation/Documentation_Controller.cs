/*
 * Magix - A Modular-based Framework for building Web Applications 
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

namespace Magix.Brix.Components.ActiveControllers.Documentation
{
    /**
     * Level1: Contains the logic for our 'Class Browser' which can browse all the 
     * classes in the system and make some changes to them by enabling them and
     * disabling them by either overriding specific events or by disabling entire
     * controllers or modules all together
     */
    [ActiveController]
    public class Documentation_Controller : ActiveController
    {
        /**
         * Level2: Will return the menu items needed to start the class browser
         */
        [ActiveEvent(Name = "Magix.Publishing.GetPluginMenuItems")]
        protected void Magix_Publishing_GetPluginMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["Items"]["Admin"]["Caption"].Value = "Admin";

            Node tmp = e.Params["Items"]["Admin"]["Items"]["DoxDotNet"];
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
            node["Top"].Value = 1;
            node["Last"].Value = false;
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

            node["ButtonCssClass"].Value = "span-8 clear-left down-1";
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
            // Getting setting for 'Level of Documentation' [easy, intermediate or advanced or 'super']
            Node xx = new Node();

            xx["Name"].Value = "Magix.Brix.Components.ActiveControllers.Documentation.CurrentLevel";
            xx["Default"].Value = "1";

            RaiseEvent(
                "Magix.Common.GetUserSetting",
                xx);

            int level = int.Parse(xx["Value"].Get<string>(), CultureInfo.InvariantCulture);

            Node node = new Node();
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

            node["Index"]["Introduction"]["Header"].Value = "Introduction";
            node["Index"]["Introduction"]["Description"].Value = "A short introduction to Magix " +
"and what you can expect from it. Explains the terms and introduces Magix using a Tutorial-Like " +
"communication form. This is the main content parts of the book, while the other parts are more " +
"reference like in structure. Start out here to get an introduction of the different terms if you " +
"are new to Magix, and use the other parts as a Reference Guide later when wondering about specific subjects. " +
"Also remember that there are tons of stuff about Magix on YouTube and on the internet in general in case you're "+
"stuck with a very specific subject";

            foreach (Namespace idx in Docs.GetNamespaces(level))
            {
                if (new List<Doxygen.NET.Type>(idx.GetClasses(level)).Count > 0)
                {
                    foreach (Class idxC in idx.GetClasses(level))
                    {
                        AddClassToNodeForBookDistro(idxC, node);
                    }
                }
            }

            node["Index"]["Actions"]["Header"].Value = "Meta Actions";
            node["Index"]["Actions"]["Description"].Value = "Reference documentation about the " +
                "MetaActions that was present, and had documentation within your installation upon " +
                "the generation of this document";

            RaiseEvent(
                "Magix.Core.GetAllToolTips",
                node["Pages"]["Introduction"]);

            foreach (Namespace idx in Docs.GetNamespaces(level))
            {
                if (new List<Doxygen.NET.Type>(idx.GetClasses(level)).Count > 0)
                {
                    foreach (Class idxC in idx.GetClasses(level))
                    {
                        AddClassToNodeForBookDistroPages(idxC, node);
                    }
                }
            }

            RaiseEvent(
                "Magix.PDF.CreatePDF",
                node);
        }

        private void AddClassToNodeForBookDistroPages(Class cls, Node node)
        {
            node["Pages"][cls.FullName]["Header"].Value = cls.FullName;

            string description = cls.Description;

            foreach (Method idx in cls.Methods)
            {
                if(string.IsNullOrEmpty(idx.Description))
                    continue;
                description += "\r\n" + "\r\n";
                description += idx.Name;
                description += "\r\n";
                description += idx.Description;
            }

            node["Pages"][cls.FullName]["Description"].Value = description;
        }

        private void AddClassToNodeForBookDistro(Class cls, Node node)
        {
            node["Index"][cls.FullName]["Header"].Value = cls.FullName;
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
            node["CssClass"].Value = "tree-item-namespace";
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
            outerMostNamespace["Items"][cls.FullName]["CssClass"].Value = "tree-item-class";
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
            node["Top"].Value = 1;
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
            node["Top"].Value = 1;
            node["Last"].Value = true;
            node["IsCreate"].Value = false;
            node["ReuseNode"].Value = true;

            node["FullTypeName"].Value = typeof(Documentation_Controller).FullName + "-META";

            node["Class"].Value = cls.FullName;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
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
                        xx["Message"].Value = err.Message +
                            "File; " +
                            idx.FullName;
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
