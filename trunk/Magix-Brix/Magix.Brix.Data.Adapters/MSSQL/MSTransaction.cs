/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Magix.Brix.Types;
using Magix.Brix.Data.Internal;
using Magix.Brix.Loader;
using System.Data;

namespace Magix.Brix.Data.Adapters.MSSQL
{
    /**
     * Level4: Implementation of Transaction class for MS SQL Data Adapter
     */
    public class MSTransaction : Transaction
    {
        SqlTransaction _trans;

        public MSTransaction(SqlConnection connection, Adapter ad)
            : base(ad)
        {
            _trans = connection.BeginTransaction();
        }

        /*
         * Rolls back all changes
         */
        protected override void Rollback()
        {
            base.Rollback();
            _trans.Rollback();
            _trans = null;
        }

        /*
         * Commits all changes to the database
         */
        public override void Commit()
        {
            _trans.Commit();
            _trans = null;
            base.Commit();
        }

        public override IDbTransaction Trans
        {
            get { return _trans; }
        }
    }
}
