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
	/// Implements the Procedure Search view, which supports searching for procedures that would not otherwise
	/// appear in worklists.
	/// </summary>
	[ExtensionOf(typeof(WorklistViewExtensionPoint))]
	public class ProcedureSearchView : ViewBase
	{
		/// <summary>
		/// Defines the predicate that determines which procedures are represented in the view.
		/// </summary>
		class InclusionPredicate : ViewInclusionPredicate<Procedure, ProcedureSearchCriteria>
		{
			public InclusionPredicate(TimeSpan retentionTime)
				: base(retentionTime, criteria => criteria.EndTime)
			{
			}

			/// <summary>
			/// Called to obtain the criteria that define this predicate.
			/// </summary>
			/// <returns></returns>
			protected override ProcedureSearchCriteria[] GetCriteriaCore()
			{
				var criteria = base.GetCriteriaCore();
				foreach (var c in criteria)
				{
					// ghost procedures are not searchable
					c.Status.NotEqualTo(ProcedureStatus.GH);
				}
				return criteria;
			}
		}

		/// <summary>
		/// Defines the predicate that determines whether an existing view item should be removed from this view.
		/// </summary>
		class ShrinkPredicate : ViewExclusionPredicate<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria>
		{
			public ShrinkPredicate(TimeSpan retentionTime)
				: base(retentionTime, criteria => criteria.Procedure.EndTime)
			{
			}
		}

		/// <summary>
		/// Implementation of <see cref="IViewShrinker"/> for this view.
		/// </summary>
		class ViewShrinker : ViewShrinkerBase<ProcedureSearchViewItem, ProcedureSearchViewItemSearchCriteria, IProcedureSearchViewItemBroker>
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
				return new ViewBuildMapping[]
				       	{
				       		new ViewBuildMapping<Procedure, ProcedureSearchViewItem, IProcedureSearchViewSourceBroker>()
				       	};
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProcedureSearchView()
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
			return new ProcedureSearchViewUpdater(new InclusionPredicate(RetentionTime));
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
			get { return TimeSpan.FromDays(Settings.ProcedureSearchViewItemRetentionTime); }
		}

		/// <summary>
		/// Gets the owls settings.
		/// </summary>
		private OwlsViewSettings Settings { get; set; }
	}
}
