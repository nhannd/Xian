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
using ClearCanvas.Codecs;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	public abstract class ImageSop : Sop
	{
		private static readonly object _syncLock = new object();
		private static volatile ImageCodecMap _imageCodecMap;

		#region General Image Module

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSop"/>.
		/// </summary>
		protected ImageSop()
		{
		}

		private static ImageCodecMap ImageCodecMap
		{
			get
			{
				if (_imageCodecMap == null)
				{
					lock(_syncLock)
					{
						if (_imageCodecMap == null)
							_imageCodecMap = new ImageCodecMap();
					}
				}

				return _imageCodecMap;
			}
		}

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
				GetTagAsDicomStringArray(DicomTags.PatientOrientation, out patientOrientation, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.ImageType, out imageType, out tagExists);
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
				GetTag(DicomTags.AcquisitionNumber, out acquisitionNumber, out tagExists);
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
				GetTag(DicomTags.AcquisitionDate, out acquisitionDate, out tagExists);
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
				GetTag(DicomTags.AcquisitionTime, out acquisitionTime, out tagExists);
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
				GetTag(DicomTags.AcquisitionDatetime, out acquisitionDateTime, out tagExists);
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
				GetTag(DicomTags.ImagesInAcquisition, out imagesInAcquisition, out tagExists);
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
				GetTag(DicomTags.ImageComments, out imageComments, out tagExists);
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
				GetTag(DicomTags.LossyImageCompression, out lossyImageCompression, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.LossyImageCompressionRatio, out lossyImageCompressionRatios, out tagExists);
				
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
		/// Returns a <see cref="PixelSpacing"/> object with zero for all its values when no data is available or the existing data is bad/incorrect.
		/// </remarks>
		public virtual PixelSpacing PixelSpacing
		{
			get
			{
				bool tagExists;
				string pixelSpacing;
				GetTagAsDicomStringArray(DicomTags.PixelSpacing, out pixelSpacing, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.ImageOrientationPatient, out imageOrientationPatient, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.ImagePositionPatient, out imagePositionPatient, out tagExists);
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
				GetTag(DicomTags.SliceThickness, out sliceThickness, out tagExists);
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
				GetTag(DicomTags.SliceLocation, out sliceLocation, out tagExists);
				return sliceLocation;
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
				GetTag(DicomTags.SamplesPerPixel, out samplesPerPixel, out tagExists);
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
				GetTag(DicomTags.PhotometricInterpretation, out photometricInterpretation, out tagExists);
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
				GetTag(DicomTags.Rows, out rows, out tagExists);
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
				GetTag(DicomTags.Columns, out columns, out tagExists);
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
				GetTag(DicomTags.BitsAllocated, out bitsAllocated, out tagExists);
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
				GetTag(DicomTags.BitsStored, out bitsStored, out tagExists);
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
				GetTag(DicomTags.HighBit, out highBit, out tagExists);
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
				GetTag(DicomTags.PixelRepresentation, out pixelRepresentation, out tagExists);
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
				GetTag(DicomTags.PlanarConfiguration, out planarConfiguration, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.PixelAspectRatio, out pixelAspectRatio, out tagExists);
				if (tagExists)
				{
					double[] values;
					if (DicomStringHelper.TryGetDoubleArray(pixelAspectRatio, out values) && values.Length == 2)
						return new PixelAspectRatio(values[0], values[1]);
				}

				return new PixelAspectRatio(1, 1);
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
				GetTag(DicomTags.RescaleIntercept, out rescaleIntercept, out tagExists);
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
				GetTag(DicomTags.RescaleSlope, out rescaleSlope, out tagExists);
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
				GetTag(DicomTags.RescaleType, out rescaleType, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.WindowCenter, out windowCenterValues, out tagExists);
				if (tagExists)
				{
					string windowWidthValues;
					GetTagAsDicomStringArray(DicomTags.WindowWidth, out windowWidthValues, out tagExists);
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
				GetTagAsDicomStringArray(DicomTags.WindowCenterWidthExplanation, out windowCenterAndWidthExplanations, out tagExists);
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
				GetTag(DicomTags.FrameOfReferenceUid, out frameOfReferenceUid, out tagExists);
				return frameOfReferenceUid ?? "";
			}
		}

		#endregion

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
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </remarks>
		/// <seealso cref="NormalizePixelData"/>
		/// <seealso cref="DecompressPixelData"/>
		public abstract byte[] GetNormalizedPixelData();

		#region Helper Methods

		/// <summary>
		/// Converts the input (possibly compressed) pixel data to a standard,
		/// 'normalized' format.
		///  </summary>
		/// <remarks>
		/// <para>
		/// Normally, this helper method would be called from (subclass) implementations of
		/// <see cref="GetNormalizedPixelData"/> the first time it is accessed.
		/// </para>
		/// <para>
		/// For grayscale images, this method will simply decompress the input pixel
		/// data and return the 'normalized' data.  For colour images, the input pixel data will be
		/// decompressed (if necessary) and converted to ARGB format before returning
		/// the 'normalized' data.
		/// </para>
		/// </remarks>
		/// <seealso cref="DecompressPixelData"/>
		/// <seealso cref="GetNormalizedPixelData"/>
		protected byte[] NormalizePixelData(byte[] compressedPixelData)
		{
			byte[] normalizedPixelData = DecompressPixelData(compressedPixelData);

			// If it's a colour image, we want to change the colour space to ARGB
			// so that it's easily consumed downstream
			if (this.PhotometricInterpretation != PhotometricInterpretation.Monochrome1 &&
			    this.PhotometricInterpretation != PhotometricInterpretation.Monochrome2)
			{
				int sizeInBytes = this.Rows * this.Columns * 4;
				byte[] newPixelData = new byte[sizeInBytes];

				ColorSpaceConverter.ToArgb(
					this.PhotometricInterpretation,
					this.PlanarConfiguration,
					normalizedPixelData,
					newPixelData);

				normalizedPixelData = newPixelData;
			}

			return normalizedPixelData;
		}

		/// <summary>
		/// Decompresses/Decodes pixel data, if necessary.
		/// </summary>
		/// <param name="compressedPixelData">The pixel data to decompress.</param>
		/// <remarks>
		/// <para>
		/// Normally, this method would not be called by subclasses; usually
		/// <see cref="NormalizePixelData"/> would be called from within
		/// <see cref="GetNormalizedPixelData"/> to convert the raw pixel
		/// data to a standard format for rendering/processing.
		/// </para>
		/// <para>
		/// Internally, this method uses <see cref="IImageCodec"/> extensions
		/// (<see cref="ImageCodecMap"/>) to perform the decompression.
		/// </para>
		/// </remarks>
		/// <seealso cref="NormalizePixelData"/>
		/// <seealso cref="GetNormalizedPixelData"/>
		protected byte[] DecompressPixelData(byte[] compressedPixelData)
		{
			return ImageCodecMap.DecompressPixelData(
				compressedPixelData,
				TransferSyntaxUID,
				Rows,
				Columns,
				BitsAllocated,
				BitsStored,
				PixelRepresentation,
				PhotometricInterpretationHelper.GetString(PhotometricInterpretation),
				SamplesPerPixel,
				PlanarConfiguration);
		}

		#endregion

		/// <summary>
		/// Gets the pixel spacing appropriate to the modality.
		/// </summary>
		/// <remarks>
		/// For projection based modalities (i.e. CR, DX and MG), Imager Pixel Spacing is
		/// returned as the pixel spacing.  For all other modalities, the standard
		/// Pixel Spacing is returned.
		/// </remarks>
		public PixelSpacing GetModalityPixelSpacing()
		{
			if (String.Compare(Modality, "CR", true) == 0 ||
				String.Compare(Modality, "DX", true) == 0 ||
				String.Compare(Modality, "MG", true) == 0)
			{
				double pixelSpacingRow = 0, pixelSpacingColumn = 0;

				bool tagExists;
				GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingRow, 0, out tagExists);
				if (tagExists)
					GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingColumn, 1, out tagExists);

				if (!tagExists)
				{
					pixelSpacingRow = pixelSpacingColumn = 0;
				}

				return new PixelSpacing(pixelSpacingRow, pixelSpacingColumn);
			}
			else
			{
				return this.PixelSpacing;
			}
		}

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected override void ValidateInternal()
		{
			base.ValidateInternal();

			ValidateAllowableTransferSyntax();

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

		private void ValidateAllowableTransferSyntax()
		{
			//Right now, Dicom Images are restricted to these transfer syntaxes for viewing purposes.
			if (this.TransferSyntaxUID != "1.2.840.10008.1.2" &&
				this.TransferSyntaxUID != "1.2.840.10008.1.2.1" &&
				this.TransferSyntaxUID != "1.2.840.10008.1.2.2")
				throw new SopValidationException(SR.ExceptionInvalidTransferSyntaxUID);
		}
	}
}
