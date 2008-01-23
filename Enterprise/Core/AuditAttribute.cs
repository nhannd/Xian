using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// When applied to a service operation, specifies that the operation should be
    /// recorded in the audit log using the specified recorder class.
    /// </summary>
    /// <remarks>
    /// The auditor class must implement <see cref="IServiceOperationRecorder"/> and have
    /// a default constructor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuditAttribute : Attribute
    {
        private readonly Type _recorderClass;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recorderClass"></param>
        public AuditAttribute(Type recorderClass)
        {
            _recorderClass = recorderClass;
        }

        /// <summary>
        /// Gets the implementation of <see cref="IServiceOperationRecorder"/> that
        /// will create the audit log entry.
        /// </summary>
        internal Type RecorderClass
        {
            get { return _recorderClass; }
        }
    }
}
