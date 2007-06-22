using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class StaffAssembler
    {
        public StaffSummary CreateStaffSummary(Staff staff)
        {
            if (staff == null)
                return null;

            PersonNameAssembler assembler = new PersonNameAssembler();
            return new StaffSummary(
                staff.GetRef(),
                assembler.CreatePersonNameDetail(staff.Name),
                (staff.Is<Practitioner>() ? (staff.As<Practitioner>()).LicenseNumber : ""));
        }

        public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            return new StaffDetail(
                assembler.CreatePersonNameDetail(staff.Name),
                CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(
                    staff.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return telephoneNumberAssembler.CreateTelephoneDetail(phone, context);
                    }),
                CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(
                    staff.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateAddressDetail(address, context);
                    }),
                    (staff.Is<Practitioner>() ? (staff.As<Practitioner>()).LicenseNumber : "")
                    );
        }

        public void UpdateStaff(StaffDetail detail, Staff staff)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            assembler.UpdatePersonName(detail.PersonNameDetail, staff.Name);

            foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, staff.TelephoneNumbers);
            }

            foreach (AddressDetail addressDetail in detail.Addresses)
            {
                addressAssembler.AddAddress(addressDetail, staff.Addresses);
            }

            if (staff.Is<Practitioner>())
                (staff.As<Practitioner>()).LicenseNumber = detail.LicenseNumber;
        }
    }
}
