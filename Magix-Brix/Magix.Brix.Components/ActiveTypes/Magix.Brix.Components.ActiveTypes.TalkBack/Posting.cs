/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using Magix.Brix.Data;
using Magix.Brix.Types;
using Magix.Brix.Components.ActiveTypes.Users;

namespace Magix.Brix.Components.ActiveTypes.TalkBack
{
    [ActiveType]
    public class Posting : ActiveType<Posting>
    {
        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Content { get; set; }

        [ActiveField]
        public DateTime When { get; set; }

        [ActiveField(IsOwner=false)]
        public UserBase User { get; set; }

        [ActiveField]
        public LazyList<Posting> Children { get; set; }

        [ActiveField(BelongsTo = true)]
        public Posting Parent { get; set; }

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
