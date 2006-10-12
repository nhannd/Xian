using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container of image boxes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>PhysicalWorkspace</b> and its related classes <see cref="ImageBox"/> and
	/// <see cref="Tile"/> collectively describe how images are positioned and sized
	/// on the screen.  A <b>PhysicalWorkspace</b> contains zero or more image boxes, which
	/// in turn each contain zero or more tiles.  Image boxes can be arranged arbitrarily
	/// in a workspace whereas tiles in an image box must be laid out as a rectangular
	/// grid. A tile contains an image, specifically a <see cref="PresentationImage"/>.
	/// </para>
	/// <para>
	/// Workspace layouts are described in normalized coordinates.
	/// The top left corner of the workspace corresponds to (0,0) and the bottom right
	/// to (1,1).  When an <see cref="ImageBox"/> is defined and added to a workspace, 
	/// it is done using that coordinate system.  Thus, for example, an <see cref="ImageBox"/> 
	/// that occupies the lower right quadrant of the workspace is described by the 
	/// rectangle (0.5, 0.5, 1.0, 1.0).  
	/// </para>
	/// <para>
	/// In general, a <b>PhysicalWorkspace</b> is associated with
	/// a <see cref="System.Windows.Forms.Control"/> of some kind in the View plugin.
	/// (In the standard implementation of ClearCanvas, <b>PhysicalWorkspace</b> is 
	/// associated with <see cref="WorkspaceForm"/> in ClearCanvas.Workstation.View.dll.)	
	/// In that control, images are rendered according to the layout described
	/// by <b>PhysicalWorkspace</b>.  When the control is resized, the
	/// <see cref="PhysicalWorkspace.ClientRectangle"/> property should be set so that
	/// the physical workspace's image boxes and tiles are also resized accordingly.
	/// </para>
	/// <para>
	/// Because <b>PhysicalWorkspace</b> is always associated with a 
	/// <see cref="System.Windows.Forms.Control"/> of some kind in the View plugin, it
	/// also acts as a entry conduit for mouse and keyboard input messages via
	/// <b>PhysicalWorkspace</b>'s <see cref="IUIEventHandler"/> interface.  Input messages
	/// are sent downward to the image boxes, tiles, presentation images, and layers,
	/// giving each a chance to handle the message.  
	/// </para>
	/// <para>
	/// By default, when constructed, <b>PhysicalWorkspace</b> contains zero image boxes.
	/// </para>
	/// </remarks>
	public class PhysicalWorkspace : IPhysicalWorkspace
	{
		#region Private Fields

		private ImageBoxCollection _imageBoxes;
		private IImageViewer _imageViewer;
		private ImageBox _selectedImageBox;
		private int _rows;
		private int _columns;
		private bool _isRectangularImageBoxGrid;
		private bool _layoutRefreshRequired;

		private event EventHandler _drawingEvent;
		private event EventHandler<ImageBoxEventArgs> _imageBoxAddedEvent;
		private event EventHandler<ImageBoxEventArgs> _imageBoxRemovedEvent;

		#endregion

		internal PhysicalWorkspace(IImageViewer imageViewer)
		{
            Platform.CheckForNullReference(imageViewer, "parentWorkspace");

            _imageViewer = imageViewer;
			this.ImageBoxes.ItemAdded += new EventHandler<ImageBoxEventArgs>(OnImageBoxAdded);
			this.ImageBoxes.ItemRemoved += new EventHandler<ImageBoxEventArgs>(OnImageBoxRemoved);
		}

		#region Public properties

		public ImageBoxCollection ImageBoxes
		{
			get 
			{
				if (_imageBoxes == null)
					_imageBoxes = new ImageBoxCollection();

				return _imageBoxes; 
			}
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		//public bool IsRectangularImageBoxGrid
		//{
		//    get { return _isRectangularImageBoxGrid; }

		//    // This is temporary.  Ideally, given a number of rectangles,
		//    // this class should be able to determine whether rectangles
		//    // form a rectangular grid.  Until we implement that, we'll have
		//    // to rely on the client of this class to set this property to 
		//    // false if it's not a rectangular gride of image boxes.
		//    set { _isRectangularImageBoxGrid = value; }
		//}

		public bool LayoutRefreshRequired
		{
			get { return _layoutRefreshRequired; }
		}
		
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Gets the associated <see cref="LogicalWorkspace"/>.
		/// </summary>
		/// <value>The associated <see cref="LogicalWorkspace"/></value>
		public ILogicalWorkspace LogicalWorkspace
		{
			get 
			{
				Platform.CheckMemberIsSet(this.ImageViewer, "PhysicalWorkspace.ImageViewer");

                return this.ImageViewer.LogicalWorkspace; 
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="ImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="ImageBox"/></value>
		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox as IImageBox; }
			internal set
			{
				if (_selectedImageBox != null)
					_selectedImageBox.Deselect();

				_selectedImageBox = value as ImageBox;
			}
		}

		#endregion

		#region Public events

		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		public event EventHandler<ImageBoxEventArgs> ImageBoxAdded
		{
			add { _imageBoxAddedEvent += value; }
			remove { _imageBoxAddedEvent -= value; }
		}

		public event EventHandler<ImageBoxEventArgs> ImageBoxRemoved
		{
			add { _imageBoxRemovedEvent += value; }
			remove { _imageBoxRemovedEvent -= value; }
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
				DisposeImageBoxes();
			}
		}

		private void DisposeImageBoxes()
		{
			if (this.ImageBoxes == null)
				return;

			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBox.Dispose();

			_imageBoxes = null;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a rectangular grid of image boxes.
		/// </summary>
		/// <remarks>
		/// This is a convenience method that creates a rectangular grid
		/// of image boxes.  Each time this method is called, any existing image boxes
		/// are removed from the workspace and new ones added, even if the number of rows
		/// and columns has not changed from the previous call.
		/// Arbitrary, non-rectangular <see cref="ImageBox"/> grids can be
		/// created by manually instantiating <see cref="ImageBox"/> objects, setting their
		/// normalized coordinates then adding them to the <see cref="PhysicalWorkspace"/>
		/// using <see cref="AddImageBox"/>.
		/// </remarks>
		/// <param name="rows">Number of <see cref="ImageBox"/> rows.</param>
		/// <param name="columns">Number of <see cref="ImageBox"/> columns.</param>
		/// <exception cref="ArgumentException"><paramref name="numberOfRows"/> or 
		/// <paramref name="numberOfColumns"/> is less than 1.</exception>
		public void SetImageBoxGrid(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			if (_rows == rows && _columns == columns)
				return;

			_rows = rows;
			_columns = columns;
			_isRectangularImageBoxGrid = true;

			this.ImageBoxes.Clear();

			double imageBoxWidth = (1.0d / columns);
			double imageBoxHeight = (1.0d / rows);

			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					double x = column * imageBoxWidth;
					double y = row * imageBoxHeight;
					RectangleF rect = new RectangleF((float)x, (float)y, (float)imageBoxWidth, (float)imageBoxHeight);

					ImageBox imageBox = new ImageBox();
					imageBox.NormalizedRectangle = rect;
					this.ImageBoxes.Add(imageBox);
				}
			}
		}

		public void Draw()
		{
			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
			_layoutRefreshRequired = false;
		}

		#region IMemorable Members

		/// <summary>
		/// Creates a memento for this <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <returns>A memento for this <see cref="PhysicalWorkspace"/>.</returns>
		/// <remarks>
		/// This method is used to remember the current state of a
		/// <see cref="PhysicalWorkspace"/>.  The memento remembers the actual <see cref="ImageBox"/>
		/// <i>instances</i> contained in the <see cref="PhysicalWorkspace"/>.  Calling
		/// <see cref="PhysicalWorkspace.SetMemento"/> at a later time restores 
		/// those instances.
		/// </remarks>
		public IMemento CreateMemento()
		{
			MementoList imageBoxMementos = new MementoList();

			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBoxMementos.AddMemento(imageBox.CreateMemento());

			PhysicalWorkspaceMemento workspaceMemento =
				new PhysicalWorkspaceMemento(this.LogicalWorkspace,
											 new ImageBoxCollection(this.ImageBoxes),
											 imageBoxMementos);

			return workspaceMemento;
		}

		/// <summary>
		/// Sets this <see cref="PhysicalWorkspace"/> with a previously created memento.
		/// </summary>
		/// <param name="memento">Memento to set.</param>
		/// <remarks>
		/// This method restores the state of a <see cref="PhysicalWorkspace"/> with
		/// a memento previously created by <see cref="PhysicalWorkspace.CreateMemento"/>.
		/// </remarks>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			PhysicalWorkspaceMemento workspaceMemento = memento as PhysicalWorkspaceMemento;
			Platform.CheckForInvalidCast(workspaceMemento, "memento", "PhysicalWorkspaceMemento");

			this.ImageBoxes.Clear();

			for (int i = 0; i < workspaceMemento.ImageBoxes.Count; i++)
			{
				IMemento imageBoxMemento = workspaceMemento.ImageBoxMementos[i];
				IImageBox imageBox = workspaceMemento.ImageBoxes[i];
				imageBox.SetMemento(imageBoxMemento);

				this.ImageBoxes.Add(imageBox);
			}
		}

		#endregion

		#endregion

		#region Private methods

		private void OnImageBoxAdded(object sender, ImageBoxEventArgs e)
		{
			_layoutRefreshRequired = true;

			ImageBox imageBox = e.ImageBox as ImageBox;
			imageBox.ImageViewer = this.ImageViewer;
			imageBox.ParentPhysicalWorkspace = this;
			EventsHelper.Fire(_imageBoxAddedEvent, this, e);
		}

		private void OnImageBoxRemoved(object sender, ImageBoxEventArgs e)
		{
			_layoutRefreshRequired = true;

			if (e.ImageBox.Selected)
				this.SelectedImageBox = null;

			EventsHelper.Fire(_imageBoxRemovedEvent, this, e);
		}

		#endregion
	}
}