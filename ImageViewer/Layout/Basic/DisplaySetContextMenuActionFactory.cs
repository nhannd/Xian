using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class DisplaySetContextMenuActionFactory : IContextMenuActionFactory
	{
		public DisplaySetContextMenuActionFactory()
		{}

		private void AssignDisplaySetToImageBox(IImageViewer viewer, IDisplaySet displaySet)
		{
			IImageBox imageBox = viewer.SelectedImageBox;
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			// always create a 'fresh copy' to show in the image box.  We never want to show
			// the 'originals' (e.g. the ones in IImageSet.DisplaySets) because we want them 
			// to remain clean and unaltered - consider them to be templates for what actually
			// gets shown.
			imageBox.DisplaySet = displaySet.CreateFreshCopy();

			imageBox.Draw();
			//this.ImageViewer.SelectedImageBox[0, 0].Select();

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Enqueue(memorableCommand);
			viewer.CommandHistory.AddCommand(historyCommand);
		}

		private IClickAction CreateDisplaySetAction(ContextMenuActionFactoryArgs args, IDisplaySet displaySet)
		{
			MenuAction action = UnavailableImageSetContextMenuActionFactory.CreateAction(args);
			action.Label = displaySet.Name;
			action.SetClickHandler(delegate { AssignDisplaySetToImageBox(args.ImageViewer, displaySet); });
			action.Checked = args.ImageViewer.SelectedImageBox != null &&
				args.ImageViewer.SelectedImageBox.DisplaySet != null &&
				args.ImageViewer.SelectedImageBox.DisplaySet.Uid == displaySet.Uid;

			return action;
		}

		#region IContextMenuActionFactory Members

		public ClearCanvas.Desktop.Actions.IAction[] CreateActions(ContextMenuActionFactoryArgs args)
		{
			List<IAction> actions = new List<IAction>();

			foreach (IDisplaySet displaySet in args.ImageSet.DisplaySets)
				actions.Add(CreateDisplaySetAction(args, displaySet));

			return actions.ToArray();
		}

		#endregion
	}
}
