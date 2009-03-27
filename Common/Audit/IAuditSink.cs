using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Audit
{
	public interface IAuditSink
	{
		void WriteEntry(AuditLogEntryDetail entry);
	}
}
