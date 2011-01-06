#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

		[UpdateOperation(ChangeSetAuditable = false)]
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
