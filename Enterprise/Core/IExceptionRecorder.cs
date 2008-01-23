using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines an interface for creating an <see cref="ExceptionLogEntry"/> that records
    /// information about an exception.
    /// </summary>
    public interface IExceptionRecorder
    {
        /// <summary>
        /// Creates a <see cref="ExceptionLogEntry"/> for the specified operation and exception.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="e">The exception that was thrown.</param>
        /// <returns></returns>
        ExceptionLogEntry CreateLogEntry(string operation, Exception e);
    }
}
