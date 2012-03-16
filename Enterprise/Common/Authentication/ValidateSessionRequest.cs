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
	public class ValidateSessionRequest : DataContractBase, IDefinesCacheKey
	{
		public ValidateSessionRequest(string userName, SessionToken sessionToken)
		{
			this.UserName = userName;
			this.SessionToken = sessionToken;
		}

		/// <summary>
		/// User account.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Session token.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;

		/// <summary>
		/// Indicates whether the set of authorizations for this user should be returned in the response.
		/// </summary>
		[DataMember]
		public bool GetAuthorizations;


        #region IDefinesCacheKey Members

        string IDefinesCacheKey.GetCacheKey()
        {
            return string.Format("{0}:{1}:{2}", UserName, SessionToken.Id, GetAuthorizations);
        }

        #endregion
    }
}
