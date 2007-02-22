using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CheckInProcedureStep entity
    /// </summary>
	public partial class CheckInProcedureStep : ProcedureStep
	{
        public CheckInProcedureStep(RequestedProcedure procedure)
            :base(procedure)
        {
        }

        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public CheckInProcedureStep()
        {
        }
        
        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public override string Name
        {
            get { return "CheckIn"; }
        }
	
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