using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using vtk;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImageRenderer : IRenderer
	{
		private VtkRenderingSurface _surface;
		private vtkRenderer _vtkRenderer;

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
			CreateRenderer();
			AddLayers(args);
			_surface.Draw();
		}

		#endregion

		private void CreateRenderer()
		{
			if (_vtkRenderer == null)
			{
				_vtkRenderer = new vtk.vtkRenderer();
				_vtkRenderer.SetBackground(0.0f, 0.0f, 0.0f);
				_surface.RenderWindow.AddRenderer(_vtkRenderer);
			}
		}

		private void AddLayers(DrawArgs args)
		{
			IAssociatedTissues volume = args.PresentationImage as IAssociatedTissues;

			if (volume == null)
				return;

			GraphicCollection layers = volume.TissueLayers;
			vtkPropCollection props = _vtkRenderer.GetViewProps();

			foreach (VolumeGraphic volumeGraphic in layers)
			{
				if (props.IsItemPresent(volumeGraphic.VtkProp) == 0)
					_vtkRenderer.AddViewProp(volumeGraphic.VtkProp);

				//if (volumeLayer.OldVtkProp != null)
				//{

				//    if (props.IsItemPresent(volumeLayer.OldVtkProp) != 0)
				//    {
				//        props.RemoveItem(volumeLayer.OldVtkProp);
				//        volumeLayer.OldVtkProp = null;
				//    }
				//}
			}
		}


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
				Platform.Log(LogLevel.Error, e);
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
