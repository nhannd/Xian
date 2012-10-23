#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IStudyLocator", Namespace = StudyManagementNamespace.Value)]
	public interface IStudyLocator
	{
		LocateStudiesResult LocateStudies(LocateStudiesRequest request);
		LocateSeriesResult LocateSeries(LocateSeriesRequest request);
		LocateImagesResult LocateImages(LocateImagesRequest request);
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateFailureInfo : DataContractBase
	{
		[DataMember(IsRequired = false)]
		public QueryFailedFault Fault { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerAE { get; set; }

		[DataMember(IsRequired = false)]
		public string ServerName { get; set; }

		public LocateFailureInfo() {}

		public LocateFailureInfo(QueryFailedFault fault, string faultDescription)
		{
			Fault = fault;
			Description = faultDescription;
		}
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateStudiesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public StudyRootStudyIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateStudiesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<StudyRootStudyIdentifier> Studies { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateSeriesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public SeriesIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateSeriesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<SeriesIdentifier> Series { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateImagesRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public ImageIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = StudyManagementNamespace.Value)]
	public class LocateImagesResult : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public IList<ImageIdentifier> Images { get; set; }

		[DataMember(IsRequired = false)]
		public IList<LocateFailureInfo> Failures { get; set; }
	}
}