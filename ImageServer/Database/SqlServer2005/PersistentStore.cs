using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

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
        private String _connectionString;
        private ITransactionNotifier _transactionNotifier;

        #region IPersistentStore Members

        public void Initialize()
        {
            // Retrieve the partial connection string named databaseConnection
            // from the application's app.config or web.config file.
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ImageServerConnectString"];

            if (null != settings)
            {
                // Retrieve the partial connection string.
                _connectionString = settings.ConnectionString;
            }
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
                throw new PersistenceException("Unexpected exception opening database connection", e);
            }
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
                throw new PersistenceException("Unexpected exception opening database connection", e);
            }
        }

        #endregion
    }
}
