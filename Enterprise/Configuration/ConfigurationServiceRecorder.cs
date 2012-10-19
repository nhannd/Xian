#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
	public class ConfigurationServiceRecorder : IServiceOperationRecorder
	{
		[DataContract]
		class OperationData
		{
			[DataMember]
			public string Operation;
			[DataMember]
			public string DocumentName;
			[DataMember]
			public string DocumentVersion;
			[DataMember]
			public string DocumentUser;
			[DataMember]
			public string DocumentInstanceKey;
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

			var data = new OperationData
						{
							Operation = "SetConfigurationDocument",
							DocumentName = request.DocumentKey.DocumentName,
							DocumentVersion = request.DocumentKey.Version.ToString(),
							DocumentUser = request.DocumentKey.User ?? "{application}",
							DocumentInstanceKey = StringUtilities.NullIfEmpty(request.DocumentKey.InstanceKey) ?? "{default}"
						};


			var xml = JsmlSerializer.Serialize(data, "Audit");
			recorderContext.Write(data.Operation, xml);
		}
	}
}
