using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	public class ImageAvailabilityProcessor
	{
		private volatile bool _shouldStop;
		private Thread _sleeper;

		private int _sleepDuration;
		private int _batchSize;

		private IUpdateImageAvailabilityStrategy _updateStrategy;

		public ImageAvailabilityProcessor()
		{
			_sleeper = null;
		}

		public void Start()
		{
			ImageAvailabilitySettings settings = new ImageAvailabilitySettings();
			_batchSize = settings.BatchSize;
			_sleepDuration = settings.SleepDurationInSeconds * 1000;

			try
			{
				_updateStrategy = (IUpdateImageAvailabilityStrategy)(new UpdateImageAvailabilityStrategyExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				_updateStrategy = new DefaultUpdateImageAvailabilityStrategy();
			}

			while (_shouldStop == false)
			{
				// Schedule a WorkQueueItem for every new procedure
				while (_shouldStop == false)
				{
					IList<Procedure> procedures = NextBatchOfProcedures();

					// quit if no outstanding items
					if (procedures.Count == 0 && _shouldStop == false)
						break;

					ScheduleWorkQueueItems(procedures);
				}

				IList<WorkQueueItem> items = NextBatchOfWorkQueueItems();

				// sleep if no outstanding items
				if (items.Count == 0 && _shouldStop == false)
				{
					_sleeper = new Thread(Sleep);
					_sleeper.Start();
					_sleeper.Join();
					_sleeper = null;
				}

				ProcessWorkQueueItems(items);
			}
		}

		public void RequestStop()
		{
			_shouldStop = true;
			if (_sleeper != null && _sleeper.ThreadState == ThreadState.WaitSleepJoin)
			{
				_sleeper.Abort();
			}
		}

		private void Sleep()
		{
			Thread.Sleep(_sleepDuration);
		}

		private IList<Procedure> NextBatchOfProcedures()
		{
			IList<Procedure> procedures;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					// Find a list of procedures that match the criteria
					ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
					criteria.Status.EqualTo(ProcedureStatus.IP);
					criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

					SearchResultPage page = new SearchResultPage(0, _batchSize);
					procedures = scope.Context.GetBroker<IProcedureBroker>().Find(criteria, page);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfProcedures", e);
					scope.Dispose();
					procedures = new List<Procedure>();
				}
			}

			return procedures;
		}

		private void ScheduleWorkQueueItems(IEnumerable<Procedure> procedures)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)scope.Context;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				foreach (Procedure p in procedures)
				{
					try
					{
						context.Lock(p);
						_updateStrategy.ScheduleWorkQueueItem(p, context);

						// Set this to Not Available so the worklist item doesn't get scheduled again for this procedure
						p.ImageAvailability = Healthcare.ImageAvailability.N;
					}
					catch (Exception e)
					{
						ExceptionLogger.Log("ImageAvailabilityProcessor.ScheduleWorkQueueItems", e);
					}
				}

				scope.Complete();
			}
		}

		private IList<WorkQueueItem> NextBatchOfWorkQueueItems()
		{
			IList<WorkQueueItem> items;

			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				try
				{
					DateTime now = DateTime.Now;

					// Find a list of procedures that match the criteria
					WorkQueueItemSearchCriteria criteria = new WorkQueueItemSearchCriteria();
					criteria.Type.EqualTo(_updateStrategy.ScheduledWorkQueueItemType);
					criteria.Status.EqualTo(WorkQueueStatus.PN);
					criteria.ExpirationTime.MoreThan(now);
					criteria.ScheduledTime.LessThan(now);

					SearchResultPage page = new SearchResultPage(0, _batchSize);
					items = scope.Context.GetBroker<IWorkQueueItemBroker>().Find(criteria, page);

					scope.Complete();
				}
				catch (Exception e)
				{
					ExceptionLogger.Log("ImageAvailabilityProcessor.NextBatchOfWorkQueueItems", e);
					scope.Dispose();
					items = new List<WorkQueueItem>();
				}
			}

			return items;
		}

		private void ProcessWorkQueueItems(IEnumerable<WorkQueueItem> items)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)scope.Context;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				foreach (WorkQueueItem item in items)
				{
					try
					{
						context.Lock(item);
						_updateStrategy.Update(item, context);
					}
					catch (Exception e)
					{
						ExceptionLogger.Log("ImageAvailabilityProcessor.ProcessWorkQueueItems", e);

						ImageAvailabilitySettings settings = new ImageAvailabilitySettings();
						item.ScheduledTime = DateTime.Now.AddMinutes(settings.NextScheduledTimeForErrorInMinutes);
						item.FailureCount++;
						item.FailureDescription = e.Message;
					}
					finally
					{
						item.ProcessedTime = DateTime.Now;
					}
				}

				scope.Complete();
			}
		}
	}
}
