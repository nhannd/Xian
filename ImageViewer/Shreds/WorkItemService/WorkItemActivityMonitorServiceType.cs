using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class WorkItemActivityMonitorServiceType : IWorkItemActivityMonitorService, IDisposable
    {
        private readonly IWorkItemActivityCallback _callback;

        public WorkItemActivityMonitorServiceType()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IWorkItemActivityCallback>();
        }

        #region Implementation of IWorkItemActivityMonitorService

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            try
            {
				WorkItemPublishSubscribeHelper.SubscribeWorkItemsChanged(_callback);
                return new WorkItemSubscribeResponse();
            }
            catch (Exception e)
            {
                var message = SR.ExceptionErrorProcessingSubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            try
            {
				WorkItemPublishSubscribeHelper.UnsubscribeWorkItemsChanged(_callback);
				return new WorkItemUnsubscribeResponse();
            }
            catch (Exception e)
            {
                var message = SR.ExceptionErrorProcessingUnsubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public void Refresh(WorkItemRefreshRequest request)
        {
        	ThreadPool.QueueUserWorkItem(
				delegate
        		{
					try
					{
						using (var context = new DataAccessContext())
						{
							var broker = context.GetWorkItemBroker();

							var dbList = broker.GetWorkItems(null, null, null);

							// send in batches of 200
							foreach (var batch in BatchItems(dbList, 200))
							{
								WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(batch.Select(WorkItemHelper.FromWorkItem).ToList());
							}
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
						var message = SR.ExceptionErrorProcessingRefresh;
						var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
						throw new WorkItemServiceException(exceptionMessage);
					}
				});
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
				WorkItemPublishSubscribeHelper.UnsubscribeWorkItemsChanged(_callback);
			}
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

		private static IEnumerable<List<T>> BatchItems<T>(IEnumerable<T> items, int batchSize)
		{
			var batch = new List<T>();
			foreach (var item in items)
			{
				batch.Add(item);
				if(batch.Count == batchSize)
				{
					yield return batch;
					batch = new List<T>();
				}
			}

			if (batch.Count > 0)
				yield return batch;
		}
    }
}
