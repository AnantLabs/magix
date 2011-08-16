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
     * Level3: A type in our documentation system. Struct, interface, class etc
     */
    public class Type : IDocItem
    {
        public Type()
        {
            BaseTypes = new List<string>();
            Members = new List<Member>();
        }

        #region IDocItem Members

        public virtual string ID { get; protected internal set; }
        public virtual string Kind { get; protected internal set; }
        public virtual string FullName { get; protected internal set; }
        public virtual string Description { get; protected internal set; }

        #endregion

        /**
         * Level3: All the nested namespaces within our type
         */
        public virtual Namespace Namespace { get; protected internal set; }

        /**
         * Level3: All our nested types
         */
        public virtual List<Type> NestedTypes { get; protected internal set; }

        /**
         * Level3: All our members. Including properties, methods, fields, enums etc
         */
        public virtual List<Member> Members { get; protected internal set; }

        /**
         * Level3: The base class and interfaces of this class
         */
        public virtual List<string> BaseTypes { get; protected internal set; }

        /**
         * Level3: Returns the name of the type
         */
        public virtual string Name
        {
            get 
            { 
                return FullName.Contains(".") ? 
                    FullName.Remove(0, FullName.LastIndexOf(".") + 1): 
                    FullName; 
            }
        }

        /**
         * Level3: Returns all methods to caller
         */
        public List<Member> Methods
        {
            get { return Members.FindAll(FindByKind("function")); }
        }

        /**
         * Level3: Returns methods above threshold of documentation level back to caller
         */
        public IEnumerable<Member> GetMethods(int level)
        {
            foreach (Member idx in Members.FindAll(FindByKind("function")))
            {
                if (string.IsNullOrEmpty(idx.Description))
                    continue; // Only returning documented stuff ...

                if (level >= 4)
                    yield return idx; // Returning EVERYTHING ...!!

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

        /**
         * Level3: Returns constructors back to caller
         */
        public List<Member> Constructors
        {
            get { return Members.FindAll(FindByKind("ctor")); }
        }

        /**
         * Level3: Returns properties back to caller
         */
        public List<Member> Properties
        {
            get { return Members.FindAll(FindByKind("property")); }
        }

        /**
         * Level3: Returns events back to caller
         */
        public List<Member> Events
        {
            get { return Members.FindAll(FindByKind("event")); }
        }

        /**
         * Level3: Returns member delegates back to caller
         */
        public List<Member> MemberDelegates
        {
            get { return Members.FindAll(FindByKind("memberdelegates")); }
        }

        private Predicate<Member> FindByKind(string kind)
        {
            return delegate(Member member) { return member.Kind == kind; };
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
