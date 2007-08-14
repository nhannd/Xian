using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Codecs;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	public abstract class ImageSop : Sop
	{
		#region General Image Module

		/// <summary>
		/// Gets the instance number.
		/// </summary>
		public virtual int InstanceNumber
		{
			get
			{
				bool tagExists;
				int instanceNumber;
				GetTag(DicomTags.InstanceNumber, out instanceNumber, out tagExists);
				return instanceNumber;
			}
		}

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="PatientOrientation"/> is returned even when no data is available; 
		/// it will simply have values of "" for its <see cref="PatientOrientation.Row"/> and <see cref="PatientOrientation.Column"/> properties.
		/// </remarks>
		public virtual PatientOrientation PatientOrientation
		{
			get
			{
				bool tagExists;
				string patientOrientation;
				GetTagArray(DicomTags.PatientOrientation, out patientOrientation, out tagExists);
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
				GetTagArray(DicomTags.ImageType, out imageType, out tagExists);
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
				GetTag(DicomTags.ImagesinAcquisition, out imagesInAcquisition, out tagExists);
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
				GetTagArray(DicomTags.LossyImageCompressionRatio, out lossyImageCompressionRatios, out tagExists);
				
				double[] values;
				DicomStringHelper.TryGetDoubleArray(lossyImageCompressionRatios, out values);
				return values;
			}
		}

		/// <summary>
		/// Gets the presentation LUT shape.
		/// </summary>
		public virtual string PresentationLUTShape
		{
			get
			{
				bool tagExists;
				string presentationLUTShape;
				GetTag(DicomTags.PresentationLUTShape, out presentationLUTShape, out tagExists);
				return presentationLUTShape ?? "";
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
				GetTagArray(DicomTags.PixelSpacing, out pixelSpacing, out tagExists);
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
				GetTagArray(DicomTags.ImageOrientationPatient, out imageOrientationPatient, out tagExists);
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
				GetTagArray(DicomTags.ImagePositionPatient, out imagePositionPatient, out tagExists);
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
				GetTag(DicomTags.SamplesperPixel, out samplesPerPixel, out tagExists);
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
				GetTagArray(DicomTags.PixelAspectRatio, out pixelAspectRatio, out tagExists);
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
				GetTagArray(DicomTags.WindowCenter, out windowCenterValues, out tagExists);
				if (tagExists)
				{
					string windowWidthValues;
					GetTagArray(DicomTags.WindowWidth, out windowWidthValues, out tagExists);
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
				GetTagArray(DicomTags.WindowCenterWidthExplanation, out windowCenterAndWidthExplanations, out tagExists);
				return DicomStringHelper.GetStringArray(windowCenterAndWidthExplanations);
			}
		}

		#endregion

		public abstract byte[] GetNormalizedPixelData();

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
