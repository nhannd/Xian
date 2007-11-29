using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RegistrationCompletedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationCompletedProtocolWorklist")]
    public partial class RegistrationCompletedProtocolWorklist : Worklist
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        private RegistrationWorklistItemSearchCriteria[] QueryConditions
        {
            get
            {
                RegistrationWorklistItemSearchCriteria criteria = new RegistrationWorklistItemSearchCriteria();
                criteria.ProtocolProcedureStep.State.EqualTo(ActivityStatus.CM);
                DateTime? lower = Platform.Time.AddMonths(-1);
                DateTime? upper = Platform.Time.AddDays(14);
                criteria.RequestedProcedure.ScheduledStartTime.Between(lower, upper);
                return new RegistrationWorklistItemSearchCriteria[] { criteria };
            }
        }

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklist(typeof(ProtocolAssignmentStep), QueryConditions, this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetProtocolWorklistCount(typeof(ProtocolAssignmentStep), QueryConditions, this);
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