using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class MailFaxReportRequest : DataContractBase
	{
		public MailFaxReportRequest(EntityRef reportRef)
		{
			this.ReportRef = reportRef;
			this.Recipients = new List<MailFaxRecipientDetail>();
		}

		[DataMember]
		public EntityRef ReportRef;

		[DataMember]
		public List<MailFaxRecipientDetail> Recipients;
	}

	[DataContract]
	public class MailFaxRecipientDetail : DataContractBase
	{
		public MailFaxRecipientDetail(EntityRef practitionerRef, EntityRef contactPointRef)
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