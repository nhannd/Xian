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
            summary.LicenseNumber = practitioner.LicenseNumber;

            PersonNameAssembler assembler = new PersonNameAssembler();
            summary.PersonNameDetail = assembler.CreatePersonNameDetail(practitioner.Name);

            return summary;
        }

        public PractitionerDetail CreatePractitionerDetail(Practitioner practitioner)
        {
            PractitionerDetail detail = new PractitionerDetail();
            detail.LicenseNumber = practitioner.LicenseNumber;

            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(practitioner.Name);

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
            detail.LicenseNumber = practitioner.LicenseNumber;

            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(practitioner.Name);

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
