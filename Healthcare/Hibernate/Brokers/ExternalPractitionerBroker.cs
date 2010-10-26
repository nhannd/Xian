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

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerBroker
	{
		#region IExternalPractitionerBroker Members

		public IList<ExternalPractitioner> GetMergeCandidates(ExternalPractitioner practitioner)
		{
			var criteria = GetMergeCandidatesCriteria(practitioner);
			return Find(criteria);
		}

		public int CountMergeCandidates(ExternalPractitioner practitioner)
		{
			var criteria = GetMergeCandidatesCriteria(practitioner);
			return (int) Count(criteria);
		}

		#endregion

		private static ExternalPractitionerSearchCriteria[] GetMergeCandidatesCriteria(ExternalPractitioner practitioner)
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
