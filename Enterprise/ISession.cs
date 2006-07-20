using System;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the public interface for a session.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Obtains a read context for use by the application.  It is up to the implementation to 
        /// determine whether to instantiate a new read context or to return a cached instance.
        /// </summary>
        /// <returns></returns>
        IReadContext GetReadContext();

        /// <summary>
        /// Obtains a new update context for use by the application.
        /// </summary>
        /// <returns></returns>
        IUpdateContext GetUpdateContext();

        /// <summary>
        /// Returns the service manager associated with this session.
        /// </summary>
        IServiceManager ServiceManager { get; }

        /// <summary>
        /// Returns the transaction monitor associated with this session.
        /// </summary>
        ITransactionMonitor TransactionMonitor { get; }
    }
}
