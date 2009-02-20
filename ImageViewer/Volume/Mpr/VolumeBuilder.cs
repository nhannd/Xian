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

#define ALLOW_UNSAFE

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO: Comments from SB
	// Consider an interface like this as opposed to static methods.
	// VolumeBuilder builder = new VolumeBuilder(frames);
	// ...set options (e.g. strictness, etc). 
	// builder.Validate();
	///Volume volume = builder.Build(); //Build() can call Validate() too.
	/// <summary>
	/// This utility class aids in creating a Volume (currently a VTK volume) from a collection of Frames
	/// </summary>
	internal class VolumeBuilder
	{
		#region Public methods

		//ggerade ToDo: See Stewart's comments above
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
				if (Math.Round(currentSpacing * 100) != Math.Round(spacing * 100))
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
			// Sort the frames by slice location to ensure coordinate consistency in our volumes.
			// Sort by increasing location results in frames being added F to H, R to L, and A to P
			frames.Sort(new SliceLocationComparer());

			// Clone the first frame's dicom header info and use it as the volume model
			IDicomMessageSopDataSource dataSource = (IDicomMessageSopDataSource) frames[0].ParentImageSop.DataSource;
			DicomMessageBase firstFrameDicom = dataSource.SourceMessage;

			// Selectively copy from original headers
			DicomMessageBase modelDicomFile = new DicomFile("", new DicomAttributeCollection(),
			                                                CreateVolumeDataSet(firstFrameDicom.DataSet));

			// Use the first frame's orientation as our volume orientation
			Matrix orientationPatient = ImageOrientationPatientToMatrix(frames[0].ImageOrientationPatient);

			bool dataUnsigned = frames[0].PixelRepresentation == 0;

			Vector3D originPatient = new Vector3D((float) frames[0].ImagePositionPatient.X,
			                                      (float) frames[0].ImagePositionPatient.Y,
			                                      (float) frames[0].ImagePositionPatient.Z);

			Vector3D spacing = new Vector3D((float) frames[0].PixelSpacing.Column, (float) frames[0].PixelSpacing.Row,
			                                CalcSliceSpacing(frames));

			// Determine Gantry/Detector Tilt from orientation matrix
			double tilt = GetGantryTiltRadians(orientationPatient);

			// If the series was obtained with Gantry/Detector Tilt we will pad the frames as they're
			//	added to the volume to create a normalized cuboid volume that contains the tilted volume.
			// This is the number of rows that we will pad for each frame, it affects the overall
			//	dimensions of the volume.
			int padRows = GetNumberOfRowsToPad(frames, spacing, tilt);

			// Relying on the frames being uniform, so we'll base width/height off of first frame
			Vector3D dimensions = new Vector3D(frames[0].Columns, frames[0].Rows + padRows, frames.Count);

			if (dataUnsigned)
			{
				ushort[] volumeArray = BuildVolumeUnsignedShortArray(frames, dimensions, spacing, orientationPatient);

				Volume vol = new Volume(volumeArray, dimensions, spacing, originPatient, orientationPatient, modelDicomFile);
				return vol;
			}
			else
			{
				short[] volumeArray = BuildVolumeShortArray(frames, dimensions, spacing, orientationPatient);

				Volume vol = new Volume(volumeArray, dimensions, spacing, originPatient, orientationPatient, modelDicomFile);
				return vol;
			}
		}

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
			Vector3D xOrient = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
			Vector3D yOrient = new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY, (float) orientation.ColumnZ);
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

		private static double GetGantryTiltRadians(Matrix orientationPatient)
		{
			double aboutXradians = GetRotateAboutXRadians(orientationPatient);
			double aboutYradians = GetRotateAboutYRadians(orientationPatient);
			//double aboutZradians = GetRotateAboutZRadians(orientationPatient);

			double tilt = 0d;
			if (EqualWithinTolerance(aboutXradians, 0f, .001f) == false)
			{
				if (EqualWithinTolerance(aboutYradians, 0f, .001f) == false)
					throw new Exception("Patient orientation is tilted about X and Y, not supported");

				tilt = aboutXradians *
				       orientationPatient[0, 0]; // This flips euler sign in prone position, so that tilt is correctly signed
			}
			else if (EqualWithinTolerance(aboutYradians, 0f, .001f) == false)
			{
				tilt = aboutYradians *
				       orientationPatient[0, 1]; // This flips euler sign in Decubitus Left position
			}
			return tilt;
		}

		private static int GetNumberOfRowsToPad(List<Frame> frames, Vector3D spacing, double aboutXradians)
		{
			double padRowsMm = Math.Tan(aboutXradians) *
			                   (frames[frames.Count - 1].ImagePositionPatient.Z - frames[0].ImagePositionPatient.Z);
			// ensure this pad is always positive for sizing calculations
			return Math.Abs((int) (padRowsMm / spacing.Y + 0.5f));
		}

		private static double GetRotateAboutXRadians(Matrix orientationPatient)
		{
			return Math.Atan2(orientationPatient[2, 1], orientationPatient[2, 2]);
		}

		private static double GetRotateAboutYRadians(Matrix orientationPatient)
		{
			return -Math.Asin(orientationPatient[2, 0]);
		}

		// Unused for now, might be useful someday.
		//private static double GetRotateAboutZRadians(Matrix orientationPatient)
		//{
		//    return Math.Atan2(orientationPatient[1, 0], orientationPatient[0, 0]);
		//}

		//ggerade ToRes: I'd like to templatize this and the unsigned version below, but I'm not sure
		//	how to get the unsafe section templatized...
		private static short[] BuildVolumeShortArray(List<Frame> frames, Vector3D dimensions, Vector3D spacing,
		                                             Matrix orientationPatient)
		{
			int volumeSize = (int) (dimensions.X * dimensions.Y * dimensions.Z);
			short[] volumeArray = new short[volumeSize];
			float lastFramePos = (float) frames[frames.Count - 1].ImagePositionPatient.Z;
			// Determine Gantry/Detector Tilt from orientation matrix
			double tilt = GetGantryTiltRadians(orientationPatient);
			// Determine the number of rows to pad per frame, a function of the Z extent and tilt angle
			int padRows = GetNumberOfRowsToPad(frames, spacing, tilt);

			int imageIndex = 0;
			foreach (Frame frame in frames)
			{
				short padPixelValue = -1024; //ggerade ToDo: Make this min pixel or something smarter

				int start, end = 0;
				int padTop = 0;
				if (padRows > 0)  // Pad top
				{
					float deltaMm = lastFramePos - (float) frame.ImagePositionPatient.Z;
					double padTopMm = Math.Tan(tilt) * deltaMm;
					padTop = (int) (padTopMm / spacing.Y + 0.5f);
					// This accounts for the tilt in negative radians, we start padding from
					//	the bottom first in that case
					if (tilt < 0) padTop += padRows;

					start = imageIndex * (int) dimensions.X * (int) dimensions.Y;
					end = start + padTop * (int) dimensions.X;
					for (int i = start; i < end; i++)
						volumeArray[i] = padPixelValue;
				}

				// Copy frame data
				//
				byte[] frameData = frame.GetNormalizedPixelData();
				start = end;
				end = start + frameData.Length / sizeof (short);

#if ALLOW_UNSAFE
				unsafe
				{
					// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
					fixed (byte* pbFrame = frameData)
					{
						short* psFrame = (short*) pbFrame;
						int j = 0;
						for (int i = start; i < end; i++)
						{
							volumeArray[i] = psFrame[j];
							j++;
						}
					}
				}
#else // Safe alternative. Never measured performance difference but I assume it's not insignificant.
				int j = 0;
				for (int i = start; i < end; i++)
				{
					ushort lowbyte = frameData[j];
					ushort highbyte = frameData[j + 1];

					short val = (short)((highbyte << 8) | lowbyte);
					volumeArray[i] = val;

					j += 2;
				}
#endif
				imageIndex++;


				if (padRows > 0)  // Pad bottom
				{
					int padBottom = padRows - padTop;
					start = end;
					end = start + (padBottom * (int) dimensions.X);
					for (int i = start; i < end; i++)
						volumeArray[i] = padPixelValue;
				}
			}
			return volumeArray;
		}

		private static ushort[] BuildVolumeUnsignedShortArray(List<Frame> frames, Vector3D dimensions, Vector3D spacing,
		                                                      Matrix orientationPatient)
		{
			int volumeSize = (int) (dimensions.X * dimensions.Y * dimensions.Z);
			ushort[] volumeArray = new ushort[volumeSize];
			float lastFramePos = (float) frames[frames.Count - 1].ImagePositionPatient.Z;
			// Determine Gantry/Detector Tilt from orientation matrix
			double tilt = GetGantryTiltRadians(orientationPatient);
			// Determine the number of rows to pad per frame, a function of the Z extent and tilt angle
			int padRows = GetNumberOfRowsToPad(frames, spacing, tilt);

			int imageIndex = 0;
			foreach (Frame frame in frames)
			{
				ushort padPixelValue = 0; //ggerade ToDo: Make this min pixel or something smarter

				// Pad top
				//
				float deltaMm = lastFramePos - (float) frame.ImagePositionPatient.Z;
				double padTopMm = Math.Tan(tilt) * deltaMm;
				int padTop = (int) (padTopMm / spacing.Y + 0.5f);
				// This accounts for the tilt in negative radians, we start padding from
				//	the bottom first in that case
				if (tilt < 0) padTop += padRows;

				int start = imageIndex * (int) dimensions.X * (int) dimensions.Y;
				int end = start + padTop * (int) dimensions.X;
				for (int i = start; i < end; i++)
					volumeArray[i] = padPixelValue;

				// Copy frame data
				//
				byte[] frameData = frame.GetNormalizedPixelData();
				start = end;
				end = start + frameData.Length / sizeof (ushort);

#if ALLOW_UNSAFE
				unsafe
				{
					// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
					fixed (byte* pbFrame = frameData)
					{
						ushort* pusFrame = (ushort*) pbFrame;
						int j = 0;
						for (int i = start; i < end; i++)
						{
							volumeArray[i] = pusFrame[j];
							j++;
						}
					}
				}
#else // Safe code
				int j = 0;

				for (int i = start; i < end; i++)
				{
					ushort lowbyte = frameData[j];
					ushort highbyte = frameData[j + 1];
					volumeArray[i] = (ushort) ((highbyte << 8) | lowbyte);
					j += 2;
				}
#endif
				imageIndex++;

				// Pad bottom
				//
				int padBottom = padRows - padTop;
				start = end;
				end = start + (padBottom * (int) dimensions.X);
				for (int i = start; i < end; i++)
					volumeArray[i] = padPixelValue;
			}
			return volumeArray;
		}

		private static bool EqualWithinTolerance(double d1, double d2, float tolerance)
		{
			return Math.Abs(d1 - d2) < tolerance;
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