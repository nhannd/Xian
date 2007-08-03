using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    /// <summary>
    /// SQL Server implementation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private String _connectionString = "Data Source=127.0.0.1;User ID=sa;Password=swsoftware;Initial Catalog=ImageServer";
        private ITransactionNotifier _transactionNotifier;

        #region IPersistentStore Members

        public void Initialize()
        {
            
        }

        public void SetTransactionNotifier(ITransactionNotifier transactionNotifier)
        {
            _transactionNotifier = transactionNotifier;
        }

        public IReadContext OpenReadContext()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);
                
                connection.Open();

                return new ReadContext(connection, _transactionNotifier);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
            }
            return null;
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);

                return new UpdateContext(connection, _transactionNotifier, mode);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
            }
            return null;
        }

        #endregion
    }
}
