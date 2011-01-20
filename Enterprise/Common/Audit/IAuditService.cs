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
using System.ServiceModel;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Audit
{
	/// <summary>
	/// Defines a service for working with the audit log.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(false)]
	public interface IAuditService
	{
		/// <summary>
		/// Writes an entry to the audit log.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		WriteEntryResponse WriteEntry(WriteEntryRequest request);
	}
}
