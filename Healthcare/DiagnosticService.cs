using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// DiagnosticService entity
    /// </summary>
	public partial class DiagnosticService : Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public virtual void AddRequestedProcedureType(RequestedProcedureType rpt)
        {
            if (this.RequestedProcedureTypes.Contains(rpt))
            {
                throw new HealthcareWorkflowException(
                    string.Format("Diagnostic Service {0} already contains Requested Procedure Type {1}",
                    this.Id, rpt.Id));
            }

            this.RequestedProcedureTypes.Add(rpt);
        }
		
		
		#region Object overrides

        public override bool Equals(object that)
        {
            DiagnosticService other = that as DiagnosticService;
            return other != null && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

		#endregion
    }
}