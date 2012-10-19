#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class ListUserSessionsRequest : DataContractBase
	{
		public ListUserSessionsRequest(string userName)
		{
			UserName = userName;
		}

		/// <summary>
		/// Filter by UserName (required).
		/// </summary>
		[DataMember(IsRequired = true)]
		public string UserName;
	}
}