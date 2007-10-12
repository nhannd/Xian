#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Actions;
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	public delegate ActionModelNode MenuModelDelegate();

	public class DropDownButtonActionAttribute : ActionInitiatorAttribute
	{
		private string _path;
		private string _menuModelMethod;

		/// <summary>
		/// Initializes a new instance of <see cref="DropDownButtonActionAttribute"/>.
		/// </summary>
		/// <param name="actionID">The action ID.</param>
		/// <param name="path">A path indicating which toolbar the dropdown button should
		/// appear.</param>
		/// <param name="menuModelMethod">The method in the target class (i.e. the
		/// class to which this attribute is applied) that returns the 
		/// menu model from which the dropdown menu is built. The method signature
		/// must of the form <see cref="MenuModelDelegate"/>.</param>
		public DropDownButtonActionAttribute(string actionID, string path, string menuModelMethod)
			: base(actionID)
		{
			Platform.CheckForEmptyString(actionID, "actionID");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForEmptyString(menuModelMethod, "menuModelMethod");

			_path = path;
			_menuModelMethod = menuModelMethod;
		}

		/// <summary>
		/// Override of <see cref="ActionAttribute.Apply"/>
		/// </summary>
		/// <param name="builder"></param>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_path, builder.ResourceResolver);
			builder.Action = new DropDownButtonAction(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Persistent = true;
			builder.Action.Label = path.LastSegment.LocalizedText;

			ValidateMenuModelMethod(builder.ActionTarget, _menuModelMethod);

			MenuModelDelegate menuModelDelegate =
				(MenuModelDelegate)Delegate.CreateDelegate(
					typeof(MenuModelDelegate), 
					builder.ActionTarget, 
					_menuModelMethod);

			((DropDownButtonAction)builder.Action).SetMenuModelDelegate(menuModelDelegate);

		}

		private void ValidateMenuModelMethod(object target, string methodName)
		{
			MethodInfo info = target.GetType().GetMethod(
				methodName, 
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null,
				Type.EmptyTypes,
				null);

			if (info == null)
			{
				throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, target.GetType().FullName));
			}
		}
	}
}
