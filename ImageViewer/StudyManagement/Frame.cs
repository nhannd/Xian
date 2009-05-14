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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the DICOM concept of a frame.
	/// </summary>
	public partial class Frame : IDisposable
	{
		#region Private fields

		private readonly ImageSop _parentImageSop;
		private readonly int _frameNumber;
		
		private readonly object _syncLock = new object();
		private volatile NormalizedPixelSpacing _normalizedPixelSpacing;
		private volatile ImagePlaneHelper _imagePlaneHelper;

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
		/// Gets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUID
		{
			get { return ParentImageSop.StudyInstanceUID;  }
		}

		/// <summary>
		/// Gets the Series Instance UID.
		/// </summary>
		public string SeriesInstanceUID
		{
			get { return ParentImageSop.SeriesInstanceUID; }
		}

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public string SopInstanceUID
		{
			get { return ParentImageSop.SopInstanceUID; }
		}
		
		/// <summary>
		/// Gets the frame number.
		/// </summary>
		public int FrameNumber
		{
			get { return _frameNumber; }
		}

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="PatientOrientation"/> is returned even when no data is available; 
		/// it will simply have values of "" for its 
		/// <see cref="Dicom.Iod.PatientOrientation.Row"/> and 
		/// <see cref="Dicom.Iod.PatientOrientation.Column"/> properties.
		/// </remarks>
		public virtual PatientOrientation PatientOrientation
		{
			get
			{
				string patientOrientation;
				patientOrientation = _parentImageSop[DicomTags.PatientOrientation].ToString();
				if (!string.IsNullOrEmpty(patientOrientation))
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
				string imageType;
				imageType = _parentImageSop[DicomTags.ImageType].ToString();
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
				int acquisitionNumber;
				acquisitionNumber = _parentImageSop[DicomTags.AcquisitionNumber].GetInt32(0, 0);
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
				string acquisitionDate;
				acquisitionDate = _parentImageSop[DicomTags.AcquisitionDate].GetString(0, null);
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
				string acquisitionTime;
				acquisitionTime = _parentImageSop[DicomTags.AcquisitionTime].GetString(0, null);
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
				string acquisitionDateTime;
				acquisitionDateTime = _parentImageSop[DicomTags.AcquisitionDatetime].GetString(0, null);
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
				int imagesInAcquisition;
				imagesInAcquisition = _parentImageSop[DicomTags.ImagesInAcquisition].GetInt32(0, 0);
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
				string imageComments;
				imageComments = _parentImageSop[DicomTags.ImageComments].GetString(0, null);
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
				string lossyImageCompression;
				lossyImageCompression = _parentImageSop[DicomTags.LossyImageCompression].GetString(0, null);
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
				string lossyImageCompressionRatios;
				lossyImageCompressionRatios = _parentImageSop[DicomTags.LossyImageCompressionRatio].ToString();

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
				string pixelSpacing;
				pixelSpacing = _parentImageSop[DicomTags.PixelSpacing].ToString();
				if (!string.IsNullOrEmpty(pixelSpacing))
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
				string imageOrientationPatient;
				imageOrientationPatient = _parentImageSop[DicomTags.ImageOrientationPatient].ToString();
				if (!string.IsNullOrEmpty(imageOrientationPatient))
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
				string imagePositionPatient;
				imagePositionPatient = _parentImageSop[DicomTags.ImagePositionPatient].ToString();
				if (!string.IsNullOrEmpty(imagePositionPatient))
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
				double sliceThickness;
				sliceThickness = _parentImageSop[DicomTags.SliceThickness].GetFloat64(0, 0);
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
				double sliceLocation;
				sliceLocation = _parentImageSop[DicomTags.SliceLocation].GetFloat64(0, 0);
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
				string imagerPixelSpacing;
				imagerPixelSpacing = _parentImageSop[DicomTags.ImagerPixelSpacing].ToString();
				if (!string.IsNullOrEmpty(imagerPixelSpacing))
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
				ushort samplesPerPixel;
				samplesPerPixel = _parentImageSop[DicomTags.SamplesPerPixel].GetUInt16(0, 0);
				return samplesPerPixel;
			}
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				string photometricInterpretation;
				photometricInterpretation = _parentImageSop[DicomTags.PhotometricInterpretation].GetString(0, null);
				return PhotometricInterpretation.FromCodeString(photometricInterpretation);
			}
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public virtual int Rows
		{
			get
			{
				ushort rows;
				rows = _parentImageSop[DicomTags.Rows].GetUInt16(0, 0);
				return rows;
			}
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public virtual int Columns
		{
			get
			{
				ushort columns;
				columns = _parentImageSop[DicomTags.Columns].GetUInt16(0, 0);
				return columns;
			}
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		public virtual int BitsAllocated
		{
			get
			{
				ushort bitsAllocated;
				bitsAllocated = _parentImageSop[DicomTags.BitsAllocated].GetUInt16(0, 0);
				return bitsAllocated;
			}
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		public virtual int BitsStored
		{
			get
			{
				ushort bitsStored;
				bitsStored = _parentImageSop[DicomTags.BitsStored].GetUInt16(0, 0);
				return bitsStored;
			}
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		public virtual int HighBit
		{
			get
			{
				ushort highBit;
				highBit = _parentImageSop[DicomTags.HighBit].GetUInt16(0, 0);
				return highBit;
			}
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		public virtual int PixelRepresentation
		{
			get
			{
				ushort pixelRepresentation;
				pixelRepresentation = _parentImageSop[DicomTags.PixelRepresentation].GetUInt16(0, 0);
				return pixelRepresentation;
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
				ushort planarConfiguration;
				planarConfiguration = _parentImageSop[DicomTags.PlanarConfiguration].GetUInt16(0, 0);
				return planarConfiguration;
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
				string pixelAspectRatio;
				pixelAspectRatio = _parentImageSop[DicomTags.PixelAspectRatio].ToString();
				if (!string.IsNullOrEmpty(pixelAspectRatio))
				{
					int[] values;
					if (DicomStringHelper.TryGetIntArray(pixelAspectRatio, out values) && values.Length == 2)
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
				double rescaleIntercept;
				rescaleIntercept = _parentImageSop[DicomTags.RescaleIntercept].GetFloat64(0, 0);
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
				double rescaleSlope;
				rescaleSlope = _parentImageSop[DicomTags.RescaleSlope].GetFloat64(0, 0);
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
				string rescaleType;
				rescaleType = _parentImageSop[DicomTags.RescaleType].GetString(0, null);
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
				string windowCenterValues;
				windowCenterValues = _parentImageSop[DicomTags.WindowCenter].ToString();
				if (!string.IsNullOrEmpty(windowCenterValues))
				{
					string windowWidthValues;
					windowWidthValues = _parentImageSop[DicomTags.WindowWidth].ToString();
					if (!string.IsNullOrEmpty(windowWidthValues))
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
				string windowCenterAndWidthExplanations;
				windowCenterAndWidthExplanations = _parentImageSop[DicomTags.WindowCenterWidthExplanation].ToString();
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
				string frameOfReferenceUid;
				frameOfReferenceUid = _parentImageSop[DicomTags.FrameOfReferenceUid].GetString(0, null);
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
				double spacingBetweenSlices;
				spacingBetweenSlices = _parentImageSop[DicomTags.SpacingBetweenSlices].GetFloat64(0, 0);
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
			get { return PhotometricInterpretation.IsColor; }
		}

		/// <summary>
		/// Gets the <see cref="ImagePlaneHelper"/> for this <see cref="ImageSop"/>.
		/// </summary>
		public ImagePlaneHelper ImagePlaneHelper
		{
			get
			{
				if (_imagePlaneHelper == null)
				{
					lock (_syncLock)
					{
						if (_imagePlaneHelper == null)
							_imagePlaneHelper = new ImagePlaneHelper(this);
					}
				}

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
				{
					lock (_syncLock)
					{
						if (_normalizedPixelSpacing == null)
							_normalizedPixelSpacing = new NormalizedPixelSpacing(this);
					}
				}

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
		public byte[] GetNormalizedPixelData()
		{
			return _parentImageSop.DataSource.GetFrameData(FrameNumber).GetNormalizedPixelData();
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
			_parentImageSop.DataSource.GetFrameData(FrameNumber).Unload();
		}

		public IFrameReference CreateTransientReference()
		{
			return new FrameReference(this);
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
