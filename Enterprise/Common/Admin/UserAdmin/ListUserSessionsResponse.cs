#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class ListUserSessionsResponse : DataContractBase
	{
		public ListUserSessionsResponse(string userName, List<UserSessionSummary> sessions)
		{
			UserName = userName;
			Sessions = sessions;
		}

		[DataMember(IsRequired = true)]
		public string UserName;

		/// <summary>
		/// List of sessions for this user (null if there's none)
		/// </summary>
		[DataMember(IsRequired = false)]
		public List<UserSessionSummary> Sessions;
	}
}