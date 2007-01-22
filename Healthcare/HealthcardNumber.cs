using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// HealthcardNumber component
    /// </summary>
	public partial class HealthcardNumber : IFormattable
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Equivalence comparison which ignores validity range
        /// </summary>
        /// <param name="that">The HealthcardNumber to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsEquivalentTo(HealthcardNumber that)
        {
            return (that != null) &&

            ((this._id == default(string)) ? (that._id == default(string)) : this._id.Equals(that._id, StringComparison.CurrentCultureIgnoreCase)) &&

            ((this._assigningAuthority == default(string)) ? (that._assigningAuthority == default(string)) : this._assigningAuthority.Equals(that._assigningAuthority, StringComparison.CurrentCultureIgnoreCase)) &&

            ((this._versionCode == default(string)) ? (that._versionCode == default(string)) : this._versionCode.Equals(that._versionCode, StringComparison.CurrentCultureIgnoreCase)) &&

            ((this._expiryDate == default(DateTime?)) ? (that._expiryDate == default(DateTime?)) : this._expiryDate.Value.Date.Equals(that._expiryDate.Value.Date)) &&

                true;
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            return string.Format("{0} {1} {2}", this.AssigningAuthority, this.Id, this.VersionCode);
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}