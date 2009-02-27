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

		protected override IList<PublicationStep> GetNextEntityBatch(int batchSize)
		{
			// Get scheduled steps, where the "publishing cool-down" has elapsed
            // eg LastFailureTime is more than a specified number of seconds ago
			PublicationStepSearchCriteria noFailures = GetCriteria();
			noFailures.LastFailureTime.IsNull();
			noFailures.Scheduling.StartTime.SortAsc(0);

			PublicationStepSearchCriteria failures = GetCriteria();
			failures.LastFailureTime.LessThan(Platform.Time.AddSeconds(-_settings.FailedItemRetryDelay));

			PublicationStepSearchCriteria[] criteria = new PublicationStepSearchCriteria[] { noFailures, failures };
			SearchResultPage page = new SearchResultPage(0, batchSize);

			return PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().Find(criteria, page);
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

		private static PublicationStepSearchCriteria GetCriteria()
		{
			PublicationStepSearchCriteria criteria = new PublicationStepSearchCriteria();
			criteria.State.EqualTo(ActivityStatus.SC);
			criteria.Scheduling.Performer.Staff.IsNotNull();
			criteria.Scheduling.StartTime.LessThan(Platform.Time);
			return criteria;
		}
	}
}
