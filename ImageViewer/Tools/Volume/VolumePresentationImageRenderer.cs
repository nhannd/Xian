using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImageRenderer : IRenderer
	{
		private VtkRenderingSurface _surface;
		private bool _volumeCreated = false;

		public VolumePresentationImageRenderer()
		{

		}

		#region IRenderer Members

		public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			if (_surface == null)
				_surface = new VtkRenderingSurface(windowID);

			_surface.SetSize(width, height);

			return _surface;
		}

		public void Draw(DrawArgs args)
		{
			if (!_volumeCreated && _surface != null)
			{
				AddConeToWindow(_surface.vtkRenderWindow);
				_volumeCreated = true;
			}
		}

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

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing && _surface != null)
			{
				_surface.Dispose();
			}
		}

		private void AddConeToWindow(vtkRenderWindow renWin)
		{
			// 
			// Next we create an instance of vtkConeSource and set some of its
			// properties. The instance of vtkConeSource "cone" is part of a visualization
			// pipeline (it is a source process object); it produces data (output type is
			// vtkPolyData) which other filters may process.
			//
			vtk.vtkConeSource cone = new vtk.vtkConeSource();
			cone.SetHeight(2.0f);
			cone.SetRadius(1.0f);
			cone.SetResolution(200);

			// 
			// In this example we terminate the pipeline with a mapper process object.
			// (Intermediate filters such as vtkShrinkPolyData could be inserted in
			// between the source and the mapper.)  We create an instance of
			// vtkPolyDataMapper to map the polygonal data into graphics primitives. We
			// connect the output of the cone souece to the input of this mapper.
			//
			vtk.vtkPolyDataMapper coneMapper = new vtk.vtkPolyDataMapper();
			coneMapper.SetInput(cone.GetOutput());

			// 
			// Create an actor to represent the cone. The actor orchestrates rendering of
			// the mapper's graphics primitives. An actor also refers to properties via a
			// vtkProperty instance, and includes an internal transformation matrix. We
			// set this actor's mapper to be coneMapper which we created above.
			//
			vtk.vtkActor coneActor = new vtk.vtkActor();
			coneActor.SetMapper(coneMapper);

			//
			// Create the Renderer and assign actors to it. A renderer is like a
			// viewport. It is part or all of a window on the screen and it is
			// responsible for drawing the actors it has.  We also set the background
			// color here
			//
			vtk.vtkRenderer ren1 = new vtk.vtkRenderer();
			ren1.AddActor(coneActor);
			ren1.SetBackground(0.0f, 0.0f, 0.0f);

			//
			// Finally we create the render window which will show up on the screen
			// We put our renderer into the render window using AddRenderer. We also
			// set the size to be 300 pixels by 300
			//
			renWin.AddRenderer(ren1);

			//vtk.vtkRenderWindowInteractor iren = renWin.GetInteractor();

			//{
			//    m_boxWidget = new vtk.vtkBoxWidget();
			//    m_boxWidget.SetInteractor(iren);
			//    m_boxWidget.SetPlaceFactor(1.25f);

			//    m_boxWidget.SetProp3D(coneActor);
			//    m_boxWidget.PlaceWidget();

			//    m_boxWidget.AddObserver((uint)vtk.EventIds.InteractionEvent,
			//        new vtk.vtkDotNetCallback(myCallback));

			//    m_boxWidget.On();
			//}
		}
	}
}
