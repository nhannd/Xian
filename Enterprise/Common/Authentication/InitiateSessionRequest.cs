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

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class InitiateSessionRequest : DataContractBase
	{
        public InitiateSessionRequest(string user, string application, string hostName, string password)
            :this(user, application, hostName, password, false)
        {
        }

		public InitiateSessionRequest(string user, string application, string hostName, string password, bool getAuthorizations)
		{
			this.UserName = user;
			this.Password = password;
			this.Application = application;
			this.HostName = hostName;
            this.GetAuthorizations = getAuthorizations;
		}

		/// <summary>
		/// User account to begin session for.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Application name.
		/// </summary>
		[DataMember]
		public string Application;

		/// <summary>
		/// Host computer name.
		/// </summary>
		[DataMember]
		public string HostName;

		/// <summary>
		/// Password.
		/// </summary>
		[DataMember]
		public string Password;

		/// <summary>
		/// Indicates whether the set of authorizations for this user should be returned in the response.
		/// </summary>
		[DataMember]
		public bool GetAuthorizations;

	}
}
