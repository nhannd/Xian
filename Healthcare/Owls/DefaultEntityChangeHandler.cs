#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Default generic implementation of <see cref="IEntityChangeHandler{TViewItem,TViewItemSearchCriteria}"/>. 
	/// </summary>
	/// <typeparam name="TViewItem">View item class to be updated.</typeparam>
	/// <typeparam name="TViewItemSearchCriteria">Search criteria for view item class to be updated.</typeparam>
	/// <typeparam name="TEntity">Entity class to handle changes for.</typeparam>
	public class DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, TEntity>
		: EntityChangeHandler<TViewItem, TViewItemSearchCriteria, TEntity>
		where TViewItem : Entity
		where TViewItemSearchCriteria : EntitySearchCriteria, new()
		where TEntity : Entity
	{
		public delegate void UpdateViewItemDelegate(TViewItem viewItem, TEntity entity);
		public delegate IList<TViewItem> AddViewItemsDelegate(TEntity entity, IPersistenceContext persistenceContext);


		private readonly Converter<TEntity, ISearchPredicate<TViewItem, TViewItemSearchCriteria>> _affectedViewItemsPredicateProvider;
		private readonly ISearchPredicate _inclusionPredicate;
		private readonly UpdateViewItemDelegate _updateViewItemHandler;
		private readonly AddViewItemsDelegate _addViewItemsHandler;

		#region Public API

		/// <summary>
		/// Creates a change handler that behaves passively.
		/// </summary>
		/// <param name="affectedViewItemsPredicateProvider"></param>
		/// <param name="updateViewItemHandler">Method to update an existing view item in response to a change in the source entity.</param>
		/// <remarks>
		/// A passive handler cannot add or remove view items.  It can only update existing view items in response to 
		/// a change in the source data.
		/// </remarks>
		/// <returns></returns>
		public static IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> CreatePassive(
			Converter<TEntity, ISearchPredicate<TViewItem, TViewItemSearchCriteria>> affectedViewItemsPredicateProvider,
			UpdateViewItemDelegate updateViewItemHandler)
		{
			// a passive handler cannot add new items to the view,
			// so we can safely ignore newly created entities
			return new DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, TEntity>(
				affectedViewItemsPredicateProvider,
				null,
				null,
				updateViewItemHandler) {Passive = true, IgnoreCreates = true};
		}

		/// <summary>
		/// Creates a handler with active behaviour.
		/// </summary>
		/// <param name="affectedViewItemsPredicateProvider"></param>
		/// <param name="inclusionPredicate">Predicate that determines whether a given entity instance should be represented in the view.</param>
		/// <param name="addViewItemsHandler">Function to create view items corresponding to a given root entity instance.</param>
		/// <param name="updateViewItemHandler">Method to update an existing view item in response to a change in the source entity.</param>
		/// <remarks>
		/// An active handler is one which can add or remove items from the view, in addition to updating existing view items.
		/// </remarks>
		/// <returns></returns>
		public static IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> CreateActive(
			Converter<TEntity, ISearchPredicate<TViewItem, TViewItemSearchCriteria>> affectedViewItemsPredicateProvider,
			ISearchPredicate inclusionPredicate,
			AddViewItemsDelegate addViewItemsHandler,
			UpdateViewItemDelegate updateViewItemHandler)
		{
			return new DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, TEntity>(
				affectedViewItemsPredicateProvider,
				inclusionPredicate,
				addViewItemsHandler,
				updateViewItemHandler);
		}

		#endregion

		#region Protected API


		/// <summary>
		/// Gets or sets a value indicating whether this instance behaves passively or actively.
		/// </summary>
		protected internal bool Passive { get; set; }

		/// <summary>
		/// Called to obtain the search criteria for finding view items that may be affected by the specified
		/// entity instance.
		/// </summary>
		/// <param name="entity"></param>
		protected override ISearchPredicate<TViewItem, TViewItemSearchCriteria> GetAffectedViewItemPredicateCore(TEntity entity)
		{
			return _affectedViewItemsPredicateProvider(entity);
		}

		/// <summary>
		/// Called to process the specified entity change.
		/// </summary>
		/// <remarks>
		/// This method will be called once for each view item that is affected by a given changed entity.
		/// The method may respond by a) updating the specified view item directly, b) adding new view items
		/// via the context, or c) removing the specified view item via the context.
		/// </remarks>
		/// <param name="change"></param>
		/// <param name="changedEntity"></param>
		/// <param name="viewItem"></param>
		/// <param name="context"></param>
		protected override void ProcessItemCore(EntityChange change, TEntity changedEntity, TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			// if passive, we can only update
			if(Passive)
			{
				Update(changedEntity, viewItem);
				return;
			}

			// update the item if the entity is still a member of the view, otherwise remove it
			if(IsMember(changedEntity))
			{
				Update(changedEntity, viewItem);
			}
			else
			{
				Remove(viewItem, context);
			}
		}

		/// <summary>
		/// Called to process the specified entity change.
		/// </summary>
		/// <remarks>
		/// This method will be invoked in the case where the changed entity has no existing affected view items.
		/// This allows the method to add new items to the view even when there are no existing items
		/// associated with the changed entity.
		/// </remarks>
		/// <param name="change"></param>
		/// <param name="changedEntity"></param>
		/// <param name="context"></param>
		protected override void ProcessNoItemsCore(EntityChange change, TEntity changedEntity, IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			// if passive, we can't add new items
			if(Passive)
				return;

			// if the entity is a member, add new view item(s)
			if (IsMember(changedEntity))
			{
				Add(changedEntity, context);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="affectedViewItemsPredicateProvider"></param>
		/// <param name="inclusionPredicate"></param>
		/// <param name="addViewItemsHandler"></param>
		/// <param name="updateViewItemHandler"></param>
		private DefaultEntityChangeHandler(
			Converter<TEntity, ISearchPredicate<TViewItem, TViewItemSearchCriteria>> affectedViewItemsPredicateProvider,
			ISearchPredicate inclusionPredicate,
			AddViewItemsDelegate addViewItemsHandler,
			UpdateViewItemDelegate updateViewItemHandler)
		{
			_affectedViewItemsPredicateProvider = affectedViewItemsPredicateProvider;
			_inclusionPredicate = inclusionPredicate;
			_addViewItemsHandler = addViewItemsHandler;
			_updateViewItemHandler = updateViewItemHandler;

			// we don't bother handling deletes, because they don't occur in practice
			this.IgnoreDeletes = true;
		}

		/// <summary>
		/// Used by active handler to test if the specified root entity instance is included in the view.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private bool IsMember(TEntity instance)
		{
			return _inclusionPredicate.Test(instance);
		}

		/// <summary>
		/// Used by active handlers to add new view items in response to the specified root entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="context"></param>
		private void Add(TEntity entity, IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			var items = _addViewItemsHandler(entity, context.PersistenceContext);
			foreach (var item in items)
			{
				context.AddedItems.Add(item);
			}
		}

		/// <summary>
		/// Used by active handlers to remove the specified view item from the view.
		/// </summary>
		/// <param name="viewItem"></param>
		/// <param name="context"></param>
		private static void Remove(TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			context.RemovedItems.Add(viewItem);
		}

		/// <summary>
		/// Called to update the specified view item in response to a change to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="viewItem"></param>
		private void Update(TEntity entity, TViewItem viewItem)
		{
			_updateViewItemHandler(viewItem, entity);
		}

		#endregion
	}
}
