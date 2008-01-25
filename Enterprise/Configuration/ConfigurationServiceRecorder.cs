using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
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

            string name = (string) info.Arguments[0];
            Version version = (Version)info.Arguments[1];
            string user = (string)info.Arguments[2];
            string instanceKey = (string)info.Arguments[3];

            writer.WriteStartDocument();
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", info.OperationMethodInfo.Name);
            writer.WriteAttributeString("documentName", name);
            writer.WriteAttributeString("documentVersion", version.ToString());
            writer.WriteAttributeString("documentUser", user ?? "{application}");
            writer.WriteAttributeString("instanceKey", instanceKey ?? "{default}");
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }
    }
}
