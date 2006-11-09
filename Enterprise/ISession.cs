using System;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the public interface for a session.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Returns the service manager associated with this session.
        /// </summary>
        IServiceManager ServiceManager { get; }

        /// <summary>
        /// Returns the transaction notifier associated with this session.
        /// </summary>
        ITransactionNotifier TransactionNotifier { get; }
    }
}
