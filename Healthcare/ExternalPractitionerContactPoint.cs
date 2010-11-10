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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ExternalPractitionerContactPoint entity
    /// </summary>
	public partial class ExternalPractitionerContactPoint : ClearCanvas.Enterprise.Core.Entity
	{
		private delegate TOutput Converter<TInput1, TInput2, TOutput>(TInput1 input1, TInput2 input2);

		/// <summary>
		/// Creates a new contact point that is the result of merging the two specified contact points.
		/// </summary>
		/// <param name="right"></param>
		/// <param name="left"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="preferredCommunicationMode"></param>
		/// <returns></returns>
		public static ExternalPractitionerContactPoint MergeContactPoints(
			ExternalPractitionerContactPoint right,
			ExternalPractitionerContactPoint left,
			string name,
			string description,
			ResultCommunicationMode preferredCommunicationMode
			)
		{
			// sanity checks
			if (!Equals(right.Practitioner, left.Practitioner))
				throw new WorkflowException("Only contact points belonging to the same practitioner can be merged.");
			if (right.Deactivated || left.Deactivated)
				throw new WorkflowException("Cannot merge a contact point that is de-activated.");
			if (right.IsMerged || left.IsMerged)
				throw new WorkflowException("Cannot merge a contact point that has already been merged.");

			var practitioner = right.Practitioner; // same practitioner for both

			// create new contact point, using specified name and description
			// the new contact point is default if either of the source contact points were the default
			// start with empty value collections
			var result = new ExternalPractitionerContactPoint(
				practitioner,
				name,
				description,
				preferredCommunicationMode,
				right.IsDefaultContactPoint || left.IsDefaultContactPoint,
				new List<TelephoneNumber>(), 
				new List<Address>(), 
				new List<EmailAddress>(), 
				null);

			practitioner.ContactPoints.Add(result);

			// merge value collections from source contact points, and update source contact points appropriately
			foreach (var contactPoint in new[] { right, left })
			{
				// merge all value collections into the result
				MergeValueCollection(result.TelephoneNumbers, contactPoint.TelephoneNumbers, (x, y) => x.IsSameNumber(y));
				MergeValueCollection(result.Addresses, contactPoint.Addresses, (x, y) => x.IsSameAddress(y));
				MergeValueCollection(result.EmailAddresses, contactPoint.EmailAddresses, (x, y) => x.IsSameEmailAddress(y));

				// mark as merged
				contactPoint.SetMergedInto(result);
			}

			// mark the practitioner as being edited
			practitioner.MarkEdited();

			return result;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="practitioner"></param>
        public ExternalPractitionerContactPoint(ExternalPractitioner practitioner)
        {
            _practitioner = practitioner;
            _practitioner.ContactPoints.Add(this);

            _telephoneNumbers = new List<ClearCanvas.Healthcare.TelephoneNumber>();
            _addresses = new List<ClearCanvas.Healthcare.Address>();
            _emailAddresses = new List<ClearCanvas.Healthcare.EmailAddress>();
        }

        public virtual Address CurrentAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                    delegate(Address address) { return address.Type == AddressType.B && address.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentFaxNumber
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.FX && phone.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentPhoneNumber
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

		public virtual EmailAddress CurrentEmailAddress
		{
			get
			{
				return CollectionUtils.SelectFirst(this.EmailAddresses,
				  delegate(EmailAddress emailAddress) { return emailAddress.IsCurrent; });
			}
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
		public virtual ExternalPractitionerContactPoint GetUltimateMergeDestination()
		{
			var dest = this;
			while (dest.MergedInto != null)
				dest = dest.MergedInto;

			return dest;
		}

		/// <summary>
		/// Creates a copy of this contact point, assigning it to the specified owner.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public virtual ExternalPractitionerContactPoint CreateCopy(ExternalPractitioner owner)
		{
			var copy = new ExternalPractitionerContactPoint(
				owner,
				_name,
				_description,
				_preferredResultCommunicationMode,
				_isDefaultContactPoint,
				CollectionUtils.Map(_telephoneNumbers, (TelephoneNumber tn) => (TelephoneNumber)tn.Clone()),
				CollectionUtils.Map(_addresses, (Address a) => (Address)a.Clone()),
				CollectionUtils.Map(_emailAddresses, (EmailAddress e) => (EmailAddress)e.Clone()),
				null) {Deactivated = _deactivated};

			owner.ContactPoints.Add(copy);
			return copy;
		}

		/// <summary>
		/// Marks this contact point as being merged into the specified other.
		/// </summary>
		/// <param name="other"></param>
		protected internal virtual void SetMergedInto(ExternalPractitionerContactPoint other)
		{
			_mergedInto = other;
			_deactivated = true;
			_isDefaultContactPoint = false;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Copies items in the source collection into the dest collection, if the item does not already exist in the dest collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="isSameFunction"></param>
		private static void MergeValueCollection<T>(ICollection<T> dest, IEnumerable<T> src, Converter<T, T, bool> isSameFunction)
			where T : ICloneable
		{
			foreach (var srcItem in src)
			{
				if (CollectionUtils.Contains(dest, destItem => isSameFunction(destItem, srcItem)))
					continue;

				var copy = (T)srcItem.Clone();
				dest.Add(copy);
			}
		}

	}
}