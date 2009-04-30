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

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IReadContext"/>.
    /// </summary>
    public class ReadContext : PersistenceContext, IReadContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory"></param>
        internal ReadContext(PersistentStore pstore)
            : base(pstore)
        {
        }

        #region Protected overrides

        protected override ISession CreateSession()
        {
            ISession session = this.PersistentStore.SessionFactory.OpenSession();

            // never write changes to the database from a read context
            session.FlushMode = FlushMode.Never;
            return session;
        }

        protected override void SynchStateCore()
        {
            // do nothing
        }

        internal override bool ReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // commit the transaction - nothing will be flushed to the DB
                    CommitTransaction();
                }
                catch (Exception e)
                {
                    HandleHibernateException(e, SR.ExceptionCloseContext);
                }
            }

            base.Dispose(disposing);
        }

        protected override EntityLoadFlags DefaultEntityLoadFlags
        {
            get { return EntityLoadFlags.None; }
        }

        protected override void LockCore(Entity entity, DirtyState dirtyState)
        {
            if (dirtyState != DirtyState.Clean)
                throw new InvalidOperationException();

            this.Session.Lock(entity, LockMode.None);
        }

        #endregion
    }
}
