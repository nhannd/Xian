#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines an interface to a class that updates a view in response to a change to an entity.
	/// </summary>
	/// <typeparam name="TViewItem">Class of view item to be updated.</typeparam>
	/// <typeparam name="TViewItemSearchCriteria">Search criteria class for view item class to be updated.</typeparam>
	public interface IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>
		where TViewItem : Entity
		where TViewItemSearchCriteria : EntitySearchCriteria
	{
		/// <summary>
		/// Gets a value indicating if this handler is interested in the specified change.
		/// </summary>
		/// <param name="change"></param>
		/// <returns></returns>
		bool IsInterested(EntityChange change);

		/// <summary>
		/// Gets a predicate that determines whether a given view item may be affected by the specified change.
		/// </summary>
		/// <remarks>
		/// The predicate is used to find all view items that may be affected by a change, and may therefore
		/// need to be updated or removed.
		/// </remarks>
		/// <param name="change">The change in question.</param>
		/// <param name="changedEntity">The changed entity.</param>
		/// <returns>A predicate that computes whether a given view item may be affected by the specified change.</returns>
		ISearchPredicate<TViewItem, TViewItemSearchCriteria> GetAffectedViewItemPredicate(EntityChange change, Entity changedEntity);

		/// <summary>
		/// Called to process the specified view item in response to the specified change.
		/// </summary>
		/// <remarks>
		/// This method may wish to add, update, or remove items from the view.
		/// To add or remove a view item, use the collections available on the context interface.
		/// To update the view item, simply update its properties directly.
		/// </remarks>
		/// <param name="change"></param>
		/// <param name="changedEntity"></param>
		/// <param name="viewItem"></param>
		/// <param name="context"></param>
		void ProcessItem(EntityChange change, Entity changedEntity, TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context);

		/// <summary>
		/// Called to process the specified change in the event that no view items associated with the changed entity were found.
		/// </summary>
		/// <remarks>
		/// This method may wish to add or remove items from the view.
		/// To add or remove a view item, use the collections available on the context interface.
		/// </remarks>
		/// <param name="change"></param>
		/// <param name="changedEntity"></param>
		/// <param name="context"></param>
		void ProcessNoItems(EntityChange change, Entity changedEntity, IEntityChangeHandlerProcessContext<TViewItem> context);
	}

	/// <summary>
	/// Defines a callback interface that is passed to
	///  <see cref="IEntityChangeHandler{TViewItem,TViewItemSearchCriteria}.ProcessItem"/> and
	///  <see cref="IEntityChangeHandler{TViewItem,TViewItemSearchCriteria}.ProcessNoItems"/>.
	/// </summary>
	/// <typeparam name="TViewItem"></typeparam>
	public interface IEntityChangeHandlerProcessContext<TViewItem>
	{
		/// <summary>
		/// Gets the collection of items that will be added to the view in response to this change.
		/// </summary>
		/// <remarks>
		/// Handlers that wish to add items to the view must add them to this collection.
		/// </remarks>
		ICollection<TViewItem> AddedItems { get; }

		/// <summary>
		/// Gets the collection of items that will be removed from the view in response to this change.
		/// </summary>
		/// <remarks>
		/// Handlers that wish to remove items from the view must add them to this collection.
		/// </remarks>
		ICollection<TViewItem> RemovedItems { get; }

		/// <summary>
		/// Provides handlers with access to persistence context.
		/// </summary>
		IPersistenceContext PersistenceContext { get; }
	}
}
