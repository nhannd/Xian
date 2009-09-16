using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal class DefaultContextMenuActionFactory : IContextMenuActionFactory
	{
		public DefaultContextMenuActionFactory()
		{}

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
			MenuAction action = CreateAction(args);
			action.Label = displaySet.Name;
			action.SetClickHandler(delegate { AssignDisplaySetToImageBox(args.ImageViewer, displaySet); });
			action.Checked = args.ImageViewer.SelectedImageBox != null &&
				args.ImageViewer.SelectedImageBox.DisplaySet != null &&
				args.ImageViewer.SelectedImageBox.DisplaySet.Uid == displaySet.Uid;

			return action;
		}

		private IClickAction CreateUnavailableStudyAction(ContextMenuActionFactoryArgs args)
		{
			UnavailableImageSetDescriptor descriptor = (UnavailableImageSetDescriptor)args.ImageSet.Descriptor;

			MenuAction action = CreateAction(args);
			action.Label = GetActionLabel(descriptor);
			action.SetClickHandler(delegate
			{
				args.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok);
			});

			return action;
		}

		private MenuAction CreateAction(ContextMenuActionFactoryArgs args)
		{
			string actionId = args.GetNextActionId();
			string fullyQualifiedActionId = args.GetFullyQualifiedActionId(actionId);

			string pathString = String.Format("{0}/{1}", args.BasePath, actionId);
			ActionPath path = new ActionPath(pathString, null);
			return new MenuAction(fullyQualifiedActionId, path, ClickActionFlags.CheckParents, null);
		}

		#region IContextMenuActionFactory Members

		public IAction[] CreateActions(ContextMenuActionFactoryArgs args)
		{
			List<IAction> actions = new List<IAction>();
			if (args.ImageSet.Descriptor is UnavailableImageSetDescriptor)
			{
				actions.Add(CreateUnavailableStudyAction(args));
			}
			else
			{
				foreach (IDisplaySet displaySet in args.ImageSet.DisplaySets)
					actions.Add(CreateDisplaySetAction(args, displaySet));
			}

			return actions.ToArray();
		}

		#endregion
	}
}