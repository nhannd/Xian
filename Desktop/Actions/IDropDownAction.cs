#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a menu containing other <see cref="IAction"/>s.
	/// </summary>
	public interface IDropDownAction : IAction
	{
		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		ActionModelNode DropDownMenuModel { get; }
	}
}
