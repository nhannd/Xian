#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListConfigurationDocumentsResponse : DataContractBase
	{
		public ListConfigurationDocumentsResponse(List<ConfigurationDocumentHeader> documents)
		{
			Documents = documents;
		}

		[DataMember]
		public List<ConfigurationDocumentHeader> Documents;
	}
}
