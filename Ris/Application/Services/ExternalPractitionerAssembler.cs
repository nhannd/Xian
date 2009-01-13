#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ExternalPractitionerAssembler
    {
        public ExternalPractitionerSummary CreateExternalPractitionerSummary(ExternalPractitioner prac, IPersistenceContext context)
        {
            ExternalPractitionerSummary summary = new ExternalPractitionerSummary(
                prac.GetRef(),
                new PersonNameAssembler().CreatePersonNameDetail(prac.Name),
                prac.LicenseNumber,
                prac.BillingNumber,
				prac.Deactivated);

            return summary;
        }

        public ExternalPractitionerDetail CreateExternalPractitionerDetail(ExternalPractitioner prac, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();

            ExternalPractitionerDetail detail = new ExternalPractitionerDetail(
				prac.GetRef(),
                assembler.CreatePersonNameDetail(prac.Name),
                prac.LicenseNumber,
                prac.BillingNumber,
                CollectionUtils.Map<ExternalPractitionerContactPoint, ExternalPractitionerContactPointDetail>(
                    prac.ContactPoints,
                    delegate(ExternalPractitionerContactPoint cp)
                    {
                        return CreateExternalPractitionerContactPointDetail(cp, context);
                    }),
                    new Dictionary<string, string>(prac.ExtendedProperties),
					prac.Deactivated);


            return detail;
        }

        public void UpdateExternalPractitioner(ExternalPractitionerDetail detail, ExternalPractitioner prac, IPersistenceContext context)
        {
            // validate that only one contact point is specified as default
            List<ExternalPractitionerContactPointDetail> defaultPoints = CollectionUtils.Select(detail.ContactPoints,
                delegate(ExternalPractitionerContactPointDetail cp) { return cp.IsDefaultContactPoint; });
            if(defaultPoints.Count > 1)
                throw new RequestValidationException(SR.ExceptionOneDefaultContactPoint);

            PersonNameAssembler assembler = new PersonNameAssembler();
            assembler.UpdatePersonName(detail.Name, prac.Name);

            prac.LicenseNumber = detail.LicenseNumber;
            prac.BillingNumber = detail.BillingNumber;
        	prac.Deactivated = detail.Deactivated;

            // update contact points collection
            CollectionSynchronizeHelper<ExternalPractitionerContactPoint, ExternalPractitionerContactPointDetail> syncHelper
                = new CollectionSynchronizeHelper<ExternalPractitionerContactPoint, ExternalPractitionerContactPointDetail>(
                    delegate (ExternalPractitionerContactPoint cp, ExternalPractitionerContactPointDetail cpDetail)
                    {
                        // ignore version in this comparison - deal with this issue in the update delegate
                        return cp.GetRef().Equals(cpDetail.ContactPointRef, true);
                    },
                    delegate (ExternalPractitionerContactPointDetail cpDetail, ICollection<ExternalPractitionerContactPoint> cps)
                    {
                        // create a new contact point
                        ExternalPractitionerContactPoint cp = new ExternalPractitionerContactPoint(prac);
                        UpdateExternalPractitionerContactPoint(cpDetail, cp, context);
                        cps.Add(cp);
                    },
                    delegate(ExternalPractitionerContactPoint cp, ExternalPractitionerContactPointDetail cpDetail, ICollection<ExternalPractitionerContactPoint> cps)
                    {
                        UpdateExternalPractitionerContactPoint(cpDetail, cp, context);
                    },
                    delegate(ExternalPractitionerContactPoint cp, ICollection<ExternalPractitionerContactPoint> cps)
                    {
                        cps.Remove(cp);
                    });

            syncHelper.Synchronize(prac.ContactPoints, detail.ContactPoints);


            // explicitly copy each pair, so that we don't remove any properties that the client may have removed
            foreach (KeyValuePair<string, string> pair in detail.ExtendedProperties)
            {
                prac.ExtendedProperties[pair.Key] = pair.Value;
            }
        }

        public ExternalPractitionerContactPointSummary CreateExternalPractitionerContactPointSummary(ExternalPractitionerContactPoint contactPoint)
        {
            return new ExternalPractitionerContactPointSummary(contactPoint.GetRef(),
                contactPoint.Name,
                contactPoint.Description,
                contactPoint.IsDefaultContactPoint,
				contactPoint.Deactivated);
        }

        public ExternalPractitionerContactPointDetail CreateExternalPractitionerContactPointDetail(ExternalPractitionerContactPoint contactPoint,
            IPersistenceContext context)
        {
            TelephoneNumberAssembler telephoneNumberAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();

            TelephoneNumber currentPhone = contactPoint.CurrentPhoneNumber;
            TelephoneNumber currentFax = contactPoint.CurrentFaxNumber;
            Address currentAddress = contactPoint.CurrentAddress;

            return new ExternalPractitionerContactPointDetail(
                contactPoint.GetRef(),
                contactPoint.Name,
                contactPoint.Description,
                contactPoint.IsDefaultContactPoint,
                EnumUtils.GetEnumValueInfo(contactPoint.PreferredResultCommunicationMode, context),
                CollectionUtils.Map<TelephoneNumber, TelephoneDetail>(
                    contactPoint.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return telephoneNumberAssembler.CreateTelephoneDetail(phone, context);
                    }),
                CollectionUtils.Map<Address, AddressDetail>(
                    contactPoint.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateAddressDetail(address, context);
                    }),
                    currentPhone == null ? null : telephoneNumberAssembler.CreateTelephoneDetail(currentPhone, context),
                    currentFax == null ? null : telephoneNumberAssembler.CreateTelephoneDetail(currentFax, context),
                    currentAddress == null ? null : addressAssembler.CreateAddressDetail(currentAddress, context),
					contactPoint.Deactivated
                );
       }

       public void UpdateExternalPractitionerContactPoint(ExternalPractitionerContactPointDetail detail, ExternalPractitionerContactPoint contactPoint,
           IPersistenceContext context)
       {
           contactPoint.Name = detail.Name;
           contactPoint.Description = detail.Description;
           contactPoint.IsDefaultContactPoint = detail.IsDefaultContactPoint;
           contactPoint.PreferredResultCommunicationMode = EnumUtils.GetEnumValue<ResultCommunicationMode>(detail.PreferredResultCommunicationMode);
		   contactPoint.Deactivated = detail.Deactivated;

           TelephoneNumberAssembler phoneAssembler = new TelephoneNumberAssembler();
           AddressAssembler addressAssembler = new AddressAssembler();

           contactPoint.TelephoneNumbers.Clear();
           if (detail.TelephoneNumbers != null)
           {
               foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
               {
                   contactPoint.TelephoneNumbers.Add(phoneAssembler.CreateTelephoneNumber(phoneDetail));
               }
           }

           contactPoint.Addresses.Clear();
           if (detail.Addresses != null)
           {
               foreach (AddressDetail addressDetail in detail.Addresses)
               {
                   contactPoint.Addresses.Add(addressAssembler.CreateAddress(addressDetail));
               }
           }
       }
   }
}
