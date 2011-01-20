#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerBroker
	{
		#region IExternalPractitionerBroker Members

		public IList<ExternalPractitioner> GetDuplicates(ExternalPractitioner practitioner)
		{
			var criteria = GetDuplicatesCriteria(practitioner);
			return Find(criteria);
		}

		public int GetDuplicatesCount(ExternalPractitioner practitioner)
		{
			var criteria = GetDuplicatesCriteria(practitioner);
			return (int) Count(criteria);
		}

		#endregion

		private static ExternalPractitionerSearchCriteria[] GetDuplicatesCriteria(ExternalPractitioner practitioner)
		{
			var criteria = new List<ExternalPractitionerSearchCriteria>();

			var baseCriteria = new ExternalPractitionerSearchCriteria();
			baseCriteria.NotEqualTo(practitioner);
			baseCriteria.Deactivated.EqualTo(false);

			var nameCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
			nameCriteria.Name.FamilyName.EqualTo(practitioner.Name.FamilyName);
			nameCriteria.Name.GivenName.EqualTo(practitioner.Name.GivenName);
			criteria.Add(nameCriteria);

			if (!string.IsNullOrEmpty(practitioner.LicenseNumber))
			{
				var licenseNumberCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
				licenseNumberCriteria.LicenseNumber.EqualTo(practitioner.LicenseNumber);
				criteria.Add(licenseNumberCriteria);
			}

			if (!string.IsNullOrEmpty(practitioner.BillingNumber))
			{
				var billingNumberCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
				billingNumberCriteria.BillingNumber.EqualTo(practitioner.BillingNumber);
				criteria.Add(billingNumberCriteria);
			}

			return criteria.ToArray();
		}
	}
}
