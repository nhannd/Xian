using System;
using System.Management;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Application.Services.Login
{
    public class LoginServiceRecorder : ServiceOperationRecorderBase
    {
        private static string _cpuId;

        /// <summary>
        /// Default constructor required.
        /// </summary>
        public LoginServiceRecorder()
        {
            if (string.IsNullOrEmpty(_cpuId))
                _cpuId = GetCPUId();
        }

        protected override string Category
        {
            get { return "Authentication"; }
        }

        public override AuditLogEntry CreateLogEntry(ServiceOperationInvocationInfo invocationInfo)
        {
            // because the login service is not authenticated, the thread that handles the request 
            // does not have a principal, and the logEntry will not be associated with a user
            // therefore, we modify the user to indicate the userName that submitted the request

            AuditLogEntry logEntry = base.CreateLogEntry(invocationInfo);
            if(logEntry != null)
            {
                LoginServiceRequestBase request = (LoginServiceRequestBase) invocationInfo.Arguments[0];
                logEntry.User = request.UserName;
            }
            return logEntry;
        }

        protected override bool WriteXml(XmlWriter writer, ServiceOperationInvocationInfo info)
        {
            // don't bother logging failed attempts
            if (info.Exception != null)
                return false;

            LoginServiceRequestBase request = (LoginServiceRequestBase) info.Arguments[0];
            writer.WriteStartDocument();
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", info.OperationMethodInfo.Name);
            writer.WriteAttributeString("user", request.UserName);
            writer.WriteAttributeString("clientIP", StringUtilities.EmptyIfNull(request.ClientIP));
            writer.WriteAttributeString("cpuID", StringUtilities.EmptyIfNull(_cpuId));
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        private static string GetCPUId()
        {
            try
            {
                string cpuInfo = null;
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();

                    // only return cpuInfo from first CPU
                    if (!string.IsNullOrEmpty(cpuInfo))
                        break;
                }

                return cpuInfo;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
                return null;
            }
        }
    }
}
