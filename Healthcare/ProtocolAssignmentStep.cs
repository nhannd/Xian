using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ProtocolAssignmentStep entity
    /// </summary>
    public partial class ProtocolAssignmentStep : ProtocolProcedureStep
    {
        public ProtocolAssignmentStep(Protocol protocol) : base(protocol)
        {
        }

        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        public virtual bool CanAccept
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanReject
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSuspend
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSave
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public bool CanEdit(Staff staff)
        {
            return this.State == ActivityStatus.IP && this.PerformingStaff == staff;
        }

        #region ProcedureStep overrides

        protected override bool IsPreStep
        {
            // Do not start requested procedure
            get { return true; }
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