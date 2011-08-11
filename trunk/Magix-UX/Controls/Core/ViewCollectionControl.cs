/*
 * Magix - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using Magix.UX.Widgets;

namespace Magix.UX.Widgets.Core
{
    /**
     * Abstract base class for widgets which are supposed to contain only one type
     * of child widgets. Contains a lot of usable shorthands like for instance 
     * Enumerable support, and an index operator overload.
     */
    public abstract class ViewCollectionControl<T> : Panel where T: Control
    {
        /**
         * Enumerable of all the Views inside the control
         */
        public IEnumerable<T> Views
        {
            get
            {
                foreach (Control idx in ViewControlCollection)
                {
                    T tmp = idx as T;
                    if (tmp != null)
                        yield return tmp;
                }
            }
        }

        protected override void AddParsedSubObject(object obj)
        {
            if ((obj is T) || (obj is LiteralControl))
                base.AddParsedSubObject(obj);
            else
                throw new ArgumentException("You tried to add up a child control into a ViewCollectionControl which was not of the type the control would want to accept.");
        }

        /**
         * Shorthand to retrieve a specific View
         */
        public T this[int index]
        {
            get
            {
                int idx = 0;
                foreach (T idxView in Views)
                {
                    if (idx == index)
                        return idxView;
                    idx += 1;
                }
                throw new OverflowException("The View with the specified index doesn't exist inside the control");
            }
        }

        /**
         * Shorthand to retrieve the view with the given value as its ID
         */
        public T this[string ID]
        {
            get
            {
                foreach (T idxView in Views)
                {
                    if (idxView.ID == ID)
                        return idxView;
                }
                throw new OverflowException("The View with the specified ID doesn't exist inside the control");
            }
        }

        protected virtual ControlCollection ViewControlCollection
        {
            get { return Controls; }
        }

        /**
         * Shorthand to retrieve the index of a specific View
         */
        public int this[T view]
        {
            get
            {
                int idxNo = 0;
                foreach (Control idx in Views)
                {
                    if (idx is T)
                    {
                        if (idx.ID == view.ID)
                            return idxNo;
                        idxNo += 1;
                    }
                }
                throw new IndexOutOfRangeException("View doesn't exist within the Control");
            }
        }
    }
}
