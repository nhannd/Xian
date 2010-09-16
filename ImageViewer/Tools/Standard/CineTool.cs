#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuCine", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarCine", "Activate", KeyStroke = XKeys.C)]
	[Tooltip("activate", "TooltipCine")]
	[IconSet("activate", IconScheme.Colour, "Icons.CineToolSmall.png", "Icons.CineToolMedium.png", "Icons.CineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Cine")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class CineTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();

		private SynchronizationContext _synchronizationContext;

		public CineTool() {}

		public void Activate()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			// check if a cine component is already displayed
			if (_shelves.ContainsKey(desktopWindow))
			{
				_shelves[desktopWindow].Activate();
			}
			else
			{
				LaunchShelf(desktopWindow, new CineApplicationComponent(desktopWindow), ShelfDisplayHint.DockFloat);
			}
		}

		private static void LaunchShelf(IDesktopWindow desktopWindow, IApplicationComponent component, ShelfDisplayHint shelfDisplayHint)
		{
			IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleCine, "Cine", shelfDisplayHint);
			_shelves[desktopWindow] = shelf;
			_shelves[desktopWindow].Closed += OnShelfClosed;
		}

		private static void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			// We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
			// ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
			// exist at the DesktopWindow level and there can only be one of each type of shelf
			// open at the same time per DesktopWindow (otherwise things look funny).  Because of 
			// this, we need to allow this event handling method to be called after this tool has
			// already been disposed (e.g. viewer workspace closed), which is why we store the 
			// _desktopWindow variable.

			IShelf shelf = (IShelf) sender;
			shelf.Closed -= OnShelfClosed;
			_shelves.Remove(shelf.DesktopWindow);
		}

		public override void Initialize()
		{
			base.Initialize();

			_synchronizationContext = SynchronizationContext.Current;

			Context.DesktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceItemActivationChanged;
			Context.Viewer.EventBroker.ImageBoxSelected += OnImageViewerImageBoxSelected;
			Context.Viewer.EventBroker.DisplaySetChanged += OnImageViewerDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Context.Viewer.EventBroker.DisplaySetChanged -= OnImageViewerDisplaySetChanged;
				Context.Viewer.EventBroker.ImageBoxSelected -= OnImageViewerImageBoxSelected;
				Context.DesktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceItemActivationChanged;
			}

			_synchronizationContext = null;

			base.Dispose(disposing);
		}

		private void OnWorkspaceItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
		{
			// retry autocine whenever the parent viewer workspace is activated
			if (e.Item.Active && ReferenceEquals(e.Item.Component, Context.Viewer))
				TryStartAutoCine();
		}

		private void OnImageViewerImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			// retry autocine whenever the selected image box changes
			TryStartAutoCine();
		}

		private void OnImageViewerDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			// retry autocine whenever the display set in the selected image box changes
			if (e.NewDisplaySet != null && ReferenceEquals(e.NewDisplaySet.ImageBox, Context.Viewer.SelectedImageBox))
			{
				// this is not as straightforward since the cine component stops if the user commits some undoable command, such as stack or changing the display set
				// we actually have to post the call to TryStartAutoCine so that it gets called *after* the GUI event finishes processing
				if (_synchronizationContext != null)
					_synchronizationContext.Post(o => TryStartAutoCine(), null);
			}
		}

		/// <summary>
		/// Called to attempt automatically starting cine in the context of this tool's parent <see cref="ImageViewerTool.ImageViewer"/>.
		/// </summary>
		protected virtual void TryStartAutoCine()
		{
			var desktopWindow = Context.DesktopWindow;
			if (IsAutoCineEnabled(Context.Viewer.SelectedImageBox))
			{
				// if we should auto cine the selected image box, find the existing cine component or create a new one if necessary
				CineApplicationComponent cineComponent;
				if (_shelves.ContainsKey(desktopWindow))
				{
					cineComponent = (CineApplicationComponent) _shelves[desktopWindow].Component;
					_shelves[desktopWindow].Activate();
				}
				else
				{
					cineComponent = new CineApplicationComponent(desktopWindow);
					LaunchShelf(desktopWindow, cineComponent, ShelfDisplayHint.DockFloat);
				}

				// start cine if it isn't already playing
				if (!cineComponent.Running)
					cineComponent.StartCine();
			}
		}

		/// <summary>
		/// Checks to see if the viewer should automatically start cine on the contents of the specified <see cref="IImageBox"/>.
		/// </summary>
		/// <param name="imageBox">The <see cref="IImageBox"/> to be checked.</param>
		/// <returns>True if the viewer should automatically start cine on the contents of <paramref name="imageBox"/>; False otherwise.</returns>
		protected internal static bool IsAutoCineEnabled(IImageBox imageBox)
		{
			if (imageBox == null || imageBox.SelectedTile == null)
				return false;

			var imageSopProvider = imageBox.SelectedTile.PresentationImage as IImageSopProvider;
			if (imageSopProvider != null)
			{
				var imageSop = imageSopProvider.ImageSop;
				if (imageSop.NumberOfFrames > 1
				    && ToolSettings.Default.ToolSettingsProfile[imageSop.Modality].AutoCineMultiframes.GetValueOrDefault(false))
				{
					return true;
				}
			}
			return false;
		}
	}
}