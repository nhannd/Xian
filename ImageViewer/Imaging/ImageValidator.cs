using System;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model.StudyManagement;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for ImageValidator.
	/// </summary>
	public class ImageValidator
	{
		public static void ValidateImage(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			ValidateSOPInstanceUID(image.SopInstanceUID);
			ValidateSeriesInstanceUID(image.SeriesInstanceUID);
			ValidateStudyInstanceUID(image.StudyInstanceUID);
			ValidatePatientID(image.PatientId);
			ValidateTransferSyntaxUID(image.TransferSyntaxUID);
			ValidateRows(image.Rows);
			ValidateColumns(image.Columns);
			ValidateBitsAllocated(image.BitsAllocated);
			ValidateBitsStored(image.BitsStored);
			ValidateHighBit(image.HighBit);
			ValidateSamplesPerPixel(image.SamplesPerPixel);
			ValidatePixelRepresentation(image.PixelRepresentation);
			ValidatePlanarConfiguration(image.PlanarConfiguration);
			ValidatePhotometricInterpretation(image.PhotometricInterpretation);

			ValidateImagePropertyRelationships(image);
		}

		public static void ValidateImagePropertyRelationships(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			if (image.BitsStored > image.BitsAllocated)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidBitsStoredBitsAllocated, image.BitsStored, image.BitsAllocated));

			if (image.HighBit > image.BitsAllocated - 1)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidHighBitBitsAllocated, image.HighBit, image.BitsAllocated));

			if ((String.Compare(image.PhotometricInterpretation, "MONOCHROME1") == 0 ||
				String.Compare(image.PhotometricInterpretation, "MONOCHROME2") == 0) &&
				image.SamplesPerPixel != 1)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretationSamplesPerPixel, image.PhotometricInterpretation, image.SamplesPerPixel));
			}

			if ((String.Compare(image.PhotometricInterpretation, "RGB") == 0 ||
				String.Compare(image.PhotometricInterpretation, "HSV") == 0 ||
				String.Compare(image.PhotometricInterpretation, "YBR_FULL") == 0 ||
				String.Compare(image.PhotometricInterpretation, "YBR_FULL_422") == 0 ||
				String.Compare(image.PhotometricInterpretation, "YBR_PARTIAL_422") == 0 ||
				String.Compare(image.PhotometricInterpretation, "YBR_ICT") == 0 ||
				String.Compare(image.PhotometricInterpretation, "YBR_RCT") == 0) &&
				image.SamplesPerPixel != 3)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretationSamplesPerPixel, image.PhotometricInterpretation, image.SamplesPerPixel));
			}

			if ((String.Compare(image.PhotometricInterpretation, "ARGB") == 0 ||
				String.Compare(image.PhotometricInterpretation, "CMYK") == 0) &&
				image.SamplesPerPixel != 4)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretationSamplesPerPixel, image.PhotometricInterpretation, image.SamplesPerPixel));
			}

			//int correctStride;

			//if (image.Columns % 4 != 0)
			//    correctStride = image.Columns / 4 * 4 + 4;
			//else
			//    correctStride = image.Columns;

			//if (image.Stride != correctStride)
			//    throw new ImageValidationException(String.Format(SR.ExceptionInvalidStride, correctStride, image.Stride));

			//int correctImageSizeInPixels = image.Rows * image.Columns;
			//int correctImageSizeInBytes =  correctImageSizeInPixels * image.BitsAllocated / 8;

			//if (image.SizeInBytes != correctImageSizeInBytes)
			//    throw new ImageValidationException(String.Format(SR.ExceptionInvalidSizeInBytes, correctImageSizeInBytes, image.SizeInBytes));

			// TODO: commented this out for now, since by accessing the pixel data, the dicom dataset object was being unloaded
			//if (image.GetPixelData().Length != correctImageSizeInBytes)
			//	throw new ImageValidationException(SR.ExceptionPixelData(image.GetPixelData().Length, correctImageSizeInBytes));
		}

		public static void ValidateRows(int rows)
		{
			if (rows <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidRows, rows));
		}

		public static void ValidateColumns(int columns)
		{
			if (columns <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidColumns, columns));
		}

		public static void ValidateBitsAllocated(int bitsAllocated)
		{
			if (bitsAllocated != 8 && 
				bitsAllocated != 16)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidBitsAllocated, bitsAllocated));
			}
		}

		public static void ValidateBitsStored(int bitsStored)
		{
			if (bitsStored <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidBitsStored, bitsStored));
		}

		public static void ValidateHighBit(int highBit)
		{
			if (highBit <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidHighBit, highBit));
		}

		public static void ValidateSamplesPerPixel(int samplesPerPixel)
		{
			if (samplesPerPixel != 1 && 
				samplesPerPixel != 3 && 
				samplesPerPixel !=4)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSamplesPerPixel, samplesPerPixel));
			}
		}

		public static void ValidatePixelRepresentation(int pixelRepresentation)
		{
			if (pixelRepresentation != 0 && pixelRepresentation != 1)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPixelRepresentation, pixelRepresentation));
		}

		public static void ValidatePlanarConfiguration(int planarConfiguration)
		{
			if (planarConfiguration != 0 && planarConfiguration != 1)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPlanarConfiguration, planarConfiguration));
		}

		public static void ValidatePhotometricInterpretation(string photometricInterpretation)
		{
			if (String.Compare(photometricInterpretation, "MONOCHROME1", true) != 0 &&
				String.Compare(photometricInterpretation, "MONOCHROME2", true) != 0 &&
				String.Compare(photometricInterpretation, "PALETTE COLOR", true) != 0 &&
				String.Compare(photometricInterpretation, "RGB", true) != 0 &&
				String.Compare(photometricInterpretation, "HSV", true) != 0 &&
				String.Compare(photometricInterpretation, "ARGB", true) != 0 &&
				String.Compare(photometricInterpretation, "CMYK", true) != 0 &&
				String.Compare(photometricInterpretation, "YBR_FULL", true) != 0 &&
				String.Compare(photometricInterpretation, "YBR_FULL_422", true) != 0 &&
				String.Compare(photometricInterpretation, "YBR_PARTIAL_422", true) != 0 &&
				String.Compare(photometricInterpretation, "YBR_ICT", true) != 0 &&
				String.Compare(photometricInterpretation, "YBR_RCT", true) != 0)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretation, photometricInterpretation));
			}
		}

		public static void ValidateTransferSyntaxUID(string uid)
		{
			if (uid != "1.2.840.10008.1.2" &&
				uid != "1.2.840.10008.1.2.1" &&
				uid != "1.2.840.10008.1.2.2")
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidTransferSyntaxUID));
		}

		public static void ValidatePatientID(string id)
		{
			if (id.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPatientID));
		}

		public static void ValidateStudyInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidStudyInstanceUID));
		}

		public static void ValidateSeriesInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSeriesInstanceUID));
		}
		public static void ValidateSOPInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSOPInstanceUID));
		}
	}
}
