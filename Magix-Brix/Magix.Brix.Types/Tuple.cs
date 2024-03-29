﻿/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Diagnostics;

namespace Magix.Brix.Types
{
    /**
     * Level3: A helper structure to make it possible to create multiple return values or "couple"
     * objects together. Type is a natural immutable type and hence also 100% thread safe.
     * Though objects contained within the type might not be. This class is something .Net should
     * have had natively, but doesn't [really]
     */
    [Serializable]
    public struct Tuple<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;

        /**
         * Level3: CTOR taking left/first and right/second arguments.
         */
        [DebuggerStepThrough]
        public Tuple(TLeft left, TRight right)
        {
            _left = left;
            _right = right;
        }

        /**
         * Level3: Returns the left or first object
         */
        public TLeft Left
        {
            [DebuggerStepThrough]
            get { return _left; }
        }

        /**
         * Level3: Returns the right or second argument
         */
        public TRight Right
        {
            [DebuggerStepThrough]
            get { return _right; }
        }

        /**
         * Level3: Will return true if the two objects 
         */
        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (obj.GetType() != typeof (Tuple<TLeft, TRight>))
                return false;

            return Equals((Tuple<TLeft, TRight>) obj);
        }

        [DebuggerStepThrough]
        public static bool operator ==(Tuple<TLeft, TRight> left, Tuple<TRight, TLeft> right)
        {
            return left.Equals(right);
        }

        [DebuggerStepThrough]
        public static bool operator !=(Tuple<TLeft, TRight> left, Tuple<TRight, TLeft> right)
        {
            return !left.Equals(right);
        }

        [DebuggerStepThrough]
        public bool Equals(Tuple<TLeft, TRight> other)
        {
            return Equals(other._left, _left) && Equals(other._right, _right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_left.GetHashCode()*397) ^ _right.GetHashCode();
            }
        }
    }
}
