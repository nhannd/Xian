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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that has both a drop-down and a button, where each can behave independently.
	/// </summary>
	public class DropDownButtonAction : ButtonAction, IDropDownAction
	{
		private GetMenuModelDelegate _menuModelDelegate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The fully qualified action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="flags">Flags that control the style of the action.</param>
		/// <param name="resolver">A resource resolver that will be used to resolve text and image resources.</param>
		public DropDownButtonAction(string actionId, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
			: base(actionId, path, flags, resolver)
		{
		}

		#region IDropDownButtonAction Members

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
