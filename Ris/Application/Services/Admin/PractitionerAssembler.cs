using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;

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

        public PractitionerDetail CreatePractitionerDetail(Practitioner practitioner, IPersistenceContext context)
        {
            PractitionerDetail detail = new PractitionerDetail();
            detail.LicenseNumber = practitioner.LicenseNumber;

            PersonNameAssembler assembler = new PersonNameAssembler();
            detail.PersonNameDetail = assembler.CreatePersonNameDetail(practitioner.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneNumber phoneNumber in practitioner.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneNumberAssembler.CreateTelephoneDetail(phoneNumber, context));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (Address address in practitioner.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddressDetail(address, context));
            }

            return detail;
        }

        public void UpdatePractitioner(PractitionerDetail detail, Practitioner practitioner)
        {
            practitioner.LicenseNumber = detail.LicenseNumber;

            PersonNameAssembler assembler = new PersonNameAssembler();
            assembler.UpdatePersonName(detail.PersonNameDetail, practitioner.Name);

            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, practitioner.TelephoneNumbers);
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            foreach (AddressDetail addressDetail in detail.Addresses)
            {
                addressAssembler.AddAddress(addressDetail, practitioner.Addresses);
            }

        }
    }
}
