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
				try
				{
					_transaction.Commit();
				}
				catch (SqlException e)
				{
					// Discovered during 1.5 testing that when an timeout exception happens on
					// a Commit(), the transaction is still committed in the background.  
					// Added in a catch of the timeout exception only (by looking at SqlException.Number == -2 
					// to identify a timeout exception), and also logged a warning message.
					// 
					// Found -2 magic number for timeout here:  
					// http://blog.colinmackay.net/archive/2007/06/23/65.aspx
					// and here:
					// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/2e30b3b1-2481-4a8d-b458-4dd4ec799a3f
					if (e.Number != -2)
						throw;

					Platform.Log(LogLevel.Warn,e,"Timeout encountered on Commit of transaction.  Assuming transaction has completed.");
				}
            	_transaction.Dispose();
                _transaction = null;
            }
            else
            {
                string errorMessage = "Attempting to commit transaction that is invalid. ";
                errorMessage += "Stack Trace: " + Environment.StackTrace;   

                Platform.Log(LogLevel.Error, errorMessage);
            }
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
