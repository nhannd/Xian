using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	[KnownType(typeof(DicomServerApplicationEntity))]
	[KnownType(typeof(StreamingServerApplicationEntity))]
	[DataContract]
	public class ApplicationEntity : IApplicationEntity
	{
		public ApplicationEntity()
		{}

		public ApplicationEntity(string aeTitle)
		{
			AETitle = aeTitle;
		}

		public ApplicationEntity(string aeTitle, string description)
			: this(aeTitle)
		{
			Description = description;
		}

		public ApplicationEntity(string aeTitle, string description, string location)
			: this(aeTitle, description)
		{
			Location = location;
		}

		#region IApplicationEntity Members

		[DataMember(IsRequired = true)]
		public string AETitle { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string Location { get; set; }

		#endregion
	}
}
