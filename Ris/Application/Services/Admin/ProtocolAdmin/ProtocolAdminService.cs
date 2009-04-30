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
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ProtocolAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IProtocolAdminService))]
    public class ProtocolAdminService : ApplicationServiceBase, IProtocolAdminService
    {
        #region IProtocolAdminService Members

		[ReadOperation]
    	public ListProtocolCodesResponse ListProtocolCodes(ListProtocolCodesRequest request)
    	{
			ProtocolCodeSearchCriteria where = new ProtocolCodeSearchCriteria();
			where.Name.SortAsc(0);
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

    		IList<ProtocolCode> codes = PersistenceContext.GetBroker<IProtocolCodeBroker>().Find(where, request.Page);

			ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
			return new ListProtocolCodesResponse(
				CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(codes,
					delegate (ProtocolCode code)
					{
						return assembler.GetProtocolCodeSummary(code);
					}));
    	}

		[ReadOperation]
		public LoadProtocolCodeForEditResponse LoadProtocolCodeForEdit(LoadProtocolCodeForEditRequest request)
    	{
			ProtocolCode code = this.PersistenceContext.Load<ProtocolCode>(request.ProtocolCodeRef);

			ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
			return new LoadProtocolCodeForEditResponse(assembler.GetProtocolCodeDetail(code));
		}

    	[UpdateOperation]
        public AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request)
        {
			ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
			ProtocolCode protocolCode = new ProtocolCode();
			assembler.UpdateProtocolCode(protocolCode, request.ProtocolCode);
            this.PersistenceContext.Lock(protocolCode, DirtyState.New);

            this.PersistenceContext.SynchState();

            return new AddProtocolCodeResponse(assembler.GetProtocolCodeSummary(protocolCode));
        }

        [UpdateOperation]
        public UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request)
        {
        	ProtocolCode code = PersistenceContext.Load<ProtocolCode>(request.ProtocolCode.ProtocolCodeRef);
			ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
			assembler.UpdateProtocolCode(code, request.ProtocolCode);

			this.PersistenceContext.SynchState();

			return new UpdateProtocolCodeResponse(assembler.GetProtocolCodeSummary(code));
		}

        [UpdateOperation]
        public DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request)
        {
			try
			{
				IProtocolCodeBroker broker = PersistenceContext.GetBroker<IProtocolCodeBroker>();
				ProtocolCode item = broker.Load(request.ProtocolCodeRef, EntityLoadFlags.Proxy);
				broker.Delete(item);

				PersistenceContext.SynchState();

				return new DeleteProtocolCodeResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete,
					TerminologyTranslator.Translate(typeof(ProtocolCode))));
			}
		}

        [ReadOperation]
        public ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request)
        {
			ProtocolGroupSearchCriteria where = new ProtocolGroupSearchCriteria();
			where.Name.SortAsc(0);

			List<ProtocolGroupSummary> protocolGroups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
                this.PersistenceContext.GetBroker<IProtocolGroupBroker>().Find(where, request.Page),
                delegate(ProtocolGroup pg) { return new ProtocolGroupSummary(pg.GetRef(), pg.Name, pg.Description); });

            return new ListProtocolGroupsResponse(protocolGroups);
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProtocolGroups)]
		public LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request)
        {
            ProtocolGroup group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
            return new LoadProtocolGroupForEditResponse(group.GetRef(), assembler.GetProtocolGroupDetail(group, this.PersistenceContext));
        }

        [ReadOperation]
        public GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(
            GetProtocolGroupEditFormDataRequest request)
        {
			ProtocolGroupAssembler protocolAssembler = new ProtocolGroupAssembler();
			List<ProtocolCodeSummary> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
				this.PersistenceContext.GetBroker<IProtocolCodeBroker>().FindAll(false),
				delegate(ProtocolCode code) { return protocolAssembler.GetProtocolCodeSummary(code); });

            ProcedureTypeGroupAssembler assembler = new ProcedureTypeGroupAssembler();

            List<ProcedureTypeGroupSummary> readingGroups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
                this.PersistenceContext.GetBroker<IReadingGroupBroker>().FindAll(),
                delegate(ProcedureTypeGroup readingGroup) { return assembler.GetProcedureTypeGroupSummary(readingGroup, this.PersistenceContext); });

            return new GetProtocolGroupEditFormDataResponse(codes, readingGroups);
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProtocolGroups)]
		public AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request)
        {
            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();

            ProtocolGroup group = new ProtocolGroup();
            assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

            this.PersistenceContext.Lock(group, DirtyState.New);
            this.PersistenceContext.SynchState();

            return new AddProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProtocolGroups)]
		public UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request)
        {
            ProtocolGroup group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
            assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

            this.PersistenceContext.SynchState();

            return new UpdateProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProtocolGroups)]
		public DeleteProtocolGroupResponse DeleteProtocolGroup(DeleteProtocolGroupRequest request)
        {
			try
			{
				IProtocolGroupBroker broker = PersistenceContext.GetBroker<IProtocolGroupBroker>();
				ProtocolGroup item = broker.Load(request.ProtocolGroupRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteProtocolGroupResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ProtocolGroup))));
			}
        }

        #endregion
    }
}
