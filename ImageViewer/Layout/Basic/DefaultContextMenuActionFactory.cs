using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal class DefaultContextMenuActionFactory : ContextMenuActionFactory
	{
		public DefaultContextMenuActionFactory()
		{}

		#region Unavailable

		private string GetActionMessage(UnavailableImageSetDescriptor descriptor)
		{
			if (descriptor.IsOffline)
				return SR.MessageActionStudyOffline;
			else if (descriptor.IsNearline)
				return SR.MessageActionStudyNearline;
			else if (descriptor.IsInUse)
				return SR.MessageActionStudyInUse;
			else if (descriptor.IsNotLoadable)
				return SR.MessageActionNoStudyLoader;
			else
				return SR.MessageActionStudyCouldNotBeLoaded;
		}

		private string GetActionLabel(UnavailableImageSetDescriptor descriptor)
		{
			if (descriptor.IsOffline)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Offline);
			else if (descriptor.IsNearline)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Nearline);
			else if (descriptor.IsInUse)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.InUse);
			else if (descriptor.IsNotLoadable)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Unavailable);
			else
				return SR.LabelStudyCouldNotBeLoaded;
		}


		private IClickAction CreateUnavailableStudyAction(ContextMenuActionFactoryArgs args)
		{
			UnavailableImageSetDescriptor descriptor = (UnavailableImageSetDescriptor)args.ImageSet.Descriptor;

			MenuAction action = CreateMenuAction(args, GetActionLabel(descriptor),
					delegate
					{
						args.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok);
					});

			return action;
		}

		#endregion

		#region Display Sets

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

		private IAction[] CreateDisplaySetActions(ContextMenuActionFactoryArgs args)
		{
			List<IAction> actions = new List<IAction>();

			foreach (IDisplaySet displaySet in args.ImageSet.DisplaySets)
			{
				IDisplaySet theDisplaySet = displaySet;
				MenuAction action = CreateMenuAction(args, displaySet.Name,
					delegate { AssignDisplaySetToImageBox(args.ImageViewer, theDisplaySet); });

				action.Checked = args.ImageViewer.SelectedImageBox != null &&
					args.ImageViewer.SelectedImageBox.DisplaySet != null &&
					args.ImageViewer.SelectedImageBox.DisplaySet.Uid == theDisplaySet.Uid;

				actions.Add(action);
			}

			return actions.ToArray();
		}

		#endregion

		public override IAction[] CreateActions(ContextMenuActionFactoryArgs args)
		{
			List<IAction> actions = new List<IAction>();
	
			if (args.ImageSet.Descriptor is UnavailableImageSetDescriptor)
				actions.Add(CreateUnavailableStudyAction(args));
			else
				actions.AddRange(CreateDisplaySetActions(args));

			return actions.ToArray();
		}
	}
}
