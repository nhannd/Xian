using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendReportToQueueRequest : DataContractBase
	{
		public SendReportToQueueRequest(EntityRef reportRef, EntityRef orderRef)
		{
			this.ReportRef = reportRef;
			this.OrderRef = orderRef;
			this.Recipients = new List<PublishRecipientDetail>();
		}

		[DataMember]
		public EntityRef ReportRef;

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public List<PublishRecipientDetail> Recipients;
	}

	[DataContract]
	public class PublishRecipientDetail : DataContractBase
	{
		public PublishRecipientDetail(EntityRef practitionerRef, EntityRef contactPointRef)
		{
			this.PractitionerRef = practitionerRef;
			this.ContactPointRef = contactPointRef;
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public EntityRef ContactPointRef; 
	}
}