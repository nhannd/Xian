#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstract base class that supplies most of the functionality required to 
	/// implement <see cref="IEntityChangeHandler{TViewItem,TViewItemSearchCriteria}"/> for a given class of entity.
	/// </summary>
	/// <typeparam name="TViewItem">Class of view item that the change handler is responsible for updating.</typeparam>
	/// <typeparam name="TViewItemSearchCriteria">Search criteria class for view item.</typeparam>
	/// <typeparam name="TEntity">Class of entity that the change handler handles changes to.</typeparam>
	public abstract class EntityChangeHandler<TViewItem, TViewItemSearchCriteria, TEntity> : IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>
		where TViewItem : Entity
		where TViewItemSearchCriteria : EntitySearchCriteria, new()
		where TEntity : Entity
	{

		#region IEntityChangeHandler Members

		/// <summary>
		/// Gets a value indicating if this handler is interested in the specified change.
		/// </summary>
		/// <param name="change"></param>
		/// <returns></returns>
		public virtual bool IsInterested(EntityChange change)
		{
			if (IgnoreCreates && change.ChangeType == EntityChangeType.Create)
				return false;
			if (IgnoreDeletes && change.ChangeType == EntityChangeType.Delete)
				return false;

			return IsInterestingEntityClass(change.GetEntityClass());
		}

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
		public ISearchPredicate<TViewItem, TViewItemSearchCriteria> GetAffectedViewItemPredicate(EntityChange change, Entity changedEntity)
		{
			return GetAffectedViewItemPredicateCore(changedEntity.As<TEntity>());
		}

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
		public void ProcessItem(EntityChange change, Entity changedEntity, TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			ProcessItemCore(change, changedEntity.As<TEntity>(), viewItem, context);
		}

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
		public void ProcessNoItems(EntityChange change, Entity changedEntity,IEntityChangeHandlerProcessContext<TViewItem> context)
		{
			ProcessNoItemsCore(change, changedEntity.As<TEntity>(), context);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the search predicate for finding view items that may be affected by the specified
		/// entity instance.
		/// </summary>
		/// <param name="entity"></param>
		protected abstract ISearchPredicate<TViewItem, TViewItemSearchCriteria> GetAffectedViewItemPredicateCore(TEntity entity);

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
		protected abstract void ProcessItemCore(EntityChange change, TEntity changedEntity, TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context);

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
		protected abstract void ProcessNoItemsCore(EntityChange change, TEntity changedEntity, IEntityChangeHandlerProcessContext<TViewItem> context);

		/// <summary>
		/// Gets a value indicating whether to ignore creations of new entities.
		/// Subclasses should set this value once in the constructor.
		/// </summary>
		protected bool IgnoreCreates { get; set; }

		/// <summary>
		/// Gets a value indicating whether to ignore deletions of entities.
		/// Subclasses should set this value once in the constructor.
		/// </summary>
		protected bool IgnoreDeletes { get; set; }

		#endregion

		#region Helpers

		private static bool IsInterestingEntityClass(Type entityClass)
		{
			// either the class of entity that we are responsible for, or a subclass of that class
			return Equals(entityClass, typeof(TEntity)) || entityClass.IsSubclassOf(typeof(TEntity));
		}

		#endregion

	}
}
