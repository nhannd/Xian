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

		internal static DisplaySet CreateSagittalDisplaySet(Volume vol)
		{
			DisplaySet displaySet = new DisplaySet(String.Format("{0} (Sagittal)", "MPR"),
			                                       String.Format("Sagittal.{0}", Guid.NewGuid()));

			// A new series UID for our new Sops
			string seriesInstanceUid = DicomUid.GenerateUid().UID;

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			Vector3D point = new Vector3D(vol.Origin);

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			float increment = 5*vol.SpacingX;
			//float increment = 1;

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

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			Vector3D point = new Vector3D(vol.Origin);

			// Arbitrarily chose an increment of 5 * spacing for now (generates about 100 images for 512x512 CT)
			float increment = 5*vol.SpacingY;
			//float increment = 1;

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

			// Slice through this point. Using origin for other axes ensures ImagePosition consistency.
			Vector3D point = new Vector3D(vol.Origin);

			int columns = 0, rows = 0;
			int sliceIndex = 0;

			//ggerade ToRes: This business ensures consistency of instance numbers, should I keep it?
			//	I'm not sure it's really relevant in the end, but based on the current implementation it keeps the
			//	"native" MPR DisplaySet in sync with the source DisplaySet.
			float start, end, increment;
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

			for (float pos = start;
			     vol.InstanceAndSliceLocationReversed ? pos >= end : pos <= end;
			     pos += increment, sliceIndex++)
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
			reslicer.SetInterpolationModeToLinear();

			// Note: When the VTK docs state that the output spacing defaults to the input data,
			//	apparently they mean the raw volume. Without this call the spacing that is provided in the
			//	input volume is not taken into account.
			reslicer.SetOutputSpacing(vol.SpacingX, vol.SpacingY, vol.SpacingZ);

			reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

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

			// Update orientation vectors
			//
			// Transform volume relative orientation to Dicom Orientation
			Matrix transformedResliceAxes = resliceAxes*vol.DicomOrientationPatientMatrix;

			ImageOrientationPatient imageOrientation =
				new ImageOrientationPatient(transformedResliceAxes[0, 0],
				                            transformedResliceAxes[0, 1],
				                            transformedResliceAxes[0, 2],
				                            transformedResliceAxes[1, 0],
				                            transformedResliceAxes[1, 1],
				                            transformedResliceAxes[1, 2]);

			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(0, (float) imageOrientation.RowX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(1, (float) imageOrientation.RowY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(2, (float) imageOrientation.RowZ);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(3, (float) imageOrientation.ColumnX);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(4, (float) imageOrientation.ColumnY);
			sliceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(5, (float) imageOrientation.ColumnZ);

			// Update image positions
			//
			//ggerade ToDo: Get the image vs patient orientations ironed out so that these can be normal transforms!
			float ippX = resliceAxes[3, 0];
			float ippY = resliceAxes[3, 1];
			float ippZ = resliceAxes[3, 2];

			if (vol.DicomOrientationPatientMatrix[0, 0] == 1 && vol.DicomOrientationPatientMatrix[1, 1] == 1) // axial vol
			{
				if (resliceAxes[0, 1] < 0) // sag slice
					ippY += (vol.Height - 1)*vol.SpacingY;
				if (resliceAxes[0, 2] < 0 || resliceAxes[1, 2] < 0) // sag/cor slices
					ippZ += (vol.Depth - 1)*vol.SpacingZ;

				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, ippX);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, ippY);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, ippZ);
			}
			else if (vol.DicomOrientationPatientMatrix[0, 0] == 1) // coronal vol
			{
				if (resliceAxes[0, 1] < 0) // sag slice
					ippY -= ((vol.Height - 5)*vol.SpacingY);
				if (resliceAxes[0, 0] == 1 && resliceAxes[1, 2] < 0) // cor slice
					ippY = ((vol.Height - 5)*vol.SpacingY) - ippY;
				if (resliceAxes[0, 2] < 0 || resliceAxes[1, 2] < 0) // sag/cor slice
					ippZ += (vol.Depth - 1)*vol.SpacingZ;
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, ippX);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, ippZ);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, ippY);
			}
			else // sag vol
			{
				if (resliceAxes[0, 1] < 0) // sag slice
					ippY -= ((vol.Width - 5)*vol.SpacingX);
				if (resliceAxes[0, 0] == 1 && resliceAxes[1, 2] < 0) // cor slice
					ippY = ((vol.Height - 5)*vol.SpacingY) - ippY;
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, ippZ);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, ippX);
				sliceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, ippY);
			}

			//ggerade ToRes: Any reason to write SliceLocation? Needs to be along the ortho vector.
			//float sliceLocation = 0;
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
		                                                          Matrix resliceAxes,
		                                                          int columns, int rows)
		{
			DicomFile clonedDicom = CreateSliceDicom(vol, resliceAxes, columns, rows);

			ImageSop imageSop = new ImageSop(new VolumeSliceSopDataSource(clonedDicom, vol, resliceAxes));

			return imageSop;
		}

		//ggerade ToOpt: Figure out how to get output dimensions withoug having to actually reslice the data
		private static void CalcOutputDimensions(Volume vol, Matrix resliceAxes, out int columns, out int rows)
		{
			vtkImageReslice reslicer = new vtkImageReslice();
			reslicer.SetInput(vol._VtkImageData);
			reslicer.SetInformationInput(vol._VtkImageData);
			reslicer.SetOutputDimensionality(2);
			reslicer.SetInterpolationModeToLinear();
			reslicer.SetOutputSpacing(vol.SpacingX, vol.SpacingY, vol.SpacingZ);
			reslicer.SetResliceAxes(MatrixToVtkMatrix(resliceAxes));

			vtkExecutive exec = reslicer.GetExecutive();
			exec.Update();

			vtkImageData imageData = reslicer.GetOutput();
			int[] dimensions = imageData.GetDimensions();
			columns = dimensions[0];
			rows = dimensions[1];
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
			int sliceDataSize = sliceDimensions[0]*sliceDimensions[1]*sliceDimensions[2];
			IntPtr sliceDataPtr = sliceImageData.GetScalarPointer();

			byte[] pixelData = new byte[sliceDataSize*sizeof (short)];

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
			return new Matrix(4, 4, new float[4, 4]
			                        	{
			                        		{1, 0, 0, 0},
			                        		{0, 0, -1, 0},
			                        		{0, 1, 0, 0},
			                        		{point.X, point.Y, point.Z, 1}
			                        	});
		}

		private static Matrix CreateResliceAxesAxial(Vector3D point)
		{
			return new Matrix(4, 4, new float[4, 4]
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