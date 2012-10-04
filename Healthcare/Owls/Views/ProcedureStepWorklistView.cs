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
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls.Views
{
	/// <summary>
	/// Implements the ProcedureStep worklist view, which supports all procedure-step based worklists.
	/// </summary>
	[ExtensionOf(typeof(WorklistViewExtensionPoint))]
	public class ProcedureStepWorklistView : ViewBase
	{
		/// <summary>
		/// Defines the predicate that determines which procedure steps are represented in the view.
		/// </summary>
		class InclusionPredicate : ViewInclusionPredicate<ProcedureStep, ProcedureStepSearchCriteria>
		{
			public InclusionPredicate(TimeSpan retentionTime)
				: base(retentionTime, criteria => criteria.EndTime)
			{
			}
		}

		/// <summary>
		/// Defines the predicate that determines whether an existing view item should be removed from this view.
		/// </summary>
		class ShrinkPredicate : ViewExclusionPredicate<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria>
		{
			public ShrinkPredicate(TimeSpan retentionTime)
				: base(retentionTime, criteria => criteria.ProcedureStep.EndTime)
			{
			}
		}

		/// <summary>
		/// Implementation of <see cref="IViewShrinker"/> for this view.
		/// </summary>
		class ViewShrinker : ViewShrinkerBase<ProcedureStepWorklistViewItem, ProcedureStepWorklistViewItemSearchCriteria, IProcedureStepWorklistViewItemBroker>
		{
			public ViewShrinker(TimeSpan retentionTime)
				: base(new ShrinkPredicate(retentionTime))
			{
			}
		}

		/// <summary>
		/// Implementation of <see cref="IViewBuilder"/> for this view.
		/// </summary>
		class ViewBuilder : ViewBuilderBase
		{
			public ViewBuilder(TimeSpan retentionTime)
				: base(new InclusionPredicate(retentionTime))
			{
			}

			/// <summary>
			/// Called to obtain a set of view build mappings that define how the view is built from source data.
			/// </summary>
			/// <returns></returns>
			protected override IList<ViewBuildMapping> GetViewBuildMappings()
			{
				// need a separate mapping for each family of procedure steps, because
				// they require different source brokers
				return new ViewBuildMapping[]
				       	{
							new ViewBuildMapping<RegistrationProcedureStep, RegistrationWorklistViewItem, IRegistrationWorklistViewSourceBroker>(),
							new ViewBuildMapping<ModalityProcedureStep, ModalityWorklistViewItem, IModalityWorklistViewSourceBroker>(),
							new ViewBuildMapping<DocumentationProcedureStep, ModalityWorklistViewItem, IModalityWorklistViewSourceBroker>(),
							new ViewBuildMapping<ProtocolProcedureStep, ProtocolWorklistViewItem, IProtocolWorklistViewSourceBroker>(),
							new ViewBuildMapping<ReportingProcedureStep, ReportingWorklistViewItem, IReportingWorklistViewSourceBroker>(),
				       	};
			}

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProcedureStepWorklistView()
		{
			this.Settings = new OwlsViewSettings();
		}

		/// <summary>
		/// Gets the builder for this view.
		/// </summary>
		/// <returns></returns>
		public override IViewBuilder CreateBuilder()
		{
			return new ViewBuilder(RetentionTime);
		}

		/// <summary>
		/// Gets the updater for this view.
		/// </summary>
		/// <returns></returns>
		public override IViewUpdater CreateUpdater()
		{
			return new ProcedureStepWorklistViewUpdater(new InclusionPredicate(RetentionTime));
		}

		/// <summary>
		/// Gets the shrinker for this view.
		/// </summary>
		/// <returns></returns>
		public override IViewShrinker CreateShrinker()
		{
			return new ViewShrinker(RetentionTime);
		}

		/// <summary>
		/// Gets the retention time.
		/// </summary>
		private TimeSpan RetentionTime
		{
			get { return TimeSpan.FromDays(Settings.WorklistViewItemRetentionTime); }
		}

		/// <summary>
		/// Gets the owls settings.
		/// </summary>
		private OwlsViewSettings Settings { get; set; }
	}
}
