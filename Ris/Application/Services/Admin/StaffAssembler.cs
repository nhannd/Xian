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
            summary.FamilyName = staff.Name.FamilyName;
            summary.GivenName = staff.Name.GivenName;
            summary.MiddleName = staff.Name.MiddleName;
            summary.Prefix = staff.Name.Prefix;
            summary.Suffix = staff.Name.Suffix;
            summary.Degree = staff.Name.Degree;

            Practitioner practitioner = staff as Practitioner;
            if (practitioner != null)
                summary.LicenseNumber = practitioner.LicenseNumber;

            return summary;
        }

        public StaffDetail CreateStaffDetail(Staff staff)
        {
            StaffDetail detail = new StaffDetail();
            detail.FamilyName = staff.Name.FamilyName;
            detail.GivenName = staff.Name.GivenName;
            detail.MiddleName = staff.Name.MiddleName;
            detail.Prefix = staff.Name.Prefix;
            detail.Suffix = staff.Name.Suffix;
            detail.Degree = staff.Name.Degree;

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (ClearCanvas.Healthcare.TelephoneNumber phoneNumber in staff.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneNumberAssembler.CreateTelephoneNumber(phoneNumber));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (ClearCanvas.Healthcare.Address address in staff.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddress(address));
            }

            return detail;
        }

        public void UpdateStaff(Staff staff, StaffDetail detail)
        {
            detail.FamilyName = staff.Name.FamilyName;
            detail.GivenName = staff.Name.GivenName;
            detail.MiddleName = staff.Name.MiddleName;
            detail.Prefix = staff.Name.Prefix;
            detail.Suffix = staff.Name.Suffix;
            detail.Degree = staff.Name.Degree;

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (ClearCanvas.Healthcare.TelephoneNumber phoneNumber in staff.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneNumber, detail.TelephoneNumbers);
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (ClearCanvas.Healthcare.Address address in staff.Addresses)
            {
                telephoneNumberAssembler.AddAddress(address, detail.Addresses);
            }
        }
    }
}
