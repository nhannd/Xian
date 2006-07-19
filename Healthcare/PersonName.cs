using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Implements a simplified version of the HL7 XPN (Extended Person Name) data type
    /// </summary>
	public partial class PersonName
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static PersonName New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new PersonName();
		}

        /// <summary>
        /// Returns a string formatting the person name for purpose of display.
        /// NB. should probably create an overload that takes a format string for more
        /// flexibility
        /// </summary>
        /// <returns></returns>
        public string Format()
        {
            return string.Format("{0}, {1} {2}", FamilyName, GivenName, MiddleName).Trim();
        }
	}
}