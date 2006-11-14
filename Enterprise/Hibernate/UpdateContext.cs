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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="transactionNotifier"></param>
        /// <param name="mode"></param>
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
            switch (dirtyState)
            {
                case DirtyState.Dirty:
                    this.Session.Update(entity);
                    break;
                case DirtyState.New:
                    this.Session.Save(entity);
                    break;
                case DirtyState.Clean:
                    this.Session.Lock(entity, LockMode.None);
                    break;
            }
        }

        #region IUpdateContext members

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

        public void Resume(UpdateContextSyncMode mode)
        {
            base.Resume();

            this.Session.FlushMode = mode == UpdateContextSyncMode.Flush ? FlushMode.Auto : FlushMode.Never;

            // clear the previous change set
            // (this is only necessary if the context was previously opened in Flush mode, which is not a typical usage)
            this.Interceptor.ClearChangeSet();
        }

        #endregion

        public override void Resume()
        {
            Resume(UpdateContextSyncMode.Flush);
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

        /// <summary>
        /// Specifies that the version should be checked.  This makes sense, as a default, in an update context
        /// to ensure versioning isn't violated.
        /// </summary>
        protected override EntityLoadFlags DefaultEntityLoadFlags
        {
            get { return EntityLoadFlags.CheckVersion; }
        }
    }
}
