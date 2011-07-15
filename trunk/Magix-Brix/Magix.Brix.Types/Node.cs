/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace Magix.Brix.Types
{
    /**
     * Helper class to pass around data in a "JSON kind of way" without having
     * to convert to JSON strings. Create a new instance, and just start appending
     * items to it like this;
     * <pre>
     *    Node n = new Node();
     *    n["Customer"]["Name"] = "John Doe";
     *    n["Customer"]["Adr"] = "NY";
     * </pre>
     */
    [Serializable]
    public class Node : IList<Node>
    {
        // Implementation of list
        private readonly List<Node> _children = new List<Node>();

        // Parent node
        private Node _parent;

        private string _name;
        private object _value;

        /**
         * Default CTOR, creates a new node with no name and no value and no children
         */
        [DebuggerStepThrough]
        public Node()
            : this(null)
        { }

        /**
         * Creates a new node with the given name
         */
        [DebuggerStepThrough]
        public Node(string name)
            : this(name, null)
        { }

        /**
         * Creates a new node with the given name and the given value
         */
        [DebuggerStepThrough]
        public Node(string name, object value)
            : this(name, value, null)
        { }

        [DebuggerStepThrough]
        private Node(string name, object value, Node parent)
        {
            _name = name;
            _value = value;
            _parent = parent;
        }

        [DebuggerStepThrough]
        public static Node FromJSONString(string json)
        {
            List<string> tokens = ExtractTokens(json);
            if (tokens.Count == 0)
                throw new ArgumentException("JSON string was empty");
            if (tokens[0] != "{")
                throw new ArgumentException("JSON string wasn't an object, missing opening token at character 0");
            int idxToken = 1;
            Node retVal = new Node();
            while (tokens[idxToken] != "}")
            {
                ParseToken(tokens, ref idxToken, retVal);
            }
            return retVal;
        }

        [DebuggerStepThrough]
        private static void ParseToken(IList<string> tokens, ref int idxToken, Node node)
        {
            if (tokens[idxToken] == "{")
            {
                // Opening new object...
                Node next = new Node();
                node._children.Add(next);
                next._parent = node;
                idxToken += 1;
                while (tokens[idxToken] != "}")
                {
                    ParseToken(tokens, ref idxToken, next);
                }
            }
            else if (tokens[idxToken] == ":")
            {
                switch(tokens[idxToken - 1])
                {
                    case "\"Name\"":
                        node.Name = tokens[idxToken + 1].Replace("\\\"", "\"").Trim('"');
                        break;
                    case "\"Value\"":
                        node.Value = tokens[idxToken + 1].Replace("\\\"", "\"").Trim('"');
                        break;
                    case "\"Children\"":
                        idxToken += 1;
                        while (tokens[idxToken] != "]")
                        {
                            ParseToken(tokens, ref idxToken, node);
                        }
                        break;
                }
            }
            idxToken += 1;
            return;
        }

        [DebuggerStepThrough]
        private static List<string> ExtractTokens(string json)
        {
            List<string> tokens = new List<string>();
            for (int idx = 0; idx < json.Length; idx++)
            {
                switch (json[idx])
                {
                    case '{':
                    case '}':
                    case ':':
                    case ',':
                    case '[':
                    case ']':
                        tokens.Add(new string(json[idx], 1));
                        break;
                    case '"':
                        {
                            string str = "\"";
                            idx += 1;
                            while (true)
                            {
                                if (json[idx] == '"' && str.Substring(str.Length - 1, 1) != "\\")
                                {
                                    break;
                                }
                                str += json[idx];
                                idx += 1;
                            }
                            str += "\"";
                            tokens.Add(str);
                        } break;
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;
                    default:
                        throw new ArgumentException(
                            string.Format(
                                "Illegal token found in JSON string at character {0}, token was {1}, string was; \"{2}\"",
                                idx,
                                json[idx],
                                json.Substring(Math.Max(0, idx - 5))));
                }
            }
            return tokens;
        }

        /**
         * Returns the Parent node of the current node in the hierarchy
         */
        public Node Parent
        {
            [DebuggerStepThrough]
            get { return _parent; }
        }

        /**
         * Returns the name of the node
         */
        public string Name
        {
            [DebuggerStepThrough]
            get { return _name; }
            [DebuggerStepThrough]
            private set { _name = value; }
        }

        /**
         * Returns the value of the object
         */
        public object Value
        {
            [DebuggerStepThrough]
            get { return _value; }
            [DebuggerStepThrough]
            set { _value = value; }
        }

        /**
         * Returns the value of the object to type of T
         */
        [DebuggerStepThrough]
        public T Get<T>()
        {
            return _value == null ? default(T) : (T)_value;
        }

        /**
         * Returns the value of the object to type of T, and if
         * object cannot for some reason be converted will return 
         * the "default" value...
         */
        [DebuggerStepThrough]
        public T Get<T>(T defaultValue)
        {
            if (_value == null)
                return defaultValue;
            return (T)_value;
        }

        [DebuggerStepThrough]
        public T Get<T>(T defaultValue, bool force)
        {
            if (string.IsNullOrEmpty(_value as string))
                return defaultValue;
            return (T)_value;
        }

        /**
         * Returns the first node that matches the given Predicate
         */
        [DebuggerStepThrough]
        public Node Find(Predicate<Node> functor)
        {
            if (functor(this))
                return this;
            foreach (Node idx in this)
            {
                if (functor(idx))
                    return idx;
                Node tmp = idx.Find(functor);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        /**
         * Returns true if node exists in children collection
         */
        [DebuggerStepThrough]
        public bool Exists(Predicate<Node> functor)
        {
            return Exists(functor, false);
        }

        /**
         * Returns true if node exists in children collection
         */
        [DebuggerStepThrough]
        public bool Exists(Predicate<Node> functor, bool flat)
        {
            if (!flat && functor(this))
                return true;
            foreach (Node idx in this._children)
            {
                if (functor(idx))
                    return true;
                if (flat)
                    continue;
                bool tmp = idx.Exists(functor);
                if (tmp)
                    return true;
            }
            return false;
        }

        /**
         * Will "disconnect" the node from its parent node. Useful for parsing subtrees
         * where you're dependant upon the DNA code or something...
         */
        [DebuggerStepThrough]
        public Node UnTie()
        {
            _parent.Remove(this);
            _parent = null;
            return this;
        }

        /**
         * Returns the node with the given Name. If that node doesn't exist
         * a new node will be created with the given name and appended into the
         * Children collection and then be returned.
         */
        public Node this[string name]
        {
            [DebuggerStepThrough]
            get
            {
                Node retVal = _children.Find(
                    delegate(Node idx)
                    {
                        return idx.Name == name;
                    });
                if (retVal == null)
                {
                    retVal = new Node(name, null, this);
                    _children.Add(retVal);
                }
                return retVal;
            }
            [DebuggerStepThrough]
            set
            {
                int idxNo = -1;
                if (_children.Exists(
                    delegate(Node idx)
                    {
                        idxNo += 1;
                        return idx.Name == name;
                    }))
                {
                    _children[idxNo] = value;
                }
                else
                    _children.Add(value);
            }
        }

        public Node this[string name, bool forceCreation]
        {
            [DebuggerStepThrough]
            get
            {
                if (forceCreation)
                    return this[name];
                Node retVal = _children.Find(
                    delegate(Node idx)
                    {
                        return idx.Name == name;
                    });
                return retVal;
            }
        }

        [DebuggerStepThrough]
        public int IndexOf(Node item)
        {
            return _children.IndexOf(item);
        }

        [DebuggerStepThrough]
        public void Insert(int index, Node item)
        {
            _children.Insert(index, item);
            item._parent = this;
        }

        [DebuggerStepThrough]
        public void RemoveAt(int index)
        {
            _children[index]._parent = null;
            _children.RemoveAt(index);
        }

        public Node this[int index]
        {
            [DebuggerStepThrough]
            get
            {
                return _children[index];
            }
            [DebuggerStepThrough]
            set
            {
                _children[index] = value;
                value._parent = this;
            }
        }

        [DebuggerStepThrough]
        public void Add(Node item)
        {
            _children.Add(item);
            item._parent = this;
        }

        [DebuggerStepThrough]
        public void AddRange(IEnumerable<Node> items)
        {
            foreach (Node idx in items)
            {
                Add(idx);
            }
        }

        [DebuggerStepThrough]
        public void Clear()
        {
            foreach (Node idx in _children)
            {
                idx._parent = null;
            }
            _children.Clear();
        }

        [DebuggerStepThrough]
        public bool Contains(Node item)
        {
            return _children.Contains(item);
        }

        [DebuggerStepThrough]
        public bool Contains(string itemName)
        {
            return _children.Exists(
                delegate(Node idx)
                {
                    return idx.Name == itemName;
                });
        }

        [DebuggerStepThrough]
        public void CopyTo(Node[] array, int arrayIndex)
        {
            foreach (Node idx in _children)
            {
                idx._parent = null;
            }
            _children.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            [DebuggerStepThrough]
            get { return _children.Count; }
        }

        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        [DebuggerStepThrough]
        public bool Remove(Node item)
        {
            bool retVal = _children.Remove(item);
            if (retVal)
                item._parent = null;
            return retVal;
        }

        [DebuggerStepThrough]
        public IEnumerator<Node> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            string retVal = "";
            retVal += _name;
            if (_value != null)
                retVal += ":" + _value;
            if (_children != null)
                retVal += ":" + _children.Count;
            return retVal;
        }

        [DebuggerStepThrough]
        public string ToJSONString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool hasChild = false;
            if(!string.IsNullOrEmpty(Name))
            {
                builder.AppendFormat(@"""Name"":""{0}""", Name);
                hasChild = true;
            }
            if(Value != null)
            {
                if (hasChild)
                    builder.Append(",");
                string value = "";
                switch(Value.GetType().FullName)
                {
                    case "System.String":
                        value = Value.ToString().Replace("\"", "\\\"");
                        break;
                    case "System.DateTime":
                        value = ((DateTime)Value).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                        break;
                    default:
                        value = Value.ToString();
                        break;
                }
                builder.AppendFormat(@"""Value"":""{0}""", value);
                hasChild = true;
            }
            if(_children.Count > 0)
            {
                if (hasChild)
                    builder.Append(",");
                builder.Append(@"""Children"":[");
                bool first = true;
                foreach(Node idx in _children)
                {
                    if (!first)
                        builder.Append(",");
                    first = false;
                    builder.Append(idx.ToJSONString());
                }
                builder.Append("]");
            }
            builder.Append("}");
            return builder.ToString();
        }

        [DebuggerStepThrough]
        public void Sort(Comparison<Node> del)
        {
            _children.Sort(del);
        }
    }
}
