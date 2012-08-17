#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Login
{
	/// <summary>
	/// Provides application login operations
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	[Authentication(false)]
	public interface ILoginService
	{
		/// <summary>
		/// Gets the list of facilities so that the user can choose their current working facility.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request);
	}
}
