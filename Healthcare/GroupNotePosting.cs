using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// GroupNotePosting entity
    /// </summary>
	public partial class GroupNotePosting : ClearCanvas.Healthcare.NotePosting
	{
		/// <summary>
		/// Gets a value indicating whether this posting can be acknowledged by the specified staff.
		/// </summary>
		/// <param name="acknowledger"></param>
		/// <returns></returns>
		protected internal override bool CanAcknowledge(Staff acknowledger)
		{
			return base.CanAcknowledge(acknowledger) && _recipient.Members.Contains(acknowledger);
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