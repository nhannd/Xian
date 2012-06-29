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

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public abstract class LoginServiceRequestBase : DataContractBase
    {
        protected LoginServiceRequestBase(string userName, string clientIP, string clientMachineID)
        {
            this.UserName = userName;
            this.ClientIP = clientIP;
			this.ClientMachineID = clientMachineID;
        }

        /// <summary>
        /// UserName. Required.
        /// </summary>
        [DataMember]
        public string UserName;

        /// <summary>
        /// IP address of the client workstation for auditing purposes. Optional.
        /// </summary>
        [DataMember]
        public string ClientIP;

        /// <summary>
        /// Machine Id of the client workstation for auditing purposes. Optional.
        /// </summary>
        [DataMember]
		public string ClientMachineID;
    }
}
