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
