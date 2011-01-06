#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Enumeration of types of drag-drop operations.
    /// </summary>
	public enum DragDropKind
    {
		/// <summary>
		/// No-op.
		/// </summary>
        None,

		/// <summary>
		/// The drag-drop operation is a move operation.
		/// </summary>
        Move,
		
		/// <summary>
		/// The drag-drop operation is a copy operation.
		/// </summary>
		Copy
    }
}
