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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// ExternalPractitioner entity
	/// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class ExternalPractitioner
	{
		/// <summary>
		/// Creates a new practitioner that is the result of merging the two specified practitioners.
		/// </summary>
		/// <param name="right"></param>
		/// <param name="left"></param>
		/// <param name="name"></param>
		/// <param name="licenseNumber"></param>
		/// <param name="billingNumber"></param>
		/// <param name="extendedProperties"></param>
		/// <param name="defaultContactPoint"></param>
		/// <param name="deactivatedContactPoints"></param>
		/// <param name="contactPointReplacements"></param>
		/// <returns></returns>
		public static ExternalPractitioner MergePractitioners(
			ExternalPractitioner right,
			ExternalPractitioner left,
			PersonName name,
			string licenseNumber,
			string billingNumber,
			IDictionary<string, string> extendedProperties,
			ExternalPractitionerContactPoint defaultContactPoint,
			ICollection<ExternalPractitionerContactPoint> deactivatedContactPoints,
			IDictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint> contactPointReplacements)
		{
			// sanity check
			if (right.Deactivated || left.Deactivated)
				throw new WorkflowException("Cannot merge a practitioner that is de-activated.");
			if (right.IsMerged || left.IsMerged)
				throw new WorkflowException("Cannot merge a practitioner that has already been merged.");

			// update properties on result record
			var result = new ExternalPractitioner { Name = name, LicenseNumber = licenseNumber, BillingNumber = billingNumber };

			ExtendedPropertyUtils.Update(result.ExtendedProperties, extendedProperties);

			// construct the set of retained contact points
			var retainedContactPoints = new HashedSet<ExternalPractitionerContactPoint>();
			retainedContactPoints.AddAll(contactPointReplacements.Values);

			// add any existing contact point that was not in the replacement list (because it is implicitly being retained)
			foreach (var contactPoint in CollectionUtils.Concat(right.ContactPoints, left.ContactPoints))
			{
				if (!contactPointReplacements.ContainsKey(contactPoint))
					retainedContactPoints.Add(contactPoint);
			}

			// for all retained contact points, create a copy attached to the result practitioner,
			// and mark the original as having been merged into the copy
			foreach (var original in retainedContactPoints)
			{
				var copy = original.CreateCopy(result);
				result.ContactPoints.Add(copy);

				copy.IsDefaultContactPoint = original.Equals(defaultContactPoint);
				copy.Deactivated = original.Deactivated || deactivatedContactPoints.Contains(original);
				original.SetMergedInto(copy);
			}

			// for all replaced contact points, mark the original as being merged into the 
			// copy of the replacement
			foreach (var kvp in contactPointReplacements)
			{
				kvp.Key.SetMergedInto(kvp.Value.MergedInto);
			}

			// mark both left and right as edited and merged
			foreach (var practitioner in new[] { right, left })
			{
				practitioner.SetMergedInto(result);
				practitioner.MarkEdited();
			}

			// mark the result as being edited
			result.MarkEdited();
			return result;
		}

		/// <summary>
		/// Gets the default contact point, or null if no default contact point exists.
		/// </summary>
		public virtual ExternalPractitionerContactPoint DefaultContactPoint
		{
			get
			{
				return CollectionUtils.SelectFirst(_contactPoints, cp => cp.IsDefaultContactPoint);
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

		/// <summary>
		/// Gets a value indicating whether this entity was merged.
		/// </summary>
		public virtual bool IsMerged
		{
			get { return _mergedInto != null; }
		}

		/// <summary>
		/// Gets the ultimate merge destination by following the merge link chain to the end.
		/// </summary>
		/// <returns></returns>
		public virtual ExternalPractitioner GetUltimateMergeDestination()
		{
			var dest = this;
			while (dest.MergedInto != null)
				dest = dest.MergedInto;

			return dest;
		}

		/// <summary>
		/// Marks this practitioner as being merged into the specified other.
		/// </summary>
		/// <param name="other"></param>
		protected internal virtual void SetMergedInto(ExternalPractitioner other)
		{
			_mergedInto = other;
			_deactivated = true;
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