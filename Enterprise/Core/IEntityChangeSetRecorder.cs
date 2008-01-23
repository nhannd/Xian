using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines an interface for creating a <see cref="AuditLogEntry"/> that records
    /// information about a set of <see cref="EntityChange"/> objects.
    /// </summary>
    public interface IEntityChangeSetRecorder
    {
        /// <summary>
        /// Gets or sets a logical operation name for the operation that produced the change set.
        /// </summary>
        string OperationName { get; set; }

        /// <summary>
        /// Creates a <see cref="AuditLogEntry"/> for the specified change set.
        /// </summary>
        /// <param name="changeSet"></param>
        /// <returns></returns>
        AuditLogEntry CreateLogEntry(IEnumerable<EntityChange> changeSet);
    }
}
