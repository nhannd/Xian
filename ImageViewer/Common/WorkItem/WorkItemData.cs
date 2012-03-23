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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [DataContract(Name = "WorkItemPriority", Namespace = ImageViewerNamespace.Value)]
    public enum WorkItemPriorityEnum
    {
        [EnumMember]
        Stat = 1,
        [EnumMember]
        Normal = 2
    }

    [DataContract(Name = "WorkItemStatus", Namespace = ImageViewerNamespace.Value)]
    public enum WorkItemStatusEnum
    {
        [EnumMember]
        Pending,
        [EnumMember]
        InProgress,
        [EnumMember]
        Complete,
        [EnumMember]
        Idle,
        [EnumMember]
        Deleted,
        [EnumMember]
        Canceled,
        [EnumMember]
        Failed
    }

    // REmove?  Not sure due to wanting to Query by type or subscribe by type
    [DataContract(Name = "WorkItemType", Namespace = ImageViewerNamespace.Value)]
    public enum WorkItemTypeEnum
    {
        [EnumMember]
        StudyProcess = 1,
        [EnumMember]
        StudyDelete = 2,
        [EnumMember]
        SeriesDelete = 3,
        [EnumMember]
        Import = 4,
        [EnumMember]
        DicomSend = 5,
        [EnumMember]
        DicomRetrieve = 6,
        [EnumMember]
        ReIndex = 7,
        [EnumMember]
        ReapplyRules = 8,
    }

    /// <summary>
    /// Base WorkItem representing a unit of Work to be done.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    public class WorkItemData : DataContractBase
    {
        /// <summary>
        /// The Identifier for the WorkItem.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long Identifier { get; set; }

        /// <summary>
        /// The Priority of the WorkItem
        /// </summary>
        [DataMember(IsRequired = true)]
        public WorkItemPriorityEnum Priority { get; set; }

        /// <summary>
        /// The current status of the WorkItem
        /// </summary>
        [DataMember(IsRequired = true)]
        public WorkItemStatusEnum Status { get; set; }

        [DataMember(IsRequired = true)]
        public WorkItemTypeEnum Type { get; set; }

        [DataMember(IsRequired = false)]
        public string StudyInstanceUid { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime InsertTime { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime ScheduledTime { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime ExpirationTime { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = true)]
        public int FailureCount { get; set; }

        [DataMember(IsRequired = false)]
        public string Description { get; set; }

        [DataMember(IsRequired = false)]
        public WorkItemRequest Request { get; set; }

        [DataMember(IsRequired = false)]
        public WorkItemProgress Progress { get; set; }

        [DataMember(IsRequired = false)]
        public IPatientData Patient { get; set; }

        [DataMember(IsRequired = false)]
        public IStudyData Study { get; set; }
    }
}
