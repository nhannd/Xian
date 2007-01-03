using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RequestedProcedureType entity
    /// </summary>
	public partial class RequestedProcedureType : Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public virtual void AddScheduledProcedureStepType(ScheduledProcedureStepType spt)
        {
            if (this.ScheduledProcedureStepTypes.Contains(spt))
            {
                throw new HealthcareWorkflowException(
                    string.Format("Requested Procedure Type {0} already contains Scheduled Procedure Step Type {1}",
                    this.Id, spt.Id));
            }

            this.ScheduledProcedureStepTypes.Add(spt);
        }

        public virtual string Format()
        {
            return string.Format("{0} ({1})", _name, _id);
        }
		
		#region Object overrides
		
		public override bool Equals(object that)
		{
            RequestedProcedureType other = that as RequestedProcedureType;
            return other != null && other.Id == this.Id;
		}
		
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}
		
		#endregion

	}
}