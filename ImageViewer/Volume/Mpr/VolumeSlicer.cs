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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Factory for slices of a <see cref="Volume"/>.
	/// </summary>
	public class VolumeSlicer : IDisposable
	{
		private readonly IVolumeReference _volume;
		private readonly IVolumeSlicerParams _slicerParams;
		private readonly string _seriesInstanceUid;

		private float _sliceSpacing;

		public VolumeSlicer(Volume vol, IVolumeSlicerParams slicerParams, string seriesInstanceUid)
		{
			_volume = vol.CreateTransientReference();
			_slicerParams = slicerParams;
			_seriesInstanceUid = seriesInstanceUid;
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
		}

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				_volume.Dispose();
			}
		}

		#region Slicing

		private float GetSliceSpacing()
		{
			if (_sliceSpacing == 0f)
			{
				_sliceSpacing = _slicerParams.SliceSpacing;
				if (_sliceSpacing == 0f)
					_sliceSpacing = GetDefaultSpacing();
			}
			return _sliceSpacing;
		}

		public IEnumerable<ISliceSopDataSource> CreateSlices()
		{
			Vector3D initialThroughPoint = GetSliceThroughPoint();

			// Determine spacing vector along which we will slice
			Vector3D spacingVector = GetSliceSpacing()*GetSliceNormalVector();
			// I chose to use the volume diagonal magnitude to define the maximum number of slices as
			//	it seemed like a reasonable upper limit to the number of slices in a display set.
			//	Note that in the worst case scenario, this would only generate half the number of possible
			//	slices. Consider the slice plane with normal along the volume diagonal, and through point
			//	on a corner of the volume. This would require the full maxSlices defined here, but because
			//	we start half way in one direction we would only get half of what is possible.
			int maxSlices = (int) (_volume.Volume.DiagonalMagnitude/GetSliceSpacing() + 0.5f);

			// Start slicing half way in one direction. Chose to start positive and step
			//	negative as it creates a DisplaySet that is sorted such that the MPR sort
			//	order is consistent with the default 2D sort order. Perhaps in the future
			//	it could be based off of the order of the source DisplaySet.
			Vector3D startPoint = initialThroughPoint + (maxSlices/2)*spacingVector;

			// Walk along trying to create maxSlices, if a point is outside the volume we'll
			//	skip it, ensuring that all slices are from through points that are in the volume.
			int pointIndex = 0, sliceIndex = 0;
			while (sliceIndex < maxSlices)
			{
				Vector3D throughPoint = startPoint - (pointIndex*spacingVector);
				pointIndex++;

				// Don't generate slice if point is not in volume
				if (_volume.Volume.IsPointInVolume(throughPoint) == false)
					// If we've already found some slices, or we've reached maxSlices and haven't
					//	found any, it's time to call it a day
					if (sliceIndex > 0 || pointIndex > maxSlices)
						break;
					else
						// Check next point location
						continue;

				yield return CreateSlice(sliceIndex, throughPoint);
				sliceIndex++;
			}

			// If the through point is outside volume we want to ensure that at least one slice is generated
			if (sliceIndex == 0)
				yield return CreateSlice(sliceIndex, initialThroughPoint);
		}

		private VolumeSliceSopDataSource CreateSlice(int sliceIndex, Vector3D throughPoint)
		{
			float thicknessAndSpacing = Math.Abs(GetSliceSpacing());
			VolumeSliceSopDataSource slice = new VolumeSliceSopDataSource(_volume.Volume, _slicerParams, throughPoint);
			slice[DicomTags.SliceThickness].SetFloat32(0, thicknessAndSpacing);
			slice[DicomTags.SpacingBetweenSlices].SetFloat32(0, thicknessAndSpacing);
			slice[DicomTags.SeriesInstanceUid].SetString(0, _seriesInstanceUid);
			slice[DicomTags.InstanceNumber].SetInt32(0, sliceIndex + 1);
			return slice;
		}

		private Vector3D GetSliceThroughPoint()
		{
			Vector3D throughPoint;
			if (_slicerParams.SliceThroughPointPatient != null)
				throughPoint = _volume.Volume.ConvertToVolume(_slicerParams.SliceThroughPointPatient);
			else
				throughPoint = _volume.Volume.CenterPoint;
			return throughPoint;
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
				Vector3D actualSliceSpacingVector = new Vector3D(normalVec.X*_volume.Volume.VoxelSpacing.X,
				                                                 normalVec.Y*_volume.Volume.VoxelSpacing.Y, normalVec.Z*_volume.Volume.VoxelSpacing.Z);

				return actualSliceSpacingVector;
			}
		}

		private float GetDefaultSpacing()
		{
			// By default, adjust magnitude of vector by whole factor based on max volume spacing
			Vector3D spacingVector = ActualSliceSpacingVector;
			if (spacingVector.Magnitude < _volume.Volume.MaxSpacing/2f)
			{
				int spacingFactor = (int) (_volume.Volume.MaxSpacing/spacingVector.Magnitude);
				spacingVector *= spacingFactor;
			}
			return spacingVector.Magnitude;
		}

		#endregion

		#region Pixel Data Generation

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		public byte[] CreateSliceNormalizedPixelData(Vector3D throughPoint)
		{
			Matrix resliceAxes = new Matrix(_slicerParams.SlicingPlaneRotation);
			resliceAxes[3, 0] = throughPoint.X;
			resliceAxes[3, 1] = throughPoint.Y;
			resliceAxes[3, 2] = throughPoint.Z;

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
				vtkImageData volumeVtkWrapper = _volume.Volume.ObtainPinnedVtkVolume();
				reslicer.SetInput(volumeVtkWrapper);
				reslicer.SetInformationInput(volumeVtkWrapper);

				// Must instruct reslicer to output 2D images
				reslicer.SetOutputDimensionality(2);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(_volume.Volume.PadValue);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(_volume.Volume.VoxelSpacing.X, _volume.Volume.VoxelSpacing.Y, _volume.Volume.VoxelSpacing.Z);

				reslicer.SetResliceAxes(VtkHelper.ConvertToVtkMatrix(resliceAxes));

				// Clamp the output based on the slice extent
				int sliceExtentX = GetSliceExtentX();
				int sliceExtentY = GetSliceExtentY();
				reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, 0);

				// Set the output origin to reflect the slice through point. The slice extent is
				//	centered on the slice through point.
				// VTK output origin is derived from the center image being 0,0
				float originX = -sliceExtentX*EffectiveSpacing/2;
				float originY = -sliceExtentY*EffectiveSpacing/2;
				reslicer.SetOutputOrigin(originX, originY, 0);

				switch (_slicerParams.InterpolationMode)
				{
					case VolumeSlicerInterpolationMode.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case VolumeSlicerInterpolationMode.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case VolumeSlicerInterpolationMode.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				using (vtkExecutive exec = reslicer.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);

					exec.Update();

					_volume.Volume.ReleasePinnedVtkVolume();

					return reslicer.GetOutput();
				}
			}
		}

		private static byte[] CreatePixelDataFromVtkSlice(vtkImageData sliceImageData)
		{
			int[] sliceDimensions = sliceImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0]*sliceDimensions[1];
			IntPtr sliceDataPtr = sliceImageData.GetScalarPointer();
			byte[] pixelData = MemoryManager.Allocate<byte>(sliceDataSize*sizeof (short));

			Marshal.Copy(sliceDataPtr, pixelData, 0, sliceDataSize*sizeof (short));
			return pixelData;
		}

		#region Slabbing Code

#if SLAB

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
#endif

		#endregion

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentX()
		{
			if (_slicerParams.SliceExtentXMillimeters != 0f)
				return (int) (_slicerParams.SliceExtentXMillimeters/EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentY()
		{
			if (_slicerParams.SliceExtentYMillimeters != 0f)
				return (int) (_slicerParams.SliceExtentYMillimeters/EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		private int MaxOutputImageDimension
		{
			get
			{
				// This doesn't give us enough extra room, so I decided to use the diagonal along long and short dimensions
				//return (int)(LongAxisMagnitude / EffectiveSpacing + 0.5f);
				float longOutputDimension = _volume.Volume.LongAxisMagnitude/EffectiveSpacing;
				float shortOutputDimenstion = _volume.Volume.ShortAxisMagnitude/EffectiveSpacing;
				return (int) Math.Sqrt(longOutputDimension*longOutputDimension + shortOutputDimenstion*shortOutputDimenstion);
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
			get { return _volume.Volume.MinSpacing; }
		}

		#endregion

		#region Reslice Matrix helpers

		private Vector3D GetSliceNormalVector()
		{
			Matrix _resliceAxes = _slicerParams.SlicingPlaneRotation;
			return new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
		}

		#endregion

		#endregion
	}
}