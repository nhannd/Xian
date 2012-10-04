#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Owls.Brokers;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstract base class providing boiler plate functionatliy for implementations of <see cref="IViewBuilder"/>.
	/// </summary>
	public abstract class ViewBuilderBase : IViewBuilder
	{
		#region ViewBuildMapping

		/// <summary>
		/// Represents a mapping between view item class, and the root entity and source broker used to generate instances of the view item.
		/// </summary>
		protected abstract class ViewBuildMapping
		{
			/// <summary>
			/// Gets the view item class.
			/// </summary>
			public abstract Type ViewItemClass { get; }

			/// <summary>
			/// Gets the root entity class.
			/// </summary>
			public abstract Type RootEntityClass { get; }

			/// <summary>
			/// Gets the source broker interface.
			/// </summary>
			public abstract Type SourceBrokerInterface { get; }
		}

		/// <summary>
		/// Represents a mapping between view item class, and the root entity and source broker used to generate instances of the view item.
		/// </summary>
		protected class ViewBuildMapping<TRootEntity, TViewItem, TSourceBroker> : ViewBuildMapping
			where TRootEntity : Entity
			where TViewItem : Entity
			where TSourceBroker : IViewSourceBroker
		{
			public override Type ViewItemClass
			{
				get { return typeof (TViewItem); }
			}
			public override Type RootEntityClass
			{
				get { return typeof (TRootEntity); }
			}
			public override Type SourceBrokerInterface
			{
				get { return typeof (TSourceBroker); }
			}
		}

		#endregion

		#region BuildAction classes

		/// <summary>
		/// Abstract base implementation of <see cref="IViewBuildAction"/>.
		/// </summary>
		abstract class BuildAction : IViewBuildAction
		{
			/// <summary>
			/// Gets or sets the elevated connection string that has sufficient privileges to drop and re-create tables and indexes.
			/// </summary>
			public string ElevatedConnectionString { get; set; }

			/// <summary>
			/// Executes this action.
			/// </summary>
			/// <param name="context"></param>
			/// <param name="progressCallback"></param>
			public abstract void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback);

			/// <summary>
			/// Reports the specified message to the specified progress callback function.
			/// </summary>
			/// <param name="progressCallback"></param>
			/// <param name="message"></param>
			/// <param name="args"></param>
			protected static void Report(Action<PopulateViewProgress> progressCallback, string message, params object[] args)
			{
				progressCallback(new PopulateViewProgress(string.Format(message, args)));
			}
		}

		/// <summary>
		/// Action to drop and re-create a view table.
		/// </summary>
		class DropAndRecreateTableAction : BuildAction
		{
			private readonly Type _viewItemClass;

			internal DropAndRecreateTableAction(Type viewItemClass)
			{
				_viewItemClass = viewItemClass;
			}

			public override void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback)
			{
				var ddlBroker = context.GetBroker<IViewDdlBroker>();

				// drop existing table
				Report(progressCallback, "Dropping table for {0}...", _viewItemClass.Name);
				ddlBroker.DropTable(_viewItemClass, new ViewDdlBrokerOptions {ElevatedConnectionString = this.ElevatedConnectionString});
				Report(progressCallback, "Dropped table for {0}.", _viewItemClass.Name);

				// re-create table
				Report(progressCallback, "Creating table for {0}...", _viewItemClass.Name);
				ddlBroker.CreateTable(_viewItemClass, new ViewDdlBrokerOptions { ElevatedConnectionString = this.ElevatedConnectionString });
				Report(progressCallback, "Created table for {0}.", _viewItemClass.Name);
			}
		}

		/// <summary>
		/// Action to add indexes to a view table.
		/// </summary>
		class AddIndexesAction : BuildAction
		{
			private readonly Type _viewItemClass;

			internal AddIndexesAction(Type viewItemClass)
			{
				_viewItemClass = viewItemClass;
			}

			public override void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback)
			{
				var ddlBroker = context.GetBroker<IViewDdlBroker>();

				// add the indexes
				Report(progressCallback, "Creating indexes for {0}...", _viewItemClass.Name);
				ddlBroker.AddIndexes(_viewItemClass, new ViewDdlBrokerOptions { ElevatedConnectionString = this.ElevatedConnectionString });
				Report(progressCallback, "Created indexes for {0}.", _viewItemClass.Name);
			}
		}

		/// <summary>
		/// Action to drop indexes from a view table.
		/// </summary>
		class DropIndexesAction : BuildAction
		{
			private readonly Type _viewItemClass;

			internal DropIndexesAction(Type viewItemClass)
			{
				_viewItemClass = viewItemClass;
			}

			public override void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback)
			{
				var ddlBroker = context.GetBroker<IViewDdlBroker>();

				// drop the indexes
				Report(progressCallback, "Dropping indexes for {0}...", _viewItemClass.Name);
				ddlBroker.DropIndexes(_viewItemClass, new ViewDdlBrokerOptions { ElevatedConnectionString = this.ElevatedConnectionString });
				Report(progressCallback, "Dropped indexes for {0}.", _viewItemClass.Name);
			}
		}

		/// <summary>
		/// Action to populate a view table from source data.
		/// </summary>
		class PopulateTableAction : BuildAction
		{
			private readonly ViewBuildMapping _mapping;

			internal PopulateTableAction(ViewBuildMapping mapping)
			{
				_mapping = mapping;
			}

			internal int ReadBatchSize { get; set; }
			internal int WriteBatchSize { get; set; }
			internal ISearchPredicate InclusionPredicate { get; set; }

			public override void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback)
			{
				// populate table
				var buildViewArgs = new PopulateViewArgs(_mapping.RootEntityClass, InclusionPredicate)
				                    	{
				                    		ReadBatchSize = ReadBatchSize,
				                    		WriteBatchSize = WriteBatchSize,
				                    		ProgressCallback = progressCallback
				                    	};

				Report(progressCallback, "Populating view {0} from root {1}...", _mapping.ViewItemClass.Name, _mapping.RootEntityClass.Name);

				var sourceBroker = (IViewSourceBroker)context.GetBroker(_mapping.SourceBrokerInterface);
				sourceBroker.PopulateView(buildViewArgs);

				Report(progressCallback, "Populated view {0} from root {1}.", _mapping.ViewItemClass.Name, _mapping.RootEntityClass.Name);
			}
		}

		#endregion


		private readonly ISearchPredicate _inclusionPredicate;

		protected ViewBuilderBase(ISearchPredicate inclusionPredicate)
		{
			_inclusionPredicate = inclusionPredicate;
		}

		#region IViewBuilder members

		public IViewBuildAction[] CreateBuildActions(CreateBuildActionsArgs args)
		{
			return GetBuildActions(args, GetViewBuildMappings());
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain a set of view build mappings that define how the view is built from source data.
		/// </summary>
		/// <returns></returns>
		protected abstract IList<ViewBuildMapping> GetViewBuildMappings();

		#endregion

		#region Helpers

		/// <summary>
		/// Returns the appropriate set of build actions required to carry out the requested action.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="viewBuildMappings"></param>
		/// <returns></returns>
		private IViewBuildAction[] GetBuildActions(CreateBuildActionsArgs args, IEnumerable<ViewBuildMapping> viewBuildMappings)
		{
			// create the set of build actions
			var actions = new List<IViewBuildAction>();

			var invertedMap = CollectionUtils.GroupBy(viewBuildMappings, vbm => vbm.ViewItemClass);
			foreach (var viewItemClass in invertedMap.Keys)
			{
				if(args.Action == UtilityAction.Build)
				{
					// drop and recreate table for each class
					actions.Add(new DropAndRecreateTableAction(viewItemClass) { ElevatedConnectionString = args.ElevatedConnectionString });

					// populate for each ViweBuildMapping
					var mappings = invertedMap[viewItemClass];
					foreach (var vbm in mappings)
					{
						var p = new PopulateTableAction(vbm)
						{
							ReadBatchSize = args.ReadBatchSize,
							WriteBatchSize = args.WriteBatchSize,
							ElevatedConnectionString = args.ElevatedConnectionString,
							InclusionPredicate = _inclusionPredicate,
						};

						actions.Add(p);
					}

					// add indexes to table for each class
					// note: we do not add constraints (foreign and unique keys) as they are not relevant for owls tables
					// and will negatively impact performance
					actions.Add(new AddIndexesAction(viewItemClass) { ElevatedConnectionString = args.ElevatedConnectionString });
				}
				else if (args.Action == UtilityAction.Reindex)
				{
					// drop the indexes except for the PK
					actions.Add(new DropIndexesAction(viewItemClass) { ElevatedConnectionString = args.ElevatedConnectionString });
					
					// add indexes, except for the PK
					// note: we do not add constraints (foreign and unique keys) as they are not relevant for owls tables
					// and will negatively impact performance
					actions.Add(new AddIndexesAction(viewItemClass) { ElevatedConnectionString = args.ElevatedConnectionString });
				}

			}

			return actions.ToArray();
		}

		#endregion
	}
}
