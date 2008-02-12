
namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that, when clicked, displays a menu containing other actions.
	/// </summary>
	/// <remarks>
	/// The action is not itself an <see cref="IClickAction"/>, in that the action of
	/// clicking it is not customizable; it can only show the associated menu items
	/// (from <see cref="DropDownMenuModel"/>).
	/// </remarks>
	public interface IDropDownButtonAction : IAction
	{
		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		ActionModelNode DropDownMenuModel { get; }
	}
}
