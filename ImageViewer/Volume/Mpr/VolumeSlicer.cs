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

// Define this to enable the experimental slab code. You also need to enable the dependent DLLs in
//	ImageViewer_dis.proj (search for Slab)
//#define SLAB

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Volume utility class aids in extracting 2D slices from a Volume
	/// </summary>
	[System.Obsolete("JY")]
	public class VolumeSlicer
	{
		#region Private fields

		private readonly Volume _volume;

		private VolumeSlicing _slicing;
		private float _sliceSpacing;

		#endregion

		#region Public methods

		public VolumeSlicer(Volume vol)
		{
			_volume = vol;
		}

		public Volume Volume
		{
			get { return _volume; }
		}

		public VolumeSlicing Slicing
		{
			get { return _slicing; }
			set { _slicing = value; }
		}

		#region Slicer Settings

		private float GetSliceSpacing()
		{
			if (_sliceSpacing == 0f)
			{
				_sliceSpacing = _slicing.SliceSpacing;
				if (_sliceSpacing == 0f)
					_sliceSpacing = GetDefaultSpacing();
			}
			return _sliceSpacing;
		}

		#endregion

		#region Create Slice interface

		private ImageSop CreateSliceImageSop(Vector3D point)
		{
			// You must first call one of the SetSlicePlane methods before calling this.
			Debug.Assert(_slicing != null);

			ReslicePoint = point;

			VolumeSliceSopDataSource sliceDicom = new VolumeSliceSopDataSource(_volume.DataSet, this, _slicing.ResliceAxes);

			// ensure each sop has unique Uid
			sliceDicom[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);

			// Update rows and columns to reflect actual output size
			int columns = GetSliceExtentX();
			int rows = GetSliceExtentY();
			sliceDicom[DicomTags.Columns].SetUInt16(0, (ushort)columns);
			sliceDicom[DicomTags.Rows].SetUInt16(0, (ushort)rows);

			// Update Image Orientation (patient)
			//
			Matrix resliceAxesPatientOrientation = _volume.RotateToPatientOrientation(_slicing.ResliceAxes);

			ImageOrientationPatient imageOrientation =
				new ImageOrientationPatient(resliceAxesPatientOrientation[0, 0],
											resliceAxesPatientOrientation[0, 1],
											resliceAxesPatientOrientation[0, 2],
											resliceAxesPatientOrientation[1, 0],
											resliceAxesPatientOrientation[1, 1],
											resliceAxesPatientOrientation[1, 2]);

			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(0, (float)imageOrientation.RowX);
			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(1, (float)imageOrientation.RowY);
			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(2, (float)imageOrientation.RowZ);
			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(3, (float)imageOrientation.ColumnX);
			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(4, (float)imageOrientation.ColumnY);
			sliceDicom[DicomTags.ImageOrientationPatient].SetFloat32(5, (float)imageOrientation.ColumnZ);

			// Update Image Position (patient)
			//
			Vector3D topLeftOfSlicePatient = GetTopLeftOfSlicePatient(columns, rows);

			sliceDicom[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			sliceDicom[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			sliceDicom[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			return new ImageSop(sliceDicom);
		}

		#endregion

		#region Create DisplaySet interface

		//TODO: resolve naming before plane is set.
		private DisplaySet CreateDisplaySet(string displaySetName)
		{
			string name = String.Format("MPR ({0})", displaySetName);
			DisplaySet displaySet = new DisplaySet(name, Guid.NewGuid().ToString());
			displaySet.Description = name;
			PopulateDisplaySetFull(displaySet);
			return displaySet;
		}

		public void PopulateDisplaySetOneImage(IDisplaySet displaySet)
		{
			ReleaseExistingPresImages(displaySet);
			Vector3D throughPoint = GetSliceThroughPoint();
			CreateSliceAndAddToDisplaySet(0, throughPoint, displaySet);
		}

		public void PopulateDisplaySetFull(IDisplaySet displaySet)
		{
			ReleaseExistingPresImages(displaySet);
			Vector3D initialThroughPoint = GetSliceThroughPoint();

			// Determine spacing vector along which we will slice
			Vector3D spacingVector = GetSliceSpacing() * GetSliceNormalVector();
			// I chose to use the volume diagonal magnitude to define the maximum number of slices as
			//	it seemed like a reasonable upper limit to the number of slices in a display set.
			//	Note that in the worst case scenario, this would only generate half the number of possible
			//	slices. Consider the slice plane with normal along the volume diagonal, and through point
			//	on a corner of the volume. This would require the full maxSlices defined here, but because
			//	we start half way in one direction we would only get half of what is possible.
			int maxSlices = (int)(_volume.DiagonalMagnitude / GetSliceSpacing() + 0.5f);

			// Start slicing half way in one direction. Chose to start positive and step
			//	negative as it creates a DisplaySet that is sorted such that the MPR sort
			//	order is consistent with the default 2D sort order. Perhaps in the future
			//	it could be based off of the order of the source DisplaySet.
			Vector3D startPoint = initialThroughPoint + (maxSlices / 2) * spacingVector;

			// Walk along trying to create maxSlices, if a point is outside the volume we'll
			//	skip it, ensuring that all slices are from through points that are in the volume.
			int pointIndex = 0, sliceIndex = 0;
			while(sliceIndex < maxSlices)
			{
				Vector3D throughPoint = startPoint - (pointIndex * spacingVector);
				pointIndex++;

				// Don't generate slice if point is not in volume
				if (_volume.IsPointInVolume(throughPoint) == false)
					// If we've already found some slices, or we've reached maxSlices and haven't
					//	found any, it's time to call it a day
					if (sliceIndex > 0 || pointIndex > maxSlices)
						break;
					else
						// Check next point location
						continue;

				CreateSliceAndAddToDisplaySet(sliceIndex, throughPoint, displaySet);
				sliceIndex++;
			}

			// If the through point is outside volume we want to ensure that at least one slice is generated
			if (sliceIndex == 0)
				CreateSliceAndAddToDisplaySet(sliceIndex, initialThroughPoint, displaySet);
		}

		private static void ReleaseExistingPresImages(IDisplaySet displaySet)
		{
			// Release old images
			foreach (IPresentationImage image in displaySet.PresentationImages)
				image.Dispose();
			displaySet.PresentationImages.Clear();
		}

		private Vector3D GetSliceThroughPoint()
		{
			Vector3D throughPoint;
			if (_slicing.SliceThroughPointPatient != null)
				throughPoint = _volume.ConvertToVolume(_slicing.SliceThroughPointPatient);
			else
				throughPoint = _volume.CenterPoint;
			return throughPoint;
		}

		private void CreateSliceAndAddToDisplaySet(int sliceIndex, Vector3D throughPoint, IDisplaySet displaySet)
		{
			ImageSop imageSop = CreateSliceImageSop(throughPoint);
			DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);
			SetSeriesLevelDicomAttributes(presImage, sliceIndex, displaySet.Uid, GetSliceSpacing());
			displaySet.PresentationImages.Add(presImage);
			imageSop.Dispose();
		}

		/// <summary>
		/// This should be useful for implementing external spacing controls. Actual slice spacing
		/// is tied to actual volume resolution and can be useful in determing spacing values.
		/// E.g. you can use the actual spacing (or multiples of) to establish defaults. You also 
		/// might only allow spacing values that are multiples of the actual spacing.
		/// </summary>
		public float ActualSliceSpacing
		{
			get { return ActualSliceSpacingVector.Magnitude; }
		}

		#endregion

		#endregion

		#region Implementation

		#region Slice Spacing

		// This uses the slice plane and volume spacing to arrive at the actual spacing
		//	vector along the orthogonal vector
		private Vector3D ActualSliceSpacingVector
		{
			get
			{
				Vector3D normalVec = GetSliceNormalVector();

				// Normal components by spacing components
				Vector3D actualSliceSpacingVector = new Vector3D(normalVec.X * _volume.Spacing.X,
					normalVec.Y * _volume.Spacing.Y, normalVec.Z * _volume.Spacing.Z);

				return actualSliceSpacingVector;
			}
		}

		private float GetDefaultSpacing()
		{
			// By default, adjust magnitude of vector by whole factor based on max volume spacing
			Vector3D spacingVector = ActualSliceSpacingVector;
			if (spacingVector.Magnitude < _volume.MaxSpacing / 2f)
			{
				int spacingFactor = (int)(_volume.MaxSpacing / spacingVector.Magnitude);
				spacingVector *= spacingFactor;
			}
			return spacingVector.Magnitude;
		}

		#endregion

		#region Pixel Data Generation

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		internal byte[] GenerateFrameNormalizedPixelData(Matrix resliceAxes)
		{
#if SLAB
			using (vtkImageData imageData = GenerateVtkSlab(resliceAxes, 10))  // baked 10 voxels for testing
			{
				byte[] pixelData = MipPixelDataFromVtkSlab(imageData);
				imageData.ReleaseData();

				return pixelData;
			}
#else
			using (vtkImageData imageData = GenerateVtkSlice(resliceAxes))
			{
				byte[] pixelData = CreatePixelDataFromVtkSlice(imageData);
				imageData.ReleaseData();

				return pixelData;
			}
#endif
		}

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				VtkHelper.RegisterVtkErrorEvents(reslicer);

				// Obtain a pinned VTK volume for the reslicer. We'll release this when
				//	VTK is done reslicing.
				vtkImageData volumeVtkWrapper = _volume.ObtainPinnedVtkVolume();
				reslicer.SetInput(volumeVtkWrapper);
				reslicer.SetInformationInput(volumeVtkWrapper);

				// Must instruct reslicer to output 2D images
				reslicer.SetOutputDimensionality(2);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(_volume.PadValue);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

				reslicer.SetResliceAxes(VtkHelper.ConvertToVtkMatrix(resliceAxes));

				// Clamp the output based on the slice extent
				int sliceExtentX = GetSliceExtentX();
				int sliceExtentY = GetSliceExtentY();
				reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, 0);

				// Set the output origin to reflect the slice through point. The slice extent is
				//	centered on the slice through point.
				// VTK output origin is derived from the center image being 0,0
				float originX = -sliceExtentX * EffectiveSpacing / 2;
				float originY = -sliceExtentY * EffectiveSpacing / 2;
				reslicer.SetOutputOrigin(originX, originY, 0);

				switch (_slicing.InterpolationMode)
				{
					case InterpolationModes.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case InterpolationModes.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case InterpolationModes.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				using (vtkExecutive exec = reslicer.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);

					exec.Update();

					_volume.ReleasePinnedVtkVolume();

					return reslicer.GetOutput();
				}
			}
		}

		private static byte[] CreatePixelDataFromVtkSlice(vtkImageData sliceImageData)
		{
			int[] sliceDimensions = sliceImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0] * sliceDimensions[1];
			IntPtr sliceDataPtr = sliceImageData.GetScalarPointer();
			byte[] pixelData = MemoryManager.Allocate<byte>(sliceDataSize * sizeof(short));

			Marshal.Copy(sliceDataPtr, pixelData, 0, sliceDataSize * sizeof(short));
			return pixelData;
		}

		// Extract slab in specified orientation, if slabThickness is 1, this is identical
		//	to GenerateVtkSlice above, so they should be collapsed at some point.
		// TODO: Tie into Dicom for slice, will need to adjust thickness at least
		private vtkImageData GenerateVtkSlab(Matrix resliceAxes, int slabThicknessInVoxels)
		{
			// Thickness should be at least 1
			if (slabThicknessInVoxels < 1)
				slabThicknessInVoxels = 1;

			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				VtkHelper.RegisterVtkErrorEvents(reslicer);

				// Obtain a pinned VTK volume for the reslicer. We'll release this when
				//	VTK is done reslicing.
				vtkImageData volumeVtkWrapper = _volume.ObtainPinnedVtkVolume();
				reslicer.SetInput(volumeVtkWrapper);
				reslicer.SetInformationInput(volumeVtkWrapper);

				if (slabThicknessInVoxels > 1)
					reslicer.SetOutputDimensionality(3);
				else
					reslicer.SetOutputDimensionality(3);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(_volume.PadValue);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

				reslicer.SetResliceAxes(VtkHelper.ConvertToVtkMatrix(resliceAxes));

				// Clamp the output based on the slice extent
				int sliceExtentX = GetSliceExtentX();
				int sliceExtentY = GetSliceExtentY();
				reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, slabThicknessInVoxels-1);

				// Set the output origin to reflect the slice through point. The slice extent is
				//	centered on the slice through point.
				// VTK output origin is derived from the center image being 0,0
				float originX = -sliceExtentX * EffectiveSpacing / 2;
				float originY = -sliceExtentY * EffectiveSpacing / 2;
				reslicer.SetOutputOrigin(originX, originY, 0);

				switch (_slicing.InterpolationMode)
				{
					case InterpolationModes.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case InterpolationModes.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case InterpolationModes.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				using (vtkExecutive exec = reslicer.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);

					exec.Update();

					_volume.ReleasePinnedVtkVolume();

					return reslicer.GetOutput();
				}
			}
		}

		private static byte[] MipPixelDataFromVtkSlab(vtkImageData slabImageData)
		{
#if true // Do our own MIP, albeit slowly
			int[] sliceDimensions = slabImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0] * sliceDimensions[1];
			IntPtr slabDataPtr = slabImageData.GetScalarPointer();

			byte[] pixelData = MemoryManager.Allocate<byte>(sliceDataSize * sizeof(short));

			// Init with first slice
			Marshal.Copy(slabDataPtr, pixelData, 0, sliceDataSize * sizeof(short));

			// Walk through other slices, finding maximum
			unsafe
			{
				short* psSlab = (short*) slabDataPtr;

				fixed (byte* pbFrame = pixelData)
				{
					short* psFrame = (short*)pbFrame;
					for (int sliceIndex = 1; sliceIndex < sliceDimensions[2]; sliceIndex++)
					{
						for (int i = 0; i < sliceDataSize-1; ++i)
						{
							int slabIndex = sliceIndex * sliceDataSize + i;
							if (psSlab[slabIndex] > psFrame[i])
								psFrame[i] = psSlab[slabIndex];
						}
					}
				}
			}

			return pixelData;

#else // Ideally we'd use VTK to do the MIP (MinIP, Average...)
				vtkVolumeRayCastMIPFunction mip = new vtkVolumeRayCastMIPFunction();
				vtkVolumeRayCastMapper mapper = new vtkVolumeRayCastMapper();

				mapper.SetVolumeRayCastFunction(mip);
				mapper.SetInput(slabImageData);

				//TODO: Need to figure out how to use mapper to output vtkImageData

				vtkImageAlgorithm algo = new vtkImageAlgorithm();
				algo.SetInput(mapper.GetOutputDataObject(0));
				
				using (vtkExecutive exec = mapper.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);
					exec.Update();

					// Note: These report no output port, must have to do something else to get mapper to give us data
					//return exec.GetOutputData(0);
					return mapper.GetOutputDataObject(0);
				}
#endif
		}


		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentX()
		{
			if (_slicing.SliceExtentXMillimeters != 0f)
				return (int)(_slicing.SliceExtentXMillimeters / EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentY()
		{
			if (_slicing.SliceExtentYMillimeters != 0f)
				return (int)(_slicing.SliceExtentYMillimeters / EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		private int MaxOutputImageDimension
		{
			get
			{
				// This doesn't give us enough extra room, so I decided to use the diagonal along long and short dimensions
				//return (int)(LongAxisMagnitude / EffectiveSpacing + 0.5f);
				float longOutputDimension = _volume.LongAxisMagnitude / EffectiveSpacing;
				float shortOutputDimenstion = _volume.ShortAxisMagnitude / EffectiveSpacing;
				return (int)Math.Sqrt(longOutputDimension * longOutputDimension + shortOutputDimenstion * shortOutputDimenstion);
			}
		}

		/// <summary>
		/// The effective spacing defines output spacing for slices generated by the VolumeSlicer.
		/// </summary>
		private float EffectiveSpacing
		{
			// Because we supply the real spacing to the VTK reslicer, the slices are interpolated
			//	as if the volume were isotropic. This results in an effective spacing that is the
			//	minimum spacing for the volume.
			get { return _volume.MinSpacing; }
		}

		#endregion

		#region Slice Dicom Generation

		private static void SetSeriesLevelDicomAttributes(IImageSopProvider presImage, int sliceIndex,
														  string seriesInstanceUid, float increment)
		{
			ISopDataSource dicomData = presImage.ImageSop.DataSource;
			dicomData[DicomTags.SeriesInstanceUid].SetString(0, seriesInstanceUid);
			dicomData[DicomTags.InstanceNumber].SetString(0, Convert.ToString(sliceIndex + 1));

			// Note: These are required by the spatial locator
			float thicknessAndSpacing = Math.Abs(increment);
			dicomData[DicomTags.SliceThickness].SetFloat32(0, thicknessAndSpacing);
			dicomData[DicomTags.SpacingBetweenSlices].SetFloat32(0, thicknessAndSpacing);
		}

		// VTK treats the reslice point as the center of the output image. Given the plane orientation
		//	and size of the output image, we can derive the top left of the output image in patient space
		private Vector3D GetTopLeftOfSlicePatient(int columns, int rows)
		{
			// This is the center of the output image
			PointF centerImageCoord = new PointF(columns / 2f, rows / 2f);

			// These offsets define the x and y vector magnitudes to arrive at our point
			float offsetX = centerImageCoord.X * EffectiveSpacing;
			float offsetY = centerImageCoord.Y * EffectiveSpacing;

			// To determine top left of slice in volume, subtract offset vectors along x and y
			//
			// Our reslice place x and y vectors
			Vector3D xVec = GetSliceXVector();
			Vector3D yVec = GetSliceYVector();
			// Offset along x and y from reslicePoint
			Vector3D topLeftOfSliceVolume = ReslicePoint - (offsetX * xVec + offsetY * yVec);

			// Convert volume point to patient space
			return _volume.ConvertToPatient(topLeftOfSliceVolume);
		}
		
		#endregion

		#region Reslice Matrix helpers

		private Vector3D GetSliceXVector()
		{
			Matrix _resliceAxes = _slicing.ResliceAxes;
			return new Vector3D(_resliceAxes[0, 0], _resliceAxes[0, 1], _resliceAxes[0, 2]);
		}

		private Vector3D GetSliceYVector()
		{
			Matrix _resliceAxes = _slicing.ResliceAxes;
			return new Vector3D(_resliceAxes[1, 0], _resliceAxes[1, 1], _resliceAxes[1, 2]);
		}
		
		private Vector3D GetSliceNormalVector()
		{
			Matrix _resliceAxes = _slicing.ResliceAxes;
			return new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
		}

		private Vector3D ReslicePoint
		{
			get
			{
				Matrix _resliceAxes = _slicing.ResliceAxes;
				return new Vector3D(_resliceAxes[3, 0],
				                    _resliceAxes[3, 1],
				                    _resliceAxes[3, 2]);
			}
			set
			{
				Matrix _resliceAxes = _slicing.ResliceAxes;
				_resliceAxes[3, 0] = value.X;
				_resliceAxes[3, 1] = value.Y;
				_resliceAxes[3, 2] = value.Z;
			}
		}

		#endregion

		#endregion
	}
}
