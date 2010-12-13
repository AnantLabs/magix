/*
 * MagicBRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBRIX is licensed as GPLv3.
 */

using System;
using Magic.Brix.Data;

namespace HelloWorldTypes
{
    [ActiveType]
    public class Counter : ActiveType<Counter>
    {
        [ActiveField]
        public int Value { get; set; }
    }
}
