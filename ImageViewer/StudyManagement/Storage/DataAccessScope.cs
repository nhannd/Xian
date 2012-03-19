#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Data;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	/// <summary>
	/// Manages creation and lifetime of data context objects.
	/// </summary>
	/// <remarks>
	/// As per this blog post (http://matthewmanela.com/blog/sql-ce-3-5-with-linq-to-sql/), there are
	/// some performance considerations to take into account when using linq-to-sql with sql-ce. Basically
	/// we don't want to allow L2S to manage the connection, but rather we manage it ourselves.
	/// </remarks>
	public class DataAccessScope : IDisposable
	{
		private const string DefaultDatabaseFileName = "dicom_store.sdf";

		[ThreadStatic]
		private static DataAccessScope _head;

		/// <summary>
		/// Gets the currently active <see cref="DataAccessScope"/>, or null if none exists.
		/// </summary>
		public static DataAccessScope Current
		{
			get { return _head; }
		}

		private readonly DataAccessScope _parent;
		private readonly bool _ownsContext;
		private readonly DicomStoreDataContext _context;
		private readonly IDbConnection _connection;
		private readonly IDbTransaction _transaction;
		private bool _disposed;

		/// <summary>
		/// Creates a new data access scope, re-using an existing open context if one exists for the current thread.
		/// </summary>
		public DataAccessScope()
			:this(false)
		{
		}

		private DataAccessScope(bool newContext)
		{
			if(newContext || _head == null)
			{
				// initialize a connection and transaction
				_connection = CreateConnection();
				_transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
				_context = new DicomStoreDataContext(_connection);
				_ownsContext = true;
			}
			else
			{
				// inherit an existing context
				_context = _head._context;
				_ownsContext = false;
			}

			_parent = _head;
			_head = this;
		}

		public WorkItemBroker GetWorkItemBroker()
		{
			return new WorkItemBroker(_context);
		}

        public WorkItemUidBroker GetWorkItemUidBroker()
        {
            return new WorkItemUidBroker(_context);
        }

		public RuleBroker GetRuleBroker()
		{
			return new RuleBroker(_context);
		}

		public StudyBroker GetStudyBroker()
		{
			return new StudyBroker(_context);
		}

		/// <summary>
		/// Submits any pending changes to the database, but does not commit the transaction.
		/// </summary>
		public void SubmitChanges()
		{
			_context.SubmitChanges();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			_head = _parent;
			if (_ownsContext)
			{
				try
				{
					_context.SubmitChanges();
					_transaction.Commit();
				}
				finally
				{
					// dispose of connection, since we own it
					_context.Dispose();
					_connection.Close();
					_connection.Dispose();
				}
			}
		}

		/// <summary>
		/// Gets the connection to the default database.
		/// </summary>
		/// <returns></returns>
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
