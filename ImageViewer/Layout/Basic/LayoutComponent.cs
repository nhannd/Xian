using System;
using System.Collections.Generic;
using System.Text;

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
    public class LayoutComponent : ApplicationComponent
    {
        private int _imageBoxRows;
        private int _imageBoxColumns;
        private int _tileRows;
        private int _tileColumns;
        private event EventHandler _layoutSubjectChanged;

        private IImageViewer _imageViewer;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutComponent()
        {
        }

        /// <summary>
        /// Gets/sets the subject <see cref="IImageViewer"/> that this component is associated with.  Note that
        /// null is a valid value.  Setting this property to null dissociates it from any image viewer.
        /// </summary>
        public IImageViewer Subject
        {
            get { return _imageViewer; }
            set
            {
                if (value != _imageViewer)
                {
                    // stop listening to the old image viewer, if one was set
                    if (_imageViewer != null)
                    {
                        _imageViewer.EventBroker.TileSelected -= TileSelectedEventHandler;
                    }

                    _imageViewer = value;

                    // start listening to the new image viewer, if one has been set
                    if (_imageViewer != null)
                    {
						_imageViewer.EventBroker.TileSelected += TileSelectedEventHandler;
                    }
                    UpdateFromImageViewer();
                }
            }
        }

        #region ApplicationComponent overrides

        /// <summary>
        /// Override of <see cref="ApplicationComponent.Start"/>
        /// </summary>
        public override void Start()
        {
            base.Start();

            UpdateFromImageViewer();
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
        /// Notifies the view that the layout subject has changed.  The view should
        /// refresh itself entirely to reflect the state of this component.
        /// </summary>
        public event EventHandler LayoutSubjectChanged
        {
            add { _layoutSubjectChanged += value; }
            remove { _layoutSubjectChanged -= value; }
        }

        /// <summary>
        /// Indicates to the view whether the image-box section of the user-interface should be enabled
        /// </summary>
        public bool ImageBoxSectionEnabled
        {
            get { return _imageViewer != null; }
        }

        /// <summary>
        /// Indicates to the view whether the tile section of the user-interface should be enabled
        /// </summary>
        public bool TileSectionEnabled
        {
            get { return this.ImageBoxSectionEnabled && _imageViewer.PhysicalWorkspace.SelectedImageBox != null; }
        }

        /// <summary>
        /// Gets/sets the number of image box rows
        /// </summary>
        public int ImageBoxRows
        {
            get { return _imageBoxRows; }
            set { _imageBoxRows = value; }
        }

        /// <summary>
        /// Gets/sets the number of image box columns
        /// </summary>
        public int ImageBoxColumns
        {
            get { return _imageBoxColumns; }
            set { _imageBoxColumns = value; }
        }

        /// <summary>
        /// Gets/sets the number of tile rows
        /// </summary>
        public int TileRows
        {
            get { return _tileRows; }
            set { _tileRows = value; }
        }

        /// <summary>
        /// Gets/sets the number of tile columns
        /// </summary>
        public int TileColumns
        {
            get { return _tileColumns; }
            set { _tileColumns = value; }
        }

        /// <summary>
        /// Called by the view to apply the image layout to the subject
        /// </summary>
        public void ApplyImageBoxLayout()
        {
            if (_imageViewer == null)
                return;

			IPhysicalWorkspace physicalWorkspace = _imageViewer.PhysicalWorkspace;
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

			_imageViewer.CommandHistory.AddCommand(command);

            // need to update the controls since no tile will be selected now
            UpdateFromImageViewer();
        }

        /// <summary>
        /// Called by the view to apply the tile layout to the subject
        /// </summary>
        public void ApplyTileLayout()
        {
            if (_imageViewer == null)
                return;

			IImageBox imageBox = _imageViewer.PhysicalWorkspace.SelectedImageBox;
			UndoableCommand command = new UndoableCommand(imageBox);
			command.Name = SR.CommandLayoutTiles;
			command.BeginState = imageBox.CreateMemento();

            int index = imageBox.TopLeftPresentationImageIndex;

            imageBox.SetTileGrid(_tileRows, _tileColumns);
            imageBox.TopLeftPresentationImageIndex = index;
            imageBox.Draw();
			imageBox.SelectDefaultTile();

			command.EndState = imageBox.CreateMemento();

			_imageViewer.CommandHistory.AddCommand(command);

            // need to update the controls since no tile will be selected now
            UpdateFromImageViewer();
        }

        #endregion

        #region Internal methods and event handlers

        /// <summary>
        /// Updates the component in response to a change in the selected image box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TileSelectedEventHandler(object sender, TileSelectedEventArgs e)
        {
            UpdateFromImageViewer();
        }

        /// <summary>
        /// Updates this component to reflect the state of the currently selected
        /// image box in the subject image viewer.
        /// </summary>
        private void UpdateFromImageViewer()
        {
            if (_imageViewer != null)
            {
                _imageBoxRows = _imageViewer.PhysicalWorkspace.Rows;
                _imageBoxColumns = _imageViewer.PhysicalWorkspace.Columns;

                if (_imageViewer.PhysicalWorkspace.SelectedImageBox != null)
                {
                    _tileRows = _imageViewer.PhysicalWorkspace.SelectedImageBox.Rows;
                    _tileColumns = _imageViewer.PhysicalWorkspace.SelectedImageBox.Columns;
                }
            }
            EventsHelper.Fire(_layoutSubjectChanged, this, new EventArgs());
        }

        #endregion
    }
}
