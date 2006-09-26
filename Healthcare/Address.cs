using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


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
            _validFrom = source.ValidFrom;
            _validUntil = source.ValidUntil;
        }

        /// <summary>
        /// Valid From
        /// </summary>
        public DateTime? ValidFrom
        {
            get { return _validFrom == null ? _validFrom : _validFrom.Value.Date; }
            set { _validFrom = (value == null) ? value : value.Value.Date; }
        }

        /// <summary>
        /// Valid Until
        /// </summary>
        public DateTime? ValidUntil
        {
            get { return _validUntil == null ? _validUntil : _validUntil.Value.Date; }
            set { _validUntil = (value == null) ? value : value.Value.Date; }
        }

        public string Format()
        {
            return string.Format("{0}, {1} {2} {3}", _street, _city, _province, _postalCode);
        }
	}
}