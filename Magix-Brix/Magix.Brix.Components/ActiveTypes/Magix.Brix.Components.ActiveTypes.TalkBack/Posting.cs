/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveTypes.TalkBack
{
    /**
     * Level2: One Talkback posting. If Parent is null, this is a top-level posting, and
     * the Children collection might have content. If not, it's a child itself of
     * another top level posting, as in a 'reply'
     */
    [ActiveType]
    public class Posting : ActiveType<Posting>
    {
        /**
         * Level2: The descriptive header of the posting
         */
        [ActiveField]
        public string Header { get; set; }

        /**
         * Level2: The actual content of the posting
         */
        [ActiveField]
        public string Content { get; set; }

        /**
         * Level2: Automatically changed value of when posting was created
         */
        [ActiveField]
        public DateTime When { get; set; }

        /**
         * Level2: Which user created the posting
         */
        [ActiveField(IsOwner=false)]
        public UserBase User { get; set; }

        /**
         * Level2: Only top-level postings have children. These children can be perceived
         * as 'replies' to the 'OP' [Original Poster]
         */
        [ActiveField]
        public LazyList<Posting> Children { get; set; }

        /**
         * Level2: If this is null, this is a top level posting, in which case it might have
         * children
         */
        [ActiveField(BelongsTo = true)]
        public Posting Parent { get; set; }

        /**
         * Level3: Overridden to set the User and When poperties
         */
        public override void Save()
        {
            if (ID == 0 && User == null)
            {
                User = UserBase.Current;
                When = DateTime.Now;
            }
            base.Save();
        }
    }
}
