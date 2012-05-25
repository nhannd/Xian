#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class ImageViewerWorkItemNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/workitem";
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemInsertRequest : DataContractBase
    {
        [DataMember]
        public WorkItemRequest Request { get; set; }

        [DataMember]
        public WorkItemProgress Progress { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemInsertResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUpdateRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public long Identifier { get; set; }

        [DataMember]
        public WorkItemPriorityEnum? Priority { get; set; }

        [DataMember]
        public WorkItemStatusEnum? Status { get; set; }

        [DataMember]
        public DateTime? ScheduledTime { get; set; }

        [DataMember]
        public DateTime? ExpirationTime { get; set; }

        [DataMember]
        public bool? Cancel { get; set; }

        [DataMember]
        public bool? Delete { get; set; }

    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUpdateResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemQueryRequest : DataContractBase
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public WorkItemStatusEnum? Status { get; set; }

        [DataMember]
        public string StudyInstanceUid { get; set; }

        [DataMember]
        public long? Identifier { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemQueryResponse : DataContractBase
    {
        [DataMember]
        public IEnumerable<WorkItemData> Items { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemSubscribeRequest : DataContractBase
    {
        //[DataMember]
        //public CultureInfo Culture { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemSubscribeResponse : DataContractBase
    {        
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUnsubscribeRequest : DataContractBase
    {
        [DataMember]
        public string Type { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUnsubscribeResponse : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemRefreshRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemRefreshResponse : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemPublishRequest : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemPublishResponse : DataContractBase
    {
    }

    [ServiceContract(SessionMode = SessionMode.Required,
                        CallbackContract = typeof(IWorkItemActivityCallback),
                        ConfigurationName = "IWorkItemActivityMonitorService",
                        Namespace = ImageViewerWorkItemNamespace.Value)]
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemActivityMonitorService
    {
        [OperationContract]
        WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request);

        [OperationContract]
        WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request);

        [OperationContract(IsOneWay = true)]
        void Refresh(WorkItemRefreshRequest request);

        [OperationContract]
        WorkItemPublishResponse Publish(WorkItemPublishRequest request);
    }

    /// <summary>
    /// Service for the creation, manipulation, and monitoring of WorkItems.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, 
        ConfigurationName = "IWorkItemService",
        Namespace = ImageViewerWorkItemNamespace.Value)]
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemService
    {
        [OperationContract]
        WorkItemInsertResponse Insert(WorkItemInsertRequest request);

        [OperationContract]
        WorkItemUpdateResponse Update(WorkItemUpdateRequest request);

        [OperationContract]
        WorkItemQueryResponse Query(WorkItemQueryRequest request);
    }
}
