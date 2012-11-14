#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that, when clicked, displays a menu containing other <see cref="IAction"/>s.
	/// </summary>
	/// <remarks>
	/// The <see cref="DropDownAction"/> is not itself an <see cref="IClickAction"/>, in that the action of
	/// clicking it is not customizable; it can only show the associated <see cref="DropDownMenuModel"/> items.
	/// </remarks>
	public class DropDownAction : Action, IDropDownAction
	{
        private Func<ActionModelNode> _menuModelDelegate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve resources associated with this action.</param>
		public DropDownAction(string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
		}

		#region IDropDownAction Members

		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get { return _menuModelDelegate(); }
		}

		#endregion

        public void SetMenuModelDelegate(Func<ActionModelNode> menuModelDelegate)
		{
			_menuModelDelegate = menuModelDelegate;
		}
	}
}