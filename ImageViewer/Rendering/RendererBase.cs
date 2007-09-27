using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Allows Renderers to publish the total time elapsed for a particular method to aid in 
	/// debugging and optimization.
	/// </summary>
	public static class RenderPerformanceReportBroker
	{
		/// <summary>
		/// A Delegate for publishing performance of a method.
		/// </summary>
		public delegate void PerformanceReportDelegate(string methodName, double totalTime);

		/// <summary>
		/// A Delegate that can be subscribed to in order to receive performance reports.
		/// </summary>
		public static PerformanceReportDelegate PerformanceReport;
		
		/// <summary>
		/// Called from within a method to publish performance reports to subscribers.
		/// </summary>
		public static void PublishPerformanceReport(string methodName, double totalTime)
		{
			if (PerformanceReport == null)
				return;
			
			EventsHelper.Fire(PerformanceReport, methodName, totalTime);
		}
	}

	/// <summary>
	/// A Template class providing the base functionality for an <see cref="IRenderer"/>.
	/// </summary>
	public abstract class RendererBase : IRenderer
	{
		#region Internal Renderer Template Class

		/// <summary>
		/// A Template for the <b>Internal Renderer</b> used by the parent <see cref="IRenderer"/>, in this case, <see cref="RendererBase"/>.
		/// </summary>
		protected abstract class InternalRenderer
		{
			private readonly DrawMode _drawMode;
			private readonly CompositeGraphic _sceneGraph;

			/// <summary>
			/// Constructor.
			/// </summary>
			protected InternalRenderer(DrawMode drawMode, CompositeGraphic sceneGraph)
			{
				Platform.CheckForNullReference(sceneGraph, "sceneGraph");

				_drawMode = drawMode;
				_sceneGraph = sceneGraph;
			}

			private InternalRenderer()
			{
			}

			/// <summary>
			/// Gets the <see cref="DrawMode"/>
			/// </summary>
			protected DrawMode DrawMode
			{
				get { return _drawMode; }
			}

			/// <summary>
			/// Gets the <b>SceneGraph</b> that is to be rendered.
			/// </summary>
			protected CompositeGraphic SceneGraph
			{
				get { return _sceneGraph; }
			}

			/// <summary>
			/// Called by <see cref="RendererBase.Draw"/>.  It should not normally be necessary to override this method.
			/// </summary>
			public virtual void Draw()
			{
				try
				{
					if (DrawMode == DrawMode.Render)
						Render();
					else
						Refresh();
				}
				catch(Exception e)
				{
					ShowErrorMessage(e.Message);
				}
			}

			/// <summary>
			/// Traverses and draws the scene graph.  Inheritors should override 
			/// this method to do any necessary work before calling the base method.
			/// </summary>
			protected virtual void Render()
			{
				DrawSceneGraph(SceneGraph);
				DrawTextOverlay(SceneGraph.ParentPresentationImage);
			}

			/// <summary>
			/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
			/// Inheritors must implement this method.
			/// </summary>
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
						else if (graphic is RectanglePrimitive)
							DrawRectanglePrimitive((RectanglePrimitive)graphic);
						else if (graphic is PointPrimitive)
							DrawPointPrimitive((PointPrimitive)graphic);
						else if (graphic is InvariantRectanglePrimitive)
							DrawInvariantRectanglePrimitive((InvariantRectanglePrimitive)graphic);
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
				if (layout == null)
					return;

				foreach (AnnotationBox annotationBox in layout.AnnotationBoxes)
				{
					string annotationText = annotationBox.GetAnnotationText(presentationImage);
					if (!String.IsNullOrEmpty(annotationText))
						DrawAnnotationBox(annotationText, annotationBox);
				}

				clock.Stop();
				RenderPerformanceReportBroker.PublishPerformanceReport("RendererBase.DrawTextOverlay", clock.Seconds);
			}

			/// <summary>
			/// Draws a <see cref="ImageGraphic"/>.  Must be overridden and implemented.
			/// </summary>
			protected abstract void DrawImageGraphic(ImageGraphic imageGraphic);

			/// <summary>
			/// Draws a <see cref="LinePrimitive"/>.  Must be overridden and implemented.
			/// </summary>
			protected abstract void DrawLinePrimitive(LinePrimitive line);

			/// <summary>
			/// Draws a <see cref="RectanglePrimitive"/>.  Must be overridden and implemented.
			/// </summary>
			protected abstract void DrawRectanglePrimitive(RectanglePrimitive rect);

			/// <summary>
			/// Draws a <see cref="PointPrimitive"/>.  Must be overridden and implemented.
			/// </summary>
			protected abstract void DrawPointPrimitive(PointPrimitive pointPrimitive);

			/// <summary>
			/// Draws an <see cref="InvariantRectanglePrimitive"/>.  Must be overridden and implemented.
			/// </summary>
			protected abstract void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect);

			/// <summary>
			/// Draws a <see cref="InvariantTextPrimitive"/>.  Must be overridden and implemented.
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
		}

		#endregion

		/// <summary>
		/// Factory method for an <see cref="InternalRenderer"/>.  The returned object should be thread-safe.
		/// </summary>
		protected abstract InternalRenderer GetInternalRenderer(DrawArgs drawArgs);

		#region IRenderer Members

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public abstract IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height);

		/// <summary>
		/// Called by the framework to draw the Scene Graph.
		/// </summary>
		public void Draw(DrawArgs args)
		{
			InternalRenderer internalRenderer = GetInternalRenderer(args);
			if (internalRenderer == null)
				return;

			try 
			{
				internalRenderer.Draw();
			}
			finally
			{
				if (internalRenderer is IDisposable)
					((IDisposable)internalRenderer).Dispose();
			}
		}

		#endregion
	}
}
