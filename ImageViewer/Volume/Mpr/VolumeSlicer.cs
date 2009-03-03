#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Volume utility class aids in extracting 2D slices from a Volume
	/// </summary>
	public class VolumeSlicer
	{
		#region Private fields

		private readonly Volume _volume;
		private Matrix _resliceAxes;

		private Vector3D _sliceThroughPointPatient;
		private float _sliceExtentMm;
		private float _sliceSpacing;

		//ggerade ToRef: Switch from degrees to radians, allows finer control
		private int _rotateAboutX;
		private int _rotateAboutY;
		private int _rotateAboutZ;

		private InterpolationModes _interpolationMode = InterpolationModes.Linear;

		#endregion

		#region Public methods

		public VolumeSlicer(Volume vol)
		{
			_volume = vol;
		}

		#region Slicer Settings

		public void SetSlicePlanePatient(Vector3D sourceOrientationColumnPatient, Vector3D sourceOrientationRowPatient,
		                                 Vector3D startPointPatient, Vector3D endPointPatient)
		{
			Vector3D sourceOrientationNormalPatient = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);
			Vector3D normalLinePatient = (endPointPatient - startPointPatient).Normalize();
			Vector3D normalPerpLinePatient = sourceOrientationNormalPatient.Cross(normalLinePatient);

			Vector3D slicePlanePatientX = normalLinePatient;
			Vector3D slicePlanePatientY = sourceOrientationNormalPatient;
			Vector3D slicePlanePatientZ = normalPerpLinePatient;

			Matrix slicePlanePatientOrientation = new Matrix
				(4, 4, new float[4,4]
				       	{
				       		{slicePlanePatientX.X, slicePlanePatientX.Y, slicePlanePatientX.Z, 0},
				       		{slicePlanePatientY.X, slicePlanePatientY.Y, slicePlanePatientY.Z, 0},
				       		{slicePlanePatientZ.X, slicePlanePatientZ.Y, slicePlanePatientZ.Z, 0},
				       		{0, 0, 0, 1}
				       	});

			_resliceAxes = _volume.RotateToVolumeOrientation(slicePlanePatientOrientation);
			Vector3D lineMiddlePointPatient = new Vector3D(
			    (startPointPatient.X + endPointPatient.X) / 2,
			    (startPointPatient.Y + endPointPatient.Y) / 2,
			    (startPointPatient.Z + endPointPatient.Z) / 2);

			this.SliceThroughPointPatient = lineMiddlePointPatient;

			this.SliceExtentMillimeters = (endPointPatient - startPointPatient).Magnitude;
		}

		public void SetSlicePlaneIdentity()
		{
			_resliceAxes = CreateResliceAxesIdentity();
		}

		public void SetSlicePlaneOrthoX()
		{
			_resliceAxes = CreateResliceAxesOrthoX();
		}

		public void SetSlicePlaneOrthoY()
		{
			_resliceAxes = CreateResliceAxesOrthoY();
		}

		public void SetSlicePlaneOblique(int rotateX, int rotateY, int rotateZ)
		{
			RotateAboutX = rotateX;
			RotateAboutY = rotateY;
			RotateAboutZ = rotateZ;
			_resliceAxes = CreateResliceAxesOblique(rotateX, rotateY, rotateZ);
		}

		public Vector3D SliceThroughPointPatient
		{
			set { _sliceThroughPointPatient = new Vector3D(value); }
		}

		public float SliceExtentMillimeters
		{
			set { _sliceExtentMm = value; }
		}

		public float SliceSpacing
		{
			get
			{
				if (_sliceSpacing == 0f)
					_sliceSpacing = GetDefaultSpacing();
				return _sliceSpacing;
			}
			set { _sliceSpacing = value; }	
		}

		public float ActualSliceSpacing
		{
			get { return ActualSliceSpacingVector.Magnitude; }
		}

		public enum InterpolationModes
		{
			NearestNeighbor,
			Linear,
			Cubic
		}

		public InterpolationModes InterpolationMode
		{
			get { return _interpolationMode; }
			set { _interpolationMode = value; }
		}

		public int RotateAboutX
		{
			get { return _rotateAboutX; }
			private set { _rotateAboutX = value; }
		}

		public int RotateAboutY
		{
			get { return _rotateAboutY; }
			private set { _rotateAboutY = value; }
		}

		public int RotateAboutZ
		{
			get { return _rotateAboutZ; }
			private set { _rotateAboutZ = value; }
		}

		#endregion

		public ImageSop CreateSliceImageSop(Vector3D point)
		{
			// You must first call one of the SetSlicePlane methods before calling this.
			Debug.Assert(_resliceAxes != null);

			SetReslicePoint(point);

			DicomFile sliceDicom = CreateSliceDicom();

			return new ImageSop(new VolumeSliceSopDataSource(sliceDicom, this, _resliceAxes));
		}

		#region Create DisplaySet utilities

		//TODO: resolve naming before plane is set.
		public DisplaySet CreateDisplaySet(string displaySetName)
		{
			string name = String.Format("MPR ({0})", displaySetName);
			DisplaySet displaySet = new DisplaySet(name, Guid.NewGuid().ToString(), name);
			PopulateDisplaySet(displaySet, false);
			return displaySet;
		}

		internal void PopulateDisplaySet(IDisplaySet displaySet, bool singleSlice)
		{
			// Slice through this point
			Vector3D throughPoint;
			if (_sliceThroughPointPatient != null)
				throughPoint = _volume.ConvertToVolume(_sliceThroughPointPatient);
			else
				throughPoint = _volume.CenterPoint;

			Vector3D spacingVector = this.SliceSpacing * GetSliceNormalVector();

			//ggerade ToDo: Determine the length of the vector that passes through the volume. Or
			//  stop when throughPoint is outside volume, which seems easier.
			// For now just use the longest axis by the spacingVector
			int numSlices = 1;
			//ggerade ToRef: find a better way to encapsulate generating one vs the whole set
			if (!singleSlice)
				numSlices = (int)(_volume.LongAxisMagnitude / spacingVector.Magnitude + 0.5f);

			Vector3D startPoint = throughPoint + (numSlices / 2) * spacingVector;

			foreach (IPresentationImage image in displaySet.PresentationImages)
				image.Dispose();

			displaySet.PresentationImages.Clear();

			int sliceIndex;
			for (sliceIndex = 0; sliceIndex < numSlices; sliceIndex++)
			{
				Vector3D point = startPoint - (sliceIndex * spacingVector);
				ImageSop imageSop = CreateSliceImageSop(point);
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);
				SetSeriesLevelDicomAttributes(presImage, sliceIndex, displaySet.Uid, spacingVector.Magnitude);
				displaySet.PresentationImages.Add(presImage);
				imageSop.Dispose();
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

		private Vector3D GetSliceNormalVector()
		{
			return new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
		}

		private static void SetSeriesLevelDicomAttributes(IImageSopProvider presImage, int sliceIndex,
		                                                  string seriesInstanceUid, float increment)
		{
			IDicomMessageSopDataSource dicomData = (IDicomMessageSopDataSource) presImage.ImageSop.DataSource;
			dicomData.SourceMessage.DataSet[DicomTags.SeriesInstanceUid].SetString(0, seriesInstanceUid);
			dicomData.SourceMessage.DataSet[DicomTags.InstanceNumber].SetString(0, Convert.ToString(sliceIndex + 1));

			// Note: These are required by the spatial locator
			float thicknessAndSpacing = Math.Abs(increment);
			dicomData.SourceMessage.DataSet[DicomTags.SliceThickness].SetFloat32(0, thicknessAndSpacing);
			dicomData.SourceMessage.DataSet[DicomTags.SpacingBetweenSlices].SetFloat32(0, thicknessAndSpacing);
		}

		#endregion

		#endregion

		#region Implementation

		#region Pixel Data generation

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		internal byte[] GenerateFrameNormalizedPixelData(Matrix resliceAxes)
		{
			using (vtkImageData vtkSlice = GenerateVtkSlice(resliceAxes))
			{
				byte[] pixelData = CreatePixelDataFromVtkSlice(vtkSlice);
				vtkSlice.ReleaseData();

				return pixelData;
			}
		}

		private static byte[] CreatePixelDataFromVtkSlice(vtkImageData sliceImageData)
		{
			int[] sliceDimensions = sliceImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0] * sliceDimensions[1] * sliceDimensions[2];
			IntPtr sliceDataPtr = sliceImageData.GetScalarPointer();
			byte[] pixelData = new byte[sliceDataSize * sizeof (short)];

			CopySliceToFrame(pixelData, sliceDataPtr, sliceDataSize);
			return pixelData;
		}

		private static unsafe void CopySliceToFrame(byte[] pixelData, IntPtr sliceDataPtr, int sliceDataSize)
		{
			short* psSlice = (short*) sliceDataPtr;

			// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
			fixed (byte* pbFrame = pixelData)
			{
				//ggerade ToOpt: Find an optimized way to do this memcpy (investigate IL cpblk opcode)
				// Better yet, find a way not to copy...
				short* psFrame = (short*) pbFrame;
				for (int i = 0; i < sliceDataSize; ++i)
				{
					psFrame[i] = psSlice[i];
				}
			}
		}

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				// The volume has everything it needs to init the vtkImageData wrapper that VTK requires
				reslicer.SetInput(_volume._VtkVolume);
				reslicer.SetInformationInput(_volume._VtkVolume);

				// Must instruct reslicer to output 2D images
				//ggerade ToRes: Can we get 3D output for slabs? How to specify thickness? Not sure it's possible, need to research.
				reslicer.SetOutputDimensionality(2);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(_volume.PadValue);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

				reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

				// Clamp the output based on the slice extent
				//ggerade ToRes: Should extent always be X here? need to think about other viewports.
				int sliceExtentX = GetSliceExtentX();
				reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, _volume.LargestOutputImageDimension - 1, 0, 0);

				// Set the output origin to reflect the slice through point. The slice extent is
				//	centered on the slice through point.
				// VTK output origin is derived from the center image being 0,0
				float originX = -sliceExtentX * _volume.EffectiveSpacing / 2;
				float originY = -_volume.LargestOutputImageDimension * _volume.EffectiveSpacing / 2;
				reslicer.SetOutputOrigin(originX, originY, 0);

				switch (_interpolationMode)
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
					//ggerade ToRes: Are these VTK observers useful?
					//exec.AddObserver(123, VtkReslicerExecutiveCallback);
					exec.Update();

					return reslicer.GetOutput();
				}
			}
		}

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentX()
		{
			if (_sliceExtentMm != 0f)
				return (int)(_sliceExtentMm / _volume.EffectiveSpacing + 0.5f);
			else
				return _volume.LargestOutputImageDimension;
		}

		// Used by VTK ResliceImage observer
		//private static void VtkReslicerExecutiveCallback(vtkObject vtkObj, uint eid, object obj, IntPtr nativeSomethingOrOther)
		//{
		//    Debug.WriteLine(eid);
		//}

		#endregion

		#region Dicom Slice Generation

		private DicomFile CreateSliceDicom()
		{
			// Start with the volume's model Dicom attributes
			DicomMessageBase modelDicom = _volume._ModelDicom;
			DicomFile sliceDicom = new DicomFile("", modelDicom.MetaInfo.Copy(), modelDicom.DataSet.Copy());
			DicomAttributeCollection sliceDataSet = sliceDicom.DataSet;

			// ensure each sop has unique Uid
			sliceDataSet[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);

			// Update rows and columns to reflect actual output size
			int columns, rows;
			columns = GetSliceExtentX();
			rows = _volume.LargestOutputImageDimension;
			sliceDataSet[DicomTags.Columns].SetUInt16(0, (ushort)columns);
			sliceDataSet[DicomTags.Rows].SetUInt16(0, (ushort) rows);

			// Update Image Orientation (patient)
			//
			Matrix resliceAxesPatientOrientation = _volume.RotateToPatientOrientation(_resliceAxes);

			ImageOrientationPatient imageOrientation =
				new ImageOrientationPatient(resliceAxesPatientOrientation[0, 0],
				                            resliceAxesPatientOrientation[0, 1],
				                            resliceAxesPatientOrientation[0, 2],
				                            resliceAxesPatientOrientation[1, 0],
				                            resliceAxesPatientOrientation[1, 1],
				                            resliceAxesPatientOrientation[1, 2]);

			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(0, (float) imageOrientation.RowX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(1, (float) imageOrientation.RowY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(2, (float) imageOrientation.RowZ);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(3, (float) imageOrientation.ColumnX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(4, (float) imageOrientation.ColumnY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(5, (float) imageOrientation.ColumnZ);

			// Update Image Position (patient)
			//
			Vector3D topLeftOfSlicePatient = GetTopLeftOfSlicePatient(columns, rows);

			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			return sliceDicom;
		}

		private Vector3D GetTopLeftOfSlicePatient(int columns, int rows)
		{
			Vector3D reslicePoint = GetReslicePoint();

			Vector3D xVec = new Vector3D(_resliceAxes[0, 0], _resliceAxes[0, 1], _resliceAxes[0, 2]);
			Vector3D yVec = new Vector3D(_resliceAxes[1, 0], _resliceAxes[1, 1], _resliceAxes[1, 2]);
			Vector3D zVec = new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);

			PointF centerPlaneCoord = new PointF(columns / 2f, rows / 2f);

			// These offsets define the x and y vector magnitudes to arrive at our point
			float offsetX = centerPlaneCoord.X * _volume.EffectiveSpacing;
			float offsetY = centerPlaneCoord.Y * _volume.EffectiveSpacing;

			Vector3D volumePoint = reslicePoint - (offsetX * xVec + offsetY * yVec);

			//ggerade ToRes: This adjusts the coordinate such that it is in the middle of the slice spacing,
			//	I need to figure out how to get the direction (sign) correctly for all cases.
			//Vector3D zVec = new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
			//volumePoint += zVec * (this.SliceSpacing / 2);

			// Convert 3D volume point to patient point
			return _volume.ConvertToPatient(volumePoint);
		}
		
		#endregion

		#region Reslice Matrix helpers

		// Note: vtkMatrix4x4 and Math.Matrix are transposed! This is due to the
		//	fact that vtkMatrix4x4 uses an x,y interface where Matrix uses a row,col interface.
		private static vtkMatrix4x4 MatrixToVtkMatrix(Matrix matrix)
		{
			vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					vtkMatrix.SetElement(column, row, matrix[row, column]);

			return vtkMatrix;
		}

		private Vector3D GetReslicePoint()
		{
			return new Vector3D(_resliceAxes[3, 0],
			                    _resliceAxes[3, 1],
			                    _resliceAxes[3, 2]);
		}

		private void SetReslicePoint(Vector3D point)
		{
			_resliceAxes[3, 0] = point.X;
			_resliceAxes[3, 1] = point.Y;
			_resliceAxes[3, 2] = point.Z;
		}

		private static Matrix CreateResliceAxesOrthoY()
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{0, 1, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{1, 0, 0, 0},
			                        		{0, 0, 0, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesOrthoX()
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{0, 1, 0, 0},
			                        		{0, 0, 0, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesIdentity()
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 1, 0, 0},
			                        		{0, 0, 1, 0},
			                        		{0, 0, 0, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesOblique(int rotateX, int rotateY, int rotateZ)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);
			Matrix aboutZ = CalcRotateMatrixAboutZ(rotateZ);

			return aboutX * aboutY * aboutZ;
		}

		private static readonly Matrix _identityMatrix = new Matrix(4, 4, new float[4,4]
		                                                                  	{
		                                                                  		{1, 0, 0, 0},
		                                                                  		{0, 1, 0, 0},
		                                                                  		{0, 0, 1, 0},
		                                                                  		{0, 0, 0, 1}
		                                                                  	});

		private static Matrix CalcRotateMatrixAboutX(int rotateXdegrees)
		{
			Matrix aboutX;

			if (rotateXdegrees != 0)
			{
				float sinX = (float) Math.Sin(rotateXdegrees * Math.PI / 180);
				float cosX = (float) Math.Cos(rotateXdegrees * Math.PI / 180);
				aboutX = new Matrix(4, 4, new float[4,4]
				                          	{
				                          		{1, 0, 0, 0},
				                          		{0, cosX, -sinX, 0},
				                          		{0, sinX, cosX, 0},
				                          		{0, 0, 0, 1}
				                          	});
			}
			else
				aboutX = _identityMatrix;

			return aboutX;
		}

		private static Matrix CalcRotateMatrixAboutY(int rotateYdegrees)
		{
			Matrix aboutY;

			if (rotateYdegrees != 0)
			{
				float sinY = (float) Math.Sin(rotateYdegrees * Math.PI / 180);
				float cosY = (float) Math.Cos(rotateYdegrees * Math.PI / 180);
				aboutY = new Matrix(4, 4, new float[4,4]
				                          	{
				                          		{cosY, 0, sinY, 0},
				                          		{0, 1, 0, 0},
				                          		{-sinY, 0, cosY, 0},
				                          		{0, 0, 0, 1}
				                          	});
			}
			else
				aboutY = _identityMatrix;

			return aboutY;
		}

		private static Matrix CalcRotateMatrixAboutZ(int rotateZdegrees)
		{
			Matrix aboutZ;

			if (rotateZdegrees != 0)
			{
				float sinZ = (float) Math.Sin(rotateZdegrees * Math.PI / 180);
				float cosZ = (float) Math.Cos(rotateZdegrees * Math.PI / 180);
				aboutZ = new Matrix(4, 4, new float[4,4]
				                          	{
				                          		{cosZ, -sinZ, 0, 0},
				                          		{sinZ, cosZ, 0, 0},
				                          		{0, 0, 1, 0},
				                          		{0, 0, 0, 1}
				                          	});
			}
			else
				aboutZ = _identityMatrix;

			return aboutZ;
		}

		#endregion

		#endregion
	}
}