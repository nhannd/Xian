using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;

using NHibernate;
using System.Data;

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
        private bool _readOnly;
        private DefaultInterceptor _interceptor;
        private NHibernate.ISession _session;
        private NHibernate.ITransaction _transaction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="readOnly"></param>
        internal PersistenceContext(ISessionFactory sessionFactory, bool readOnly)
        {
            _session = sessionFactory.OpenSession(_interceptor = new DefaultInterceptor());
            _readOnly = readOnly;

            BeginTransaction();
        }

        #region IPersistenceContext members

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
                WrapAndRethrow(e, SR.ErrorCommitFailure);
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
                WrapAndRethrow(e, SR.ErrorResumeContext);
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

        
        /// <summary>
        /// Default <see cref="EntityLoadFlags"/> to be used by this context
        /// </summary>
        protected abstract EntityLoadFlags DefaultEntityLoadFlags { get; }

        /// <summary>
        /// Loads the specified entity into this context
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        internal Entity Load(EntityRefBase entityRef)
        {
            return this.Load(entityRef, this.DefaultEntityLoadFlags);
        }

        /// <summary>
        /// Loads the specified entity into this context
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal Entity Load(EntityRefBase entityRef, EntityLoadFlags flags)
        {
            Entity entity = null;

            try
            {
                // use Session.Load(...) rather than Session.Get(...), because Session.Get will always 
                // resolve a lazy proxy, whether necessary or not, which is obviously undesirable

                // Session.Load with LockMode.None will simply return a proxy.  If we do not need to do
                // version checking, then a proxy is sufficient- hence this provides a good performance optimization.
                // However, the downside is that if the OID does not exist, this will not be known until later,
                // when the proxy is resolved.  If the proxy is never resolved, and the non-existent entity
                // is referenced in a relationship, the violation of integrity constraints will not become apparent
                // until the session is flushed.

                // Session.Load with LockMode.Read will force the proxy to be resolved- this is necessary
                // in order to read the Version property for version checking

                entity = (Entity)this.Session.Load(EntityUtils.GetType(entityRef), EntityUtils.GetOID(entityRef),
                    (flags & EntityLoadFlags.CheckVersion) != 0 ? LockMode.Read : LockMode.None);

                // if the Proxy flag was not specified, then initialize the full object
                if ((flags & EntityLoadFlags.Proxy) == 0)
                {
                    if (!NHibernateUtil.IsInitialized(entity))
                        NHibernateUtil.Initialize(entity);
                }
            }
            catch (ObjectNotFoundException hibernateException)
            {
                // note that we will only get here if LockMode.Read was used in the above block,
                // or the entity is not proxied
                // if LockMode.None was used and the entity is proxied, no exception is thrown
                throw new EntityNotFoundException(hibernateException);
            }

            // check version if necessary
            if ((flags & EntityLoadFlags.CheckVersion) != 0 && !EntityUtils.CheckVersion(entityRef, entity))
                throw new ConcurrencyException(null);

            System.Diagnostics.Debug.Assert(entity != null);

            return entity;
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

            _transaction.Commit();
            _transaction = null;
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
                throw new ConcurrencyException(e);
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
        protected DefaultInterceptor Interceptor
        {
            get { return _interceptor; }
        }
    }
}
