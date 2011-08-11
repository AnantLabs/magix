/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Magix.Brix.Types
{
    public delegate IEnumerable FunctorGetItems();

    /**
     * Helper class for Lazy Loading of Child ActiveTypes objects. Basically
     * just a list generic type, that'll not load objects before needed ...
     */
    public class LazyList<T> : IList<T>, IList
    {
        private List<T> _list;
        private readonly FunctorGetItems _functor;
        private bool _listRetrieved;

        /**
         * CTOR taking a delegate to use when items should be fetched
         */
        public LazyList(FunctorGetItems functor)
        {
            _functor = functor;
        }

        /**
         * Default empty CTOR
         */
        public LazyList()
        { }

        private void FillList()
        {
            if (_listRetrieved)
                return;
            _list = new List<T>();
            _listRetrieved = true;
            if (_functor == null)
                return;
            foreach (object tmp in _functor())
            {
                _list.Add((T)tmp);
            }
        }

        /**
         * Returns true if list is already populated
         */
        public bool ListRetrieved
        {
            get { return _listRetrieved; }
        }

        /**
         * Sorts given a Comparison delegate
         */
        public void Sort(Comparison<T> functor)
        {
            FillList();
            _list.Sort(functor);
        }

        /**
         * Traverses the list and returns true if the item you're looking for exists,
         * as in the predicate returns true
         */
        public bool Exists(Predicate<T> functor)
        {
            FillList();
            foreach (T idx in _list)
            {
                if (functor(idx))
                    return true;
            }
            return false;
        }

        /**
         * Traverses the list and returns the first item matching the predicate
         */
        public T Find(Predicate<T> functor)
        {
            FillList();
            foreach (T idx in _list)
            {
                if (functor(idx))
                    return idx;
            }
            return default(T);
        }

        public IEnumerable<T> FindAll(Predicate<T> functor)
        {
            FillList();
            foreach (T idx in _list)
            {
                if (functor(idx))
                    yield return idx;
            }
        }

        /**
         * Adds a range of new items to the list collection
         */
        public void AddRange(IEnumerable<T> collection)
        {
            FillList();
            _list.AddRange(collection);
        }

        /**
         * Removes all items matching the given predicate
         */
        public void RemoveAll(Predicate<T> functor)
        {
            List<T> toBeRemoved = new List<T>();
            foreach (T idx in this)
            {
                if (functor(idx))
                {
                    toBeRemoved.Add(idx);
                }
            }
            foreach (T idx in toBeRemoved)
            {
                Remove(idx);
            }
        }

        /**
         * Returns the index of the given item, if it exists in the list
         */
        public int IndexOf(T item)
        {
            FillList();
            return _list.IndexOf(item);
        }

        /**
         * Inserts a new item at the given position
         */
        public void Insert(int index, T item)
        {
            FillList();
            _list.Insert(index, item);
        }

        /**
         * Removes the item at the given index
         */
        public void RemoveAt(int index)
        {
            FillList();
            _list.RemoveAt(index);
        }

        /**
         * Gets or sets the item at the specific index
         */
        public T this[int index]
        {
            get
            {
                FillList();
                return _list[index];
            }
            set
            {
                FillList();
                _list[index] = value;
            }
        }

        /**
         * Appends a new item to the list
         */
        public void Add(T item)
        {
            FillList();
            _list.Add(item);
        }

        /**
         * Clears the list of all its items
         */
        public void Clear()
        {
            FillList();
            _list.Clear();
        }

        /**
         * Returns true if the specific item exists in the list
         */
        public bool Contains(T item)
        {
            FillList();
            return _list.Contains(item);
        }

        /**
         * Copies the list to the given array, starting at the given offset
         */
        public void CopyTo(T[] array, int arrayIndex)
        {
            FillList();
            _list.CopyTo(array, arrayIndex);
        }

        /**
         * Returns the number of items in our list
         */
        public int Count
        {
            get
            {
                FillList();
                return _list.Count;
            }
        }

        /**
         * Will return false, always!
         */
        public bool IsReadOnly
        {
            get { return false; }
        }

        /**
         * Removes the specific item from the list, returns true if an item was removed.
         * Returns false if the item doesn't exist in the list
         */
        public bool Remove(T item)
        {
            FillList();
            return _list.Remove(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }

        int IList.Add(object value)
        {
            FillList();
            _list.Add((T)value);
            return ((IList)this).IndexOf(value);
        }

        bool IList.Contains(object value)
        {
            FillList();
            return _list.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            FillList();
            return _list.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            FillList();
            _list.Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            _list.Remove((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = (T)value;
            }
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
