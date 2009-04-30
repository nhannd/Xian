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
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Attribute class used to define <see cref="DropDownButtonAction"/>s.
	/// </summary>
	public class DropDownButtonActionAttribute : ButtonActionAttribute
	{
		private readonly string _menuModelPropertyName;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        /// <param name="clickHandler">Name of the method that will be invoked when the button is clicked.</param>
		/// <param name="menuModelPropertyName">The name of the property in the target class (i.e. the
		/// class to which this attribute is applied) that returns the menu model as an <see cref="ActionModelNode"/>.</param>
		public DropDownButtonActionAttribute(string actionID, string pathHint, string clickHandler, string menuModelPropertyName)
            : base(actionID, pathHint, clickHandler)
        {
        	_menuModelPropertyName = menuModelPropertyName;
        }

		/// <summary>
		/// Constructs/initializes an <see cref="DropDownButtonAction"/> via the given <see cref="IActionBuildingContext"/>.
		/// </summary>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			base.Apply(builder);

			((DropDownButtonAction)builder.Action).SetMenuModelDelegate(
				DropDownActionAttribute.CreateGetMenuModelDelegate(builder.ActionTarget, _menuModelPropertyName));
		}

        /// <summary>
        /// Factory method to instantiate the action.
        /// </summary>
		/// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="path">The path to the action in the toolbar model.</param>
        /// <param name="flags">Flags specifying how the button should respond to being clicked.</param>
        /// <param name="resolver">The action resource resolver used to resolve the action path and icons.</param>
        /// <returns>A <see cref="ClickAction"/>.</returns>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
        {
            return new DropDownButtonAction(actionID, path, flags, resolver);
        }
	}
}
