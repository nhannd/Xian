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
	public class TerminateSessionRequest : DataContractBase
	{
		public TerminateSessionRequest(string userName, SessionToken token)
		{
			this.UserName = userName;
			this.SessionToken = token;
		}

		/// <summary>
		/// User account.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Session token identifying session to terminate.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;

	}
}
