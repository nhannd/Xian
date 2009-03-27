using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.Enterprise.Core.Audit
{
	class AuditLogEntryAssembler
	{
		public AuditLogEntry CreateAuditLogEntry(AuditEntryInfo info)
		{
			return new AuditLogEntry(
				info.Category,
				info.TimeStamp,
				info.HostName,
				info.Application,
				info.User,
				info.Operation,
				info.Details);
		}
	}
}
