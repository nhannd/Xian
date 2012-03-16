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
	public class ChangePasswordRequest : DataContractBase
	{
		public ChangePasswordRequest(string user, string password, string newPassword)
		{
			this.UserName = user;
			this.CurrentPassword = password;
			this.NewPassword = newPassword;
		}

		/// <summary>
		/// User account.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Current password.
		/// </summary>
		[DataMember]
		public string CurrentPassword;

		/// <summary>
		/// New password.
		/// </summary>
		[DataMember]
		public string NewPassword;

	}
}
