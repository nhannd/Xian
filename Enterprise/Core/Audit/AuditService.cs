using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Audit;

namespace ClearCanvas.Enterprise.Core.Audit
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IAuditService))]
	public class AuditService : CoreServiceLayer, IAuditService
	{
		#region IAuditService Members

		[UpdateOperation]
		public WriteEntryResponse WriteEntry(WriteEntryRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.LogEntry, "LogEntry");

			AuditLogEntryAssembler assembler = new AuditLogEntryAssembler();
			AuditLogEntry logEntry = assembler.CreateAuditLogEntry(request.LogEntry);

			// save the log entry
			PersistenceContext.Lock(logEntry, DirtyState.New);

			return new WriteEntryResponse();
		}

		#endregion
	}
}
