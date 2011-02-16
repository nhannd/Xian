#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Desktop;

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

		private readonly DrawMode _drawMode;
		private readonly IRenderingSurface _renderingSurface;
		private CompositeGraphic _sceneGraph;
		private readonly Screen _screen;
		private float _dpi = 96;
		private object _tag;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="DrawArgs"/>.
		/// </summary>
		public DrawArgs(
			IRenderingSurface surface, 
			Screen screen,
			DrawMode drawMode)
		{
			_renderingSurface = surface;
			_screen = screen;
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
		/// Gets the <see cref="ClearCanvas.ImageViewer.Rendering.DrawMode"/>.
		/// </summary>
		public DrawMode DrawMode
		{
			get { return _drawMode; }
		}

		/// <summary>
		/// Gets information about the screen on which the <see cref="DrawArgs.SceneGraph"/>
		/// will be drawn.
		/// </summary>
		/// <remarks>
		/// If the tile to be drawn straddles two screens, the information returned
		/// will be that of the screen on which the larger portion of the <see cref="Tile"/>
		/// resides.
		/// </remarks>
		public Screen Screen
		{
			get { return _screen; }
		}

		/// <summary>
		/// Gets or sets the resolution of the output device in DPI to be used in scaling vector graphics appropriately.
		/// </summary>
		/// <remarks>
		/// The vector graphics are drawn for display at a nominal screen resolution (usually 96 DPI). If the destination
		/// output device has a significantly larger DPI, setting this value will allow font sizes and line widths
		/// to be scaled appropriately so that the size of the vector graphics relative to the rest of the image remains
		/// relatively constant.
		/// </remarks>
		public float Dpi
		{
			get { return _dpi; }
			set
			{
				Platform.CheckPositive(value, "Dpi");
				_dpi = value;
			}
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
