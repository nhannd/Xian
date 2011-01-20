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
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Alerts
{
	public abstract class ExternalPractitionerAlerts
	{
		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class IncompleteDataAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "IncompleteDataAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var reasons = new List<string>();

				TestName(entity.Name, ref reasons);

				if (string.IsNullOrEmpty(entity.LicenseNumber))
					reasons.Add(SR.AlertExternalPractitionerLicenseNumberMissing);

				if (string.IsNullOrEmpty(entity.BillingNumber))
					reasons.Add(SR.AlertExternalPractitionerBillingNumberMissing);

				return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
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
		}

		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class IncompleteContactPointDataAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "IncompleteContactPointDataAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var reasons = new List<string>();

				if (entity.ContactPoints == null || entity.ContactPoints.Count == 0)
				{
					reasons.Add(SR.AlertContactPointsMissing);
				}
				else
				{
					TestContactPoints(entity.ContactPoints, ref reasons);
				}

				return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
			}

			private static void TestContactPoints(IEnumerable<ExternalPractitionerContactPoint> contactPoints, ref List<string> reasons)
			{
				var hasMissingWorkAddress = false;
				var hasMissingWorkTelephone = false;
				var hasMissingWorkFax = false;

				CollectionUtils.ForEach(contactPoints,
					delegate(ExternalPractitionerContactPoint cp)
						{
							// Test for at least one contact point that does not have work address
							if (hasMissingWorkAddress == false && !TestWorkAddress(cp.Addresses))
								hasMissingWorkAddress = true;

							// Test for at least one contact point that does not have work telephone
							if (hasMissingWorkTelephone == false && !TestWorkTelephone(cp.TelephoneNumbers))
								hasMissingWorkTelephone = true;

							// Test for at least one contact point that does not have work fax
							if (hasMissingWorkFax == false && !TestWorkFax(cp.TelephoneNumbers))
								hasMissingWorkFax = true;
						});

				if (hasMissingWorkAddress)
					reasons.Add(SR.AlertWorkAddressMissing);

				if (hasMissingWorkTelephone)
					reasons.Add(SR.AlertWorkTelephoneNumberMissing);

				if (hasMissingWorkFax)
					reasons.Add(SR.AlertWorkFaxNumberMissing);
			}

			private static bool TestWorkAddress(ICollection<Address> addresses)
			{
				if (addresses == null || addresses.Count == 0)
					return false;

				return CollectionUtils.Contains(addresses, a => a.Type == AddressType.B);
			}

			private static bool TestWorkTelephone(ICollection<TelephoneNumber> phoneNumbers)
			{
				if (phoneNumbers == null || phoneNumbers.Count == 0)
					return false;

				return CollectionUtils.Contains(phoneNumbers, tn => tn.Equipment == TelephoneEquipment.PH && tn.Use == TelephoneUse.WPN);
			}

			private static bool TestWorkFax(ICollection<TelephoneNumber> phoneNumbers)
			{
				if (phoneNumbers == null || phoneNumbers.Count == 0)
					return false;

				return CollectionUtils.Contains(phoneNumbers, tn => tn.Equipment == TelephoneEquipment.FX && tn.Use == TelephoneUse.WPN);
			}
		}

		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class PossibleDuplicateAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "PossibleDuplicateAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var broker = context.GetBroker<IExternalPractitionerBroker>();
				var count = broker.GetDuplicatesCount(entity);
				return count > 0 ? new AlertNotification(this.Id, new string[] { }) : null;
			}
		}
	}
}
