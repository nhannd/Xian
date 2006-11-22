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
		private vtkProp _vtkProp;

		#endregion

		public VolumePresentationImage(IDisplaySet displaySet)
		{
			_displaySet = displaySet;

			ValidateSliceData();
			_vtkProp = CreateIsocontourVolume();
			//_vtkProp = CreateVolumeRendering();
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

		public vtkProp VtkProp
		{
			get { return _vtkProp; }
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

		private vtkProp CreateIsocontourVolume()
		{
			vtkImageData imageData = CreateVolumeImageData();

			if (IsDataUnsigned())
				imageData.GetPointData().SetScalars(BuildUnsignedVolumeImageData());
			else
				imageData.GetPointData().SetScalars(BuildSignedVolumeImageData());

			vtkContourFilter extractor = new vtk.vtkContourFilter();
			extractor.SetInput(imageData);
			//extractor.SetValue(0, 100);
			extractor.SetValue(0, 400);
			//extractor.GenerateValues(5, 2, 30);

			vtkPolyDataNormals normals = new vtk.vtkPolyDataNormals();
			normals.SetInputConnection(extractor.GetOutputPort());
			normals.SetFeatureAngle(60.0);

			vtkStripper stripper = new vtk.vtkStripper();
			stripper.SetInputConnection(normals.GetOutputPort());

			vtkPolyDataMapper mapper = new vtk.vtkPolyDataMapper();
			mapper.SetInputConnection(stripper.GetOutputPort());
			//mapper.SetScalarRange(2, 30);
			mapper.ScalarVisibilityOff();

			vtkActor actor = new vtk.vtkActor();
			actor.SetMapper(mapper);
			actor.GetProperty().SetDiffuseColor(1, 1, .9412);
			//skin.GetProperty().SetDiffuseColor(1, .49, .25);
			actor.GetProperty().SetSpecular(.3);
			actor.GetProperty().SetSpecularPower(20);

			return actor;
		}

		private vtkProp CreateVolumeRendering()
		{
			vtkImageData imageData = CreateVolumeImageData();

			if (IsDataUnsigned())
				imageData.GetPointData().SetScalars(BuildUnsignedVolumeImageData());
			else
				imageData.GetPointData().SetScalars(BuildSignedVolumeImageData());

			int window = 500;
			int level = -240 +1024;

			vtkPiecewiseFunction opacityTransferFunction = new vtkPiecewiseFunction();
			//opacityTransferFunction.AddSegment(level - window / 2, 1.0, level + window / 2, 1.0);
			opacityTransferFunction.AddPoint(level - window / 2, 0.0);
			opacityTransferFunction.AddPoint(level, 1.0);
			opacityTransferFunction.AddPoint(level + window / 2, 0.0);
			//opacityTransferFunction.AddSegment(900, 1.0, 4000, 1.0);
			opacityTransferFunction.ClampingOff();

			vtkColorTransferFunction colorMap = new vtkColorTransferFunction();
			//vtkPiecewiseFunction colorMap = new vtkPiecewiseFunction();
			//colorMap.AddRGBSegment(0, 0, 0, 0, 2000, 255, 255, 128);
			colorMap.SetColorSpaceToRGB();

			//colorMap.AddPoint(level-window/2, 0);
			//colorMap.AddPoint(level+window/2, 255);
			//colorMap.AddRGBPoint(level - window / 2, 200.0 / 255.0f, 4.0 / 255.0f, 10.0 / 255.0f);
			//colorMap.AddRGBPoint(level, 200, 4, 10);
			//colorMap.AddRGBPoint(level + window / 2, 255.0 / 255.0f, 255.0 / 255.0f, 128.0 / 255.0f);
			colorMap.AddRGBSegment(level - window / 2, 200.0/255.0f, 4.0/255.0f, 10.0/255.0f, level + window / 2, 255.0/255.0f, 255.0/255.0f, 128.0/255.0f);
			colorMap.ClampingOff();

			vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
			volumeProperty.ShadeOn();
			volumeProperty.SetInterpolationTypeToLinear();
			volumeProperty.SetColor(colorMap);
			volumeProperty.SetScalarOpacity(opacityTransferFunction);
			volumeProperty.SetDiffuse(0.7);
			volumeProperty.SetAmbient(0.1);
			volumeProperty.SetSpecular(.3);
			volumeProperty.SetSpecularPower(20);

			//int numChannels = volumeProperty.GetColorChannels();

			//vtkPiecewiseFunction grayFunction = volumeProperty.GetGrayTransferFunction();
			//vtkColorTransferFunction colorFunction = volumeProperty.GetRGBTransferFunction();

			//vtkOpenGLVolumeTextureMapper3D volumeMapper = new vtkOpenGLVolumeTextureMapper3D();
			//volumeMapper.SetPreferredMethodToNVidia();
			//volumeMapper.SetInput(imageData);

			vtkFixedPointVolumeRayCastMapper volumeMapper = new vtkFixedPointVolumeRayCastMapper();
			volumeMapper.SetInput(imageData);

			vtkVolume volume = new vtkVolume();
			volume.SetMapper(volumeMapper);
			volume.SetProperty(volumeProperty);

			return volume;
		}

		private vtkImageData CreateVolumeImageData()
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

		private vtkUnsignedShortArray BuildUnsignedVolumeImageData()
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

		private vtkShortArray BuildSignedVolumeImageData()
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

				short val = (short)((highbyte << 8) | lowbyte);

//				if (val >= 0)
					volumeData[i] = val;
//				else
//					volumeData[i] = 0;

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
			if (disposing && _vtkProp != null)
			{
				_vtkProp.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
