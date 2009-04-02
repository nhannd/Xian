using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base implementation of <see cref="IServiceOperationRecorder"/>.
    /// </summary>
    public abstract class ServiceOperationRecorderBase : IServiceOperationRecorder
    {
        #region IServiceOperationRecorder Members

        /// <summary>
        /// Writes the specified service operation invocation to the specified audit log.
        /// </summary>
        public void WriteLogEntry(ServiceOperationInvocationInfo invocationInfo, AuditLog auditLog)
        {
            string xml;
            bool messageGenerated = WriteXml(invocationInfo, out xml);
            if(messageGenerated)
            {
            	WriteToLog(invocationInfo, auditLog, xml);
            }
        }

		/// <summary>
		/// Gets the category that should be assigned to the audit entries.
		/// </summary>
		public abstract string Category { get; }
		
		#endregion

		#region Protected API


        /// <summary>
        /// Writes the detailed message to the specified XML writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="invocationInfo"></param>
        /// <returns>True if a log message was written, or false to indicate that no log message was written.</returns>
        /// <remarks>
        /// The resulting XML will be used to populate the audit log Details property.
        /// Return false to opt out of auditing the specified invocation (for example, if the invocation
        /// inidcates that an exception was thrown by the service, you may not want to audit it).
        /// </remarks>
        protected abstract bool WriteXml(XmlWriter writer, ServiceOperationInvocationInfo invocationInfo);

		/// <summary>
		/// If the call to <see cref="WriteXml(XmlWriter,ServiceOperationInvocationInfo)"/> returns true,
		/// called to write the XML to the audit log.
		/// </summary>
		/// <remarks>
		/// Override this method to customize writing of the log entry.
		/// </remarks>
		/// <param name="invocationInfo"></param>
		/// <param name="auditLog"></param>
		/// <param name="details"></param>
		protected virtual void WriteToLog(ServiceOperationInvocationInfo invocationInfo, AuditLog auditLog, string details)
		{
			auditLog.WriteEntry(invocationInfo.OperationName, details);
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

	}
}
