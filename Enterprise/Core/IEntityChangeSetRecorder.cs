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
