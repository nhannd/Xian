using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Audit
{
	[ExtensionOf(typeof(AuditSinkExtensionPoint))]
	public class AuditSink : IAuditSink
	{
		#region IAuditSink Members

		public void WriteEntry(AuditLogEntryDetail entry)
		{
			Platform.GetService<IAuditService>(
				delegate(IAuditService service)
				{
					service.WriteEntry(new WriteEntryRequest(entry));
				});
		}

		#endregion
	}
}
