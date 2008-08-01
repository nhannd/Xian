using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class FaxReportRequest : DataContractBase
	{
		public FaxReportRequest(EntityRef reportRef)
		{
			this.ReportRef = reportRef;
			this.Recipients = new List<FaxRecipientDetail>();
		}

		[DataMember]
		public EntityRef ReportRef;

		[DataMember]
		public List<FaxRecipientDetail> Recipients;
	}

	[DataContract]
	public class FaxRecipientDetail : DataContractBase
	{
		public FaxRecipientDetail(EntityRef practitionerRef, EntityRef contactPointRef)
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