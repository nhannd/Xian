using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;

using NHibernate;
using System.Data;
using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Defines the extension point for all NHibernate broker classes.
    /// </summary>
    [ExtensionPoint()]
    public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
    {
    }

    /// <summary>
    /// Abstract base class for NHibernate persistence context implementations.
    /// </summary>
    public abstract class PersistenceContext : IPersistenceContext
    {
        private ISessionFactory _sessionFactory;
        private bool _readOnly;
        private DefaultInterceptor _interceptor;
        private NHibernate.ISession _session;
        private NHibernate.ITransaction _transaction;
        private ITransactionRecorder _transactionRecorder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="readOnly"></param>
        internal PersistenceContext(ISessionFactory sessionFactory, bool readOnly)
        {
            _sessionFactory = sessionFactory;
            _session = _sessionFactory.OpenSession(_interceptor = new DefaultInterceptor());
            _readOnly = readOnly;

            BeginTransaction();
        }

        #region IPersistenceContext members

        public ITransactionRecorder TransactionRecorder
        {
            get { return _transactionRecorder; }
            set { _transactionRecorder = value; }
        }

        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            TBrokerInterface broker = (TBrokerInterface)xp.CreateExtension(new TypeExtensionFilter(typeof(TBrokerInterface)));
            broker.SetContext(this);
            return broker;
        }

        public abstract void Lock(Entity entity);
        public abstract void Lock(Entity entity, DirtyState dirtyState);

        public virtual void Suspend()
        {
            try
            {
                System.Diagnostics.Debug.Assert(this.InTransaction);

                // commit the current transaction
                // if we are suspending an application-transaction, typically we are in FlushMode.Never,
                // so changes won't be written to the DB
                // however, if we are in FlushMode.Auto, this behaviour is still correct, because
                // we can only assume that the application intends for changes to be written to the DB in this case
                CommitTransaction();
                this.Session.Disconnect();
            }
            catch (Exception e)
            {
                // wrap NHibernate exception
                WrapAndRethrow(e, SR.ExceptionCommitFailure);
            }
        }

        public virtual void Resume()
        {
            try
            {
                this.Session.Reconnect();
                BeginTransaction();
            }
            catch (Exception e)
            {
                // wrap NHibernate exception
                WrapAndRethrow(e, SR.ExceptionResumeContext);
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
        /// Allows a broker to create an ADO.NET command, rather than using NHibernate.  The command
        /// will execute on the same connection and within the same transaction (assuming one exists)
        /// as any other operation on this context.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IDbCommand CreateSqlCommand(string sql)
        {
            IDbCommand cmd = this.Session.Connection.CreateCommand();
            cmd.CommandText = sql;
            if (this.InTransaction)
            {
                this.Session.Transaction.Enlist(cmd);
            }

            return cmd;
        }

        public NHibernate.IQuery CreateHibernateQuery(string hql)
        {
            return this.Session.CreateQuery(hql);
        }

        
        /// <summary>
        /// Default <see cref="EntityLoadFlags"/> to be used by this context
        /// </summary>
        protected abstract EntityLoadFlags DefaultEntityLoadFlags { get; }

        /// <summary>
        /// Loads the specified entity into this context
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        public virtual TEntity Load<TEntity>(EntityRef entityRef)
            where TEntity : Entity
        {
            return this.Load<TEntity>(entityRef, this.DefaultEntityLoadFlags);
        }

        public virtual TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags)
            where TEntity : Entity
        {
            try
            {
                // use a proxy if EntityLoadFlags.Proxy is specified and EntityLoadFlags.CheckVersion is not specified (CheckVersion overrides Proxy)
                bool useProxy = (flags & EntityLoadFlags.CheckVersion) == 0 && (flags & EntityLoadFlags.Proxy) == EntityLoadFlags.Proxy;
                Entity entity = (Entity)Load(EntityRefUtils.GetClass(entityRef), EntityRefUtils.GetOID(entityRef), useProxy);

                // check version if necessary
                if ((flags & EntityLoadFlags.CheckVersion) == EntityLoadFlags.CheckVersion && !EntityRefUtils.GetVersion(entityRef).Equals(entity.Version))
                    throw new EntityVersionException(EntityRefUtils.GetOID(entityRef), null);

                return (TEntity)entity;

            }
            catch (ObjectNotFoundException hibernateException)
            {
                // note that we will only get here if useProxy == false in the above block
                // if the entity is proxied, verification of its existence is deferred until the proxy is realized
                throw new EntityNotFoundException(hibernateException);
            }
        }

        internal EnumValue LoadEnumValue(Type enumValueClass, string code)
        {
            // always use a proxy
            // no need to check for ObjectNotFoundException, because we are using a proxy, so we won't get this exception anyway
            return (EnumValue)Load(enumValueClass, code, true);
        }

        private object Load(Type persistentClass, object oid, bool useProxy)
        {
            return useProxy ?
                this.Session.Load(persistentClass, oid, LockMode.None)
                : this.Session.Get(persistentClass, oid);
        }

        public bool IsProxyLoaded(Entity entity)
        {
            return NHibernateUtil.IsInitialized(entity);
        }

        public bool IsCollectionLoaded(IEnumerable collection)
        {
            return NHibernateUtil.IsInitialized(collection);
        }


        /// <summary>
        /// True if this context is read-only.
        /// </summary>
        internal bool ReadOnly
        {
            get { return _readOnly; }
        }

        /// <summary>
        /// True if there is currently a transaction in progress on this context
        /// </summary>
        internal bool InTransaction
        {
            get { return _transaction != null; }
        }

        /// <summary>
        /// Begins a transaction 
        /// </summary>
        protected void BeginTransaction()
        {
            System.Diagnostics.Debug.Assert(_transaction == null, "There is already a transaction in progress");
            _transaction = this.Session.BeginTransaction();
        }

        /// <summary>
        /// Commits the transaction
        /// </summary>
        protected void CommitTransaction()
        {
            System.Diagnostics.Debug.Assert(_transaction != null, "There is no transaction to commit");

            // do a final flush prior to commit, so that the DefaultInterceptor will capture the entire change set
            // prior to auditing
            SynchState();

            // do auditing prior to commit
            AuditTransaction();

            _transaction.Commit();
            _transaction = null;
        }

        /// <summary>
        /// Synchronizes the state of the context with the persistent store, ensuring that any new entities
        /// have OIDs generated, and writing all changes to the database.
        /// </summary>
        public void SynchState()
        {
            if (_session.FlushMode != FlushMode.Never)
            {
                _session.Flush();
            }
        }

        /// <summary>
        /// Creates and saves a <see cref="TransactionRecord"/> for the current transaction, assuming the
        /// <see cref="TransactionRecorder"/> property is set.
        /// </summary>
        private void AuditTransaction()
        {
            if (_transactionRecorder != null)
            {
                //NB: for the time being, we cannot audit read-only contexts, because we are using the same session
                //to audit, and the flush-mode is "never" for a read-only context.  We should use a separate session,
                //but it does not work with NHibernate 1.0.3
                if (this.ReadOnly)
                    return;

                TransactionRecord record = _transactionRecorder.CreateTransactionRecord(this.Interceptor.EntityChangeSet);
/* NB. Does not work with NHibernate 1.0.3
                // obtain an audit session, based on the same ADO connection and same DB transaction
                using (ISession session = _sessionFactory.OpenSession(this.Session.Connection))
                {
                    session.Save(record);
                    session.Flush();
                }
 */ 

                // for now, use the same session
                this.Session.Save(record);
            }
        }

        /// <summary>
        /// Rollsback the transaction
        /// </summary>
        protected void RollbackTransaction()
        {
            System.Diagnostics.Debug.Assert(_transaction != null, "There is no transaction to rollback");

            _transaction.Rollback();
            _transaction = null;
        }

        /// <summary>
        /// Wraps an NHibernate exception and rethrows it
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        protected void WrapAndRethrow(Exception e, string message)
        {
            if (e is StaleObjectStateException)
            {
                throw new EntityVersionException((e as StaleObjectStateException).Identifier, e);
            }
            else if (e is ObjectNotFoundException)
            {
                throw new EntityNotFoundException(e);
            }
            ///TODO any other specific kinds of exceptions we need to consider?
            else
            {
                throw new PersistenceException(message, e);
            }
        }

        /// <summary>
        /// Provides access the NHibernate Session object.
        /// </summary>
        internal NHibernate.ISession Session
        {
            get { return _session; }
        }

        /// <summary>
        /// Implementation of the Dispose pattern
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_session != null)
                {
                    _session.Close();
                    _session = null;
                }
            }
        }

        /// <summary>
        /// Provides access to the interceptor
        /// </summary>
        internal DefaultInterceptor Interceptor
        {
            get { return _interceptor; }
        }
    }
}
