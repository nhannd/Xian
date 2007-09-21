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
		//private ITile _tile;
		//private IDisplaySet _displaySet;
		//private IImageBox _imageBox;

		private IRenderingSurface _renderingSurface;
		//private Rectangle _clientRectangle;
		//private Rectangle _clipRectangle;
		private DrawMode _drawMode;

		private object _tag;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="DrawArgs"/> with
		/// the specified rendering surface, client rectangle,
		/// clip rectangle and drawing mode.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="drawMode"></param>
		public DrawArgs(
			IRenderingSurface surface, 
			DrawMode drawMode)
		{
			_renderingSurface = surface;
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
		/// Gets the rendering surface.
		/// </summary>
		public IRenderingSurface RenderingSurface
		{
			get { return _renderingSurface; }
		}

		/// <summary>
		/// Gets the <see cref="DrawMode"/>.
		/// </summary>
		public DrawMode DrawMode
		{
			get { return _drawMode; }
		}

		/// <summary>
		/// Gets or sets user-defined data.
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
	}
}
