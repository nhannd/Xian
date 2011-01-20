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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

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
		private static readonly Dictionary<IImageViewer, CineTool> _tools = new Dictionary<IImageViewer, CineTool>();

		private SynchronizationContext _synchronizationContext;
		private bool _autoCineEnabled = true;

		public CineTool() {}

		public void Activate()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			// check if a layout component is already displayed
			if (_shelves.ContainsKey(desktopWindow))
			{
				_shelves[desktopWindow].Activate();
			}
			else
			{
				LaunchShelf(desktopWindow, new CineApplicationComponent(desktopWindow), ShelfDisplayHint.DockFloat);
			}
		}

		protected IImageBox SelectedImageBox
		{
			get
			{
				if (this.ImageViewer == null)
					return null;
				return this.ImageViewer.SelectedImageBox;
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
			base.ImageViewer.DesktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceItemActivationChanged;
			base.ImageViewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;

			_tools.Add(base.ImageViewer, this);

			_synchronizationContext = SynchronizationContext.Current;
		}

		protected override void Dispose(bool disposing)
		{
			_synchronizationContext = null;

			_tools.Remove(base.ImageViewer);

			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.ImageViewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			base.ImageViewer.DesktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceItemActivationChanged;
			base.Dispose(disposing);
		}

		protected virtual void OnWorkspaceItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
		{
			if (e.Item.Active && e.Item.Component == this.ImageViewer)
			{
				TryUpdateAutoCineComponent(this.Context.DesktopWindow, this.SelectedImageBox);
			}
		}

		protected virtual void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (this.SelectedImageBox != null && this.ImageViewer.SelectedImageBox.DisplaySet == e.NewDisplaySet)
			{
				this.TryUpdateAutoCineComponent(this.Context.DesktopWindow, this.SelectedImageBox);
			}
		}

		protected virtual void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			this.TryUpdateAutoCineComponent(this.Context.DesktopWindow, e.SelectedImageBox);
		}

		/// <summary>
		/// This handles autocine state change operations within the same viewer instance
		/// </summary>
		protected virtual void TryUpdateAutoCineComponent(IDesktopWindow desktopWindow, IImageBox selectedImageBox)
		{
			// if we're in autocine mode, then start cine if image supports it (opening shelf as necessary), else stop cine and close shelf
			if (_autoCineEnabled)
			{
				if (selectedImageBox != null
					&& selectedImageBox.SelectedTile != null
					&& CineApplicationComponent.CanAutoPlay(selectedImageBox.SelectedTile.PresentationImage))
				{
					CineApplicationComponent component;
					if (!_shelves.ContainsKey(desktopWindow))
					{
						component = new CineApplicationComponent(desktopWindow);
						component.Reverse = false;
						LaunchShelf(desktopWindow, component, ShelfDisplayHint.DockFloat);
					}
					else
					{
						component = (CineApplicationComponent) _shelves[desktopWindow].Component;
					}

					// queue an attempt to start auto cine
					_synchronizationContext.Post(TryStartAutoCineCallback, component);
				}
				else
				{
					if (_shelves.ContainsKey(desktopWindow))
						_shelves[desktopWindow].Close(UserInteraction.NotAllowed);
				}
			}
		}

		private static void TryStartAutoCineCallback(object component)
		{
			try
			{
				CineApplicationComponent cineApplicationComponent = component as CineApplicationComponent;
				if (cineApplicationComponent != null && cineApplicationComponent.DesktopWindow != null)
					cineApplicationComponent.TryStartAutoCine();
			}
			catch (Exception ex)
			{
				// since this callback is posted directly to the message pump, we *must* log and silence exceptions, else the entire app will die
				Platform.Log(LogLevel.Debug, ex, "AutoCine callback threw an exception in TryStartAutoCineCallback.");
#if DEBUG
				throw;
#endif
			}
		}

		internal static bool GetAutoCineEnabled(IImageViewer imageViewer)
		{
			if (_tools.ContainsKey(imageViewer))
				return _tools[imageViewer]._autoCineEnabled;
			return false;
		}

		internal static void SetAutoCineEnabled(IImageViewer imageViewer, bool value)
		{
			if (_tools.ContainsKey(imageViewer))
				_tools[imageViewer]._autoCineEnabled = value;
		}
	}
}