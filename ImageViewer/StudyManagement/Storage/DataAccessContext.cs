#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	/// <summary>
	/// Manages a data-access unit of work, valid for a single transaction.
	/// </summary>
	/// <remarks>
	/// As per this blog post (http://matthewmanela.com/blog/sql-ce-3-5-with-linq-to-sql/), there are
	/// some performance considerations to take into account when using linq-to-sql with sql-ce. Basically
	/// we don't want to allow L2S to manage the connection, but rather we manage it ourselves.
	/// </remarks>
	public class DataAccessContext : IDisposable
	{
		private const string DefaultDatabaseFileName = "dicom_store.sdf";

		private readonly DicomStoreDataContext _context;
		private readonly IDbConnection _connection;
		private readonly IDbTransaction _transaction;
		private bool _transactionCommitted;
		private bool _disposed;

		public DataAccessContext()
		{
			// initialize a connection and transaction
			_connection = CreateConnection();
			_transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
			_context = new DicomStoreDataContext(_connection);
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			if(_disposed)
				throw new InvalidOperationException("Already disposed.");

			_disposed = true;

			if(!_transactionCommitted)
			{
				_transaction.Rollback();
			}

			_context.Dispose();
			_connection.Close();
			_connection.Dispose();
		}

		#endregion

        public ConfigurationBroker GetConfigurationBroker()
        {
            return new ConfigurationBroker(_context);
        }

        public DeviceBroker GetDeviceBroker()
		{
            return new DeviceBroker(_context);
		}

		public WorkItemBroker GetWorkItemBroker()
		{
			return new WorkItemBroker(_context);
		}

        public WorkItemUidBroker GetWorkItemUidBroker()
        {
            return new WorkItemUidBroker(_context);
        }

		public StudyBroker GetStudyBroker()
		{
			return new StudyBroker(_context);
		}

		public RuleBroker GetRuleBroker()
		{
			return new RuleBroker(_context);
		}

        public StudyRootQuery GetStudyRootQuery()
        {
            return new StudyRootQuery(_context);
        }

	    /// <summary>
		/// Commits the transaction.
		/// </summary>
		/// <remarks>
		/// After a successful call to this method, this context instance should be disposed.
		/// </remarks>
		public void Commit()
		{
			if(_transactionCommitted)
				throw new InvalidOperationException("Transaction already committed.");
			_context.SubmitChanges();
			_transaction.Commit();
			_transactionCommitted = true;
		}

		private static IDbConnection CreateConnection()
		{
			return CreateConnection(Path.Combine(Platform.ApplicationDataDirectory, DefaultDatabaseFileName));
		}

		/// <summary>
		/// Creates a connection to the specified database file, creating the database
		/// if it does not exist.
		/// </summary>
		/// <param name="databaseFile"></param>
		/// <returns></returns>
		private static IDbConnection CreateConnection(string databaseFile)
		{
			var connectString = string.Format("Data Source = {0}", databaseFile);

			// create the database if it does not exist
			using (var context = new DicomStoreDataContext(connectString))
			{
				if (!context.DatabaseExists())
				{
					// ensure the parent directory exists before trying to create database
					Directory.CreateDirectory(Path.GetDirectoryName(databaseFile));

					context.CreateDatabase();
				}
			}

			// now we can create a long-lived connection
			var connection = new SqlCeConnection(connectString);
			connection.Open();
			return connection;
		}
	}
}
