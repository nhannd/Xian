using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// NoteReadActivity entity
    /// </summary>
	public partial class NoteReadActivity : ClearCanvas.Enterprise.Core.Entity
	{

        /// <summary>
        /// Marks this read-activity as being acknowledged.
        /// </summary>
        /// <param name="acknowledgedBy"></param>
        public virtual void Acknowledge(Staff acknowledgedBy)
        {
            if(_isAcknowledged)
                throw new WorkflowException("Already acknowledged.");

            _acknowledgedBy = new NoteReader(acknowledgedBy, Platform.Time);
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