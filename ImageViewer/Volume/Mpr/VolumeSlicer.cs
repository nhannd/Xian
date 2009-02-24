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
#if true
			Vector3D sourceOrientationNormalPatient = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);
			Vector3D normalLinePatient = (endPointPatient - startPointPatient).Normalize();
			//Vector3D normalLinePatient = AbsoluteDelta(endPointPatient, startPointPatient).Normalize();
			Vector3D normalPerpLinePatient = sourceOrientationNormalPatient.Cross(normalLinePatient);

			Vector3D slicePlanePatientXNormal = normalLinePatient;
			Vector3D slicePlanePatientYNormal = sourceOrientationNormalPatient;
			Vector3D slicePlanePatientZNormal = normalPerpLinePatient;

			Matrix slicePlanePatientOrientation = new Matrix
				(4, 4, new float[4,4]
				       	{
				       		{slicePlanePatientXNormal.X, slicePlanePatientXNormal.Y, slicePlanePatientXNormal.Z, 0},
				       		{slicePlanePatientYNormal.X, slicePlanePatientYNormal.Y, slicePlanePatientYNormal.Z, 0},
				       		{slicePlanePatientZNormal.X, slicePlanePatientZNormal.Y, slicePlanePatientZNormal.Z, 0},
				       		{0, 0, 0, 1}
				       	});

			//Vector3D patientOrientationOrtho = sourceOrientationRowPatient.Cross(sourceOrientationColumnPatient);
			//Matrix srcPlanePatientOrientation = new Matrix
			//    (4, 4, new float[4, 4]
			//            {
			//                {sourceOrientationRowPatient.X, sourceOrientationRowPatient.Y, sourceOrientationRowPatient.Z, 0},
			//                {sourceOrientationColumnPatient.X, sourceOrientationColumnPatient.Y, sourceOrientationColumnPatient.Z, 0},
			//                {patientOrientationOrtho.X, patientOrientationOrtho.Y, patientOrientationOrtho.Z, 0},
			//                {0, 0, 0, 1}
			//            });
			//Matrix srcPlaneOrientation = _volume.RotateToVolumeOrientation(srcPlanePatientOrientation);

			_resliceAxes = _volume.RotateToVolumeOrientation(slicePlanePatientOrientation);

#elif false
			Vector3D patientOrientationOrtho = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);

			Matrix srcPlanePatientOrientation = new Matrix
				(4, 4, new float[4, 4]
				       	{
				       		{sourceOrientationColumnPatient.X, sourceOrientationColumnPatient.Y, sourceOrientationColumnPatient.Z, 0},
				       		{sourceOrientationRowPatient.X, sourceOrientationRowPatient.Y, sourceOrientationRowPatient.Z, 0},
				       		{patientOrientationOrtho.X, patientOrientationOrtho.Y, patientOrientationOrtho.Z, 0},
				       		{0, 0, 0, 1}
				       	});

			Matrix srcPlaneOrientation = _volume.RotateToVolumeOrientation(srcPlanePatientOrientation);
			//Vector3D srcPlaneXNormal = new Vector3D(srcPlaneOrientation[0, 0], srcPlaneOrientation[0, 1], srcPlaneOrientation[0, 2]);
			//Vector3D srcPlaneYNormal = new Vector3D(srcPlaneOrientation[1, 0], srcPlaneOrientation[1, 1], srcPlaneOrientation[1, 2]);
			Vector3D srcPlaneZNormal = new Vector3D(srcPlaneOrientation[2, 0], srcPlaneOrientation[2, 1], srcPlaneOrientation[2, 2]);

			Vector3D startPoint = _volume.ConvertToVolume(startPointPatient);
			Vector3D endPoint = _volume.ConvertToVolume(endPointPatient);

			Vector3D slicePlaneXNormal = srcPlaneZNormal;
			Vector3D slicePlaneYNormal = (endPoint - startPoint).Normalize();
			//Vector3D slicePlaneYNormal = AbsoluteDelta(endPoint, startPoint).Normalize();
			Vector3D slicePlaneZNormal = slicePlaneXNormal.Cross(slicePlaneYNormal);

			_resliceAxes =
				new Matrix
				(4, 4, new float[4, 4]
			            {
			                {slicePlaneXNormal.X, slicePlaneXNormal.Y, slicePlaneXNormal.Z, 0},
			                {slicePlaneYNormal.X, slicePlaneYNormal.Y, slicePlaneYNormal.Z, 0},
			                {slicePlaneZNormal.X, slicePlaneZNormal.Y, slicePlaneZNormal.Z, 0},
			                {0, 0, 0, 1}
			            });
#else
	//Vector3D sourceOrientationNormalPatient = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);
			Vector3D sourceOrientationNormalPatient = sourceOrientationRowPatient.Cross(sourceOrientationColumnPatient);
			Vector3D normalLinePatient = (endPointPatient - startPointPatient).Normalize();
			//Vector3D normalLinePatient = AbsoluteDelta(endPointPatient, startPointPatient).Normalize();
			//Vector3D normalPerpLinePatient = sourceOrientationNormalPatient.Cross(normalLinePatient);
			//Vector3D normalPerpLinePatient = normalLinePatient.Cross(sourceOrientationNormalPatient);
			//Vector3D slicePlanePatientXNormal = sourceOrientationNormalPatient;
			//Vector3D slicePlanePatientYNormal = normalLinePatient;
			//Vector3D slicePlanePatientZNormal = normalPerpLinePatient;



			Vector3D patientOrientationOrtho = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);

			Matrix srcPlanePatientOrientation = new Matrix
				(4, 4, new float[4, 4]
			            {
			                {sourceOrientationColumnPatient.X, sourceOrientationColumnPatient.Y, sourceOrientationColumnPatient.Z, 0},
			                {sourceOrientationRowPatient.X, sourceOrientationRowPatient.Y, sourceOrientationRowPatient.Z, 0},
			                {patientOrientationOrtho.X, patientOrientationOrtho.Y, patientOrientationOrtho.Z, 0},
			                {0, 0, 0, 1}
			            });

			Matrix srcPlaneOrientation = _volume.RotateToVolumeOrientation(srcPlanePatientOrientation);
			Matrix normal = new Matrix(4, 1);
			normal.SetColumn(0, sourceOrientationNormalPatient.X, sourceOrientationNormalPatient.Y, sourceOrientationNormalPatient.Z, 1F);
			Matrix rotatedNormal = srcPlaneOrientation * normal;
			Matrix line = new Matrix(4,1);
			normal.SetColumn(0, normalLinePatient.X,normalLinePatient.Y,normalLinePatient.Z,1f);
			Matrix rotatedLine = srcPlaneOrientation * line;

			Vector3D slicePlanePatientXNormal = new Vector3D(rotatedNormal[0,0],rotatedNormal[1,0],rotatedNormal[2,0]);
			Vector3D slicePlanePatientYNormal = new Vector3D(rotatedLine[0, 0], rotatedLine[1, 0], rotatedLine[2, 0]);
			Vector3D slicePlanePatientZNormal = slicePlanePatientXNormal.Cross(slicePlanePatientYNormal);
		

			Matrix slicePlanePatientOrientation = new Matrix
				(4, 4, new float[4,4]
				       	{
				       		{slicePlanePatientXNormal.X, slicePlanePatientXNormal.Y, slicePlanePatientXNormal.Z, 0},
				       		{slicePlanePatientYNormal.X, slicePlanePatientYNormal.Y, slicePlanePatientYNormal.Z, 0},
				       		{slicePlanePatientZNormal.X, slicePlanePatientZNormal.Y, slicePlanePatientZNormal.Z, 0},
				       		{0, 0, 0, 1}
				       	});




			_resliceAxes = _volume.RotateToVolumeOrientation(slicePlanePatientOrientation);
#endif

			Vector3D centerPointPatient = new Vector3D(
				startPointPatient.X + (endPointPatient.X - startPointPatient.X) / 2,
				startPointPatient.Y + (endPointPatient.X - startPointPatient.Y) / 2,
				startPointPatient.Z + (endPointPatient.X - startPointPatient.Z) / 2);

			Vector3D sliceThroughPoint = _volume.ConvertToVolume(centerPointPatient);

			this.SetReslicePoint(sliceThroughPoint);
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
			PopulateDisplaySet(displaySet);
			return displaySet;
		}

		internal void PopulateDisplaySet(IDisplaySet displaySet)
		{
			// Slice through this point
			Vector3D throughPoint;
			if (_sliceThroughPointPatient != null)
				throughPoint = _volume.ConvertToVolume(_sliceThroughPointPatient);
			else
				throughPoint = _volume.CenterPoint;

			Vector3D spacingVector = this.SliceSpacing * GetSliceNormalVector();

			//ggerade ToDo: Determine the length of the vector that passes through the volume
			// For now just use the longest axis by the spacingVector
			int numSlices = (int)(_volume.LongAxisMagnitude / spacingVector.Magnitude + 0.5f);

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

		#endregion

		#region Dicom Slice Generation

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				reslicer.SetInput(_volume._VtkVolume);
				reslicer.SetInformationInput(_volume._VtkVolume);

				reslicer.SetOutputDimensionality(2);

				reslicer.SetBackgroundLevel(_volume.PadValue);

				// Note: When the VTK docs state that the output spacing defaults to the input data,
				//	apparently they mean the raw volume. Without this call the spacing that is provided in the
				//	input volume is not taken into account.
				reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

				reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

				int sliceExtent = GetSliceExtent();
				reslicer.SetOutputExtent(0, sliceExtent - 1, 0, sliceExtent - 1, 0, 0);

#if false
	// Determine the center image volume point
	//
			Vector3D reslicePoint = GetReslicePoint(_resliceAxes);
			Vector3D xVec = new Vector3D(_resliceAxes[0, 0], _resliceAxes[0, 1], _resliceAxes[0, 2]);
			Vector3D yVec = new Vector3D(_resliceAxes[1, 0], _resliceAxes[1, 1], _resliceAxes[1, 2]);
			Vector3D n = xVec.Cross(yVec); // or zVec
			Vector3D w = reslicePoint - _volume.CenterPoint;
			Vector3D centerImageVolumePoint = n.Dot(w) * n;

			Vector3D delta = reslicePoint - centerImageVolumePoint;
			float deltaX = delta.Dot(xVec);
			float deltaY = delta.Dot(yVec);

			reslicer.SetOutputOrigin(deltaX, deltaY, 0);
#elif false
			if (_sliceThroughPointPatient != null)
			{
				//Vector3D reslicePoint = GetReslicePoint(_resliceAxes);
				//Matrix inputPoint = new Matrix(4, 1, new float[4, 1] { { reslicePoint.X }, { reslicePoint.Y }, { reslicePoint.Z }, { 1 } });
				//Matrix outputPoint = resliceAxes.Transpose() * inputPoint;
				//reslicer.SetOutputOrigin(outputPoint[0, 0], outputPoint[1, 0], 0);
								
				//Matrix inputOrigin = new Matrix(4, 1, new float[4, 1] { { 0 }, { 0 }, { 0 }, { 1 } });
				//Matrix outputOrigin = resliceAxes.Transpose() * inputOrigin;
				//reslicer.SetOutputOrigin(outputOrigin[0, 0], outputOrigin[1, 0], 0);
				
				Matrix inputOrigin = new Matrix(1, 4, new float[1, 4] { { 0, 0, 0, 1 } });
				Matrix outputOrigin = inputOrigin * resliceAxes;
				reslicer.SetOutputOrigin(-outputOrigin[0, 2], -outputOrigin[0, 1], 0);
			}
#endif

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
		private int GetSliceExtent()
		{
			if (_sliceExtentMm != 0f)
				return (int) (_sliceExtentMm / _volume.EffectiveSpacing);
			else
				return _volume.LargestOutputImageDimension;
		}

		// Used by VTK ResliceImage observer
		//private static void VtkReslicerExecutiveCallback(vtkObject vtkObj, uint eid, object obj, IntPtr nativeSomethingOrOther)
		//{
		//    Debug.WriteLine(eid);
		//}

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
			columns = rows = GetSliceExtent();
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

#if true
			// Determine the center image volume point
			//
			Vector3D n = xVec.Cross(yVec); // or zVec
			Vector3D w = reslicePoint - _volume.CenterPoint;

			Vector3D centerImageVolumePoint = n.Dot(w) * n;
#else
			Vector3D centerImageVolumePoint = new Vector3D(reslicePoint);
#endif

			PointF centerPlaneCoord = new PointF(columns / 2f, rows / 2f);

			// These offsets define the x and y vector magnitudes to arrive at our point
			float offsetX = (centerPlaneCoord.X - sliceCoord.X) * _volume.EffectiveSpacing + spacingX;
			float offsetY = (centerPlaneCoord.Y - sliceCoord.Y) * _volume.EffectiveSpacing + spacingY;

			Vector3D volumePoint = centerImageVolumePoint - (offsetX * xVec + offsetY * yVec);

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