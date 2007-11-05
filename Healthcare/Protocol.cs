namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Protocol entity
    /// </summary>
    public partial class Protocol : ClearCanvas.Enterprise.Core.Entity
    {
        public Protocol(RequestedProcedure procedure)
        {
            _procedure = procedure;
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

	}
}