/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Loader;

namespace Magix.Brix.Components.ActiveTypes.MetaViews
{
    [ActiveType]
    public class MetaView : ActiveType<MetaView>
    {
        [ActiveType]
        public class MetaViewProperty : ActiveType<MetaViewProperty>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public bool ReadOnly { get; set; }

            [ActiveField]
            public string Description { get; set; }

            [ActiveField]
            public string Action { get; set; }

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

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string TypeName { get; set; }

        [ActiveField]
        public LazyList<MetaViewProperty> Properties { get; set; }

        public override void Save()
        {
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
