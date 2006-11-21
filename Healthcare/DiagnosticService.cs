using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// DiagnosticService entity
    /// </summary>
	public partial class DiagnosticService : Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
		
		#region Object overrides

        public override bool Equals(object that)
        {
            DiagnosticService other = that as DiagnosticService;
            return other != null && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", _name, _id);
        }
		
		#endregion

	}
}