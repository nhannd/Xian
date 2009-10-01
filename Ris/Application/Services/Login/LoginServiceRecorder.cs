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
