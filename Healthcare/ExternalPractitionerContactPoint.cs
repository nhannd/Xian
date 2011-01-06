#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ExternalPractitionerContactPoint entity
    /// </summary>
	public partial class ExternalPractitionerContactPoint : ClearCanvas.Enterprise.Core.Entity
	{

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
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}