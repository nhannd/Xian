using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the interface to an update context.  An update context allows the application to perform both read
    /// and write operations on a persistent store within the scope of a single transaction.
    /// </summary>
    public interface IUpdateContext : IPersistenceContext
    {
        /// <summary>
        /// Returns true if the transaction is currently open.
        /// </summary>
        bool InTransaction { get; }

        /// <summary>
        /// Attempts to flush and commit all changes made within this update context to the persistent store.
        /// If this operation succeeds, the state of the persistent store will be syncrhonized with the state
        /// of all domain objects that are attached to this context, and the context can continue to be used
        /// for read operations only.
        /// If the operation fails, an exception will be thrown.
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all changes made within this update context.  The persistent store will not be synchronized
        /// with the state of the domain objects that are attached to this context.  Note that this implies that
        /// these domain objects are effectively invalid and should be discarded.  After a call to this method,
        /// this context is no longer valid for use and must be disposed.
        /// </summary>
        void Rollback();
    }
}
