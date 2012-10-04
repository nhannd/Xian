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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines an extension point for OWLS views.
	/// </summary>
	[ExtensionPoint]
	public class WorklistViewExtensionPoint : ExtensionPoint<IView>
	{
	}

	/// <summary>
	/// Implemementation of <see cref="IEntityChangeSetListener"/> that is responsible
	/// for updating worklist views.
	/// </summary>
	[ExtensionOf(typeof(EntityChangeSetListenerExtensionPoint))]
	public class WorklistViewChangeSetListener : IEntityChangeSetListener
	{
		#region IEntityChangeSetListener Members

		/// <summary>
		/// Occurs immediately prior to committing a change-set.
		/// </summary>
		/// <param name="args"></param>
		public void PreCommit(EntityChangeSetPreCommitArgs args)
		{
			UpdateViews(args.ChangeSet, args.PersistenceContext);
		}

		public void PostCommit(EntityChangeSetPostCommitArgs args)
		{
		}

		#endregion

		private static void UpdateViews(EntityChangeSet changeSet, IPersistenceContext context)
		{
			// check if any view updaters are interested in this change-set
			var updaters = CollectionUtils.Select(GetViewUpdaters(), u => u.IsUpdateRequired(changeSet));

			// if so, give those updaters a chance to update their views accordingly
			if (updaters.Count > 0)
			{
				foreach (var updater in updaters)
				{
					updater.Update(changeSet, context);
				}
			}
		}

		private static IEnumerable<IViewUpdater> GetViewUpdaters()
		{
			return CollectionUtils.Map<IView, IViewUpdater>(
				new WorklistViewExtensionPoint().CreateExtensions(), view => view.CreateUpdater());
		}
	}
}
