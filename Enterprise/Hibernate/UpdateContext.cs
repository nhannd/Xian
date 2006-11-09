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
        private ITransactionNotifier _transactionNotifier;

        internal UpdateContext(ISessionFactory sessionFactory, ITransactionNotifier transactionNotifier, UpdateContextSyncMode mode)
            :base(sessionFactory, false)
        {
            _transactionNotifier = transactionNotifier;
            this.Session.FlushMode = mode == UpdateContextSyncMode.Flush ? FlushMode.Auto : FlushMode.Never;
        }

        public override void Lock(Entity entity)
        {
            Lock(entity, DirtyState.Clean);
        }

        public override void Lock(Entity entity, DirtyState dirtyState)
        {
            if (dirtyState == DirtyState.Dirty)
                this.Session.SaveOrUpdate(entity);
            else
                this.Session.Lock(entity, LockMode.None);
        }

        public void Commit()
        {
            try
            {
                if (!this.InTransaction)
                    throw new InvalidOperationException(SR.ErrorNoCurrentTransaction);

                CommitTransaction();
  
                if (_transactionNotifier != null)
                {
                    _transactionNotifier.Queue(this.Interceptor.EntityChangeSet);
                }
            }
            catch (Exception e)
            {
                WrapAndRethrow(e, SR.ErrorCommitFailure);
            }
        }

        public override void Resume()
        {
            Resume(UpdateContextSyncMode.Flush);
        }

        public void Resume(UpdateContextSyncMode mode)
        {
            base.Resume();

            this.Session.FlushMode = mode == UpdateContextSyncMode.Flush ? FlushMode.Auto : FlushMode.Never;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.InTransaction)
                {
                    try
                    {
                        // assume the transaction failed and rollback
                        RollbackTransaction();
                    }
                    catch (Exception e)
                    {
                        WrapAndRethrow(e, SR.ErrorCloseContext);
                    }
                }
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        protected override EntityLoadFlags DefaultEntityLoadFlags
        {
            get { return EntityLoadFlags.CheckVersion; }
        }
    }
}
