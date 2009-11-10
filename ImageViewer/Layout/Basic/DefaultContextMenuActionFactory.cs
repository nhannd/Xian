using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public partial class ContextMenuLayoutTool
	{
		private class DefaultContextMenuActionFactory : ActionFactory
		{
			public DefaultContextMenuActionFactory()
			{ }

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


			private IClickAction CreateUnavailableStudyAction(IActionFactoryContext context)
			{
				UnavailableImageSetDescriptor descriptor = (UnavailableImageSetDescriptor)context.ImageSet.Descriptor;

				MenuAction action = CreateMenuAction(context, GetActionLabel(descriptor),
										() => context.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok));

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

			private IAction[] CreateDisplaySetActions(IActionFactoryContext context)
			{
				List<IAction> actions = new List<IAction>();

				foreach (IDisplaySet displaySet in context.ImageSet.DisplaySets)
				{
					IDisplaySet theDisplaySet = displaySet;
					MenuAction action = CreateMenuAction(context, displaySet.Name,
						delegate { AssignDisplaySetToImageBox(context.ImageViewer, theDisplaySet); });

					action.Checked = context.ImageViewer.SelectedImageBox != null &&
						context.ImageViewer.SelectedImageBox.DisplaySet != null &&
						context.ImageViewer.SelectedImageBox.DisplaySet.Uid == theDisplaySet.Uid;

					actions.Add(action);
				}

				return actions.ToArray();
			}

			#endregion

			public override IAction[] CreateActions(IActionFactoryContext context)
			{
				List<IAction> actions = new List<IAction>();

				if (context.ImageSet.Descriptor is UnavailableImageSetDescriptor)
					actions.Add(CreateUnavailableStudyAction(context));
				else
					actions.AddRange(CreateDisplaySetActions(context));

				return actions.ToArray();
			}
		}
	}
}