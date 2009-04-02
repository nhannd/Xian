using System;
using System.Management;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Application.Services.Login
{
	/// <summary>
	/// Records custom information about operations on <see cref="ILoginService"/>.
	/// </summary>
    public class LoginServiceRecorder : ServiceOperationRecorderBase
    {
        /// <summary>
        /// Default constructor required.
        /// </summary>
        public LoginServiceRecorder()
        {
        }

		/// <summary>
		/// Gets the category that should be assigned to the audit entries.
		/// </summary>
		public override string Category
        {
            get { return "Authentication"; }
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
            writer.WriteAttributeString("machineID", StringUtilities.EmptyIfNull(request.ClientMachineID));
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }

		protected override void WriteToLog(ServiceOperationInvocationInfo invocationInfo, AuditLog auditLog, string details)
		{
			// because the login service is not authenticated, the thread that handles the request 
			// does not have a principal, and the logEntry will not be associated with a user
			// therefore, we explicitly specify the user to indicate who submitted the request
			LoginServiceRequestBase request = (LoginServiceRequestBase)invocationInfo.Arguments[0];
			auditLog.WriteEntry(invocationInfo.OperationName, details, request.UserName);
		}
	}
}
