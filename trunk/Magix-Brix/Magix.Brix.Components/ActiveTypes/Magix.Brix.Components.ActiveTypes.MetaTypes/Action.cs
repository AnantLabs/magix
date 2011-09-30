/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using System.Reflection;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.MetaTypes
{
    /**
     * Contains the serialized content of the Meta Action system in Magix. One Action is a wrapper around
     * an Active Event, as described in the O2 Architecture Document. Basically, an Action can be thought
     * of as a 'serialized version' of an ActiveEvent being raised, which you then can choose to raise
     * whenever you wish, at the trigger of a button for instance. By creating your own actions, you
     * can effectively implement your own type of logic and flow in Magix without having to code.
     * Every Action must have a unique name, and is often either referenced by its Name or its ID
     * property.
     */
    [ActiveType]
    public class Action : ActiveType<Action>
    {
        /**
         * Used to encapsulate one Action 'parameter' in Magix. Every Action can send
         * a tree structure of 'parameters' when it's being executed to its caller. These
         * parameters can contain anything you wish, and can also be typed. This class encapsulates
         * this logic
         */
        [ActiveType]
        public class ActionParams : ActiveType<ActionParams>
        {
            public ActionParams()
            {
                Children = new LazyList<ActionParams>();
            }

            /**
             * Name of parameter. Becomes the Node's name
             */
            [ActiveField]
            public string Name { get; set; }

            /**
             * Value of parameter. Becomes the Node's Value
             */
            [ActiveField]
            public string Value { get; set; }

            /**
             * TypeName of parameter. Decides what type the Value is, and if it should
             * be converted before Action is raised
             */
            [ActiveField]
            public string TypeName { get; set; }

            /**
             * Children of this parameter
             */
            [ActiveField]
            public LazyList<ActionParams> Children { get; set; }

            [ActiveField(BelongsTo = true)]
            public ActionParams ParentParam { get; set; }

            [ActiveField(BelongsTo = true)]
            public Action ParentAction { get; set; }

            /**
             * Adding default value to TypeName if none is given
             */
            public override void Save()
            {
                if (string.IsNullOrEmpty(TypeName))
                    TypeName = "System.String";

                if (ParentParam != null || 
                    ParentAction != null)
                {
                    if ((ParentParam == null ? ParentAction.Params : ParentParam.Children).Exists(
                        delegate(ActionParams idx)
                        {
                            return idx.Name == this.Name && idx.ID != this.ID && idx.ID != 0;
                        }))
                    {
                        throw new ArgumentException("Sorry buddy, but that name is already taken on this level ...");
                    }
                }

                base.Save();
            }

            /**
             * Performs a deep copy of the Parameter and all its children and returns back to caller
             */
            public ActionParams Clone()
            {
                return DeepCopy(this);
            }

            private ActionParams DeepCopy(ActionParams actionParams)
            {
                ActionParams ret = new ActionParams();

                ret.Name = Name;
                ret.TypeName = TypeName;
                ret.Value = Value;

                foreach (ActionParams idx in Children)
                {
                    ret.Children.Add(idx.Clone());
                }

                return ret;
            }

            public override string ToString()
            {
                return Name + ":" + Value + ":" + Children.Count;
            }
        }

        public Action()
        {
            Params = new LazyList<ActionParams>();
            StripInput = true;
        }

        /**
         * Must be unique. Used to de-reference the action in other places. Normal
         * coding convention should be to add at least your company name as the action's
         * prefix or something, to avoid name clashings
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Friendly description, shown to user, which should explain the action in 'human readable text'
         * for the end user
         */
        [ActiveField]
        public string Description { get; set; }

        /**
         * The Name of the ActiveEvent being raised by this action
         */
        [ActiveField]
        public string EventName { get; set; }

        /**
         * The parameters to the ActiveEvent. Will become serialized as Nodes being passed
         * into the Active Event
         */
        [ActiveField]
        public LazyList<ActionParams> Params { get; set; }

        /**
         * If true, will CUT the incoming node, which might be useful to make sure your nodes doesn't
         * grow into 'Kingdom Come'
         */
        [ActiveField]
        public bool StripInput { get; set; }

        /**
         * Automatically maintained by Magix to give you a date of creation for the record
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * Overridden to make sure name is unique, among other things
         */
        public override void Save()
        {
            if (ID == 0)
                Created = DateTime.Now;

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

        /**
         * Will perform a deep copy of the Action, with all of its parameters, and return back 
         * to caller
         */
        public Action Copy()
        {
            return DeepCopy(this);
        }

        private Action DeepCopy(Action action)
        {
            Action ret = new Action();
            ret.Description = Description;
            ret.EventName = EventName;
            ret.Name = "Copy - " + Name;
            ret.StripInput = StripInput;
            foreach (ActionParams idx in Params)
            {
                ret.Params.Add(idx.Clone());
            }
            return ret;
        }
    }
}
