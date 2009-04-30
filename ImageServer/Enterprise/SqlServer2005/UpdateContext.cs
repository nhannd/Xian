#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Baseline implementation of <see cref="IUpdateContext"/> for use with ADO.NET and SQL server.
    /// </summary>
    /// <remarks>
    /// This mechanism uses transaction wrappers in ADO.NET.  The transaction is started when the update
    /// context is created.
    /// </remarks>
    public class UpdateContext : PersistenceContext,IUpdateContext
    {
        #region Private Members
        private SqlTransaction _transaction;
        private UpdateContextSyncMode _mode;
        #endregion

        #region Constructors
        internal UpdateContext(SqlConnection connection, ITransactionNotifier transactionNotifier, UpdateContextSyncMode mode)
            : base (connection, transactionNotifier)
        {
            _transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            _mode = mode;
        }
        #endregion

        #region Public Members
        public SqlTransaction Transaction
        {
            get { return _transaction; }
        }
        #endregion

        #region PersistenceContext Overrides
        public override void Suspend()
        {
        }

        public override void Resume()
        { 
        }
        #endregion

        #region IUpdateContext Members

        void IUpdateContext.Commit()
        {
            if (_transaction != null && _transaction.Connection != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            else
                Platform.Log(LogLevel.Error, "Attempting to commit transaction that is invalid");
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    try
                    {
                        if (_transaction.Connection != null)
                            _transaction.Rollback();
                        _transaction = null;
                    }
                    catch (SqlException e)
                    {
                        Platform.Log(LogLevel.Error, e);
                        _transaction = null;
                    }
                }
                // end the transaction
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        #endregion

    }
}
