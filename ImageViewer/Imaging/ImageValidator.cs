using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
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

			if ((image.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 
				|| image.PhotometricInterpretation == PhotometricInterpretation.Monochrome2) && 
				image.SamplesPerPixel != 1)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretationSamplesPerPixel, image.PhotometricInterpretation, image.SamplesPerPixel));
			}

            if (image.SamplesPerPixel != 1)
            {
                if (image.PlanarConfiguration == -1)
                    throw new ImageValidationException(String.Format(SR.ExceptionInvalidMissingPlanarConfiguration));
                else if (image.PlanarConfiguration != 0 && image.PlanarConfiguration != 1)
                    throw new ImageValidationException(String.Format(SR.ExceptionInvalidPlanarConfiguration));
            }
            
			if ((image.PhotometricInterpretation == PhotometricInterpretation.Rgb ||
				image.PhotometricInterpretation == PhotometricInterpretation.YbrFull ||
				image.PhotometricInterpretation == PhotometricInterpretation.YbrFull422 ||
				image.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422 ||
				image.PhotometricInterpretation == PhotometricInterpretation.YbrIct ||
				image.PhotometricInterpretation == PhotometricInterpretation.YbrRct) &&
				image.SamplesPerPixel != 3)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretationSamplesPerPixel, image.PhotometricInterpretation, image.SamplesPerPixel));
			}
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

		public static void ValidatePhotometricInterpretation(PhotometricInterpretation photometricInterpretation)
		{
			if (photometricInterpretation == PhotometricInterpretation.Unknown)
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
