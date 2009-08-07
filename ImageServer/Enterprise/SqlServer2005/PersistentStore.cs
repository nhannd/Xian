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
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// SQL Server implementation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private static readonly object _syncRoot = new object();
        private static int _connectionCounter = 0;
        private String _connectionString;
        private ITransactionNotifier _transactionNotifier;
        private int _maxPoolSize;

        #region IPersistentStore Members

		public Version Version
		{
			get
			{
				using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
					IPersistentStoreVersionEntityBroker broker = read.GetBroker<IPersistentStoreVersionEntityBroker>();
					PersistentStoreVersionSelectCriteria criteria = new PersistentStoreVersionSelectCriteria();
					criteria.Major.SortDesc(0);
					criteria.Minor.SortDesc(1);
					criteria.Build.SortDesc(2);
					criteria.Revision.SortDesc(3);

					IList<PersistentStoreVersion> versions = broker.Find(criteria);
					if (versions.Count == 0)
						return null;

					PersistentStoreVersion ver = CollectionUtils.FirstElement(versions);

					return new Version(
						int.Parse(ver.Major),
						int.Parse(ver.Minor),
						int.Parse(ver.Build),
						int.Parse(ver.Revision));
				}
			}
		}

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
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(_connectionString);
                _maxPoolSize = sb.MaxPoolSize;
            }
        }

        public void SetTransactionNotifier(ITransactionNotifier transactionNotifier)
        {
            _transactionNotifier = transactionNotifier;
        }

		private SqlConnection OpenConnection()
		{
			// Needed for retries;
			Random rand = null;

			for (int i = 1;; i++)
			{
				try
				{
					SqlConnection connection = new SqlConnection(_connectionString);
                    connection.Open();
                    connection.Disposed += new EventHandler(Connection_Disposed);
					
                    lock(_syncRoot)
				    {
                        _connectionCounter++;
                        if (SqlServerSettings.Default.ConnectionPoolUsageWarningLevel<=0)
                        {
                            Platform.Log(LogLevel.Warn, "# Max SqlConnection Pool Size={0}, current Db Connections={1}", _maxPoolSize, _connectionCounter);
                        }
                        else if (_connectionCounter > _maxPoolSize / SqlServerSettings.Default.ConnectionPoolUsageWarningLevel)
                        {
                            if (_connectionCounter%3==0)
                            {
								Platform.Log(LogLevel.Warn, "# Max SqlConnection Pool Size={0}, current Db Connections={1}", _maxPoolSize, _connectionCounter);
							}
                        }
                    }
				    return connection;
				}
				catch (SqlException e)
				{
					// The connection failed.  Check the Sql error class 0x14 is for connection failure, let the 
					// other error types through.
					if ((i >= 10) || e.Class != 0x14)
						throw;

					if (rand == null) rand = new Random();

					// Sleep a random amount between 5 and 10 seconds
					int sleepTime = rand.Next(5 * 1000, 10 * 1000);
					Platform.Log(LogLevel.Warn,"Failure connecting to the database, sleeping {0} milliseconds and retrying", sleepTime);
					Thread.Sleep(sleepTime);
				}
			}
		}

        void Connection_Disposed(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                _connectionCounter --;
            }
        }

    	public IReadContext OpenReadContext()
        {
            try
            {
            	SqlConnection connection = OpenConnection();
                
                return new ReadContext(connection, _transactionNotifier);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Exception when opening database connection for reading");

                throw new PersistenceException("Unexpected exception opening database connection for reading", e);
            }
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            try
            {
				SqlConnection connection = OpenConnection();

                return new UpdateContext(connection, _transactionNotifier, mode);
            }
            catch (Exception e)
            {
				Platform.Log(LogLevel.Fatal, e, "Exception when opening database connection for update");

                throw new PersistenceException("Unexpected exception opening database connection for update", e);
            }
            }


    	#endregion
    }
}
