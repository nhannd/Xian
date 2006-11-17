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
		private vtkActor _volumeActor;

		#endregion

		public VolumePresentationImage(IDisplaySet displaySet)
		{
			_displaySet = displaySet;

			ValidateSliceData();
			_volumeActor = CreateVolume();
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

		public vtkActor VolumeActor
		{
			get { return _volumeActor; }
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

		#endregion

		public override IPresentationImage Clone()
		{
			return null;
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

		private vtkActor CreateVolume()
		{
			vtkImageData vtkVolume = CreateVtkVolume();

			if (IsDataUnsigned())
				vtkVolume.GetPointData().SetScalars(BuildUnsignedVolumeData());
			else
				vtkVolume.GetPointData().SetScalars(BuildSignedVolumeData());

			vtkContourFilter extractor = new vtk.vtkContourFilter();
			extractor.SetInput(vtkVolume);
			extractor.SetValue(0, 450);
			//extractor.SetValue(0, 1450);

			vtkPolyDataNormals normals = new vtk.vtkPolyDataNormals();
			normals.SetInputConnection(extractor.GetOutputPort());
			normals.SetFeatureAngle(60.0);
			
			vtkStripper stripper = new vtk.vtkStripper();
			stripper.SetInputConnection(normals.GetOutputPort());
			
			vtkPolyDataMapper mapper = new vtk.vtkPolyDataMapper();
			mapper.SetInputConnection(stripper.GetOutputPort());
			mapper.ScalarVisibilityOff();

			vtkActor actor = new vtk.vtkActor();
			actor.SetMapper(mapper);
			actor.GetProperty().SetDiffuseColor(1, 1, .9412);
			//skin.GetProperty().SetDiffuseColor(1, .49, .25);
			actor.GetProperty().SetSpecular(.3);
			actor.GetProperty().SetSpecularPower(20);

			return actor;
		}

		private vtkImageData CreateVtkVolume()
		{
			vtkImageData volume = new vtkImageData();
			volume.SetDimensions(this.Width, this.Height, this.Depth);
			volume.SetSpacing(GetImageSop().PixelSpacing.Column, GetImageSop().PixelSpacing.Row, GetSliceSpacing());
			volume.AllocateScalars();

			if (IsDataUnsigned())
				volume.SetScalarTypeToUnsignedShort();
			else
				volume.SetScalarTypeToShort();

			return volume;
		}

		private vtkUnsignedShortArray BuildUnsignedVolumeData()
		{
			ushort[] volumeData = new ushort[this.SizeInVoxels];

			int imageIndex = 0;

			foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
			{
				AddUnsignedSliceToVolume(volumeData, slice, imageIndex);
				imageIndex++;
			}

			vtkUnsignedShortArray vtkVolumeData = new vtkUnsignedShortArray();
			vtkVolumeData.SetArray(volumeData, volumeData.Length, 1);
			
			return vtkVolumeData;
		}

		private vtkShortArray BuildSignedVolumeData()
		{
			short[] volumeData = new short[this.SizeInVoxels];

			int imageIndex = 0;

			foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
			{
				AddSignedSliceToVolume(volumeData, slice, imageIndex);
				imageIndex++;
			}

			vtkShortArray vtkVolumeData = new vtkShortArray();
			vtkVolumeData.SetArray(volumeData, volumeData.Length, 1);

			return vtkVolumeData;
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

		private void AddSignedSliceToVolume(short[] volumeData, DicomPresentationImage slice, int imageIndex)
		{
			byte[] sliceData = slice.ImageLayer.GetPixelData();
			int start = imageIndex * sliceData.Length / 2;
			int end = start + sliceData.Length / 2;

			int j = 0;

			for (int i = start; i < end; i++)
			{
				ushort lowbyte = sliceData[j];
				ushort highbyte = sliceData[j + 1];
				volumeData[i] = (short)((highbyte << 8) | lowbyte);
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
			if (disposing && _volumeActor != null)
			{
				_volumeActor.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
