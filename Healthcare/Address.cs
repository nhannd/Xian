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

		/// <summary>
		/// Factory method
		/// </summary>
		public static Address New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new Address();
		}

        public void CopyFrom(Address source)
        {
            _type = source.Type;
            _street = source.Street;
            _city = source.City;
            _province = source.Province;
            _country = source.Country;
            _postalCode = source.PostalCode;
            _validRange = (source._validRange == null) ? null : new DateTimeRange(source._validRange.From, source._validRange.Until);
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        public string Format()
        {
            return string.Format("{0}, {1} {2} {3}", _street, _city, _province, _postalCode);
        }
	}
}