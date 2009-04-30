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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Attribute class used to define <see cref="DropDownAction"/>s.
	/// </summary>
	public class DropDownActionAttribute : ActionInitiatorAttribute
	{
		private readonly string _path;
		private readonly string _menuModelPropertyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The action ID.</param>
		/// <param name="path">A path indicating which toolbar the dropdown button should appear on.</param>
		/// <param name="menuModelPropertyName">The name of the property in the target class (i.e. the
		/// class to which this attribute is applied) that returns the menu model as an <see cref="ActionModelNode"/>.</param>
		public DropDownActionAttribute(string actionID, string path, string menuModelPropertyName)
			: base(actionID)
		{
			Platform.CheckForEmptyString(actionID, "actionID");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForEmptyString(menuModelPropertyName, "menuModelPropertyName");

			_path = path;
			_menuModelPropertyName = menuModelPropertyName;
		}

		/// <summary>
		/// Constructs/initializes a <see cref="DropDownAction"/> via the given <see cref="IActionBuildingContext"/>.
		/// </summary>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_path, builder.ResourceResolver);
			builder.Action = new DropDownAction(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Persistent = true;
			builder.Action.Label = path.LastSegment.LocalizedText;

			((DropDownAction)builder.Action).SetMenuModelDelegate(
				CreateGetMenuModelDelegate(builder.ActionTarget, _menuModelPropertyName));
		}

		/// <summary>
		/// Validates the property exists and has a public get method before returning them as out parameters.
		/// </summary>
		/// <exception cref="ActionBuilderException">Thrown if the property doesn't exist or does not have a public get method.</exception>
		protected internal static void GetPropertyAndGetter(object target, string propertyName, Type type, out PropertyInfo info, out MethodInfo getter)
		{
			info = target.GetType().GetProperty(propertyName, type);
			if (info == null)
			{
				throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotExist, propertyName, target.GetType().FullName));
			}

			getter = info.GetGetMethod();
			if (getter == null)
			{
				throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotHavePublicGetMethod, propertyName, target.GetType().FullName));
			}
		}

		internal static GetMenuModelDelegate CreateGetMenuModelDelegate(object target, string propertyName)
		{
			PropertyInfo info;
			MethodInfo getter;
			GetPropertyAndGetter(target, propertyName, typeof(ActionModelNode), out info, out getter);

			return (GetMenuModelDelegate)Delegate.CreateDelegate(typeof(GetMenuModelDelegate), target, getter);
		}
	}
}
