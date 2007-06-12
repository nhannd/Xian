using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IStaffAdminService))]
    public class StaffAdminService : ApplicationServiceBase, IStaffAdminService
    {
        #region IStaffAdminService Members

        [ReadOperation]
        // note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin
        // because it is used in non-admin situations - perhaps we need to create a separate operation???
        public ListStaffResponse ListStaff(ListStaffRequest request)
        {
            StaffSearchCriteria criteria = new StaffSearchCriteria();
            if (!string.IsNullOrEmpty(request.FirstName))
                criteria.Name.GivenName.StartsWith(request.FirstName);
            if (!string.IsNullOrEmpty(request.LastName))
                criteria.Name.FamilyName.StartsWith(request.LastName);

            SearchResultPage page = new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows);

            StaffAssembler assembler = new StaffAssembler();
            return new ListStaffResponse(
                CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
                    PersistenceContext.GetBroker<IStaffBroker>().Find(criteria, page),
                    delegate(Staff s)
                    {
                        return assembler.CreateStaffSummary(s);
                    }));
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]
        public LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request)
        {
            // note that the version of the StaffRef is intentionally ignored here (default behaviour of ReadOperation)
            Staff s = (Staff)PersistenceContext.Load(request.StaffRef);
            StaffAssembler assembler = new StaffAssembler();

            return new LoadStaffForEditResponse(s.GetRef(), assembler.CreateStaffDetail(s, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request)
        {
            //TODO:  replace "dummy" lists
            List<string> dummyCountries = new List<string>();
            dummyCountries.Add("Canada");

            List<string> dummyProvinces = new List<string>();
            dummyProvinces.Add("Ontario");

            return new LoadStaffEditorFormDataResponse(
                CollectionUtils.Map<AddressTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                    PersistenceContext.GetBroker<IAddressTypeEnumBroker>().Load().Items,
                    delegate(AddressTypeEnum e)
                    {
                        return new EnumValueInfo(e.Code.ToString(), e.Value);
                    }),
                dummyProvinces,
                dummyCountries,
                (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false));

        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]
        public AddStaffResponse AddStaff(AddStaffRequest request)
        {
            Staff staff = new Staff();
            StaffAssembler assembler = new StaffAssembler();
            assembler.UpdateStaff(request.StaffDetail, staff);

            CheckForDuplicateStaff(request.StaffDetail.PersonNameDetail.FamilyName,
                request.StaffDetail.PersonNameDetail.GivenName,
                staff);

            PersistenceContext.Lock(staff, DirtyState.New);

            // ensure the new staff is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddStaffResponse(assembler.CreateStaffSummary(staff));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]
        public UpdateStaffResponse UpdateStaff(UpdateStaffRequest request)
        {
            Staff staff = (Staff)PersistenceContext.Load(request.StaffRef, EntityLoadFlags.CheckVersion);

            StaffAssembler assembler = new StaffAssembler();
            assembler.UpdateStaff(request.StaffDetail, staff);

            CheckForDuplicateStaff(request.StaffDetail.PersonNameDetail.FamilyName,
                request.StaffDetail.PersonNameDetail.GivenName,
                staff);

            return new UpdateStaffResponse(assembler.CreateStaffSummary(staff));
        }

        #endregion

        /// <summary>
        /// Helper method to check that the staff with the same name does not already exist
        /// </summary>
        /// <param name="familiyName"></param>
        /// <param name="givenName"></param>
        /// <param name="subject"></param>
        private void CheckForDuplicateStaff(string familiyName, string givenName, Staff subject)
        {
            try
            {
                StaffSearchCriteria where = new StaffSearchCriteria();
                where.Name.FamilyName.EqualTo(familiyName);
                where.Name.GivenName.EqualTo(givenName);

                IList<Staff> duplicateList = PersistenceContext.GetBroker<IStaffBroker>().Find(where, new SearchResultPage(0, 2));
                foreach (Staff duplicate in duplicateList)
                {
                    if (duplicate != subject)
                        throw new RequestValidationException(string.Format(SR.ExceptionStaffAlreadyExist, familiyName, givenName));
                }
            }
            catch (EntityNotFoundException)
            {
                // no duplicates
            }
        }
    }
}
