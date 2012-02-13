#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
    [DataContract]
    public class ResetPasswordResponse : DataContractBase
    {
        public ResetPasswordResponse(string email)
        {
            EmailAddress = email;
        }

        /// <summary>
        /// Email address the reset password email was sent to.
        /// </summary>
        [DataMember]
        public string EmailAddress;
    }
}
