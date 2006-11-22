using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using vtk;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImageRenderer : IRenderer
	{
		private VtkRenderingSurface _surface;
		private bool _propsCreated = false;

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
				_surface.WindowID = windowID;

			_surface.SetSize(width, height);

			return _surface;
		}

		public void Draw(DrawArgs args)
		{
			if (!_propsCreated && _surface != null)
			{
				AddLayers(args);
				_propsCreated = true;
			}
			_surface.Draw();
		}

		private void AddLayers(DrawArgs args)
		{
			vtk.vtkRenderer renderer = new vtk.vtkRenderer();
			renderer.SetBackground(0.0f, 0.0f, 0.0f);

			LayerCollection layers = args.PresentationImage.LayerManager.RootLayerGroup.Layers;

			foreach (VolumeLayer volumeLayer in layers)
			{
				renderer.AddViewProp(volumeLayer.VtkProp);
			}

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
