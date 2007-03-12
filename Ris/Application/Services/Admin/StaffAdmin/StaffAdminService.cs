using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.StaffAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class StaffAdminService : ApplicationServiceBase, IStaffAdminService
    {
        [ReadOperation]
        public IList<StaffAdmin> FindStaffs(string surname, string givenName)
        {
            Platform.CheckForNullReference(surname, "surname");

            IStaffBroker broker = this.CurrentContext.GetBroker<IStaffBroker>();

            StaffSearchCriteria criteria = new StaffSearchCriteria();
            criteria.Name.FamilyName.Like(StringCriteriaWithWildcardAppendedTo(surname));
            if (givenName != null)
            {
                criteria.Name.GivenName.Like(StringCriteriaWithWildcardAppendedTo(givenName));
            }

            return broker.Find(criteria);
        }

        private string StringCriteriaWithWildcardAppendedTo(string surname)
        {
            return surname.IndexOf('%') < 0 ? surname + "%" : surname;
        }

        [ReadOperation]
        public IList<StaffAdmin> GetAllStaffs()
        {
            return this.CurrentContext.GetBroker<IStaffBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddStaff(StaffAdmin staff)
        {
            this.CurrentContext.Lock(staff, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdateStaff(StaffAdmin staff)
        {
            this.CurrentContext.Lock(staff, DirtyState.Dirty);
        }

        [ReadOperation]
        public StaffAdmin LoadStaff(EntityRef staffRef, bool withDetails)
        {
            IStaffBroker staffBroker = CurrentContext.GetBroker<IStaffBroker>();
            StaffAdmin staff = staffBroker.Load(staffRef);
            if (withDetails)
            {
                staffBroker.LoadAddressesForStaff(staff);
                staffBroker.LoadTelephoneNumbersForStaff(staff);
            }
            return staff;
        }
    }
}
