#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls.Views
{
	/// <summary>
	/// Implementation of <see cref="IViewUpdater"/> for <see cref="ProcedureSearchView"/>.
	/// </summary>
	internal class ProcedureSearchViewUpdater : WorklistViewBaseUpdater<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria, IProcedureSearchViewItemBroker>
	{
		private readonly ISearchPredicate _inclusionPredicate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inclusionPredicate">Predicate that determines whether a given Procedure should be represented in the view.</param>
		internal ProcedureSearchViewUpdater(ISearchPredicate inclusionPredicate)
		{
			_inclusionPredicate = inclusionPredicate;
		}

		/// <summary>
		/// Gets the set of change handlers that may update this view.
		/// </summary>
		protected override IList<IEntityChangeHandler<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria>> ChangeHandlers
		{
			get
			{
				// add an active Procedure handler to the inherited set of handlers
				var baseHandlers = base.ChangeHandlers;
				baseHandlers.Add(CreateProcedureChangeHandler());
				return baseHandlers;
			}
		}

		/// <summary>
		/// Creates a change handler that adds, updates and deletes view items in response to changes in Procedure entities.
		/// </summary>
		/// <returns></returns>
		private IEntityChangeHandler<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria> CreateProcedureChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria, Procedure>.CreateActive(
				entity => MakePredicate<ProcedureSearchViewItemSearchCriteria>(criteria => criteria.Procedure.Instance.EqualTo(entity)),
				_inclusionPredicate,
				AddViewItems,
				(viewItem, entity) => viewItem.SetProcedureInfo(entity, true));
		}

		/// <summary>
		/// Helper method to add view items.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static IList<ProcedureSearchViewItem> AddViewItems(Procedure entity, IPersistenceContext persistenceContext)
		{
			var sourceBroker = persistenceContext.GetBroker<IProcedureSearchViewSourceBroker>();
			var items = sourceBroker.GetViewItems(entity);

			// a Procedure should always lead to a row- if we don't get one, this represents some kind of error
			if (items.Count == 0)
				throw new OwlsViewUpdaterException("Failed to create view items because source broker returned 0 rows.");

			return items;
		}
	}
}
