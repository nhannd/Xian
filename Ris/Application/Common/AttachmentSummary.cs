using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class AttachmentSummary : DataContractBase
	{
		public AttachmentSummary(EnumValueInfo category, StaffSummary attachedBy, AttachedDocumentSummary document)
		{
			this.Category = category;
			this.AttachedBy = attachedBy;
			this.Document = document;
		}

		[DataMember]
		public EnumValueInfo Category;

		[DataMember]
		public StaffSummary AttachedBy;

		[DataMember]
		public AttachedDocumentSummary Document;
	}
}