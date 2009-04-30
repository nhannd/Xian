#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
