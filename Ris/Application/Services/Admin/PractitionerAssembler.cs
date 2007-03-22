using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class PractitionerAssembler
    {
        public PractitionerSummary CreatePractitionerSummary(Practitioner practitioner)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            return new PractitionerSummary(
                practitioner.GetRef(),
                assembler.CreatePersonNameDetail(practitioner.Name),
                practitioner.LicenseNumber);
        }

        public PractitionerDetail CreatePractitionerDetail(Practitioner practitioner, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            return new PractitionerDetail(
                assembler.CreatePersonNameDetail(practitioner.Name),
                CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(
                    practitioner.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return telephoneNumberAssembler.CreateTelephoneDetail(phone, context);
                    }),
                CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(
                    practitioner.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateTelephoneDetail(address, context);
                    }),
                practitioner.LicenseNumber);
        }

        public void UpdatePractitioner(PractitionerDetail detail, Practitioner practitioner)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            assembler.UpdatePersonName(detail.PersonNameDetail, practitioner.Name);
            practitioner.LicenseNumber = detail.LicenseNumber;

            foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
            {
                telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, practitioner.TelephoneNumbers);
            }

            foreach (AddressDetail addressDetail in detail.Addresses)
            {
                addressAssembler.AddAddress(addressDetail, practitioner.Addresses);
            }

        }
    }
}
