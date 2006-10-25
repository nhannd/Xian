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
		/// <summary>
		/// Factory method
		/// </summary>
		public static TelephoneNumber New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new TelephoneNumber();
		}

        public void CopyFrom(TelephoneNumber source)
        {
            _use = source.Use;
            _equipment = source.Equipment;
            _countryCode = source.CountryCode;
            _areaCode = source.AreaCode;
            _number = source.Number;
            _extension = source.Extension;
            _validRange = (source._validRange == null) ? null : new DateTimeRange(source._validRange.From, source._validRange.Until);
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }    
        }

        public string Format()
        {
            return _extension == null ?
                string.Format("{0} ({1}) {2}", _countryCode, _areaCode, _number) :
                string.Format("{0} ({1}) {2} x{3}", _countryCode, _areaCode, _number, _extension);
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