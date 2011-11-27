/*
 * Magix-BRIX - A Web Application Framework for ASP.NET
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix-BRIX is licensed as GPLv3.
 */

using System;
using System.Web.UI;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using Magix.UX.Widgets;
using Magix.UX.Effects;
using System.Diagnostics;
using Magix.UX;
using Magix.Brix.Data;
using System.Collections.Generic;

namespace Magix.Brix.Components
{
    /**
     * Level3: Helper class for going through cache to retrieve Active Types. Will 
     * cache most queries
     */
    public abstract class ActiveTypeCached<T> : ActiveType<T> where T: class
    {
        public static new T SelectFirst(params Criteria[] args)
        {
            string key = typeof(T).FullName;
            foreach (Criteria idx in args)
            {
                key += idx.PropertyName;
                if (idx.Value != null)
                    key += idx.Value.GetHashCode().ToString();
            }
            Node n = new Node();
            n["Key"].Value = key;
            ActiveEvents.Instance.RaiseActiveEvent(
                typeof(T),
                "Magix.Common.GetFromCache",
                n);
            if (n.Contains("Value"))
                return n["Value"].Value as T;
            T r = ActiveType<T>.SelectFirst(args);
            n["Value"].Value = r;
            ActiveEvents.Instance.RaiseActiveEvent(
                typeof(T),
                "Magix.Common.AddToCache",
                n);
            return r;
        }

        public static new IEnumerable<T> Select(params Criteria[] args)
        {
            bool hasId = false;
            if (args != null && args.Length > 0)
            {
                foreach (Criteria idx in args)
                {
                    if (idx is CritID)
                    {
                        hasId = true;
                        break;
                    }
                }
                if (hasId)
                {
                    foreach (Criteria idx in args)
                    {
                        if (idx is CritID)
                        {
                            T tmp = (T)Adapter.Instance.SelectByID(
                                GetType(typeof(T)),
                                (int)idx.Value);
                            if (tmp != null)
                                yield return tmp;
                        }
                    }
                }
            }
            if (!hasId)
            {
                string key = typeof(T[]).FullName;
                foreach (Criteria idx in args)
                {
                    if (idx != null)
                        key += idx.GetHashCode().ToString();
                }
                Node n = new Node();
                n["Key"].Value = key;
                ActiveEvents.Instance.RaiseActiveEvent(
                    typeof(T),
                    "Magix.Common.GetFromCache",
                    n);
                if (n.Contains("Value"))
                {
                    foreach (T idx in n["Value"].Value as T[])
                    {
                        yield return idx;
                    }
                }
                else
                {
                    List<T> cts = new List<T>();
                    foreach (T idx in Adapter.Instance.Select(GetType(typeof(T)), null, args))
                    {
                        cts.Add(idx);
                    }
                    n["Value"].Value = cts.ToArray();
                    ActiveEvents.Instance.RaiseActiveEvent(
                        typeof(T),
                        "Magix.Common.AddToCache",
                        n);
                    foreach (T idx in n["Value"].Value as T[])
                    {
                        yield return (T)idx;
                    }
                }
            }
        }

        public override void Save()
        {
            base.Save();

            Node n = new Node();
            n["Key"].Value = typeof(T).FullName;
            ActiveEvents.Instance.RaiseActiveEvent(
                typeof(T),
                "Magix.Common.RemoveFromCache",
                n);
            n["Key"].Value = typeof(T[]).FullName;
            ActiveEvents.Instance.RaiseActiveEvent(
                typeof(T),
                "Magix.Common.RemoveFromCache",
                n);
        }
    }
}
