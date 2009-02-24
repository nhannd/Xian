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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="LayoutComponent"/>
	/// </summary>
	[ExtensionPoint()]
	public sealed class LayoutComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// This component allows a user to control the layout of an associated image viewer.
	/// Set the <see cref="LayoutComponent.Subject"/> property to the <see cref="IImageViewer"/>
	/// that is to be controlled.
	/// </summary>
	[AssociateView(typeof (LayoutComponentViewExtensionPoint))]
	public class LayoutComponent : ImageViewerToolComponent
	{
		private int _imageBoxRows;
		private int _imageBoxColumns;
		private int _tileRows;
		private int _tileColumns;

		/// <summary>
		/// Constructor
		/// </summary>
		public LayoutComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow) {}

		#region Public properties and methods for use by the view

		/// <summary>
		/// Indicates to the view whether the image-box section of the user-interface should be enabled
		/// </summary>
		public bool ImageBoxSectionEnabled
		{
			get
			{
				return this.ImageViewer != null && !this.ImageViewer.PhysicalWorkspace.Locked;
			}
		}

		/// <summary>
		/// Indicates to the view whether the tile section of the user-interface should be enabled
		/// </summary>
		public bool TileSectionEnabled
		{
			get
			{
				return this.ImageBoxSectionEnabled && this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null;
			}
		}

		/// <summary>
		/// Gets the maximum allowable rows for image boxes.
		/// </summary>
		public int MaximumImageBoxRows
		{
			get { return LayoutConfigurationSettings.MaximumImageBoxRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for image boxes.
		/// </summary>
		public int MaximumImageBoxColumns
		{
			get { return LayoutConfigurationSettings.MaximumImageBoxColumns; }
		}

		/// <summary>
		/// Gets the maximum allowable rows for tiles.
		/// </summary>
		public int MaximumTileRows
		{
			get { return LayoutConfigurationSettings.MaximumTileRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for tiles.
		/// </summary>
		public int MaximumTileColumns
		{
			get { return LayoutConfigurationSettings.MaximumTileColumns; }
		}

		/// <summary>
		/// Gets/sets the number of image box rows
		/// </summary>
		public int ImageBoxRows
		{
			get { return _imageBoxRows; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumImageBoxRows);
				if (newValue == _imageBoxRows)
					return;

				_imageBoxRows = newValue;
				NotifyPropertyChanged("ImageBoxRows");
			}
		}

		/// <summary>
		/// Gets/sets the number of image box columns
		/// </summary>
		public int ImageBoxColumns
		{
			get { return _imageBoxColumns; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumImageBoxColumns);

				if (newValue == _imageBoxColumns)
					return;

				_imageBoxColumns = newValue;
				NotifyPropertyChanged("ImageBoxColumns");
			}
		}

		/// <summary>
		/// Gets/sets the number of tile rows
		/// </summary>
		public int TileRows
		{
			get { return _tileRows; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumTileRows);
				if (newValue == _tileRows)
					return;

				_tileRows = newValue;
				NotifyPropertyChanged("TileRows");
			}
		}

		/// <summary>
		/// Gets/sets the number of tile columns
		/// </summary>
		public int TileColumns
		{
			get { return _tileColumns; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumTileColumns);
				if (newValue == _tileColumns)
					return;

				_tileColumns = newValue;
				NotifyPropertyChanged("TileColumns");
			}
		}

		public void Configure()
		{
			LayoutConfigurationApplicationComponent.Configure(this.Host.DesktopWindow);
		}

		/// <summary>
		/// Called by the view to apply the image layout to the subject
		/// </summary>
		public void ApplyImageBoxLayout()
		{
			if (this.ImageViewer == null)
				return;

			SetImageBoxLayout(this.ImageViewer, this.ImageBoxRows, this.ImageBoxColumns);

			Update();
		}

		/// <summary>
		/// Called by the view to apply the tile layout to the subject
		/// </summary>
		public void ApplyTileLayout()
		{
			if (this.ImageViewer == null)
				return;

			SetTileLayout(this.ImageViewer, this.TileRows, this.TileColumns);

			Update();
		}

		#endregion

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			// stop listening to the old image viewer, if one was set
			if (e.DeactivatedImageViewer != null)
			{
				e.DeactivatedImageViewer.PhysicalWorkspace.LockedChanged -= OnPhysicalWorkspaceLockedChanged;
				e.DeactivatedImageViewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;
			}

			// start listening to the new image viewer, if one has been set
			if (e.ActivatedImageViewer != null)
			{
				e.ActivatedImageViewer.PhysicalWorkspace.LockedChanged += OnPhysicalWorkspaceLockedChanged;
				e.ActivatedImageViewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
			}

			Update();
		}

		private void OnPhysicalWorkspaceLockedChanged(object sender, EventArgs e)
		{
			Update();
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			Update();
		}

		private void Update()
		{
			if (this.ImageViewer != null)
			{
				ImageBoxRows = this.ImageViewer.PhysicalWorkspace.Rows;
				ImageBoxColumns = this.ImageViewer.PhysicalWorkspace.Columns;

				if (this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null)
				{
					TileRows = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Rows;
					TileColumns = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Columns;
				}
			}

			NotifyPropertyChanged("ImageBoxSectionEnabled");
			NotifyPropertyChanged("TileSectionEnabled");
		}

		public static void SetImageBoxLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutConfigurationSettings.MaximumImageBoxRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutConfigurationSettings.MaximumImageBoxColumns, "columns");

			IPhysicalWorkspace physicalWorkspace = imageViewer.PhysicalWorkspace;
			if (physicalWorkspace.Locked)
				return;

			int tileRows = Math.Max(1, physicalWorkspace.SelectedImageBox.Rows);
			int tileColumns = Math.Max(1, physicalWorkspace.SelectedImageBox.Columns);

			MemorableUndoableCommand command = new MemorableUndoableCommand(physicalWorkspace);
			command.Name = SR.CommandLayoutImageBoxes;
			command.BeginState = physicalWorkspace.CreateMemento();

			int oldRows = physicalWorkspace.Rows;
			int oldColumns = physicalWorkspace.Columns;
			KeyValuePair<IDisplaySet, int>[,] oldDisplaySets = new KeyValuePair<IDisplaySet, int>[oldRows,oldColumns];
			for (int row = 0; row < oldRows; ++row)
			{
				for (int column = 0; column < oldColumns; ++column)
				{
					IImageBox imageBox = physicalWorkspace[row, column];
					oldDisplaySets[row, column] = new KeyValuePair<IDisplaySet, int>(imageBox.DisplaySet, imageBox.TopLeftPresentationImageIndex);
				}
			}

			physicalWorkspace.SetImageBoxGrid(rows, columns);

			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
				imageBox.SetTileGrid(tileRows, tileColumns);

			// Try to keep existing display sets in the same row/column position.
			for (int row = 0; row < physicalWorkspace.Rows && row < oldRows; ++row)
			{
				for (int column = 0; column < physicalWorkspace.Columns && column < oldColumns; ++column)
				{
					KeyValuePair<IDisplaySet, int> kvp = oldDisplaySets[row, column];
					if (kvp.Key != null)
					{
						IImageBox imageBox = physicalWorkspace[row, column];
						imageBox.DisplaySet = kvp.Key;
						imageBox.TopLeftPresentationImageIndex = kvp.Value;
					}
				}
			}

			physicalWorkspace.Draw();
			physicalWorkspace.SelectDefaultImageBox();

			command.EndState = physicalWorkspace.CreateMemento();

			imageViewer.CommandHistory.AddCommand(command);
		}

		public static void SetTileLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutConfigurationSettings.MaximumTileRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutConfigurationSettings.MaximumTileColumns, "columns");

			IImageBox imageBox = imageViewer.PhysicalWorkspace.SelectedImageBox;
			if (imageBox.ParentPhysicalWorkspace.Locked)
				return;

			MemorableUndoableCommand command = new MemorableUndoableCommand(imageBox);
			command.Name = SR.CommandLayoutTiles;
			command.BeginState = imageBox.CreateMemento();

			int index = imageBox.TopLeftPresentationImageIndex;

			imageBox.SetTileGrid(rows, columns);
			imageBox.TopLeftPresentationImageIndex = index;
			imageBox.Draw();
			imageBox.SelectDefaultTile();

			command.EndState = imageBox.CreateMemento();

			imageViewer.CommandHistory.AddCommand(command);
		}
	}
}