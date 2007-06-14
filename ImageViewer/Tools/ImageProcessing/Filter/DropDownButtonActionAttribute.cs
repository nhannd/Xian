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
