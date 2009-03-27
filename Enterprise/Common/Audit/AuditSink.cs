using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Audit
{
	/// <summary>
	/// An implementation of <see cref="IAuditSink"/> that sinks to the <see cref="IAuditService"/>.
	/// </summary>
	[ExtensionOf(typeof(AuditSinkExtensionPoint))]
	public class AuditSink : IAuditSink
	{
		#region IAuditSink Members

		/// <summary>
		/// Writes the specified entry to the sink.
		/// </summary>
		/// <param name="entry"></param>
		public void WriteEntry(AuditEntryInfo entry)
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
