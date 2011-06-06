#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ConfigurationDocumentKey : DataContractBase, IDefinesCacheKey
	{
		private string _documentName;
		private Version _version;
		private string _user;
		private string _instanceKey;

		public ConfigurationDocumentKey(string documentName, Version version, string user, string instanceKey)
		{
			_documentName = documentName;
			_version = version;
			_user = user;
			_instanceKey = instanceKey;
		}

		[DataMember]
		public string DocumentName
		{
			get { return _documentName; }
			private set { _documentName = value; }
		}

		[DataMember]
		public Version Version
		{
			get { return _version; }
			private set { _version = value; }
		}

		[DataMember]
		public string User
		{
			get { return _user; }
			private set { _user = value; }
		}

		[DataMember]
		public string InstanceKey
		{
			get { return _instanceKey; }
			private set { _instanceKey = value; }
		}

        #region IDefinesCacheKey Members

        string IDefinesCacheKey.GetCacheKey()
        {
            return string.Format("{0}:{1}:{2}:{3}",
                this.DocumentName,
                this.Version,
                StringUtilities.EmptyIfNull(this.User),
                StringUtilities.EmptyIfNull(this.InstanceKey));
        }

        #endregion
    }
}
