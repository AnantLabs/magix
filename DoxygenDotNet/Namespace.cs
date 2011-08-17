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

namespace Doxygen.NET
{
    /**
     * Level3: Encapsulates a C# namespace within our documentation system
     */
    public class Namespace : IDocItem
    {
        #region IDocItem Members

        public string ID { get; protected internal set; }

        public string Kind 
        {
            get { return "namespace"; }
        }

        public string FullName { get; protected internal set; }
        public string Description { get; protected set; }

        #endregion

        public Namespace()
        {
            Types = new List<Type>();
        }

        /**
         * Level3: All the types within this namespace. Interfaces, classes etc
         */
        public List<Type> Types { get; protected internal set; }

        /**
         * Level3: All the classes in the namespace
         */
        public List<Type> Classes
        {
            get { return Types.FindAll(FindByKind("class")); }
        }

        /**
         * Level3: Will return all classes with a documentation level 
         * [level1, level2 etc] above or equal to the given level parameter
         */
        public IEnumerable<Type> GetClasses(int level)
        {
            foreach (Class idx in Classes)
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue; // Only returning documented stuff ...

                if (level >= 4)
                {
                    yield return idx; // Returning EVERYTHING ...!!
                }
                else
                {
                    string tmpLevelStr = idx.Description ?? "";
                    if (tmpLevelStr.Length > 6)
                    {
                        tmpLevelStr = tmpLevelStr.Substring(0, 6);
                        switch (tmpLevelStr.ToLowerInvariant())
                        {
                            case "level1":
                                yield return idx;
                                break;
                            case "level2":
                                if (level >= 2)
                                    yield return idx;
                                break;
                            case "level3":
                                if (level >= 3)
                                    yield return idx;
                                break;
                        }
                    }
                }
            }
        }

        /**
         * Level3: Returns all value types in our documentation
         */
        public List<Type> Structs
        {
            get { return Types.FindAll(FindByKind("struct")); }
        }

        /**
         * Level3: Returns all enums in our documentation
         */
        public List<Type> Enums
        {
            get { return Types.FindAll(FindByKind("enum")); }
        }

        /**
         * Level3: Returns all delegates in our documentation
         */
        public List<Type> Delegates
        {
            get { return Types.FindAll(FindByKind("delegate")); }
        }

        private Predicate<Type> FindByKind(string kind)
        {
            return delegate(Type type) { return type.Kind == kind; };
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
