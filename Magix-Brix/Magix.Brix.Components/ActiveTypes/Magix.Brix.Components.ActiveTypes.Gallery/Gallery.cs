/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Text;
using System.Collections.Generic;
using Magix.Brix.Data;
using Magix.Brix.Types;

namespace Magix.Brix.Components.ActiveTypes.Gallery
{
    [ActiveType]
    public class Gallery : ActiveType<Gallery>
    {
        [ActiveType]
        public class Image : ActiveType<Image>
        {
            [ActiveField]
            public string FileName { get; set; }
        }

        public Gallery()
        {
            Images = new LazyList<Image>();
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public LazyList<Image> Images { get; set; }

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
    }
}
