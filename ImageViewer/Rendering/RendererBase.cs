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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A Template class providing the base functionality for an <see cref="IRenderer"/>.
	/// </summary>
	/// <remarks>
	/// See the remarks section for <see cref="RendererFactoryBase"/> regarding the 
	/// thread-safety of this object (it is not thread-safe).  For this reason, you should
	/// use this class in tandem with the <see cref="RendererFactoryBase"/>, although it
	/// is not required that you do so.
	/// </remarks>
	public abstract class RendererBase : IRenderer
	{
		private DrawMode _drawMode;
		private CompositeGraphic _sceneGraph;
		private IRenderingSurface _surface;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected RendererBase()
		{
		}

		/// <summary>
		/// Gets the <see cref="ClearCanvas.ImageViewer.Rendering.DrawMode"/>.
		/// </summary>
		protected DrawMode DrawMode
		{
			get { return _drawMode; }
			set { _drawMode = value; }
		}

		/// <summary>
		/// Gets the <b>SceneGraph</b> that is to be rendered.
		/// </summary>
		protected CompositeGraphic SceneGraph
		{
			get { return _sceneGraph; }
			set { _sceneGraph = value; }
		}

		/// <summary>
		/// Gets the <see cref="IRenderingSurface"/> that is to be rendered upon.
		/// </summary>
		protected IRenderingSurface Surface
		{
			get { return _surface; }
			set { _surface = value; }
		}

		/// <summary>
		/// Draws the <see cref="IPresentationImage"/> passed in through the <see cref="DrawArgs"/>.
		/// </summary>
		/// <remarks>
		/// This method is called by the <see cref="PresentationImage"/> whenever
		/// <see cref="IDrawable.Draw"/> is called.  If you are implementing
		/// your own renderer, <see cref="DrawArgs"/> contains all you need to 
		/// know to perform the rendering, such as the <see cref="IRenderingSurface"/>, etc.  
		/// </remarks>
		public virtual void Draw(DrawArgs drawArgs)
		{
			try
			{
				Initialize(drawArgs); 
				
				if (drawArgs.RenderingSurface.ClientRectangle.Width == 0 || drawArgs.RenderingSurface.ClientRectangle.Height == 0)
					return;
								
				if (DrawMode == DrawMode.Render)
					Render();
				else
					Refresh();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				ShowErrorMessage(e.Message);
			}
		}

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public abstract IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height);

		/// <summary>
		/// Initializes the member variables before calling <see cref="Render"/> or <see cref="Refresh"/>.
		/// </summary>
		protected virtual void Initialize(DrawArgs drawArgs)
		{
			_drawMode = drawArgs.DrawMode;
			_sceneGraph = drawArgs.SceneGraph;
			_surface = drawArgs.RenderingSurface;
		}

		/// <summary>
		/// Traverses and draws the scene graph.  
		/// </summary>
		/// <remarks>
		/// Inheritors should override this method to do any necessary work before calling the base method.
		/// </remarks>
		protected virtual void Render()
		{
			DrawSceneGraph(SceneGraph);
			DrawTextOverlay(SceneGraph.ParentPresentationImage);
		}

		/// <summary>
		/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
		/// </summary>
		/// <remarks>
		/// Inheritors must implement this method.
		/// </remarks>
		protected abstract void Refresh();

		/// <summary>
		/// Traverses and Renders the Scene Graph.
		/// </summary>
		protected void DrawSceneGraph(CompositeGraphic sceneGraph)
		{
			foreach (Graphic graphic in sceneGraph.Graphics)
			{
				if (graphic.Visible)
				{
					graphic.OnDrawing();

					if (graphic is CompositeGraphic)
						DrawSceneGraph((CompositeGraphic)graphic);
					else if (graphic is ImageGraphic)
						DrawImageGraphic((ImageGraphic)graphic);
					else if (graphic is LinePrimitive)
						DrawLinePrimitive((LinePrimitive)graphic);
					else if (graphic is InvariantLinePrimitive)
						DrawInvariantLinePrimitive((InvariantLinePrimitive)graphic);
					else if (graphic is CurvePrimitive)
						DrawCurvePrimitive((CurvePrimitive)graphic);
					else if (graphic is RectanglePrimitive)
						DrawRectanglePrimitive((RectanglePrimitive)graphic);
					else if (graphic is InvariantRectanglePrimitive)
						DrawInvariantRectanglePrimitive((InvariantRectanglePrimitive)graphic);
					else if (graphic is EllipsePrimitive)
						DrawEllipsePrimitive((EllipsePrimitive)graphic);
					else if (graphic is InvariantEllipsePrimitive)
						DrawInvariantEllipsePrimitive((InvariantEllipsePrimitive)graphic);
					else if (graphic is ArcPrimitive)
						DrawArcPrimitive((IArcGraphic)graphic);
					else if (graphic is InvariantArcPrimitive)
						DrawArcPrimitive((IArcGraphic)graphic);
					else if (graphic is PointPrimitive)
						DrawPointPrimitive((PointPrimitive)graphic);
					else if (graphic is InvariantTextPrimitive)
						DrawTextPrimitive((InvariantTextPrimitive)graphic);
				}
			}
		}

		/// <summary>
		/// Draws the Text Overlay.
		/// </summary>
		protected void DrawTextOverlay(IPresentationImage presentationImage)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (presentationImage == null || !(presentationImage is IAnnotationLayoutProvider))
				return;

			IAnnotationLayout layout = ((IAnnotationLayoutProvider)presentationImage).AnnotationLayout;
			if (layout == null || !layout.Visible)
				return;

			foreach (AnnotationBox annotationBox in layout.AnnotationBoxes)
			{
				if (annotationBox.Visible)
				{
					string annotationText = annotationBox.GetAnnotationText(presentationImage);
					if (!String.IsNullOrEmpty(annotationText))
						DrawAnnotationBox(annotationText, annotationBox);
				}
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("RendererBase", "DrawTextOverlay", clock.Seconds);
		}

		/// <summary>
		/// Draws an <see cref="ImageGraphic"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawImageGraphic(ImageGraphic imageGraphic);

		/// <summary>
		/// Draws a <see cref="LinePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawLinePrimitive(LinePrimitive line);

		/// <summary>
		/// Draws a <see cref="InvariantLinePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantLinePrimitive(InvariantLinePrimitive line);

		/// <summary>
		/// Draws a <see cref="CurvePrimitive"/>. Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawCurvePrimitive(CurvePrimitive curve);

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawRectanglePrimitive(RectanglePrimitive rectangle);

		/// <summary>
		/// Draws a <see cref="InvariantRectanglePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rectangle);

		/// <summary>
		/// Draws a <see cref="EllipsePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawEllipsePrimitive(EllipsePrimitive ellipse);

		/// <summary>
		/// Draws a <see cref="InvariantEllipsePrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse);

		/// <summary>
		/// Draws a <see cref="ArcPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawArcPrimitive(IArcGraphic arc);

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawPointPrimitive(PointPrimitive pointPrimitive);

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawTextPrimitive(InvariantTextPrimitive textPrimitive);

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.  Must be overridden and implemented.
		/// </summary>
		protected abstract void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox);

		/// <summary>
		/// Draws an error message in the Scene Graph's client area of the screen.
		/// </summary>
		protected abstract void ShowErrorMessage(string message);

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Dispose method.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
		#endregion
	}
}