using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer
{
	public abstract class PresentationImage : IPresentationImage, IUIEventHandler
	{
		#region Private Fields

		private LayerManager _layerManager;
		private ImageViewerComponent _imageViewer;
		private DisplaySet _parentDisplaySet;
		private Tile _tile;

		private bool _selected = false;
		private bool _linked = true;

		protected IRenderer _imageRenderer;
		
		// TODO: Perhaps each layer should have its own ILayerRenderer?  
		// The idea is to delegate the actual rendering to the layers themselves, since
		// they know best how to render themselves.  If the Layer.Renderer is null, then
		// the basic image renderer will render it in the default way.  Otherwise, it will
		// use the renderer associated with the layer. 
		// Fine grained objects like primitives might use the flyweight pattern to 
		// prevent too many renderers.
		
		// TODO: To make the renderer smarter, at the beginning of the draw method,
		// analyze the RedrawRequired proper of the layers in the PresentationImage.
		// e.g., in the case where every layer needs to be redrawn, draw the image
		// first, followed by the graphics.  This would be done all to one buffer.
		// In the case where a graphic is simply being moved over an image that is
		// not changing, the image layer's redraw flag would be false, but the
		// graphic layer's flag would be true.  In that situation, make a copy
		// of the rendered image buffer, then just blt it as the graphic is moved.
		// (Maybe we need to incorporate some notion of state into the renderer?)


		#endregion

		protected PresentationImage()
		{

		}

		#region Public Properties

		/// <summary>
		/// Gets the parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value><b>null</b> if <see cref="PresentationImage"/> has not been
		/// added to a <see cref="DisplaySet"/>.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer as IImageViewer; }
			internal set 
			{ 
				_imageViewer = value as ImageViewerComponent;
				//this.LayerManager.ImageViewer = _imageViewer;
			}
		}

		/// <summary>
		/// Gets the parent <see cref="DisplaySet"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if <see cref="PresentationImage"/> has not been
		/// added to a <see cref="DisplaySet"/>.</value>
		public IDisplaySet ParentDisplaySet
		{
			get { return _parentDisplaySet as IDisplaySet; }
			internal set { _parentDisplaySet = value as DisplaySet; }
		}

		/// <summary>
		/// Gets the associated <see cref="TileComponent"/>.
		/// </summary>
		/// <value><b>null</b> if <see cref="PresentationImage"/>
		/// is not currently visible.</value>
		public ITile Tile
		{
			get { return _tile as ITile; }
			internal set 
			{
				if (_tile != value)
				{
					_tile = value as Tile;

					if (_tile != null)
						_tile.PresentationImage = this;
					else
					{
						// If the image is no longer associated with a Tile,
						// i.e., it's no longer visible, then dispose of the
						// renderer and set the renderer to null so we don't
						// hog memory.  (It may be better to keep the renderer
						// around for performance reasons, but we'll see.)
						DisposeRenderer();
					}
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="LayerManager"/>.
		/// </summary>
		public LayerManager LayerManager
		{
			get
			{
				if (_layerManager == null)
				    _layerManager = new LayerManager(this);

				return _layerManager;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="PresentationImage"/> is visible.
		/// </summary>
		public bool Visible
		{
			get { return this.Tile != null; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="PresentationImage"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			internal set
			{
				if (_selected == value)
					return;

				_selected = value;

				if (!_selected)
					return;

				if (this.ImageViewer == null)
					return;

				this.ImageViewer.EventBroker.OnPresentationImageSelected(
				    new PresentationImageSelectedEventArgs(this));
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="PresentationImage"/> is linked.
		/// </summary>
		public bool Linked
		{
			get { return _linked; }
			set
			{
				if (_linked != value)
				{
					_linked = value;

					if (_linked)
						_parentDisplaySet.LinkPresentationImage(this);
					else
						_parentDisplaySet.UnlinkPresentation(this);
				}
			}
		}

		public abstract IRenderer ImageRenderer { get; }

		#endregion

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

		#region Public Events

		public event EventHandler<MouseCaptureChangingEventArgs> NotifyCaptureChanging
		{
			add { this.LayerManager.NotifyCaptureChanging += value; }
			remove { this.LayerManager.NotifyCaptureChanging -= value; }
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
				DisposeRenderer();
				DisposeLayerManager();
			}
		}

		private void DisposeRenderer()
		{
			if (this.ImageRenderer == null)
				return;

			this.ImageRenderer.Dispose();
			_imageRenderer = null;
		}

		private void DisposeLayerManager()
		{
			if (this.LayerManager == null)
				return;

			// TODO: Add disposal to LayerManager
			//this.LayerManager.Dispose();
		}

		public void Draw()
		{
			if (this.Visible)
				this.Tile.Draw();
		}

		public virtual void OnDraw(DrawArgs drawArgs)
		{
			drawArgs.PresentationImage = this;
			drawArgs.DisplaySet = this.ParentDisplaySet;

			ImageDrawingEventArgs args = new ImageDrawingEventArgs(this);
			this.ImageViewer.EventBroker.OnImageDrawing(args);

			this.LayerManager.RootLayerGroup.RedrawRequired = true;
			this.LayerManager.RootLayerGroup.DestinationRectangle = drawArgs.ClientRectangle;

			this.ImageRenderer.Draw(drawArgs);
		}

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			SetSelectedObjects(e);

			bool handled = this.LayerManager.OnMouseDown(e);

			if (!handled)
			{
				MouseTool tool = _imageViewer.MouseButtonToolMap[e.Button];

				if (tool != null)
					tool.OnMouseDown(e);
			}

			return true;
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			SetSelectedObjects(e);

			bool handled = this.LayerManager.OnMouseMove(e);

			if (!handled)
			{
				MouseTool tool = _imageViewer.MouseButtonToolMap[e.Button];

				if (tool != null)
					tool.OnMouseMove(e);
			}

			return true;
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			SetSelectedObjects(e);

			bool handled = this.LayerManager.OnMouseUp(e);

			if (!handled)
			{
				MouseTool tool = _imageViewer.MouseButtonToolMap[e.Button];

				if (tool != null)
					tool.OnMouseUp(e);
			}

			return true;
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{

			return true;
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			return true;
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			return true;
		}

		#endregion

		private void SetSelectedObjects(XMouseEventArgs e)
		{
			e.SelectedPresentationImage = this;
			e.SelectedDisplaySet = this.ParentDisplaySet;
		}
	}
}
