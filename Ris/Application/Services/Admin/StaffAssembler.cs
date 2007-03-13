using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

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

        public StaffDetail CreateStaffDetail(Staff staff)
        {
            StaffDetail detail = new StaffDetail();

            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(staff.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in staff.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneNumberAssembler.CreateTelephoneDetail(phoneNumber));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in staff.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddressDetail(address));
            }

            return detail;
        }

        public void UpdateStaff(Staff staff, StaffDetail detail)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(staff.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in staff.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneNumber, detail.TelephoneNumbers);
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in staff.Addresses)
            {
                telephoneNumberAssembler.AddAddress(address, detail.Addresses);
            }
        }
    }
}
