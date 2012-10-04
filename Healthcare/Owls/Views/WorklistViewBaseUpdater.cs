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
using System;

namespace ClearCanvas.Healthcare.Owls.Views
{
	/// <summary>
	/// Abstract intermediate class for view updaters that operate on subclasses of <see cref="WorklistViewItemBase"/>.
	/// </summary>
	/// <typeparam name="TViewItem"></typeparam>
	/// <typeparam name="TViewItemSearchCriteria"></typeparam>
	/// <typeparam name="TViewItemBroker"></typeparam>
	internal abstract class WorklistViewBaseUpdater<TViewItem, TViewItemSearchCriteria, TViewItemBroker> : ViewUpdaterBase<TViewItem, TViewItemSearchCriteria, TViewItemBroker>
		where TViewItem : WorklistViewItemBase
		where TViewItemSearchCriteria : WorklistViewItemBaseSearchCriteria, new()
		where TViewItemBroker : IEntityBroker<TViewItem, TViewItemSearchCriteria>
	{
		private readonly IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>[] _changeHandlers;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected WorklistViewBaseUpdater()
		{
			// define a base set of change handlers that will be inherited by all subclasses
			// all worklist views must respond to changes in PatientProfile, Visit, and Order entities
			_changeHandlers = new []
          	{
				CreatePatientProfileChangeHandler(),
				CreateVisitChangeHandler(),
				CreateOrderChangeHandler(),
          	};
		}

		#region Protected API

		/// <summary>
		/// Gets the set of change handlers that may update this view.
		/// </summary>
		protected override IList<IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>> ChangeHandlers
		{
			get { return new List<IEntityChangeHandler<TViewItem, TViewItemSearchCriteria>>(_changeHandlers); }
		}

		/// <summary>
		/// Helper method to construct a search predicate of the specified type, invoking the callback to initialize the criteria.
		/// </summary>
		/// <typeparam name="TCriteria"></typeparam>
		/// <param name="action"></param>
		/// <returns>An instance of the specified criteria class, that has been passed to the specified action callback.</returns>
		protected static ISearchPredicate<TViewItem, TViewItemSearchCriteria> MakePredicate<TCriteria>(Action<TCriteria> action)
			where TCriteria : TViewItemSearchCriteria, new()
		{
			var criteria = new TCriteria();
			action(criteria);
			return new StaticSearchPredicate<TViewItem, TViewItemSearchCriteria>(criteria);
		}

		#endregion

		#region Change Handler factory methods

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in PatientProfile entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> CreatePatientProfileChangeHandler()
		{
			return DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, PatientProfile>.CreatePassive(
				entity => MakePredicate<TViewItemSearchCriteria>(criteria => criteria.PatientProfile.Instance.EqualTo(entity)),
				(viewItem, entity) => viewItem.SetPatientProfileInfo(entity));
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in Visit entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> CreateVisitChangeHandler()
		{
			return DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, Visit>.CreatePassive(
				entity => MakePredicate<TViewItemSearchCriteria>(criteria => criteria.Visit.Instance.EqualTo(entity)),
				(viewItem, entity) => viewItem.SetVisitInfo(entity));
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in Order entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<TViewItem, TViewItemSearchCriteria> CreateOrderChangeHandler()
		{
			return DefaultEntityChangeHandler<TViewItem, TViewItemSearchCriteria, Order>.CreatePassive(
				entity => MakePredicate<TViewItemSearchCriteria>(criteria => criteria.Order.Instance.EqualTo(entity)),
				(viewItem, entity) => viewItem.SetOrderInfo(entity, true));
		}

		#endregion
	}
}
