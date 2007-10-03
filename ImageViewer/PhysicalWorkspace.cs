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
	/// A container for image boxes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="PhysicalWorkspace"/> and its related classes <see cref="ImageBox"/> and
	/// <see cref="Tile"/> collectively describe how images are positioned and sized
	/// on the screen.  A <see cref="PhysicalWorkspace"/> contains zero or more image 
	/// boxes, which in turn each contain zero or more tiles.  Image boxes can be 
	/// arranged arbitrarily in a workspace whereas tiles in an image box must be laid 
	/// out as a rectangular grid. A tile contains a <see cref="PresentationImage"/>.
	/// </para>
	/// <para>
	/// Physical workspace layouts are described in normalized coordinates.
	/// The top left corner of the workspace corresponds to (0,0) and the bottom right
	/// (1,1).  When an <see cref="IImageBox"/> is defined and added to a workspace, 
	/// it is done using that coordinate system.  See 
	/// <see cref="IImageBox.NormalizedRectangle"/> for an example.
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

		private event EventHandler _drawingEvent;
		private event EventHandler _layoutCompletedEvent;

		#endregion

		internal PhysicalWorkspace(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

            _imageViewer = imageViewer;
			this.ImageBoxes.ItemAdded += OnImageBoxAdded;
			this.ImageBoxes.ItemRemoved += OnImageBoxRemoved;
		}

		#region Public properties

		/// <summary>
		/// Gets the collection of <see cref="IImageBox"/> objects that belong
		/// to this <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>When a <see cref="PhysicalWorkspace "/> is first instantiated,
		/// <see cref="ImageBoxes"/> is empty.</remarks>
		public ImageBoxCollection ImageBoxes
		{
			get 
			{
				if (_imageBoxes == null)
					_imageBoxes = new ImageBoxCollection();

				return _imageBoxes; 
			}
		}

		/// <summary>
		/// Gets the number of rows of <see cref="IImageBox"/> objects in the
		/// <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Rows"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns of <see cref="IImageBox"/> objects in the
		/// <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Columns"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Returns the image box at a specified row & column index.
		/// </summary>
		/// <param name="row">the zero-based row index of the image box to retrieve</param>
		/// <param name="column">the zero-based column index of the image box to retrieve</param>
		/// <returns>the image box at the specified row and column indices</returns>
		/// <remarks>This method is only valid if <see cref="SetImageBoxGrid"/> has been called and/or the 
		/// layout is, in fact, rectangular.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if the layout is not currently rectangular</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if either of the row/column indices are out of range</exception>
		public IImageBox this[int row, int column]
		{
			get
			{
				if (this.Rows <= 0 || this.Columns <= 0)
					throw new InvalidOperationException(SR.ExceptionLayoutIsNotRectangular);

				Platform.CheckArgumentRange(row, 0, this.Rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, this.Columns - 1, "column");
				
				return this.ImageBoxes[row * this.Columns + column];
			}
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Gets the associated <see cref="ILogicalWorkspace"/>.
		/// </summary>
		public ILogicalWorkspace LogicalWorkspace
		{
			get { return this.ImageViewer.LogicalWorkspace; }
		}

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
			internal set
			{
				if (_selectedImageBox != null)
					_selectedImageBox.Deselect();

				_selectedImageBox = value as ImageBox;
			}
		}

		#endregion

		#region Public events

		/// <summary>
		/// Occurs when the <see cref="PhysicalWorkspace"/> is drawn.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		/// <summary>
		/// Occurs when all changes to image box collection are complete.
		/// </summary>
		/// <remarks>
		/// <see cref="LayoutCompleted"/> is raised by the Framework when
		/// <see cref="SetImageBoxGrid"/> has been called.  If you are adding/removing
		/// <see cref="IImageBox"/> objects manually, you should raise this event when
		/// you're done by calling <see cref="OnLayoutCompleted"/>.  This event is
		/// consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		public event EventHandler LayoutCompleted
		{
			add { _layoutCompletedEvent += value; }
			remove { _layoutCompletedEvent -= value; }
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
				Platform.Log(LogLevel.Error, e);
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
		/// Creates a rectangular <see cref="IImageBox"/> grid.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <see cref="SetImageBoxGrid"/> is a convenience method that adds
		/// <see cref="IImageBox"/> objects to the <see cref="IPhysicalWorkspace"/>
		/// in a rectangular grid.
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="rows"/> or 
		/// <paramref name="columns"/> is less than 1.</exception>
		public void SetImageBoxGrid(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			if (_rows == rows && _columns == columns)
				return;

			_rows = rows;
			_columns = columns;

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

			OnLayoutCompleted();
		}

		/// <summary>
		/// Raises the <see cref="LayoutCompleted"/> event.
		/// </summary>
		/// <remarks>
		/// If you are adding/removing <see cref="IImageBox"/> objects manually 
		/// (i.e., instead of using <see cref="SetImageBoxGrid"/>), you should call
		/// <see cref="OnLayoutCompleted"/> to raise the <see cref="LayoutCompleted"/> event.  
		/// This event is consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		public void OnLayoutCompleted()
		{
			EventsHelper.Fire(_layoutCompletedEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Selects the first <see cref="IImageBox"/> in the image box collection.
		/// </summary>
		/// <remarks>
		/// When <see cref="SetImageBoxGrid"/> has been used to setup the 
		/// <see cref="IPhysicalWorkspace"/>, the first <see cref="IImageBox"/> in the
		/// image box collection will be the top-left <see cref="IImageBox"/>.
		/// </remarks>
		public void SelectDefaultImageBox()
		{
			if (this.ImageBoxes.Count > 0)
			{
				IImageBox topLeftImageBox = this.ImageBoxes[0];

				if (topLeftImageBox != null)
					topLeftImageBox.SelectDefaultTile();
			}
		}

		/// <summary>
		/// Draws the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		public void Draw()
		{
			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
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
		/// <see cref="SetMemento"/> at a later time restores 
		/// those instances.
		/// </remarks>
		public IMemento CreateMemento()
		{
			MementoList imageBoxMementos = new MementoList();

			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBoxMementos.AddMemento(imageBox.CreateMemento());

			PhysicalWorkspaceMemento workspaceMemento =
				new PhysicalWorkspaceMemento(new ImageBoxCollection(this.ImageBoxes),
											 imageBoxMementos, 
											 this.Rows,
											 this.Columns);

			return workspaceMemento;
		}

		/// <summary>
		/// Sets this <see cref="PhysicalWorkspace"/> with a previously created memento.
		/// </summary>
		/// <param name="memento">Memento to set.</param>
		/// <remarks>
		/// This method restores the state of a <see cref="PhysicalWorkspace"/> with
		/// a memento previously created by <see cref="CreateMemento"/>.
		/// </remarks>
		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			PhysicalWorkspaceMemento workspaceMemento = memento as PhysicalWorkspaceMemento;
			Platform.CheckForInvalidCast(workspaceMemento, "memento", "PhysicalWorkspaceMemento");

			this.ImageBoxes.Clear();

			_rows = workspaceMemento.Rows;
			_columns = workspaceMemento.Columns;

			for (int i = 0; i < workspaceMemento.ImageBoxes.Count; i++)
			{
				IMemento imageBoxMemento = workspaceMemento.ImageBoxMementos[i];
				IImageBox imageBox = workspaceMemento.ImageBoxes[i];
				imageBox.SetMemento(imageBoxMemento);

				this.ImageBoxes.Add(imageBox);
			}

			OnLayoutCompleted();

			Draw();
		}

		#endregion

		#endregion

		#region Private methods

		private void OnImageBoxAdded(object sender, ImageBoxEventArgs e)
		{
			ImageBox imageBox = e.ImageBox as ImageBox;
			imageBox.ImageViewer = this.ImageViewer;
			imageBox.ParentPhysicalWorkspace = this;
		}

		private void OnImageBoxRemoved(object sender, ImageBoxEventArgs e)
		{
			if (e.ImageBox.Selected)
				this.SelectedImageBox = null;

			e.ImageBox.DisplaySet = null;
		}

		#endregion
	}
}