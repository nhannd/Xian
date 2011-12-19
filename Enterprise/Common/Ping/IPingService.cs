#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Common.Ping
{
	[DataContract]
	public class PingRequest : DataContractBase, IDefinesCacheKey //cache so we're not hitting the service constantly.
	{
		#region IDefinesCacheKey Members

		public string GetCacheKey()
		{
			return "Ping";
		}

		#endregion
	}

	[DataContract]
	public class PingResponse : DataContractBase
	{
	}

	/// <summary>
	/// Defines a "pingable" service.
	/// </summary>
	/// <remarks>
	/// This does not necessarily have to be a service in and of itself.  It can also be implemented by other services
	/// to indicate that they are alive.
	/// </remarks>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(false)]
	public interface IPingService
	{
		/// <summary>
		/// Verifies that the service is up and running.
		/// </summary>
		[OperationContract]
		PingResponse Ping(PingRequest request);
	}
}
