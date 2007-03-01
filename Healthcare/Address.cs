using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Address component
    /// </summary>
	public partial class Address : IFormattable
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Equivalence comparison which ignores validity range
        /// </summary>
        /// <param name="that">The Address to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsEquivalentTo(Address that)
        {
            return (that != null) &&
                ((this._unit == default(string)) ? (that._unit == default(string)) : this._unit.Equals(that._unit, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._street == default(string)) ? (that._street == default(string)) : this._street.Equals(that._street, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._city == default(string)) ? (that._city == default(string)) : this._city.Equals(that._city, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._province == default(string)) ? (that._province == default(string)) : this._province.Equals(that._province, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._postalCode == default(string)) ? (that._postalCode == default(string)) : this._postalCode.Replace(" ", "").Equals(that._postalCode.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._country == default(string)) ? (that._country == default(string)) : this._country.Equals(that._country, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._type == default(AddressType)) ? (that._type == default(AddressType)) : this._type.Equals(that._type))  &&
                true;
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(_unit))
            {
                sb.Append(_unit);
                sb.Append("-");
            }
            sb.AppendFormat("{0}, {1} {2} {3}", _street, _city, _province, _postalCode);
            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}
