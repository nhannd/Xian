#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class UserSessionSummary : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public string SessionId;

		[DataMember(IsRequired = false)]
		public string Application;

		[DataMember(IsRequired = false)]
		public string HostName;

		[DataMember(IsRequired = false)]
		public DateTime CreationTime;

		[DataMember(IsRequired = false)]
		public DateTime ExpiryTime;
	}
}