/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.MetaViews
{
    /**
     * Contains the logic for our views in our 'Meta Application System'. A MetaView is
     * something that defines what the user is supposed to see in regards to MetaObjects.
     * Basically the UI 'representation' of your object graphs and properties. A MetaView
     * can become the 'engine' for a WebPart through the MetaView_Single and MetaView_Multiple
     * plugins for Magix
     */
    [ActiveType]
    public class MetaView : ActiveType<MetaView>
    {
        /**
         * One 'property' or 'field' for a MetaView. Each MetaView has several properties, which
         * defines which fields to show/collect from the end user on 'behalf of' the meta object.
         * These properties are defined through this class
         */
        [ActiveType]
        public class MetaViewProperty : ActiveType<MetaViewProperty>
        {
            /**
             * Name of field. Can also contain 'widget typename' and other types of 'logic'
             */
            [ActiveField]
            public string Name { get; set; }

            /**
             * If true, property is read-only
             */
            [ActiveField]
            public bool ReadOnly { get; set; }

            /**
             * Description of property. Often used as tooltips or something similar
             */
            [ActiveField]
            public string Description { get; set; }

            /**
             * "|" separated list of names to actions being raised upon 'clicking' this property
             * somehow. Actions will normally be raised in consecutive order, the order they're 
             * placed inside this property
             */
            [ActiveField]
            public string Action { get; set; }

            /**
             * Which MetaView do we belong to
             */
            [ActiveField(BelongsTo=true)]
            public MetaView MetaView { get; set; }

            internal MetaViewProperty Clone()
            {
                MetaViewProperty ret = new MetaViewProperty();
                ret.Action = Action;
                ret.Description = Description;
                ret.Name = Name;
                ret.ReadOnly = ReadOnly;
                return ret;
            }
        }

        public MetaView()
        {
            Properties = new LazyList<MetaViewProperty>();
        }

        /**
         * Name of MetaView
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * The 'Type Name' of the MetaObject associated with this view. If this is a SingleView for
         * instance, it'll create exclusively types of the given TypeName. If it's a MultiView,
         * it'll exclusively show items of this type. Take care when designing your MetaType hierarchy
         */
        [ActiveField]
        public string TypeName { get; set; }

        /**
         * All the properties for the MetaView
         */
        [ActiveField]
        public LazyList<MetaViewProperty> Properties { get; set; }

        /**
         * Automatically maintained by Magix to give you a date of creation 
         * for the record
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * Overridden to make sure Name is unique, among other things
         */
        public override void Save()
        {
            string name = Name;

            if (ID == 0 || Created == DateTime.MinValue)
                Created = DateTime.Now;

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
         * Will perform a deep-copy on the entire MetaView with all its properties and return
         * to caller
         */
        public MetaView Copy()
        {
            return DeepCopy(this);
        }

        private MetaView DeepCopy(MetaView mv)
        {
            MetaView ret = new MetaView();
            ret.Name = mv.Name;
            ret.TypeName = mv.TypeName;
            foreach (MetaViewProperty idx in Properties)
            {
                MetaViewProperty p = idx.Clone();
                ret.Properties.Add(p);
                p.MetaView = ret;
            }
            return ret;
        }
    }
}
