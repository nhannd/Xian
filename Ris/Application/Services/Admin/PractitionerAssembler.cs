using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class PractitionerAssembler
    {
        public PractitionerSummary CreatePractitionerSummary(Practitioner practitioner)
        {
            PractitionerSummary summary = new PractitionerSummary();
            summary.StaffRef = practitioner.GetRef();
            summary.FamilyName = practitioner.Name.FamilyName;
            summary.GivenName = practitioner.Name.GivenName;
            summary.MiddleName = practitioner.Name.MiddleName;
            summary.Prefix = practitioner.Name.Prefix;
            summary.Suffix = practitioner.Name.Suffix;
            summary.Degree = practitioner.Name.Degree;
            summary.LicenseNumber = practitioner.LicenseNumber;

            return summary;
        }

        public PractitionerDetail CreatePractitionerDetail(Practitioner practitioner)
        {
            PractitionerDetail detail = new PractitionerDetail();
            detail.FamilyName = practitioner.Name.FamilyName;
            detail.GivenName = practitioner.Name.GivenName;
            detail.MiddleName = practitioner.Name.MiddleName;
            detail.Prefix = practitioner.Name.Prefix;
            detail.Suffix = practitioner.Name.Suffix;
            detail.Degree = practitioner.Name.Degree;
            detail.LicenseNumber = practitioner.LicenseNumber;

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in practitioner.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneNumberAssembler.CreateTelephoneDetail(phoneNumber));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in practitioner.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddressDetail(address));
            }

            return detail;
        }

        public void UpdatePractitioner(Practitioner practitioner, PractitionerDetail detail)
        {
            detail.FamilyName = practitioner.Name.FamilyName;
            detail.GivenName = practitioner.Name.GivenName;
            detail.MiddleName = practitioner.Name.MiddleName;
            detail.Prefix = practitioner.Name.Prefix;
            detail.Suffix = practitioner.Name.Suffix;
            detail.Degree = practitioner.Name.Degree;
            detail.LicenseNumber = practitioner.LicenseNumber;

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in practitioner.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneNumber, detail.TelephoneNumbers);
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in practitioner.Addresses)
            {
                telephoneNumberAssembler.AddAddress(address, detail.Addresses);
            }
        }
    }
}
