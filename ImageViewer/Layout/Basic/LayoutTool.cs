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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuLayoutManager", "Show")]
	[DropDownButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarLayoutManager", "Show", "LayoutDropDownMenuModel")]
	[IconSet("show", IconScheme.Colour, "Icons.LayoutToolSmall.png", "Icons.LayoutToolMedium.png", "Icons.LayoutToolLarge.png")]
	[Tooltip("show", "TooltipLayoutManager")]
	[GroupHint("show", "Application.Workspace.Layout.Basic")]
	/// <summary>
	/// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf and coordinates
	/// it so that it reflects the state of the active workspace, as well as provides a dropdown custom action
	/// that can directly change the layout in the active imageviewer.
	/// </summary>
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LayoutTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		private IDesktopWindow _desktopWindow;
		private ActionModelRoot _actionModel;

		/// <summary>
		/// Constructor
		/// </summary>
		public LayoutTool()
		{
			_desktopWindow = null;
		}

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
					                                                          LayoutConfigurationSettings.MaximumImageBoxRows,
					                                                          LayoutConfigurationSettings.MaximumImageBoxColumns,
					                                                          this.SetImageBoxLayout, pathBoxes, resolver);
					root.InsertAction(actionBoxes);

					ActionPath pathTiles = new ActionPath("root/ToolbarLayoutTilesChooser", resolver);
					LayoutChangerAction actionTiles = new LayoutChangerAction("chooseTileLayout",
					                                                          LayoutConfigurationSettings.MaximumTileRows,
					                                                          LayoutConfigurationSettings.MaximumTileColumns,
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
			LayoutComponent.SetImageBoxLayout(base.ImageViewer, rows, columns);
		}

		/// <summary>
		/// Sets the layout of the current imageviewer to the specified number of tiles.
		/// </summary>
		/// <param name="rows">The number of rows to show.</param>
		/// <param name="columns">The number of columns to show.</param>
		public void SetTileLayout(int rows, int columns)
		{
			LayoutComponent.SetTileLayout(base.ImageViewer, rows, columns);
		}

		public void Show()
		{
			// check if a layout component is already displayed
			if (_shelves.ContainsKey(this.Context.DesktopWindow))
			{
				_shelves[this.Context.DesktopWindow].Activate();
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
				_shelves[_desktopWindow] = shelf;

				_shelves[_desktopWindow].Closed += OnShelfClosed;
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

			_shelves[_desktopWindow].Closed -= OnShelfClosed;
			_shelves.Remove(_desktopWindow);
			_desktopWindow = null;
		}
	}
}