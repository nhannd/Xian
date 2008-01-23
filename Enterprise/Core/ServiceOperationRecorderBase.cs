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
        /// Creates a <see cref="AuditLogEntry"/> for the specified service operation invocation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="serviceClass">The service class that provides the operation.</param>
        /// <param name="operation">The <see cref="MethodInfo"> object that describes the operation.</see></param>
        /// <param name="args">The list of arguments that were provided to the operation.</param>
        /// <returns></returns>
        public AuditLogEntry CreateLogEntry(string operationName, Type serviceClass, MethodInfo operation, object[] args)
        {
            AuditLogEntry logEntry =
                new AuditLogEntry(this.Category, operationName, WriteXml(serviceClass, operation, args));
            OnCreateLogEntry(logEntry, serviceClass, operation, args);
            return logEntry;
        }

        #endregion

        private string WriteXml(Type serviceClass, MethodInfo operation, object[] args)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer, serviceClass, operation, args);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Allows subclasses to adjust the properties of the <see cref="AuditLogEntry"/> that was created.
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="serviceClass"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual void OnCreateLogEntry(AuditLogEntry logEntry, Type serviceClass, MethodInfo operation, object[] args)
        {
            // do nothing
        }

        /// <summary>
        /// Gets the category, which is used to set the <see cref="AuditLogEntry.Category"/> property.
        /// </summary>
        protected abstract string Category { get; }

        /// <summary>
        /// Writes the detailed message to the specified XML writer.  This message will set the <see cref="AuditLogEntry.Details"/> property.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serviceClass"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        protected abstract void WriteXml(XmlWriter writer, Type serviceClass, MethodInfo operation, object[] args);
    }
}
