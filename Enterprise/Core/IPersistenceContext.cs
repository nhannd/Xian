#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;
namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Base interface for a persistence context. See <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
	/// </summary>
	/// <remarks>
	/// A persistence context is an implementation of the unit-of-work
	/// and identity map patterns, and defines a scope in which the application can perform a set of operations on
	/// a persistent store.  This interface is not implemented directly.
	/// </remarks>
	/// <seealso cref="IReadContext"/>
	/// <seealso cref="IUpdateContext"/>
	public interface IPersistenceContext : IPersistenceBrokerFactory, IDisposable
	{
		/// <summary>
		/// Locks the specified entity into the context. 
		/// </summary>
		/// <remarks>
		/// If this is an update context, the entity will be treated as "clean".
		/// Use the other overload to specify that the entity is new or dirty.</remarks>
		/// <param name="entity"></param>
		void Lock(Entity entity);

		/// <summary>
		/// Locks the specified entity into the context with the specified <see cref="DirtyState"/>.
		/// </summary>
		/// <remarks>
		/// Note that it does not make sense to lock an entity into a read context with <see cref="DirtyState.Dirty"/>,
		/// and an exception will be thrown.
		/// </remarks>
		/// <param name="entity"></param>
		/// <param name="state"></param>
		void Lock(Entity entity, DirtyState state);


		/// <summary>
		/// Locks the specified enum value into the context. 
		/// </summary>
		/// <remarks>
		/// Call this method to re-associate an enum value instance with this context,
		/// in order for an entity to be able to reference the instance.
		/// </remarks>
		void Lock(EnumValue enumValue);

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <returns></returns>
		TEntity Load<TEntity>(EntityRef entityRef) where TEntity : Entity;

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags) where TEntity : Entity;

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		Entity Load(EntityRef entityRef, EntityLoadFlags flags);

		/// <summary>
		/// Synchronizes the state of the persistent store (database) with the state of this context.
		/// </summary>
		/// <remarks>
		/// This method will ensure that any pending writes to the persistent store are flushed, and that
		/// any generated object identifiers for new persistent objects are generated and assigned to those objects. 
		/// </remarks>
		void SynchState();
	}
}
