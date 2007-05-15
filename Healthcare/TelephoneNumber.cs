using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
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