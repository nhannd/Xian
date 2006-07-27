using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IUpdateContext"/>.
    /// </summary>
    public class UpdateContext : PersistenceContext, IUpdateContext
    {
        private ITransaction _transaction;

        internal UpdateContext(ISessionFactory sessionFactory)
            :base(sessionFactory, false)
        {
            _transaction = this.Session.BeginTransaction();
        }

        public EntityChange[] EntityChangeSet
        {
            get { return this.Interceptor.EntityChangeSet; }
        }

        public bool InTransaction
        {
            get { return _transaction != null; }
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
                _transaction = null;
            }
            catch (StaleObjectStateException e)
            {
                // wrap NHibernate exception
                throw new ConcurrentModificationException(e);
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        public override void Close()
        {
            if (_transaction != null)
            {
                Rollback();
            }

            base.Close();
        }
    }
}
