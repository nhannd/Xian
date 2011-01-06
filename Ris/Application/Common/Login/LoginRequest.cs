#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	public class LoginRequest : LoginServiceRequestBase
	{
		public LoginRequest(string user, string password, string hostName, string clientIP, string clientMachineId)
			: base(user, clientIP, clientMachineId)
		{
			this.Password = password;
			this.HostName = hostName;
		}

		/// <summary>
		/// Password. Required.
		/// </summary>
		[DataMember]
		public string Password;

		/// <summary>
		/// Name of the host computer that is logging in.
		/// </summary>
		[DataMember]
		public string HostName;
	}
}
