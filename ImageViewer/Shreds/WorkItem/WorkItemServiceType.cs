#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Shreds.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.WorkItem
{
    [Serializable]
    internal class WorkItemServiceException : Exception
    {
        public WorkItemServiceException(string message)
            : base(message)
        {
        }

        protected WorkItemServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WorkItemServiceType : IWorkItemService
    {
        private readonly IWorkItemActivityCallback _callback;

        public WorkItemServiceType()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IWorkItemActivityCallback>();
        }

        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            try
            {
                return WorkItemService.Instance.Insert(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingInsert;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            try
            {
                return WorkItemService.Instance.Update(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingUpdate;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemQueryResponse Query(WorkItemQueryRequest request)
        {
            try
            {
                return WorkItemService.Instance.Query(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingQuery;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            try
            {
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(_callback, "WorkItem");
	
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
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(_callback, "WorkItem");
	
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
    }
}
