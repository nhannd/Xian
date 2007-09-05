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
    public class ExternalPractitionerAssembler
    {
        public ExternalPractitionerSummary CreateExternalPractitionerSummary(ExternalPractitioner prac, IPersistenceContext context)
        {
            ExternalPractitionerSummary summary = new ExternalPractitionerSummary();
            summary.PractitionerRef = prac.GetRef();
            summary.Name = new PersonNameAssembler().CreatePersonNameDetail(prac.Name);
            if (prac.LicenseNumber != null)
            {
                summary.LicenseNumber = new CompositeIdentifierDetail(prac.LicenseNumber.Id, prac.LicenseNumber.AssigningAuthority);
            }
            return summary;
        }

        public ExternalPractitionerDetail CreateExternalPractitionerDetail(ExternalPractitioner prac, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            ExternalPractitionerDetail detail = new ExternalPractitionerDetail();
            detail.Name = assembler.CreatePersonNameDetail(prac.Name);
            detail.TelephoneNumbers = CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(
                    prac.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return telephoneNumberAssembler.CreateTelephoneDetail(phone, context);
                    });
            detail.Addresses = CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(
                    prac.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateAddressDetail(address, context);
                    });

            if (prac.LicenseNumber != null)
            {
                detail.LicenseNumber = new CompositeIdentifierDetail(prac.LicenseNumber.Id, prac.LicenseNumber.AssigningAuthority);
            }

            return detail;
        }

        public void UpdateExternalPractitioner(ExternalPractitionerDetail detail, ExternalPractitioner prac)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            assembler.UpdatePersonName(detail.Name, prac.Name);

            if (detail.TelephoneNumbers != null)
            {
                foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
                {
                    telephoneNumberAssembler.AddTelephoneNumber(phoneDetail, prac.TelephoneNumbers);
                }
            }

            if (detail.Addresses != null)
            {
                foreach (AddressDetail addressDetail in detail.Addresses)
                {
                    addressAssembler.AddAddress(addressDetail, prac.Addresses);
                }
            }

            prac.LicenseNumber = new CompositeIdentifier(detail.LicenseNumber.Id, detail.LicenseNumber.AssigningAuthority);
        }
    }
}
