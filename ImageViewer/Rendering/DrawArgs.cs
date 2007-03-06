using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Drawing mode enumeration.
	/// </summary>
	public enum DrawMode
	{
		/// <summary>
		/// Renders the image from scratch
		/// </summary>
		Render = 0,

		/// <summary>
		/// Refreshes the image by only repainting the rendered image.
		/// </summary>
		Refresh = 1
	}

	/// <summary>
	/// Provides data for the implementer of <see cref="IRenderer"/>.
	/// </summary>
	public class DrawArgs
	{
		#region Private Fields

		private CompositeGraphic _sceneGraph;
		private IPresentationImage _presentationImage;
		private ITile _tile;
		private IDisplaySet _displaySet;
		private IImageBox _imageBox;

		private IRenderingSurface _renderingSurface;
		private Rectangle _clientRectangle;
		private Rectangle _clipRectangle;
		private DrawMode _drawMode;

		private object _tag;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="DrawArgs"/> with
		/// the specified rendering surface, client rectangle,
		/// clip rectangle and drawing mode.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="clientRectangle"></param>
		/// <param name="clipRectangle"></param>
		/// <param name="drawMode"></param>
		public DrawArgs(
			IRenderingSurface surface, 
			Rectangle clientRectangle, 
			Rectangle clipRectangle,
			DrawMode drawMode)
		{
			_renderingSurface = surface;
			_clientRectangle = clientRectangle;
			_clipRectangle = clipRectangle;
			_drawMode = drawMode;
		}

		/// <summary>
		/// Gets the scene graph.
		/// </summary>
		public CompositeGraphic SceneGraph
		{
			get { return _sceneGraph; }
			internal set { _sceneGraph = value; }
		}

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/> being rendered.
		/// </summary>
		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
			internal set { _presentationImage = value; }
		}

		/// <summary>
		/// Gets the <see cref="ITile"/> in which the <see cref="IPresentationImage"/>
		/// is being rendered.
		/// </summary>
		public ITile Tile
		{
			get { return _tile; }
			internal set { _tile = value; }
		}

		/// <summary>
		/// Gets the <see cref="IDisplaySet"/> to which the <see cref="IPresentationImage"/>
		/// belongs.
		/// </summary>
		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
			internal set { _displaySet = value; }
		}

		/// <summary>
		/// Gets the <see cref="IImageBox"/> to which the <see cref="ITile"/> belongs.
		/// </summary>
		public IImageBox ImageBox
		{
			get { return _imageBox; }
			internal set { _imageBox = value; }
		}

		/// <summary>
		/// Gets or sets user-defined data.
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		/// <summary>
		/// Gets the rendering surface.
		/// </summary>
		public IRenderingSurface RenderingSurface
		{
			get { return _renderingSurface; }
		}

		/// <summary>
		/// Gets the rectangle to which the image will be rendered.
		/// </summary>
		/// <remarks>
		/// This is the rectangle of the view onto the <see cref="ITile"/>.
		/// </remarks>
		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		/// <summary>
		/// Gets the rectangle that requires repainting.
		/// </summary>
		/// <remarks>
		/// The implementer of <see cref="IRenderer"/> should use this rectangle
		/// to intelligently perform the <see cref="DrawMode.Refresh"/> operation.
		/// </remarks>
		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
		}

		/// <summary>
		/// Gets the <see cref="DrawMode"/>.
		/// </summary>
		public DrawMode DrawMode
		{
			get { return _drawMode; }
		}
	}
}
