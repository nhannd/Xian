using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Login;
using ClearCanvas.Common.Utilities;

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

        protected override void OnCreateLogEntry(AuditLogEntry logEntry, Type serviceClass, MethodInfo operation, object[] args)
        {
            // because the login service is not authenticated, the thread that handles the request 
            // does not have a principal, and the logEntry will not be associated with a user
            // therefore, we modify the user to indicate the userName that submitted the request

            LoginServiceRequestBase request = args[0] as LoginServiceRequestBase;
            if(request != null)
            {
                logEntry.User = request.UserName;
            }
        }

        protected override void WriteXml(XmlWriter writer, Type serviceClass, MethodInfo operation, object[] args)
        {
            LoginServiceRequestBase request = args[0] as LoginServiceRequestBase;
            if (request != null)
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("action");
                writer.WriteAttributeString("type", operation.Name);
                writer.WriteAttributeString("user", request.UserName);
                writer.WriteAttributeString("clientIP", StringUtilities.EmptyIfNull(request.ClientIP));
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
