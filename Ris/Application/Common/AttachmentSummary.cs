using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class AttachmentSummary : DataContractBase
	{
		public AttachmentSummary(EnumValueInfo category, StaffSummary attachedBy, DateTime attachedTime, AttachedDocumentSummary document)
		{
			this.Category = category;
			this.AttachedBy = attachedBy;
			this.AttachedTime = attachedTime;
			this.Document = document;
		}

		[DataMember]
		public EnumValueInfo Category;

		[DataMember]
		public StaffSummary AttachedBy;

		[DataMember]
		public DateTime AttachedTime;

		[DataMember]
		public AttachedDocumentSummary Document;
	}
}