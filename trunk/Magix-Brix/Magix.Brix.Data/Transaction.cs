/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Magix.Brix.Data
{
    public abstract class Transaction : IDisposable
    {
        private bool _disposed;
        private bool _comitted;

        public abstract IDbTransaction Trans
        {
            get;
        }

        protected virtual void Rollback()
        {
            Adapter.Instance.InvalidateCache();
        }

        public virtual void Commit()
        {
            _comitted = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (!_comitted)
                    {
                        Rollback();
                    }
                }
            }
            _disposed = true;
        }
    }
}
