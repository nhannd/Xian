#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration
{
	/// <summary>
	/// Manages a data-access unit of work, valid for a single transaction.
	/// </summary>
	/// <remarks>
	/// As per this blog post (http://matthewmanela.com/blog/sql-ce-3-5-with-linq-to-sql/), there are
	/// some performance considerations to take into account when using linq-to-sql with sql-ce. Basically
	/// we don't want to allow L2S to manage the connection, but rather we manage it ourselves.
	/// </remarks>
	internal class DataAccessContext : IDisposable
	{
		private const string DefaultDatabaseFileName = "configuration.sdf";

		private readonly string _databaseFilename;
		private readonly ConfigurationDataContext _context;
		private readonly IDbConnection _connection;
		private readonly IDbTransaction _transaction;
		private bool _transactionCommitted;
		private bool _disposed;

		public DataAccessContext()
			: this(DefaultDatabaseFileName)
		{

		}

		internal DataAccessContext(string databaseFilename)
		{
			// initialize a connection and transaction
			_databaseFilename = databaseFilename;
			_connection = CreateConnection();
			_transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
            _context = new ConfigurationDataContext(_connection);
			//_context.Log = Console.Out;
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");

			_disposed = true;

			if (!_transactionCommitted && _transaction != null)
			{
				_transaction.Rollback();
			}

			_context.Dispose();
			_connection.Close();
			_connection.Dispose();
		}

		#endregion


		public ConfigurationDocumentBroker GetConfigurationDocumentBroker()
		{
            return new ConfigurationDocumentBroker(_context);
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		/// <remarks>
		/// After a successful call to this method, this context instance should be disposed.
		/// </remarks>
		public void Commit()
		{
			if (_transactionCommitted)
				throw new InvalidOperationException("Transaction already committed.");
			_context.SubmitChanges();
			if (_transaction != null)
				_transaction.Commit();
			_transactionCommitted = true;
		}

		private IDbConnection CreateConnection()
		{
			return CreateConnection(_databaseFilename);
		}

		/// <summary>
		/// Creates a connection to the specified database file, creating the database
		/// if it does not exist.
		/// </summary>
		/// <param name="databaseFile"></param>
		/// <returns></returns>
		private IDbConnection CreateConnection(string databaseFile)
		{
			return SqlCeDatabaseHelper<ConfigurationDataContext>.CreateConnection(databaseFile);
		}
	}
}
