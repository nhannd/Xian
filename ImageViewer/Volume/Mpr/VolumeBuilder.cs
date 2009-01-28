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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// This utility class aids in creating a Volume (currently a VTK volume) from a collection of Frames
	/// </summary>
	internal class VolumeBuilder
	{
		#region Public methods

		//ggerade ToRes: What should this interface be like? Some things to think about:
		//  - Should give a reason for failure, hints or something
		//	- Perhaps need a first pass that throws out collections that just won't work at all?
		//	- Allow for orientation and spacing allowable tolerances
		//  - Should have a way to add smarts to try to correct (not validate, but prepare maybe?) would need
		//		to tie that into a UI that allowed user to have a say. So need a inclusion/exclusion state or something?
		//  - Would be nice to have smarts to group likely candidates, maybe allow multiple MPRs to load. Need a volume
		//		selection shelf or something.
		//  - How to deal with overlaps? "holes" in the set? Uneven spacing?
		//
		// As a first prototype we just have a yay or nay interface with a simple reason to show to the user
		public static bool ValidateFrames(List<Frame> frames, out string reason)
		{
			// Ensure we have at least 3? frames
			if (frames.Count < 3)
			{
				reason = "Display Set must have at least 3 frames";
				return false;
			}

			// Ensure all frames have the same orientation
			ImageOrientationPatient orient = frames[0].ImageOrientationPatient;
			foreach (Frame frame in frames)
			{
				if (frame.ImageOrientationPatient.IsNull)
				{
					reason = "Each image in Display Set must have orientation set";
					return false;
				}
				if (frame.ImageOrientationPatient.Equals(orient) == false)
				{
					reason = "Each image in Display Set must have same orientation";
					return false;
				}
			}

			// Ensure all frames are equally spaced
			frames.Sort(new SliceLocationComparer());
			float spacing = 0;
			Frame lastFrame = frames[0];
			for (int i = 1; i < frames.Count; i++)
			{
				Frame currentFrame = frames[i];
				float currentSpacing = CalcSpaceBetweenPlanes(currentFrame, lastFrame);
				if (currentSpacing < 0.01f)
				{
					reason = "Two images are at same patient location, MPR requires evenly spaced images";
					return false;
				}
				if (spacing == 0f)
					spacing = currentSpacing;
				if (Math.Round(currentSpacing * 1000) != Math.Round(spacing * 1000))
				{
					reason = "Inconsistent spacing betweeen images, MPR requires evenly spaced images";
					return false;
				}
				lastFrame = currentFrame;
			}

			reason = string.Empty;
			return true;
		}

		// Test out idea of grouping compatible frames (would need some serious hardening if we like the idea...)
		public static List<List<Frame>> SplitFrameGroups(List<Frame> frames)
		{
			List<List<Frame>> frameGroups = new List<List<Frame>>();

			// Group by like orientation
			ImageOrientationPatient currentOrient = new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			List<Frame> currentList = null;
			foreach (Frame frame in frames)
			{
				if (currentList == null || (frame.ImageOrientationPatient.Equals(currentOrient) == false))
				{
					// Don't include groups with less than 3 frames
					if (currentList != null && currentList.Count > 2)
						frameGroups.Add(currentList);
					currentList = new List<Frame>();
					currentOrient = frame.ImageOrientationPatient;
				}
				currentList.Add(frame);
			}
			if (currentList != null && currentList.Count > 2)
				frameGroups.Add(currentList);

			return frameGroups;
		}

		public static Volume BuildVolume(List<Frame> frames)
		{
			// Sort the frames by instance/frame number to determine first frame and
			//	Instance vs Location directions
			frames.Sort(new InstanceAndFrameNumberComparer());

			// Clone the first frame's dicom header info and use it as the volume model
			IDicomMessageSopDataSource dataSource = (IDicomMessageSopDataSource)frames[0].ParentImageSop.DataSource;
			DicomMessageBase firstFrameDicom = dataSource.SourceMessage;

			// Selectively copy from original headers
			DicomMessageBase modelDicomFile = new DicomFile("", new DicomAttributeCollection(),
			                                                CreateVolumeDataSet(firstFrameDicom.DataSet));

			// Use the first frame's orientation as our volume orientation
			Matrix volumeOrientation = ImageOrientationPatientToMatrix(frames[0].ImageOrientationPatient);

			//ggerade ToRes: This is currently used by the slicer to ensure consistency of instance numbers,
			//	I'm not sure it's really relevant in the end, but based on the current implementation it keeps the
			//	"native" MPR DisplaySet in sync with the source DisplaySet.
			//double direction = frames[1].SliceLocation - frames[0].SliceLocation;
			double direction = Math.Abs(volumeOrientation[2, 0]) * (frames[1].ImagePositionPatient.X - frames[0].ImagePositionPatient.X) +
			                   volumeOrientation[2, 1] * (frames[1].ImagePositionPatient.Y - frames[0].ImagePositionPatient.Y) +
			                   volumeOrientation[2, 2] * (frames[1].ImagePositionPatient.Z - frames[0].ImagePositionPatient.Z);
			bool instanceAndSliceLocationReversed = direction < 0;

			vtkImageData imageData = new vtkImageData();

			bool dataUnsigned = frames[0].PixelRepresentation == 0;

			// Currently relying on the frames being uniform, so we'll just use the first frame for width/height
			int width = frames[0].Columns;
			int height = frames[0].Rows;
			int depth = frames.Count;
			int volumeSize = width * height * depth;
			imageData.SetDimensions(width, height, depth);

			double spacingX = frames[0].PixelSpacing.Column;
			double spacingY = frames[0].PixelSpacing.Row;
			double spacingZ = CalcSliceSpacing(frames);
			imageData.SetSpacing(spacingX, spacingY, spacingZ);

			double originX, originY, originZ;

			// Now sort by slice location to ensure we get the VTK coordinates in sync
			if (volumeOrientation[0,0] == 1 && volumeOrientation[1,1] == 1) // axial
			{
				frames.Sort(new SliceLocationComparer());
				originX = frames[0].ImagePositionPatient.X;
				originY = frames[0].ImagePositionPatient.Y;
				originZ = frames[0].ImagePositionPatient.Z;
			}
			else if (volumeOrientation[0,0] == 1)  // coronal
			{
				frames.Sort(new SliceLocationComparer(true));
				originX = frames[0].ImagePositionPatient.X;
				originY = frames[0].ImagePositionPatient.Z;
				originZ = frames[0].ImagePositionPatient.Y;
			}
			else
			{
				frames.Sort(new SliceLocationComparer(true));
				originX = frames[0].ImagePositionPatient.Y;
				originY = frames[0].ImagePositionPatient.Z;
				originZ = frames[0].ImagePositionPatient.X;
			}

			imageData.SetOrigin(originX, originY, originZ);

			// Transform patient origin to normalized VTK origin
			//Vector3D vtkOrigin = TransformPatientOrigin(frames[0].ImagePositionPatient, volumeOrientation);
			//imageData.SetOrigin(vtkOrigin.X, vtkOrigin.Y, vtkOrigin.Z);

			if (dataUnsigned)
			{
				imageData.SetScalarTypeToUnsignedShort();
				imageData.AllocateScalars();

				ushort[] volumeArray = BuildVolumeUnsignedShortArray(frames, volumeSize);

				imageData.GetPointData().SetScalars(CreateVtkUnsignedShortArrayWrapper(volumeArray));

				// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
				imageData.UpdateInformation();

				Volume vol = new Volume(volumeArray, imageData, volumeOrientation, modelDicomFile, instanceAndSliceLocationReversed);
				return vol;
			}
			else
			{
				imageData.SetScalarTypeToShort();
				imageData.AllocateScalars();

				short[] volumeArray = BuildVolumeShortArray(frames, volumeSize);

				imageData.GetPointData().SetScalars(CreateVtkShortArrayWrapper(volumeArray));

				imageData.UpdateInformation();

				Volume vol = new Volume(volumeArray, imageData, volumeOrientation, modelDicomFile, instanceAndSliceLocationReversed);
				return vol;
			}
		}

		// This was intended to create a volume without the vtkImageData, may still be useful
		//public static Volume BuildVolume2(IList<Frame> frames)
		//{
		//    // Clone the first frame's dicom header info and use it as the volume model
		//    DicomFile firstFrameDicomFile = (DicomFile)frames[1].ParentImageSop.NativeDicomObject;
		//    DicomFile modelDicomFile = new DicomFile("", firstFrameDicomFile.MetaInfo.Copy(), firstFrameDicomFile.DataSet.Copy());

		//    Volume vol = new Volume(volShortArray, dataUnsigned,
		//                            frames[0].Columns, frames[0].Rows, frames.Count,
		//                            frames[0].PixelSpacing.Column, frames[0].PixelSpacing.Row, CalcSliceSpacing(frames),
		//                            0, 0, 0,
		//                            modelDicomFile);
		//    return vol;
		//}

		#endregion

		#region Implementation

		private static DicomAttributeCollection CreateVolumeDataSet(DicomAttributeCollection srcDataSet)
		{
			DicomAttributeCollection volumeDataSet = new DicomAttributeCollection();

			//Sop Common
			volumeDataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);

			//Patient
			volumeDataSet[DicomTags.PatientId] = srcDataSet[DicomTags.PatientId].Copy();
			volumeDataSet[DicomTags.PatientsName] = srcDataSet[DicomTags.PatientsName].Copy();

			volumeDataSet[DicomTags.PatientsBirthDate] = srcDataSet[DicomTags.PatientsBirthDate].Copy();
			volumeDataSet[DicomTags.PatientsSex] = srcDataSet[DicomTags.PatientsSex].Copy();

			//Study
			volumeDataSet[DicomTags.StudyInstanceUid] = srcDataSet[DicomTags.StudyInstanceUid].Copy();
			volumeDataSet[DicomTags.StudyDate] = srcDataSet[DicomTags.StudyDate].Copy();
			volumeDataSet[DicomTags.StudyTime] = srcDataSet[DicomTags.StudyTime].Copy();
			volumeDataSet[DicomTags.AccessionNumber] = srcDataSet[DicomTags.AccessionNumber].Copy();
			volumeDataSet[DicomTags.StudyDescription] = srcDataSet[DicomTags.StudyDescription].Copy();

			volumeDataSet[DicomTags.ReferringPhysiciansName] = srcDataSet[DicomTags.ReferringPhysiciansName].Copy();
			volumeDataSet[DicomTags.StudyId] = srcDataSet[DicomTags.StudyId].Copy();

			//Series
			volumeDataSet[DicomTags.Modality] = srcDataSet[DicomTags.Modality].Copy();
			volumeDataSet[DicomTags.SeriesNumber].SetStringValue("0");
			volumeDataSet[DicomTags.SeriesDescription] = srcDataSet[DicomTags.SeriesDescription].Copy();

			//SC Equipment
			volumeDataSet[DicomTags.ConversionType].SetStringValue("WSD");

			//General Image
			volumeDataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
			volumeDataSet[DicomTags.PixelSpacing] = srcDataSet[DicomTags.PixelSpacing].Copy();
			volumeDataSet[DicomTags.FrameOfReferenceUid] = srcDataSet[DicomTags.FrameOfReferenceUid].Copy();

			//Image Pixel
			volumeDataSet[DicomTags.SamplesPerPixel] = srcDataSet[DicomTags.SamplesPerPixel].Copy();
			volumeDataSet[DicomTags.PhotometricInterpretation] = srcDataSet[DicomTags.PhotometricInterpretation].Copy();
			volumeDataSet[DicomTags.BitsAllocated] = srcDataSet[DicomTags.BitsAllocated].Copy();
			volumeDataSet[DicomTags.BitsStored] = srcDataSet[DicomTags.BitsStored].Copy();
			volumeDataSet[DicomTags.HighBit] = srcDataSet[DicomTags.HighBit].Copy();
			volumeDataSet[DicomTags.PixelRepresentation] = srcDataSet[DicomTags.PixelRepresentation].Copy();

			volumeDataSet[DicomTags.WindowWidth] = srcDataSet[DicomTags.WindowWidth].Copy();
			volumeDataSet[DicomTags.WindowCenter] = srcDataSet[DicomTags.WindowCenter].Copy();
			volumeDataSet[DicomTags.RescaleSlope] = srcDataSet[DicomTags.RescaleSlope].Copy();
			volumeDataSet[DicomTags.RescaleIntercept] = srcDataSet[DicomTags.RescaleIntercept].Copy();

			return volumeDataSet;
		}

		private static float CalcSliceSpacing(IList<Frame> frames)
		{
			if (frames.Count < 2)
				return 0.0f;

			return CalcSpaceBetweenPlanes(frames[0], frames[1]);
		}

		private static float CalcSpaceBetweenPlanes(Frame frame1, Frame frame2)
		{
			Vector3D point1 = frame1.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D point2 = frame2.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D delta = point1 - point2;

			return delta.IsNull ? 0f : delta.Magnitude;
		}

		private static Matrix ImageOrientationPatientToMatrix(ImageOrientationPatient orientation)
		{
			Vector3D xOrient = new Vector3D((float)orientation.RowX, (float)orientation.RowY, (float)orientation.RowZ);
			Vector3D yOrient = new Vector3D((float)orientation.ColumnX, (float)orientation.ColumnY, (float)orientation.ColumnZ);
			Vector3D zOrient = xOrient.Cross(yOrient);

			Matrix matrix = new Matrix(4, 4, new float[4,4]
			                                 	{
			                                 		{xOrient.X, xOrient.Y, xOrient.Z, 0},
			                                 		{yOrient.X, yOrient.Y, yOrient.Z, 0},
			                                 		{zOrient.X, zOrient.Y, zOrient.Z, 0},
			                                 		{0, 0, 0, 1}
			                                 	});
			return matrix;
		}

		private static Vector3D TransformPatientOrigin(ImagePositionPatient patientOrigin, Matrix volumeOrientation)
		{
			Matrix normalPatientOrigin = new Matrix(4, 4,
			                                        new float[4,4]
			                                        	{
			                                        		{1, 0, 0, 0},
			                                        		{0, 1, 0, 0},
			                                        		{0, 0, 1, 0},
			                                        		{
			                                        			(float)patientOrigin.X, (float)patientOrigin.Y,
			                                        			(float)patientOrigin.Z, 1
			                                        		}
			                                        	});

			Matrix transformedOrigin = normalPatientOrigin * volumeOrientation;
			return new Vector3D(transformedOrigin[3, 0], transformedOrigin[3, 1], transformedOrigin[3, 2]);
		}

		private static short[] BuildVolumeShortArray(IEnumerable<Frame> frames, int volumeSize)
		{
			short[] volumeArray = new short[volumeSize];

			int imageIndex = 0;

			foreach (Frame frame in frames)
			{
				byte[] frameData = frame.GetNormalizedPixelData();

				int start = imageIndex * frameData.Length / sizeof(short);
				int end = start + frameData.Length / sizeof(short);

#if false // Safe code. Below, used unsafe code to work with shorts directly
				int j = 0;
				for (int i = start; i < end; i++)
				{
					ushort lowbyte = frameData[j];
					ushort highbyte = frameData[j + 1];

					short val = (short)((highbyte << 8) | lowbyte);
					volumeArray[i] = val;

					j += 2;
				}
#else
				unsafe
				{
					// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
					fixed (byte* pbFrame = frameData)
					{
						short* psFrame = (short*)pbFrame;
						int j = 0;
						for (int i = start; i < end; i++)
						{
							volumeArray[i] = psFrame[j];
							j++;
						}
					}
				}
#endif

				imageIndex++;
			}
			return volumeArray;
		}

		private static ushort[] BuildVolumeUnsignedShortArray(IEnumerable<Frame> frames, int volumeSize)
		{
			ushort[] volumeArray = new ushort[volumeSize];

			int imageIndex = 0;

			foreach (Frame frame in frames)
			{
				byte[] sliceData = frame.GetNormalizedPixelData();

				int start = imageIndex * sliceData.Length / 2;
				int end = start + sliceData.Length / 2;

				int j = 0;

				for (int i = start; i < end; i++)
				{
					ushort lowbyte = sliceData[j];
					ushort highbyte = sliceData[j + 1];
					volumeArray[i] = (ushort)((highbyte << 8) | lowbyte);
					j += 2;
				}

				imageIndex++;
			}
			return volumeArray;
		}

		private static vtkShortArray CreateVtkShortArrayWrapper(short[] shortArray)
		{
			vtkShortArray vtkShortArray = new vtkShortArray();
			vtkShortArray.SetArray(shortArray, shortArray.Length, 1);
			return vtkShortArray;
		}

		private static vtkUnsignedShortArray CreateVtkUnsignedShortArrayWrapper(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

#if false // this may be useful for working with unmanaged memory instead of pinned managed memory
		private static unsafe IntPtr BuildVolumeUnmanagedShortArray(IEnumerable<Frame> frames, int volumeSize)
		{
			short* psVolArray = (short*)Memory.Alloc(volumeSize*sizeof(short));

			int imageIndex = 0;

			foreach (Frame frame in frames)
			{
				byte[] frameData = frame.GetNormalizedPixelData();

				int start = imageIndex * frameData.Length / sizeof(short);
				int end = start + frameData.Length / sizeof(short);

				// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
				fixed (byte* pbFrame = frameData)
				{
					short* psFrame = (short*)pbFrame;
					int j = 0;
					for (int i = start; i < end; i++)
					{
						psVolArray[i] = psFrame[j];
						j++;
					}
				}

				imageIndex++;
			}

			return (IntPtr)psVolArray;
		}
#endif

		#endregion
	}
}

#if false // this may be useful for comparing unmanaged vs pinned memory

public static unsafe class Memory
{
	// Handle for the process heap. This handle is used 
	// in all calls to
	// the HeapXXX APIs in the methods below.
	private static int ph = GetProcessHeap();
	// Allocates a memory block of the given size. 
	// The allocated memory is
	// automatically initialized to zero.
	public static void* Alloc(int size)
	{
		void* result = HeapAlloc(ph, HEAP_ZERO_MEMORY, size);
		if (result == null) throw new OutOfMemoryException();
		return result;
	}

	// Copies count bytes from src to dst. The source and destination
	// blocks are permitted to overlap.
	public static void Copy(void* src, void* dst, int count)
	{
		byte* ps = (byte*)src;
		byte* pd = (byte*)dst;
		if (ps > pd)
		{
			for (; count != 0; count--) *pd++ = *ps++;
		}
		else if (ps < pd)
		{
			for (ps += count, pd += count; count != 0; count--)
				*--pd = *--ps;
		}
	}

	// Frees a memory block.
	public static void Free(void* block)
	{
		if (!HeapFree(ph, 0, block)) throw new InvalidOperationException();
	}

	// Re-allocates a memory block. If the reallocation request is for a
	// larger size, the additional region of memory is automatically
	// initialized to zero.
	public static void* ReAlloc(void* block, int size)
	{
		void* result = HeapReAlloc(ph, HEAP_ZERO_MEMORY, block, size);
		if (result == null) throw new OutOfMemoryException();
		return result;
	}

	// Returns the size of a memory block.
	public static int SizeOf(void* block)
	{
		int result = HeapSize(ph, 0, block);
		if (result == -1) throw new InvalidOperationException();
		return result;
	}

	// Heap API flags
	private const int HEAP_ZERO_MEMORY = 0x00000008;
	// Heap API functions
	[DllImport("kernel32")]
	private static extern int GetProcessHeap();

	[DllImport("kernel32")]
	private static extern void* HeapAlloc(int hHeap, int flags, int size);

	[DllImport("kernel32")]
	private static extern bool HeapFree(int hHeap, int flags, void* block);

	[DllImport("kernel32")]
	private static extern void* HeapReAlloc(int hHeap, int flags,
	                                        void* block, int size);

	[DllImport("kernel32")]
	private static extern int HeapSize(int hHeap, int flags, void* block);
}
#endif