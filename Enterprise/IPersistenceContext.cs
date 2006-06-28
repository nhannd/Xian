using System;
namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Base interface for a persistence context.  A persistence context is an implementation of the unit-of-work
    /// pattern, and defines a scope in which the application can perform a set of operations on a persistent store.
    /// This interface is not implemented directly.  See <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
    /// 
    /// Also acts as a factory to obtain instances of <see cref="IPersistenceBroker"/>.
    /// </summary>
    /// <seealso cref="IReadContext"/>
    /// <seealso cref="IUpdateContext"/>
    public interface IPersistenceContext : IDisposable
    {
        /// <summary>
        /// Returns a broker that implements the specified interface and that is configured to operate on
        /// this persistence context.
        /// </summary>
        /// <typeparam name="TBrokerInterface">The interface of the broker to obtain</typeparam>
        /// <returns></returns>
        TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;

        /// <summary>
        /// Closes this persistence context.  No further work can be performed.
        /// </summary>
        void Close();
    }
}
