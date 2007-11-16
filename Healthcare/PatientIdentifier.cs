using System;
using System.Collections;
using System.Text;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientIdentifier component
    /// </summary>
	public partial class PatientIdentifier : IFormattable
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            return string.Format("{0} {1}", _assigningAuthority.Code, _id);
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}