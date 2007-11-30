using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PublicationStep entity
    /// </summary>
	public partial class PublicationStep : ClearCanvas.Healthcare.ReportingProcedureStep
	{
        public PublicationStep(ReportingProcedureStep previousStep)
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
            get { return "Publication"; }
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

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            // complete the report part when publication is complete
            if (newState == ActivityStatus.CM)
                this.ReportPart.Complete();

            base.OnStateChanged(previousState, newState);
        }
    }
}