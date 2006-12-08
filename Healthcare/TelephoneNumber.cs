using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// TelephoneNumber component
    /// </summary>
	public partial class TelephoneNumber
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }    
        }

        /// <summary>
        /// Default Telephone Number string, suppresses the country code for numbers within the installed site
        /// </summary>
        /// <returns>A string representation of the telephone number</returns>
        public string Format()
        {
            /// todo: replace hard-coded "1" with the site's default country code
            return this.Format("1");
        }

        /// <summary>
        /// Telephone number string, which suppresses country code output for the specified country code
        /// </summary>
        /// <param name="filteredCountryCode">Country code to suppress.  Null will display all country codes.</param>
        /// <returns>A string representation of the telephone number</returns>
        public string Format(string filteredCountryCode)
        {
            string number = "";

            number += (_countryCode != null && !_countryCode.Trim().Equals("") && _countryCode != filteredCountryCode) 
                ? string.Format("+{0} ", _countryCode) : "";
            number += string.Format("({0}) {1}-{2}", _areaCode, _number.Substring(0,3), _number.Substring(3));
            number += (_extension != null && !_extension.Trim().Equals(""))
                ? string.Format(" x{0}", _extension) : "";

            return number.ToString();
        }

        /// <summary>
        /// Equivalence comparison which ignores ValidRange
        /// </summary>
        /// <param name="that">The TelephoneNumber to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsEquivalentTo(TelephoneNumber that)
        {
            return (that != null) &&
                ((this._countryCode == default(string)) ? (that._countryCode == default(string)) : this._countryCode.Equals(that._countryCode)) &&
                ((this._areaCode == default(string)) ? (that._areaCode == default(string)) : this._areaCode.Equals(that._areaCode)) &&
                ((this._number == default(string)) ? (that._number == default(string)) : this._number.Equals(that._number)) &&
                ((this._extension == default(string)) ? (that._extension == default(string)) : this._extension.Equals(that._extension)) &&
                ((this._use == default(TelephoneUse)) ? (that._use == default(TelephoneUse)) : this._use.Equals(that._use)) &&
                ((this._equipment == default(TelephoneEquipment)) ? (that._equipment == default(TelephoneEquipment)) : this._equipment.Equals(that._equipment)) &&
                true;

        }

	}
}