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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	[DataContract]
	public class ListSettingsPropertiesRequest : DataContractBase, IDefinesCacheKey
	{
		public ListSettingsPropertiesRequest(SettingsGroupDescriptor group)
		{
			Group = group;
		}

		[DataMember]
		public SettingsGroupDescriptor Group;

        #region IDefinesCacheKey Members

        public string GetCacheKey()
        {
            return string.Format("{0}:{1}", this.Group.Name, this.Group.Version);
        }

        #endregion
    }
}
