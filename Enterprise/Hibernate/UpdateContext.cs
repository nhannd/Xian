using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IUpdateContext"/>.
    /// </summary>
    public class UpdateContext : PersistenceContext, IUpdateContext
    {
        private UpdateContextInterceptor _interceptor;
        private ITransactionRecorder _transactionRecorder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="transactionNotifier"></param>
        /// <param name="mode"></param>
        internal UpdateContext(PersistentStore pstore, UpdateContextSyncMode mode)
            : base(pstore)
        {
            if (mode == UpdateContextSyncMode.Hold)
                throw new NotSupportedException("UpdateContextSyncMode.Hold is not supported");
        }

        #region IUpdateContext members

        /// <summary>
        /// Gets or sets the transaction recorder for auditing.
        /// </summary>
        public ITransactionRecorder TransactionRecorder
        {
            get { return _transactionRecorder; }
            set { _transactionRecorder = value; }
        }

        public void Commit()
        {
            try
            {
                // sync state prior to commit, this ensures that all entities are validated and changes
                // recorded by the interceptor
                SynchStateCore();

                // do audit
                AuditTransaction();

                // do final commit
                CommitTransaction();

                /* Transaction notification is non-existent right now
                if (this.PersistentStore.TransactionNotifier != null)
                {
                    this.PersistentStore.TransactionNotifier.Queue(this.Interceptor.EntityChangeSet);
                }
                */
            }
            catch (Exception e)
            {
                HandleHibernateException(e, SR.ExceptionCommitFailure);
            }
        }

        #endregion

        #region Protected overrides

        protected override ISession CreateSession()
        {
            return this.PersistentStore.SessionFactory.OpenSession(_interceptor = new UpdateContextInterceptor());
        }

        protected override void LockCore(Entity entity, DirtyState dirtyState)
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

        internal override bool ReadOnly
        {
            get { return false; }
        }

        protected override void SynchStateCore()
        {
            this.Session.Flush();

            // check validation results
            CheckValidation();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // assume the transaction failed and rollback
                    RollbackTransaction();
                }
                catch (Exception e)
                {
                    HandleHibernateException(e, SR.ExceptionCloseContext);
                }
            }

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

        #endregion


        #region Helpers

        /// <summary>
        /// Creates and saves a <see cref="TransactionRecord"/> for the current transaction, assuming the
        /// <see cref="TransactionRecorder"/> property is set.
        /// </summary>
        private void AuditTransaction()
        {
            if (_transactionRecorder != null)
            {

                TransactionRecord txRecord = _transactionRecorder.CreateTransactionRecord(_interceptor.EntityChangeSet);

                /* NB. Does not work with NHibernate 1.0.3
                // obtain an audit session, based on the same ADO connection and same DB transaction
                using (ISession session = _sessionFactory.OpenSession(this.Session.Connection))
                {
                   session.Save(record);
                   session.Flush();
                }
                */

                // for now, use the same session
                this.Session.Save(txRecord);
            }
        }

        private void CheckValidation()
        {
        }

        #endregion
    }
}
