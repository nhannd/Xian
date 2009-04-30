#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
		/// Gets or sets user-defined data.
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
	}
}
