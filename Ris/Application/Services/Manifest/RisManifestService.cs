#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Manifest;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Ris.Application.Services.Manifest
{
	[ServiceImplementsContract(typeof (IRisManifestService))]
	[ExtensionOf(typeof (ApplicationServiceExtensionPoint))]
	public class RisManifestService : ApplicationServiceBase, IRisManifestService
	{
		public GetStatusResponse GetStatus(GetStatusRequest request)
		{
			Platform.Log(LogLevel.Debug, "Received manifest status query.");
			return new GetStatusResponse {IsValid = ManifestVerification.Valid};
		}
	}
}