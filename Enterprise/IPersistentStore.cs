using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the interface to a persistent store, and acts as a factory for obtaining instances of 
    /// <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
    /// </summary>
    public interface IPersistentStore
    {
        /// <summary>
        /// Called by the framework to initialize the persistent store.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the transaction notifier, which the store can use to notify the application of changes
        /// made to its entities.
        /// </summary>
        /// <param name="transactionNotifier"></param>
        void SetTransactionNotifier(ITransactionNotifier transactionNotifier);

        /// <summary>
        /// Obtains an <see cref="IReadContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a read context</returns>
        IReadContext OpenReadContext();

        /// <summary>
        /// Obtains an <see cref="IUpdateContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a update context</returns>
        IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode);
    }
}
