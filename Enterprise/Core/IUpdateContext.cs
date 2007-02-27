using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to an update context.  An update context allows the application read
    /// data from a persistent store, and to synchronize the persistent store with changes made to that data.
    /// </summary>
    public interface IUpdateContext : IPersistenceContext
    {
        /// <summary>
        /// Resumes this context, assuming it has been suspended.  Typically specify <see cref="UpdateContextSyncMode.Flush"/>
        /// only if this is the final step in the application transaction.
        /// </summary>
        /// <param name="syncMode"></param>
        void Resume(UpdateContextSyncMode syncMode);


        /// <summary>
        /// Attempts to flush and commit all changes made within this update context to the persistent store.
        /// If this operation succeeds, the state of the persistent store will be syncrhonized with the state
        /// of all domain objects that are attached to this context, and the context can continue to be used
        /// for read operations only.
        /// If the operation fails, an exception will be thrown.
        /// </summary>
        void Commit();
    }
}
