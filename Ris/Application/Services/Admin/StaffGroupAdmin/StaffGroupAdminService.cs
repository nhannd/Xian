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

using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;
using System.Threading;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffGroupAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IStaffGroupAdminService))]
    public class StaffGroupAdminService : ApplicationServiceBase, IStaffGroupAdminService
    {
        #region IStaffGroupAdminService Members

        [ReadOperation]
        public TextQueryResponse<StaffGroupSummary> TextQuery(StaffGroupTextQueryRequest request)
        {
            var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
            var assembler = new StaffGroupAssembler();

            var helper = new TextQueryHelper<StaffGroup, StaffGroupSearchCriteria, StaffGroupSummary>(
                    delegate
                    {
                        var rawQuery = request.TextQuery;

                        // allow matching on name (assume entire query is a name which may contain spaces)
                        var nameCriteria = new StaffGroupSearchCriteria();
                        nameCriteria.Name.StartsWith(rawQuery);
						if(request.ElectiveGroupsOnly)
							nameCriteria.Elective.EqualTo(true);

                        return new []{ nameCriteria };
                    },
                    assembler.CreateSummary,
                    (criteria, threshold) => broker.Count(criteria) <= threshold,
                    broker.Find);

            return helper.Query(request);
        }

        [ReadOperation]
        public ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request)
        {
            Platform.CheckForNullReference(request, "request");

        	var where = new StaffGroupSearchCriteria();
			where.Name.SortAsc(0);
			if (request.ElectiveGroupsOnly)
				where.Elective.EqualTo(true);
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

            var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
			var items = broker.Find(where, request.Page);

            var assembler = new StaffGroupAssembler();
            return new ListStaffGroupsResponse(
                CollectionUtils.Map(items, (StaffGroup item) => assembler.CreateSummary(item))
                );
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public LoadStaffGroupForEditResponse LoadStaffGroupForEdit(LoadStaffGroupForEditRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroupRef, "request.StaffGroupRef");

            var item = PersistenceContext.Load<StaffGroup>(request.StaffGroupRef);

            var assembler = new StaffGroupAssembler();
            return new LoadStaffGroupForEditResponse(assembler.CreateDetail(item, PersistenceContext));
        }

        [ReadOperation]
        public LoadStaffGroupEditorFormDataResponse LoadStaffGroupEditorFormData(LoadStaffGroupEditorFormDataRequest request)
        {
			var allStaff = PersistenceContext.GetBroker<IStaffBroker>().FindAll(false);

			var worklistClasses = WorklistAdminService.ListClassesHelper(null, null, false);

			// grab the persistent worklists
			var broker = PersistenceContext.GetBroker<IWorklistBroker>();
			var persistentClassNames = 
				CollectionUtils.Select(worklistClasses, t => !Worklist.GetIsStatic(t))
				.ConvertAll(t => Worklist.GetClassName(t));

			var adminWorklists = broker.Find(null, false, persistentClassNames, null);
			
			var staffAssembler = new StaffAssembler();
			var worklistAssembler = new WorklistAssembler();
            return new LoadStaffGroupEditorFormDataResponse(
                CollectionUtils.Map(allStaff, (Staff staff) => staffAssembler.CreateStaffSummary(staff, PersistenceContext)),
				CollectionUtils.Map(adminWorklists, (Worklist worklist) => worklistAssembler.GetWorklistSummary(worklist, PersistenceContext))
				);
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public AddStaffGroupResponse AddStaffGroup(AddStaffGroupRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroup, "request.StaffGroup");

            var item = new StaffGroup();

            var assembler = new StaffGroupAssembler();
			var worklistEditable = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Worklist);
			assembler.UpdateStaffGroup(item, request.StaffGroup, worklistEditable, true, PersistenceContext);

            PersistenceContext.Lock(item, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddStaffGroupResponse(assembler.CreateSummary(item));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public UpdateStaffGroupResponse UpdateStaffGroup(UpdateStaffGroupRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.StaffGroup, "request.StaffGroup");
            Platform.CheckMemberIsSet(request.StaffGroup.StaffGroupRef, "request.StaffGroup.StaffGroupRef");

            var item = PersistenceContext.Load<StaffGroup>(request.StaffGroup.StaffGroupRef);

            var assembler = new StaffGroupAssembler();
			var worklistEditable = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.Worklist);
			assembler.UpdateStaffGroup(item, request.StaffGroup, worklistEditable, false, PersistenceContext);

            PersistenceContext.SynchState();

            return new UpdateStaffGroupResponse(assembler.CreateSummary(item));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.StaffGroup)]
		public DeleteStaffGroupResponse DeleteStaffGroup(DeleteStaffGroupRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IStaffGroupBroker>();
				var item = broker.Load(request.StaffGroupRef, EntityLoadFlags.Proxy);

				// Remove worklist association before deleting a staff group
				var worklists = PersistenceContext.GetBroker<IWorklistBroker>().Find(item);
				CollectionUtils.ForEach(worklists, worklist => worklist.GroupSubscribers.Remove(item));
				
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteStaffGroupResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(StaffGroup))));
			}
		}

    	#endregion
    }
}
