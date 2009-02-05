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
	internal static class VolumeSlicer
	{
		#region Public methods

		#region Create ImageSop Utilities

		public static ImageSop CreateSagittalSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesSagittal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public static ImageSop CreateCoronalSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesCoronal(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		public static ImageSop CreateAxialSlice(Volume vol, Vector3D sliceThroughPoint)
		{
			Matrix resliceAxes = CreateResliceAxesAxial(sliceThroughPoint);

			return CreateSlice(vol, resliceAxes);
		}

		private static ImageSop CreateSlice(Volume vol, Matrix resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			return CreateSliceImageSopGeneratePixelData(vol, resliceAxes, vtkSlice);
		}

		#endregion

		#region Create DisplaySet utilities

		//ggerade ToDo: Provide spacing interface
		internal static DisplaySet CreateSagittalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Sagittal)", "MPR"),
			                                       String.Format("Sagittal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			float increment = 5 * vol.Spacing.X;
			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (float pos = vol.MinXCoord; pos < vol.MaxXCoord; pos += increment, sliceIndex++)
			{
				point.X = pos;

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

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			float increment = 5 * vol.Spacing.Y;
			int columns = 0, rows = 0;
			int sliceIndex = 0;
			for (float pos = vol.MinYCoord; pos < vol.MaxYCoord; pos += increment, sliceIndex++)
			{
				point.Y = pos; // coronal

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

			// Slice through this point, start with center
			Vector3D point = vol.CalcCenterPoint();

			float increment = vol.Spacing.Z;
			int columns = 0, rows = 0;
			int sliceIndex = 0;

			for (float pos = vol.MinZCoord; pos < vol.MaxZCoord; pos += increment, sliceIndex++)
			{
				point.Z = pos;

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

		private static ImageSop CreateSagittalSliceDelayPixelData(Volume vol, Vector3D point,
		                                                          ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesSagittal(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateCoronalSliceDelayPixelData(Volume vol, Vector3D point,
		                                                         ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesCoronal(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		private static ImageSop CreateAxialSliceDelayPixelData(Volume vol, Vector3D point,
		                                                       ref int columns, ref int rows)
		{
			Matrix resliceAxes = CreateResliceAxesAxial(point);

			if (columns == 0 || rows == 0)
				CalcOutputDimensions(vol, resliceAxes, out columns, out rows);

			ImageSop imageSop = CreateSliceImageSopDelayPixelData(vol, resliceAxes, columns, rows);
			return imageSop;
		}

		#endregion

		#endregion

		// This method is used by the VolumeSliceSopDataSource to generate pixel data on demand
		internal static byte[] GenerateFrameNormalizedPixelData(Volume vol, Matrix resliceAxes)
		{
			vtkImageData vtkSlice = GenerateVtkSlice(vol, resliceAxes);

			byte[] pixelData = CreatePixelDataFromVtkSlice(vtkSlice);

			return pixelData;
		}

		#endregion

		#region Implementation

		private static vtkImageData GenerateVtkSlice(Volume vol, Matrix resliceAxes)
		{
			// Extract slice in specified orientation
			vtkImageReslice reslicer = new vtkImageReslice();

			reslicer.SetInput(vol._VtkImageData);
			reslicer.SetInformationInput(vol._VtkImageData);

			reslicer.SetOutputDimensionality(2);
			
			//ggerade ToDo: Add interpolation mode interface
			//reslicer.SetInterpolationModeToLinear();
			reslicer.SetInterpolationModeToCubic();

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
			Vector3D topLeftOfSlicePatient = ConvertSliceCoordToPatient(new PointF(0, 0), vol, resliceAxes);

			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			return sliceDicom;
		}

		private static Vector3D ConvertSliceCoordToPatient(PointF sliceCoord, Volume vol, Matrix resliceAxes)
		{
			Vector3D reslicePoint = GetReslicePoint(resliceAxes);

			// First Convert 2D slice coord to 3D volume point
			//ggerade ToDo: This still needs to be generalized for non-orthogonal reslicing
			//	time to bust out some vector arithmetic...
			//ggerade ToRes: Why are the spacing adjustments necessary?
			Vector3D topLeftOfSlice;
			if (resliceAxes[0, 0] == 1 && resliceAxes[1, 1] == 1) // axial
			{
				topLeftOfSlice = new Vector3D(vol.MinXCoord + vol.Spacing.X + sliceCoord.X,
				                              vol.MinYCoord + vol.Spacing.Y + sliceCoord.Y,
				                              reslicePoint.Z);
			}
			else if (resliceAxes[0, 0] == 1 && resliceAxes[1, 2] == -1) // coronal
			{
				topLeftOfSlice = new Vector3D(vol.MinXCoord + vol.Spacing.X + sliceCoord.X,
				                              reslicePoint.Y,
				                              vol.MaxZCoord - vol.Spacing.Z + sliceCoord.Y);
			}
			else if (resliceAxes[0, 1] == -1 && resliceAxes[1, 2] == -1) // sagital
			{
				topLeftOfSlice = new Vector3D(reslicePoint.X,
				                              vol.MaxYCoord - vol.Spacing.Y + sliceCoord.Y,
				                              vol.MaxZCoord - vol.Spacing.Z + sliceCoord.X);
			}
			else
			{
				topLeftOfSlice = reslicePoint;
			}

			// Convert volume slice point to patient point
			return vol.ConvertToPatient(topLeftOfSlice);
		}

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
			reslicer.SetInput(vol._VtkImageData);
			reslicer.SetInformationInput(vol._VtkImageData);
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

		#endregion

		#endregion
	}
}