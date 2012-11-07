#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Messages;
using ClearCanvas.Web.Services.View;
using ImageBoxEntity = ClearCanvas.ImageViewer.Web.Common.Entities.ImageBox;
using TileEntity = ClearCanvas.ImageViewer.Web.Common.Entities.Tile;

namespace ClearCanvas.ImageViewer.Web.View
{
	internal class ImageBoxView : WebView<ImageBoxEntity>
	{
		private ImageBox _imageBox;
		private readonly List<TileView> _tileViews = new List<TileView>();

		private int ImageCount
		{
			get { return _imageBox.DisplaySet != null ? _imageBox.DisplaySet.PresentationImages.Count : 0; }	
		}

		private Common.Entities.Tile[] GetTileEntities()
		{
			return _tileViews.Select(v => v.GetEntity()).ToArray();
		}

		public override void SetModelObject(object modelObject)
		{
			_imageBox = (ImageBox)modelObject;
		}

        protected override void Initialize()
        {
            _imageBox.Drawing += OnImageBoxDrawing;
            _imageBox.SelectionChanged += OnSelectionChanged;
            _imageBox.LayoutCompleted += OnLayoutCompleted;

            RefreshTileViews(false);
        }

		protected override void UpdateEntity(ImageBoxEntity entity)
		{
			entity.ImageCount = ImageCount;
			entity.NormalizedRectangle = _imageBox.NormalizedRectangle;
			entity.Selected = _imageBox.Selected;
			entity.TopLeftPresentationImageIndex = _imageBox.TopLeftPresentationImageIndex;
			entity.Tiles = GetTileEntities();
		}

		public void Draw(bool updateProperty)
		{
			foreach (TileView tileView in _tileViews)
				tileView.Draw(false);

            if (updateProperty)
            {
                NotifyEntityPropertyChanged("ImageCount", ImageCount);
                NotifyEntityPropertyChanged("TopLeftPresentationImageIndex", _imageBox.TopLeftPresentationImageIndex);
            }
		}

		private void OnSelectionChanged(object sender, ItemEventArgs<IImageBox> e)
		{
			NotifyEntityPropertyChanged("Selected", e.Item.Selected);
		}

		private void OnImageBoxDrawing(object sender, System.EventArgs e)
		{
			Draw(true);
		}

		private void OnLayoutCompleted(object sender, System.EventArgs e)
		{
			RefreshTileViews(true);
		}

		private void RefreshTileViews(bool notify)
		{
			DisposeTileViews();

			foreach (ITile tile in _imageBox.Tiles)
			{
				var newView = new TileView();
				((IWebView)newView).SetModelObject(tile);
				_tileViews.Add(newView);
			}

			if (!notify)
				return;

			NotifyEntityPropertyChanged("Tiles", GetTileEntities());
			NotifyEntityPropertyChanged("ImageCount", ImageCount);
			NotifyEntityPropertyChanged("TopLeftPresentationImageIndex", _imageBox.TopLeftPresentationImageIndex);
		}

		public override void ProcessMessage(Message message)
		{
            if (message is UpdatePropertyMessage)
            {
                ProcessUpdatePropertyMessage(message as UpdatePropertyMessage);
            }
		}

	    private void ProcessUpdatePropertyMessage(UpdatePropertyMessage message)
	    {
            if (message.PropertyName == "TopLeftPresentationImageIndex")
            {
                Platform.CheckForNullReference(message.Value, "message.Value");
                Platform.CheckForNullReference(_imageBox, "_imageBox");
                int newIndex = (int)message.Value;
                if (newIndex!=_imageBox.TopLeftPresentationImageIndex)
                {
                    _imageBox.TopLeftPresentationImageIndex = newIndex;

					// Assume the image index is only updated by the client
					// when the scroll bar is used. If this is the case, the
					// image box should be selected.
                    if (!_imageBox.Selected)
                        _imageBox.SelectDefaultTile();
                    
                    Draw(false);
                }
	        }
	    }

	    protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _imageBox != null)
			{
				_imageBox.SelectionChanged -= OnSelectionChanged;
				_imageBox.Drawing -= OnImageBoxDrawing;
				_imageBox.LayoutCompleted -= OnLayoutCompleted;
				DisposeTileViews();
				_imageBox = null;
			}
		}

		private void DisposeTileViews()
		{
			foreach (TileView tileView in _tileViews)
				tileView.Dispose();

			_tileViews.Clear();
		}
	}
}
