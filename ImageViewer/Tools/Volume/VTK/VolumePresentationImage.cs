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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public class VolumePresentationImage : PresentationImage, IAssociatedTissues
	{
		#region Private fields

		private IDisplaySet _displaySet;
		private vtkImageData _vtkImageData;
		private short _minimumPixelValue;
		
		#endregion

		public VolumePresentationImage(IDisplaySet displaySet)
		{
			_displaySet = displaySet;

			ValidateSliceData();
		}


		#region Public properties

		#region IAssociatedTissues Members

		public GraphicCollection TissueLayers
		{
			get { return this.SceneGraph.Graphics; }
		}

		#endregion

		public override IRenderer ImageRenderer
		{
			get 
			{
				if (base.ImageRenderer == null)
					base.ImageRenderer = new VolumePresentationImageRenderer();

				return base.ImageRenderer;
			}
		}

		public vtkImageData VtkImageData
		{
			get 
			{
				if (_vtkImageData == null)
					_vtkImageData = CreateVolumeImageData();

				return _vtkImageData; 
			}
		}

		public short MinimumPixelValue
		{
			get { return _minimumPixelValue; }
		}

		public int Width
		{
			get { return GetImageGraphic().Columns; }
		}

		public int Height
		{
			get { return GetImageGraphic().Rows; }
		}

		public int Depth
		{
			get { return _displaySet.PresentationImages.Count; }
		}

		public int SizeInVoxels
		{
			get { return this.Width * this.Height * this.Depth; }
		}

		public double RescaleSlope
		{
			get { return GetFirstFrame().RescaleSlope; }
		}

		public double RescaleIntercept
		{
			get { return GetFirstFrame().RescaleIntercept; }
		}

		#endregion

		public override IPresentationImage CreateFreshCopy()
		{
			return new VolumePresentationImage(_displaySet);
		}

		private IPresentationImage GetDicomPresentationImage(int i)
		{
			return _displaySet.PresentationImages[i];
		}

		private IPresentationImage GetDicomPresentationImage()
		{
			return GetDicomPresentationImage(0);
		}

		private ImageSop GetImageSop()
		{
			return (GetDicomPresentationImage() as IImageSopProvider).ImageSop;
		}

		private ImageGraphic GetImageGraphic()
		{
			return (GetDicomPresentationImage() as IImageGraphicProvider).ImageGraphic;
		}

		private Frame GetFirstFrame()
		{
			return GetImageSop().Frames[1];
		}

		private bool IsDataUnsigned()
		{
			return (GetFirstFrame().PixelRepresentation == 0);
		}

		private vtkImageData CreateVolumeImageData()
		{
			vtkImageData imageData = new vtkImageData();
			imageData.SetDimensions(this.Width, this.Height, this.Depth);
			imageData.SetSpacing(GetFirstFrame().PixelSpacing.Column, GetFirstFrame().PixelSpacing.Row, GetSliceSpacing());
			imageData.AllocateScalars();
			imageData.SetScalarTypeToUnsignedShort();
			imageData.GetPointData().SetScalars(BuildVolumeImageData());
			
			return imageData;
		}

		private vtkUnsignedShortArray BuildVolumeImageData()
		{
			ushort[] volumeData = new ushort[this.SizeInVoxels];

			int imageIndex = 0;

			if (IsDataUnsigned())
			{
				foreach (IImageGraphicProvider slice in _displaySet.PresentationImages)
				{
					AddUnsignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}
			else
			{
				FindMinimumPixelValue();

				foreach (IImageGraphicProvider slice in _displaySet.PresentationImages)
				{
					AddSignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}

			vtkUnsignedShortArray vtkVolumeData = new vtkUnsignedShortArray();
			vtkVolumeData.SetArray(volumeData, new VtkIdType(volumeData.Length), 1);
			
			return vtkVolumeData;
		}

		private void FindMinimumPixelValue()
		{
			_minimumPixelValue = short.MaxValue;

			foreach (IImageGraphicProvider slice in _displaySet.PresentationImages)
			{
				byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
				int length = sliceData.Length / 2;

				for (int i = 0; i < length; i+=2)
				{
					ushort lowbyte = sliceData[i];
					ushort highbyte = sliceData[i + 1];
					short pixelValue = (short)((highbyte << 8) | lowbyte);

					if (pixelValue < _minimumPixelValue)
						_minimumPixelValue = pixelValue;
				}
			}
		}

		private void AddUnsignedSliceToVolume(ushort[] volumeData, IImageGraphicProvider slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
			int start = imageIndex * sliceData.Length / 2;
			int end = start + sliceData.Length / 2;

			int j = 0;

			for (int i = start; i < end ; i++)
			{
				ushort lowbyte = sliceData[j];
				ushort highbyte = sliceData[j + 1];
				volumeData[i] = (ushort)((highbyte << 8) | lowbyte);
				j += 2;
			}
		}

		private void AddSignedSliceToVolume(ushort[] volumeData, IImageGraphicProvider slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageGraphic.PixelData.Raw;
			int start = imageIndex * sliceData.Length / 2;
			int end = start + sliceData.Length / 2;

			int j = 0;

			for (int i = start; i < end; i++)
			{
				ushort lowbyte = sliceData[j];
				ushort highbyte = sliceData[j + 1];

				short val = (short)((highbyte << 8) | lowbyte);
				volumeData[i] = (ushort)(val - _minimumPixelValue);

				j += 2;
			}
		}

		private void ValidateSliceData()
		{
		}

		private double GetSliceSpacing()
		{
			if (_displaySet.PresentationImages.Count > 1)
			{
				Frame slice1 = (GetDicomPresentationImage(0) as IImageSopProvider).ImageSop.Frames[1];
				Frame slice2 = (GetDicomPresentationImage(1) as IImageSopProvider).ImageSop.Frames[1];
				double sliceSpacing = Math.Abs(slice2.ImagePositionPatient.Z - slice1.ImagePositionPatient.Z);

				return sliceSpacing;
			}
			else
			{
				return 0.0d;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _vtkImageData != null)
			{
				_vtkImageData.Dispose();
			}

			base.Dispose(disposing);
		}

	}
}
