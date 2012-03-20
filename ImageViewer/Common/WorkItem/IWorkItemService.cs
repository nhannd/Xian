using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [DataContract]
    public class WorkItemInsertRequest : DataContractBase
    {
        [DataMember]
        public WorkItemRequest Request { get; set; }
    }

    [DataContract]
    public class WorkItemInsertResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract]
    public class WorkItemUpdateRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public int Identifier { get; set; }

        [DataMember]
        public WorkItemPriorityEnum? Priority { get; set; }

        [DataMember]
        public DateTime? ScheduledTime { get; set; }

        [DataMember]
        public DateTime? ExpirationTime { get; set; }

        [DataMember]
        public bool? Cancel { get; set; }
    }

    [DataContract]
    public class WorkItemUpdateResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract]
    public class WorkItemQueryRequest : DataContractBase
    {
        [DataMember]
        public WorkItemTypeEnum? Type { get; set; }

        [DataMember]
        public WorkItemStatusEnum? Status { get; set; }

        [DataMember]
        public string StudyInstanceUid { get; set; }

        [DataMember]
        public long? Identifier { get; set; }
    }

    [DataContract]
    public class WorkItemQueryResponse : DataContractBase
    {
        [DataMember]
        public IEnumerable<WorkItemData> Items { get; set; }
    }

    [DataContract]
    public class WorkItemSubscribeRequest : DataContractBase
    {
        [DataMember]
        public CultureInfo Culture { get; set; }

        [DataMember]
        public WorkItemTypeEnum? Type { get; set; }

        [DataMember]
        public Guid? Identifier { get; set; }
    }

    [DataContract]
    public class WorkItemSubscribeResponse : DataContractBase
    {
        
    }

    [DataContract]
    public class WorkItemUnsubscribeRequest : DataContractBase
    {
        [DataMember]
        public WorkItemTypeEnum? Type { get; set; }
    }

    [DataContract]
    public class WorkItemUnsubscribeResponse : DataContractBase
    {

    }

    /// <summary>
    /// Service for the creation, manipulation, and monitoring of WorkItems.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required,
                        CallbackContract = typeof(IWorkItemActivityCallback),
                        ConfigurationName = "IWorkItemService")]
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemService
    {
        [OperationContract]
        WorkItemInsertResponse Insert(WorkItemInsertRequest request);

        [OperationContract]
        WorkItemUpdateResponse Update(WorkItemUpdateRequest request);

        [OperationContract]
        WorkItemQueryResponse Query(WorkItemQueryRequest request);

        [OperationContract]
        WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request);

        [OperationContract]
        WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest type);
    }
}
