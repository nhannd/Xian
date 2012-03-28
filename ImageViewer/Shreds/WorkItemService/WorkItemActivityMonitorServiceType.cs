using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

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
                //TODO (Marmot): Need to update so we can limit by WorkItemType?
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(_callback, "WorkItemChanged");
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
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(_callback, "WorkItemChanged");
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
    }
}
