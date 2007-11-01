using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare 
{

    /// <summary>
    /// TechnologistUndocumentedWorklist entity
    /// </summary>
    public partial class TechnologistUndocumentedWorklist : Worklist
    {
        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        #region Worklist overrides

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IModalityWorklistBroker>(context).GetUndocumentedWorklist(this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IModalityWorklistBroker>(context).GetUndocumentedWorklistCount(this);
        }

        public override string NameSuffix
        {
            get { return " - Undocumented"; }
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