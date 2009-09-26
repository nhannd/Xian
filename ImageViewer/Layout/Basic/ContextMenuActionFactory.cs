using System;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public abstract class ContextMenuActionFactory : IContextMenuActionFactory
	{
		protected ContextMenuActionFactory()
		{
		}

		protected MenuAction CreateMenuAction(ContextMenuActionFactoryArgs args, string label, ClickHandlerDelegate clickHandler)
		{
			Platform.CheckForEmptyString(label, "label");
			Platform.CheckForNullReference(clickHandler, "clickHandler");

			MenuAction menuAction = CreateMenuAction(args);
			menuAction.Label = label;
			menuAction.SetClickHandler(clickHandler);
			return menuAction;
		}

		protected MenuAction CreateMenuAction(ContextMenuActionFactoryArgs args)
		{
			Platform.CheckForNullReference(args, "args");

			string actionId = args.GetNextActionId();
			string fullyQualifiedActionId = args.GetFullyQualifiedActionId(actionId);

			string pathString = String.Format("{0}/{1}", args.BasePath, actionId);
			ActionPath path = new ActionPath(pathString, null);
			return new MenuAction(fullyQualifiedActionId, path, ClickActionFlags.CheckParents, null);
		}

		#region IContextMenuActionFactory Members

		public abstract IAction[] CreateActions(ContextMenuActionFactoryArgs args);

		#endregion
	}
}