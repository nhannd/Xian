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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.DicomServices;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the DICOM concept of a frame.
	/// </summary>
	public class Frame : IDisposable
	{
		#region Private fields

		private readonly ImageSop _parentImageSop;
		private readonly int _frameNumber;
		private NormalizedPixelSpacing _normalizedPixelSpacing;
		private ImagePlaneHelper _imagePlaneHelper;
		protected volatile byte[] _pixelData;
		protected readonly object _syncLock = new object();

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="Frame"/> with the
		/// specified parameters.
		/// </summary>
		/// <param name="parentImageSop"></param>
		/// <param name="frameNumber">The first frame is frame 1.</param>
		protected internal Frame(ImageSop parentImageSop, int frameNumber)
		{
			Platform.CheckForNullReference(parentImageSop, "parentImageSop");
			Platform.CheckPositive(frameNumber, "frameNumber");
			_parentImageSop = parentImageSop;
			_frameNumber = frameNumber;
		}

		/// <summary>
		/// Gets the parent <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ParentImageSop
		{
			get { return _parentImageSop; }
		}

		/// <summary>
		/// Gets the frame number.
		/// </summary>
		public int FrameNumber
		{
			get { return _frameNumber;}
		}

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="PatientOrientation"/> is returned even when no data is available; 
		/// it will simply have values of "" for its 
		/// <see cref="ClearCanvas.Dicom.PatientOrientation.Row"/> and 
		/// <see cref="ClearCanvas.Dicom.PatientOrientation.Column"/> properties.
		/// </remarks>
		public virtual PatientOrientation PatientOrientation
		{
			get
			{
				bool tagExists;
				string patientOrientation;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.PatientOrientation, out patientOrientation, out tagExists);
				if (tagExists)
				{
					string[] values = DicomStringHelper.GetStringArray(patientOrientation);
					if (values.Length == 2)
						return new PatientOrientation(values[0], values[1]);
				}

				return new PatientOrientation("", "");
			}
		}

		/// <summary>
		/// Gets the image type.  The entire Image Type value should be returned as a Dicom string array.
		/// </summary>
		public virtual string ImageType
		{
			get
			{
				bool tagExists;
				string imageType;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.ImageType, out imageType, out tagExists);
				return imageType ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		public virtual int AcquisitionNumber
		{
			get
			{
				bool tagExists;
				int acquisitionNumber;
				_parentImageSop.GetTag(DicomTags.AcquisitionNumber, out acquisitionNumber, out tagExists);
				return acquisitionNumber;
			}
		}

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		public virtual string AcquisitionDate
		{
			get
			{
				bool tagExists;
				string acquisitionDate;
				_parentImageSop.GetTag(DicomTags.AcquisitionDate, out acquisitionDate, out tagExists);
				return acquisitionDate ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		public virtual string AcquisitionTime
		{
			get
			{
				bool tagExists;
				string acquisitionTime;
				_parentImageSop.GetTag(DicomTags.AcquisitionTime, out acquisitionTime, out tagExists);
				return acquisitionTime ?? "";
			}
		}

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		public virtual string AcquisitionDateTime
		{
			get
			{
				bool tagExists;
				string acquisitionDateTime;
				_parentImageSop.GetTag(DicomTags.AcquisitionDatetime, out acquisitionDateTime, out tagExists);
				return acquisitionDateTime ?? "";
			}
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		public virtual int ImagesInAcquisition
		{
			get
			{
				bool tagExists;
				int imagesInAcquisition;
				_parentImageSop.GetTag(DicomTags.ImagesInAcquisition, out imagesInAcquisition, out tagExists);
				return imagesInAcquisition;
			}
		}

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		public virtual string ImageComments
		{
			get
			{
				bool tagExists;
				string imageComments;
				_parentImageSop.GetTag(DicomTags.ImageComments, out imageComments, out tagExists);
				return imageComments ?? "";
			}
		}

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		public virtual string LossyImageCompression
		{
			get
			{
				bool tagExists;
				string lossyImageCompression;
				_parentImageSop.GetTag(DicomTags.LossyImageCompression, out lossyImageCompression, out tagExists);
				return lossyImageCompression ?? "";
			}
		}

		/// <summary>
		/// Gets the lossy image compression ratio.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		public virtual double[] LossyImageCompressionRatio
		{
			get
			{
				bool tagExists;
				string lossyImageCompressionRatios;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.LossyImageCompressionRatio, out lossyImageCompressionRatios, out tagExists);

				double[] values;
				DicomStringHelper.TryGetDoubleArray(lossyImageCompressionRatios, out values);
				return values;
			}
		}

		#endregion

		#region Image Plane Module

		/// <summary>
		/// Gets the pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// in calculations that require the pixel spacing.
		/// </remarks>
		public virtual PixelSpacing PixelSpacing
		{
			get
			{
				bool tagExists;
				string pixelSpacing;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.PixelSpacing, out pixelSpacing, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(pixelSpacing, out values) && values.Length == 2)
						return new PixelSpacing(values[0], values[1]);
				}

				return new PixelSpacing(0, 0);
			}
		}

		/// <summary>
		/// Gets the image orientation patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="ImageOrientationPatient"/> object with zero for all its values when no data is available or the existing data is bad/incorrect.
		/// </remarks>
		public virtual ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				bool tagExists;
				string imageOrientationPatient;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.ImageOrientationPatient, out imageOrientationPatient, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imageOrientationPatient, out values) && values.Length == 6)
						return new ImageOrientationPatient(values[0], values[1], values[2], values[3], values[4], values[5]);
				}

				return new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the image position patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="ImagePositionPatient"/> object with zero for all its values when no data is available or the existing data is bad/incorrect.
		/// </remarks>
		public virtual ImagePositionPatient ImagePositionPatient
		{
			get
			{
				bool tagExists;
				string imagePositionPatient;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.ImagePositionPatient, out imagePositionPatient, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imagePositionPatient, out values) && values.Length == 3)
						return new ImagePositionPatient(values[0], values[1], values[2]);
				}

				return new ImagePositionPatient(0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the slice thickness.
		/// </summary>
		public virtual double SliceThickness
		{
			get
			{
				bool tagExists;
				double sliceThickness;
				_parentImageSop.GetTag(DicomTags.SliceThickness, out sliceThickness, out tagExists);
				return sliceThickness;
			}
		}

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		public virtual double SliceLocation
		{
			get
			{
				bool tagExists;
				double sliceLocation;
				_parentImageSop.GetTag(DicomTags.SliceLocation, out sliceLocation, out tagExists);
				return sliceLocation;
			}
		}

		#endregion

		#region X-ray Acquisition Module

		/// <summary>
		/// Gets the imager pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// in calculations that require the imager pixel spacing.
		/// </remarks>
		public virtual PixelSpacing ImagerPixelSpacing
		{
			get
			{
				bool tagExists;
				string imagerPixelSpacing;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.ImagerPixelSpacing, out imagerPixelSpacing, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(imagerPixelSpacing, out values) && values.Length == 2)
						return new PixelSpacing(values[0], values[1]);
				}

				return new PixelSpacing(0, 0);
			}
		}

		#endregion

		#region Image Pixel Module

		#region Type 1

		/// <summary>
		/// Gets the samples per pixel.
		/// </summary>
		public virtual int SamplesPerPixel
		{
			get
			{
				bool tagExists;
				ushort samplesPerPixel;
				_parentImageSop.GetTag(DicomTags.SamplesPerPixel, out samplesPerPixel, out tagExists);
				return (int)samplesPerPixel;
			}
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				bool tagExists;
				string photometricInterpretation;
				_parentImageSop.GetTag(DicomTags.PhotometricInterpretation, out photometricInterpretation, out tagExists);
				return PhotometricInterpretationHelper.FromString(photometricInterpretation);
			}
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public virtual int Rows
		{
			get
			{
				bool tagExists;
				ushort rows;
				_parentImageSop.GetTag(DicomTags.Rows, out rows, out tagExists);
				return (int)rows;
			}
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public virtual int Columns
		{
			get
			{
				bool tagExists;
				ushort columns;
				_parentImageSop.GetTag(DicomTags.Columns, out columns, out tagExists);
				return (int)columns;
			}
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		public virtual int BitsAllocated
		{
			get
			{
				bool tagExists;
				ushort bitsAllocated;
				_parentImageSop.GetTag(DicomTags.BitsAllocated, out bitsAllocated, out tagExists);
				return (int)bitsAllocated;
			}
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		public virtual int BitsStored
		{
			get
			{
				bool tagExists;
				ushort bitsStored;
				_parentImageSop.GetTag(DicomTags.BitsStored, out bitsStored, out tagExists);
				return (int)bitsStored;
			}
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		public virtual int HighBit
		{
			get
			{
				bool tagExists;
				ushort highBit;
				_parentImageSop.GetTag(DicomTags.HighBit, out highBit, out tagExists);
				return (int)highBit;
			}
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		public virtual int PixelRepresentation
		{
			get
			{
				bool tagExists;
				ushort pixelRepresentation;
				_parentImageSop.GetTag(DicomTags.PixelRepresentation, out pixelRepresentation, out tagExists);
				return (int)pixelRepresentation;
			}
		}

		#endregion
		#region Type 1C

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		public virtual int PlanarConfiguration
		{
			get
			{
				bool tagExists;
				ushort planarConfiguration;
				_parentImageSop.GetTag(DicomTags.PlanarConfiguration, out planarConfiguration, out tagExists);
				return (int)planarConfiguration;
			}
		}

		/// <summary>
		/// Gets the pixel aspect ratio.
		/// </summary>
		/// <remarks>
		/// A default value of 1/1 is returned if no data is available.
		/// </remarks>
		public virtual PixelAspectRatio PixelAspectRatio
		{
			get
			{
				bool tagExists;
				string pixelAspectRatio;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.PixelAspectRatio, out pixelAspectRatio, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(pixelAspectRatio, out values) && values.Length == 2)
						return new PixelAspectRatio(values[0], values[1]);
				}

				return new PixelAspectRatio(0, 0);
			}
		}

		#endregion
		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		public virtual double RescaleIntercept
		{
			get
			{
				bool tagExists;
				double rescaleIntercept;
				_parentImageSop.GetTag(DicomTags.RescaleIntercept, out rescaleIntercept, out tagExists);
				return rescaleIntercept;
			}
		}

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		/// <remarks>
		/// 1.0 is returned if no data is available.
		/// </remarks>
		public virtual double RescaleSlope
		{
			get
			{
				bool tagExists;
				double rescaleSlope;
				_parentImageSop.GetTag(DicomTags.RescaleSlope, out rescaleSlope, out tagExists);
				if (rescaleSlope == 0.0)
					return 1.0;

				return rescaleSlope;
			}
		}

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		public virtual string RescaleType
		{
			get
			{
				bool tagExists;
				string rescaleType;
				_parentImageSop.GetTag(DicomTags.RescaleType, out rescaleType, out tagExists);
				return rescaleType ?? "";
			}
		}

		#endregion

		#region VOI LUT Module

		/// <summary>
		/// Gets the window width and center.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		public virtual Window[] WindowCenterAndWidth
		{
			get
			{
				bool tagExists;
				string windowCenterValues;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.WindowCenter, out windowCenterValues, out tagExists);
				if (tagExists)
				{
					string windowWidthValues;
					_parentImageSop.GetMultiValuedTagRaw(DicomTags.WindowWidth, out windowWidthValues, out tagExists);
					if (tagExists)
					{
						if (!String.IsNullOrEmpty(windowCenterValues) && !String.IsNullOrEmpty(windowWidthValues))
						{
							List<Window> windows = new List<Window>();

							double[] windowCenters;
							double[] windowWidths;
							DicomStringHelper.TryGetDoubleArray(windowCenterValues, out windowCenters);
							DicomStringHelper.TryGetDoubleArray(windowWidthValues, out windowWidths);

							if (windowCenters.Length > 0 && windowCenters.Length == windowWidths.Length)
							{
								for (int i = 0; i < windowWidths.Length; ++i)
									windows.Add(new Window(windowWidths[i], windowCenters[i]));

								return windows.ToArray();
							}
						}
					}
				}

				return new Window[] { };
			}
		}

		/// <summary>
		/// Gets the window width and center explanation.
		/// </summary>
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get
			{
				bool tagExists;
				string windowCenterAndWidthExplanations;
				_parentImageSop.GetMultiValuedTagRaw(DicomTags.WindowCenterWidthExplanation, out windowCenterAndWidthExplanations, out tagExists);
				return DicomStringHelper.GetStringArray(windowCenterAndWidthExplanations);
			}
		}

		#endregion

		#region Frame of Reference Module

		/// <summary>
		/// Gets the frame of reference uid for the image.
		/// </summary>
		public virtual string FrameOfReferenceUid
		{
			get
			{
				bool tagExists;
				string frameOfReferenceUid;
				_parentImageSop.GetTag(DicomTags.FrameOfReferenceUid, out frameOfReferenceUid, out tagExists);
				return frameOfReferenceUid ?? "";
			}
		}

		#endregion

		#region MR Image Module

		/// <summary>
		/// Gets the spacing between the slices.
		/// </summary>
		public virtual double SpacingBetweenSlices
		{
			get
			{
				bool tagExists;
				double spacingBetweenSlices;
				_parentImageSop.GetTag(DicomTags.SpacingBetweenSlices, out spacingBetweenSlices, out tagExists);
				return spacingBetweenSlices;
			}
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether the image is colour.
		/// </summary>
		/// <returns>
		/// <b>true</b> if <see cref="PhotometricInterpretation"/> is anything other than
		/// MONOCHROME1 or MONOCHROME2.
		/// </returns>
		public bool IsColor
		{
			get
			{
				return this.PhotometricInterpretation != PhotometricInterpretation.Monochrome1 &&
				       this.PhotometricInterpretation != PhotometricInterpretation.Monochrome2;
			}
		}

		/// <summary>
		/// Gets the <see cref="ImagePlaneHelper"/> for this <see cref="ImageSop"/>.
		/// </summary>
		public ImagePlaneHelper ImagePlaneHelper
		{
			get
			{
				if (_imagePlaneHelper == null)
					_imagePlaneHelper = new ImagePlaneHelper(this);

				return _imagePlaneHelper;
			}
		}

		/// <summary>
		/// Gets the pixel spacing appropriate to the modality.
		/// </summary>
		/// <remarks>
		/// For projection based modalities (i.e. CR, DX and MG), Imager Pixel Spacing is
		/// returned as the pixel spacing.  For all other modalities, the standard
		/// Pixel Spacing is returned. Clients who require the pixel spacing should use this
		/// property as opposed to the raw DICOM pixel spacing property in <see cref="ImageSop.PixelSpacing"/>.
		/// </remarks>
		public NormalizedPixelSpacing NormalizedPixelSpacing
		{
			get
			{
				if (_normalizedPixelSpacing == null)
					_normalizedPixelSpacing = new NormalizedPixelSpacing(this);

				return _normalizedPixelSpacing;
			}
		}

		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data is unchanged.</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted
		/// into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// <para>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </para>
		/// <para>
		/// Pixel data is reloaded when this method is called after a 
		/// call to <see cref="UnloadPixelData"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="ToArgb"/>
		public virtual byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				lock (_syncLock)
				{
					if (_pixelData == null)
					{
						this.ParentImageSop.Load();

						DicomMessageBase message = this.ParentImageSop.NativeDicomObject;
						_pixelData = GetNormalizedPixelData(message);
					}
				}
			}

			return _pixelData;
		}

		protected byte[] GetNormalizedPixelData(DicomMessageBase message)
		{
			PhotometricInterpretation photometricInterpretation;
			byte[] rawPixelData;

			if (!message.TransferSyntax.Encapsulated)
			{
				DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(message);
				// DICOM library uses zero-based frame numbers
				rawPixelData = pixelData.GetFrame(this.FrameNumber - 1);
				photometricInterpretation = this.PhotometricInterpretation;
			}
			else if (DicomCodecRegistry.GetCodec(message.TransferSyntax) != null)
			{
				DicomCompressedPixelData pixelData = new DicomCompressedPixelData(message);
				string pi;
				rawPixelData = pixelData.GetFrame(this.FrameNumber - 1, out pi);
				photometricInterpretation = PhotometricInterpretationHelper.FromString(pi);
			}
			else
				throw new DicomCodecException("Unsupported transfer syntax");

			if (this.IsColor)
				rawPixelData = this.ToArgb(rawPixelData, photometricInterpretation);

			return rawPixelData;
		}

		protected byte[] GetNormalizedPixelData(byte[] compressedPixelData)
		{
			DicomMessageBase message = ParentImageSop.NativeDicomObject;
			PhotometricInterpretation photometricInterpretation;
			byte[] rawPixelData;

			if (!message.TransferSyntax.Encapsulated)
			{
				rawPixelData = compressedPixelData;
				photometricInterpretation = this.PhotometricInterpretation;
			}
			else if (DicomCodecRegistry.GetCodec(message.TransferSyntax) != null)
			{
				DicomCompressedPixelData pixelData = new DicomCompressedPixelData(message, compressedPixelData);
				string pi;
				rawPixelData = pixelData.GetFrame(this.FrameNumber - 1, out pi);
				photometricInterpretation = PhotometricInterpretationHelper.FromString(pi);
			}
			else
				throw new DicomCodecException("Unsupported transfer syntax");

			if (this.IsColor)
				rawPixelData = this.ToArgb(rawPixelData, photometricInterpretation);

			return rawPixelData;
		}
		
		/// <summary>
		/// Unloads the pixel data.
		/// </summary>
		/// <remarks>
		/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
		/// Calling this method will not necessarily result in an immediate decrease in memory
		/// usage, since it merely releases the reference to the pixel data; it is up to the
		/// garbage collector to free the memory.  Calling <see cref="GetNormalizedPixelData"/>
		/// will reload the pixel data.
		/// </remarks>
		public void UnloadPixelData()
		{
			lock (_syncLock)
			{
				_pixelData = null;
			}
		}

		/// <summary>
		/// Converts colour pixel data to ARGB.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Normally, this helper method would be called from (subclass) implementations of
		/// <see cref="GetNormalizedPixelData"/> the first time it is accessed.
		/// </para>
		/// </remarks>
		/// <seealso cref="GetNormalizedPixelData()"/>
		protected byte[] ToArgb(
			byte[] pixelData, 
			PhotometricInterpretation photometricInterpretation)
		{
			int sizeInBytes = this.Rows * this.Columns * 4;
			byte[] argbPixelData = new byte[sizeInBytes];

			// Convert palette colour images to ARGB so we don't get interpolation artifacts
			// when rendering.
			if (this.PhotometricInterpretation == PhotometricInterpretation.PaletteColor)
			{
				ColorSpaceConverter.ToArgb(
					this.BitsAllocated, 
					this.PixelRepresentation != 0 ? true : false,
					pixelData, 
					argbPixelData,
					CreateColorMap());
			}
			// Convert RGB and YBR variants to ARGB
			else
			{
				ColorSpaceConverter.ToArgb(
					photometricInterpretation,
					this.PlanarConfiguration,
					pixelData,
					argbPixelData);
			}

			return argbPixelData;
		}

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		internal void Validate()
		{
			DicomValidator.ValidateRows(this.Rows);
			DicomValidator.ValidateColumns(this.Columns);
			DicomValidator.ValidateBitsAllocated(this.BitsAllocated);
			DicomValidator.ValidateBitsStored(this.BitsStored);
			DicomValidator.ValidateHighBit(this.HighBit);
			DicomValidator.ValidateSamplesPerPixel(this.SamplesPerPixel);
			DicomValidator.ValidatePixelRepresentation(this.PixelRepresentation);
			DicomValidator.ValidatePhotometricInterpretation(this.PhotometricInterpretation);

			DicomValidator.ValidateImagePropertyRelationships
				(
					this.BitsStored, 
					this.BitsAllocated, 
					this.HighBit, 
					this.PhotometricInterpretation, 
					this.PlanarConfiguration, 
					this.SamplesPerPixel
				);
		}

		private IDataLut CreateColorMap()
		{
			bool tagExists;
			int lutSize, firstMappedPixel, bitsPerLutEntry;

			_parentImageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out lutSize, 0, out tagExists);

			if (!tagExists)
				throw new Exception("LUT Size missing.");

			_parentImageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out firstMappedPixel, 1, out tagExists);

			if (!tagExists)
				throw new Exception("First Mapped Pixel missing.");

			_parentImageSop.GetTag(DicomTags.RedPaletteColorLookupTableDescriptor, out bitsPerLutEntry, 2, out tagExists);

			if (!tagExists)
				throw new Exception("Bits Per Entry missing.");

			byte[] redLut, greenLut, blueLut;

			_parentImageSop.GetTag(DicomTags.RedPaletteColorLookupTableData, out redLut, out tagExists);

			if (!tagExists)
				throw new Exception("Red Palette Color LUT missing.");

			_parentImageSop.GetTag(DicomTags.GreenPaletteColorLookupTableData, out greenLut, out tagExists);

			if (!tagExists)
				throw new Exception("Green Palette Color LUT missing.");

			_parentImageSop.GetTag(DicomTags.BluePaletteColorLookupTableData, out blueLut, out tagExists);

			if (!tagExists)
				throw new Exception("Blue Palette Color LUT missing.");

			// The DICOM standard says that if the LUT size is 0, it means that it's 65536 in size.
			if (lutSize == 0)
				lutSize = 65536;

			return new PaletteColorMap(
				lutSize,
				firstMappedPixel,
				bitsPerLutEntry,
				redLut,
				greenLut,
				blueLut);
		}

		/// <summary>
		/// Inheritors should override this method to do additional cleanup.
		/// </summary>
		/// <remarks>
		/// Frames should never be disposed by client code; they are disposed by
		/// the parent <see cref="ImageSop"/>.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
