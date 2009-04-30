#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ModalityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityAdminService))]
    public class ModalityAdminService : ApplicationServiceBase, IModalityAdminService
    {
        #region IModalityAdminService Members

        [ReadOperation]
        public ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request)
        {
            ModalitySearchCriteria criteria = new ModalitySearchCriteria();
			criteria.Id.SortAsc(0);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

            ModalityAssembler assembler = new ModalityAssembler();
            return new ListAllModalitiesResponse(
                CollectionUtils.Map<Modality, ModalitySummary, List<ModalitySummary>>(
                    PersistenceContext.GetBroker<IModalityBroker>().Find(criteria, request.Page),
                    delegate(Modality m)
                    {
                        return assembler.CreateModalitySummary(m);
                    }));
        }

        [ReadOperation]
        public LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request)
        {
            // note that the version of the ModalityRef is intentionally ignored here (default behaviour of ReadOperation)
            Modality m = PersistenceContext.Load<Modality>(request.ModalityRef);
            ModalityAssembler assembler = new ModalityAssembler();

            return new LoadModalityForEditResponse(assembler.CreateModalityDetail(m));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Modality)]
        public AddModalityResponse AddModality(AddModalityRequest request)
        {
            Modality modality = new Modality();
            ModalityAssembler assembler = new ModalityAssembler();
            assembler.UpdateModality(request.ModalityDetail, modality);

            PersistenceContext.Lock(modality, DirtyState.New);

            // ensure the new modality is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddModalityResponse(assembler.CreateModalitySummary(modality));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Modality)]
		public UpdateModalityResponse UpdateModality(UpdateModalityRequest request)
        {
            Modality modality = PersistenceContext.Load<Modality>(request.ModalityDetail.ModalityRef, EntityLoadFlags.CheckVersion);

            ModalityAssembler assembler = new ModalityAssembler();
            assembler.UpdateModality(request.ModalityDetail, modality);

            return new UpdateModalityResponse(assembler.CreateModalitySummary(modality));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Modality)]
		public DeleteModalityResponse DeleteModality(DeleteModalityRequest request)
		{
			try
			{
				IModalityBroker broker = PersistenceContext.GetBroker<IModalityBroker>();
				Modality item = broker.Load(request.ModalityRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteModalityResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Modality))));
			}
		}

        #endregion

    }
}
