#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IUpdateContext"/>.
    /// </summary>
    public class UpdateContext : PersistenceContext, IUpdateContext
    {
        private UpdateContextInterceptor _interceptor;
        private IEntityChangeSetRecorder _changeSetRecorder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"></param>
        internal UpdateContext(PersistentStore pstore, UpdateContextSyncMode mode)
            : base(pstore)
        {
            if (mode == UpdateContextSyncMode.Hold)
                throw new NotSupportedException("UpdateContextSyncMode.Hold is not supported");

            // create a default change-set logger
            _changeSetRecorder = new DefaultEntityChangeSetRecorder();
        }

        #region IUpdateContext members

        /// <summary>
        /// Gets or sets the change-set logger for auditing.
        /// </summary>
        /// <remarks>
        /// Setting this property to null will disable change-set auditing for this update context.
        /// </remarks>
        public IEntityChangeSetRecorder ChangeSetRecorder
        {
            get { return _changeSetRecorder; }
            set { _changeSetRecorder = value; }
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
                    PreValidate(entity);
                    this.Session.Save(entity);
                    break;
                case DirtyState.Clean:
                    this.Session.Lock(entity, LockMode.None);
                    break;
            }
        }

        private void PreValidate(Entity entity)
        {
            // This is really a HACK
            // we need to test the required field rules before NHibernate gets a chance to complain about them
            // in order to provide more descriptive error message (the NHibernate error messages suck)
            Validation.Validate(entity, 
                delegate(ISpecification rule)
                {
                    return rule is RequiredSpecification;
                });
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
        /// Writes an audit log entry for the current change-set, assuming the
        /// <see cref="ChangeSetRecorder"/> property is set.
        /// </summary>
        private void AuditTransaction()
        {
            if (_changeSetRecorder != null)
            {
				// write to the "ChangeSet" audit log
				AuditLog auditLog = new AuditLog("ChangeSet");
                _changeSetRecorder.WriteLogEntry(_interceptor.EntityChangeSet, auditLog);
            }
        }

        private void CheckValidation()
        {
        }

        #endregion
    }
}
