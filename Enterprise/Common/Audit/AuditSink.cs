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
