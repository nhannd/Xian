#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

				if (context.ImageSet.Descriptor is UnavailableImageSetDescriptor)
					actions.Add(CreateUnavailableStudyAction(context));
				else
					actions.AddRange(CreateDisplaySetActions(context));

				return actions.ToArray();
			}
		}
	}
}