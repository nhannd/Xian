#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	//TODO (CR Sept 2010): restore data contract references to this later (e.g. next time we are releasing migration services).
	//Make sure and analyze everthing that might be affected first.
	public static class DicomNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/dicom";
	}

	[KnownType(typeof(DicomServerApplicationEntity))]
	[KnownType(typeof(StreamingServerApplicationEntity))]
	//[DataContract(Namespace = DicomNamespace.Value)]
	public class ApplicationEntity : IApplicationEntity
	{
		public ApplicationEntity()
		{}

		public ApplicationEntity(string aeTitle)
		{
			AETitle = aeTitle;
		}

		public ApplicationEntity(string aeTitle, string name, string description, string location)
			: this(aeTitle)
		{
			Description = description;
			Name = name;
			Location = location;
		}

		#region IApplicationEntity Members

		[DataMember(IsRequired = true)]
		public string AETitle { get; set; }

		[DataMember(IsRequired = false)]
		public string Name { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string Location { get; set; }

		#endregion
	}
}
