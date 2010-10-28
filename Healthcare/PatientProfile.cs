#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientProfile entity
    /// </summary>
    [UniqueKey("Mrn", new string[] { "Mrn.Id", "Mrn.AssigningAuthority" })]
	public partial class PatientProfile : Entity
	{
        private void CustomInitialize()
        {
        }

        public virtual DateTime? DateOfBirth 
        {
            get { return _dateOfBirth == null ? _dateOfBirth : _dateOfBirth.Value.Date; }
			set { _dateOfBirth = value == null ? value : value.Value.Date; } 
        }

        public virtual Address CurrentHomeAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                    delegate(Address address) { return address.Type == AddressType.R && address.IsCurrent; });
            }
        }

        public virtual Address CurrentWorkAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                   delegate(Address address) { return address.Type == AddressType.B && address.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentHomePhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.PRN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentWorkPhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                    delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual void SetDeceased(DateTime timeOfDeath)
        {
            this.DeathIndicator = true;
            this.TimeOfDeath = timeOfDeath;
        }
    }
}