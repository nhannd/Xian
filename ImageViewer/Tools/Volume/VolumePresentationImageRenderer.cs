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
				RenderVolume(args);
				_volumeCreated = true;
			}
		}

		private void RenderVolume(DrawArgs args)
		{
			VolumePresentationImage volumePresentationImage = args.PresentationImage as VolumePresentationImage;
			vtkActor volumeActor = volumePresentationImage.VolumeActor;

			vtk.vtkRenderer renderer = new vtk.vtkRenderer();
			renderer.AddActor(volumeActor);
			renderer.SetBackground(0.0f, 0.0f, 0.0f);

			_surface.vtkRenderWindow.AddRenderer(renderer);

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
