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
using Magix.Brix.Components.ActiveTypes.Publishing;

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
        public string Folder { get; set; }

        [ActiveField]
        public DateTime Created { get; set; }

        [ActiveField(IsOwner = false)]
        public User User { get; set; }

        [ActiveField]
        public LazyList<Image> Images { get; set; }

        public override void Save()
        {
            if (Folder == null)
                Folder = "media/images/";

            if (User == null && ID == 0)
            {
                User = User.Current;
            }
            if (ID == 0)
            {
                Created = DateTime.Now;
            }

            if (Name == null)
            {
                if (User != null)
                    Name = User.Username; // Defaulting name to username if possible ...
                else
                    Name = "Default name";
            }

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
