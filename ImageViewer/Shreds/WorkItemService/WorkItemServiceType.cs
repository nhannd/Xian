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
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    //TODO (Marmot): Shouldn't throw real exceptions across service boundaries.
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

        public WorkItemPublishResponse Publish(WorkItemPublishRequest request)
        {
            try
            {
                return WorkItemService.Instance.Publish(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingPublish;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }
    }
}
