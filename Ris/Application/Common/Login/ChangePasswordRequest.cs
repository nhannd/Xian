#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public class ChangePasswordRequest : LoginServiceRequestBase
    {
		public ChangePasswordRequest(string user, string password, string newPassword, string clientIP, string clientMachineID)
			: base(user, clientIP, clientMachineID)
        {
            this.Password = password;
            this.NewPassword = newPassword;
        }

        /// <summary>
        /// Password. Required.
        /// </summary>
        [DataMember]
        public string Password;

        /// <summary>
        /// New Password. Required.
        /// </summary>
        [DataMember]
        public string NewPassword;
    }
}
