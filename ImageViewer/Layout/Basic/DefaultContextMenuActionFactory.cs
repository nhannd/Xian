#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

			private string GetActionMessage(IDicomImageSetDescriptor descriptor)
			{
				if (descriptor.IsOffline)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyOffline;
				else if (descriptor.IsNearline)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyNearline;
				else if (descriptor.IsInUse)
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyInUse;
				else if (descriptor.IsNotLoadable)
                    return ClearCanvas.ImageViewer.SR.MessageInfoNoStudyLoader;
				else
                    return ClearCanvas.ImageViewer.SR.MessageInfoStudyCouldNotBeLoaded;
			}

            private string GetActionLabel(IDicomImageSetDescriptor descriptor)
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
			    var descriptor = context.ImageSet.Descriptor as IDicomImageSetDescriptor;
                if (descriptor == null || descriptor.LoadStudyError == null)
                    return null;

				return CreateMenuAction(context, GetActionLabel(descriptor),
										() => context.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok));
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

				IImageBox imageBox = context.ImageViewer.SelectedImageBox;
				if (imageBox != null && !imageBox.DisplaySetLocked)
				{
					foreach (IDisplaySet displaySet in context.ImageSet.DisplaySets)
					{
						IDisplaySet theDisplaySet = displaySet;
						MenuAction action = CreateMenuAction(context, displaySet.Name,
											() => AssignDisplaySetToImageBox(context.ImageViewer, theDisplaySet));

						action.Checked = context.ImageViewer.SelectedImageBox != null &&
							context.ImageViewer.SelectedImageBox.DisplaySet != null &&
							context.ImageViewer.SelectedImageBox.DisplaySet.Uid == theDisplaySet.Uid;

						actions.Add(action);
					}
				}

				return actions.ToArray();
			}

			#endregion

			public override IAction[] CreateActions(IActionFactoryContext context)
			{
				List<IAction> actions = new List<IAction>();

			    var unavailable = CreateUnavailableStudyAction(context);
                if (unavailable != null)
                    actions.Add(unavailable);
				else
					actions.AddRange(CreateDisplaySetActions(context));

				return actions.ToArray();
			}
		}
	}
}