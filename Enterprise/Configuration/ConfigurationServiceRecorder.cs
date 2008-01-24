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

        protected override void WriteXml(XmlWriter writer, Type serviceClass, MethodInfo operation, object[] args)
        {
            string name = (string) args[0];
            Version version = (Version) args[1];
            string user = (string) args[2];
            string instanceKey = (string) args[3];
            string content = (string) args[4];

            writer.WriteStartDocument();
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", operation.Name);
            writer.WriteAttributeString("documentName", name);
            writer.WriteAttributeString("documentVersion", version.ToString());
            writer.WriteAttributeString("documentUser", user ?? "{application}");
            writer.WriteAttributeString("instanceKey", instanceKey ?? "{default}");
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }
}
