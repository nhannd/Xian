using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The final image that is presented to the user in a <see cref="Tile"/>.
	/// </summary>
	/// <remarks>
	/// An <see cref="IPresentationImage"/> can be thought of as a “scene” 
	/// composed of many parts, be they images, lines, text, etc.  It is the
	/// image that is presented to the user in a <see cref="Tile"/>.
	/// </remarks>
	public abstract class PresentationImage : IPresentationImage
	{
		#region Private Fields

		private ImageViewerComponent _imageViewer;
		private DisplaySet _parentDisplaySet;
		private Tile _tile;

		private bool _selected = false;
		private bool _linked = true;
		private string _uid;
		
		private IRenderer _imageRenderer;

		private SceneGraph _sceneGraph;
		private ISelectableGraphic _selectedGraphic;
		private IFocussableGraphic _focussedGraphic;

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

		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImage"/>.
		/// </summary>
		protected PresentationImage()
		{
		}

		#region Public Properties

		/// <summary>
		/// Gets the parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer as IImageViewer; }
			internal set 
			{ 
				_imageViewer = value as ImageViewerComponent;
				this.SceneGraph.SetImageViewer(_imageViewer);
			}
		}

		/// <summary>
		/// Gets the parent <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The parent <see cref="IDisplaySet"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> has not
		/// been added to the <see cref="IDisplaySet"/> yet.</value>
		public IDisplaySet ParentDisplaySet
		{
			get { return _parentDisplaySet as IDisplaySet; }
			internal set { _parentDisplaySet = value as DisplaySet; }
		}

		/// <summary>
		/// Gets the associated <see cref="ITile"/>.
		/// </summary>
		/// <value>The <see cref="ITile"/> that currently contains the
		/// <see cref="PresentationImage"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> is not currently visible.</value>
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
				}
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
		/// Gets a value indicating whether the <see cref="PresentationImage"/> is selected.
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

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IPresentationImage"/>.
		/// </summary>
		public string Uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		/// <summary>
		/// Gets the <see cref="SceneGraph"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="SceneGraph"/> is a tree of <see cref="IGraphic"/> objects
		/// that represents what the user sees in a <see cref="Tile"/>.  If you're writing
		/// tools, you should avoid accessing the <see cref="SceneGraph"/> directly as it 
		/// is intended only for the renderer to traverse.  Instead, add and remove
		/// from the <see cref="SceneGraph"/> through interfaces on <see cref="PresentationImage"/>
		/// subclasses.
		/// </remarks>
		public CompositeGraphic SceneGraph
		{
			get 
			{
				if (_sceneGraph == null)
				{
					_sceneGraph = new SceneGraph();
					_sceneGraph.SetParentPresentationImage(this);
				}

				return _sceneGraph; 
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently selected.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		public virtual ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
			set
			{
				if (_selectedGraphic == value)
					return;

				if (_selectedGraphic != null)
					_selectedGraphic.Selected = false;

				_selectedGraphic = value;

				if (this.ImageViewer != null)
					if (this.ImageViewer.EventBroker != null)
						this.ImageViewer.EventBroker.OnGraphicSelected(new GraphicSelectedEventArgs(_selectedGraphic));
			}
		}

		/// <summary>
		/// Gets the currently focussed <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently focussed.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		public virtual IFocussableGraphic FocussedGraphic
		{
			get { return _focussedGraphic; }
			set
			{
				if (_focussedGraphic == value)
					return;

				if (_focussedGraphic != null)
					_focussedGraphic.Focussed = false;

				_focussedGraphic = value;
			}
		}

		/// <summary>
		/// Gets this <see cref="PresentationImage"/>'s image renderer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The creation of the image renderer is left to the subclass.
		/// This allows the greatest flexibility, since it is sometimes the case
		/// that a subclass of <see cref="PresentationImage"/> needs 
		/// a specialized <see cref="IRenderer"/>.
		/// </para>
		/// <para>
		/// In general, <see cref="ImageRenderer"/> should be considered an internal
		/// Framework property and should not be used.
		/// </para>
		/// </remarks>
		public virtual IRenderer ImageRenderer 
		{
			get { return _imageRenderer; }
			protected set { _imageRenderer = value; }
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
				DisposeSceneGraph();
				DisposeRenderer();
			}
		}

		private void DisposeSceneGraph()
		{
			if (this.SceneGraph == null)
				return;

			this.SceneGraph.Dispose();
			_sceneGraph = null;
		}

		private void DisposeRenderer()
		{
			if (this.ImageRenderer == null)
				return;

			this.ImageRenderer.Dispose();
			_imageRenderer = null;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a clone of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <returns></returns>
		public abstract IPresentationImage Clone();

		/// <summary>
		/// Draws the <see cref="PresentationImage"/>.
		/// </summary>
		public void Draw()
		{
			if (this.Visible)
				this.Tile.Draw();
		}

		/// <summary>
		/// Raises the <see cref="EventBroker.ImageDrawing"/> event and
		/// renders the <see cref="PresentationImage"/>.
		/// </summary>
		/// <param name="drawArgs"></param>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public virtual void OnDraw(DrawArgs drawArgs)
		{
			drawArgs.SceneGraph = this.SceneGraph;
			drawArgs.PresentationImage = this;
			drawArgs.DisplaySet = this.ParentDisplaySet;

			(this.SceneGraph as SceneGraph).ClientRectangle = drawArgs.ClientRectangle;

			// Let others know that we're about to draw
			ImageDrawingEventArgs args = new ImageDrawingEventArgs(this);
			this.ImageViewer.EventBroker.OnImageDrawing(args);

			this.ImageRenderer.Draw(drawArgs);
		}

		#endregion
	}
}
