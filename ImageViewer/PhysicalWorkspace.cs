using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model
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
	public class PhysicalWorkspace : IDrawable, IUIEventHandler, IMemorable
	{
		private ImageBoxCollection _imageBoxes = new ImageBoxCollection();
		private ImageWorkspace _parentWorkspace;
		private ClientArea _clientArea = new ClientArea();
		private UIEventHandler<ImageBox> _uiEventHandler;
		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;
		private bool _imageBoxLayoutChanged;
		private ImageBox _selectedImageBox;
		private int _rows;
		private int _columns;
		private bool _isRectangularImageBoxGrid;

		internal PhysicalWorkspace(ImageWorkspace parentWorkspace)
		{
			Platform.CheckForNullReference(parentWorkspace, "parentWorkspace");

			_parentWorkspace = parentWorkspace;
			_clientArea.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
			_uiEventHandler = new UIEventHandler<ImageBox>(this.ImageBoxes);
			_imageBoxes.ItemAdded += new EventHandler<ImageBoxEventArgs>(OnImageBoxAdded);
			_imageBoxes.ItemRemoved += new EventHandler<ImageBoxEventArgs>(OnImageBoxRemoved);
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _imageBoxes; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		public bool IsRectangularImageBoxGrid
		{
			get { return _isRectangularImageBoxGrid; }

			// This is temporary.  Ideally, given a number of rectangles,
			// this class should be able to determine whether rectangles
			// form a rectangular grid.  Until we implement that, we'll have
			// to rely on the client of this class to set this property to 
			// false if it's not a rectangular gride of image boxes.
			set { _isRectangularImageBoxGrid = value; }
		}

		public ImageWorkspace ParentWorkspace
		{
			get { return _parentWorkspace; }
		}

		/// <summary>
		/// Gets the associated <see cref="LogicalWorkspace"/>.
		/// </summary>
		/// <value>The associated <see cref="LogicalWorkspace"/></value>
		public LogicalWorkspace LogicalWorkspace
		{
			get 
			{
				if (this.ParentWorkspace == null)
					return null;

				return this.ParentWorkspace.LogicalWorkspace; 
			}
		}

		/// <summary>
		/// Gets or sets this physical workspace's client rectangle.
		/// </summary>
		/// <value>The physical workspace's client rectangle.</value>
		/// <remarks>
		/// When the <see cref="System.Windows.Forms.Control"/> object associated with
		/// the <see cref="PhysicalWorkspace"/> is resized, this property should be
		/// set to the control's new size so that the client rectangles of associated
		/// image boxes and tiles are also resized accordingly.  Note that setting
		/// this property does <i>not</i> result in the physical control being resized.
		/// </remarks>
		public Rectangle ClientRectangle
		{
			get
			{
				return _clientArea.ClientRectangle;
			}
			set
			{
				// Since the normalized rectangle is (0,0,1,1), the client and
				// parent rectangles will be the same. We set the *parent* rectangle
				// in m_ClientArea so that the ClientRectangle will be computed automatically
				_clientArea.ParentRectangle = value;

				foreach (ImageBox imageBox in this.ImageBoxes)
					imageBox.ParentRectangle = _clientArea.ClientRectangle;
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="ImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="ImageBox"/></value>
		public ImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
			internal set
			{
				if (_selectedImageBox != null)
					_selectedImageBox.Selected = false;

				_selectedImageBox = value;
			}
		}

		/// <summary>
		/// Occurs when any <see cref="PresentationImage"/> in the workspace
		/// is asked to be drawn.
		/// </summary>
		/// <remarks>
		/// The event handler receives an argument of type <see cref="ImageDrawingEventArgs"/>.
		/// The <see cref="System.Windows.Forms.Control"/> object associated with the
		/// <b>PhysicalWorkspace</b> must subscribe to this event so that knows
		/// when a draw request has been made.  <see cref="ImageDrawingEventArgs"/> contains
		/// all the information necessary for the View plugin to render the image.
		/// </remarks>
		public event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add
			{
				_imageDrawingEvent += value;
			}
			remove
			{
				_imageDrawingEvent -= value;
			}
		}

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

			_rows = rows;
			_columns = columns;
			_isRectangularImageBoxGrid = true;

			this.ImageBoxes.Clear();

			float imageBoxWidth = 1.0f / columns;
			float imageBoxHeight = 1.0f / rows;

			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					float x = column * imageBoxWidth;
					float y = row * imageBoxHeight;
					RectangleF rect = new RectangleF(x, y, imageBoxWidth, imageBoxHeight);

					ImageBox imageBox = new ImageBox();
					imageBox.NormalizedRectangle = rect;
					this.ImageBoxes.Add(imageBox);
				}
			}
		}

		//public override void Cleanup()
		//{
		//    RemoveAllImageBoxes();
		//}

		#region IDrawable

		/// <summary>
		/// Draws all currently visible images in this <see cref="PhysicalWorkspace"/>.
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
			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBox.Draw(paintNow);
		}

		#endregion

		#region IUIEventHandler Members

		/// <summary>
		/// Handles mouse down event.
		/// </summary>
		/// <param name="e">Mouse event data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="sender"/> or
		/// <paramref name="e"/> is <b>null</b></exception>
		public bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnMouseDown(e);
		}

		/// <summary>
		/// Handles mouse move event.
		/// </summary>
		/// <param name="e">Mouse event data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="sender"/> or
		/// <paramref name="e"/> is <b>null</b></exception>
		public bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnMouseMove(e);
		}

		/// <summary>
		/// Handles mouse up event.
		/// </summary>
		/// <param name="e">Mouse event data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="sender"/> or
		/// <paramref name="e"/> is <b>null</b></exception>
		public bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnMouseUp(e);
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnMouseWheel(e);
		}

		/// <summary>
		/// Handles key down event.
		/// </summary>
		/// <param name="e">Key event data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="sender"/> or
		/// <paramref name="e"/> is <b>null</b></exception>
		public bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnKeyDown(e);
		}

		/// <summary>
		/// Handles key up event.
		/// </summary>
		/// <param name="e">Key event data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><paramref name="sender"/> or
		/// <paramref name="e"/> is <b>null</b></exception>
		public bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return _uiEventHandler.OnKeyUp(e);
		}

		#endregion

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
											 _clientArea, 
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

			// TODO:  check if we actually need this
			//_logicalWorkspace = workspaceMemento.LogicalWorkspace;
			_clientArea = workspaceMemento.ClientArea;

			for (int i = 0; i < workspaceMemento.ImageBoxes.Count; i++)
			{
				IMemento imageBoxMemento = workspaceMemento.ImageBoxMementos[i];
				ImageBox imageBox = workspaceMemento.ImageBoxes[i];
				imageBox.SetMemento(imageBoxMemento);
			
				this.ImageBoxes.Add(imageBox);
			}
		}

		#endregion

		private void OnImageBoxAdded(object sender, ImageBoxEventArgs e)
		{
			e.ImageBox.ParentPhysicalWorkspace = this;
			e.ImageBox.ParentRectangle = _clientArea.ClientRectangle;
			e.ImageBox.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
			_imageBoxLayoutChanged = true;
		}

		private void OnImageBoxRemoved(object sender, ImageBoxEventArgs e)
		{
			e.ImageBox.ImageDrawing -= new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
			_imageBoxLayoutChanged = true;

			if (e.ImageBox.Selected)
				this.SelectedImageBox = null;
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			e.PhysicalWorkspace = this;

			if (_imageBoxLayoutChanged)
			{
				e.ImageBoxLayoutChanged = _imageBoxLayoutChanged;
				_imageBoxLayoutChanged = false;
			}

			EventsHelper.Fire(_imageDrawingEvent, this, e);
		}
	}
}