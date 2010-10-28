#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
    public class ConfigurationServiceRecorder : ServiceOperationRecorderBase
    {
		/// <summary>
		/// Gets the category that should be assigned to the audit entries.
		/// </summary>
		public override string Category
        {
            get { return "Configuration"; }
        }

		/// <summary>
		/// Writes the detailed message to the specified XML writer.
		/// </summary>
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
