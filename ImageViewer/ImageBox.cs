using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Extension point for views onto <see cref="TileComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ImageBoxViewExtensionPoint : ExtensionPoint<IView>
	{
	}

	/// <summary>
	/// TileComponent class
	/// </summary>
	[AssociateView(typeof(ImageBoxViewExtensionPoint))]
	public class ImageBox : IImageBox
	{
		#region Private Fields
		
		private TileCollection _tiles;
		private IImageViewer _imageViewer; 
		private PhysicalWorkspace _parentPhysicalWorkspace;
		private DisplaySet _displaySet;
		private Tile _selectedTile;
		private RectangleF _normalizedRectangle;
		private bool _selected = false;
		private int _rows;
		private int _columns;
		private bool _layoutRefreshRequired;

		private event EventHandler _drawingEvent;
		private event EventHandler<ImageBoxEventArgs> _selectionChangedEvent;
		private event EventHandler<TileEventArgs> _tileAddedEvent;
		private event EventHandler<TileEventArgs> _tileRemovedEvent;

		#endregion
		
		public ImageBox()
		{
			this.Tiles.ItemAdded += new EventHandler<TileEventArgs>(OnTileAdded);
			this.Tiles.ItemRemoved += new EventHandler<TileEventArgs>(OnTileRemoved);
		}

		#region Public properties

		public TileCollection Tiles
		{
			get 
			{
				if (_tiles == null)
					_tiles = new TileCollection();

				return _tiles; 
			}
		}

		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set 
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (Tile tile in this.Tiles)
						tile.ImageViewer = _imageViewer;

					if (_displaySet != null)
						_displaySet.ImageViewer = _imageViewer;
				}
			}
		}

		/// <summary>
		/// Gets this image box's parent <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <value>this image box's parent <see cref="PhysicalWorkspace"/>.</value>
		public IPhysicalWorkspace ParentPhysicalWorkspace
		{
			get { return _parentPhysicalWorkspace as IPhysicalWorkspace; }
			internal set { _parentPhysicalWorkspace = value as PhysicalWorkspace; }
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set { _normalizedRectangle = value; }
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
		public IDisplaySet DisplaySet
		{
			get { return _displaySet as IDisplaySet; }
			set
			{
				// Don't bother if the existing value is the same as the new value
				// and if the tile layout hasn't changed.  Note that if the tile
				// layout has changed, then we can't just return, since we
				// need to reassign the images to all the tiles.
				if (_displaySet == value /*&& !_tileLayoutChanged*/)
					return;

				//if (value.PresentationImages.Count == 0)
				//	throw new InvalidOperationException("A DisplaySet must have at least one PresentationImage in it before it can be put in an ImageBox");

				// Break association with the old display set (should we dispose too?)
				if (_displaySet != null)
				{
					_displaySet.ImageBox = null;
					_displaySet.Selected = false;
				}

				// Assign the new DisplaySet.  Value can be null.
				_displaySet = value as DisplaySet;

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
					_displaySet.ImageBox = this;
					_displaySet.Selected = this.Selected;

					if (_displaySet.ImageViewer == null)
						_displaySet.ImageViewer = this.ImageViewer;

					// Force the images in the display set to fill
					// the tiles of this image box
					this.TopLeftPresentationImageIndex = 0;
				}

			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ImageBox"/> is
		/// selected.
		/// </summary>
		/// <value><b>true</b> if selected; <b>false</b> otherwise.</value>
		/// <remarks>
		/// An <see cref="ImageBox"/> is selected when 
		/// <see cref="ImageBox"/> selection is mutually exclusive, i.e., only one
		/// <see cref="ImageBox"/> is ever selected at a given time.  
		/// </remarks>
		public bool Selected
		{
			get { return _selected; }
			private set 
			{
				if (_selected != value)
				{
					_selected = value;
					EventsHelper.Fire(_selectionChangedEvent, this, new ImageBoxEventArgs(this));
				}
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="Tile"/>.
		/// </summary>
		/// <value>The currently selected <see cref="Tile"/>.</value>
		public ITile SelectedTile
		{
			get { return _selectedTile; }
			internal set
			{
				// Deselect the old Tile
				if (_selectedTile != null)
					_selectedTile.Deselect();

				_selectedTile = value as Tile;

				if (_selectedTile == null)
					return;

				// Select this ImageBox
				Select();
			}
		}

		public Color BorderColor
		{
			get
			{
				// TODO: remove these hard codes and make this configurable
				if (this.Selected)
					return Color.White;
				else
					return Color.DarkGray;
			}
		}

		public int BorderWidth
		{
			get { return 2; }
		}

		public int InsetWidth
		{
			get { return 5; }
		}


		/// <summary>
		/// Gets the number of rows of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The number of rows of tiles in this <see cref="ImageBox"/>.</value>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The number of columns of tiles in this <see cref="ImageBox"/>.</value>
		public int Columns
		{
			get { return _columns; }
		}

		public bool LayoutRefreshRequired
		{
			get { return _layoutRefreshRequired; }
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
		public IPresentationImage TopLeftPresentationImage
		{
			get
			{
				if (this.Tiles.Count == 0)
					return null;

				ITile tile = this.Tiles[0];

				return tile.PresentationImage;
			}
			set
			{
				Platform.CheckForNullReference(value, "TopLeftPresentationImage");

				Platform.CheckMemberIsSet(this.DisplaySet, "DisplaySet");

				// If specified presentationImage cannot be found in DisplaySet, an
				// exception will be thrown in DisplaySet.IndexOfPresentationImage
				int imageIndex = _displaySet.PresentationImages.IndexOf(value);

				FlowImages(imageIndex);
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
				Platform.CheckPositive(this.DisplaySet.PresentationImages.Count, "ImageBox.DisplaySet.PresentationImages.Count");

				int imageIndex;

				if (value < 0)
					imageIndex = 0;
				else if (value > this.DisplaySet.PresentationImages.Count - 1)
					imageIndex = this.DisplaySet.PresentationImages.Count - 1;
				else
					imageIndex = value;

				FlowImages(imageIndex);
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
		public ITile this[int row, int column]
		{
			get
			{
				Platform.CheckIndexRange(row, 0, this.Rows - 1, this);
				Platform.CheckIndexRange(column, 0, this.Columns - 1, this);

				int index = row * this.Columns + column;
				return this.Tiles[index];
			}
		}

		#endregion

		#region Public events

		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		public event EventHandler<ImageBoxEventArgs> SelectionChanged
		{
			add { _selectionChangedEvent += value; }
			remove { _selectionChangedEvent -= value; }
		}

		public event EventHandler<TileEventArgs> TileAdded
		{
			add { _tileAddedEvent += value; }
			remove { _tileAddedEvent -= value; }
		}

		public event EventHandler<TileEventArgs> TileRemoved
		{
			add { _tileRemovedEvent += value; }
			remove { _tileRemovedEvent -= value; }
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeTiles();
			}
		}

		private void DisposeTiles()
		{
			if (this.Tiles == null)
				return;

			foreach (Tile tile in this.Tiles)
				tile.Dispose();

			_tiles = null;
		}

		#endregion 

		#region Public methods

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

		public void Draw()
		{
			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
			_layoutRefreshRequired = false;
		}

		private void Select()
		{
			if (!this.Selected)
			{
				Platform.CheckMemberIsSet(this.DisplaySet, "ImageBox.DisplaySet");
				Platform.CheckMemberIsSet(this.ParentPhysicalWorkspace, "ImageBox.ParentPhysicalWorkspace");
				Platform.CheckMemberIsSet(this.ImageViewer, "ImageBox.ImageViewer");

				this.Selected = true;
				_displaySet.Selected = true;
				_parentPhysicalWorkspace.SelectedImageBox = this;
				this.ImageViewer.EventBroker.OnImageBoxSelected(new ImageBoxSelectedEventArgs(this));
			}
		}

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

			foreach (ITile tile in this.Tiles)
				tileMementos.AddMemento(tile.CreateMemento());

			ImageBoxMemento imageBoxMemento =
					new ImageBoxMemento(this.DisplaySet,
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

			for (int i = 0; i < imageBoxMemento.Tiles.Count; i++)
			{
				IMemento tileMemento = imageBoxMemento.TileMementos[i];
				ITile tile = imageBoxMemento.Tiles[i];
				tile.SetMemento(tileMemento);
				this.Tiles.Add(tile);
			}

			this.DisplaySet = imageBoxMemento.DisplaySet;
			_rows = imageBoxMemento.Rows;
			_columns = imageBoxMemento.Columns;
		}

		#endregion

		#endregion

		#region Internal/private methods

		internal void Deselect()
		{
			if (this.Selected)
			{
				Platform.CheckMemberIsSet(this.DisplaySet, "ImageBox.DisplaySet");

				this.Selected = false;
				_displaySet.Selected = false;

				if (this.SelectedTile != null)
				{
					_selectedTile.Deselect();
					_selectedTile = null;
				}
			}
		}

		private void FlowImages(int imageIndex)
		{
			Platform.CheckArgumentRange(imageIndex, 0, this.DisplaySet.PresentationImages.Count, "startIndex");

			int startImageIndex;
			int maxStartImageIndex = _displaySet.PresentationImages.Count - this.Tiles.Count;

			// Case when there are as many or more images than tiles
			if (maxStartImageIndex >= 0)
			{
				if (imageIndex > maxStartImageIndex)
					startImageIndex = maxStartImageIndex;
				else
					startImageIndex = imageIndex;
			}
			// Case when there are fewer images than tiles
			else
			{
				startImageIndex = 0;
			}

			// If the starting index is less than the top-left index, or if there's no
			// image in the top left tile yet, then we iterate through the tiles in ascending order.
			// Otherwise, we iterate through in descending order.
			// The reason is this:  Consider the example when we have 4 images in 4 tiles, where
			// the number corresponds to the image, and the position in the list corresponds to the
			// tile in which the images resides.  If we want to reflow the images so that the first image is 3,
			// this is what the list would look like after each iteration:
			//
			// 2,3,4,5
			// 3,3,4,5
			// 3,4,4,5
			// 3,4,5,5
			// 3,4,5,6
			//
			// The problem here is that at the end of any given iteration, the same image can be
			// in two tiles simultaneously.  This causes a number synchronization problems.  What we want
			// is for there to ever only be one instance of an image in the current tiles.
			// The way to do this is to iterate through the list backwards like this:
			//
			// 2,3,4,5
			// 2,3,4,6
			// 2,3,5,6
			// 2,4,5,6
			// 3,4,5,6
			if (startImageIndex <= this.TopLeftPresentationImageIndex ||
				this.TopLeftPresentationImageIndex == -1)
			{
				for (int tileIndex = 0; tileIndex < this.Tiles.Count; tileIndex++)
					SetImage(startImageIndex, tileIndex);
			}
			else
			{
				for (int tileIndex = this.Tiles.Count - 1; tileIndex >= 0; tileIndex--)
					SetImage(startImageIndex, tileIndex);
			}
		}

		private void SetImage(int startImageIndex, int tileIndex)
		{
			Tile tile = this.Tiles[tileIndex] as Tile;

			// If there's an image, put it in a tile
			if (startImageIndex + tileIndex < _displaySet.PresentationImages.Count)
				tile.PresentationImage = this.DisplaySet.PresentationImages[startImageIndex + tileIndex];
			// If there are no images left (the case when there are fewer images than tiles)
			// then just set the tile to blank
			else
				tile.PresentationImage = null;
		}

		private void OnTileAdded(object sender, TileEventArgs e)
		{
			_layoutRefreshRequired = true;

			Tile tile = e.Tile as Tile;
			tile.ImageViewer = this.ImageViewer;
			tile.ParentImageBox = this;
			EventsHelper.Fire(_tileAddedEvent, this, e);
		}

		private void OnTileRemoved(object sender, TileEventArgs e)
		{
			_layoutRefreshRequired = true;

			if (e.Tile.Selected)
				this.SelectedTile = null;

			EventsHelper.Fire(_tileRemovedEvent, this, e);
		}

		#endregion
	}
}
