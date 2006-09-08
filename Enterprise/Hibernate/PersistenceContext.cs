using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;

using NHibernate;

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
        private NHibernate.ISession _session;
        private bool _readOnly;
        private DefaultInterceptor _interceptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="readOnly"></param>
        internal PersistenceContext(ISessionFactory sessionFactory, bool readOnly)
        {
            _session = sessionFactory.OpenSession(_interceptor = new DefaultInterceptor());
            _readOnly = readOnly;
        }

        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            TBrokerInterface broker = (TBrokerInterface)xp.CreateExtension(new TypeExtensionFilter(typeof(TBrokerInterface)));
            broker.SetContext(this);
            return broker;
        }

        public void Reattach(Entity entity)
        {
            // if the entity is not part of the current session, re-attach
            if (!this.Session.Contains(entity))
            {
                // NHibernate docs say to use LockMode.Read in this scenario - not sure why
                this.Session.Lock(entity, LockMode.Read);
            }
        }

        /// <summary>
        /// True if this object is read-only.
        /// </summary>
        internal bool ReadOnly
        {
            get { return _readOnly; }
        }

        /// <summary>
        /// Provides access the NHibernate Session object.
        /// </summary>
        internal NHibernate.ISession Session
        {
            get { return _session; }
        }

        public virtual void Close()
        {
            if (_session != null)
            {
                _session.Close();
                _session = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        protected DefaultInterceptor Interceptor
        {
            get { return _interceptor; }
        }
    }
}
