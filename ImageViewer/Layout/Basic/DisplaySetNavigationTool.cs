#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[KeyboardAction("previous", "imageviewer-keyboard/ToolsStandardPreviousDisplaySet", "PreviousDisplaySet", KeyStroke = XKeys.E)]
	[MenuAction("previous", "global-menus/MenuTools/MenuStandard/MenuPreviousDisplaySet", "PreviousDisplaySet")]
	[ButtonAction("previous", "global-toolbars/ToolbarStandard/ToolbarPreviousDisplaySet", "PreviousDisplaySet")]
	[Tooltip("previous", "TooltipPreviousDisplaySet")]
	[IconSet("previous", IconScheme.Colour, "Icons.PreviousDisplaySetToolSmall.png", "Icons.PreviousDisplaySetToolMedium.png", "Icons.PreviousDisplaySetToolLarge.png")]
	[GroupHint("previous", "Tools.Navigation.DisplaySets.Previous")]

	[KeyboardAction("next", "imageviewer-keyboard/ToolsStandardNextDisplaySet", "NextDisplaySet", KeyStroke = XKeys.N)]
	[MenuAction("next", "global-menus/MenuTools/MenuStandard/MenuNextDisplaySet", "NextDisplaySet")]
	[ButtonAction("next", "global-toolbars/ToolbarStandard/ToolbarNextDisplaySet", "NextDisplaySet")]
	[Tooltip("next", "TooltipNextDisplaySet")]
	[IconSet("next", IconScheme.Colour, "Icons.NextDisplaySetToolSmall.png", "Icons.NextDisplaySetToolMedium.png", "Icons.NextDisplaySetToolLarge.png")]
	[GroupHint("next", "Tools.Navigation.DisplaySets.Next")]

	[EnabledStateObserver("next", "Enabled", "EnabledChanged")]
	[EnabledStateObserver("previous", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DisplaySetNavigationTool : Tool<IImageViewerToolContext>
	{
		// NOTE: this is purposely *not* derived from ImageViewerTool because that class sets Enabled differently than we want,
		// and we would have to override the methods and do nothing in order for it to work properly, which is a bit hacky.

		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		public DisplaySetNavigationTool()
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		private void OnStudyLoaded(object sender, ItemEventArgs<Study> e)
		{
			UpdateEnabled();
		}

		private void OnImageLoaded(object sender, ItemEventArgs<Sop> e)
		{
			UpdateEnabled();
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		private IDisplaySet GetSourceDisplaySet()
		{
			IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
			if (imageBox == null)
				return null;

			IDisplaySet currentDisplaySet = imageBox.DisplaySet;

			if (currentDisplaySet == null || currentDisplaySet.ParentImageSet == null)
				return null;

			return CollectionUtils.SelectFirst(currentDisplaySet.ParentImageSet.DisplaySets,
									   delegate(IDisplaySet displaySet)
									   {
										   return displaySet.Uid == currentDisplaySet.Uid;
									   });
		}

		private void UpdateEnabled()
		{
			IDisplaySet sourceDisplaySet = GetSourceDisplaySet();
			Enabled = sourceDisplaySet != null && sourceDisplaySet.ParentImageSet.DisplaySets.Count > 1;
		}

		public override void Initialize()
		{
			base.Initialize();

			UpdateEnabled();

			base.Context.Viewer.EventBroker.ImageLoaded += OnImageLoaded;
			base.Context.Viewer.EventBroker.StudyLoaded += OnStudyLoaded;
			base.Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageLoaded -= OnImageLoaded;
			base.Context.Viewer.EventBroker.StudyLoaded -= OnStudyLoaded;
			base.Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void NextDisplaySet()
		{
			AdvanceDisplaySet(+1);
		}

		public void PreviousDisplaySet()
		{
			AdvanceDisplaySet(-1);
		}

		public void AdvanceDisplaySet(int direction)
		{
			IDisplaySet sourceDisplaySet = GetSourceDisplaySet();
			if (sourceDisplaySet == null)
				return;

			IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
			IImageSet parentImageSet = sourceDisplaySet.ParentImageSet;

			int sourceDisplaySetIndex = parentImageSet.DisplaySets.IndexOf(sourceDisplaySet);
			sourceDisplaySetIndex += direction;

			if (sourceDisplaySetIndex < 0)
				sourceDisplaySetIndex = parentImageSet.DisplaySets.Count - 1;
			else if (sourceDisplaySetIndex >= parentImageSet.DisplaySets.Count)
				sourceDisplaySetIndex = 0;

			MemorableUndoableCommand command = new MemorableUndoableCommand(imageBox);
			command.BeginState = imageBox.CreateMemento();

			imageBox.DisplaySet = parentImageSet.DisplaySets[sourceDisplaySetIndex].CreateFreshCopy();
			imageBox.Draw();

			command.EndState = imageBox.CreateMemento();
			base.Context.Viewer.CommandHistory.AddCommand(command);
		}
	}
}
