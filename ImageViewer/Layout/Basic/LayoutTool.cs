#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using System;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuLayoutManager", "Show")]
	[DropDownButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarLayoutManager", "Show", "LayoutDropDownMenuModel")]
	[IconSet("show", IconScheme.Colour, "Icons.LayoutToolSmall.png", "Icons.LayoutToolMedium.png", "Icons.LayoutToolLarge.png")]
	[Tooltip("show", "TooltipLayoutManager")]
	[GroupHint("show", "Application.Workspace.Layout.Basic")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	/// <summary>
	/// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf and coordinates
	/// it so that it reflects the state of the active workspace, as well as provides a dropdown custom action
	/// that can directly change the layout in the active imageviewer.
	/// </summary>
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LayoutTool : Tool<IImageViewerToolContext>
	{
		[ThreadStatic]
		private static Dictionary<IDesktopWindow, IShelf> _shelves;
		
		private IDesktopWindow _desktopWindow;
		private ActionModelRoot _actionModel;
		private bool _enabled;

		/// <summary>
		/// Constructor
		/// </summary>
		public LayoutTool()
		{
			_desktopWindow = null;
		}

		public static Dictionary<IDesktopWindow, IShelf> Shelves
		{
			get
			{
				if (_shelves == null)
					_shelves = new Dictionary<IDesktopWindow, IShelf>();
				return _shelves;
			}	
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (value == _enabled)
					return;

				_enabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged;

		/// <summary>
		/// Gets the action model for the layout drop down menu.
		/// </summary>
		public ActionModelNode LayoutDropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
				{
					ActionModelRoot root = new ActionModelRoot();
					ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

					ActionPath pathBoxes = new ActionPath("root/ToolbarLayoutBoxesChooser", resolver);
					LayoutChangerAction actionBoxes = new LayoutChangerAction("chooseBoxLayout",
					                                                          LayoutSettings.MaximumImageBoxRows,
					                                                          LayoutSettings.MaximumImageBoxColumns,
					                                                          this.SetImageBoxLayout, pathBoxes, resolver);
					root.InsertAction(actionBoxes);

					ActionPath pathTiles = new ActionPath("root/ToolbarLayoutTilesChooser", resolver);
					LayoutChangerAction actionTiles = new LayoutChangerAction("chooseTileLayout",
					                                                          LayoutSettings.MaximumTileRows,
					                                                          LayoutSettings.MaximumTileColumns,
					                                                          this.SetTileLayout, pathTiles, resolver);
					root.InsertAction(actionTiles);

					_actionModel = root;
				}

				return _actionModel;
			}
		}

		/// <summary>
		/// Sets the layout of the current imageviewer to the specified number of imageboxes.
		/// </summary>
		/// <param name="rows">The number of rows to show.</param>
		/// <param name="columns">The number of columns to show.</param>
		public void SetImageBoxLayout(int rows, int columns)
		{
			LayoutComponent.SetImageBoxLayout(base.Context.Viewer, rows, columns);
		}

		/// <summary>
		/// Sets the layout of the current imageviewer to the specified number of tiles.
		/// </summary>
		/// <param name="rows">The number of rows to show.</param>
		/// <param name="columns">The number of columns to show.</param>
		public void SetTileLayout(int rows, int columns)
		{
			LayoutComponent.SetTileLayout(base.Context.Viewer, rows, columns);
		}

		public override void Initialize()
		{
			base.Initialize();
			base.Context.Viewer.PhysicalWorkspace.LockedChanged += new System.EventHandler(OnLockedChanged);
			Enabled = !base.Context.Viewer.PhysicalWorkspace.Locked;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.PhysicalWorkspace.LockedChanged -= new System.EventHandler(OnLockedChanged);
			base.Dispose(disposing);
		}
		
		private void OnLockedChanged(object sender, System.EventArgs e)
		{
			Enabled = !base.Context.Viewer.PhysicalWorkspace.Locked;
		}

		public void Show()
		{
			// check if a layout component is already displayed
			if (Shelves.ContainsKey(this.Context.DesktopWindow))
			{
				Shelves[this.Context.DesktopWindow].Activate();
			}
			else
			{
				_desktopWindow = this.Context.DesktopWindow;

				LayoutComponent layoutComponent = new LayoutComponent(_desktopWindow);

				IShelf shelf = ApplicationComponent.LaunchAsShelf(
					_desktopWindow,
					layoutComponent,
					SR.TitleLayoutManager,
					"Layout",
					ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);
				Shelves[_desktopWindow] = shelf;

				Shelves[_desktopWindow].Closed += OnShelfClosed;
			}
		}

		private void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			// We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
			// ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
			// exist at the DesktopWindow level and there can only be one of each type of shelf
			// open at the same time per DesktopWindow (otherwise things look funny).  Because of 
			// this, we need to allow this event handling method to be called after this tool has
			// already been disposed (e.g. viewer workspace closed), which is why we store the 
			// _desktopWindow variable.

			Shelves[_desktopWindow].Closed -= OnShelfClosed;
			Shelves.Remove(_desktopWindow);
			_desktopWindow = null;
		}
	}
}