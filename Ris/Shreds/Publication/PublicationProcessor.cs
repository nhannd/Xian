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
    public class PublicationProcessor : QueueProcessor<PublicationStep>
	{
		private object[] _publicationActions;

        public PublicationProcessor(int batchSize, TimeSpan sleepTime)
            :base(batchSize, sleepTime)
        {
            _publicationActions = new PublicationActionExtensionPoint().CreateExtensions();
        }

		protected override IList<PublicationStep> GetNextBatch(int batchSize)
		{
			IList<PublicationStep> items;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				// Get scheduled steps, where the "publishing cool-down" has elapsed
				PublicationStepSearchCriteria noFailures = GetCriteria();
				noFailures.LastFailureTime.IsNull();
				noFailures.Scheduling.StartTime.SortAsc(0);

				PublicationStepSearchCriteria failures = GetCriteria();
				failures.LastFailureTime.IsNotNull();
				failures.LastFailureTime.LessThan(Platform.Time.AddMinutes(-5));

				PublicationStepSearchCriteria[] criteria = new PublicationStepSearchCriteria[] { noFailures, failures };
				SearchResultPage page = new SearchResultPage(0, batchSize);

				items = PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().Find(criteria, page);

				scope.Complete();
			}

			return items;
		}

        protected override void ProcessItem(PublicationStep item)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				try
				{
					context.Lock(item);

					foreach (IPublicationAction action in _publicationActions)
					{
						action.Execute(item, context);
					}

					item.Complete(item.AssignedStaff);
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("PublicationProcessor.ProcessItem", e);
					item.Fail();
				}

				scope.Complete();
			}
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
