/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;
using Magix.Brix.Data;

namespace Magix.Brix.Components.ActiveTypes
{
    /**
     * Level3: Helper class for Tip [Today's Tips] logic
     */
    public sealed class TipOfToday
    {
        /**
         * Level4: One actual Tip
         */
        [ActiveType]
        public class Tip : ActiveType<Tip>
        {
            /**
             * Level4: The actual text of the tip
             */
            [ActiveField]
            public string Value { get; set; }

            /**
             * Level4: Its number in regards to being displayed
             */
            [ActiveField]
            public int No { get; set; }
        }

        /**
         *Level4:  Normally stored on a per user level to remember which tip the
         * user has seen last, so we know which one to show him next
         */
        [ActiveType]
        public class TipPosition : ActiveType<TipPosition>
        {
            /**
             * Level4: Actual position of 'current tip', corresponds to No in Tip
             */
            [ActiveField]
            public int Position { get; set; }

            /**
             * Level4: Normally a 'per user unique key' for filtering in regards to
             * different users
             */
            [ActiveField]
            public string Seed { get; set; }
        }

        private static TipOfToday _instance;

        private TipOfToday()
        { }

        /**
         * Level3: Getter to gain access to today's tips
         */
        public static TipOfToday Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(TipOfToday))
                    {
                        if (_instance == null)
                        {
                            _instance = new TipOfToday();
                        }
                    }
                }
                return _instance;
            }
        }

        /**
         * Level3: Only way to create a tip form the outside
         */
        public void CreateTip(string tip)
        {
            Tip t = new Tip();
            t.Value = tip;
            t.No = Tip.Count;
            t.Save();
        }

        /**
         * Level3: Number of tips in database
         */
        public int Count
        {
            get { return Tip.Count; }
        }

        private static string GetToolTip(string seed, int addition)
        {
            TipPosition pos = TipPosition.SelectFirst(
                Criteria.Eq("Seed", seed));
            if (pos == null)
            {
                pos = new TipPosition();
                pos.Position = 0 - addition;
                pos.Seed = seed;
            }
            if (addition != 0)
            {
                pos.Position = pos.Position + addition;
                if (pos.Position >= Tip.Count)
                    pos.Position = 0;
                if (pos.Position < 0)
                    pos.Position = Tip.Count - 1;
            }
            Tip retVal = Tip.SelectFirst(Criteria.Eq("No", pos.Position));
            if (retVal == null)
            {
                // Defaulting to first ...
                pos.Position = 0;
                retVal = Tip.SelectFirst(Criteria.Eq("No", pos.Position));
                if (retVal == null)
                {
                    pos.Save();
                    return null; // No tips here .....
                }
            }
            pos.Save();
            return retVal.Value;
        }

        /**
         * Level3: Will return the 'previous' tip with the given seed
         */
        public string Previous(string seed)
        {
            return GetToolTip(seed, -1);
        }

        /**
         * Level3: Will return the 'next' tip with the given seed
         */
        public string Next(string seed)
        {
            return GetToolTip(seed, 1);
        }

        /**
         * Level3: Will return the 'current' tip with the given seed, and not in any ways
         * increament or decrement the 'currently viewed tips' like the sibling
         * methods will
         */
        public string Current(string seed)
        {
            return GetToolTip(seed, 0);
        }
    }
}