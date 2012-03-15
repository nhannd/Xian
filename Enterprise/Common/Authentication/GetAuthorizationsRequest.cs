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
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
    public class GetAuthorizationsRequest : DataContractBase, IDefinesCacheKey
	{
		public GetAuthorizationsRequest(string user, SessionToken sessionToken)
		{
			this.UserName = user;
			this.SessionToken = sessionToken;
		}

		/// <summary>
		/// User account to obtain authorizations for.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Session token.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;

        #region IDefinesCacheKey Members

        string IDefinesCacheKey.GetCacheKey()
        {
            return string.Format("{0}:{1}", UserName, SessionToken.Id);
        }

        #endregion
    }
}
