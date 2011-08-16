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
    /**
     * Level3: Implements transactional support for your updates and inserts. Use through
     * the C# using keyword to get automatic rollbacks. Or implement finally yourself
     * in your code. Remember to call 'Commit' before Transaction is lost, since otherwise.
     * Caputt. Default is Rollback
     */
    public abstract class Transaction : IDisposable
    {
        private bool _disposed;
        private bool _comitted;
        private Adapter _ad;

        /**
         * Level3: NOT meant for accessing directly
         */
        public Transaction(Adapter ad)
        {
            _ad = ad;
        }

        /**
         * Level3: NOT meant for accessing directly
         */
        public abstract IDbTransaction Trans
        {
            get;
        }

        /**
         * Level3: Will do a Rollback on your entire changes to the database. Will also
         * reset the cache
         */
        protected virtual void Rollback()
        {
            Adapter.Instance.InvalidateCache();
            _ad.ResetTransaction();
        }

        /**
         * Will Commit the entire batch of jobs, and release the transaction
         */
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
