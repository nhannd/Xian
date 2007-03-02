using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageViewer;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    /// <summary>
    /// Defines an extension point for views onto a <see cref="LayoutComponent"/>
    /// </summary>
    [ExtensionPoint()]
    public class LayoutComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// This component allows a user to control the layout of an associated image viewer.
    /// Set the <see cref="LayoutComponent.Subject"/> property to the <see cref="IImageViewer"/>
    /// that is to be controlled.
    /// </summary>
    [AssociateView(typeof(LayoutComponentViewExtensionPoint))]
	public class LayoutComponent : ImageViewerToolComponent
    {
		private int _imageBoxRows;
        private int _imageBoxColumns;
        private int _tileRows;
        private int _tileColumns;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutComponent(IImageViewer imageViewer)
        {
			this.ImageViewer = imageViewer;
        }

        #region ApplicationComponent overrides

        /// <summary>
        /// Override of <see cref="ApplicationComponent.Start"/>
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Override of <see cref="ApplicationComponent.Stop"/>
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region Public properties and methods for use by the view


        /// <summary>
        /// Indicates to the view whether the image-box section of the user-interface should be enabled
        /// </summary>
        public bool ImageBoxSectionEnabled
        {
            get { return this.ImageViewer != null && this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null; }
        }

        /// <summary>
        /// Indicates to the view whether the tile section of the user-interface should be enabled
        /// </summary>
        public bool TileSectionEnabled
        {
            get { return this.ImageBoxSectionEnabled && this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null; }
        }

        /// <summary>
        /// Gets/sets the number of image box rows
        /// </summary>
        public int ImageBoxRows
        {
            get { return _imageBoxRows; }
            set
			{
				_imageBoxRows = Math.Max(value, 1);
				_imageBoxRows = Math.Min(_imageBoxRows, StoredLayoutConfiguration.MaximumImageBoxRows);
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
				_imageBoxColumns = Math.Max(value, 1);
				_imageBoxColumns = Math.Min(_imageBoxColumns, StoredLayoutConfiguration.MaximumImageBoxColumns);
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
				_tileRows = Math.Max(value, 1);
				_tileRows = Math.Min(_tileRows, StoredLayoutConfiguration.MaximumTileRows);
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
				_tileColumns = Math.Max(value, 1);
				_tileColumns = Math.Min(_tileColumns, StoredLayoutConfiguration.MaximumTileColumns);
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

			IPhysicalWorkspace physicalWorkspace = this.ImageViewer.PhysicalWorkspace;
			UndoableCommand command = new UndoableCommand(physicalWorkspace);
			command.Name = SR.CommandLayoutImageBoxes;
			command.BeginState = physicalWorkspace.CreateMemento();

            physicalWorkspace.SetImageBoxGrid(_imageBoxRows, _imageBoxColumns);

            foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
                imageBox.SetTileGrid(_tileRows, _tileColumns);

            LayoutManager.FillPhysicalWorkspace(physicalWorkspace, physicalWorkspace.LogicalWorkspace);
            physicalWorkspace.Draw();
			physicalWorkspace.SelectDefaultImageBox();

			command.EndState = physicalWorkspace.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);

            OnSubjectChanged();
        }

        /// <summary>
        /// Called by the view to apply the tile layout to the subject
        /// </summary>
        public void ApplyTileLayout()
        {
            if (this.ImageViewer == null)
                return;

			IImageBox imageBox = this.ImageViewer.PhysicalWorkspace.SelectedImageBox;
			UndoableCommand command = new UndoableCommand(imageBox);
			command.Name = SR.CommandLayoutTiles;
			command.BeginState = imageBox.CreateMemento();

            int index = imageBox.TopLeftPresentationImageIndex;

            imageBox.SetTileGrid(_tileRows, _tileColumns);
            imageBox.TopLeftPresentationImageIndex = index;
            imageBox.Draw();
			imageBox.SelectDefaultTile();

			command.EndState = imageBox.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);

            OnSubjectChanged();
        }

        #endregion

		protected override void OnSubjectChanged()
		{
			if (this.ImageViewer != null)
			{
				_imageBoxRows = this.ImageViewer.PhysicalWorkspace.Rows;
				_imageBoxColumns = this.ImageViewer.PhysicalWorkspace.Columns;

				if (this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null)
				{
					_tileRows = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Rows;
					_tileColumns = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Columns;
				}
			}

			base.OnSubjectChanged();
		}
    }
}
