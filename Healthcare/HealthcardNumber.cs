using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// HealthcardNumber component
    /// </summary>
	public partial class HealthcardNumber
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public string Format()
        {
            return string.Format("{0} {1} {2}", _assigningAuthority, _id, _versionCode).Trim();
        }
	}
}