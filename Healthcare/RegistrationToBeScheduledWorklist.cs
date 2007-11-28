using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RegistrationToBeScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationToBeScheduledWorklist")]
    public partial class RegistrationToBeScheduledWorklist : ClearCanvas.Healthcare.Worklist
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
                criteria.Order.Status.In(new OrderStatus[] { OrderStatus.SC });
                criteria.Order.SchedulingRequestDateTime.Between(Platform.Time.Date.AddDays(-7), Platform.Time.Date.AddDays(14));
                criteria.RequestedProcedure.ScheduledStartTime.IsNull();
                return new RegistrationWorklistItemSearchCriteria[] { criteria };
            }
        }

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IRegistrationWorklistBroker>(context).GetWorklist(QueryConditions, this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetWorklistCount(QueryConditions, this);
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