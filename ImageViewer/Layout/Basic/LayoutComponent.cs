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
using System.Collections;
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
			get { return LayoutSettings.MaximumImageBoxRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for image boxes.
		/// </summary>
		public int MaximumImageBoxColumns
		{
			get { return LayoutSettings.MaximumImageBoxColumns; }
		}

		/// <summary>
		/// Gets the maximum allowable rows for tiles.
		/// </summary>
		public int MaximumTileRows
		{
			get { return LayoutSettings.MaximumTileRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for tiles.
		/// </summary>
		public int MaximumTileColumns
		{
			get { return LayoutSettings.MaximumTileColumns; }
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
			LayoutConfigurationComponent.Configure(this.Host.DesktopWindow);
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

		private static object[,] GetImageBoxMementos(IPhysicalWorkspace physicalWorkspace)
		{
			int rows = physicalWorkspace.Rows;
			int columns = physicalWorkspace.Columns;

			object[,] mementos = new object[rows, columns];

			for (int row = 0; row < rows; ++row)
			{
				for (int column = 0; column < columns; ++column)
				{
					IImageBox imageBox = physicalWorkspace[row, column];
					if (imageBox.DisplaySet != null)
						mementos[row, column] = imageBox.CreateMemento();
				}
			}

			return mementos;
		}

		private static Queue GetOffScreenImageBoxMementos(IPhysicalWorkspace physicalWorkspace, object[,] oldImageBoxMementos)
		{
			int oldRows = oldImageBoxMementos.GetLength(0);
			int oldColumns = oldImageBoxMementos.GetLength(1);

			int newRows = physicalWorkspace.Rows;
			int newColumns = physicalWorkspace.Columns;

			int sameRows = Math.Min(oldRows, newRows);
			int sameColumns = Math.Min(oldColumns, newColumns);

			Queue offScreenMementos = new Queue();
			//Get mementos for all the display sets that have gone off-screen, from top-to-bottom, left-to-right.

			for (int row = 0; row < sameRows; ++row)
			{
				for (int column = sameColumns; column < oldColumns; ++column)
				{
					object memento = oldImageBoxMementos[row, column];
					if (memento != null)
						offScreenMementos.Enqueue(memento);
				}
			}

			for (int row = sameRows; row < oldRows; ++row)
			{
				for (int column = 0; column < oldColumns; ++column)
				{
					object memento = oldImageBoxMementos[row, column];
					if (memento != null)
						offScreenMementos.Enqueue(memento);
				}
			}

			return offScreenMementos;
		}

		private static IEnumerable<IImageBox> GetAvailableEmptyImageBoxes(IPhysicalWorkspace physicalWorkspace)
		{
			Stack<IImageBox> imageBoxes = new Stack<IImageBox>();

			//go top to bottom, right to left, stopping before the first non-empty image box.
			for (int row = 0; row < physicalWorkspace.Rows; ++row)
			{
				for (int column = physicalWorkspace.Columns - 1; column >= 0; --column)
				{
					IImageBox imageBox = physicalWorkspace[row, column];
					if (imageBox.DisplaySet == null)
						imageBoxes.Push(imageBox);
					else
						break; //skip to the next row
				}

				while (imageBoxes.Count > 0)
					yield return imageBoxes.Pop();
			}
		}

		private static IDisplaySet GetNextDisplaySet(IPhysicalWorkspace physicalWorkspace)
		{
			foreach (IImageSet imageSet in physicalWorkspace.LogicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					bool alreadyVisible = false;
					foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
					{
						if (imageBox.DisplaySet != null && imageBox.DisplaySet.Uid == displaySet.Uid)
						{
							alreadyVisible = true;
							break;
						}
					}

					if (!alreadyVisible)
						return displaySet.CreateFreshCopy();
				}
			}

			return null;
		}

		private static void FillImageBoxes(IEnumerable<IImageBox> imageBoxes, Queue oldImageBoxMementos)
		{
			foreach (IImageBox imageBox in imageBoxes)
			{
				if (oldImageBoxMementos.Count > 0)
				{
					imageBox.SetMemento(oldImageBoxMementos.Dequeue());
				}
				else
				{
					imageBox.SetTileGrid(1, 1);
					imageBox.DisplaySet = GetNextDisplaySet(imageBox.ParentPhysicalWorkspace);
				}
			}
		}

		private static void SetImageBoxLayout(IPhysicalWorkspace physicalWorkspace, int rows, int columns)
		{
			object[,] oldImageBoxMementos = GetImageBoxMementos(physicalWorkspace);

			physicalWorkspace.SetImageBoxGrid(rows, columns);

			Queue offScreenMementos = GetOffScreenImageBoxMementos(physicalWorkspace, oldImageBoxMementos);

			int newRows = physicalWorkspace.Rows;
			int newColumns = physicalWorkspace.Columns;

			int oldRows = oldImageBoxMementos.GetLength(0);
			int oldColumns = oldImageBoxMementos.GetLength(1);

			int sameRows = Math.Min(oldRows, newRows);
			int sameColumns = Math.Min(oldColumns, newColumns);

			// Try to keep existing display sets in the same row/column position, if possible.
			for (int row = 0; row < sameRows; ++row)
			{
				for (int column = 0; column < sameColumns; ++column)
				{
					object memento = oldImageBoxMementos[row, column];
					if (memento == null)
						physicalWorkspace[row, column].SetTileGrid(1, 1);
					else
						physicalWorkspace[row, column].SetMemento(memento);
				}
			}

			// Fill in available image boxes, preferably with display sets that went 'off-screen',
			// followed by new ones that are not already visible.
			FillImageBoxes(GetAvailableEmptyImageBoxes(physicalWorkspace), offScreenMementos);
		}

		private static void SetImageBoxLayoutSimple(IPhysicalWorkspace physicalWorkspace, int rows, int columns)
		{
			Queue oldMementos = new Queue();
			foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
				oldMementos.Enqueue(imageBox.CreateMemento());

			physicalWorkspace.SetImageBoxGrid(rows, columns);

			FillImageBoxes(physicalWorkspace.ImageBoxes, oldMementos);
		}

		public static void SetImageBoxLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutSettings.MaximumImageBoxRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutSettings.MaximumImageBoxColumns, "columns");

			IPhysicalWorkspace physicalWorkspace = imageViewer.PhysicalWorkspace;
			if (physicalWorkspace.Locked)
				return;

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(physicalWorkspace);
			memorableCommand.BeginState = physicalWorkspace.CreateMemento();

			bool isOldLayoutRectangular = physicalWorkspace.Rows > 0 && physicalWorkspace.Columns > 0;
			if (isOldLayoutRectangular)
			{
				SetImageBoxLayout(physicalWorkspace, rows, columns);
			}
			else
			{
				SetImageBoxLayoutSimple(physicalWorkspace, rows, columns);
			}

			physicalWorkspace.Draw();
			physicalWorkspace.SelectDefaultImageBox();

			memorableCommand.EndState = physicalWorkspace.CreateMemento();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(physicalWorkspace);
			historyCommand.Name = SR.CommandLayoutImageBoxes;
			historyCommand.Enqueue(memorableCommand);

			imageViewer.CommandHistory.AddCommand(historyCommand);
		}

		public static void SetTileLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutSettings.MaximumTileRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutSettings.MaximumTileColumns, "columns");

			IImageBox imageBox = imageViewer.PhysicalWorkspace.SelectedImageBox;
			if (imageBox.ParentPhysicalWorkspace.Locked)
				return;

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			int index = imageBox.TopLeftPresentationImageIndex;

			imageBox.SetTileGrid(rows, columns);
			imageBox.TopLeftPresentationImageIndex = index;
			imageBox.Draw();
			imageBox.SelectDefaultTile();

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Name = SR.CommandLayoutTiles; 
			historyCommand.Enqueue(memorableCommand);
			imageViewer.CommandHistory.AddCommand(historyCommand);
		}
	}
}