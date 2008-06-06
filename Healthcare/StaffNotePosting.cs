using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// StaffNotePosting entity
    /// </summary>
	public partial class StaffNotePosting : ClearCanvas.Healthcare.NotePosting
	{
    	/// <summary>
    	/// Gets a value indicating whether this posting can be acknowledged by the specified staff.
    	/// </summary>
    	/// <param name="acknowledger"></param>
    	/// <returns></returns>
    	protected internal override bool CanAcknowledge(Staff acknowledger)
		{
			return !this.IsAcknowledged && Equals(_recipient, acknowledger);
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