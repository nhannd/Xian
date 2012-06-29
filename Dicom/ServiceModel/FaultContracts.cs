#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel
{
    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownDestinationAEFault
	{
		public UnknownDestinationAEFault()
		{}
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownCalledAEFault
	{
		public UnknownCalledAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownCallingAEFault
	{
		public UnknownCallingAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class UnknownSourceAEFault
	{
		public UnknownSourceAEFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyOfflineFault
	{
		public StudyOfflineFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyInUseFault
	{
		public StudyInUseFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyNearlineFault
	{
		public StudyNearlineFault()
		{ }

		[DataMember(IsRequired = false)]
		public bool IsStudyBeingRestored { get; set; }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
	public class StudyNotFoundFault
	{
		public StudyNotFoundFault()
		{ }
	}

    [DataContract(Namespace = DicomNamespace.Value)]
    public class SeriesNotFoundFault
    {
        public SeriesNotFoundFault()
        { }
    }
}
