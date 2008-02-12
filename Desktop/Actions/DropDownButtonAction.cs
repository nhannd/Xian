using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Actions
{
	internal delegate ActionModelNode GetMenuModelDelegate();

	/// <summary>
	/// Models a toolbar item that, when clicked, displays a menu containing other actions.
	/// </summary>
	/// <remarks>
	/// The action is not itself an <see cref="IClickAction"/>, in that the action of
	/// clicking it is not customizable; it can only show the associated menu items
	/// (from <see cref="DropDownMenuModel"/>).
	/// </remarks>
	public class DropDownButtonAction : Action, IDropDownButtonAction
	{
		private GetMenuModelDelegate _menuModelDelegate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve resources associated with this action.</param>
		public DropDownButtonAction(string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
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