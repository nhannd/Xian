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
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class WorkItemRequestTypeProvider
    {
        private static readonly List<Type> List = new List<Type>
                                                      {
                                                          typeof (WorkItemRequest),
                                                          typeof (DicomSendRequest),
                                                          typeof (DicomImportRequest),
                                                          typeof (DicomRetrieveRequest),
                                                      };

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
        {
            foreach (var type in List)
                yield return type;
        }
    }

    /// <summary>
    /// Base Request object for the creation of <see cref="WorkItem"/>s.
    /// </summary>
    [XmlInclude(typeof(DicomSendRequest))]
    [XmlInclude(typeof(DicomImportRequest))]
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("b2d86945-96b7-4563-8281-02142e84ffc3")]
    public abstract class WorkItemRequest : DataContractBase
    {
        [DataMember]
        public WorkItemPriorityEnum Priority { get; set; }

        [DataMember]
        public WorkItemTypeEnum Type { get; set; }

        public abstract string ActivityType { get; }

        public abstract string ActivityDescription { get; }

    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("c6a4a14e-e877-45a3-871d-bb06054dd837")]
    public class DicomSendRequest : WorkItemRequest
    {
        public DicomSendRequest()
        {
            Type = WorkItemTypeEnum.DicomSend;
            Priority = WorkItemPriorityEnum.Normal;
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

        [DataMember(IsRequired = true)]
        public StudyIdentifier Study { get; set; }

        [DataMember(IsRequired = true)]
        public PatientRootPatientIdentifier Patient { get; set; }

        public override string ActivityType
        {
            get { return SR.DicomSendRequest_ActivityType; }
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendRequest_ActivityDescription, AeTitle, Patient.PatientsName); }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
    [WorkItemRequestDataContract("{1c63c863-aa4e-4672-bee5-8aa3db16edd5}")]
    public class DicomAutoRouteRequest : DicomSendRequest
    {
        public override string ActivityType
        {
            get { return SR.DicomAutoRouteRequest_ActivityType; }
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomAutoRouteRequest_ActivityDescription, AeTitle, Patient.PatientsName); }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for importing files/studies.
    /// </summary>
    [DataContract(Namespace = ImageViewerNamespace.Value)]
	[WorkItemRequestDataContract("02b7d427-1107-4458-ade3-67ee6779a766")]
	public abstract class DicomImportRequest : WorkItemRequest
    {
        DicomImportRequest()
        {
            Type = WorkItemTypeEnum.Import;
            Priority = WorkItemPriorityEnum.Stat;
        }

        [DataMember(IsRequired = true)]
        public bool Recursive { get; set; }

        [DataMember(IsRequired = true)]
        public IEnumerable<string> FileExtensions { get; set; }

        [DataMember(IsRequired = true)]
        public IEnumerable<string> FilePaths { get; set; }
    }


    /// <summary>
    /// DICOM Retrieve Request
    /// </summary>
	[WorkItemRequestDataContract("0e04fa53-3f45-4ae2-9444-f3208047757c")]
	public class DicomRetrieveRequest : WorkItemRequest
    {
        DicomRetrieveRequest()
        {
            Type = WorkItemTypeEnum.DicomRetrieve;
            Priority = WorkItemPriorityEnum.Normal;
        }

        [DataMember(IsRequired = true)]
        public string FromAETitle { get; set; }

        [DataMember(IsRequired = true)]
        public StudyIdentifier Study { get; set; }

        [DataMember(IsRequired = true)]
        public PatientRootPatientIdentifier Patient { get; set; }

        public override string ActivityType
        {
            get { return SR.DicomRetreiveRequest_ActivityType; }
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveRequest_ActivityDescription, FromAETitle, Patient.PatientsName); }
        }
    }

    /// <summary>
    /// Abstract Study Process Request
    /// </summary>
    [WorkItemRequestDataContract("4d22984a-e750-467c-ab89-f680be38c6c1")]
    public abstract class StudyProcessRequest : WorkItemRequest
    {
        protected StudyProcessRequest()
        {
            Type = WorkItemTypeEnum.StudyProcess;
            Priority = WorkItemPriorityEnum.Stat;
        }

        [DataMember(IsRequired = true)]
        public StudyIdentifier Study { get; set; }

        [DataMember(IsRequired = true)]
        public PatientRootPatientIdentifier Patient { get; set; }
    }


    /// <summary>
    /// DICOM Receive Study Request
    /// </summary>
    [WorkItemRequestDataContract("146cc54f-7b98-468b-948a-415eeffd3d7f")]
    public class DicomReceiveRequest : StudyProcessRequest
    {
        [DataMember(IsRequired = true)]
        public string FromAETitle { get; set; }

        public override string ActivityType
        {
            get { return SR.DicomReceiveRequest_ActivityType; }
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomReceiveRequest_ActivityDescription, FromAETitle, Patient.PatientsName); }
        }
    }

    /// <summary>
    /// DICOM Import Study Request
    /// </summary>
	[WorkItemRequestDataContract("2def790a-8039-4fc5-85d6-f4d3be3f2d8e")]
    public class ImportStudyRequest : StudyProcessRequest
    {
        public override string ActivityType
        {
            get { return SR.ImportStudyRequest_ActivityType; }
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.ImportStudyRequest_AcitivityDescription, Patient.PatientsName); }
        }
    }
}
