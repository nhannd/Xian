#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
				info.UserSessionId,
				info.Operation,
				info.Details);
		}
	}
}
