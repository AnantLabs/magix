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
    /**
     * Level3: Tutorial: Contains the Entity Object or Active Type as we call them for our Gallery Plugin.
     * Please notice that the Gallery plugin is more here intended as an example of how to create
     * your own C# plugins than being an actually useful plugin. Especially considering you are 
     * literally way more powerful yourself using the Meta Form Builder and such for creating
     * your own types of galleries. Though it serves as a useful example still for how to create
     * your own C# plugins, if the back-end webparts just are not powerful enough for some tasks
     * you need to solve
     */
    [ActiveType]
    public class Gallery : ActiveType<Gallery>
    {
        /**
         * Level3: Encapsulates one single Image in your Gallery. The Gallery class contains a list 
         * of these guys. The way both the Image class and the Gallery class are written out, with 
         * attributes for Activetype and inheriting from ActiveType class, is classic way of 
         * creating serializable types in Magix, at the C# core level
         */
        [ActiveType]
        public class Image : ActiveType<Image>
        {
            /**
             * Level3: Basically just a name, without the folder to an image file
             */
            [ActiveField]
            public string FileName { get; set; }
        }

        /**
         * Level3: The CTOR is needed since otherwise we might get null reference exceptions since otherwise
         * our default value of our Images will be null
         */
        public Gallery()
        {
            Images = new LazyList<Image>();
        }

        /**
         * Level3: The friendly name for our gallery, defaults to the username of the 
         * person who created it
         */
        [ActiveField]
        public string Name { get; set; }

        /**
         * Level3: Which folder on disc, relative from app main path, where all the images in this
         * same gallery exists
         */
        [ActiveField]
        public string Folder { get; set; }

        /**
         * Level3: Typical useful property, you would probably want to add to most of your active types too.
         * Helps us sort on relevance. 98% of the time, new is more relevant than old ...
         */
        [ActiveField]
        public DateTime Created { get; set; }

        /**
         * Level3: The User who created the Gallery, if any. Might be null
         */
        [ActiveField(IsOwner = false)]
        public User User { get; set; }

        /**
         * Level3: The list of Image objects, which again encapsulates the path
         */
        [ActiveField]
        public LazyList<Image> Images { get; set; }

        /**
         * Level3: Should normally contain your domain logic validation logic for your
         * entity types, well known types, domain types, active types - or whatever you want to 
         * call them. Anyway, this is where you would do most of your pre-serialization validation
         */
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
                    Name = "Gallery";
            }

            string name = Name;

            // This is a CLASSIC algorithm for figuring out 'unique names' in Magix
            int idxNo = 2;
            while (CountWhere(
                Criteria.Eq("Name", name),
                Criteria.NotId(ID)) > 0)
            {
                name = Name + " - " + idxNo.ToString();
                idxNo += 1;
            }
            Name = name;
            // Now the name should be unique ... ;)

            base.Save();
        }
    }
}
