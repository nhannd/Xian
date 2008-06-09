using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// NotePosting entity
    /// </summary>
	public partial class NotePosting : ClearCanvas.Enterprise.Core.Entity
	{
		/// <summary>
		/// Gets a value indicating whether this posting can be acknowledged by the specified staff.
		/// </summary>
		/// <param name="acknowledger"></param>
		/// <returns></returns>
		protected virtual internal bool CanAcknowledge(Staff acknowledger)
		{
			return !_isAcknowledged;
		}

        /// <summary>
        /// Marks this posting as being acknowledged.
        /// </summary>
        /// <param name="acknowledgedBy"></param>
        protected virtual internal void Acknowledge(Staff acknowledgedBy)
        {
            if(_isAcknowledged)
                throw new WorkflowException("Already acknowledged.");

            _acknowledgedBy = new NoteAcknowledgement(acknowledgedBy, Platform.Time);
            _isAcknowledged = true;
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}