#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
				var count = broker.CountMergeCandidates(entity);
				return count > 0 ? new AlertNotification(this.Id, new string[] { }) : null;
			}
		}
	}
}
