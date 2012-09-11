#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Xml;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
	public class ConfigurationServiceRecorder : IServiceOperationRecorder
	{
		public string Application
		{
			get { return "Enterprise Server"; }
		}

		string IServiceOperationRecorder.Category
		{
			get { return "Configuration"; }
		}

		public void PreCommit(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContent)
		{
		}

		public void PostCommit(IServiceOperationRecorderContext recorderContext)
		{
			var request = (ConfigurationDocumentRequestBase)recorderContext.Request;

			var sw = new StringWriter();
			using (var writer = new XmlTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("action");
				writer.WriteAttributeString("type", recorderContext.OperationMethodInfo.Name);
				writer.WriteAttributeString("documentName", request.DocumentKey.DocumentName);
				writer.WriteAttributeString("documentVersion", request.DocumentKey.Version.ToString());
				writer.WriteAttributeString("documentUser", request.DocumentKey.User ?? "{application}");
				writer.WriteAttributeString("instanceKey", request.DocumentKey.InstanceKey ?? "{default}");
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}

			recorderContext.Write(sw.ToString());
		}
	}
}
