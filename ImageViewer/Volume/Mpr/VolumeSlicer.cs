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

// This switch causes the the Sag/Coronal/Axial DisplaySet's pixel data to get pre-generated when the
//	DisplaySets are created, it's basically to test the public CreateSlice calls for now
//#define PREGEN_PIXELDATA

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
	internal class VolumeSlicer
	{
		#region Private fields

		private readonly Volume _volume;
		private Matrix _resliceAxes;
		private InterpolationModes _interpolationMode = InterpolationModes.Linear;

		//ggerade ToRef: Switch from degrees to radians, allows finer control
		private int _rotateX;
		private int _rotateY;
		private int _rotateZ;

		#endregion

		#region Public methods

		public VolumeSlicer(Volume vol)
		{
			_volume = vol;
		}

		#region Slicer Settings

		public void SetSlicePlaneSagittal()
		{
			_resliceAxes = CreateResliceAxesSagittal();
		}

		public void SetSlicePlaneCoronal()
		{
			_resliceAxes = CreateResliceAxesCoronal();
		}

		public void SetSlicePlaneAxial()
		{
			_resliceAxes = CreateResliceAxesAxial();
		}

		public void SetSlicePlaneOblique(int rotateX, int rotateY, int rotateZ)
		{
			RotateX = rotateX;
			RotateY = rotateY;
			RotateZ = rotateZ;
			_resliceAxes = CreateResliceAxesOblique(rotateX, rotateY, rotateZ);
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

		public int RotateX
		{
			get { return _rotateX; }
			private set { _rotateX = value; }
		}

		public int RotateY
		{
			get { return _rotateY; }
			private set { _rotateY = value; }
		}

		public int RotateZ
		{
			get { return _rotateZ; }
			private set { _rotateZ = value; }
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

		internal DisplaySet CreateDisplaySet(string displaySetName)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("MPR ({0})", displaySetName),
			                                       String.Format("{0}.{1}", displaySetName, Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D centerPoint = _volume.CenterPoint;

			Vector3D zVec = new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
			Vector3D spacingVector = 5 * zVec;
			float spacing = spacingVector.Magnitude;

			//ggerade ToDo: Determine the length of the vector that passes through the volume
			// Here are a few cheap ways to determine the number of slices
			//
			// This would ensure that we would include every slice along the longest diagonal in the volume,
			//	a bit overkill I thought...
			//int numSlices = (int)(vol.DiagonalMagnitude / spacingVector.Magnitude);
			// This uses the diagonal of the largest ortho plane, was still a bit too much for my liking
			//int numSlices = (int)(vol.LargestOrthogonalPlaneDiagonal / spacingVector.Magnitude);
			// So I settled on just the longest axis, creates enough slices without going out of bounds too often
			int numSlices = (int) (_volume.LongAxisMagnitude / spacingVector.Magnitude);

			Vector3D startPoint = centerPoint - (numSlices / 2) * spacingVector;

			int sliceIndex;
			for (sliceIndex = 0; sliceIndex < numSlices; sliceIndex++)
			{
				Vector3D point = startPoint + sliceIndex * spacingVector;
				ImageSop imageSop = CreateSliceImageSop(point);
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);
				SetSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);
				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal DisplaySet CreateOrthoDisplaySet(string displaySetName)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("MPR ({0})", displaySetName),
												   String.Format("{0}.{1}", displaySetName, Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D centerPoint = _volume.CenterPoint;

			Vector3D zVec = new Vector3D(_resliceAxes[2, 0], _resliceAxes[2, 1], _resliceAxes[2, 2]);
			Vector3D spacingVector = 5 * zVec;
			float spacing = spacingVector.Magnitude;

			//ggerade ToDo: Choose correct axis
			int numSlices = (int)(_volume.LongAxisMagnitude / spacingVector.Magnitude);

			//ggerade ToDo: Start on actual slice boundaries
			Vector3D startPoint = centerPoint - (numSlices / 2) * spacingVector;

			int sliceIndex;
			for (sliceIndex = 0; sliceIndex < numSlices; sliceIndex++)
			{
				Vector3D point = startPoint + sliceIndex * spacingVector;
				ImageSop imageSop = CreateSliceImageSop(point);
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);
				SetSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);
				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
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
			vtkImageData vtkSlice = GenerateVtkSlice(resliceAxes);

			byte[] pixelData = CreatePixelDataFromVtkSlice(vtkSlice);

			return pixelData;
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

		#endregion

		#region Dicom Slice Generation

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			vtkImageReslice reslicer = new vtkImageReslice();

			reslicer.SetInput(_volume._VtkVolume);
			reslicer.SetInformationInput(_volume._VtkVolume);

			reslicer.SetOutputDimensionality(2);

			// Note: When the VTK docs state that the output spacing defaults to the input data,
			//	apparently they mean the raw volume. Without this call the spacing that is provided in the
			//	input volume is not taken into account.
			reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

			//ggerade: This was just some experimental code that I might refer back to
			//vtkTransform transform = new vtkTransform();
			//transform.SetMatrix(MatrixToVtkMatrix(vol.OrientationPatientMatrix));
			//reslicer.SetResliceTransform(transform);

			reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

			reslicer.SetOutputExtent(0, _volume.LargestOutputImageDimension - 1, 0, _volume.LargestOutputImageDimension - 1, 0, 0);

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

			vtkExecutive exec = reslicer.GetExecutive();
			exec.Update();

			return reslicer.GetOutput();
		}

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
			columns = rows = _volume.LargestOutputImageDimension;
			sliceDataSet[DicomTags.Columns].SetUInt16(0, (ushort) columns);
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
			Vector3D topLeftOfSlicePatient = ConvertSliceCoordToPatient(new PointF(0, 0), columns, rows);

			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			return sliceDicom;
		}

		private Vector3D ConvertSliceCoordToPatient(PointF sliceCoord, int columns,
		                                            int rows)
		{
			Vector3D reslicePoint = GetReslicePoint(_resliceAxes);

			// Convert 2D slice coord to 3D volume point
			//
			Vector3D xVec = new Vector3D(_resliceAxes[0, 0], _resliceAxes[0, 1], _resliceAxes[0, 2]);
			Vector3D yVec = new Vector3D(_resliceAxes[1, 0], _resliceAxes[1, 1], _resliceAxes[1, 2]);

			// Determine spacing along our vectors, we'll use this to adjust our coordinates to
			//	account for the "fence-post" rule
			Vector3D spacingVec = new Vector3D(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);
			float spacingX = xVec.Dot(spacingVec);
			float spacingY = yVec.Dot(spacingVec);

#if false //ggerade ToDo: This still needs a reference point, do I need to generalize further?
	// Determine the plane coordinate of the reslice point, from this reference point we will
	//	offset to the sliceCoord along the plane.
			float resliceX = Math.Abs(xVec.Dot(reslicePoint));
			float resliceY = Math.Abs(yVec.Dot(reslicePoint));
			PointF reslicePlaneCoord = new PointF(resliceX / _volume.EffectiveSpacing, resliceY / _volume.EffectiveSpacing);

			// These offsets define the x and y vector magnitudes to arrive at our Volume point
			float offsetX = (reslicePlaneCoord.X - sliceCoord.X) * _volume.EffectiveSpacing + spacingX;
			float offsetY = (reslicePlaneCoord.Y - sliceCoord.Y) * _volume.EffectiveSpacing + spacingY;
#endif

			// These offsets define the x and y vector magnitudes to arrive at our point
			//ggerade ToRes: Relying on reslice point x/y components to be center volume, this is established
			//	by the caller... not sure this works for all cases
			PointF reslicePointRefCoord = new PointF(columns / 2f, rows / 2f);
			float offsetX1 = (reslicePointRefCoord.X - sliceCoord.X) * _volume.EffectiveSpacing - spacingX / 2;
			float offsetY1 = (reslicePointRefCoord.Y - sliceCoord.Y) * _volume.EffectiveSpacing - spacingY / 2;


			//Vector3D volumePoint = reslicePoint - (offsetX * xVec + offsetY * yVec);
			Vector3D volumePoint = reslicePoint - (offsetX1 * xVec + offsetY1 * yVec);

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

		private static Matrix CreateResliceAxesSagittal()
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{0, -1, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{1, 0, 0, 0},
			                        		{0, 0, 0, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesCoronal()
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{0, 1, 0, 0},
			                        		{0, 0, 0, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesAxial()
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

			Matrix axial = new Matrix(4, 4, new float[4,4]
			                                	{
			                                		{1, 0, 0, 0},
			                                		{0, 1, 0, 0},
			                                		{0, 0, 1, 0},
			                                		{0, 0, 0, 1}
			                                	});


			return aboutX * aboutY * aboutZ * axial;
		}

		private static Vector3D GetReslicePoint(Matrix resliceAxes)
		{
			return new Vector3D(resliceAxes[3, 0],
			                    resliceAxes[3, 1], resliceAxes[3, 2]);
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