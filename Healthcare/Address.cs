using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Address component
    /// </summary>
	public partial class Address
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        public string Format()
        {
            string address = "";
            address += (_unit != null && !_unit.Trim().Equals("")) ? string.Format("{0}-", _unit) : "";
            address += string.Format("{0}, {1} {2} {3}", _street, _city, _province, _postalCode);
            return address;
        }

        /// <summary>
        /// Equivalence comparison which ignores validity range
        /// </summary>
        /// <param name="that">The Address to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsEquivalentTo(Address that)
        {
            return (that != null) &&
                ((this._unit == default(string)) ? (that._unit == default(string)) : this._unit.Equals(that._unit)) &&
                ((this._street == default(string)) ? (that._street == default(string)) : this._street.Equals(that._street)) &&
                ((this._city == default(string)) ? (that._city == default(string)) : this._city.Equals(that._city)) &&
                ((this._province == default(string)) ? (that._province == default(string)) : this._province.Equals(that._province)) &&
                ((this._postalCode == default(string)) ? (that._postalCode == default(string)) : this._postalCode.Equals(that._postalCode)) &&
                ((this._country == default(string)) ? (that._country == default(string)) : this._country.Equals(that._country)) &&
                ((this._type == default(AddressType)) ? (that._type == default(AddressType)) : this._type.Equals(that._type))  &&
                true;
        }
	}
}
