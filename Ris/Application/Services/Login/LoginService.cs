#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Application.Services.Login
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(ILoginService))]
	public class LoginService : ApplicationServiceBase, ILoginService
	{
		#region ILoginService Members

		[ReadOperation]
		public GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request)
		{
			// load all facilities and sort by code
			var facilities = PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false);
			facilities = facilities.OrderBy(x => x.Code).ToList();

			var facilityAssembler = new FacilityAssembler();
			return new GetWorkingFacilityChoicesResponse(
				CollectionUtils.Map(facilities,
					(Facility input) => facilityAssembler.CreateFacilitySummary(input)));
		}

		#endregion
	}
}
