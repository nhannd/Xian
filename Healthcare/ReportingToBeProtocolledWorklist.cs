using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare 
{
    /// <summary>
    /// ReportingToBeProtocolledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingToBeProtocolledWorklist")]
    public partial class ReportingToBeProtocolledWorklist : Worklist
    {
        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        private ReportingWorklistItemSearchCriteria[] QueryConditions
        {
            get
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.SC);
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.IsNull();
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }
        }

        #region Worklist overrides

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IReportingWorklistBroker>(context).GetWorklist(typeof(ProtocolAssignmentStep), QueryConditions, this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IReportingWorklistBroker>(context).GetWorklistCount(typeof(ProtocolAssignmentStep), QueryConditions, this);
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