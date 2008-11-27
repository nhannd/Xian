using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds
{
	/// <summary>
	/// Defines the interface for processing publication step after it is published.
	/// </summary>
	public interface IPublicationStepProcessor
	{
		void Process(PublicationStep step, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class PublicationStepProcessorExtensionPoint : ExtensionPoint<IPublicationStepProcessor>
	{
	}

	public class PublicationProcessor : ProcessorBase<PublicationStep>
	{
		private object[] _publicationStepProcessors;

		public override void Initialize(int batchSize, int sleepDurationInSeconds)
		{
			_publicationStepProcessors = new PublicationStepProcessorExtensionPoint().CreateExtensions();

			base.Initialize(batchSize, sleepDurationInSeconds);
		}

		public override IList<PublicationStep> GetNextBatch(int batchSize)
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

		public override void ProcessItem(PublicationStep item)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				try
				{
					context.Lock(item);

					foreach (IPublicationStepProcessor processor in _publicationStepProcessors)
					{
						processor.Process(item, context);
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
