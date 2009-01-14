using System;
using System.Collections.Generic;
using System.Text;
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
