/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
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
    public class MSTransaction : Transaction
    {
        SqlTransaction _trans;

        public MSTransaction(SqlConnection connection, Adapter ad)
            : base(ad)
        {
            _trans = connection.BeginTransaction();
        }

        protected override void Rollback()
        {
            base.Rollback();
            _trans.Rollback();
            _trans = null;
        }

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
