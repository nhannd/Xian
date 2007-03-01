using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Implements a simplified version of the HL7 XPN (Extended Person Name) data type
    /// </summary>
	public partial class PersonName : IFormattable
	{
        private void CustomInitialize()
        {
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("{0}, {1} {2}", _familyName, _givenName, _middleName).Trim();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}