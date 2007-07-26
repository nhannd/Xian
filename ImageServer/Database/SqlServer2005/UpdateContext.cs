using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public class UpdateContext : PersistenceContext,IUpdateContext
    {
        private SqlTransaction _transaction;
        private UpdateContextSyncMode _mode;

        internal UpdateContext(SqlConnection connection, ITransactionNotifier transactionNotifier, UpdateContextSyncMode mode)
            : base (connection, transactionNotifier)
        {
            _transaction = connection.BeginTransaction();
        }

        #region PersistenceContext Overrides
        public override void Suspend()
        {
        }

        public override void Resume()
        { 
        }
        #endregion

        #region IUpdateContext Members

        void IUpdateContext.Resume(UpdateContextSyncMode syncMode)
        {
            _mode = syncMode;
            throw new Exception("The method or operation is not implemented.");
        }

        void IUpdateContext.Commit()
        {
            _transaction.Commit();
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
                        _transaction.Commit();
                    }
                    catch (SqlException e)
                    {
                        Platform.Log(e, LogLevel.Error);
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
