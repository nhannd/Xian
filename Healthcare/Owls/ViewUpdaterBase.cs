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
using ClearCanvas.Common.Utilities;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstract base class that supplies most of the functionality required to implement <see cref="IViewUpdater"/>.
	/// </summary>
	/// <typeparam name="TViewItem">Class of view item to update.</typeparam>
	/// <typeparam name="TViewItemSearchCriteria">Search criteria class for view item to update.</typeparam>
	/// <typeparam name="TViewItemBroker">Broker for view item class to update.</typeparam>
	public abstract class ViewUpdaterBase<TViewItem, TViewItemSearchCriteria, TViewItemBroker> : IViewUpdater
		where TViewItem : Entity
		where TViewItemSearchCriteria : EntitySearchCriteria, new()
		where TViewItemBroker : IEntityBroker<TViewItem, TViewItemSearchCriteria>
	{

		#region ChangeHandlerPair helper class

		/// <summary>
		/// Represents a pairing of a <see cref="EntityChange"/> and a <see cref="IEntityChangeHandler{TViewItem,TViewItemSearchCriteria}"/>.
		/// </summary>
		class ChangeHandlerPair
		{
			private readonly EntityChange _change;
			private readonly IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> _handler;
			private readonly IPersistenceContext _context;
			private Entity _entity;

			internal ChangeHandlerPair(EntityChange change, IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> handler,
				IPersistenceContext context)
			{
				_change = change;
				_handler = handler;
				_context = context;
			}

			public ISearchPredicate<TViewItem, TViewItemSearchCriteria> ViewItemPredicate
			{
				get { return _handler.GetAffectedViewItemPredicate(_change, this.Entity); }
			}

			public void Process(TViewItem viewItem, IEntityChangeHandlerProcessContext<TViewItem> context)
			{
				_handler.ProcessItem(_change, this.Entity, viewItem, context);
			}

			public void Process(IEntityChangeHandlerProcessContext<TViewItem> context)
			{
				_handler.ProcessNoItems(_change, this.Entity, context);
			}

			private Entity Entity
			{
				get
				{
					if (_entity == null)
					{
						_entity = _context.Load(_change.EntityRef, EntityLoadFlags.Proxy);
					}
					return _entity;
				}
			}
		}

		#endregion

		#region SafePersistenceContextWrapper helper class

		/// <summary>
		/// Wraps a persistence context, disallowing certain operations.
		/// </summary>
		class SafePersistenceContextWrapper : IPersistenceContext
		{
			private readonly IPersistenceContext _context;

			internal SafePersistenceContextWrapper(IPersistenceContext context)
			{
				_context = context;
			}

			public TBrokerInterface GetBroker<TBrokerInterface>()
				where TBrokerInterface : IPersistenceBroker
			{
				return _context.GetBroker<TBrokerInterface>();
			}

			public object GetBroker(Type brokerInterface)
			{
				return _context.GetBroker(brokerInterface);
			}

			public void Dispose()
			{
				_context.Dispose();
			}

			public TEntity Load<TEntity>(EntityRef entityRef) where TEntity : Entity
			{
				return _context.Load<TEntity>(entityRef);
			}

			public TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags) where TEntity : Entity
			{
				return _context.Load<TEntity>(entityRef, flags);
			}

			public Entity Load(EntityRef entityRef, EntityLoadFlags flags)
			{
				return _context.Load(entityRef, flags);
			}

			public void SynchState()
			{
				// do not allow access to this method, because we do not want to cause allow updates to the database at this point
				throw new InvalidOperationException("This method can not be used here.");
			}

			public void Lock(Entity entity)
			{
				// do not allow access to this method, because we do not want to cause allow updates to the database at this point
				throw new InvalidOperationException("This method can not be used here.");
			}

			public void Lock(Entity entity, DirtyState state)
			{
				// do not allow access to this method, because we do not want to cause allow updates to the database at this point
				throw new InvalidOperationException("This method can not be used here.");
			}

			public void Lock(EnumValue enumValue)
			{
				// do not allow access to this method, because we do not want to cause allow updates to the database at this point
				throw new InvalidOperationException("This method can not be used here.");
			}

		}

		#endregion

		#region ProcessContext helper class

		/// <summary>
		/// Implementation of <see cref="IEntityChangeHandlerProcessContext{TViewItem}"/>.
		/// </summary>
		class ProcessContext : IEntityChangeHandlerProcessContext<TViewItem>
		{
			private readonly IPersistenceContext _persistenceContext;
			private readonly HashedSet<TViewItem> _addedItems = new HashedSet<TViewItem>();
			private readonly HashedSet<TViewItem> _removedItems = new HashedSet<TViewItem>();

			internal ProcessContext(IPersistenceContext persistenceContext)
			{
				// wrap the persistence context so that we provide only restricted access to the handlers
				_persistenceContext = new SafePersistenceContextWrapper(persistenceContext);
			}

			/// <summary>
			/// Gets the collection of items that will be added to the view in response to this change.
			/// </summary>
			/// <remarks>
			/// Handlers that wish to add items to the view must add them to this collection.
			/// </remarks>
			public ICollection<TViewItem> AddedItems
			{
				get { return _addedItems; }
			}

			/// <summary>
			/// Gets the collection of items that will be removed from the view in response to this change.
			/// </summary>
			/// <remarks>
			/// Handlers that wish to remove items from the view must add them to this collection.
			/// </remarks>
			public ICollection<TViewItem> RemovedItems
			{
				get { return _removedItems; }
			}

			/// <summary>
			/// Provides handlers with access to persistence context.
			/// </summary>
			public IPersistenceContext PersistenceContext
			{
				get { return _persistenceContext; }
			}
		}

		#endregion

		#region Constructors

		protected ViewUpdaterBase()
		{
		}

		#endregion

		#region IViewUpdater Members

		/// <summary>
		/// Updates the view in response to the specified change-set.
		/// </summary>
		/// <param name="changeSet"></param>
		/// <param name="context"></param>
		public void Update(EntityChangeSet changeSet, IPersistenceContext context)
		{
			// determine which changes need to be processed, and which handlers are interested in processing them
			var pairs = new List<ChangeHandlerPair>();
			foreach (var change in changeSet.Changes)
			{
				foreach (var u in ChangeHandlers)
				{
					if (u.IsInterested(change))
						pairs.Add(new ChangeHandlerPair(change, u, context));
				}
			}

			// find all affected view items
			var pairAffectedItems = FindAffectedViewItems(context, pairs);

			// allow each change-handler pair to process its affected view items
			var processContext = new ProcessContext(context);
			foreach (var kvp in pairAffectedItems)
			{
				var pair = kvp.Key;
				var affectedItems = kvp.Value;

				// if there are any view items affected, 
				// allow the change-handler pair to process each one
				if (affectedItems.Count > 0)
				{
					foreach (var item in affectedItems)
					{
						pair.Process(item, processContext);
					}
				}
				else
				{
					// otherwise, allow the change-handler pair to process the change with no viewitem
					// this is effectively an outer join against view items
					pair.Process(processContext);
				}
			}

			// do post-processing
			PostProcess(processContext, context);
		}

		/// <summary>
		/// Gets a value indicating whether the view requires an update in response
		/// to the specified change-set.
		/// </summary>
		/// <param name="changeSet"></param>
		/// <returns></returns>
		public bool IsUpdateRequired(EntityChangeSet changeSet)
		{
			// update is required if change set contains a change that any handler is interested in
			return CollectionUtils.Contains(
				changeSet.Changes,
				change => CollectionUtils.Contains(
							ChangeHandlers,
							h => h.IsInterested(change)));
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Gets the set of change handlers that may update this view.
		/// </summary>
		protected abstract IList<IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>> ChangeHandlers { get; }

		#endregion

		#region Helpers

		/// <summary>
		/// For each change-handler pair specified, finds the set of affected view items, returned as deferred queries
		/// that will execute in a single batch when the first result list is accessed.
		/// </summary>
		/// <param name="brokerFactory"></param>
		/// <param name="pairs"></param>
		/// <returns></returns>
		private static Dictionary<ChangeHandlerPair, IList<TViewItem>> FindAffectedViewItems(IPersistenceBrokerFactory brokerFactory, IEnumerable<ChangeHandlerPair> pairs)
		{
			// important to use deferred queries here, so the Find doesn't actually execute the query yet
			var options = new EntityFindOptions { Defer = true };
			var broker = brokerFactory.GetBroker<TViewItemBroker>();
			return CollectionUtils.MakeDictionary(pairs,
					pair => pair,
					pair => broker.Find(pair.ViewItemPredicate.GetSearchCriteria(), options));
		}

		/// <summary>
		/// For each change-handler pair specified, finds the set of affected view items.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="pairs"></param>
		/// <returns></returns>
		// Note: this is the old implementation of FindAffectedViewItems, before deferred queries were available in Enterprise,
		// where we mashed all the criteria together in order to execute a single query.
		// I have left this code here in case we need to revert back, but there are some deficiencies with this algorithm
		// as noted in comments inside the method.
		private static Dictionary<ChangeHandlerPair, IList<TViewItem>> FindAffectedViewItems_SingleQuery(IPersistenceContext context, IEnumerable<ChangeHandlerPair> pairs)
		{
			var criteria = new List<TViewItemSearchCriteria>();
			foreach (var pair in pairs)
			{
				criteria.AddRange(pair.ViewItemPredicate.GetSearchCriteria());
			}
			// this isn't very robust - what if there are 100 items in the criteria array?  (I have observed this causing timeouts)
			// What if there are 10000 results? The single query approach is not seeming like a good idea...
			var items = context.GetBroker<TViewItemBroker>().Find(criteria.ToArray());

			// because all the search criteria were OR'd and executed as a single Find operation,
			// we need to filter the items so that the change-handler pair only sees the ones
			// that match its predicate
			return CollectionUtils.MakeDictionary(pairs,
					  pair => pair,
					  pair => (IList<TViewItem>)CollectionUtils.Select(items, pair.ViewItemPredicate.Test));
		}

		private static void PostProcess(ProcessContext processContext, IPersistenceContext uctxt)
		{
			// process removes
			if (processContext.RemovedItems.Count > 0)
			{
				var broker = uctxt.GetBroker<TViewItemBroker>();
				foreach (var item in processContext.RemovedItems)
				{
					broker.Delete(item);
				}
			}

			// process adds
			foreach (var item in processContext.AddedItems)
			{
				uctxt.Lock(item, DirtyState.New);
			}
		}
		#endregion
	}
}
