using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class Tile : IDrawable, IClientArea, IUIEventHandler, IMemorable
	{
		private PresentationImageCollection _presentationImages = new PresentationImageCollection();
		private ImageBox _parentImageBox;
		private ClientArea _clientArea = new ClientArea();
		private UIEventHandler<PresentationImage> _uiEventHandler;
		private bool _selected = false;
		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;
		private int _drawableInsetSize = 4;

		// Constructor
		public Tile()
		{
			// Add a single null reference; this will be the reference to an PresentationImage
			_presentationImages.Add(null);
			_uiEventHandler = new UIEventHandler<PresentationImage>(this._presentationImages);
		}

        public IImageViewer ParentViewer
		{
			get 
			{
				if (this.ParentImageBox == null)
					return null;

				return this.ParentImageBox.ParentViewer; 
			}
		}

		public ImageBox ParentImageBox
		{
			get
			{
				Platform.CheckMemberIsSet(_parentImageBox, "ParentImageBox");
				return _parentImageBox;
			}
			internal set
			{
				Platform.CheckForNullReference(value, "ParentImageBox");
				_parentImageBox = value;
			}
		}

		public Rectangle ClientRectangle
		{
			get
			{
				return _clientArea.ClientRectangle;
			}
		}

		public int DrawableInsetSize
		{
			get { return _drawableInsetSize; }
			set
			{
				Platform.CheckNonNegative(value, "DrawableInsetSize");
				_drawableInsetSize = value;
			}
		}

		public Rectangle DrawableClientRectangle
		{
			get
			{
				// The actual drawable area in the tile is slightly
				// smaller than the tile area.  This is to allow the
				// drawing of a border around the tile.
				return Rectangle.Inflate(this.ClientRectangle, -_drawableInsetSize, -_drawableInsetSize);
			}
		}

		public PresentationImage PresentationImage
		{
			get
			{
				PresentationImage presentationImage = _presentationImages[0];
				return presentationImage;
			}
			set
			{
				// We don't check for null, since image can be null

				// Don't bother if the existing value is the same as the new value
				if (this.PresentationImage == value)
					return;

				if (this.PresentationImage != null)
				{
					// Unsubscribe from the old PresentationImage's ImageDrawing event
					// before we assign a new PresentationImage
					this.PresentationImage.ImageDrawing -= new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
					// Unselect the old PresentationImage if this Tile is selected
					if (this.Selected)
						this.PresentationImage.Selected = false;
				}

				// Assign the new PresentationImage.  Value can be null.
				_presentationImages[0] = value;

				// Verify that there's a PresentationImage associated with this Tile
				// (there doesn't have to be)
				if (this.PresentationImage != null)
				{
					// Subscribe to its ImageDrawing event
					this.PresentationImage.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);

					// Select the new PresentationImage if this Tile is selected
					if (this.Selected)
						this.PresentationImage.Selected = true;
				}
			}
		}

		public int PresentationImageIndex
		{
			get
			{
				Platform.CheckMemberIsSet(this.PresentationImage, "PresentationImage", SR.ExceptionNoPresentationImageAssociatedWithTile);
				Platform.CheckMemberIsSet(this.ParentImageBox.DisplaySet, "ParentImageBox.DisplaySet", SR.ExceptionNoDisplaySetAssociatedWithImageBoxTile);

				DisplaySet displaySet = this.ParentImageBox.DisplaySet;

				return displaySet.PresentationImages.IndexOf(this.PresentationImage);
			}
			set
			{
				Platform.CheckMemberIsSet(this.ParentImageBox.DisplaySet, "Tile.ParentImageBox.DisplaySet", SR.ExceptionNoDisplaySetAssociatedWithImageBoxTile);

				DisplaySet displaySet = this.ParentImageBox.DisplaySet;

				int index;

				// Validate the index.  If it's out of range, limit it to the min and max
				if (value < 0)
					index = 0;
				else if (value > displaySet.PresentationImages.Count - 1)
					index = displaySet.PresentationImages.Count - 1;
				else
					index = value;

				this.PresentationImage = displaySet.PresentationImages[index];
			}
		}

		public bool Selected
		{
			get { return _selected; }
			set
			{
				Platform.CheckMemberIsSet(this.ParentImageBox, "Tile.ParentImageBox");

				if (_selected != value)
				{
					//string str = String.Format("Tile.SetSelected({0})\n", selected.ToString());
					//Trace.Write(str);

					// If there's no PresentationImage associated with this Tile,
					// then there's nothing to select/deselect, so don't change anything
					if (this.PresentationImage != null)
					{
						_selected = value;

						// If the tile is being selected...
						if (_selected)
						{
							this.ParentImageBox.SelectedTile = this;
							// Tell whoever wants to know that this Tile has been selected
							this.ParentViewer.EventBroker.OnTileSelected(
								new TileSelectedEventArgs(this));
							// Select the PresentationImage in this Tile
							this.PresentationImage.Selected = true;
						}
						// If the tile is being deselected
						else
						{
							// Deselect the PresentationImage in this Tile
							this.PresentationImage.Selected = false;
						}

						this.PresentationImage.DrawLayers(true);
					}
				}
			}
		}

		public event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add { _imageDrawingEvent += value; }
			remove { _imageDrawingEvent -= value; }
		}

		internal Rectangle ParentRectangle
		{
			get
			{
				return _clientArea.ParentRectangle;
			}
			set
			{
				_clientArea.ParentRectangle = value;

				// Tell the presentation image when the tile size changes
				if (this.PresentationImage != null)
					this.PresentationImage.LayerManager.RootLayerGroup.SpatialTransform.DestinationRectangle = this.DrawableClientRectangle;

				//this.PresentationImage.LayerManager.RootLayerGroup.DestinationRectangle = this.DrawableClientRectangle;
			}
		}

		internal RectangleF NormalizedRectangle
		{
			get
			{
				return _clientArea.NormalizedRectangle;
			}
			set
			{
				_clientArea.NormalizedRectangle = value;

				// Tell the presentation image when the tile size changes
				if (this.PresentationImage != null)
					this.PresentationImage.LayerManager.RootLayerGroup.SpatialTransform.DestinationRectangle = this.DrawableClientRectangle;
				//	this.PresentationImage.LayerManager.RootLayerGroup.DestinationRectangle = this.DrawableClientRectangle;
			}
		}

		#region IDrawable

		public void Draw(bool paintNow)
		{
			if (this.PresentationImage != null)
				this.PresentationImage.Draw(paintNow);
			else
				OnImageDrawing(this, new ImageDrawingEventArgs(null, paintNow));
		}

		#endregion

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.PresentationImage == null)
				return true;

			this.Selected = true;

			e.SelectedTile = this;

			return _uiEventHandler.OnMouseDown(e);
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.PresentationImage == null)
				return true;

			e.SelectedTile = this;

			return _uiEventHandler.OnMouseMove(e);
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.PresentationImage == null)
				return true;

			e.SelectedTile = this;

			return _uiEventHandler.OnMouseUp(e);
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.PresentationImage == null)
				return true;

			e.SelectedTile = this;

			return _uiEventHandler.OnMouseWheel(e);
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnKeyDown(e);
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnKeyUp(e);
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			// For the memento, we'll remember what PresentationImage was in this tile
			// and the location and dimensions of the tile itself
			TileMemento tileMemento = new TileMemento(this.PresentationImage, _clientArea);

			IMemento memento = tileMemento as IMemento;
			Platform.CheckForInvalidCast(memento, "tileMemento", "IMemento");

			return memento;
		}

		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			TileMemento tileMemento = memento as TileMemento;
			Platform.CheckForInvalidCast(tileMemento, "memento", "TileMemento");

			this.PresentationImage = tileMemento.PresentationImage;
			_clientArea = tileMemento.ClientArea;
		}

		#endregion

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			e.Tile = this;

			// Set the destination rectangle in the layers and calculate the transform
			// It's unfortunate that we need to do this here, as this really should
			// be set in this.PresentationImage.  The problem arises when the same
			// presentation image is hosted by more than one tile.  The destination
			// rectangles of the tiles will obviously differ but the single
			// presentation image can only store one rectangle and that 
			// leads to strange results when the tiles are drawn.
			if (e.PresentationImage != null)
			    e.PresentationImage.LayerManager.RootLayerGroup.DestinationRectangle = this.DrawableClientRectangle;

			EventsHelper.Fire(_imageDrawingEvent, this, e);
		}

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				if (ImageViewerComponent.AnnotationManager != null)
					return ImageViewerComponent.AnnotationManager.GetAnnotationBoxes(PresentationImage);

				return null;
			}
		}
	}
}