/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Globalization;

namespace Magix.Brix.Components.ActiveTypes.MetaForms
{
    /**
     * Level3: Encapsulates the logic for serializing and storing Meta Forms objects
     */
    [ActiveType]
    public class MetaForm : ActiveType<MetaForm>
    {
        /**
         * Level3: Encapsulates a serialized Node item
         */
        [ActiveType]
        public class Node : ActiveType<Node>
        {
            public Node()
            {
                Children = new LazyList<Node>();
            }

            /**
             * Level3: The Name of the Node, must be unique within the collection
             */
            [ActiveField]
            public string Name { get; internal set; }

            /**
             * Level3: The value component of the node, will be transformed into the 
             * TypeName type when serialized into a Magix.Brix.Types.Node structure
             */
            [ActiveField]
            public string Value { get; set; }

            /**
             * Level3: The name of the System type the Value represents
             */
            [ActiveField]
            public string TypeName { get; set; }

            /**
             * Level3: Its children nodes
             */
            [ActiveField]
            public LazyList<Node> Children { get; set; }

            public Node this [string name]
            {
                get
                {
                    foreach (Node idx in Children)
                    {
                        if (idx.Name == name)
                            return idx;
                    }
                    Node tmp = new Node();
                    tmp.Name = name;
                    Children.Add(tmp);
                    return tmp;
                }
            }

            /**
             * Level3: Will find the Node with the Given property name set to 
             * the given property value
             */
            public Node Find(Predicate<Node> functor)
            {
                if (functor(this))
                    return this;
                else
                {
                    foreach (Node idx in Children)
                    {
                        if (functor(idx))
                            return idx;
                    }
                }
                return null;
            }

            public override void Save()
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = "Default - Please Change";
                    Value = "default!";
                    TypeName = typeof(string).FullName;
                }
                this.ValidateUniqueChildren();
                base.Save();
            }

            private void ValidateUniqueChildren()
            {
                if (Children.ListRetrieved)
                {
                    Dictionary<string, bool> tmp = new Dictionary<string, bool>();
                    foreach (Node idx in Children)
                    {
                        if (tmp.ContainsKey(idx.Name))
                            throw new ArgumentException("You cannot name two Nodes the same name in the same level");
                        tmp[idx.Name] = true;
                    }
                }
            }
        }

        // Used as 'cache' ...
        private Magix.Brix.Types.Node _node;

        public MetaForm()
        {
            Form = new Node();
            Form.Name = "root";
            Form.Value = "";
            Form.TypeName = typeof(string).FullName;
        }

        /**
         * Level3: Name of Meta Form
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Level3: Automatically tracked date for when the Meta Form was created
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * Level3: Serialized Nodes, defining the Control Hierarchy, within this Meta Form. 
         * The root Node is the Form Object
         */
        [ActiveField]
        public Node Form { get; private set; }

        /**
         * Level3: Will return the Magix.Brix.Types.Node converted structure, and cache it
         * for future requests
         */
        public Magix.Brix.Types.Node FormNode
        {
            get
            {
                if (_node == null)
                {
                    _node = new Magix.Brix.Types.Node();
                    CreateNode(Form, _node);
                }
                return _node;
            }
        }

        private void CreateNode(Node node, Magix.Brix.Types.Node mNode)
        {
            mNode.Name = node.Name;
            mNode["_ID"].Value = node.ID;

            switch (node.TypeName)
            {
                case "System.Int32":
                    mNode.Value = int.Parse(node.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.Decimal":
                    mNode.Value = decimal.Parse(node.Value, CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    mNode.Value = DateTime.ParseExact(node.Value, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                case "System.Boolean":
                    mNode.Value = bool.Parse(node.Value);
                    break;
                case "System.String":
                default:
                    mNode.Value = node.Value;
                    break;
            }

            foreach (Node idx in node.Children)
            {
                Magix.Brix.Types.Node tmp = new Magix.Brix.Types.Node();
                CreateNode(idx, tmp);
                mNode.Add(tmp);
            }
        }

        public override void Save()
        {
            if (Name == "_ID")
                return; // This are 'cacher nodes' or 'helper nodes' not really 'here' ...

            if (ID == 0)
                Created = DateTime.Now;

            if (string.IsNullOrEmpty(Name))
                Name = "Default, please change";

            string name = Name;

            int idxNo = 2;
            while (CountWhere(
                Criteria.Eq("Name", name),
                Criteria.NotId(ID)) > 0)
            {
                name = Name + " - " + idxNo.ToString();
                idxNo += 1;
            }

            Name = name;

            base.Save();
        }
    }
}
