#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Defines the extension point for all NHibernate broker classes.
    /// </summary>
    [ExtensionPoint]
    public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
    {
    }

    public abstract class PersistenceContext : IPersistenceContext
    {
        #region Constructors
        protected PersistenceContext(SqlConnection connection, ITransactionNotifier transactionNotifier)
        {
            _connection = connection;
            _transactionNotifier = transactionNotifier;
        }
        #endregion

        #region Private Members
        private SqlConnection _connection;
        private ITransactionNotifier _transactionNotifier;
    	private IEntityChangeSetRecorder _changeSetRecorder = new ChangeSetRecorder();
        #endregion

        #region Properties
        public SqlConnection Connection
        {
            get { return _connection; }
        }

		public int CommandTimeout
		{
			get { return SqlServerSettings.Default.CommandTimeout; }
		}
        #endregion

        #region IPersistenceContext Members

        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            return (TBrokerInterface)GetBroker(typeof(TBrokerInterface));
        }

        public object GetBroker(Type brokerInterface)
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            IPersistenceBroker broker = (IPersistenceBroker)xp.CreateExtension(new TypeExtensionFilter(brokerInterface));
            broker.SetContext(this);
            return broker;
        }

        public void Lock(Entity entity)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Lock(Entity entity, DirtyState state)
        {
            throw new Exception("The method or operation is not implemented.");
        }
		
        public void Lock(EnumValue enumValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		public Entity Load(EntityRef entityRef, EntityLoadFlags flags)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public TEntity Load<TEntity>(EntityRef entityRef) where TEntity : Entity
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags) where TEntity : Entity
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsProxyLoaded(Entity entity)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsCollectionLoaded(System.Collections.IEnumerable collection)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void Suspend()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void Resume()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SynchState()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEntityChangeSetRecorder ChangeSetRecorder
        {
            get
            {
            	return _changeSetRecorder;
            }
            set
            {
            	_changeSetRecorder = value;
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}
