using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReturnToInterpreterRequest : SaveReportRequest
	{
		public ReturnToInterpreterRequest(EntityRef stepRef)
			: this(stepRef, null, null)
		{
		}

		public ReturnToInterpreterRequest(EntityRef stepRef
			, Dictionary<string, string> reportPartExtendedProperties
			, EntityRef supervisorRef)
			: base(stepRef, reportPartExtendedProperties, supervisorRef)
		{
		}
	}
}
