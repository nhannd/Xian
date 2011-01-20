#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
	[ExtensionOf(typeof(PatientAlertExtensionPoint))]
	public class PatientAllergyAlert : PatientAlertBase
	{
		public override string Id
		{
			get { return "PatientAllergyAlert"; }
		}

		public override AlertNotification Test(Patient patient, IPersistenceContext context)
		{
			if (patient.Allergies.Count == 0)
				return null;

			var reasons = CollectionUtils.Map<Allergy, string>(patient.Allergies, allergy => allergy.AllergenDescription);
			return new AlertNotification(this.Id, reasons);
		}
	}
}