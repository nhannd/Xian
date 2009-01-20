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
    internal class PublicationProcessor : QueueProcessor<PublicationStep>
	{
		private readonly object[] _publicationActions;
		private readonly PublicationShredSettings _settings;

        public PublicationProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
        {
        	_settings = settings;
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
				failures.LastFailureTime.LessThan(Platform.Time.AddSeconds(_settings.FailedItemRetryDelay));

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

					// execute each publication action
					foreach (IPublicationAction action in _publicationActions)
					{
						action.Execute(item, context);
					}

					// all actions succeeded, so mark the publication item as being completed
					item.Complete(item.AssignedStaff);

					// complete the transaction
					scope.Complete();
				}
				catch (Exception e)
				{
					// one of the actions failed
					ExceptionLogger.Log("PublicationProcessor.ProcessItem", e);

					// use a new scope to mark the item as failed
					using (PersistenceScope failScope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
					{
						IUpdateContext failContext = (IUpdateContext) PersistenceScope.CurrentContext;
						failContext.ChangeSetRecorder.OperationName = this.GetType().FullName;

						// mark item as failed, do not complete primary transaction
						item.Fail();

						// complete the failScope transaction
						failScope.Complete();
					}
				}
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
