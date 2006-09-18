using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container of tiles.
	/// </summary>
	/// <remarks>
	/// See <see cref="PhysicalWorkspace"/> for an explanation of how image boxes
	/// help describe the layout of images in a workspace.
	/// </remarks>
	public class ImageBox : IDrawable, IClientArea, IUIEventHandler, IMemorable
	{
		private TileCollection _tiles = new TileCollection();
		private PhysicalWorkspace _parentPhysicalWorkspace;
		private ClientArea _clientArea = new ClientArea();
		private UIEventHandler<Tile> _uiEventHandler;
		private DisplaySet _displaySet;
		private bool _selected = false;
		private int _rows;
		private int _columns;
		private bool _tileLayoutChanged;
		private Tile _selectedTile;
		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;
		private int _drawableInsetSize = 2;
		private bool _linkMode;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageBox"/> class.
		/// </summary>
		public ImageBox()
		{
			_uiEventHandler = new UIEventHandler<Tile>(this.Tiles);
			_tiles.ItemAdded += new EventHandler<TileEventArgs>(OnTileAdded);
			_tiles.ItemRemoved += new EventHandler<TileEventArgs>(OnTileRemoved);
		}

		public TileCollection Tiles
		{
			get { return _tiles; }
		}


		public IImageViewer ParentViewer
		{
			get 
			{
				if (this.ParentPhysicalWorkspace == null)
					return null;

				return this.ParentPhysicalWorkspace.ParentViewer; 
			}
		}

		/// <summary>
		/// Gets this image box's parent <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <value>this image box's parent <see cref="PhysicalWorkspace"/>.</value>
		public PhysicalWorkspace ParentPhysicalWorkspace
		{
			get
			{
				return _parentPhysicalWorkspace;
			}
			internal set
			{
				Platform.CheckForNullReference(value, "ParentWorkspace");
				_parentPhysicalWorkspace = value;
			}
		}

		/// <summary>
		/// Gets this image box's client rectangle.
		/// </summary>
		/// <value>The image box's client rectangle.</value>
		/// <remarks>
		/// The client rectangle is completely determined by the 
		/// <see cref="PhysicalWorkspace.ClientRectangle"/> of the 
		/// <see cref="PhysicalWorkspace"/> and the image box's
		/// <see cref="ImageBox.NormalizedRectangle"/>.
		/// </remarks>
		public Rectangle ClientRectangle
		{
			get
			{
				return _clientArea.ClientRectangle;
			}
		}

		/// <summary>
		/// Gets or sets this image box's normalized rectangle.
		/// </summary>
		/// <value>This image box's normalized rectangle.</value>
		/// <remarks>
		/// An image box's size and position in a workspace is determined by
		/// the workspace's client rectangle and the image box's normalized
		/// rectangle.  A normalized rectangle is one whose top, left, bottom and
		/// right coordinates are expressed as some fraction of 1. Thus, for example, 
		/// an <see cref="ImageBox"/> that occupies the lower right quadrant of a 
		/// workspace is described by the rectangle (0.5, 0.5, 1.0, 1.0).  If the
		/// workspace's client rectangle is (0, 0, 100, 300), the image box's
		/// client coordinates would then be (50, 150, 100, 300).
		/// </remarks>
		public RectangleF NormalizedRectangle
		{
			get
			{
				return _clientArea.NormalizedRectangle;
			}
			set
			{
				Platform.CheckForNullReference(value, "NormalizedRectangle");

				_clientArea.NormalizedRectangle = value;

				foreach (Tile tile in this.Tiles)
					tile.ParentRectangle = _clientArea.ClientRectangle;
			}
		}

		/// <summary>
		/// Gets or sets the drawable inset size in pixels.
		/// </summary>
		/// <value>The drawable inset size in pixels.</value>
		/// <remarks>
		/// Adjacent <see cref="ImageBox"/> client rectangles have no gap between them.
		/// For aesthetic reasons, a small gap is usually preferred; the actual 
		/// <see cref="ImageBox.DrawableClientRectangle"/> should be slightly inset.  
		/// This property determines the size of the inset in pixels.
		/// </remarks>
		public int DrawableInsetSize
		{
			get
			{
				return _drawableInsetSize;
			}
			set
			{
				Platform.CheckNonNegative(value, "DrawableInsetSize");
				_drawableInsetSize = value;
			}
		}

		/// <summary>
		/// Gets the drawable client rectangle.
		/// </summary>
		/// <value>The drawable client rectangle.</value>
		/// <remarks>
		/// Adjacent <see cref="ImageBox"/> client rectangles have no gap between them.
		/// For aesthetic reasons, a small gap is usually preferred; the actual 
		/// <b>DrawableClientRectangle</b> should be slightly inset.  The size of the
		/// inset is determined by <see cref="DrawableInsetSize"/>.
		/// </remarks>
		public Rectangle DrawableClientRectangle
		{
			get
			{
				// The actual drawable area in the image box is slightly
				// smaller than the image box area.  This is to allow the
				// drawing of a border around the image box.
				return Rectangle.Inflate(this.ClientRectangle, -_drawableInsetSize, -_drawableInsetSize);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="DisplaySet"/> associated with this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The <see cref="DisplaySet"/> associated with this <see cref="ImageBox"/>.
		/// <b>null</b> if the <see cref="ImageBox"/> is empty.</value>
		/// <remarks>
		/// Setting this property to a <see cref="DisplaySet"/> automatically populates the tiles
		/// in this <see cref="ImageBox"/> with presentation images contained in the 
		/// <see cref="DisplaySet"/>.  Any <see cref="DisplaySet"/> previously associated with
		/// this <see cref="ImageBox"/> is removed.  Setting this property to <b>null</b>
		/// results in an empty <see cref="ImageBox"/> and empty tiles.
		/// </remarks>
		public DisplaySet DisplaySet
		{
			get
			{
				return _displaySet;
			}
			set
			{
				// Don't bother if the existing value is the same as the new value
				// and if the tile layout hasn't changed.  Note that if the tile
				// layout has changed, then we can't just return, since we
				// need to reassign the images to all the tiles.
				if (_displaySet == value && !_tileLayoutChanged)
				    return;

				if (_displaySet != null)
				{
					// The old DisplaySet should now no longer be visible
					_displaySet.Visible = false;
					// Deselect the old DisplaySet if this ImageBox is selected
					if (this.Selected)
						_displaySet.Selected = false;
				}

				// Assign the new DisplaySet.  Value can be null.
				_displaySet = value;

				// If there's no display set associated with this image box,
				// then make sure there are no images associated with any of
				// its tiles.
				if (_displaySet == null)
				{
					foreach (Tile tile in this.Tiles)
						tile.PresentationImage = null;
				}
				// If there *is* a DisplaySet associated with
				// this ImageBox...
				else
				{
					// The new DisplaySet is now visible.
					_displaySet.Visible = true;
					// Select new DisplaySet if this ImageBox is selected
					if (this.Selected)
						_displaySet.Selected = true;

					// Force the images in the display set to fill
					// the tiles of this image box
					this.TopLeftPresentationImageIndex = 0;
				}
			}
		}

		/// <summary>
		/// Gets the number of rows of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The number of rows of tiles in this <see cref="ImageBox"/>.</value>
		public int Rows
		{
			get
			{
				return _rows;
			}
		}

		/// <summary>
		/// Gets the number of columns of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The number of columns of tiles in this <see cref="ImageBox"/>.</value>
		public int Columns
		{
			get
			{
				return _columns;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="PresentationImage"/> in the top-left 
		/// <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The <see cref="PresentationImage"/> in the top-left 
		/// <see cref="Tile"/> of this <see cref="ImageBox"/>.</value>
		/// <remarks>
		/// Because a <see cref="DisplaySet"/> is an <i>ordered</i> set of 
		/// presentation images, setting this property to a specified
		/// <see cref="PresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentException"><b>TopLeftPresentationImage</b>
		/// is not found this image box's <see cref="DisplaySet"/></exception>
		public PresentationImage TopLeftPresentationImage
		{
			get
			{
				if (this.Tiles.Count == 0)
					return null;

				Tile tile = this.Tiles[0];

				return tile.PresentationImage;
			}
			set
			{
				Platform.CheckForNullReference(value, "TopLeftPresentationImage");

				Platform.CheckMemberIsSet(this.DisplaySet, "DisplaySet");

				// If specified presentationImage cannot be found in DisplaySet, an
				// exception will be thrown in DisplaySet.IndexOfPresentationImage
				int index = _displaySet.PresentationImages.IndexOf(value);

				FlowImages(index);
			}
		}

		/// <summary>
		/// Gets or sets the index of the <see cref="PresentationImage"/> that is
		/// to be placed in the top-left <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>
		/// The index of the <see cref="PresentationImage"/> that is 
		/// to be placed in the top-left <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </value>
		/// <remarks>
		/// The index is the index of the <see cref="PresentationImage"/> in the
		/// <see cref="DisplaySet"/>.  Because a <see cref="DisplaySet"/> is an 
		/// <i>ordered</i> set of presentation images, setting this property to a specified
		/// <see cref="PresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><b>TopLeftPresentationImageIndex</b> is
		/// less than 0 or greater than or equal to the number of presentation images in this
		/// image box's <see cref="DisplaySet"/></exception>
		public int TopLeftPresentationImageIndex
		{
			get
			{
				if (this.TopLeftPresentationImage == null)
					return -1;
				else
					return _displaySet.PresentationImages.IndexOf(this.TopLeftPresentationImage);
			}
			set
			{
				Platform.CheckMemberIsSet(this.DisplaySet, "DisplaySet");

				int index;

				if (value < 0)
					index = 0;
				else if (value > this.DisplaySet.PresentationImages.Count - 1)
					index = this.DisplaySet.PresentationImages.Count - 1;
				else
					index = value;

				FlowImages(index);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ImageBox"/> is
		/// selected.
		/// </summary>
		/// <value><b>true</b> if selected; <b>false</b> otherwise.</value>
		/// <remarks>
		/// <see cref="ImageBox"/> selection is mutually exclusive, i.e., only one
		/// <see cref="ImageBox"/> is ever selected at a given time.  
		/// </remarks>
		public bool Selected
		{
			get { return _selected; }
			set
			{
				Platform.CheckMemberIsSet(this.ParentPhysicalWorkspace, "ImageBox.ParentPhysicalWorkspace");

				if (_selected != value)
				{
					//string str = String.Format("ImageBox.SetSelected({0})\n", selected.ToString());
					//Trace.Write(str);

					// If there's no DisplaySet associated with this ImageBox,
					// then there's nothing to select/deselect, so don't change anything
					if (_displaySet != null)
					{
						_selected = value;

						// If ImageBox is being selected...
						if (_selected)
						{
							_parentPhysicalWorkspace.SelectedImageBox = this;
							
							// Tell whoever wants to know that this ImageBox has been selected
							if (this.ParentViewer != null)
							{
								this.ParentViewer.EventBroker.OnImageBoxSelected(
									new ImageBoxSelectedEventArgs(this));
							}

							// Select the DisplaySet in this ImageBox
							this.DisplaySet.Selected = true;
						}

						// If the ImageBox is being deselected, make sure we
						// deselect its associated DisplaySet and any selected Tiles.
						if (!_selected && _selectedTile != null)
						{
							this.DisplaySet.Selected = false;
							_selectedTile.Selected = false;
							_selectedTile = null;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="Tile"/>.
		/// </summary>
		/// <value>The currently selected <see cref="Tile"/>.</value>
		public Tile SelectedTile
		{
			get { return _selectedTile; }
			internal set
			{
				// Deselect the old Tile
				if (_selectedTile != null)
					_selectedTile.Selected = false;

				// Select this ImageBox
				this.Selected = true;

				_selectedTile = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="Tile"/> at the specified row and column.
		/// </summary>
		/// <value>The <see cref="Tile"/> at the specified row and column.</value>
		/// <exception cref="IndexOutOfRangeException"><pararef name="row"/> or
		/// <pararef name="column"/> is less than 0 or is greater than or equal to 
		/// the <see cref="ImageBox.Rows"/> or <see cref="ImageBox.Columns"/> respectively.
		/// </exception>
		public Tile this[int row, int column]
		{
			get
			{
				Platform.CheckIndexRange(row, 0, this.Rows - 1, this);
				Platform.CheckIndexRange(column, 0, this.Columns - 1, this);

				int index = row * this.Columns + column;
				return this.Tiles[index];
			}
		}

		internal event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add { _imageDrawingEvent += value; }
			remove { _imageDrawingEvent -= value; }
		}

		/// <summary>
		/// Creates an rectangular grid of tiles.
		/// </summary>
		/// <remarks>
		/// Each time this method is called, existing tiles in the <see cref="ImageBox"/>
		/// are removed and new ones added.  The exception is when the number of rows
		/// and columns has not changed, in which case the method does nothing
		/// and returns immediately.
		/// </remarks>
		/// <param name="numberOfRows">Number of <see cref="ImageBox"/> rows.</param>
		/// <param name="numberOfColumns">Number of <see cref="ImageBox"/> columns.</param>
		/// <exception cref="ArgumentException"><paramref name="numberOfRows"/> or 
		/// <paramref name="numberOfColumns"/> is less than 1.</exception>
		public void SetTileGrid(int numberOfRows, int numberOfColumns)
		{
			Platform.CheckPositive(numberOfRows, "numberOfRows");
			Platform.CheckPositive(numberOfColumns, "numberOfColumns");

			// Don't bother if nothing's changed.
			if (numberOfRows == _rows &&
				numberOfColumns == _columns)
				return;

			ParentPhysicalWorkspace.ReleaseMouseCapture();

			_rows = numberOfRows;
			_columns = numberOfColumns;

			this.Tiles.Clear();

			double tileWidth = 1.0d / numberOfColumns;
			double tileHeight = 1.0d / numberOfRows;

			for (int row = 0; row < numberOfRows; row++)
			{
				for (int column = 0; column < numberOfColumns; column++)
				{
					double x = column * tileWidth;
					double y = row * tileHeight;
					RectangleF rect = new RectangleF((float)x, (float)y, (float)tileWidth, (float)tileHeight);

					Tile tile = new Tile();
					tile.NormalizedRectangle = rect;
					this.Tiles.Add(tile);
				}
			}
		}

		internal Rectangle ParentRectangle
		{
			get
			{
				return _clientArea.ParentRectangle;
			}
			set
			{
				Platform.CheckForNullReference(value, "ParentRectangle");

				_clientArea.ParentRectangle = value;

				foreach (Tile tile in this.Tiles)
					tile.ParentRectangle = this.DrawableClientRectangle;
			}
		}

		//public override void Cleanup()
		//{
		//    this.RemoveAllTiles();
		//}

		#region IDrawable

		/// <summary>
		/// Draws all currently visible images in this <see cref="ImageBox"/>.
		/// </summary>
		/// <param name="paintNow">If <b>true</b>, each image rectangle is invalidated and
		/// repainted immediately.  If <b>false</b>, each image rectangle is still
		/// invalidated but when the actual painting occurs is left to the the .NET 
		/// framework.</param>
		/// <remarks>
		/// The semantics of <paramref name="paintNow"/> described above are only
		/// guaranteed in the standard implementation of ClearCanvas for presentation images
		/// that do not implement the <see cref="ICustomDrawable"/> interface.  How
		/// a custom drawable image decides to interpret <paramref name="paintNow"/>
		/// is entirely up to the implementor.
		/// </remarks>
		public void Draw(bool paintNow)
		{
			foreach (Tile tile in this.Tiles)
				tile.Draw(paintNow);
		}

		#endregion

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			
			e.SelectedImageBox = this;

			if (e.ModifierKeys == XKeys.Control)
			{
				if (this.DisplaySet != null)
				{
					_linkMode = true;
					this.DisplaySet.Linked = !this.DisplaySet.Linked;
					Draw(false);
					return true;
				}
			}

			return _uiEventHandler.OnMouseDown(e);
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (_linkMode)
				return true;

			e.SelectedImageBox = this;

			return _uiEventHandler.OnMouseMove(e);
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (_linkMode)
			{
				_linkMode = false;
				return true;
			}

			e.SelectedImageBox = this;

			return _uiEventHandler.OnMouseUp(e);
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			e.SelectedImageBox = this;

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

		/// <summary>
		/// Creates a memento for this <see cref="ImageBox"/>.
		/// </summary>
		/// <returns>A memento for this <see cref="ImageBox"/>.</returns>
		/// <remarks>
		/// This method is used to remember the current state of a
		/// <see cref="ImageBox"/>.  The memento remembers the actual <see cref="Tile"/>
		/// <i>instances</i> contained in the <see cref="ImageBox"/>.  Calling
		/// <see cref="ImageBox.SetMemento"/> at a later time restores those instances.
		/// </remarks>
		public IMemento CreateMemento()
		{
			// When creating the memento, we have to remember the
			// display set, client area, rows, columns, tiles mementos AND
			// the tiles themselves.  We have to remember the actual
			// instances of the tiles, since command objects
			// have references to them.  If during image box reconstitution
			// we simply created new tiles and restored their state with 
			// tile mementos, those command objects, when undone/redone,
			// would be operating on objects that are simply floating around
			// on the heap and not part of the logical workspace tree, and thus
			// would not work.  A similar argument exists for 
			// PhysicalWorkspace.CreateMemento.
			MementoList tileMementos = new MementoList();

			foreach (Tile tile in this.Tiles)
				tileMementos.AddMemento(tile.CreateMemento());

			ImageBoxMemento imageBoxMemento = 
				new ImageBoxMemento(_displaySet, 
									_clientArea, 
									_rows, 
									_columns, 
									new TileCollection(this.Tiles),
									tileMementos);

			return imageBoxMemento;
		}

		/// <summary>
		/// Sets this <see cref="ImageBox"/> with a previously created memento.
		/// </summary>
		/// <param name="memento">Memento to set.</param>
		/// <remarks>
		/// This method restores the state of a <see cref="ImageBox"/> with
		/// a memento previously created by <see cref="ImageBox.CreateMemento"/>.
		/// </remarks>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			ImageBoxMemento imageBoxMemento = memento as ImageBoxMemento;

			Platform.CheckForInvalidCast(imageBoxMemento, "memento", "ImageBoxMemento");

			this.Tiles.Clear();

			_displaySet = imageBoxMemento.DisplaySet;
			_clientArea = imageBoxMemento.ClientArea;
			_rows = imageBoxMemento.Rows;
			_columns = imageBoxMemento.Columns;

			for (int i = 0; i < imageBoxMemento.Tiles.Count; i++)
			{
				IMemento tileMemento = imageBoxMemento.TileMementos[i];
				Tile tile = imageBoxMemento.Tiles[i];
				tile.SetMemento(tileMemento);
				this.Tiles.Add(tile);
			}
		}

		#endregion

		private void FlowImages(int startIndex)
		{
			Platform.CheckArgumentRange(startIndex, 0, this.DisplaySet.PresentationImages.Count, "startIndex");

			int index;
			int maxStartIndex = _displaySet.PresentationImages.Count - this.Tiles.Count;

			// Case when there are as many or more images than tiles
			if (maxStartIndex >= 0)
			{
				if (startIndex > maxStartIndex)
					index = maxStartIndex;
				else
					index = startIndex;
			}
			// Case when there are fewer images than tiles
			else
			{
				index = 0;
			}

			foreach (Tile tile in this.Tiles)
			{
				// If there's an image, put it in a tile
				if (index < _displaySet.PresentationImages.Count)
					tile.PresentationImage = this.DisplaySet.PresentationImages[index];
				// If there are no images left (the case when there are fewer images than tiles)
				// then just set the tile to blank
				else
					tile.PresentationImage = null;

				index++;
			}
		}

		private void OnTileAdded(object sender, TileEventArgs e)
		{
			e.Tile.ParentImageBox = this;

			if (this.DrawableClientRectangle.Width >= 0 && this.DrawableClientRectangle.Height >= 0)
				e.Tile.ParentRectangle = this.DrawableClientRectangle;

			e.Tile.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
			_tileLayoutChanged = true;
		}

		private void OnTileRemoved(object sender, TileEventArgs e)
		{
			e.Tile.ImageDrawing -= new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
			_tileLayoutChanged = true;

			if (e.Tile.Selected)
				this.SelectedTile = null;
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			e.ImageBox = this;

			if (_tileLayoutChanged)
			{
				e.TileLayoutChanged = true;
				_tileLayoutChanged = false;
			}

			EventsHelper.Fire(_imageDrawingEvent, this, e);
		}
	}
}