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
	/// <summary>
	/// This utility class aids in creating a Volume (currently a VTK volume) from a collection of Frames
	/// </summary>
	internal class VolumeBuilder
	{
		#region Private fields

		private readonly List<Frame> _frames;
		private Size3D _dimensions;
		private Vector3D _spacing;
		private Matrix _orientationPatient;
		private bool _dataUnsigned;
		// The pixel value we will use to pad areas of images extracted from this volume. Note that this not
		//	directly mapped to DICOM Pixel Padding Value (which is used for this if present)
		private int? _padValue;
		private double _tilt;
		private int _padRows;
		private int _paddedTop;

		#endregion

		#region Public methods

		public VolumeBuilder(List<Frame> frames)
		{
			_frames = frames;
		}

		//ggerade ToDo: Do away with this old school interface, use exceptions ala the DICOM validator
		//  - Should give a reason for failure, hints or something
		//	- Allow for orientation and spacing allowable tolerances
		//  - Should have a way to add smarts to try to correct (not validate, but prepare maybe?) would need
		//		to tie that into a UI that allowed user to have a say. So need a inclusion/exclusion state or something?
		//  - Would be nice to have smarts to group likely candidates, maybe allow multiple MPRs to load. Need a volume
		//		selection shelf or something.
		//  - How to deal with overlaps? "holes" in the set? Uneven spacing?
		//
		// As a first prototype we just have a yay or nay interface with a simple reason to show to the user
		public bool ValidateFrames(out string reason)
		{
			// Ensure we have at least 3? frames
			if (_frames.Count < 3)
			{
				reason = "Display Set must have at least 3 frames";
				return false;
			}

			// Ensure all frames have the same orientation
			ImageOrientationPatient orient = _frames[0].ImageOrientationPatient;
			foreach (Frame frame in _frames)
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
			_frames.Sort(new SliceLocationComparer());
			float spacing = 0;
			Frame lastFrame = _frames[0];
			for (int i = 1; i < _frames.Count; i++)
			{
				Frame currentFrame = _frames[i];
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

		public Volume BuildVolume()
		{
			// Sort the frames by slice location to ensure coordinate consistency in our volumes.
			// Sort by increasing location results in frames being added F to H, R to L, and A to P
			//ggerade ToRes: Does this stick with the caller's frame collection?
			_frames.Sort(new SliceLocationComparer());

			// Clone the first frame's dicom header info and use it as the volume model
			IDicomMessageSopDataSource dataSource = (IDicomMessageSopDataSource) _frames[0].ParentImageSop.DataSource;
			DicomMessageBase firstFrameDicom = dataSource.SourceMessage;

			// Selectively copy from original headers
			DicomMessageBase modelDicomFile = new DicomFile("", new DicomAttributeCollection(),
			                                                CreateVolumeDataSet(firstFrameDicom.DataSet));

			// Use the first frame's orientation as our volume orientation
			_orientationPatient = ImageOrientationPatientToMatrix(_frames[0].ImageOrientationPatient);

			_dataUnsigned = _frames[0].PixelRepresentation == 0;

			CheckDicomForPadValue(modelDicomFile);

			Vector3D originPatient = new Vector3D((float) _frames[0].ImagePositionPatient.X,
			                                      (float) _frames[0].ImagePositionPatient.Y,
			                                      (float) _frames[0].ImagePositionPatient.Z);

			_spacing = new Vector3D((float) _frames[0].PixelSpacing.Column, (float) _frames[0].PixelSpacing.Row,
			                        CalcSliceSpacing(_frames));

			// Determine Gantry/Detector Tilt from orientation matrix
			_tilt = GetGantryTiltRadians();

			// If the series was obtained with Gantry/Detector Tilt we will pad the frames as they're
			//	added to the volume to create a normalized cuboid volume that contains the tilted volume.
			// This is the number of rows that we will pad for each frame, it affects the overall
			//	dimensions of the volume.
			_padRows = GetNumberOfRowsToPad(_tilt);

			// Relying on the frames being uniform, so we'll base width/height off of first frame
			_dimensions = new Size3D(_frames[0].Columns, _frames[0].Rows + _padRows, _frames.Count);

			if (_dataUnsigned)
			{
				ushort[] volumeArray = BuildVolumeUnsignedShortArray();

				Volume vol = new Volume(volumeArray, _dimensions, _spacing, originPatient, _orientationPatient, modelDicomFile,
				                        (int) _padValue);
				return vol;
			}
			else
			{
				short[] volumeArray = BuildVolumeShortArray();

				Volume vol = new Volume(volumeArray, _dimensions, _spacing, originPatient, _orientationPatient, modelDicomFile,
				                        (int) _padValue);
				return vol;
			}
		}

		// Check DICOM header for indication of value to be used for padding. Leaves _padValue null if not found.
		private void CheckDicomForPadValue(DicomMessageBase dicom)
		{
			DicomAttributeCollection dataSet = dicom.DataSet;
			//ggerade ToRes: any other elements I should check? Ok with SmallestImage (seems like it would be close enough to me)?
			// Note: If you add any additional checks for tags here, realize that the volume model dicom 
			//	is what's currently passed in and therefore the tags should be included in CreateVolumeDataSet
			if (dataSet.Contains(DicomTags.PixelPaddingValue))
			{
				if (_dataUnsigned)
					_padValue = (ushort)dataSet[DicomTags.PixelPaddingValue].GetInt32(0, 0);
				else
					_padValue = (short)dataSet[DicomTags.PixelPaddingValue].GetInt32(0, 0);
			}
			else if (dataSet.Contains(DicomTags.SmallestPixelValueInSeries))
			{
				if (_dataUnsigned)
					_padValue = (ushort)dataSet[DicomTags.SmallestPixelValueInSeries].GetInt32(0, 0);
				else
					_padValue = (short)dataSet[DicomTags.SmallestPixelValueInSeries].GetInt32(0, 0);
			}
			else if (dataSet.Contains(DicomTags.SmallestImagePixelValue))
			{
				if (_dataUnsigned)
					_padValue = (ushort)dataSet[DicomTags.SmallestImagePixelValue].GetInt32(0, 0);
				else
					_padValue = (short)dataSet[DicomTags.SmallestImagePixelValue].GetInt32(0, 0);
			}
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
			volumeDataSet[DicomTags.PixelPaddingValue] = srcDataSet[DicomTags.PixelPaddingValue];
			volumeDataSet[DicomTags.PixelPaddingRangeLimit] = srcDataSet[DicomTags.PixelPaddingRangeLimit];
			volumeDataSet[DicomTags.SmallestImagePixelValue] = srcDataSet[DicomTags.SmallestImagePixelValue];
			volumeDataSet[DicomTags.LargestImagePixelValue] = srcDataSet[DicomTags.LargestImagePixelValue];
			volumeDataSet[DicomTags.SmallestPixelValueInSeries] = srcDataSet[DicomTags.SmallestPixelValueInSeries];
			volumeDataSet[DicomTags.LargestPixelValueInSeries] = srcDataSet[DicomTags.LargestPixelValueInSeries];

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

		private double GetGantryTiltRadians()
		{
			double aboutXradians = GetRotateAboutXRadians(_orientationPatient);
			double aboutYradians = GetRotateAboutYRadians(_orientationPatient);
			//double aboutZradians = GetRotateAboutZRadians(_orientationPatient);

			double tilt = 0d;
			if (EqualWithinTolerance(aboutXradians, 0f, .1f) == false)
			{
				if (EqualWithinTolerance(aboutYradians, 0f, .1f) == false)
					throw new Exception("Patient orientation is tilted about X and Y, not supported");

				tilt = aboutXradians *
				       _orientationPatient[0, 0]; // This flips euler sign in prone position, so that tilt is correctly signed
			}
			else if (EqualWithinTolerance(aboutYradians, 0f, .1f) == false)
			{
				tilt = aboutYradians *
				       _orientationPatient[0, 1]; // This flips euler sign in Decubitus Left position
			}
			return tilt;
		}

		private int GetNumberOfRowsToPad(double tilt)
		{
			double padRowsMm = Math.Tan(tilt) *
			                   (_frames[_frames.Count - 1].ImagePositionPatient.Z - _frames[0].ImagePositionPatient.Z);
			// ensure this pad is always positive for sizing calculations
			return Math.Abs((int) (padRowsMm / _spacing.Y + 0.5f));
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
		private short[] BuildVolumeShortArray()
		{
			short[] volumeArray = new short[_dimensions.Size];
			float lastFramePos = (float) _frames[_frames.Count - 1].ImagePositionPatient.Z;

			// Pramatism won out Stewart. This appears to be fairly quick operation, even on large datasets...
			// ToOpt: This iteration over the frames could be eliminated by refactoring the code below
			//	such that it first copies the frames to the volumes, then sets the padded pixel values.
			if (_padValue == null)
				_padValue = GetMinShortPixelValue();

			int imageIndex = 0;
			foreach (Frame frame in _frames)
			{
				_paddedTop = 0;
				int end = PadTop(volumeArray, frame, imageIndex, lastFramePos, (short) _padValue);

				// Copy frame data
				//
				byte[] frameData = frame.GetNormalizedPixelData();
				int start = end;
				end = start + frameData.Length / sizeof (short);

#if ALLOW_UNSAFE
				unsafe
				{
					// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
					fixed (byte* pbFrame = frameData)
					{
						short* psFrame = (short*) pbFrame;
						for (int i = start, j = 0; i < end; i++, j++)
							volumeArray[i] = psFrame[j];
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

				PadBottom(volumeArray, end, (short) _padValue);
			}
			return volumeArray;
		}

		private ushort[] BuildVolumeUnsignedShortArray()
		{
			ushort[] volumeArray = new ushort[_dimensions.Size];
			float lastFramePos = (float) _frames[_frames.Count - 1].ImagePositionPatient.Z;

			// Pramatism won out Stewart. This appears to be fairly quick operation, even on large datasets...
			if (_padValue == null)
				_padValue = GetMinUnsignedShortPixelValue();

			int imageIndex = 0;
			foreach (Frame frame in _frames)
			{
				_paddedTop = 0;
				int end = PadTop(volumeArray, frame, imageIndex, lastFramePos, (ushort) _padValue);

				// Copy frame data
				//
				byte[] frameData = frame.GetNormalizedPixelData();
				int start = end;
				end = start + frameData.Length / sizeof (ushort);

#if ALLOW_UNSAFE
				unsafe
				{
					// The fixed statement "pins" the frame data, ensuring the GC won't move the referenced data
					fixed (byte* pbFrame = frameData)
					{
						ushort* pusFrame = (ushort*) pbFrame;
						for (int i = start, j = 0; i < end; i++, j++)
							volumeArray[i] = pusFrame[j];
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

				PadBottom(volumeArray, end, (ushort) _padValue);
			}
			return volumeArray;
		}

		private unsafe int GetMinShortPixelValue()
		{
			int minValue = int.MaxValue;
			foreach (Frame frame in _frames)
			{
				byte[] frameData = frame.GetNormalizedPixelData();
				fixed (byte* pbFrame = frameData)
				{
					short* psFrame = (short*) pbFrame;
					for (int i = 0; i < frameData.Length / sizeof (short); i++)
						if (psFrame[i] < minValue)
							minValue = psFrame[i];
				}
			}
			return minValue;
		}

		private unsafe int GetMinUnsignedShortPixelValue()
		{
			int minValue = int.MaxValue;
			foreach (Frame frame in _frames)
			{
				byte[] frameData = frame.GetNormalizedPixelData();
				fixed (byte* pbFrame = frameData)
				{
					ushort* pusFrame = (ushort*) pbFrame;
					for (int i = 0; i < frameData.Length / sizeof (short); i++)
						if (pusFrame[i] < minValue)
							minValue = pusFrame[i];
				}
			}
			return minValue;
		}

		private int PadTop<T>(T[] volumeArray, Frame frame, int imageIndex, float lastFramePos, T pixelPaddingValue)
		{
			int end;
			if (_padRows > 0)
			{
				float deltaMm = lastFramePos - (float) frame.ImagePositionPatient.Z;
				double padTopMm = Math.Tan(_tilt) * deltaMm;
				_paddedTop = (int) (padTopMm / _spacing.Y + 0.5f);
				// This accounts for the tilt in negative radians, we start padding from
				//	the bottom first in that case
				if (_tilt < 0) _paddedTop += _padRows;

				int start = imageIndex * _dimensions.Width * _dimensions.Height;
				end = start + _paddedTop * _dimensions.Width;
				for (int i = start; i < end; i++)
					volumeArray[i] = pixelPaddingValue;
			}
			else
				end = imageIndex * _dimensions.Width * _dimensions.Height;
			return end;
		}

		private void PadBottom<T>(T[] volumeArray, int end, T pixelPaddingValue)
		{
			if (_padRows > 0) // Pad bottom
			{
				int padBottom = _padRows - _paddedTop;
				int start = end;
				end = start + (padBottom * _dimensions.Width);
				for (int i = start; i < end; i++)
					volumeArray[i] = pixelPaddingValue;
			}
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