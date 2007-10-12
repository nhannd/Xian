#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
