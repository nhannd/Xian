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
	public class TerminateUserSessionRequest : DataContractBase
	{
		public TerminateUserSessionRequest(List<string> sessionIds)
		{
			SessionIds = sessionIds;
		}

		/// <summary>
		/// ID of sessions to be terminated (required)
		/// </summary>
		[DataMember(IsRequired = true)]
		public List<string> SessionIds;
	}
}