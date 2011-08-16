/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
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
        private Adapter _ad;

        public Transaction(Adapter ad)
        {
            _ad = ad;
        }

        public abstract IDbTransaction Trans
        {
            get;
        }

        protected virtual void Rollback()
        {
            Adapter.Instance.InvalidateCache();
            _ad.ResetTransaction();
        }

        public virtual void Commit()
        {
            _comitted = true;
            _ad.ResetTransaction();
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
