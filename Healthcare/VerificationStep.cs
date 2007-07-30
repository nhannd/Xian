using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// VerificationStep entity
    /// </summary>
	public partial class VerificationStep : ReportingProcedureStep
	{

        public VerificationStep(ReportingProcedureStep previousStep)
            :base(previousStep)
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
            get { return "Verification"; }
        }

        public override void Complete()
        {
            if (this.ReportPart != null)
                this.ReportPart.Finalized();

            base.Complete();
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