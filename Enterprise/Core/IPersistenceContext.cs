#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        /// Returns a broker that implements the specified interface to retrieve data into this persistence context.
        /// </summary>
        /// <param name="brokerInterface"></param>
        /// <returns></returns>
        object GetBroker(Type brokerInterface);

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

        void SynchState();

    }
}
