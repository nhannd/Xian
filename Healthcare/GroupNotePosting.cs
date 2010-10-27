#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// GroupNotePosting entity
    /// </summary>
	public partial class GroupNotePosting
    {
		/// <summary>
		/// Gets a value indicating whether this posting can be acknowledged by the specified staff.
		/// </summary>
		/// <param name="acknowledger"></param>
		/// <returns></returns>
		protected internal override bool CanAcknowledge(Staff acknowledger)
		{
			return base.CanAcknowledge(acknowledger)
				&& _recipient.Members.Contains(acknowledger)
				&& !Equals(acknowledger, this.Note.Author); // #5528: user cannot ack a note on behalf of a group, if they authored the note
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