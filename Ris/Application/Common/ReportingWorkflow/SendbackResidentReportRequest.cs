using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendbackResidentReportRequest : SaveReportRequest
	{
		public SendbackResidentReportRequest(EntityRef stepRef)
			: this(stepRef, null, null)
		{
		}

		public SendbackResidentReportRequest(EntityRef stepRef
			, Dictionary<string, string> reportPartExtendedProperties
			, EntityRef supervisorRef)
			: base(stepRef, reportPartExtendedProperties, supervisorRef)
		{
		}
	}
}
