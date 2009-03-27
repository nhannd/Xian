using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Audit
{
	/// <summary>
	/// Defines an interface to an object that acts as a sink for an <see cref="AuditLog"/>.
	/// </summary>
	public interface IAuditSink
	{
		/// <summary>
		/// Writes the specified entry to the sink.
		/// </summary>
		/// <param name="entry"></param>
		void WriteEntry(AuditEntryInfo entry);
	}
}
