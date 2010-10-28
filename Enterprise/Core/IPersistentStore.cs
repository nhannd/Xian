#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core
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

		/// <summary>
		/// The version associated with the persistent store.
		/// </summary>
		Version Version { get; }
    }
}
