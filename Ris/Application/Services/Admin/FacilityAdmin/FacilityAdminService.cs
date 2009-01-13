#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IFacilityAdminService))]
    public class FacilityAdminService : ApplicationServiceBase, IFacilityAdminService
    {
        #region IFacilityAdminService Members

        [ReadOperation]
        public ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request)
        {
            FacilitySearchCriteria criteria = new FacilitySearchCriteria();
			criteria.Code.SortAsc(0);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

            FacilityAssembler assembler = new FacilityAssembler();
            return new ListAllFacilitiesResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().Find(criteria, request.Page),
                    delegate(Facility f)
                    {
                        return assembler.CreateFacilitySummary(f);
                    }));
        }

        [ReadOperation]
        public GetFacilityEditFormDataResponse GetFacilityEditFormData(GetFacilityEditFormDataRequest request)
        {
            return new GetFacilityEditFormDataResponse(EnumUtils.GetEnumValueList<InformationAuthorityEnum>(PersistenceContext));
        }

        [ReadOperation]
        public LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request)
        {
            // note that the version of the FacilityRef is intentionally ignored here (default behaviour of ReadOperation)
            Facility f = PersistenceContext.Load<Facility>(request.FacilityRef);
            FacilityAssembler assembler = new FacilityAssembler();

            return new LoadFacilityForEditResponse(assembler.CreateFacilityDetail(f));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
        public AddFacilityResponse AddFacility(AddFacilityRequest request)
        {
            Facility facility = new Facility();
            FacilityAssembler assembler = new FacilityAssembler();
            assembler.UpdateFacility(request.FacilityDetail, facility, this.PersistenceContext);

            PersistenceContext.Lock(facility, DirtyState.New);

            // ensure the new facility is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddFacilityResponse(assembler.CreateFacilitySummary(facility));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
		public UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request)
        {
            Facility facility = PersistenceContext.Load<Facility>(request.FacilityDetail.FacilityRef, EntityLoadFlags.CheckVersion);

            FacilityAssembler assembler = new FacilityAssembler();
            assembler.UpdateFacility(request.FacilityDetail, facility, this.PersistenceContext);

            return new UpdateFacilityResponse(assembler.CreateFacilitySummary(facility));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
		public DeleteFacilityResponse DeleteFacility(DeleteFacilityRequest request)
		{
			try
			{
				IFacilityBroker broker = PersistenceContext.GetBroker<IFacilityBroker>();
				Facility item = broker.Load(request.FacilityRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteFacilityResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Facility))));
			}
		}

		#endregion

    }
}
