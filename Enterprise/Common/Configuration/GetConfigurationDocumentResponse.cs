#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Configuration;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class GetConfigurationDocumentResponse : DataContractBase
	{
		public GetConfigurationDocumentResponse(ConfigurationDocumentKey documentKey, DateTime? creationTime, DateTime? modifiedTime, string content)
		{
			DocumentKey = documentKey;
			CreationTime = creationTime;
			ModifiedTime = modifiedTime;
			Content = content;
		}

		[DataMember]
		public ConfigurationDocumentKey DocumentKey;

		[DataMember]
		public DateTime? CreationTime;

		[DataMember]
		public DateTime? ModifiedTime;

		[DataMember]
		public string Content;
	}
}
