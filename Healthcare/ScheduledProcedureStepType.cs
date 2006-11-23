using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ScheduledProcedureStepType entity
    /// </summary>
	public partial class ScheduledProcedureStepType : Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public virtual string Format()
        {
            return string.Format("{0} ({1})", _name, _id);
        }
        
        #region Object overrides

        public override bool Equals(object that)
        {
            ScheduledProcedureStepType other = that as ScheduledProcedureStepType;
            return other != null && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion

	}
}