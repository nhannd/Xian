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
using System.Threading;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
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
		public const string WorkItemMutex = "WorkItem";


		private const string DefaultDatabaseFileName = "dicom_store.sdf";

        private readonly string _databaseFilename;
		private readonly DicomStoreDataContext _context;
		private readonly IDbConnection _connection;
		private readonly IDbTransaction _transaction;
		private bool _transactionCommitted;
		private bool _disposed;
		private Mutex _mutex;

		public DataAccessContext()
			:this(null)
		{
			
		}

        public DataAccessContext(string mutexName)
			:this(mutexName, DefaultDatabaseFileName)
        {
        }

		internal DataAccessContext(string mutexName, string databaseFilename)
		{
            if (!string.IsNullOrEmpty(mutexName))
            {
                _mutex = new Mutex(false, string.Format("{0}:{1}", typeof (DataAccessContext).FullName, mutexName));
                _mutex.WaitOne();
            }

			// initialize a connection and transaction
			_databaseFilename = databaseFilename;
            _connection = CreateConnection();
            _transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
            _context = new DicomStoreDataContext(_connection);
			//_context.Log = Console.Out;
		}

	    #region Implementation of IDisposable

		public void Dispose()
		{
			if(_disposed)
				throw new InvalidOperationException("Already disposed.");

			_disposed = true;

			if(!_transactionCommitted && _transaction != null)
			{
				_transaction.Rollback();
			}

			_context.Dispose();
			_connection.Close();
			_connection.Dispose();

            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex = null;
            }
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

        public StudyStoreQuery GetStudyStoreQuery()
        {
            return new StudyStoreQuery(_context);
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
            if (_transaction != null)
			    _transaction.Commit();
			_transactionCommitted = true;

            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex = null;
            }
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
	    	return SqlCeDatabaseHelper<DicomStoreDataContext>.CreateConnection(databaseFile);
		}
	}
}
