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
        DicomSend = 3,
        [EnumMember]
        AutoRoute = 4,
        [EnumMember]
        ImportFiles = 5,
        [EnumMember]
        DicomRetrieve = 6,
        [EnumMember]
        ReIndex = 7,
        [EnumMember]
        ReapplyRules = 8,
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

                                                          //Study related requests
                                                          typeof (WorkItemStudyRequest),
                                                          typeof (DicomSendRequest),
                                                          typeof (DicomAutoRouteRequest),
                                                          typeof (DicomRetrieveRequest),
                                                          typeof (StudyProcessRequest),
                                                          typeof (DicomReceiveRequest),
                                                          typeof (ImportStudyRequest),

                                                          // WorkItemProgress related
                                                          typeof (WorkItemProgress),
                                                          typeof (StudyProcessProgress),
                                                          typeof (ImportFilesProgress),
                                                          typeof (ReindexProgress),
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
    [XmlInclude(typeof(DicomAutoRouteRequest))]
    [XmlInclude(typeof(DicomRetrieveRequest))]
    [XmlInclude(typeof(StudyProcessRequest))]
    [XmlInclude(typeof(DicomReceiveRequest))]
    [XmlInclude(typeof(ImportStudyRequest))]
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("b2d86945-96b7-4563-8281-02142e84ffc3")]
    public abstract class WorkItemRequest : DataContractBase
    {
        [DataMember]
        public WorkItemPriorityEnum Priority { get; set; }

        [DataMember]
        public WorkItemTypeEnum Type { get; set; }

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

        public WorkItemStudy(DicomAttributeCollection c)
            : base(c)
        { }
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
    public class DicomSendRequest : WorkItemStudyRequest
    {
        public DicomSendRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomSend;
        }

        [DataMember]
        public string AeTitle { get; set; }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string StudyInstanceUid { get; set; }

        [DataMember]
        public string TransferSyntaxUid { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendRequest_ActivityDescription, AeTitle); }
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

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomAutoRouteRequest_ActivityDescription, AeTitle, Patient.PatientsName); }
        }
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
            Priority = WorkItemPriorityEnum.Stat;
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
            get { return string.Format(SR.ImportFilesRequest_ActivityDescription, FilePaths.Count); }
        }
    }


    /// <summary>
    /// DICOM Retrieve Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("0e04fa53-3f45-4ae2-9444-f3208047757c")]
    public class DicomRetrieveRequest : WorkItemStudyRequest
    {
        DicomRetrieveRequest()
        {
            Type = WorkItemTypeEnum.DicomRetrieve;
            Priority = WorkItemPriorityEnum.Normal;
            ActivityType = ActivityTypeEnum.DicomRetrieve;
        }

        [DataMember(IsRequired = true)]
        public string FromAETitle { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveRequest_ActivityDescription, FromAETitle); }
        }
    }

    /// <summary>
    /// Abstract Study Process Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("4d22984a-e750-467c-ab89-f680be38c6c1")]
    public abstract class StudyProcessRequest : WorkItemStudyRequest
    {
        protected StudyProcessRequest()
        {
            Type = WorkItemTypeEnum.StudyProcess;
            Priority = WorkItemPriorityEnum.Stat;
        }
    }


    /// <summary>
    /// DICOM Receive Study Request
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("146cc54f-7b98-468b-948a-415eeffd3d7f")]
    public class DicomReceiveRequest : StudyProcessRequest
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
    public class ImportStudyRequest : StudyProcessRequest
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
            Priority = WorkItemPriorityEnum.Stat;
            ActivityType = ActivityTypeEnum.ReIndex;
        }

        public override string ActivityDescription
        {
            get { return SR.ReindexRequest_ActivityDescription; }
        }
    }
}
