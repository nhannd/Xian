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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using vtk;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
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

			_surface.ClientRectangle = new Rectangle(0, 0, width, height);

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
			IAssociatedTissues volume = args.SceneGraph.ParentPresentationImage as IAssociatedTissues;

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
