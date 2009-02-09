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
		#region fields

		private InterpolationModes _interpolationMode = InterpolationModes.Linear;

		#endregion

		#region Public methods

		#region Create ImageSop Utilities
		// Note: These interfaces are currently unused, keeping around for future utility purposes

		public ImageSop CreateSagittalSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesSagittal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public ImageSop CreateCoronalSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesCoronal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public ImageSop CreateAxialSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesAxial(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		private ImageSop CreateSlice(Volume vol, Matrix resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			return CreateSliceImageSopGeneratePixelData(vol, resliceAxes, vtkSlice);
		}

		#endregion

		#region Create DisplaySet utilities

		internal static DisplaySet CreateSagittalDisplaySet(Volume vol)
		{
			return CreateSagittalDisplaySet(vol, 0, 0);
		}

		//ggerade ToDo: Provide spacing interface
		internal static DisplaySet CreateSagittalDisplaySet(Volume vol, int rotateX, int rotateY)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Sagittal)", "MPR"),
												   String.Format("Sagittal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			// Arbitrarily chose an increment
			float spacing = 4 * vol.Spacing.X;
			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (float pos = vol.MinXCoord; pos + 0.001f < vol.MaxXCoord; sliceIndex++, pos = sliceIndex * spacing)
			{
				point.X = pos;

				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image. 
				ImageSop imageSop = CreateSagittalSliceDelayPixelData(vol, point, rotateX, rotateY, ref columns, ref rows);
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal static DisplaySet CreateCoronalDisplaySet(Volume vol)
		{
			return CreateCoronalDisplaySet(vol, 0, 0);
		}

		internal static DisplaySet CreateCoronalDisplaySet(Volume vol, int rotateX, int rotateY)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Coronal)", "MPR"),
			                                       String.Format("Coronal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			// Arbitrarily chose an increment
			float spacing = 4 * vol.Spacing.Y;
			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (float pos = vol.MinYCoord; pos + 0.001f < vol.MaxYCoord; sliceIndex++, pos = sliceIndex * spacing)
			{
				point.Y = pos; // coronal

				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image.
				ImageSop imageSop = CreateCoronalSliceDelayPixelData(vol, point, rotateX, rotateY, ref columns, ref rows);
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal static DisplaySet CreateAxialDisplaySet(Volume vol)
		{
			return CreateAxialDisplaySet(vol, 0, 0);
		}

		internal static DisplaySet CreateAxialDisplaySet(Volume vol, int rotateX, int rotateY)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Axial)", "MPR"),
												   String.Format("Axial.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			float spacing = vol.Spacing.Z;
			int columns = 0, rows = 0;
			int sliceIndex = 0;

			for (float pos = vol.MinZCoord; pos + 0.001f < vol.MaxZCoord; sliceIndex++, pos = sliceIndex * spacing)
			{
				point.Z = pos;

				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image.
				ImageSop imageSop = CreateAxialSliceDelayPixelData(vol, point, rotateX, rotateY, ref columns, ref rows);

				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal static DisplaySet CreateObliqueDisplaySet(Volume vol, int rotateX, int rotateY, int rotateZ)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Oblique)", "MPR"),
												   String.Format("Oblique.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			float spacing = vol.Spacing.Z;
			int sliceIndex = 0;
			int columns = 0, rows = 0;
#if true
			ImageSop imageSop = CreateObliqueSliceDelayPixelData(vol, point, rotateX, rotateY, rotateZ, ref columns, ref rows);

			DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

			PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);

			displaySet.PresentationImages.Add(presImage);
#else

			for (float pos = vol.MinZCoord; pos + 0.001f < vol.MaxZCoord; sliceIndex++, pos = sliceIndex * spacing)
			{
				point.Z = pos;

				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image.
				ImageSop imageSop = CreateAxialSliceDelayPixelData(vol, point, rotateX, rotateY, ref columns, ref rows);

				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, spacing);

				displaySet.PresentationImages.Add(presImage);
			}
#endif

			return displaySet;
		}

		private static void PatchUpSeriesLevelDicomAttributes(IImageSopProvider presImage, int sliceIndex,
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

		#region Create PresentationImages without pixel data

		//ggerade ToRef: Once I figure out the whole column/row calc business, then these should be part of the
		//	public interface, maybe an option to generate or not is in order?

		private static ImageSop CreateSagittalSliceDelayPixelData(Volume vol, Vector3D point, int rotateX, int rotateY, ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesSagittal(point, rotateX, rotateY);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateCoronalSliceDelayPixelData(Volume vol, Vector3D point, int rotateX, int rotateY, ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesCoronal(point, rotateX, rotateY);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateAxialSliceDelayPixelData(Volume vol, Vector3D point, int rotateX, int rotateY, ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesAxial(point, rotateX, rotateY);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateObliqueSliceDelayPixelData(Volume vol, Vector3D point, int rotateX, int rotateY, int rotateZ, ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesOblique(point, rotateX, rotateY, rotateZ);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		#endregion

		#endregion

		#region Slicer Settings

		internal enum InterpolationModes
		{
			NearestNeighbor,
			Linear,
			Cubic
		}

		internal InterpolationModes InterpolationMode
		{
			get { return _interpolationMode; }
			set { _interpolationMode = value; }
		}

		#endregion

		#endregion

		#region Implementation

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		internal byte[] GenerateFrameNormalizedPixelData(Volume vol, Matrix resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			byte[] pixelData = CreatePixelDataFromVtkSlice(vtkSlice);

			return pixelData;
		}

		#region Dicom Slice Generation

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Volume vol, Matrix resliceAxes)
		{
			vtkImageReslice reslicer = new vtkImageReslice();

			reslicer.SetInput(vol._VtkVolume);
			reslicer.SetInformationInput(vol._VtkVolume);

			reslicer.SetOutputDimensionality(2);

			// Note: When the VTK docs state that the output spacing defaults to the input data,
			//	apparently they mean the raw volume. Without this call the spacing that is provided in the
			//	input volume is not taken into account.
			reslicer.SetOutputSpacing(vol.Spacing.X, vol.Spacing.Y, vol.Spacing.Z);

			//ggerade: This was just some experimental code that I might refer back to
			//vtkTransform transform = new vtkTransform();
			//transform.SetMatrix(MatrixToVtkMatrix(vol.OrientationPatientMatrix));
			//reslicer.SetResliceTransform(transform);

			reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

			//reslicer.SetOutputExtent(0,511,0,511,0,0);

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

		private static DicomFile CreateSliceDicom(Volume vol, Matrix resliceAxes, int columns, int rows)
		{
			// Start with the volume's model Dicom attributes
			DicomMessageBase modelDicom = vol._ModelDicom;
			DicomFile sliceDicom = new DicomFile("", modelDicom.MetaInfo.Copy(), modelDicom.DataSet.Copy());
			DicomAttributeCollection sliceDataSet = sliceDicom.DataSet;

			// ensure each sop has unique Uid
			sliceDataSet[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);

			// Update rows and columns to reflect actual matrix size
			sliceDataSet[DicomTags.Columns].SetUInt16(0, (ushort) columns);
			sliceDataSet[DicomTags.Rows].SetUInt16(0, (ushort) rows);

			// Update Image Orientation (patient)
			//
			Matrix resliceAxesPatientOrientation = vol.RotateToPatientOrientation(resliceAxes);

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
			Vector3D topLeftOfSlicePatient = ConvertSliceCoordToPatient(new PointF(0, 0), vol, resliceAxes, columns, rows);

			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			return sliceDicom;
		}

		private static Vector3D ConvertSliceCoordToPatient(PointF sliceCoord, Volume vol, Matrix resliceAxes, int columns, int rows)
		{
			Vector3D reslicePoint = GetReslicePoint(resliceAxes);

			// Convert 2D slice coord to 3D volume point
			//
			Vector3D xVec = new Vector3D(resliceAxes[0, 0], resliceAxes[0, 1], resliceAxes[0, 2]);
			Vector3D yVec = new Vector3D(resliceAxes[1, 0], resliceAxes[1, 1], resliceAxes[1, 2]);

//ggerade ToDo: Get the more generalzed else block below working...
#if true
			Vector3D spacingVec = new Vector3D(vol.Spacing.X, vol.Spacing.Y, vol.Spacing.Z);
			float spacingX = Math.Abs(xVec.Dot(spacingVec));
			float spacingY = Math.Abs(yVec.Dot(spacingVec));

			// These offsets define the x and y vector magnitudes to arrive at our point
			//ggerade ToRes: Relying on reslice point x/y components to be center volume, this is established
			//	by the caller... not sure this works for all cases
			float offsetX = ((columns / 2f) - sliceCoord.X) * vol.EffectiveSpacing - spacingX / 2;
			float offsetY = ((rows / 2f) - sliceCoord.Y) * vol.EffectiveSpacing - spacingY / 2;
#else
			// Determine the plane coordinate of the reslice point, from this reference point we will
			//	offset to the sliceCoord along the plane.
			float resliceX = Math.Abs(xVec.Dot(reslicePoint));
			float resliceY = Math.Abs(yVec.Dot(reslicePoint));
			PointF reslicePlaneCoord = new PointF(resliceX / vol.EffectiveSpacing, resliceY / vol.EffectiveSpacing);

			// Determine spacing along our vectors, we'll use this to adjust our coordinates to
			//	account for the "fence-post" rule
			Vector3D spacingVec = new Vector3D(vol.Spacing.X, vol.Spacing.Y, vol.Spacing.Z);
			float spacingX = xVec.Dot(spacingVec);
			float spacingY = yVec.Dot(spacingVec);

			// These offsets define the x and y vector magnitudes to arrive at our Volume point
			float offsetX = (reslicePlaneCoord.X - sliceCoord.X) * vol.EffectiveSpacing + spacingX;
			float offsetY = (reslicePlaneCoord.Y - sliceCoord.Y) * vol.EffectiveSpacing + spacingY;
#endif

			Vector3D volumePoint = reslicePoint - (offsetX * xVec + offsetY * yVec);

			// Convert 3D volume point to patient point
			return vol.ConvertToPatient(volumePoint);
		}

		#endregion

		#region Delay pixel data stuff

		private static ImageSop CreateSliceImageSopDelayPixelData(Volume vol,
		                                                          Matrix resliceAxes,
		                                                          int columns, int rows)
		{
			DicomFile sliceDicom = CreateSliceDicom(vol, resliceAxes, columns, rows);

			ImageSop imageSop = new ImageSop(new VolumeSliceSopDataSource(sliceDicom, vol, resliceAxes));

			return imageSop;
		}

		//ggerade ToOpt: Figure out how to get output dimensions withoug having to actually reslice the data
		private static void CalcOutputDimensions(Volume vol, Matrix resliceAxes, out int columns, out int rows)
		{
			vtkImageReslice reslicer = new vtkImageReslice();
			reslicer.SetInput(vol._VtkVolume);
			reslicer.SetInformationInput(vol._VtkVolume);
			reslicer.SetOutputDimensionality(2);
			reslicer.SetInterpolationModeToNearestNeighbor();
			//reslicer.SetInterpolationModeToLinear();
			reslicer.SetOutputSpacing(vol.Spacing.X, vol.Spacing.Y, vol.Spacing.Z);
			reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));
			//reslicer.SetOutputExtent(0, 511, 0, 511, 0, 0);

			vtkExecutive exec = reslicer.GetExecutive();
			exec.Update();

			vtkImageData imageData = reslicer.GetOutput();
			int[] dimensions = imageData.GetDimensions();
			columns = dimensions[0];
			rows = dimensions[1];

			//int x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;
			//imageData.GetWholeExtent(ref x0, ref x1, ref y0, ref y1, ref z0, ref z1);

			//double x0=0, x1=0, y0=0, y1=0, z0=0, z1=0;
			//imageData.GetWholeBoundingBox(ref x0, ref x1, ref y0, ref y1, ref z0, ref z1);
		}

		#endregion

		#region Generate pixel data on Sop creation stuff

		private static ImageSop CreateSliceImageSopGeneratePixelData(Volume vol, Matrix resliceAxes,
		                                                             vtkImageData sliceImageData)
		{
			byte[] pixelData = CreatePixelDataFromVtkSlice(sliceImageData);

			int[] sliceDimensions = sliceImageData.GetDimensions();
			int columns = sliceDimensions[0];
			int rows = sliceDimensions[1];

			DicomFile clonedDicom = CreateSliceDicom(vol, resliceAxes, columns, rows);

			ImageSop imageSop = new ImageSop(new VolumeSliceSopDataSource(clonedDicom, pixelData));

			return imageSop;
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

		private static Vector3D GetReslicePoint(Matrix resliceAxes)
		{
			return new Vector3D(resliceAxes[3, 0],
			                    resliceAxes[3, 1], resliceAxes[3, 2]);
		}

		private static Matrix CreateResliceAxesSagittal(Vector3D point)
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{0, -1, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{1, 0, 0, 0},
			                        		{point.X, point.Y, point.Z, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesCoronal(Vector3D point)
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{0, 1, 0, 0},
			                        		{point.X, point.Y, point.Z, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesAxial(Vector3D point)
		{
			return new Matrix(4, 4, new float[4,4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 1, 0, 0},
			                        		{0, 0, 1, 0},
			                        		{point.X, point.Y, point.Z, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesSagittal(Vector3D point, int rotateX, int rotateY)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);

			//Matrix rotate = aboutX * aboutY;
			//Matrix pointMatrix = new Matrix(1, 4, new float[1, 4] { { point.X, point.Y, point.Z, 1f } });
			//Matrix rotatedPoint = pointMatrix * rotate;

			Matrix sagittal = new Matrix(4, 4, new float[4, 4]
			                                   	{
			                                   		{0, -1, 0, 0},
			                                   		{0, 0, -1, 0},
			                                   		{1, 0, 0, 0},
			                                   		{point.X, point.Y, point.Z, 1}
													//{rotatedPoint[0,0], rotatedPoint[0,1], rotatedPoint[0,2], 1}
			                                   	});

			return aboutX * aboutY * sagittal;
		}

		private static Matrix CreateResliceAxesCoronal(Vector3D point, int rotateX, int rotateY)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);

			Matrix coronal = new Matrix(4, 4, new float[4,4]
			                                	{
			                                		{1, 0, 0, 0},
			                                		{0, 0, -1, 0},
			                                		{0, 1, 0, 0},
			                                		{point.X, point.Y, point.Z, 1}
			                                	});


			return aboutX * aboutY * coronal;
		}

		private static Matrix CreateResliceAxesAxial(Vector3D point, int rotateX, int rotateY)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);

			Matrix axial = new Matrix(4, 4, new float[4, 4]
			                                	{
			                                		{1, 0, 0, 0},
			                                		{0, 1, 0, 0},
			                                		{0, 0, 1, 0},
			                                		{point.X, point.Y, point.Z, 1}
			                                	});


			return aboutX * aboutY * axial;
		}

		private static Matrix CreateResliceAxesOblique(Vector3D point, int rotateX, int rotateY, int rotateZ)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);
			Matrix aboutZ = CalcRotateMatrixAboutZ(rotateZ);

			Matrix axial = new Matrix(4, 4, new float[4, 4]
			                                	{
			                                		{1, 0, 0, 0},
			                                		{0, 1, 0, 0},
			                                		{0, 0, 1, 0},
			                                		{point.X, point.Y, point.Z, 1}
			                                	});


			return aboutX * aboutY * aboutZ * axial;
		}

		private static readonly Matrix _identityMatrix = new Matrix(4, 4, new float[4, 4]
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