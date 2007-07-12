using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Validates DICOM image data.
	/// </summary>
	public class ImageValidator
	{
		/// <summary>y
		/// Validates an <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="image"></param>
		/// <exception cref="ImageValidationException">
		/// The image failed validation.
		/// </exception>
		/// <remarks>
		/// <see cref="ImageValidator"/> will check that all the parameters required
		/// for the proper display of an image are within allowable values.
		/// </remarks>
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

		/// <summary>
		/// Validates image property relationships.
		/// </summary>
		/// <param name="image"></param>
		/// <remarks>
		/// An example of an image property relationship: the number of
		/// bits stored cannot exceed the number of bits allocated.
		/// </remarks>
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

		/// <summary>
		/// Validate that the number of rows > 0.
		/// </summary>
		/// <param name="rows"></param>
		public static void ValidateRows(int rows)
		{
			if (rows <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidRows, rows));
		}

		/// <summary>
		/// Validate that the number of columns > 0.
		/// </summary>
		/// <param name="columns"></param>
		public static void ValidateColumns(int columns)
		{
			if (columns <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidColumns, columns));
		}

		/// <summary>
		/// Validate that the number of bits allocated is either 8 or 16.
		/// </summary>
		/// <param name="bitsAllocated"></param>
		public static void ValidateBitsAllocated(int bitsAllocated)
		{
			if (bitsAllocated != 8 && 
				bitsAllocated != 16)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidBitsAllocated, bitsAllocated));
			}
		}

		/// <summary>
		/// Validate that the number of bits stored is >= 1.
		/// </summary>
		/// <param name="bitsStored"></param>
		public static void ValidateBitsStored(int bitsStored)
		{
			if (bitsStored <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidBitsStored, bitsStored));
		}

		/// <summary>
		/// Validates that the high bit is >= 1.
		/// </summary>
		/// <param name="highBit"></param>
		public static void ValidateHighBit(int highBit)
		{
			if (highBit <= 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidHighBit, highBit));
		}

		/// <summary>
		/// Validates that the number of samples per pixel is either 1 or 3.
		/// </summary>
		/// <param name="samplesPerPixel"></param>
		/// <remarks>
		/// ARGB and CMYK photometric interpretations have been retired by DICOM and
		/// so <see cref="SamplesPerPixel"/> can only be 1 or 3.
		/// </remarks>
		public static void ValidateSamplesPerPixel(int samplesPerPixel)
		{
			if (samplesPerPixel != 1 && 
				samplesPerPixel != 3)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSamplesPerPixel, samplesPerPixel));
			}
		}

		/// <summary>
		/// Validates that the pixel representation is either 0 or 1.
		/// </summary>
		/// <param name="pixelRepresentation"></param>
		public static void ValidatePixelRepresentation(int pixelRepresentation)
		{
			if (pixelRepresentation != 0 && pixelRepresentation != 1)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPixelRepresentation, pixelRepresentation));
		}

		/// <summary>
		/// Validates that the photometric interpretation is not unknown.
		/// </summary>
		/// <param name="photometricInterpretation"></param>
		public static void ValidatePhotometricInterpretation(PhotometricInterpretation photometricInterpretation)
		{
			if (photometricInterpretation == PhotometricInterpretation.Unknown)
			{
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPhotometricInterpretation, photometricInterpretation));
			}
		}

		/// <summary>
		/// Validates that the size of the pixel data byte buffer is equal
		/// to <i>rows</i> x <i>columns</i> x <i>bitsPerPixel</i> / 8.
		/// </summary>
		/// <param name="pixelData"></param>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsPerPixel">Can be 8 or 16 in the case of grayscale images,
		/// or 32 in the case of colour images.  <i>BitsPerPixel</i> is the product
		/// of DICOM's <i>Bits Allocated</i> and DICOM's <i>Samples Per Pixel</i></param>.
		public static void ValidatePixelData(byte[] pixelData, int rows, int columns, int bitsPerPixel)
		{
			int sizeInBytes = rows * columns * bitsPerPixel / 8;

			if (pixelData.Length != sizeInBytes)
				throw new ArgumentException(SR.ExceptionInvalidPixelData);
		}

		/// <summary>
		/// Validates the transfer syntax UID.
		/// </summary>
		/// <param name="uid"></param>
		/// <remarks>
		/// At this time, allowable UIDs are: 1.2.840.10008.1.2 (Implict VR Little Endian),
		/// 1.2.840.10008.1.2.1 (Explicit VR Little Endian) and 1.2.840.10008.1.2.2
		/// (Explicit VR Big Endian).
		/// </remarks>
		public static void ValidateTransferSyntaxUID(string uid)
		{
			if (uid != "1.2.840.10008.1.2" &&
				uid != "1.2.840.10008.1.2.1" &&
				uid != "1.2.840.10008.1.2.2")
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidTransferSyntaxUID));
		}

		/// <summary>
		/// Validate that the Patient ID is not empty.
		/// </summary>
		/// <param name="id"></param>
		public static void ValidatePatientID(string id)
		{
			if (id.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidPatientID));
		}

		/// <summary>
		/// Validate that the Study Instance UID is not empty.
		/// </summary>
		/// <param name="uid"></param>
		public static void ValidateStudyInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidStudyInstanceUID));
		}

		/// <summary>
		/// Validate that the Series Instance UID is not empty.
		/// </summary>
		/// <param name="uid"></param>
		public static void ValidateSeriesInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSeriesInstanceUID));
		}

		/// <summary>
		/// Validate that the SOP Instance UID is not empty.
		/// </summary>
		/// <param name="uid"></param>
		public static void ValidateSOPInstanceUID(string uid)
		{
			if (uid.TrimEnd(' ').Length == 0)
				throw new ImageValidationException(String.Format(SR.ExceptionInvalidSOPInstanceUID));
		}
	}
}
