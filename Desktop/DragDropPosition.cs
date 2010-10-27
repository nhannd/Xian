#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration of values indicating whether an object was drag-dropped before or after the target.
	/// </summary>
	public enum DragDropPosition
	{
		/// <summary>
		/// Indicates that the object was dragged to the drop target (neither before nor after).
		/// </summary>
		Default,

		/// <summary>
		/// Indicates that the object was dragged to a point immediately before the drop target.
		/// </summary>
		Before,

		/// <summary>
		/// Indicates that the object was dragged to a point immediately after the drop target.
		/// </summary>
		After
	}
}