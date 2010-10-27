#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Trees
{
	/// <summary>
	/// Enumeration of values representing the check state of nodes in an <see cref="ITree"/>.
	/// </summary>
	public enum CheckState
	{
		/// <summary>
		/// Indicates the unchecked state. <see cref="int">Integer</see> value of -1.
		/// </summary>
		Unchecked = -1,

		/// <summary>
		/// Indicates the unknown/indeterminate/partial check state. <see cref="int">Integer</see> value of 0.
		/// </summary>
		Partial = 0,

		/// <summary>
		/// Indicates the checked state. <see cref="int">Integer</see> value of 1.
		/// </summary>
		Checked = 1
	}
}