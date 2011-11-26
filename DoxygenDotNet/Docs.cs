/*
 * Doxygen.NET - .NET object wrappers for Doxygen
 * Copyright 2009 - Ra-Software AS
 * This code is licensed under the LGPL version 3.
 * 
 * Authors: 
 * Thomas Hansen (thomas@ra-ajax.org)
 * Kariem Ali (kariem@ra-ajax.org)
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Doxygen.NET
{
    /**
     * Level3: Encapsulates the 'entry point' if you wish to Doxygen.NET. Use by
     * instantiating a new Docs item, passing in a folder to the place on the disc
     * where the documentation is, and start traversing your namespaces
     */
    public class Docs
    {
        private XmlDocument _indexXmlDoc;

        /**
         * Level3: A list of all the XML files used in this Docs instance
         */
        public List<FileInfo> XmlFiles { get; set; }

        /**
         * Level3: All directories used in this instance
         */
        public DirectoryInfo XmlDirectory { get; protected set; }

        /**
         * Level3: List of all namespace, flat, in this instance
         */
        public List<Namespace> Namespaces { get; protected set; }

        /**
         * Level3: If yes, will instantly start parsing as instance is created
         */
        public bool EagerParsing { get; set; }

        /**
         * Level3: Will only return namespaces containing classes, having 'Levelx'
         * within their description parts. This is done to filter documentation
         * according to 'difficulty level' all the way from 'For Dummies' to 
         * 'Guru'
         */
        public IEnumerable<Namespace> GetNamespaces(int level)
        {
            // Since namespaces cannot be documented [bug in Doxygen], we'll have to look for the
            // existance of any classes with a smaller level than currently being viewed
            foreach (Namespace idx in Namespaces)
            {
                if (HasClassWithLessThen(level, idx))
                    yield return idx;
            }
        }

        private bool HasClassWithLessThen(int level, Namespace ns)
        {
            foreach (Class idx in ns.Classes)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue; // Only returning documented stuff ...

                if (level >= 4)
                    return true; // Returning EVERYTHING ...!!

                string tmpLevelStr = idx.Description ?? "";
                if (tmpLevelStr.Length > 6)
                {
                    tmpLevelStr = tmpLevelStr.Substring(0, 6);
                    switch (tmpLevelStr.ToLowerInvariant())
                    {
                        case "level1":
                            return true;
                        case "level2":
                            if (level >= 2)
                                return true;
                            break;
                        case "level3":
                            if (level >= 3)
                                return true;
                            break;
                    }
                }
            }
            return false;
        }

        /**
         * Level3: Main CTOR. Will instantiate a new Docs instance and parse the
         * given Doxygen folder for doxygen files, and build a hierarchy of classes, namespaces
         * and such for your usage. PS! Yes, this does take insane amounts of memory. Please
         * do NOT use this module at all unless you need it. Look for memory bottlenecks
         * and such if in doubt
         */
        public Docs(string doxygenXmlOuputDirectoryPath)
        {
            if (!Directory.Exists(doxygenXmlOuputDirectoryPath))
                throw new Exception("The specified directory does not exist.");

            XmlDirectory = new DirectoryInfo(doxygenXmlOuputDirectoryPath);

           if (!File.Exists(Path.Combine(XmlDirectory.FullName, "index.xml")))
                throw new Exception("The specified directory does not contain an essential file, \"index.xml\".");

           EagerParsing = true;

            _indexXmlDoc = new XmlDocument();
            _indexXmlDoc.Load(Path.Combine(XmlDirectory.FullName, "index.xml"));

            XmlFiles = new List<FileInfo>(XmlDirectory.GetFiles("*.xml"));

            LoadNamespaces();
        }

        /**
         * Level3: Returns the given Namespace instance
         */
        public Namespace GetNamespaceByName(string namespaceName)
        {
            return Namespaces.Find(delegate(Namespace n) { return n.FullName == namespaceName; });
        }

        /**
         * Level3: Returns the given type
         */
        public Type GetTypeByName(string typeFullName)
        {
            string namespaceName = typeFullName.Remove(typeFullName.LastIndexOf("."));
            Namespace nspace = GetNamespaceByName(namespaceName);

            if (nspace != null)
            {
                Type type = nspace.Types.Find(delegate(Type t) { return t.FullName == typeFullName; });
                return type;
            }

            return null;
        }

        /**
         * Level3: Returns the given type by id
         */
        public Type GetTypeByID(string id)
        {
            foreach (Namespace nspace in Namespaces)
            {
                Type type = nspace.Types.Find(delegate(Type t) { return t.ID == id; });
                if (type != null)
                    return type;
            }
            return null;
        }

        // TODO: Implement yield return here one of these days. Bad bizniss to build up such
        // a huge list ...!
        /**
         * Level3: Returns all classes in a list
         */
        public List<Class> GetAllClasses()
        {
            List<Class> classes = new List<Class>();
            foreach (Namespace nspace in Namespaces)
            {
                foreach (Class c in nspace.Classes)
                {
                    classes.Add(c);
                }
            }
            return classes;
        }

        private void LoadNamespaces()
        {
            Namespaces = new List<Namespace>();
            XmlNodeList namespaceXmlNodes = _indexXmlDoc.SelectNodes("/doxygenindex/compound[@kind=\"namespace\"]");

            foreach (XmlNode namespaceXmlNode in namespaceXmlNodes)
            {
                Namespace nspace = new Namespace();
                nspace.ID = namespaceXmlNode.Attributes["refid"].Value;
                nspace.FullName = namespaceXmlNode["name"].InnerText.Replace("::", ".");

                if (EagerParsing)
                {
                    LoadTypes(nspace, false);
                }
                Namespaces.Add(nspace);
            }
            Namespaces.Sort(
                delegate(Namespace left, Namespace right)
                {
                    return left.FullName.CompareTo(right.FullName);
                });
        }

        private void LoadTypes(Namespace nspace, bool forceReload)
        {
            if (!forceReload && nspace.Types.Count > 0)
                return;

            nspace.Types = new List<Type>();

            XmlNodeList typesXmlNodes = _indexXmlDoc.SelectNodes(
                "/doxygenindex/compound[@kind=\"class\" or @kind=\"interface\" or @kind=\"enum\" or @kind=\"struct\" or @kind=\"delegate\"]");

            foreach (XmlNode typeXmlNode in typesXmlNodes)
            {
                string typeName = typeXmlNode["name"].InnerText.Replace("::", ".");
                if (typeName.Contains(".") && typeName.Remove(typeName.LastIndexOf(".")) == nspace.FullName)
                {
                    Type t = CreateNewType(typeXmlNode.Attributes["kind"].Value);

                    t.ID = typeXmlNode.Attributes["refid"].Value;
                    t.Kind = typeXmlNode.Attributes["kind"].Value;
                    t.FullName = typeName;
                    t.Namespace = nspace;

                    if (EagerParsing)
                    {
                        LoadTypesMembers(t, false);
                    }
                    nspace.Types.Add(t);
                }
            }
            nspace.Types.Sort(
                delegate(Type left, Type right)
                {
                    int leftLev = 5;
                    int rightLev = 5;

                    if (left.Description.Contains("Level"))
                    {
                        leftLev = int.Parse(
                            left.Description.Substring(
                                left.Description.IndexOf("Level") + 5, 1));
                    }
                    if (right.Description.Contains("Level"))
                    {
                        rightLev = int.Parse(
                            right.Description.Substring(
                                right.Description.IndexOf("Level") + 5, 1));
                    }
                    return leftLev.CompareTo(rightLev);
                });
        }
        
        private void LoadTypesMembers(Type t, bool forceReload)
        {
            if (!forceReload && t.Members.Count > 0)
                return;

            FileInfo typeXmlFile = XmlFiles.Find(
                delegate(FileInfo file) 
                { 
                    return file.Name.Remove(file.Name.LastIndexOf(file.Extension)) == t.ID; 
                });

            if (typeXmlFile == null || !typeXmlFile.Exists)
                return;

            XmlDocument typeDoc = new XmlDocument();
            typeDoc.Load(typeXmlFile.FullName);

            t.Description = typeDoc.SelectSingleNode("/doxygen/compounddef/detaileddescription").InnerText;

            XmlNodeList baseTypes = typeDoc.SelectNodes("/doxygen/compounddef/basecompoundref");
            
            if (baseTypes != null)
            {
                foreach (XmlNode baseType in baseTypes)
                {
                    if (baseType.Attributes.GetNamedItem("refid") != null)
                        t.BaseTypes.Add(baseType.Attributes["refid"].Value);
                }
            }

            XmlNodeList members = typeDoc.SelectNodes("/doxygen/compounddef/sectiondef/memberdef");
            
            foreach (XmlNode member in members)
            {
                string kind = member.Attributes["kind"].Value;
                string name = member["name"].InnerText;
                string args = member["argsstring"] != null ? 
                    member["argsstring"].InnerText.Replace("(", "").Replace(")", "").Trim() :
                    string.Empty;

                List<Parameter> parameters = new List<Parameter>();

                if (!string.IsNullOrEmpty(args) && kind == "function")
                {
                    string[] argsSplits = args.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string arg in argsSplits)
                    {
                        string[] argParts = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Parameter p = new Parameter();
                        p.Type = argParts[0].Trim();
                        p.Name = argParts[1].Trim();
                        parameters.Add(p);
                    }
                }
                
                if (kind == "function" && name == t.Name)
                    kind = "ctor";

                Member m = CreateNewMember(kind);

                if (parameters != null && parameters.Count > 0)
                    (m as Method).Parameters = parameters;

                m.ID = member.Attributes["id"].Value;
                m.FullName = string.Format("{0}.{1}", t.FullName, name);
                m.Name = name;
                m.Kind = kind;
                m.Description = member["detaileddescription"].InnerText;
                m.AccessModifier = member.Attributes["prot"].Value;
                m.Parent = t;
                m.ReturnType = member["type"] != null ?
                    member["type"].InnerText : string.Empty;
                t.Members.Add(m);
            }
        }

        private Type CreateNewType(string kind)
        {
            switch (kind)
            {
                case "class":
                    return new Class();
                case "interface":
                    return new Interface();
                case "delegate":
                    return new Delegate();
                case "enum":
                    return new Enum();
                case "struct":
                    return new Struct();
            }
            return new Type();
        }

        private Member CreateNewMember(string kind)
        {
            switch (kind)
            {
                case "property":
                    return new Property();
                case "event":
                    return new Event();
                case "function":
                    return new Method();
                case "variable":
                    return new Field();
                case "ctor":
                    return new Constructor();
                case "memberdelegates":
                    return new MemberDelegate();
            }
            return new Member();
        }
    }
}
