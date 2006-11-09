using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


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

        public Address CurrentHomeAddress
        {
            get
            {
                foreach (Address address in this.Addresses)
                {
                    if (address.Type == AddressType.R && address.IsCurrent)
                    {
                        return address;
                    }
                }
                return null;
            }
        }

        public Address CurrentWorkAddress
        {
            get
            {
                foreach (Address address in this.Addresses)
                {
                    if (address.Type == AddressType.B && address.IsCurrent)
                    {
                        return address;
                    }
                }
                return null;
            }
        }

        public TelephoneNumber CurrentHomePhone
        {
            get
            {
                foreach (TelephoneNumber phone in this.TelephoneNumbers)
                {
                    if (phone.Use == TelephoneUse.PRN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent)
                    {
                        return phone;
                    }
                }
                return null;
            }
        }

        public TelephoneNumber CurrentWorkPhone
        {
            get
            {
                foreach (TelephoneNumber phone in this.TelephoneNumbers)
                {
                    if (phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent)
                    {
                        return phone;
                    }
                }
                return null;
            }
        }
    }
}