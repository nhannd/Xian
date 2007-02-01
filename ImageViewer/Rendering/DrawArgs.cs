using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	public enum DrawMode
	{
		Render = 0,
		Refresh = 1
	}

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

		public CompositeGraphic SceneGraph
		{
			get { return _sceneGraph; }
			internal set { _sceneGraph = value; }
		}

		public IPresentationImage PresentationImage
		{
			get { return _presentationImage; }
			internal set { _presentationImage = value; }
		}

		public ITile Tile
		{
			get { return _tile; }
			internal set { _tile = value; }
		}

		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
			internal set { _displaySet = value; }
		}

		public IImageBox ImageBox
		{
			get { return _imageBox; }
			internal set { _imageBox = value; }
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public IRenderingSurface RenderingSurface
		{
			get { return _renderingSurface; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
		}

		public DrawMode DrawMode
		{
			get { return _drawMode; }
		}
	}
}
