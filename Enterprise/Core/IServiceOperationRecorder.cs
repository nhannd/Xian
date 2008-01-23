using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines an interface for creating a <see cref="AuditLogEntry"/> that records
    /// information about the invocation of a service operation.
    /// </summary>
    public interface IServiceOperationRecorder
    {
        /// <summary>
        /// Creates a <see cref="AuditLogEntry"/> for the specified service operation invocation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="serviceClass">The service class that provides the operation.</param>
        /// <param name="operation">The <see cref="MethodInfo"> object that describes the operation.</see></param>
        /// <param name="args">The list of arguments that were provided to the operation.</param>
        /// <returns></returns>
        AuditLogEntry CreateLogEntry(string operationName, Type serviceClass, MethodInfo operation, object[] args);
    }
}
