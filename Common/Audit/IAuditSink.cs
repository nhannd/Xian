#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
