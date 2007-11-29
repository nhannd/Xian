namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ProtocolResolutionStep entity
    /// </summary>
    public partial class ProtocolResolutionStep : ClearCanvas.Healthcare.ProtocolProcedureStep
    {
        public ProtocolResolutionStep(Protocol protocol) : base(protocol)
        {
        }

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
            // TODO: implement a test for business-key equality
            return base.Equals(that);
        }

        public override int GetHashCode()
        {
            // TODO: implement a hash-code based on the business-key used in the Equals() method
            return base.GetHashCode();
        }

        #endregion

        public bool ShouldCancel
        {
            get { return this.Protocol.Status == ProtocolStatus.RJ; }
        }

        public bool ShouldResubmit
        {
            get { return this.Protocol.Status == ProtocolStatus.SU; }
        }
	}
}