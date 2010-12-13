/*
 * MagicBrix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicBrix is licensed as GPLv3.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Magic.Brix.Types
{
    public delegate IEnumerable FunctorGetItems();

    public class LazyList<T> : IList<T>, IList
    {
        private List<T> _list;
        private readonly FunctorGetItems _functor;
        private bool _listRetrieved;

        public LazyList(FunctorGetItems functor)
        {
            _functor = functor;
        }

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

        public bool ListRetrieved
        {
            get { return _listRetrieved; }
        }

        public void Sort(Comparison<T> functor)
        {
            FillList();
            _list.Sort(functor);
        }

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

        public void AddRange(IEnumerable<T> collection)
        {
            FillList();
            _list.AddRange(collection);
        }

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

        public int IndexOf(T item)
        {
            FillList();
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            FillList();
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            FillList();
            _list.RemoveAt(index);
        }

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

        public void Add(T item)
        {
            FillList();
            _list.Add(item);
        }

        public void Clear()
        {
            FillList();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            FillList();
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            FillList();
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                FillList();
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            FillList();
            return _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }

        public int Add(object value)
        {
            FillList();
            _list.Add((T)value);
            return IndexOf(value);
        }

        public bool Contains(object value)
        {
            FillList();
            return _list.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            FillList();
            return _list.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            FillList();
            _list.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
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
