using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class StaffAssembler
    {
        public StaffSummary CreateStaffSummary(Staff staff, IPersistenceContext context)
        {
            if (staff == null)
                return null;

            return new StaffSummary(staff.GetRef(), staff.Id,
                EnumUtils.GetEnumValueInfo<StaffType>(staff.Type, context),
                new PersonNameAssembler().CreatePersonNameDetail(staff.Name));
        }

        public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            return new StaffDetail(staff.Id, EnumUtils.GetEnumValueInfo<StaffType>(staff.Type, context),
                assembler.CreatePersonNameDetail(staff.Name));
        }

        public void UpdateStaff(StaffDetail detail, Staff staff)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            staff.Id = detail.StaffId;
            staff.Type = EnumUtils.GetEnumValue<StaffType>(detail.StaffType);
            assembler.UpdatePersonName(detail.Name, staff.Name);
        }
    }
}
