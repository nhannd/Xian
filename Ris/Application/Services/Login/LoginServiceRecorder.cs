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
        /// <summary>
        /// Default constructor required.
        /// </summary>
        public LoginServiceRecorder()
        {
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
            writer.WriteAttributeString("user", StringUtilities.EmptyIfNull(request.UserName));
            writer.WriteAttributeString("clientIP", StringUtilities.EmptyIfNull(request.ClientIP));
            writer.WriteAttributeString("cpuID", StringUtilities.EmptyIfNull(request.ClientCpuID));
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }
    }
}
