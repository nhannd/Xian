#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Management;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Login;
using ClearCanvas.Enterprise.Common;

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
			// therefore, we need to explicitly specify the user, which is found in the request
			var request = (LoginServiceRequestBase)invocationInfo.Arguments[0];

			// also need to determine the session token if possible
			// note: ChangePassword does not submit a session token, so it is impossible to audit this
			SessionToken sessionToken = null;
			if (request is LoginRequest)
			{
				var response = (LoginResponse)invocationInfo.ReturnValue;
				sessionToken = response.SessionToken;
			}
			else if (request is LogoutRequest)
			{
				sessionToken = ((LogoutRequest)request).SessionToken;
			}

			auditLog.WriteEntry(invocationInfo.OperationName, details, request.UserName, sessionToken == null ? null : sessionToken.Id);
		}
	}
}
