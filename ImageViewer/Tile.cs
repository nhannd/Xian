using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Extension point for views onto <see cref="TileComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class TileViewExtensionPoint : ExtensionPoint<IView>
	{
	}

	/// <summary>
	/// TileComponent class
	/// </summary>
	[AssociateView(typeof(TileViewExtensionPoint))]
	public class Tile : ITile, IUIEventHandler
	{
		#region Private Fields

		private IImageViewer _imageViewer;
		private ImageBox _parentImageBox;
		private PresentationImage _presentationImage;
		private Rectangle _clientRectangle;
		private RectangleF _normalizedRectangle;
		private bool _selected = false;
		private bool _contextMenuEnabled;
		private Point _lastMousePoint;
		private InformationBox _informationBox;
		
		private CaptureUIEventHandler _mouseCapture;
		private CursorToken _cursorToken;

		private event EventHandler _rendererChangedEvent;
		private event EventHandler _drawingEvent;
		private event EventHandler<TileEventArgs> _selectionChangedEvent;

		private event EventHandler<MouseCaptureChangingEventArgs> _notifyCaptureChanging;
		private event EventHandler<InformationBoxChangedEventArgs> _informationBoxChanged;
		private event EventHandler _cursorTokenChanged;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public Tile()
		{
			_mouseCapture = new CaptureUIEventHandler();
		}

		#region Public properties

		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set 
			{
				_imageViewer = value;

				if (_presentationImage != null && _imageViewer != null)
					_presentationImage.ImageViewer = _imageViewer;
			}
		}

		public IImageBox ParentImageBox
		{
			get { return _parentImageBox as IImageBox; }
			internal set 
			{
				Platform.CheckForNullReference(value, "Tile.ParentImageBox");
				_parentImageBox = value as ImageBox; 
			}
		}

		/// <summary>
		/// Gets the <see cref="PresentationImage"/> associated with this
		/// <see cref="Tile"/>.
		/// </summary>
		public IPresentationImage PresentationImage
		{
			get { return _presentationImage as IPresentationImage; }
			internal set 
			{
				if (_presentationImage != value)
				{
					IRenderer oldRenderer = null;

					if (_presentationImage != null)
					{
						oldRenderer = _presentationImage.ImageRenderer;
					}

					// Disassociate the old presentation image with this tile
					if (_presentationImage != null)
						_presentationImage.Tile = null;

					// Assign the new presentation image.  Can be null.
					_presentationImage = value as PresentationImage;

					// Assuming the new value is not null, associate
					// this Tile with the new image
					if (_presentationImage != null)
					{
						_presentationImage.Tile = this;
						_presentationImage.Selected = this.Selected;

						if (_presentationImage.ImageViewer == null)
							_presentationImage.ImageViewer = this.ImageViewer;

						IRenderer newRenderer = _presentationImage.ImageRenderer;

						if (oldRenderer != null)
						{
							if (newRenderer.GetType() != oldRenderer.GetType())
								EventsHelper.Fire(_rendererChangedEvent, this, EventArgs.Empty);
						}
						else
						{
							EventsHelper.Fire(_rendererChangedEvent, this, EventArgs.Empty);
						}
					}
				}
			}
		}

		public int PresentationImageIndex
		{
			get
			{
				Platform.CheckMemberIsSet(_presentationImage, "PresentationImage", SR.ExceptionNoPresentationImageAssociatedWithTile);
				Platform.CheckMemberIsSet(this.ParentImageBox.DisplaySet, "ParentImageBox.DisplaySet", SR.ExceptionNoDisplaySetAssociatedWithImageBoxTile);

				IDisplaySet displaySet = this.ParentImageBox.DisplaySet;

				return displaySet.PresentationImages.IndexOf(this.PresentationImage);
			}
			set
			{
				Platform.CheckMemberIsSet(this.ParentImageBox.DisplaySet, "Tile.ParentImageBox.DisplaySet", SR.ExceptionNoDisplaySetAssociatedWithImageBoxTile);

				IDisplaySet displaySet = this.ParentImageBox.DisplaySet;

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
			private set 
			{
				if (_selected != value)
				{
					_selected = value;
					EventsHelper.Fire(_selectionChangedEvent, this, new TileEventArgs(this));
				}
			}
		}

		public Color BorderColor
		{
			get 
			{
				// TODO: remove these hard codes and make this configurable
				if (this.Selected)
					return Color.Yellow;
				else
					return Color.Gray;
			}
		}

		public int BorderWidth
		{
			get { return 1; }
		}

		public int InsetWidth
		{
			get { return 5; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			private set { _clientRectangle = value; }
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set { _normalizedRectangle = value; }
		}

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				if (ImageViewerComponent.AnnotationManager != null)
					return ImageViewerComponent.AnnotationManager.GetAnnotationBoxes(this.PresentationImage);

				return null;
			}
		}

		public bool ContextMenuEnabled
		{
			get { return _contextMenuEnabled; }
		}

		public InformationBox InformationBox
		{
			get { return _informationBox; }
			set
			{
				if (_informationBox == value)
					return;

				_informationBox = value;
				EventsHelper.Fire(_informationBoxChanged, this, new InformationBoxChangedEventArgs(_informationBox));
			}
		}

		public CursorToken CursorToken
		{
		    get
		    {
				return _cursorToken;
		    }
			set
			{
				if (_cursorToken == value)
					return;

				_cursorToken = value;
				EventsHelper.Fire(_cursorTokenChanged, this, new EventArgs());
			}
		}

		#endregion

		#region Public events

		public event EventHandler RendererChanged
		{
			add { _rendererChangedEvent += value; }
			remove { _rendererChangedEvent -= value; }
		}

		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		public event EventHandler<TileEventArgs> SelectionChanged
		{
			add { _selectionChangedEvent += value; }
			remove { _selectionChangedEvent -= value; }
		}

		public event EventHandler<MouseCaptureChangingEventArgs> NotifyCaptureChanging
		{
			add { _mouseCapture.NotifyCaptureChanging += value; }
			remove { _mouseCapture.NotifyCaptureChanging -= value; }
		}

		public event EventHandler<InformationBoxChangedEventArgs> InformationBoxChanged
		{
			add { _informationBoxChanged += value; }
			remove { _informationBoxChanged -= value; }
		}

		public event EventHandler CursorTokenChanged
		{
			add { _cursorTokenChanged += value; }
			remove { _cursorTokenChanged -= value; }
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
			}
		}

		#endregion

		#region Public methods

		public void Select()
		{
			if (!this.Selected)
			{
				Platform.CheckMemberIsSet(this.ParentImageBox, "Tile.ParentImageBox");
				Platform.CheckMemberIsSet(this.ImageViewer, "Tile.ImageViewer");

				if (_presentationImage != null)
				{
					this.Selected = true;
					_presentationImage.Selected = true;
					_parentImageBox.SelectedTile = this;
					this.ImageViewer.EventBroker.OnTileSelected(new TileSelectedEventArgs(this as ITile));
				}
			}
		}

		/// <summary>
		/// Draws the <see cref="PresentationImage"/> in this <see cref="TileComponent"/>.
		/// </summary>
		/// <remarks>Use this method to redraw the <see cref="PresentationImage"/> in this 
		/// <see cref="TileComponent"/>.</remarks>
		public void Draw()
		{
			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Draws the <see cref="PresentationImage"/> in this <see cref="TileComponent"/>.
		/// </summary>
		/// <param name="drawArgs"></param>
		/// <remarks>This is called by the GUI control associated with this
		/// component.  Never call this method directly.  Instead, use
		/// <see cref="Draw()"/>.</remarks>
		public void OnDraw(DrawArgs drawArgs)
		{
			// No PresentationImage associated with this Tile, so nothing to draw
			if (_presentationImage == null)
				return;

			this.ClientRectangle = drawArgs.ClientRectangle;

			drawArgs.Tile = this;
			drawArgs.ImageBox = this.ParentImageBox;

			_presentationImage.OnDraw(drawArgs);
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			// For the memento, we'll remember what PresentationImage was in this tile
			// and the location and dimensions of the tile itself
			TileMemento tileMemento = new TileMemento(this.PresentationImage);
			IMemento memento = tileMemento as IMemento;

			return memento;
		}

		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			TileMemento tileMemento = memento as TileMemento;
			Platform.CheckForInvalidCast(tileMemento, "memento", "TileMemento");

			this.PresentationImage = null;
			this.PresentationImage = tileMemento.PresentationImage;
		}

		#endregion

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			if (_mouseCapture.OnMouseDown(e))
				return true;

			if (_presentationImage == null)
				return true;

			// Select this tile if user has clicked on it
			Select();

			_contextMenuEnabled = true;
			_lastMousePoint = new Point(e.X, e.Y);

			SetSelectedObjects(e);
			return _presentationImage.OnMouseDown(e);
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			// HACK: If right mouse button is pressed and the mouse is moving,
			// the assumption we made in OnMouseDown was incorrect;
			// the user's real intent was to use a MouseTool, not bring
			// up the context menu.
			if (e.Button == XMouseButtons.Right)
			{
				// Unfortunately, OnMouseMove is called by .NET after
				// a mouse down even if the mouse hasn't moved, so 
				// we have to make sure the mouse has in fact moved
				// before we disable the context menu.
				if (_lastMousePoint != new Point(e.X, e.Y))
					_contextMenuEnabled = false;
			}

			if (_mouseCapture.OnMouseMove(e))
				return true;

			if (_presentationImage == null)
				return true;

			SetSelectedObjects(e);

			return _presentationImage.OnMouseMove(e);
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			if (_mouseCapture.OnMouseUp(e))
				return true;

			if (_presentationImage == null)
				return true;

			SetSelectedObjects(e);
			return _presentationImage.OnMouseUp(e);
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			if (_mouseCapture.OnMouseWheel(e))
				return true;

			if (_presentationImage == null)
				return true;

			SetSelectedObjects(e);
			return _presentationImage.OnMouseWheel(e);
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			if (_presentationImage == null)
				return true;

			return _presentationImage.OnKeyDown(e);
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			if (_presentationImage == null)
				return true;

			return _presentationImage.OnKeyUp(e);
		}

		#endregion

		#endregion

		#region Internal/private methods

		internal void Deselect()
		{
			if (this.Selected)
			{
				if (_presentationImage != null)
				{
					this.Selected = false;
					_presentationImage.Selected = false;
				}
			}
		}

		private void SetSelectedObjects(XMouseEventArgs e)
		{
			e.SelectedImageBox = this.ParentImageBox;
			e.SelectedTile = this;
		}

		private void OnCaptureChanging(object sender, MouseCaptureChangingEventArgs e)
		{
			EventsHelper.Fire(_notifyCaptureChanging, sender, e);
		}

		#endregion
	}
}
