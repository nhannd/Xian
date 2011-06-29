#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class GetConfigurationDocumentRequest : ConfigurationDocumentRequestBase, IDefinesCacheKey
	{
		public GetConfigurationDocumentRequest(ConfigurationDocumentKey documentKey)
			: base(documentKey)
		{
		}

		#region IDefinesCacheKey Members

		string IDefinesCacheKey.GetCacheKey()
		{
			return ((IDefinesCacheKey)this.DocumentKey).GetCacheKey();
		}

		#endregion
	}
}
