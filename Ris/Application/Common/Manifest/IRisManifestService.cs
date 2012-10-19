#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Manifest
{
	[DataContract]
	public class GetStatusRequest
		: DataContractBase {}

	[DataContract]
	public class GetStatusResponse
		: DataContractBase
	{
		[DataMember]
		public bool IsValid { get; set; }
	}

	/// <summary>
	/// Defines a service for determining the manifest verification status of the server.
	/// </summary>
	[ServiceContract]
	[Authentication(false)]
	[RisApplicationService]
	public interface IRisManifestService
	{
		[OperationContract]
		GetStatusResponse GetStatus(GetStatusRequest request);
	}
}