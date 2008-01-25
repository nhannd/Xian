using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base implementation of <see cref="IServiceOperationRecorder"/>.
    /// </summary>
    public abstract class ServiceOperationRecorderBase : IServiceOperationRecorder
    {
        #region IServiceOperationRecorder Members

        /// <summary>
        /// Creates a <see cref="AuditLogEntry"/> for the specified service operation invocation,
        /// or returns null if no log entry is created.
        /// </summary>
        /// <remarks>
        /// Override this method to perform custom creation of the <see cref="AuditLogEntry"/>.
        /// In most cases this method does not need to be overridden - instead just override
        /// <see cref="WriteXml(XmlWriter,ServiceOperationInvocationInfo)"/>.
        /// </remarks>
        /// <returns>A log entry, or null to indicate that no log entry should be created.</returns>
        public virtual AuditLogEntry CreateLogEntry(ServiceOperationInvocationInfo invocationInfo)
        {
            string xml;
            bool messageGenerated = WriteXml(invocationInfo, out xml);
            if(messageGenerated)
            {
                return new AuditLogEntry(this.Category, invocationInfo.OperationName, xml);
            }
            return null;
        }

        #endregion

        private bool WriteXml(ServiceOperationInvocationInfo invocationInfo, out string xml)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                bool success = WriteXml(writer, invocationInfo);
                xml = success ? sw.ToString() : null;
                return success;
            }
        }

        /// <summary>
        /// Gets the category, which is used to set the <see cref="AuditLogEntry.Category"/> property.
        /// </summary>
        protected abstract string Category { get; }

        /// <summary>
        /// Writes the detailed message to the specified XML writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="invocationInfo"></param>
        /// <returns>True if a log message was written, or false to indicate that no log message was written.</returns>
        /// <remarks>
        /// The resulting XML will be used to populate the <see cref="AuditLogEntry.Details"/> property.
        /// Return false to opt out of auditing the specified invocation (for example, if the invocation
        /// inidcates that an exception was thrown by the service, you may not need to audit it).
        /// </remarks>
        protected abstract bool WriteXml(XmlWriter writer, ServiceOperationInvocationInfo invocationInfo);
    }
}
