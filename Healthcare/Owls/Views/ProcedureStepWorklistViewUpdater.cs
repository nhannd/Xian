#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls.Views
{
	/// <summary>
	/// Implementation of <see cref="IViewUpdater"/> for <see cref="ProcedureStepWorklistView"/>.
	/// </summary>
	internal class ProcedureStepWorklistViewUpdater : WorklistViewBaseUpdater<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, IProcedureStepWorklistViewItemBroker>
	{
		private readonly ISearchPredicate _inclusionPredicate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inclusionPredicate"></param>
		internal ProcedureStepWorklistViewUpdater(ISearchPredicate inclusionPredicate)
		{
			_inclusionPredicate = inclusionPredicate;
		}

		/// <summary>
		/// Gets the set of change handlers that may update this view.
		/// </summary>
		protected override IList<IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria>> ChangeHandlers
		{
			get
			{
				var baseHandlers = base.ChangeHandlers;

				// add handler for changes to procedures
				baseHandlers.Add(CreateProcedureChangeHandler());
				baseHandlers.Add(CreateProcedureCheckInChangeHandler());
				
				// add handlers for changes to reports, protocols
				baseHandlers.Add(CreateReportPartChangeHandler());
				baseHandlers.Add(CreateReportChangeHandler());
				baseHandlers.Add(CreateProtocolChangeHandler());

				// add active procedure step handlers for each sub-class family
				baseHandlers.Add(CreateProcedureStepChangeHandler<RegistrationProcedureStep>(AddRegistrationViewItems));
				baseHandlers.Add(CreateProcedureStepChangeHandler<ModalityProcedureStep>(AddModalityViewItems));
				baseHandlers.Add(CreateProcedureStepChangeHandler<DocumentationProcedureStep>(AddModalityViewItems));
				baseHandlers.Add(CreateProcedureStepChangeHandler<ProtocolProcedureStep>(AddProtocolViewItems));
				baseHandlers.Add(CreateProcedureStepChangeHandler<ReportingProcedureStep>(AddReportingViewItems));
				
				return baseHandlers;
			}
		}

		#region Change Handler Factory methods

		/// <summary>
		/// Creates a change handler that adds, updates and deletes view items in response to changes in ProcedureStep entities.
		/// </summary>
		/// <returns></returns>
		private IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateProcedureStepChangeHandler<TProcedureStep>(
			DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, TProcedureStep>.AddViewItemsDelegate addViewItemsDelegate)
			where TProcedureStep : ProcedureStep
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, TProcedureStep>
				.CreateActive(
					entity => MakePredicate<ProcedureStepWorklistViewItemSearchCriteria>(criteria => criteria.ProcedureStep.Instance.EqualTo(entity)),
					_inclusionPredicate,
					addViewItemsDelegate,
					(viewItem, entity) => viewItem.SetProcedureStepInfo(entity, true)
				);
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in ProcedureCheckIn entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateProcedureCheckInChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, ProcedureCheckIn>.CreatePassive(
				entity => MakePredicate<ProcedureStepWorklistViewItemSearchCriteria>(criteria => criteria.ProcedureCheckIn.Instance.EqualTo(entity)),
				(viewItem, entity) => viewItem.SetCheckInInfo(entity));
		}


		/// <summary>
		/// Creates a change handler that updates view items in response to changes in Procedure entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateProcedureChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, Procedure>
				.CreatePassive(
					entity => MakePredicate<ProcedureStepWorklistViewItemSearchCriteria>(criteria => criteria.Procedure.Instance.EqualTo(entity)),
					(viewItem, entity) => viewItem.SetProcedureInfo(entity, true)
				);
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in Protocol entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateProtocolChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, Protocol>
				.CreatePassive(
					entity => MakePredicate<ProtocolWorklistViewItemSearchCriteria>(criteria => criteria.Protocol.Instance.EqualTo(entity)),
					(viewItem, entity) => ((ProtocolWorklistViewItem)viewItem).SetProtocolInfo(entity)
				);
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in ReportPart entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateReportPartChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, ReportPart>
				.CreatePassive(
					entity => MakePredicate<ReportingWorklistViewItemSearchCriteria>(criteria => criteria.ReportPart.Instance.EqualTo(entity)),
					(viewItem, entity) => ((ReportingWorklistViewItem)viewItem).SetReportPartInfo(entity)
				);
		}

		/// <summary>
		/// Creates a change handler that updates view items in response to changes in Report entities.
		/// </summary>
		/// <returns></returns>
		private static IEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria> CreateReportChangeHandler()
		{
			return DefaultEntityChangeHandler<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, Report>
				.CreatePassive(
					entity => MakePredicate<ReportingWorklistViewItemSearchCriteria>(criteria => criteria.Report.Instance.EqualTo(entity)),
					(viewItem, entity) => ((ReportingWorklistViewItem)viewItem).SetReportInfo(entity)
				);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Helper method to add registration view items.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static IList<ProcedureStepWorklistViewItem> AddRegistrationViewItems(ProcedureStep entity, IPersistenceContext persistenceContext)
		{
			return AddViewItems<IRegistrationWorklistViewSourceBroker, RegistrationWorklistViewItem>(entity, persistenceContext);
		}

		/// <summary>
		/// Helper method to add modality worklist view items.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static IList<ProcedureStepWorklistViewItem> AddModalityViewItems(ProcedureStep entity, IPersistenceContext persistenceContext)
		{
			return AddViewItems<IModalityWorklistViewSourceBroker, ModalityWorklistViewItem>(entity, persistenceContext);
		}

		/// <summary>
		/// Helper method to add reporting worklist view items.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static IList<ProcedureStepWorklistViewItem> AddReportingViewItems(ProcedureStep entity, IPersistenceContext persistenceContext)
		{
			return AddViewItems<IReportingWorklistViewSourceBroker, ReportingWorklistViewItem>(entity, persistenceContext);
		}

		/// <summary>
		/// Helper method to add protocol worklist view items.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static IList<ProcedureStepWorklistViewItem> AddProtocolViewItems(ProcedureStep entity, IPersistenceContext persistenceContext)
		{
			return AddViewItems<IProtocolWorklistViewSourceBroker, ProtocolWorklistViewItem>(entity, persistenceContext);
		}

		/// <summary>
		/// Helper method to add view items
		/// </summary>
		/// <typeparam name="TSourceBroker"></typeparam>
		/// <typeparam name="TViewItem"></typeparam>
		/// <param name="step"></param>
		/// <param name="brokerFactory"></param>
		/// <returns></returns>
		private static IList<ProcedureStepWorklistViewItem> AddViewItems<TSourceBroker, TViewItem>(ProcedureStep step, IPersistenceBrokerFactory brokerFactory)
			where TSourceBroker : IViewSourceBroker<TViewItem, ProcedureStep>
			where TViewItem : ProcedureStepWorklistViewItem, new()
		{
			var sourceBroker = brokerFactory.GetBroker<TSourceBroker>();
			var items = sourceBroker.GetViewItems(step);
			// a ProcedureStep should always lead to a row- if we don't get one, this represents some kind of error
			if (items.Count == 0)
				throw new OwlsViewUpdaterException("Failed to create view items because source broker returned 0 rows.");

			return CollectionUtils.Map<TViewItem, ProcedureStepWorklistViewItem>(items, item => item);
		}

		#endregion
	}
}
