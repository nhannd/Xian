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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
	/// Defines the interface for a publication action, that is, an action to be taken
    /// upon publication of a radiololgy report.
	/// </summary>
	public interface IPublicationAction
	{
		void Execute(PublicationStep step, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class PublicationActionExtensionPoint : ExtensionPoint<IPublicationAction>
	{
	}

    /// <summary>
    /// Processes <see cref="PublicationStep"/>s, performing all <see cref="IPublicationAction"/>s
    /// on each step.
    /// </summary>
    internal class PublicationProcessor : EntityQueueProcessor<PublicationStep>
	{
		private readonly object[] _publicationActions;
		private readonly PublicationShredSettings _settings;

        public PublicationProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
        {
        	_settings = settings;
            _publicationActions = new PublicationActionExtensionPoint().CreateExtensions();
        }

		public override string Name
		{
			get
			{
				return SR.PublicationShredName;
			}
		}

		protected override IList<PublicationStep> GetNextEntityBatch(int batchSize)
		{
			// Get scheduled steps, where the "publishing cool-down" has elapsed
			// eg LastFailureTime is more than a specified number of seconds ago

			var page = new SearchResultPage(0, batchSize);
			return PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().FindUnprocessedSteps(_settings.FailedItemRetryDelay, page);
		}

		protected override void MarkItemClaimed(PublicationStep item)
		{
			// do nothing
		}

		protected override void ActOnItem(PublicationStep item)
		{
			// execute each publication action
			foreach (IPublicationAction action in _publicationActions)
			{
				action.Execute(item, PersistenceScope.CurrentContext);
			}
		}

		protected override void OnItemFailed(PublicationStep item, Exception error)
		{
			// mark item as failed
			item.Fail();
		}

		protected override void OnItemSucceeded(PublicationStep item)
		{
			// all actions succeeded, so mark the publication item as being completed
			item.Complete(item.AssignedStaff);
		}
	}
}
