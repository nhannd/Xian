#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
			return base.CanAcknowledge(acknowledger) && Equals(_recipient, acknowledger);
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