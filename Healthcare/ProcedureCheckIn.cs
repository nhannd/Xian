using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ProcedureCheckIn entity
    /// </summary>
	public partial class ProcedureCheckIn : ClearCanvas.Enterprise.Core.Entity
	{
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        #region Public Operations

        /// <summary>
        /// Check in the procedure
        /// </summary>
        public virtual void CheckIn()
        {
            if (_checkInTime == null)
                _checkInTime = Platform.Time;
        }

        /// <summary>
        /// Check out the procedure
        /// </summary>
        public virtual void CheckOut()
        {
            if (_checkOutTime == null)
                _checkOutTime = Platform.Time;
        }

        public virtual bool IsNotCheckIn
        {
            get { return _checkInTime == null; }
        }

        public virtual bool IsCheckIn
        {
            get { return _checkInTime != null && _checkOutTime == null; }
        }

        public virtual bool IsCheckOut
        {
            get { return _checkOutTime != null; }
        }

        #endregion

        #region Object overrides

        public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

	}
}