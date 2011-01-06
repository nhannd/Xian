#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// ExternalPractitioner entity
	/// </summary>
	[Validation(HighLevelRulesProviderMethod="GetValidationRules")]
	public partial class ExternalPractitioner : ClearCanvas.Enterprise.Core.Entity
	{
		/// <summary>
		/// Returns the default contact point, or null if no default contact point exists.
		/// </summary>
		public virtual ExternalPractitionerContactPoint DefaultContactPoint
		{
			get
			{
				return CollectionUtils.SelectFirst(_contactPoints,
					delegate(ExternalPractitionerContactPoint cp) { return cp.IsDefaultContactPoint; });
			}
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Mark the entity as being edited.  The edit time is recorded and the entity is now unverified.
		/// </summary>
		public virtual void MarkEdited()
		{
			_lastEditedTime = Platform.Time;
			_isVerified = false;
		}

		/// <summary>
		/// Mark the entity as being verified.  The verify time is recorded.
		/// </summary>
		public virtual void MarkVerified()
		{
			_lastVerifiedTime = Platform.Time;
			_isVerified = true;
		}

		private static IValidationRuleSet GetValidationRules()
		{
			// ensure that not both the procedure type and procedure type groups filters are being applied
			var exactlyOneDefaultContactPointRule = new ValidationRule<ExternalPractitioner>(
				delegate(ExternalPractitioner externalPractitioner)
				{
					// The rule is not applicable to deactivated external practitioner
					if (externalPractitioner.Deactivated)
						return new TestResult(true, "");

					var activeDefaultContactPoints = CollectionUtils.Select(
						externalPractitioner.ContactPoints,
						contactPoint => contactPoint.IsDefaultContactPoint && !contactPoint.Deactivated);
					var success = activeDefaultContactPoints.Count == 1;

					return new TestResult(success, SR.MessageValidateExternalPractitionerRequiresExactlyOneDefaultContactPoint);
				});

			return new ValidationRuleSet(new[] { exactlyOneDefaultContactPointRule });
		}

	}
}