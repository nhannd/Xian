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

	}
}