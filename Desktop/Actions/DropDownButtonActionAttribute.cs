using System;
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Attribute class used to define <see cref="IDropDownButtonAction"/>s.
	/// </summary>
	public class DropDownButtonActionAttribute : ActionInitiatorAttribute
	{
		private readonly string _path;
		private readonly string _menuModelPropertyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The action ID.</param>
		/// <param name="path">A path indicating which toolbar the dropdown button should appear on.</param>
		/// <param name="menuModelPropertyName">The name of the property in the target class (i.e. the
		/// class to which this attribute is applied) that returns the menu model (as an <see cref="ActionModelNode"/>)
		/// from which the dropdown menu is built.</param>
		public DropDownButtonActionAttribute(string actionID, string path, string menuModelPropertyName)
			: base(actionID)
		{
			Platform.CheckForEmptyString(actionID, "actionID");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForEmptyString(menuModelPropertyName, "menuModelPropertyName");

			_path = path;
			_menuModelPropertyName = menuModelPropertyName;
		}

		/// <summary>
		/// Constructs/initializes an <see cref="IDropDownButtonAction"/>
		/// via the given <see cref="IActionBuildingContext"/>.
		/// </summary>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_path, builder.ResourceResolver);
			builder.Action = new DropDownButtonAction(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Persistent = true;
			builder.Action.Label = path.LastSegment.LocalizedText;

			PropertyInfo info;
			MethodInfo getter;
			GetPropertyAndGetter(builder.ActionTarget, _menuModelPropertyName, typeof(ActionModelNode), out info, out getter);

			((DropDownButtonAction)builder.Action).SetMenuModelDelegate(
				(GetMenuModelDelegate)Delegate.CreateDelegate(typeof(GetMenuModelDelegate), builder.ActionTarget, getter));
		}

		/// <summary>
		/// Validates the property exists and has a public get method before returning them as out parameters.
		/// </summary>
		/// <exception cref="ActionBuilderException">Thrown if the property doesn't exist or does not have a public get method.</exception>
		protected void GetPropertyAndGetter(object target, string propertyName, Type type, out PropertyInfo info, out MethodInfo getter)
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
	}
}
