#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alerts
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class IncompleteDemographicDataAlert : PatientProfileAlertBase
    {
        public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
			List<string> reasons = new List<string>();

        	TestName(profile.Name, ref reasons);
        	TestHealthcard(profile.Healthcard, ref reasons);
			TestAddresses(profile.Addresses, ref reasons);
			TestTelephoneNumbers(profile.TelephoneNumbers, ref reasons);

            if (reasons.Count > 0)
                return new AlertNotification(this.GetType(), reasons);

            return null;
        }

		private static void TestName(PersonName name, ref List<string> reasons)
		{
			if (name == null)
			{
				reasons.Add(SR.AlertFamilyNameMissing);
				return;
			}

			if (string.IsNullOrEmpty(name.FamilyName))
				reasons.Add(SR.AlertFamilyNameMissing);

			if (string.IsNullOrEmpty(name.GivenName))
				reasons.Add(SR.AlertGivenNameMissing);
		}

		private static void TestHealthcard(HealthcardNumber healthcard, ref List<string> reasons)
		{
			if (healthcard == null)
			{
				reasons.Add(SR.AlertHealthcardMissing);
				return;
			}

			if (string.IsNullOrEmpty(healthcard.Id))
				reasons.Add(SR.AlertHealthcardIdMissing);

			if (healthcard.AssigningAuthority == null)
				reasons.Add(SR.AlertHealthcardAssigningAuthorityMissing);
		}

		private static void TestAddresses(ICollection<Address> addresses, ref List<string> reasons)
		{
			if (addresses == null || addresses.Count == 0)
			{
				reasons.Add(SR.AlertResidentialAddressMissing);
				return;
			}

			bool hasResidentialAddress = CollectionUtils.Contains(addresses,
				delegate(Address a) { return a.Type == AddressType.R; });

			if (!hasResidentialAddress)
				reasons.Add(SR.AlertResidentialAddressMissing);
		}

		private static void TestTelephoneNumbers(ICollection<TelephoneNumber> phoneNumbers, ref List<string> reasons)
		{
			if (phoneNumbers == null || phoneNumbers.Count == 0)
			{
				reasons.Add(SR.AlertResidentialTelephoneNumberMissing);
				return;
			}

			bool hasResidentialPhoneNumber = CollectionUtils.Contains(phoneNumbers,
				delegate(TelephoneNumber tn) { return tn.Equipment == TelephoneEquipment.PH && tn.Use == TelephoneUse.PRN; });

			if (!hasResidentialPhoneNumber)
				reasons.Add(SR.AlertResidentialTelephoneNumberMissing);
		}
	}
}
