using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    /// <summary>
    /// Defines the extension point for all NHibernate broker classes.
    /// </summary>
    [ExtensionPoint()]
    public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
    {
    }

    public abstract class PersistenceContext : IPersistenceContext
    {
        #region Constructors
        protected PersistenceContext(SqlConnection connection)
        {
            _connection = connection;
        }
        #endregion

        #region Private Members
        private SqlConnection _connection;
        #endregion

        #region Internal Properties
        internal SqlConnection Connection
        {
            get { return _connection; }
        }
        #endregion

        #region IPersistenceContext Members

        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            TBrokerInterface broker = (TBrokerInterface)xp.CreateExtension(new TypeExtensionFilter(typeof(TBrokerInterface)));
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

        public ITransactionRecorder TransactionRecorder
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
