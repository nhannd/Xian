using System;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public abstract class ContextMenuActionFactory : IContextMenuActionFactory
	{
		protected ContextMenuActionFactory()
		{
		}

		protected MenuAction CreateMenuAction(ContextMenuActionFactoryArgs args)
		{
			return CreateAction(args);
		}

		internal static MenuAction CreateAction(ContextMenuActionFactoryArgs args)
		{
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