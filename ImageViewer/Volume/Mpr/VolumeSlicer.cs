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
	public static class VolumeSlicer
	{
		#region Public methods

		#region Create ImageSop Utilities

		//ggerade ToRes: What to use for 3D point? look at Vector3D. What about the .NET ones in Media3D?
		public static ImageSop CreateSagittalSlice(Volume vol, double[] sliceThroughPoint)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesSagittal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public static ImageSop CreateCoronalSlice(Volume vol, double[] sliceThroughPoint)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesCoronal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public static ImageSop CreateAxialSlice(Volume vol, double[] sliceThroughPoint)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesAxial(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		private static ImageSop CreateSlice(Volume vol, vtkMatrix4x4 resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			return CreateSliceImageSopGeneratePixelData(vol, resliceAxes, vtkSlice);
		}

		#endregion

		#region Create DisplaySet utilities

		internal static DisplaySet CreateSagittalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Sagittal)", "MPR"),
			                                       String.Format("Sagittal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			double[] point = new double[3];
			point[1] = vol.OriginY;
			point[2] = vol.OriginZ;

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			double increment = 5 * vol.SpacingX;
			//double increment = 1;

			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (double pos = vol.MinXCoord; pos < vol.MaxXCoord; pos += increment, sliceIndex++)
			{
				point[0] = pos;

#if PREGEN_PIXELDATA
				ImageSop imageSop = CreateSagittalSlice(vol, point);
#else
				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image. 
				//ggerade ToDo: Note that this assumes all slices will be the same size, which is only true for orthognal slices
				//	of a cuboid volume. We'll need a solution for the not so nice cases.
				ImageSop imageSop = CreateSagittalSliceDelayPixelData(vol, point, ref columns, ref rows);
#endif
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, increment);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal static DisplaySet CreateCoronalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Coronal)", "MPR"),
			                                       String.Format("Coronal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			double[] point = new double[3];
			point[0] = vol.OriginX;
			point[2] = vol.OriginZ;

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			double increment = 5 * vol.SpacingY;
			//double increment = 1;

			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (double pos = vol.MinYCoord; pos < vol.MaxYCoord; pos += increment, sliceIndex++)
			{
				point[1] = pos; // coronal

#if PREGEN_PIXELDATA
				ImageSop imageSop = CreateCoronalSlice(vol, point);
#else
				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image.
				ImageSop imageSop = CreateCoronalSliceDelayPixelData(vol, point, ref columns, ref rows);
#endif
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, increment);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		internal static DisplaySet CreateAxialDisplaySet(Volume vol)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Axial)", "MPR"),
			                                       String.Format("Axial.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			double[] point = new double[3];
			point[0] = vol.OriginX;
			point[1] = vol.OriginY;

			int columns = 0, rows = 0;
			int sliceIndex = 0;

			//ggerade ToRes: This business ensures consistency of instance numbers, should I keep it?
			//	I'm not sure it's really relevant in the end, but based on the current implementation it keeps the
			//	"native" MPR DisplaySet in sync with the source DisplaySet.
			double start, end, increment;
			if (vol.InstanceAndSliceLocationReversed)
			{
				start = vol.MaxZCoord;
				end = vol.MinZCoord;
				increment = -vol.SpacingZ;
			}
			else
			{
				start = vol.MinZCoord;
				end = vol.MaxZCoord;
				increment = vol.SpacingZ;
			}

			for (double pos = start;
			     vol.InstanceAndSliceLocationReversed ? pos >= end : pos <= end;
			     pos += increment, sliceIndex++)
			{
				point[2] = pos;

#if PREGEN_PIXELDATA
				ImageSop imageSop = CreateAxialSlice(vol, point);
#else
				// First time through columns and rows will be calculated, subsequent calls will use the values returned
				//	from the first image.
				ImageSop imageSop = CreateAxialSliceDelayPixelData(vol, point, ref columns, ref rows);
#endif
				DicomGrayscalePresentationImage presImage = new DicomGrayscalePresentationImage(imageSop.Frames[1]);

				PatchUpSeriesLevelDicomAttributes(presImage, sliceIndex, seriesInstanceUid, increment);

				displaySet.PresentationImages.Add(presImage);
			}

			return displaySet;
		}

		private static void PatchUpSeriesLevelDicomAttributes(IImageSopProvider presImage, int sliceIndex,
		                                                      string seriesInstanceUid, double increment)
		{
			IDicomMessageSopDataSource dicomData = (IDicomMessageSopDataSource)presImage.ImageSop.DataSource;
			dicomData.SourceMessage.DataSet[DicomTags.SeriesInstanceUid].SetString(0, seriesInstanceUid);
			dicomData.SourceMessage.DataSet[DicomTags.InstanceNumber].SetString(0, Convert.ToString(sliceIndex + 1));

			// Note: These are required by the spatial locator
			float thicknessAndSpacing = (float)Math.Abs(increment);
			dicomData.SourceMessage.DataSet[DicomTags.SliceThickness].SetFloat32(0, thicknessAndSpacing);
			dicomData.SourceMessage.DataSet[DicomTags.SpacingBetweenSlices].SetFloat32(0, thicknessAndSpacing);
		}

		#region Create PresentationImages without pixel data

		//ggerade ToRef: Once I figure out the whole column/row calc business, then these should be part of the
		//	public interface, maybe an option to generate or not is in order?

		private static ImageSop CreateSagittalSliceDelayPixelData(Volume vol, double[] point,
		                                                          ref int columns, ref int rows)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesSagittal(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateCoronalSliceDelayPixelData(Volume vol, double[] point,
		                                                         ref int columns, ref int rows)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesCoronal(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateAxialSliceDelayPixelData(Volume vol, double[] point,
		                                                       ref int columns, ref int rows)
		{
			vtkMatrix4x4 resliceAxes = CreateResliceAxesAxial(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		#endregion

		#endregion

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		internal static byte[] GenerateFrameNormalizedPixelData(Volume vol, vtkMatrix4x4 resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			byte[] pixelData = CreatePixelDataFromVtkSlice(vtkSlice);

			return pixelData;
		}

		#endregion

		#region Implementation

		private static vtkImageData GenerateVtkSlice(Volume vol, vtkMatrix4x4 resliceAxes)
		{
			// Extract slice in specified orientation
			vtkImageReslice reslicer = new vtkImageReslice();

			reslicer.SetInput(vol._VtkImageData);
			reslicer.SetInformationInput(vol._VtkImageData);

			reslicer.SetOutputDimensionality(2);
			reslicer.SetInterpolationModeToLinear();

			// Note: When the VTK docs state that the output spacing defaults to the input data,
			//	apparently they mean the raw volume. Without this call the spacing that is provided in the
			//	input volume is not taken into account.
			reslicer.SetOutputSpacing(vol.SpacingX, vol.SpacingY, vol.SpacingZ);

			reslicer.SetResliceAxes(resliceAxes);

			vtkExecutive exec = reslicer.GetExecutive();
			exec.Update();

			return reslicer.GetOutput();
		}

		private static DicomFile CreateSliceDicom(Volume vol, vtkMatrix4x4 resliceAxes, int columns, int rows)
		{
			// Start with the volume's model Dicom attributes
			DicomMessageBase modelDicom = vol._ModelDicom;
			DicomFile sliceDicom = new DicomFile("", modelDicom.MetaInfo.Copy(), modelDicom.DataSet.Copy());
			DicomAttributeCollection sliceDataSet = sliceDicom.DataSet;

			// ensure each sop has unique Uid
			sliceDataSet[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);

			// Update rows and columns to reflect actual matrix size
			sliceDataSet[DicomTags.Columns].SetUInt16(0, (ushort)columns);
			sliceDataSet[DicomTags.Rows].SetUInt16(0, (ushort)rows);

			// Update orientation vectors
			//
			// Transform volume relative orientation to Dicom Orientation
			vtkMatrix4x4 transformedResliceAxes = TransformResliceAxes(resliceAxes, vol.DicomOrientationPatientMatrix);

			ImageOrientationPatient imageOrientation =
				new ImageOrientationPatient(transformedResliceAxes.GetElement(0, 0),
				                            transformedResliceAxes.GetElement(1, 0),
				                            transformedResliceAxes.GetElement(2, 0),
				                            transformedResliceAxes.GetElement(0, 1),
				                            transformedResliceAxes.GetElement(1, 1),
				                            transformedResliceAxes.GetElement(2, 1));

			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(0, (float)imageOrientation.RowX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(1, (float)imageOrientation.RowY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(2, (float)imageOrientation.RowZ);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(3, (float)imageOrientation.ColumnX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(4, (float)imageOrientation.ColumnY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(5, (float)imageOrientation.ColumnZ);

			// Update image positions
			//
			double imagePositionPatientX = transformedResliceAxes.GetElement(0, 3);
			double imagePositionPatientY = transformedResliceAxes.GetElement(1, 3);
			double imagePositionPatientZ = transformedResliceAxes.GetElement(2, 3);

			//ggerade ToRes: Image positions of negative direction vectors need to be adjusted, why?
			if (transformedResliceAxes.GetElement(0, 0) < 0 || transformedResliceAxes.GetElement(0, 1) < 0)
				imagePositionPatientX += (vol.Width - 1) * vol.SpacingX;
			if (transformedResliceAxes.GetElement(1, 0) < 0 || transformedResliceAxes.GetElement(1, 1) < 0)
				imagePositionPatientY += (vol.Height - 1) * vol.SpacingY;
			if (transformedResliceAxes.GetElement(2, 0) < 0 || transformedResliceAxes.GetElement(2, 1) < 0)
				imagePositionPatientZ += (vol.Depth - 1) * vol.SpacingZ;

			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, (float)imagePositionPatientX);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, (float)imagePositionPatientY);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, (float)imagePositionPatientZ);

			//ggerade ToRes: Any reason to write SliceLocation? Needs to be along the ortho vector.
			//double sliceLocation = 0;
			//if (crossVec.X != 0)
			//    sliceLocation = resliceAxes.GetElement(0, 3);
			//else if (crossVec.Y != 0)
			//    sliceLocation = resliceAxes.GetElement(1, 3);
			//else if (crossVec.Z != 0)
			//    sliceLocation = resliceAxes.GetElement(2, 3);
			//attribs[DicomTags.SliceLocation].SetFloat32(0, (float)sliceLocation);

			return sliceDicom;
		}

		#region Delay pixel data stuff

		private static ImageSop CreateSliceImageSopDelayPixelData(Volume vol,
		                                                          vtkMatrix4x4 resliceAxes,
		                                                          int columns, int rows)
		{
			DicomFile clonedDicom = CreateSliceDicom(vol, resliceAxes, columns, rows);

			ImageSop imageSop = new ImageSop(new VolumeSliceSopDataSource(clonedDicom, vol, resliceAxes));

			return imageSop;
		}

		//ggerade ToOpt: Figure out how to get output dimensions withoug having to actually reslice the data
		private static void CalcOutputDimensions(Volume vol, vtkMatrix4x4 resliceAxes, out int columns, out int rows)
		{
			vtkImageReslice reslicer = new vtkImageReslice();
			reslicer.SetInput(vol._VtkImageData);
			reslicer.SetInformationInput(vol._VtkImageData);
			reslicer.SetOutputDimensionality(2);
			reslicer.SetInterpolationModeToLinear();
			reslicer.SetOutputSpacing(vol.SpacingX, vol.SpacingY, vol.SpacingZ);
			reslicer.SetResliceAxes(resliceAxes);

			vtkExecutive exec = reslicer.GetExecutive();
			exec.Update();

			vtkImageData imageData = reslicer.GetOutput();
			int[] dimensions = imageData.GetDimensions();
			columns = dimensions[0];
			rows = dimensions[1];
		}

		#endregion

		#region Generate pixel data on Sop creation stuff

		private static ImageSop CreateSliceImageSopGeneratePixelData(Volume vol, vtkMatrix4x4 resliceAxes,
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

			byte[] rawFrameData = new byte[sliceDataSize * sizeof(short)];

			//ggerade ToOpt: Find an optimized way to do this memcpy (investigate IL cpblk opcode)
			CopySliceToFrame(rawFrameData, sliceDataPtr, sliceDataSize);
			return rawFrameData;
		}

		private static unsafe void CopySliceToFrame(byte[] rawFrameData, IntPtr slicePtr, int sliceDataSize)
		{
			short* psSlice = (short*)slicePtr;

			// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
			fixed (byte* pbFrame = rawFrameData)
			{
				//ggerade ToOpt: Find a fast memcpy like call. Better yet, find a way not to copy...
				short* psFrame = (short*)pbFrame;
				for (int i = 0; i < sliceDataSize; ++i)
				{
					psFrame[i] = psSlice[i];
				}
			}
		}

		#endregion

		#region Reslice Matrix helpers

		private static vtkMatrix4x4 TransformResliceAxes(vtkMatrix4x4 resliceAxes, Matrix volumeOrientationMatrix)
		{
			Matrix resliceAxesMatrix = VtkMatrixToMatrix(resliceAxes);
			Matrix transformedMatrix = resliceAxesMatrix * volumeOrientationMatrix;
			return MatrixToVtkMatrix(transformedMatrix);
		}

		// Note: vtkMatrix4x4 and Math.Matrix are transposed! This would appear to be due to the
		//	fact that vtkMatrix4x4 uses an x,y interface where Matrix uses a row,col interface.
		private static Matrix VtkMatrixToMatrix(vtkMatrix4x4 vtkMatrix)
		{
			float[,] array = new float[4,4];
			Matrix matrix = new Matrix(4, 4, array);

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					matrix[row, column] = (float)vtkMatrix.GetElement(column, row);

			return matrix;
		}

		private static vtkMatrix4x4 MatrixToVtkMatrix(Matrix matrix)
		{
			vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					vtkMatrix.SetElement(column, row, matrix[row, column]);

			return vtkMatrix;
		}

		private static vtkMatrix4x4 CreateResliceAxesSagittal(double[] point)
		{
			double[] sagittalElements = {
			                            	0, 0, 1, 0,
			                            	-1, 0, 0, 0,
			                            	0, -1, 0, 0,
			                            	0, 0, 0, 1
			                            };

			// Set the slice orientation
			vtkMatrix4x4 resliceAxesMatrix = new vtkMatrix4x4();
			resliceAxesMatrix.DeepCopy(sagittalElements);
			SetSlicePoint(resliceAxesMatrix, point);
			return resliceAxesMatrix;
		}

		private static vtkMatrix4x4 CreateResliceAxesCoronal(double[] point)
		{
			double[] coronalElements = {
			                           	1, 0, 0, 0,
			                           	0, 0, 1, 0,
			                           	0, -1, 0, 0,
			                           	0, 0, 0, 1
			                           };

			// Set the slice orientation
			vtkMatrix4x4 resliceAxesMatrix = new vtkMatrix4x4();
			resliceAxesMatrix.DeepCopy(coronalElements);
			SetSlicePoint(resliceAxesMatrix, point);
			return resliceAxesMatrix;
		}

		private static vtkMatrix4x4 CreateResliceAxesAxial(double[] point)
		{
			double[] axialElements = {
			                         	1, 0, 0, 0,
			                         	0, 1, 0, 0,
			                         	0, 0, 1, 0,
			                         	0, 0, 0, 1
			                         };

			// Set the slice orientation
			vtkMatrix4x4 resliceAxesMatrix = new vtkMatrix4x4();
			resliceAxesMatrix.DeepCopy(axialElements);
			SetSlicePoint(resliceAxesMatrix, point);
			return resliceAxesMatrix;
		}

		private static void SetSlicePoint(vtkMatrix4x4 resliceAxes, double[] xyz)
		{
			// Set the point through which to slice
			resliceAxes.SetElement(0, 3, xyz[0]);
			resliceAxes.SetElement(1, 3, xyz[1]);
			resliceAxes.SetElement(2, 3, xyz[2]);
		}

		#endregion

		#endregion
	}
}