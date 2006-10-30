using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using vtk;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.StudyManagement;

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
			get { return this.DicomImage.Columns; }
		}

		public int Height
		{
			get { return this.DicomImage.Rows; }
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

		private DicomImageLayer DicomImage
		{
			get
			{
				DicomImageLayer image = (_displaySet.PresentationImages[0] as DicomPresentationImage).ImageLayer;
				return image;
			}
		}

		private vtkActor CreateVolume()
		{
			vtkImageData vtkVolume = CreateVtkVolume();
			vtkUnsignedShortArray vtkVolumeData = BuildVolumeData();
			vtkVolume.GetPointData().SetScalars(vtkVolumeData);

			vtkContourFilter extractor = new vtk.vtkContourFilter();
			extractor.SetInput(vtkVolume);
			extractor.SetValue(0, 1150);

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
			ImageSop imageSop = this.DicomImage.ImageSop;
			vtkImageData volume = new vtkImageData();
			volume.SetDimensions(this.Width, this.Height, this.Depth);
			volume.SetSpacing(imageSop.PixelSpacing.Column, imageSop.PixelSpacing.Row, 3);
			volume.AllocateScalars();
			volume.SetScalarTypeToUnsignedShort();

			return volume;
		}

		private vtkUnsignedShortArray BuildVolumeData()
		{
			ushort[] volumeData = new ushort[this.SizeInVoxels];

			int imageIndex = 0;

			foreach (DicomPresentationImage slice in _displaySet.PresentationImages)
			{
				AddSliceToVolume(volumeData, slice, imageIndex);
				imageIndex++;
			}

			vtkUnsignedShortArray vtkVolumeData = new vtkUnsignedShortArray();
			vtkVolumeData.SetArray(volumeData, volumeData.Length, 1);
			
			return vtkVolumeData;
		}

		private void AddSliceToVolume(ushort[] volumeData, DicomPresentationImage slice, int imageIndex)
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
