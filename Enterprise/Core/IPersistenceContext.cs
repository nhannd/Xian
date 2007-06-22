using System;
using System.Collections;
using ClearCanvas.Enterprise.Common;
namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Base interface for a persistence context.  A persistence context is an implementation of the unit-of-work
    /// and identity map patterns, and defines a scope in which the application can perform a set of operations on
    /// a persistent store.  This interface is not implemented directly.
    /// See <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
    /// </summary>
    /// <seealso cref="IReadContext"/>
    /// <seealso cref="IUpdateContext"/>
    public interface IPersistenceContext : IDisposable
    {
        /// <summary>
        /// Returns a broker that implements the specified interface to retrieve data into this persistence context.
        /// </summary>
        /// <typeparam name="TBrokerInterface">The interface of the broker to obtain</typeparam>
        /// <returns></returns>
        TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;

        /// <summary>
        /// Locks the specified entity into the context.  If this is an update context, the entity will be
        /// treated as "clean".  Use the other overload to specify that the entity is dirty.
        /// </summary>
        /// <param name="entity"></param>
        void Lock(Entity entity);

        /// <summary>
        /// Locks the specified entity into the context with the specified <see cref="DirtyState"/>.
        /// Note that it does not make sense to lock an entity into a read-context with <see cref="DirtyState.Dirty"/>,
        /// and an exception will be thrown.  Nevertheless, this method exists on this interface for
        /// the sake of convenience.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        void Lock(Entity entity, DirtyState state);

        /// <summary>
        /// Loads the specified entity into this context
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        TEntity Load<TEntity>(EntityRef entityRef)
            where TEntity : Entity;

        /// <summary>
        /// Loads the specified entity into this context
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags)
            where TEntity : Entity;

        bool IsProxyLoaded(Entity entity);

        bool IsCollectionLoaded(IEnumerable collection);


        /// <summary>
        /// Suspends this context.  Releases all connections and resources, but maintains the state
        /// of all persistent objects.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes a suspended context.
        /// </summary>
        void Resume();

        void SynchState();

        ITransactionRecorder TransactionRecorder { get; set; }

    }
}
