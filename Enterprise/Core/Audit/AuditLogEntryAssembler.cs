using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common.Audit;

namespace ClearCanvas.Enterprise.Core.Audit
{
	class AuditLogEntryAssembler
	{
		public AuditLogEntry CreateAuditLogEntry(AuditLogEntryDetail detail)
		{
			return new AuditLogEntry(
				detail.Category,
				detail.TimeStamp,
				detail.HostName,
				detail.Application,
				detail.User,
				detail.Operation,
				detail.Details);
		}
	}
}
