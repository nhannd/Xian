#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.ImageServer.Enterprise.Audit
{
    /// <summary>
    /// An implementation of <see cref="IAuditSink"/> that sinks to the Application Log
    /// </summary>
    [ExtensionOf(typeof(AuditSinkExtensionPoint), Enabled = true)]
    public class ApplicationLogAuditSink : IAuditSink
    {
        #region IAuditSink Members

        /// <summary>
        /// Writes the specified entry to the sink.
        /// </summary>
        /// <param name="entry"></param>
        public void WriteEntry(AuditEntryInfo entry)
        {
            
        }

        #endregion
    }
}