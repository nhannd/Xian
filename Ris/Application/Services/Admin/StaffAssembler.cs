using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class StaffAssembler
    {
        public StaffSummary CreateStaffSummary(Staff staff)
        {
            StaffSummary summary = new StaffSummary();
            summary.StaffRef = staff.GetRef();

            PersonNameAssembler assembler = new PersonNameAssembler();
            summary.PersonNameDetail = assembler.CreatePersonNameDetail(staff.Name);

            Practitioner practitioner = staff as Practitioner;
            if (practitioner != null)
                summary.LicenseNumber = practitioner.LicenseNumber;

            return summary;
        }

        public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
        {
            StaffDetail detail = new StaffDetail();

            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(staff.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in staff.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneNumberAssembler.CreateTelephoneDetail(phoneNumber, context));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in staff.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddressDetail(address, context));
            }

            return detail;
        }

        public void UpdateStaff(StaffDetail detail, Staff staff)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            assembler.UpdatePersonName(detail.PersonNameDetail, staff.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, staff.TelephoneNumbers);
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (AddressDetail addressDetail in detail.Addresses)
            {
                addressAssembler.AddAddress(addressDetail, staff.Addresses);
            }
        }
    }
}
