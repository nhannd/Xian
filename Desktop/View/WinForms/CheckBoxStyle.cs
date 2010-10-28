#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Enumeration for specifying the style of check boxes displayed on a <see cref="BindingTreeView"/> control.
	/// </summary>
	public enum CheckBoxStyle
	{
		/// <summary>
		/// Indicates that no check boxes should be displayed at all.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that standard true/false check boxes should be displayed.
		/// </summary>
		Standard,

		/// <summary>
		/// Indicates that tri-state (true/false/unknown) check boxes should be displayed.
		/// </summary>
		TriState
	}
}