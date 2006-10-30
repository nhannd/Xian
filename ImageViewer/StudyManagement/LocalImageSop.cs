using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class LocalImageSop : ImageSop
	{
		private FileDicomImage _dicomImage;

		public LocalImageSop(string filename)
		{
			_dicomImage = new FileDicomImage(filename);
		}

		protected LocalImageSop()
		{ 
		}

		public override PersonName PatientsName
		{
			get
			{
				bool tagExists;
				string patientsName;
				_dicomImage.GetTag(Dcm.PatientsName, out patientsName, out tagExists);
				if (patientsName == null)
					patientsName = "";
				return new PersonName(patientsName);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientId
		{
			get
			{
				bool tagExists;
				string patientId;
				_dicomImage.GetTag(Dcm.PatientId, out patientId, out tagExists);
				return patientId;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				bool tagExists;
				string patientsBirthDate;
				_dicomImage.GetTag(Dcm.PatientsBirthDate, out patientsBirthDate, out tagExists);
				return patientsBirthDate;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsSex
		{
			get
			{
				bool tagExists;
				string patientsSex;
				_dicomImage.GetTag(Dcm.PatientsSex, out patientsSex, out tagExists);
				return patientsSex;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyInstanceUID
		{
			get
			{
				bool tagExists;
				string studyInstanceUID;
                _dicomImage.GetTag(Dcm.StudyInstanceUID, out studyInstanceUID, out tagExists);
				return studyInstanceUID;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyDate
		{
			get
			{
				bool tagExists;
				string studyDate;
				_dicomImage.GetTag(Dcm.StudyDate, out studyDate, out tagExists);
				return studyDate;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyTime
		{
			get
			{
				bool tagExists;
				string studyTime;
				_dicomImage.GetTag(Dcm.StudyTime, out studyTime, out tagExists);
				return studyTime;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName ReferringPhysiciansName
		{
			get
			{
				bool tagExists;
				string referringPhysiciansName;
				_dicomImage.GetTag(Dcm.ReferringPhysiciansName, out referringPhysiciansName, out tagExists);
				if (referringPhysiciansName == null)
					referringPhysiciansName = "";
				return new PersonName(referringPhysiciansName);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AccessionNumber
		{
			get
			{
				bool tagExists;
				string accessionNumber;
				_dicomImage.GetTag(Dcm.AccessionNumber, out accessionNumber, out tagExists);
				return accessionNumber;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyDescription
		{
			get
			{
				bool tagExists;
				string studyDescription;
				_dicomImage.GetTag(Dcm.StudyDescription, out studyDescription, out tagExists);
				return studyDescription;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				bool tagExists;
				string nameOfPhysiciansReadingStudy;
				_dicomImage.GetTagArray(Dcm.NameOfPhysiciansReadingStudy, out nameOfPhysiciansReadingStudy, out tagExists);
				return VMStringConverter.ToPersonNameArray(nameOfPhysiciansReadingStudy);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
			{
				bool tagExists;
				string admittingDiagnosesDescription;
				_dicomImage.GetTagArray(Dcm.AdmittingDiagnosesDescription, out admittingDiagnosesDescription, out tagExists);
				return VMStringConverter.ToStringArray(admittingDiagnosesDescription);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsAge
		{
			get
			{
				bool tagExists;
				string patientsAge;
				_dicomImage.GetTag(Dcm.PatientsAge, out patientsAge, out tagExists);
				return patientsAge;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				bool tagExists;
				string additionalPatientsHistory;
				_dicomImage.GetTag(Dcm.AdditionalPatientHistory, out additionalPatientsHistory, out tagExists);
				return additionalPatientsHistory;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Modality
		{
			get
			{
				bool tagExists;
				string modality;
				_dicomImage.GetTag(Dcm.Modality, out modality, out tagExists);
				return modality;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				bool tagExists;
				string seriesInstanceUID;
                _dicomImage.GetTag(Dcm.SeriesInstanceUID, out seriesInstanceUID, out tagExists);
				return seriesInstanceUID;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int SeriesNumber
		{
			get
			{
				bool tagExists;
				int seriesNumber;
				_dicomImage.GetTag(Dcm.SeriesNumber, out seriesNumber, out tagExists);
				return seriesNumber;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesDescription
		{
			get
			{
				bool tagExists;
				string seriesDescription;
				_dicomImage.GetTag(Dcm.SeriesDescription, out seriesDescription, out tagExists);
				return seriesDescription;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}


		public override string Laterality
		{
			get
			{
				bool tagExists;
				string laterality;
				_dicomImage.GetTag(Dcm.Laterality, out laterality, out tagExists);
				return laterality;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesDate
		{
			get
			{
				bool tagExists;
				string seriesDate;
				_dicomImage.GetTag(Dcm.SeriesDate, out seriesDate, out tagExists);
				return seriesDate;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesTime
		{
			get
			{
				bool tagExists;
				string seriesTime;
				_dicomImage.GetTag(Dcm.SeriesTime, out seriesTime, out tagExists);
				return seriesTime;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				bool tagExists;
				string performingPhysiciansNames;
				_dicomImage.GetTagArray(Dcm.PerformingPhysiciansName, out performingPhysiciansNames, out tagExists);
				return VMStringConverter.ToPersonNameArray(performingPhysiciansNames);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] OperatorsName
		{
			get
			{
				bool tagExists;
				string operatorsNames;
				_dicomImage.GetTagArray(Dcm.OperatorsName, out operatorsNames, out tagExists);
				return VMStringConverter.ToPersonNameArray(operatorsNames);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				bool tagExists;
				string bodyPartExamined;
				_dicomImage.GetTag(Dcm.BodyPartExamined, out bodyPartExamined, out tagExists);
				return bodyPartExamined;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientPosition
		{
			get
			{
				bool tagExists;
				string patientPosition;
				_dicomImage.GetTag(Dcm.PatientPosition, out patientPosition, out tagExists);
				return patientPosition;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Manufacturer
		{
			get
			{
				bool tagExists;
				string manufacturer;
				_dicomImage.GetTag(Dcm.Manufacturer, out manufacturer, out tagExists);
				return manufacturer;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionName
		{
			get
			{
				bool tagExists;
				string institutionName;
				_dicomImage.GetTag(Dcm.InstitutionName, out institutionName, out tagExists);
				return institutionName;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StationName
		{
			get
			{
				bool tagExists;
				string stationName;
				_dicomImage.GetTag(Dcm.StationName, out stationName, out tagExists);
				return stationName;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				bool tagExists;
				string institutionalDepartmentName;
				_dicomImage.GetTag(Dcm.InstitutionalDepartmentName, out institutionalDepartmentName, out tagExists);
				return institutionalDepartmentName;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				bool tagExists;
				string manufacturersModelName;
				_dicomImage.GetTag(Dcm.ManufacturersModelName, out manufacturersModelName, out tagExists);
				return manufacturersModelName;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int InstanceNumber
		{
			get
			{
				bool tagExists;
				int instanceNumber;
				_dicomImage.GetTag(Dcm.InstanceNumber, out instanceNumber, out tagExists);
				return instanceNumber;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PatientOrientation PatientOrientation
		{
			get
			{
				bool tagExists;
				string patientOrientation;
				_dicomImage.GetTagArray(Dcm.PatientOrientation, out patientOrientation, out tagExists);
				if (!String.IsNullOrEmpty(patientOrientation))
				{
					string[] values = VMStringConverter.ToStringArray(patientOrientation);
					return new PatientOrientation(values[0], values[1]);
				}
				else
					return new PatientOrientation("", "");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		// Returns the entire ImageType tag, since most of the time we would just 
		// use the complete string array to compare against when filtering, etc.
		public override string ImageType
		{
			get
			{
				bool tagExists;
				string imageType;
				_dicomImage.GetTagArray(Dcm.ImageType, out imageType, out tagExists);
				return imageType;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int AcquisitionNumber
		{
			get
			{
				bool tagExists;
				int acquisitionNumber;
				_dicomImage.GetTag(Dcm.AcquisitionNumber, out acquisitionNumber, out tagExists);
				return acquisitionNumber;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDate
		{
			get
			{
				bool tagExists;
				string acquisitionDate;
				_dicomImage.GetTag(Dcm.AcquisitionDate, out acquisitionDate, out tagExists);
				return acquisitionDate;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionTime
		{
			get
			{
				bool tagExists;
				string acquisitionTime;
				_dicomImage.GetTag(Dcm.AcquisitionTime, out acquisitionTime, out tagExists);
				return acquisitionTime;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDateTime
		{
			get
			{
				bool tagExists;
				string acquisitionDateTime;
				_dicomImage.GetTag(Dcm.AcquisitionDatetime, out acquisitionDateTime, out tagExists);
				return acquisitionDateTime;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int ImagesInAcquisition
		{
			get
			{
				bool tagExists;
				int imagesInAcquisition;
				_dicomImage.GetTag(Dcm.ImagesInAcquisition, out imagesInAcquisition, out tagExists);
				return imagesInAcquisition;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageComments
		{
			get
			{
				bool tagExists;
				string imageComments;
				_dicomImage.GetTag(Dcm.ImageComments, out imageComments, out tagExists);
				return imageComments;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string LossyImageCompression
		{
			get
			{
				bool tagExists;
				string lossyImageCompression;
				_dicomImage.GetTag(Dcm.LossyImageCompression, out lossyImageCompression, out tagExists);
				return lossyImageCompression;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double[] LossyImageCompressionRatio
		{
			get
			{
				bool tagExists;
				string lossyImageCompressionRatios;
				_dicomImage.GetTagArray(Dcm.LossyImageCompressionRatio, out lossyImageCompressionRatios, out tagExists);
				return VMStringConverter.ToDoubleArray(lossyImageCompressionRatios);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PresentationLUTShape
		{
			get
			{
				bool tagExists;
				string presentationLUTShape;
				_dicomImage.GetTag(Dcm.PresentationLUTShape, out presentationLUTShape, out tagExists);
				return presentationLUTShape;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				bool tagExists;
				string pixelSpacing;
				_dicomImage.GetTagArray(Dcm.PixelSpacing, out pixelSpacing, out tagExists);
				if (tagExists)
				{
					double[] values = VMStringConverter.ToDoubleArray(pixelSpacing);
					return new PixelSpacing(values[0], values[1]);
				}

				//return an invalid value that is not null, in keeping with the pre-existing logic.
				return new PixelSpacing(-1, -1);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				bool tagExists;
				string imageOrientationPatient;
				_dicomImage.GetTagArray(Dcm.ImageOrientationPatient, out imageOrientationPatient, out tagExists);
				if (tagExists)
				{
					double[] values = VMStringConverter.ToDoubleArray(imageOrientationPatient);
					return new ImageOrientationPatient(values[0], values[1], values[2], values[3], values[4], values[5]);
				}

				return new ImageOrientationPatient(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				bool tagExists;
				string imagePositionPatient;
				_dicomImage.GetTagArray(Dcm.ImagePositionPatient, out imagePositionPatient, out tagExists);
				if (tagExists)
				{
					double[] values = VMStringConverter.ToDoubleArray(imagePositionPatient);
					return new ImagePositionPatient(values[0], values[1], values[2]);
				}

				return new ImagePositionPatient(double.NaN, double.NaN, double.NaN);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceThickness
		{
			get
			{
				bool tagExists;
				double sliceThickness;
				_dicomImage.GetTag(Dcm.SliceThickness, out sliceThickness, out tagExists);
				return sliceThickness;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceLocation
		{
			get
			{
				bool tagExists;
				double sliceLocation;
				_dicomImage.GetTag(Dcm.SliceLocation, out sliceLocation, out tagExists);
				return sliceLocation;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				bool tagExists;
				string pixelAspectRatio;
				_dicomImage.GetTagArray(Dcm.PixelAspectRatio, out pixelAspectRatio, out tagExists);
				if (tagExists)
				{
					double[] values = VMStringConverter.ToDoubleArray(pixelAspectRatio);
					return new PixelAspectRatio(values[0], values[1]);
				}

				return new PixelAspectRatio(1.0, 1.0);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return _dicomImage.SamplesPerPixel;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return _dicomImage.PhotometricInterpretation;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Rows
		{
			get
			{
				return _dicomImage.Rows;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Columns
		{
			get
			{
				return _dicomImage.Columns;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return _dicomImage.BitsAllocated;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int BitsStored
		{
			get
			{
				return _dicomImage.BitsStored;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int HighBit
		{
			get
			{
				return _dicomImage.HighBit;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return _dicomImage.PixelRepresentation;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int PlanarConfiguration
		{
			get
			{
				return _dicomImage.PlanarConfiguration;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				bool tagExists;
				double rescaleIntercept;
                _dicomImage.GetTag(Dcm.RescaleIntercept, out rescaleIntercept, out tagExists);
				return rescaleIntercept;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleSlope
		{
			get
			{
				bool tagExists;
				double rescaleSlope;
                _dicomImage.GetTag(Dcm.RescaleSlope, out rescaleSlope, out tagExists);
				return rescaleSlope;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string RescaleType
		{
			get
			{
				bool tagExists;
				string rescaleType;
				_dicomImage.GetTag(Dcm.RescaleType, out rescaleType, out tagExists);
				return rescaleType;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				bool tagExists;
				string windowCenterValues;
				_dicomImage.GetTagArray(Dcm.WindowCenter, out windowCenterValues, out tagExists);
				if (!tagExists)
					return new Window[] { new Window(double.NaN, double.NaN) };

				string windowWidthValues;
				_dicomImage.GetTagArray(Dcm.WindowWidth, out windowWidthValues, out tagExists);
				if (!tagExists)
					return new Window[] { new Window(double.NaN, double.NaN) };

				if (String.IsNullOrEmpty(windowCenterValues) || String.IsNullOrEmpty(windowWidthValues))
					return new Window[] { new Window(double.NaN, double.NaN) };

				List<Window> windows = new List<Window>();

				double[] windowCenters = VMStringConverter.ToDoubleArray(windowCenterValues);
				double[] windowWidths = VMStringConverter.ToDoubleArray(windowWidthValues);

				for (int i = 0; i < windowWidths.Length; ++i)
				{
					windows.Add(new Window(windowWidths[i], windowCenters[i]));
				}

				return windows.ToArray();

			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string[] WindowCenterAndWidthExplanation
		{
			get
			{
				bool tagExists;
				string windowCenterAndWidthExplanations;
				_dicomImage.GetTagArray(Dcm.WindowCenterWidthExplanation, out windowCenterAndWidthExplanations, out tagExists);
				return VMStringConverter.ToStringArray(windowCenterAndWidthExplanations);
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SopInstanceUID
		{
			get
			{
				bool tagExists;
				string sopInstanceUID;
                _dicomImage.GetTag(Dcm.SOPInstanceUID, out sopInstanceUID, out tagExists);
				return sopInstanceUID;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				bool tagExists;
				string transferSyntaxInstanceUID;
				_dicomImage.GetTag(Dcm.TransferSyntaxUID, out transferSyntaxInstanceUID, out tagExists);
				return transferSyntaxInstanceUID;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] GetPixelData()
		{
			return _dicomImage.GetPixelData();
		}

		public override void GetTag(DcmTagKey tag, out ushort val, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int val, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string val, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string val, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out val, position, out tagExists);
		}

		public override void GetTagArray(DcmTagKey tag, out string arrayValues, out bool tagExists)
		{
			_dicomImage.GetTagArray(tag, out arrayValues, out tagExists);
		}
	}
}
