using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

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
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(_callback, "WorkItemsChanged");
                return new WorkItemSubscribeResponse();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingSubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            try
            {
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(_callback, "WorkItemsChanged");
                return new WorkItemUnsubscribeResponse();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingUnsubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public void Refresh(WorkItemRefreshRequest request)
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
						WorkItemActivityPublisher.WorkItemsChanged(batch.Select(WorkItemHelper.FromWorkItem).ToList());
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
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(_callback, null);
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
