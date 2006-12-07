using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using vtk;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VolumePresentationImage : PresentationImage
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

		public override IRenderer ImageRenderer
		{
			get 
			{
				if (_imageRenderer == null)
					_imageRenderer = new VolumePresentationImageRenderer();

				return _imageRenderer;
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
			get { return GetDicomImageLayer().Columns; }
		}

		public int Height
		{
			get { return GetDicomImageLayer().Rows; }
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
			get { return GetImageSop().RescaleSlope; }
		}

		public double RescaleIntercept
		{
			get { return GetImageSop().RescaleIntercept; }
		}

		#endregion

		public override IPresentationImage Clone()
		{
			return new VolumePresentationImage(_displaySet);
		}

		private DicomPresentationImage GetDicomPresentationImage(int i)
		{
			return _displaySet.PresentationImages[i] as DicomPresentationImage;
		}

		private DicomPresentationImage GetDicomPresentationImage()
		{
			return GetDicomPresentationImage(0);
		}

		private ImageSop GetImageSop()
		{
			return GetDicomPresentationImage().ImageSop;
		}

		private DicomImageLayer GetDicomImageLayer()
		{
			return GetDicomPresentationImage().ImageLayer;
		}

		private bool IsDataUnsigned()
		{
			return (GetImageSop().PixelRepresentation == 0);
		}


		private vtkImageData CreateVolumeImageData()
		{
			vtkImageData imageData = new vtkImageData();
			imageData.SetDimensions(this.Width, this.Height, this.Depth);
			imageData.SetSpacing(GetImageSop().PixelSpacing.Column, GetImageSop().PixelSpacing.Row, GetSliceSpacing());
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
				foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
				{
					AddUnsignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}
			else
			{
				FindMinimumPixelValue();

				foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
				{
					AddSignedSliceToVolume(volumeData, slice, imageIndex);
					imageIndex++;
				}
			}

			vtkUnsignedShortArray vtkVolumeData = new vtkUnsignedShortArray();
			vtkVolumeData.SetArray(volumeData, volumeData.Length, 1);
			
			return vtkVolumeData;
		}

		private void FindMinimumPixelValue()
		{
			_minimumPixelValue = short.MaxValue;

			foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
			{
				byte[] sliceData = slice.ImageLayer.GetPixelData();
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

		private void AddUnsignedSliceToVolume(ushort[] volumeData, DicomPresentationImage slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageLayer.GetPixelData();
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

		private void AddSignedSliceToVolume(ushort[] volumeData, DicomPresentationImage slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageLayer.GetPixelData();
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
				ImageSop slice1 = GetDicomPresentationImage(0).ImageSop;
				ImageSop slice2 = GetDicomPresentationImage(1).ImageSop;
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
