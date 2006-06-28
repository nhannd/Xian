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
        /// Obtains an <see cref="IReadContext"/> for use by the application to perform read-only
        /// operations on this persistent store.
        /// </summary>
        /// <returns>a read context</returns>
        IReadContext GetReadContext();

        /// <summary>
        /// Obtains an <see cref="IUpdateContext"/> for use by the application to perform read and write
        /// operations on this persistent store.
        /// </summary>
        /// <returns>an update context</returns>
        IUpdateContext GetUpdateContext();
    }
}
