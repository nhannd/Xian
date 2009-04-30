#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Actions
{
	internal delegate ActionModelNode GetMenuModelDelegate();

	/// <summary>
	/// Models a toolbar item that, when clicked, displays a menu containing other <see cref="IAction"/>s.
	/// </summary>
	/// <remarks>
	/// The <see cref="DropDownAction"/> is not itself an <see cref="IClickAction"/>, in that the action of
	/// clicking it is not customizable; it can only show the associated <see cref="DropDownMenuModel"/> items.
	/// </remarks>
	public class DropDownAction : Action, IDropDownAction
	{
		private GetMenuModelDelegate _menuModelDelegate;

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

		internal void SetMenuModelDelegate(GetMenuModelDelegate menuModelDelegate)
		{
			_menuModelDelegate = menuModelDelegate;
		}
	}
}