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
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{

    [DataContract(Name = "ActivityType", Namespace = ImageViewerNamespace.Value)]
    public enum ActivityTypeEnum
    {
        [EnumMember]
        DicomReceive = 1,
        [EnumMember]
        ImportStudy = 2,
        [EnumMember]
        ImportFiles = 3,
        [EnumMember]
        DicomRetrieve = 4,
        [EnumMember]
        ReIndex = 5,
        [EnumMember]
        ReapplyRules = 6,
        [EnumMember]
        DicomSendStudy = 7,
        [EnumMember]
        DicomSendSeries = 8,
        [EnumMember]
        DicomSendSop = 9,
        [EnumMember]
        AutoRoute = 10,
        [EnumMember]
        PublishFiles = 11,
        [EnumMember]
        DeleteStudy = 12,
        [EnumMember]
        DeleteSeries = 13,
    }

    public static class WorkItemRequestTypeProvider
    {
        private static readonly List<Type> List = new List<Type>
                                                      {
                                                          // WorkItemRequest related
                                                          typeof (WorkItemRequest),
                                                          //Non-Study related requests
                                                          typeof (ReindexRequest),
                                                          typeof (ImportFilesRequest),
                                                          //typeof (ReapplyRulesRequest), todo: include dynamically

                                                          //Study related requests
                                                          typeof (WorkItemStudyRequest),
                                                          typeof (DicomSendRequest),
                                                          typeof (DicomSendSopRequest),
                                                          typeof (DicomSendSeriesRequest),
                                                          typeof (DicomSendStudyRequest),
                                                          typeof (PublishFilesRequest),
                                                          typeof (DicomAutoRouteRequest),
                                                          typeof (DicomRetrieveRequest),
                                                          typeof (DicomRetrieveStudyRequest),
                                                          typeof (DicomRetrieveSeriesRequest),
                                                          typeof (ProcessStudyRequest),
                                                          typeof (DicomReceiveRequest),
                                                          typeof (ImportStudyRequest),
                                                          typeof (DeleteStudyRequest),
                                                          typeof (DeleteSeriesRequest),

                                                          // WorkItemProgress related
                                                          typeof (WorkItemProgress),
                                                          typeof (ProcessStudyProgress),
                                                          typeof (ImportFilesProgress),
                                                          typeof (ReindexProgress),
                                                          typeof (DicomSendProgress),
                                                          typeof (DicomRetrieveProgress),
                                                          typeof (DeleteProgress),
                                                          typeof (ReapplyRulesProgress)
                                                      };

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
        {
            return List;
        }
    }


    /// <summary>
    /// Base Request object for the creation of <see cref="WorkItem"/>s.
    /// </summary>
    [XmlInclude(typeof(WorkItemStudyRequest))]
    [XmlInclude(typeof(ImportFilesRequest))]
    [XmlInclude(typeof(ReindexRequest))]
    [XmlInclude(typeof(DicomSendRequest))]
    [XmlInclude(typeof(DicomSendStudyRequest))]
    [XmlInclude(typeof(DicomSendSeriesRequest))]
    [XmlInclude(typeof(PublishFilesRequest))]
    [XmlInclude(typeof(DicomAutoRouteRequest))]
    [XmlInclude(typeof(DicomRetrieveRequest))]
    [XmlInclude(typeof(DicomRetrieveStudyRequest))]
    [XmlInclude(typeof(DicomRetrieveSeriesRequest))]
    [XmlInclude(typeof(ProcessStudyRequest))]
    [XmlInclude(typeof(DicomReceiveRequest))]
    [XmlInclude(typeof(ImportStudyRequest))]
    [XmlInclude(typeof(DeleteStudyRequest))]
    [XmlInclude(typeof(DeleteSeriesRequest))]
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("b2d86945-96b7-4563-8281-02142e84ffc3")]
    public abstract class WorkItemRequest : DataContractBase
    {
        [DataMember]
        public WorkItemPriorityEnum Priority { get; set; }

        [DataMember]
        public WorkItemTypeEnum Type { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public ActivityTypeEnum ActivityType { get; set; }

        public abstract string ActivityDescription { get; }

    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("00e165d6-44db-4bf4-b607-a8e82a395964")]
    public class WorkItemPatient : PatientRootPatientIdentifier
    {
        public WorkItemPatient()
        { }

        public WorkItemPatient(IPatientData p) 
            : base(p)
        { }

        public WorkItemPatient(DicomAttributeCollection c)
            : base(c)
        { }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("3366be52-823c-484e-b0a7-7344fed16457")]
    public class WorkItemStudy : StudyIdentifier
    {
        public WorkItemStudy()
        { }

        public WorkItemStudy(IStudyData s) 
            : base(s)
        {}

        public WorkItemStudy(DicomAttributeCollection c)
            : base(c)
        {            
            string modality = c[DicomTags.Modality].ToString();
            ModalitiesInStudy = new[] { modality };
        }
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("E0BF69EF-1854-441c-9C1B-5D334094CB85")]
    public abstract class WorkItemStudyRequest : WorkItemRequest
    {
        [DataMember(IsRequired = true)]
        public WorkItemStudy Study { get; set; }

        [DataMember(IsRequired = true)]
        public WorkItemPatient Patient { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("c6a4a14e-e877-45a3-871d-bb06054dd837")]
    public abstract class DicomSendRequest : WorkItemStudyRequest
    {
        [DataMember]
        public string Destination { get; set; }

        [DataMember]
        public CompressionType CompressionType { get; set; }

        [DataMember]
        public int CompressionLevel { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("F0C1BA64-06BD-4E97-BE55-183915656811")]
    public class DicomSendStudyRequest : DicomSendRequest
    {
        public DicomSendStudyRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomSendStudy;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendStudyRequest_ActivityDescription, Destination); }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("EF7A33C7-6B8A-470D-98F4-796780D8E50E")]
    public class DicomSendSeriesRequest : DicomSendRequest
    {        
        public DicomSendSeriesRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomSendSeries;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSeriesRequest_ActivityDescription, Destination); }
        }
    }

    
        /// <summary>
    /// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("75BD907A-45D3-471B-AD0A-DE13D422A794")]
    public class DicomSendSopRequest : DicomSendRequest
    {
        public DicomSendSopRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomSendSop;
        }

        [DataMember(IsRequired = true)]
        public string SeriesInstanceUid { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> SopInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSopRequest_ActivityDescription, Destination); }
        }
    }


    /// <summary>
    /// <see cref="WorkItemRequest"/> for publishing files to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("46DCBBF6-A8B3-4F5E-8611-5712A2BBBEFC")]
    public class PublishFilesRequest : DicomSendRequest
    {
        public PublishFilesRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.PublishFiles;
            DeletionBehaviour = DeletionBehaviour.None;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        [DataMember(IsRequired = false)]
        public List<string> FilePaths { get; set; }

        [DataMember(IsRequired = true)]
        public DeletionBehaviour DeletionBehaviour { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSeriesRequest_ActivityDescription, Destination); }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("1c63c863-aa4e-4672-bee5-8aa3db16edd5")]
    public class DicomAutoRouteRequest : DicomSendRequest
    {
        public DicomAutoRouteRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.AutoRoute;
        }

        [DataMember(IsRequired = false)]
        public int? TimeWindowStart { get; set; }

        [DataMember(IsRequired = false)]
        public int? TimeWindowEnd { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomAutoRouteRequest_ActivityDescription, Destination, Patient.PatientsName); }
        }

        public DateTime GetScheduledTime(DateTime currentTime, int postponeSeconds)
        {
            if (!TimeWindowStart.HasValue || !TimeWindowEnd.HasValue || Priority == WorkItemPriorityEnum.Stat)
                return currentTime;

            if (TimeWindowStart.Value > TimeWindowEnd.Value)
            {
                if (currentTime.Hour >= TimeWindowStart.Value
                    || currentTime.Hour < TimeWindowEnd.Value)
                {
                    return currentTime.AddSeconds(postponeSeconds);
                }

                return currentTime.Date.AddHours(TimeWindowStart.Value);
            }

            if (currentTime.Hour >= TimeWindowStart.Value
                && currentTime.Hour < TimeWindowEnd.Value)
            {
                return currentTime.AddSeconds(postponeSeconds);
            }

            return currentTime.Hour < TimeWindowStart.Value
                       ? currentTime.Date.AddHours(TimeWindowStart.Value)
                       : currentTime.Date.Date.AddDays(1d).AddHours(TimeWindowStart.Value);
        }
    }

    [DataContract(Name = "DeletionBehaviour", Namespace = ImageViewerNamespace.Value)]
    public enum DeletionBehaviour
    {
        [EnumMember]
        DeleteOnSuccess = 0,
        [EnumMember]
        DeleteAlways,
        [EnumMember]
        None
    }


    [DataContract(Name = "BadFileBehaviour", Namespace = ImageViewerNamespace.Value)]
    public enum BadFileBehaviourEnum
    {
        [EnumMember]
        Ignore = 0,
        [EnumMember]
        Move,
        [EnumMember]
        Delete
    }

    [DataContract(Name = "FileImportBehaviour", Namespace = ImageViewerNamespace.Value)]
    public enum FileImportBehaviourEnum
    {
        [EnumMember]
        Move = 0,
        [EnumMember]
        Copy,
        [EnumMember]
        Save
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for importing files/studies.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("02b7d427-1107-4458-ade3-67ee6779a766")]
	public class ImportFilesRequest : WorkItemRequest
    {
        public ImportFilesRequest()
        {
            Type = WorkItemTypeEnum.Import;
            Priority = WorkItemPriorityEnum.High;
            ActivityType = ActivityTypeEnum.ImportFiles;
        }

        [DataMember(IsRequired = true)]
        public bool Recursive { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> FileExtensions { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> FilePaths { get; set; }

        [DataMember(IsRequired = true)]
        public BadFileBehaviourEnum BadFileBehaviour { get; set; }

        [DataMember(IsRequired = true)]
        public FileImportBehaviourEnum FileImportBehaviour { get; set; }

        public override string ActivityDescription
        {
            get
            {
                return string.Format(FilePaths.Count > 1
                                         ? SR.ImportFilesRequest_ActivityDescriptionPlural
                                         : SR.ImportFilesRequest_ActivityDescription, FilePaths.Count);
            }
        }
    }

    /// <summary>
    /// DICOM Retrieve Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("0e04fa53-3f45-4ae2-9444-f3208047757c")]
    public abstract class DicomRetrieveRequest : WorkItemStudyRequest
    {
        [DataMember]
        public string Source { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("61AB2801-5284-480B-B054-F0314865D84F")]
    public class DicomRetrieveStudyRequest : DicomRetrieveRequest
    {
        public DicomRetrieveStudyRequest()
        {
            Type = WorkItemTypeEnum.DicomRetrieve;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomRetrieve;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveRequest_ActivityDescription, Source); }
        }
    }

    
    /// <summary>
    /// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("09547DF4-E8B8-45E8-ABAF-33159E2C7098")]
    public class DicomRetrieveSeriesRequest : DicomRetrieveRequest
    {
        public DicomRetrieveSeriesRequest()
        {
            Type = WorkItemTypeEnum.DicomRetrieve;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomRetrieve;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveSeriesRequest_ActivityDescription, Source); }
        }
    }


    /// <summary>
    /// Abstract Study Process Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("4d22984a-e750-467c-ab89-f680be38c6c1")]
    public abstract class ProcessStudyRequest : WorkItemStudyRequest
    {
        protected ProcessStudyRequest()
        {
            Type = WorkItemTypeEnum.ProcessStudy;
            Priority = WorkItemPriorityEnum.High;
        }
    }


    /// <summary>
    /// DICOM Receive Study Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("146cc54f-7b98-468b-948a-415eeffd3d7f")]
    public class DicomReceiveRequest : ProcessStudyRequest
    {
        public DicomReceiveRequest()
        {
            ActivityType = ActivityTypeEnum.DicomReceive;
        }

        [DataMember(IsRequired = true)]
        public string FromAETitle { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomReceiveRequest_ActivityDescription, FromAETitle); }
        }
    }

    /// <summary>
    /// DICOM Import Study Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("2def790a-8039-4fc5-85d6-f4d3be3f2d8e")]
    public class ImportStudyRequest : ProcessStudyRequest
    {
        public ImportStudyRequest()
        {
            ActivityType = ActivityTypeEnum.ImportStudy;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.ImportStudyRequest_AcitivityDescription); }
        }
    }

    /// <summary>
    /// ReindexRequest Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("875D13F2-621D-4277-8A32-34D9BF5AE40B")]
    public class ReindexRequest : WorkItemRequest
    {
        public ReindexRequest()
        {
            Type = WorkItemTypeEnum.ReIndex;
            Priority = WorkItemPriorityEnum.High;
            ActivityType = ActivityTypeEnum.ReIndex;
        }

        public override string ActivityDescription
        {
            get { return SR.ReindexRequest_ActivityDescription; }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for deleting a study.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("0A7BE406-E5BE-4E10-997F-BCA37D53FED7")]
    public class DeleteStudyRequest : WorkItemStudyRequest
    {
        public DeleteStudyRequest()
        {
            Type = WorkItemTypeEnum.DeleteStudy;
            Priority = WorkItemPriorityEnum.High;
            ActivityType = ActivityTypeEnum.DeleteStudy;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DeleteStudyRequest_ActivityDescription); }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for deleting series.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("64BCF5FF-1AFD-409B-B0EB-1E576124D61E")]
    public class DeleteSeriesRequest : WorkItemStudyRequest
    {
        public DeleteSeriesRequest()
        {
            Type = WorkItemTypeEnum.DeleteSeries;
            Priority = WorkItemPriorityEnum.High;
            ActivityType = ActivityTypeEnum.DeleteSeries;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DeleteSeriesRequest_ActivityDescription); }
        }
    }
}
