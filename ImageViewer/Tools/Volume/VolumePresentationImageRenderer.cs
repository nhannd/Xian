using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using vtk;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImageRenderer : IRenderer
	{
		private VtkRenderingSurface _surface;
		private bool _propCreated = false;

		public VolumePresentationImageRenderer()
		{

		}

		public vtkRenderWindowInteractor Interactor
		{
			get { return _surface.Interactor; }
		}

		#region IRenderer Members

		public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			if (_surface == null)
				_surface = new VtkRenderingSurface(windowID);
			else
			{
				if (_surface.WindowID == IntPtr.Zero)
					_surface.WindowID = windowID;
			}

			_surface.SetSize(width, height);

			return _surface;
		}

		public void Draw(DrawArgs args)
		{
			if (!_propCreated && _surface != null)
			{
				AddProp(args);
				_propCreated = true;
			}
			_surface.Draw();
		}

		private void AddProp(DrawArgs args)
		{
			VolumePresentationImage volumePresentationImage = args.PresentationImage as VolumePresentationImage;
			vtkProp vtkProp = volumePresentationImage.VtkProp;

			vtk.vtkRenderer renderer = new vtk.vtkRenderer();
			renderer.AddViewProp(vtkProp);
			renderer.SetBackground(0.0f, 0.0f, 0.0f);

			_surface.RenderWindow.AddRenderer(renderer);
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
	}
}
