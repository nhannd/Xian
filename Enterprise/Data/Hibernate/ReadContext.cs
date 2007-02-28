using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

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
        internal ReadContext(ISessionFactory sessionFactory)
            :base(sessionFactory, true)
        {
            // never write changes to the database from a read context
            this.Session.FlushMode = FlushMode.Never;
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
                    // end the transaction
                    CommitTransaction();
                }
                catch (Exception e)
                {
                    WrapAndRethrow(e, SR.ExceptionCloseContext);
                }
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        protected override EntityLoadFlags DefaultEntityLoadFlags
        {
            get { return EntityLoadFlags.None; }
        }

        public override void Lock(Entity entity)
        {
            this.Session.Lock(entity, LockMode.None);
        }

        public override void Lock(Entity entity, DirtyState dirtyState)
        {
            if (dirtyState == DirtyState.Dirty)
                throw new InvalidOperationException();

            Lock(entity);
        }

    }
}
