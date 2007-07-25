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

        internal UpdateContext(SqlConnection connection)
            : base (connection)
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
            throw new Exception("The method or operation is not implemented.");
        }

        void IUpdateContext.Commit()
        {
            _transaction.Commit();
        }

        #endregion
    }
}
