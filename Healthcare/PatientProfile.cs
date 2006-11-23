using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientProfile entity
    /// </summary>
	public partial class PatientProfile : Entity
	{
        private void CustomInitialize()
        {
        }

        /// <summary>
        /// Test for equality based on the MRN
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PatientProfile that = obj as PatientProfile;
            if (that == null)
                return false;

            return this.Mrn == null ? that.Mrn == null : this.Mrn.Equals(that.Mrn);
        }

        /// <summary>
        /// Gets a hash code based on the MRN
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Mrn == null ? 0 : this.Mrn.GetHashCode();
        }

        public virtual DateTime DateOfBirth
        {
            get { return _dateOfBirth.Date; }
            set { _dateOfBirth = value.Date; }
        }

        public virtual Address CurrentHomeAddress
        {
            get
            {
                return CollectionUtils.SelectFirst<Address>(this.Addresses,
                    delegate(Address address) { return address.Type == AddressType.R && address.IsCurrent; });
            }
        }

        public virtual Address CurrentWorkAddress
        {
            get
            {
                return CollectionUtils.SelectFirst<Address>(this.Addresses,
                   delegate(Address address) { return address.Type == AddressType.B && address.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentHomePhone
        {
            get
            {
                return CollectionUtils.SelectFirst<TelephoneNumber>(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.PRN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentWorkPhone
        {
            get
            {
                return CollectionUtils.SelectFirst<TelephoneNumber>(this.TelephoneNumbers,
                    delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }
    }
}