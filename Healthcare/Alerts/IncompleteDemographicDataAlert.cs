#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		public override string Id
		{
			get { return "IncompleteDemographicDataAlert"; }
		}

		public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
			var reasons = new List<string>();

        	TestName(profile.Name, ref reasons);
			TestAddresses(profile.Addresses, ref reasons);
			TestTelephoneNumbers(profile.TelephoneNumbers, ref reasons);

			var settings = new AlertsSettings();
			if(settings.MissingHealthcardInfoTriggersIncompleteDemographicDataAlert)
			{
				TestHealthcard(profile.Healthcard, ref reasons);
			}

            if (reasons.Count > 0)
                return new AlertNotification(this.Id, reasons);

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
