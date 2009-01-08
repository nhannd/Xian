using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class RejectTranscriptionRequest : SaveTranscriptionRequest
	{
		public RejectTranscriptionRequest(EntityRef reportingStepRef, EnumValueInfo rejectReason, OrderNoteDetail additionalComments)
			: this(reportingStepRef, null, rejectReason, additionalComments)
		{
		}

		public RejectTranscriptionRequest(EntityRef reportingStepRef, Dictionary<string, string> reportPartExtendedProperties, EnumValueInfo rejectReason, OrderNoteDetail additionalComments)
			: base(reportingStepRef, reportPartExtendedProperties)
		{
			this.RejectReason = rejectReason;
			this.AdditionalComments = additionalComments;
		}

		[DataMember]
		public EnumValueInfo RejectReason;

		[DataMember]
		public OrderNoteDetail AdditionalComments;
	}
}