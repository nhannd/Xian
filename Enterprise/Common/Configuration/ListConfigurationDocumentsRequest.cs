#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListConfigurationDocumentsRequest : DataContractBase
	{
		public ListConfigurationDocumentsRequest(ConfigurationDocumentQuery query)
		{
			this.Query = query;
		}

		public ListConfigurationDocumentsRequest()
		{
			this.Query = new ConfigurationDocumentQuery();
		}

		[DataMember]
		public ConfigurationDocumentQuery Query;
	}
}
