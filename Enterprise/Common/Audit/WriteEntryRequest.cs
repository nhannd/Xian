using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Audit
{
	[DataContract]
	public class WriteEntryRequest : DataContractBase
	{
		public WriteEntryRequest(AuditLogEntryDetail logEntry)
		{
			LogEntry = logEntry;
		}

		[DataMember]
		public AuditLogEntryDetail LogEntry;
	}
}
