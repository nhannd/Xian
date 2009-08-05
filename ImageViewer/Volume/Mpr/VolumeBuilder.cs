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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	partial class Volume
	{
		/// <summary>
		/// Utility class for creating a <see cref="Volume"/> from a list of <see cref="Frame"/>s.
		/// </summary>
		/// <remarks>
		/// For internal use by the <see cref="Volume.CreateVolume(IDisplaySet)"/> static helpers.
		/// You MUST call Dispose() on this class after you're done to release the source SOP cache locks.
		/// </remarks>
		private class VolumeBuilder : IDisposable
		{
			// Future improvements list
			// - Deal with overlapping frames
			// - Support uneven spacing

			#region Private fields

			private const float HALF_PI = (float) Math.PI/2;
			private const float TILT_ABSOLUTE_TOLERANCE = 0.1f;

			private readonly List<IFrameReference> _frames;
			private readonly CreateVolumeProgressCallback _callback;

			private Matrix _imageOrientationPatient;
			private Vector3D _imagePositionPatient;
			private Vector3D _voxelSpacing;
			private Size3D _volumeSize;
			private double? _gantryTilt;
			private int? _pixelPaddingValue;
			private int? _paddingRows;

			#endregion

			public VolumeBuilder(IEnumerable<Frame> frames, CreateVolumeProgressCallback callback)
			{
				_callback = callback ?? delegate { };
				_frames = new List<IFrameReference>();
				foreach (Frame frame in frames)
					_frames.Add(frame.CreateTransientReference());
			}

			public void Dispose()
			{
				foreach (IFrameReference frame in _frames)
					frame.Dispose();
				_frames.Clear();
			}

			private Matrix ImageOrientationPatient
			{
				get
				{
					if (_imageOrientationPatient == null)
						_imageOrientationPatient = ImageOrientationPatientToMatrix(_frames[0].Frame.ImageOrientationPatient);
					return _imageOrientationPatient;
				}
			}

			private Vector3D ImagePositionPatient
			{
				get
				{
					if (_imagePositionPatient == null)
						_imagePositionPatient = ImagePositionPatientToVector(_frames[0].Frame.ImagePositionPatient);
					return _imagePositionPatient;
				}
			}

			private Vector3D VoxelSpacing
			{
				get
				{
					if (_voxelSpacing == null)
					{
						Frame frame0 = _frames[0].Frame;
						Frame frame1 = _frames[1].Frame;
						PixelSpacing pixelSpacing = frame0.PixelSpacing;
						_voxelSpacing = new Vector3D((float) pixelSpacing.Column, (float) pixelSpacing.Row, CalcSpaceBetweenPlanes(frame0, frame1));
					}
					return _voxelSpacing;
				}
			}

			private double GantryTilt
			{
				get
				{
					if (!_gantryTilt.HasValue)
					{
						double aboutXradians = this.GetTiltAboutXTolerance();
						double aboutYradians = this.GetTiltAboutYTolerance();

						if (aboutXradians != 0 && aboutYradians != 0)
							// should never happen, since the validation should have caught this already
							throw new Exception("Patient orientation is tilted about X and Y, not supported");

						if (aboutXradians != 0)
							// This flips euler sign in prone position, so that tilt is correctly signed
							_gantryTilt = aboutXradians*this.ImageOrientationPatient[0, 0];
						else if (aboutYradians != 0)
							// This flips euler sign in Decubitus Left position
							_gantryTilt = aboutYradians*this.ImageOrientationPatient[0, 1];
						else
							_gantryTilt = 0;
					}
					return _gantryTilt.Value;
				}
			}

			/// <summary>
			/// Gets the specified DICOM dataset for an indication of value to be used for padding, or computes one if necessary.
			/// </summary>
			private int PixelPaddingValue
			{
				get
				{
					if (!_pixelPaddingValue.HasValue)
					{
						DicomAttribute attribute = null;
						IDicomAttributeProvider attributeProvider = _frames[0].Sop.DataSource;
						bool isSigned = attributeProvider[DicomTags.PixelRepresentation].GetInt32(0, 0) != 0;

						if (attributeProvider.TryGetAttribute(DicomTags.PixelPaddingValue, out attribute)) {}
						else if (attributeProvider.TryGetAttribute(DicomTags.SmallestPixelValueInSeries, out attribute)) {}
						else if (attributeProvider.TryGetAttribute(DicomTags.SmallestImagePixelValue, out attribute)) {}

						if (attribute != null)
						{
							if (isSigned)
								_pixelPaddingValue = (short) attribute.GetInt32(0, 0);
							else
								_pixelPaddingValue = (ushort) attribute.GetInt32(0, 0);
						}
						else
						{
							// The cost of calculating this first and then copying to the volume is a minor increment.
							// Furthermore, this value is needed before the volume is generated by iterating frames...
							_pixelPaddingValue = ComputeMinPixelValue(_frames, isSigned);
						}
					}
					return _pixelPaddingValue.Value;
				}
			}

			private int PaddingRows
			{
				get
				{
					if (!_paddingRows.HasValue)
					{
						// If the series was obtained with a Gantry/Detector Tilt, we will pad the frames as they're
						//	 added to the volume so as to create a normalized cuboid volume that contains the tilted volume.
						// This is the number of rows that we will pad for each frame, and it affects the overall
						//	 dimensions of the volume.
						// It is a function of the tilt angle and the run from first to last slice.
						double padRowsMm = Math.Tan(this.GantryTilt)*(_frames[_frames.Count - 1].Frame.ImagePositionPatient.Z - _frames[0].Frame.ImagePositionPatient.Z);

						// ensure this pad is always positive for sizing calculations
						_paddingRows = Math.Abs((int) (padRowsMm/this.VoxelSpacing.Y + 0.5f));
					}
					return _paddingRows.Value;
				}
			}

			private Size3D VolumeSize
			{
				get
				{
					if (_volumeSize == null)
					{
						// Relying on the frames being uniform, so we'll base width/height off of first frame
						_volumeSize = new Size3D(_frames[0].Frame.Columns, _frames[0].Frame.Rows + this.PaddingRows, _frames.Count);
					}
					return _volumeSize;
				}
			}

			#region Builder

			/// <summary>
			/// Creates and populates a <see cref="Volume"/> from the builder's source frames.
			/// </summary>
			public Volume Build()
			{
				PrepareFrames(_frames); // this also sorts the frames into order by slice location

				// Construct a model SOP data source based on the first frame's DICOM header
				VolumeSopDataSourcePrototype sopDataSourcePrototype = VolumeSopDataSourcePrototype.Create(_frames[0].Sop.DataSource);

				if (_frames[0].Frame.PixelRepresentation == 0)
				{
					ushort[] volumeArray = BuildVolumeArray((ushort) this.PixelPaddingValue);

					Volume vol = new Volume(null, volumeArray, this.VolumeSize, this.VoxelSpacing, this.ImagePositionPatient, this.ImageOrientationPatient, sopDataSourcePrototype,
					                        this.PixelPaddingValue);
					return vol;
				}
				else
				{
					short[] volumeArray = BuildVolumeArray((short) this.PixelPaddingValue);

					Volume vol = new Volume(volumeArray, null, this.VolumeSize, this.VoxelSpacing, this.ImagePositionPatient, this.ImageOrientationPatient, sopDataSourcePrototype,
					                        this.PixelPaddingValue);
					return vol;
				}
			}

			// Builds the volume array. Takes care of Gantry Tilt correction (pads rows at top/bottom accordingly)
			private T[] BuildVolumeArray<T>(T pixelPadValue)
			{
				T[] volumeData = new T[this.VolumeSize.Volume];

				float lastFramePos = (float) _frames[_frames.Count - 1].Frame.ImagePositionPatient.Z;

				int position = 0;
				for (int n = 0; n < _frames.Count; n++)
				{
					position = CopyFrameData(_frames[n].Frame, volumeData, position, pixelPadValue, lastFramePos, this.VolumeSize, this.PaddingRows, this.GantryTilt, this.VoxelSpacing);
					_callback(n, _frames.Count);
				}

				return volumeData;
			}

			private static int CopyFrameData<T>(Frame sourceFrame, T[] volumeData, int position, T pixelPaddingValue, float lastFrameLocation, Size3D frameDimensions, int paddingRows, double gantryTilt, Vector3D pixelSpacing)
			{
				// PadTop takes care of padding rows for gantry tilt correction
				int countRowsPaddedAtTop = 0;
				if (paddingRows > 0)
				{
					// figure out how many rows need to be padded at the top
					float deltaMm = lastFrameLocation - (float) sourceFrame.ImagePositionPatient.Z;
					double padTopMm = Math.Tan(gantryTilt)*deltaMm;
					countRowsPaddedAtTop = (int) (padTopMm/pixelSpacing.Y + 0.5f);

					// account for the tilt in negative radians: we start padding from the bottom first in this case
					if (gantryTilt < 0)
						countRowsPaddedAtTop += paddingRows;

					int stop = position + countRowsPaddedAtTop*frameDimensions.Width;
					for (int i = position; i < stop; i++)
						volumeData[i] = pixelPaddingValue;
					position = stop;
				}

				// Copy frame data
				byte[] frameData = sourceFrame.GetNormalizedPixelData();
				Buffer.BlockCopy(frameData, 0, volumeData, position*sizeof (short), frameData.Length);
				position += frameData.Length/sizeof (short);

				// Finish out any padding left over from PadTop
				if (paddingRows > 0) // Pad bottom
				{
					int stop = position + ((paddingRows - countRowsPaddedAtTop)*frameDimensions.Width);
					for (int i = position; i < stop; i++)
						volumeData[i] = pixelPaddingValue;
					position = stop;
				}

				return position;
			}

			#endregion

			#region Misc

			private static unsafe int ComputeMinPixelValue(IEnumerable<IFrameReference> frames, bool isSigned)
			{
				int minValue = int.MaxValue;
				foreach (IFrameReference frame in frames)
				{
					byte[] frameData = frame.Frame.GetNormalizedPixelData();
					fixed (byte* frameDataBytes = frameData)
					{
						if (isSigned)
						{
							int count = frameData.Length/sizeof (short);
							short* frameDataShorts = (short*) frameDataBytes;
							for (int i = 0; i < count; i++)
								if (frameDataShorts[i] < minValue)
									minValue = frameDataShorts[i];
						}
						else
						{
							int count = frameData.Length/sizeof (ushort);
							ushort* frameDataUshorts = (ushort*) frameDataBytes;
							for (int i = 0; i < count; i++)
								if (frameDataUshorts[i] < minValue)
									minValue = frameDataUshorts[i];
						}
					}
				}
				return minValue;
			}

			private double GetTiltAboutXTolerance()
			{
				float aboutXradians = (float) GetXRotation(this.ImageOrientationPatient);

				// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
				if (FloatComparer.AreEqual(aboutXradians, 0f, TILT_ABSOLUTE_TOLERANCE) ||
				    FloatComparer.AreEqual(Math.Abs(aboutXradians), HALF_PI, TILT_ABSOLUTE_TOLERANCE))
					return 0f;

				return aboutXradians;
			}

			private double GetTiltAboutYTolerance()
			{
				float aboutYradians = (float) GetYRotation(this.ImageOrientationPatient);

				// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
				if (FloatComparer.AreEqual(aboutYradians, 0f, TILT_ABSOLUTE_TOLERANCE) ||
				    FloatComparer.AreEqual(Math.Abs(aboutYradians), HALF_PI, TILT_ABSOLUTE_TOLERANCE))
					return 0f;

				return aboutYradians;
			}

			#endregion

			#region Validation and Preparation Helper

			/// <summary>
			/// Validates and prepares the provided frames for the <see cref="VolumeBuilder"/>.
			/// </summary>
			/// <exception cref="CreateVolumeException">Thrown if something is wrong with the source frames.</exception>
			private static void PrepareFrames(List<IFrameReference> _frames)
			{
				// ensure we have at least 3 frames
				if (_frames.Count < 3)
					throw new CreateVolumeException("Source dataset must contain at least three frames.");

				// ensure all frames have are from the same series, and have the same frame of reference
				string studyInstanceUid = _frames[0].Frame.StudyInstanceUID;
				string seriesInstanceUid = _frames[0].Frame.SeriesInstanceUID;
				string frameOfReferenceUid = _frames[0].Frame.FrameOfReferenceUid;
				foreach (IFrameReference frame in _frames)
				{
					if (frame.Frame.StudyInstanceUID != studyInstanceUid)
						throw new CreateVolumeException("Each frame in the source dataset must be from the same study.");
					if (frame.Frame.SeriesInstanceUID != seriesInstanceUid)
						throw new CreateVolumeException("Each frame in the source dataset must be from the same series.");
					if (frame.Frame.FrameOfReferenceUid != frameOfReferenceUid)
						throw new CreateVolumeException("Each frame in the source dataset must have the same frame of reference.");
				}

				// ensure all frames have the same orientation
				ImageOrientationPatient orient = _frames[0].Frame.ImageOrientationPatient;
				foreach (IFrameReference frame in _frames)
				{
					if (frame.Frame.ImageOrientationPatient.IsNull)
						throw new CreateVolumeException("Each frame in the source dataset must have a defined image orientation (Patient).");
					if (!frame.Frame.ImageOrientationPatient.EqualsWithinTolerance(orient, .01f))
						throw new CreateVolumeException("Each frame in the source dataset must have the same image orientation (Patient).");
				}

				// ensure all frames are sorted by slice location
				SliceLocationComparer sliceLocationComparer = new SliceLocationComparer();
				_frames.Sort(delegate(IFrameReference x, IFrameReference y) { return sliceLocationComparer.Compare(x.Frame, y.Frame); });

				// ensure all frames are equally spaced
				float? nominalSpacing = null;
				for (int i = 1; i < _frames.Count; i++)
				{
					float currentSpacing = CalcSpaceBetweenPlanes(_frames[i].Frame, _frames[i - 1].Frame);
					if (currentSpacing < 0.01f)
						throw new CreateVolumeException("Two images are at same patient location, MPR requires evenly spaced images");
					if (!nominalSpacing.HasValue)
						nominalSpacing = currentSpacing;
					if (!FloatComparer.AreEqual(currentSpacing, nominalSpacing.Value))
						throw new CreateVolumeException("Inconsistent spacing betweeen images, MPR requires evenly spaced images");
				}

				// ensure frames are not tilted about multiple axes (the gantry correction algorithm only supports rotations about one)
				if (IsMultiAxialTilt(_frames[0].Frame.ImageOrientationPatient)) // suffices to check first one... they're all co-planar now!!
					throw new CreateVolumeException("Images are tilted along multiple axes");
			}

			#endregion

			#region Gantry Tilt Helpers

			private static bool IsMultiAxialTilt(ImageOrientationPatient imageOrientationPatient)
			{
				Matrix imageOrientationPatientMatrix = ImageOrientationPatientToMatrix(imageOrientationPatient);
				return !FloatComparer.AreEqual(0f, (float) GetXRotation(imageOrientationPatientMatrix), TILT_ABSOLUTE_TOLERANCE)
				       && !FloatComparer.AreEqual(0f, (float) GetYRotation(imageOrientationPatientMatrix), TILT_ABSOLUTE_TOLERANCE);
			}

			/// <summary>
			/// Gets the rotation about the X-axis in radians.
			/// </summary>
			private static double GetXRotation(Matrix orientationPatient)
			{
				return Math.Atan2(orientationPatient[2, 1], orientationPatient[2, 2]);
			}

			/// <summary>
			/// Gets the rotation about the Y-axis in radians.
			/// </summary>
			private static double GetYRotation(Matrix orientationPatient)
			{
				return -Math.Asin(orientationPatient[2, 0]);
			}

			/// <summary>
			/// Gets the rotation about the Z-axis in radians.
			/// </summary>
			private static double GetZRotation(Matrix orientationPatient)
			{
				return Math.Atan2(orientationPatient[1, 0], orientationPatient[0, 0]);
			}

			#endregion

			#region Spacing and Orientation Helpers

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

				Matrix orientationMatrix = Math3D.OrientationMatrixFromVectors(xOrient, yOrient, zOrient);
				return orientationMatrix;
			}

			private static Vector3D ImagePositionPatientToVector(ImagePositionPatient position)
			{
				return new Vector3D((float) position.X, (float) position.Y, (float) position.Z);
			}

			#endregion
		}
	}
}