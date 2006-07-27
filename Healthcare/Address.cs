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
            _postalCode = source.Province;
            _validFrom = source.ValidFrom;
            _validUntil = source.ValidUntil;
        }
	}
}