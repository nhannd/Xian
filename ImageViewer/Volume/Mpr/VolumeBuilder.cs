#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal delegate void FrameLoadedDelegate(Frame frame);

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
		// The pixel value we will use to pad areas of images extracted from this volume. 
		// Note: not necessarily derived from DICOM Pixel Padding Value (used for this if present)
		private int? _pixelPadValue;
		private double _tilt;
		private int _padRows;
		private int _paddedRowsAtTop;
		private FrameLoadedDelegate _frameLoadedCallback;

		#endregion

		#region Public methods

		public VolumeBuilder(List<Frame> frames)
		{
			_frames = frames;
		}

		public void SetFrameLoadedCallback(FrameLoadedDelegate callback)
		{
			_frameLoadedCallback = callback;
		}

		//TODO: Do away with this old school interface, use exceptions ala the DICOM validator
		//  - Should have a way to add smarts to try to correct (not validate, but prepare maybe?) would need
		//		to tie that into a UI that allowed user to have a say. So need a inclusion/exclusion state or something?
		//  - Would be nice to have smarts to group likely candidates, maybe allow multiple MPRs to load. Need a volume
		//		selection shelf or something.
		//  - Deal with overlaps
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
				if (frame.ImageOrientationPatient.EqualsWithinTolerance(orient, .01f) == false)
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
				if (VolumeHelper.EqualsWithinTolerance(currentSpacing, spacing, .01f) == false)
				{
					reason = "Inconsistent spacing betweeen images, MPR requires evenly spaced images";
					return false;
				}
				lastFrame = currentFrame;
			}

			// Check for rotations about both X and Y (gantry correction only supports rotations about one
			//	at a time, which is standard Gantry/Detector Tilt)
			if (CheckForTwoTilts())
			{
				reason = "Images are tilted along multiple axes";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		/// <summary>
		/// Create and populate a Volume from the frames associated with this builder. It is expected that
		/// the Validate call has been made prior to calling this.
		/// </summary>
		/// <returns></returns>
		public Volume BuildVolume()
		{
			// This ensures slices are sorted consistently
			_frames.Sort(new SliceLocationComparer());

			// Clone the first frame's dicom header info and use it as the volume model
			IDicomMessageSopDataSource dataSource = (IDicomMessageSopDataSource) _frames[0].ParentImageSop.DataSource;
			DicomMessageBase firstFrameDicom = dataSource.SourceMessage;

			// Selectively copy from original headers
			DicomMessageBase modelDicomFile = new DicomFile("", new DicomAttributeCollection(),
			                                                CreateVolumeDataSet(firstFrameDicom.DataSet));

			_dataUnsigned = _frames[0].PixelRepresentation == 0;

			DeterminePixelPadValue(modelDicomFile);

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
				ushort[] volumeArray = BuildVolumeArray((ushort)_pixelPadValue);

				Volume vol = new Volume(volumeArray, _dimensions, _spacing, originPatient, OrientationPatient, modelDicomFile,
				                        (int) _pixelPadValue);
				return vol;
			}
			else
			{
				short[] volumeArray = BuildVolumeArray((short)_pixelPadValue);

				Volume vol = new Volume(volumeArray, _dimensions, _spacing, originPatient, OrientationPatient, modelDicomFile,
				                        (int) _pixelPadValue);
				return vol;
			}
		}

		#endregion

		#region Implementation

		#region Model Dicom data set creation

		private static DicomAttributeCollection CreateVolumeDataSet(IDicomAttributeProvider srcDataSet)
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

		#endregion

		#region Spacing and Orientation helpers

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

		// Derived from first frame's ImageOrienationPatient
		private Matrix OrientationPatient
		{
			get
			{
				if (_orientationPatient == null)
					// Use the first frame's orientation as our volume orientation
					_orientationPatient = ImageOrientationPatientToMatrix(_frames[0].ImageOrientationPatient);

				return _orientationPatient;
			}
		}

		private static Matrix ImageOrientationPatientToMatrix(ImageOrientationPatient orientation)
		{
			Vector3D xOrient = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
			Vector3D yOrient =
				new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY, (float) orientation.ColumnZ);
			Vector3D zOrient = xOrient.Cross(yOrient);

			Matrix orientationMatrix = VolumeHelper.OrientationMatrixFromVectors(xOrient, yOrient, zOrient);
			return orientationMatrix;
		}

		#endregion

		#region Build Volume Array stuff

		// Builds the volume array. Takes care of Gantry Tilt correction (pads rows at top/bottom accordingly)
		private T[] BuildVolumeArray<T>(T pixelPadValue)
		{
			T[] volumeArray = new T[_dimensions.Size];
			float lastFramePos = (float)_frames[_frames.Count - 1].ImagePositionPatient.Z;

			int imageIndex = 0;
			foreach (Frame frame in _frames)
			{
				// PadTop takes care of padding rows for gantry tilt correction
				//
				// This member keeps track of rows padded at top so that bottom padding can fill the void
				_paddedRowsAtTop = 0;
				int end = PadTop(volumeArray, frame, imageIndex, lastFramePos, pixelPadValue);

				// Copy frame data
				//
				byte[] frameData = frame.GetNormalizedPixelData();
				int start = end;
				end = start + frameData.Length / sizeof(short);

				// BlockCopy start and length are in src buffer type size (bytes)
				Buffer.BlockCopy(frameData, 0, volumeArray, start * sizeof(short), frameData.Length);
				imageIndex++;

				// Finish out any padding left over from PadTop
				PadBottom(volumeArray, end, pixelPadValue);

				if (_frameLoadedCallback != null)
					_frameLoadedCallback(frame);
			}
			return volumeArray;
		}

		// This indicates the number of rows each frame will be padded as it's added to the volume.
		// It is a function of the tilt angle and the run from first to last slice.
		private int GetNumberOfRowsToPad(double tilt)
		{
			double padRowsMm = Math.Tan(tilt) *
							   (_frames[_frames.Count - 1].ImagePositionPatient.Z - _frames[0].ImagePositionPatient.Z);
			// ensure this pad is always positive for sizing calculations
			return Math.Abs((int)(padRowsMm / _spacing.Y + 0.5f));
		}

		// This method takes care of any gantry tilt correction padding at the top of each frame. You always pad
		//	the same number of rows per frame, but as you go from frame to frame the padding shifts from
		//	Top to Bottom (or vice-versa). The return value indicates the index in the volume at which
		//	the real frame should begin to be copied.
		private int PadTop<T>(T[] volumeArray, Frame frame, int imageIndex, float lastFramePos, T pixelPaddingValue)
		{
			int padTopEnd;
			if (_padRows > 0)
			{
				float deltaMm = lastFramePos - (float)frame.ImagePositionPatient.Z;
				double padTopMm = Math.Tan(_tilt) * deltaMm;
				_paddedRowsAtTop = (int)(padTopMm / _spacing.Y + 0.5f);
				// This accounts for the tilt in negative radians, we start padding from
				//	the bottom first in that case
				if (_tilt < 0) _paddedRowsAtTop += _padRows;

				int start = imageIndex * _dimensions.Width * _dimensions.Height;
				padTopEnd = start + _paddedRowsAtTop * _dimensions.Width;
				for (int i = start; i < padTopEnd; i++)
					volumeArray[i] = pixelPaddingValue;
			}
			else
				padTopEnd = imageIndex * _dimensions.Width * _dimensions.Height;
			return padTopEnd;
		}

		// This method finishes out the padding for the bottom of the frame. It picks up where the frame
		//	ended and pads the remaining rows left over from PadTop
		private void PadBottom<T>(T[] volumeArray, int frameEnd, T pixelPaddingValue)
		{
			if (_padRows > 0) // Pad bottom
			{
				int padBottom = _padRows - _paddedRowsAtTop;
				int start = frameEnd;
				frameEnd = start + (padBottom * _dimensions.Width);
				for (int i = start; i < frameEnd; i++)
					volumeArray[i] = pixelPaddingValue;
			}
		}

		#endregion

		#region Pixel Pad Value stuff

		// Check DICOM header for indication of value to be used for padding. If not found, use the minimum
		//	pixel value for the volume.
		private void DeterminePixelPadValue(DicomMessageBase dicom)
		{
			DicomAttributeCollection dataSet = dicom.DataSet;
			// Note: If you add any additional checks for tags here, realize that the volume model dicom 
			//	is what's currently passed in and therefore the tags should be included in CreateVolumeDataSet
			if (dataSet.Contains(DicomTags.PixelPaddingValue))
			{
				if (_dataUnsigned)
					_pixelPadValue = (ushort)dataSet[DicomTags.PixelPaddingValue].GetInt32(0, 0);
				else
					_pixelPadValue = (short)dataSet[DicomTags.PixelPaddingValue].GetInt32(0, 0);
			}
			else if (dataSet.Contains(DicomTags.SmallestPixelValueInSeries))
			{
				if (_dataUnsigned)
					_pixelPadValue = (ushort)dataSet[DicomTags.SmallestPixelValueInSeries].GetInt32(0, 0);
				else
					_pixelPadValue = (short)dataSet[DicomTags.SmallestPixelValueInSeries].GetInt32(0, 0);
			}
			else if (dataSet.Contains(DicomTags.SmallestImagePixelValue))
			{
				if (_dataUnsigned)
					_pixelPadValue = (ushort)dataSet[DicomTags.SmallestImagePixelValue].GetInt32(0, 0);
				else
					_pixelPadValue = (short)dataSet[DicomTags.SmallestImagePixelValue].GetInt32(0, 0);
			}

			// Pragmatism won out Stewart. The cost of calculating this first and then copying to the
			//	volume is a minor increment, so it's probably not worth trying to combine the two ops.
			if (_pixelPadValue == null)
			{
				if (_dataUnsigned)
					_pixelPadValue = GetMinUnsignedShortPixelValue();
				else
					_pixelPadValue = GetMinShortPixelValue();
			}
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
				if (_frameLoadedCallback != null)
					_frameLoadedCallback(frame);
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
				if (_frameLoadedCallback != null)
					_frameLoadedCallback(frame);
			}
			return minValue;
		}

		#endregion

		#region Gantry Tilt helpers

		// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
		private double GetTiltAboutXTolerance(float withinTolerance)
		{
			double aboutXradians = GetRotateAboutXRadians(OrientationPatient);

			if (VolumeHelper.EqualsWithinTolerance(aboutXradians, 0d, withinTolerance) ||
				VolumeHelper.EqualsWithinTolerance(Math.Abs(aboutXradians), Math.PI / 2, withinTolerance))
				return 0f;

			return aboutXradians;
		}

		// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
		private double GetTiltAboutYTolerance(float withinTolerance)
		{
			double aboutYradians = GetRotateAboutYRadians(OrientationPatient);

			if (VolumeHelper.EqualsWithinTolerance(aboutYradians, 0d, withinTolerance) ||
				VolumeHelper.EqualsWithinTolerance(Math.Abs(aboutYradians), Math.PI / 2, withinTolerance))
				return 0f;

			return aboutYradians;
		}

		private bool CheckForTwoTilts()
		{
			return GetTiltAboutXTolerance(.1f) != 0 && GetTiltAboutYTolerance(.1f) != 0;
		}

		private double GetGantryTiltRadians()
		{
			double aboutXradians = GetTiltAboutXTolerance(0.1f);
			double aboutYradians = GetTiltAboutYTolerance(0.1f);

			if (aboutXradians != 0 && aboutYradians != 0)
				throw new Exception("Patient orientation is tilted about X and Y, not supported");

			double tilt = 0d;
			if (aboutXradians != 0)
				tilt = aboutXradians *
					   OrientationPatient[0, 0]; // This flips euler sign in prone position, so that tilt is correctly signed
			else if (aboutYradians != 0)
				tilt = aboutYradians *
					   OrientationPatient[0, 1]; // This flips euler sign in Decubitus Left position
			return tilt;
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

		#endregion

		#endregion
	}
}