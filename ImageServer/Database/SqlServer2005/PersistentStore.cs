using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    /// <summary>
    /// SQL Server implemenation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private String _connectionString;

        #region IPersistentStore Members

        public void Initialize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetTransactionNotifier(ITransactionNotifier transactionNotifier)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IReadContext OpenReadContext()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);

                return new ReadContext(connection);
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Fatal);
            }
            return null;
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);

                return new UpdateContext(connection);
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Fatal);
            }
            return null;
        }

        #endregion
    }
}
