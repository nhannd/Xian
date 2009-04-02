using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
    public class ConfigurationServiceRecorder : ServiceOperationRecorderBase
    {
        protected override string Category
        {
            get { return "Configuration"; }
        }

        protected override bool WriteXml(XmlWriter writer, ServiceOperationInvocationInfo info)
        {
            // don't bother logging failed attempts
            if(info.Exception != null)
                return false;

			ConfigurationDocumentRequestBase request = (ConfigurationDocumentRequestBase)info.Arguments[0];

            writer.WriteStartDocument();
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", info.OperationMethodInfo.Name);
            writer.WriteAttributeString("documentName", request.DocumentKey.DocumentName);
            writer.WriteAttributeString("documentVersion", request.DocumentKey.Version.ToString());
            writer.WriteAttributeString("documentUser", request.DocumentKey.User ?? "{application}");
            writer.WriteAttributeString("instanceKey", request.DocumentKey.InstanceKey ?? "{default}");
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }
    }
}
