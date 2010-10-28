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
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines an interface for writing an audit log entry that records
    /// information about a set of <see cref="EntityChange"/> objects.
    /// </summary>
    public interface IEntityChangeSetRecorder
    {
        /// <summary>
        /// Gets or sets a logical operation name for the operation that produced the change set.
        /// </summary>
        string OperationName { get; set; }

        /// <summary>
        /// Writes an audit log entry for the specified change set.
        /// </summary>
        void WriteLogEntry(IEnumerable<EntityChange> changeSet, AuditLog auditLog);
    }
}
