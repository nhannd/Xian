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

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IStaffAdminService))]
    public class StaffAdminService : ApplicationServiceBase, IStaffAdminService
    {
        [ReadOperation]
        public FindStaffsResponse FindStaffs(FindStaffsRequest request)
        {
            //Note: FamilyName should never be null because the null reference check is done by WCF using the DataMember IsRequired attribute
            StaffSearchCriteria criteria = new StaffSearchCriteria();
            criteria.Name.FamilyName.Like(StringCriteriaWithWildcardAppendedTo(request.FamilyName));
            if (request.GivenName != null)
            {
                criteria.Name.GivenName.Like(StringCriteriaWithWildcardAppendedTo(request.GivenName));
            }

            StaffAssembler assembler = new StaffAssembler();
            return new FindStaffsResponse(
                CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
                    PersistenceContext.GetBroker<IStaffBroker>().Find(criteria),
                    delegate(Staff s)
                    {
                        return assembler.CreateStaffSummary(s);
                    }));
        }

        private string StringCriteriaWithWildcardAppendedTo(string name)
        {
            return name.IndexOf('%') < 0 ? name + "%" : name;
        }

        [ReadOperation]
        public ListAllStaffsResponse ListAllStaffs(ListAllStaffsRequest request)
        {
            StaffAssembler assembler = new StaffAssembler();
            return new ListAllStaffsResponse(
                CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(
                    PersistenceContext.GetBroker<IStaffBroker>().FindAll(),
                    delegate(Staff s)
                    {
                        return assembler.CreateStaffSummary(s);
                    }));
        }

        [ReadOperation]
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
        public AddStaffResponse AddStaff(AddStaffRequest request)
        {
            Staff staff = new Staff();
            StaffAssembler assembler = new StaffAssembler();
            assembler.UpdateStaff(request.StaffDetail, staff);

            // TODO prior to accepting this add request, we should check that the same staff does not already exist

            PersistenceContext.Lock(staff, DirtyState.New);

            // ensure the new staff is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddStaffResponse(assembler.CreateStaffSummary(staff));
        }

        [UpdateOperation]
        public UpdateStaffResponse UpdateStaff(UpdateStaffRequest request)
        {
            Staff staff = (Staff)PersistenceContext.Load(request.StaffRef, EntityLoadFlags.CheckVersion);

            StaffAssembler assembler = new StaffAssembler();
            assembler.UpdateStaff(request.StaffDetail, staff);

            // TODO prior to accepting this update request, we should check that the same staff does not already exist

            return new UpdateStaffResponse(assembler.CreateStaffSummary(staff));
        }
    }
}
