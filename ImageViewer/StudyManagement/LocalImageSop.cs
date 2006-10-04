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

		public override string PatientsName
		{
			get
			{
				bool tagExists;
				string patientsName;
				_dicomImage.GetTag(Dcm.PatientsName, out patientsName, out tagExists);
				return patientsName;
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

		public override string ReferringPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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

		public override string NameOfPhysiciansReadingStudy
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AdmittingDiagnosesDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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

		public override string SeriesNumber
		{
			get
			{
				bool tagExists;
				string seriesNumber;
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PerformingPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string OperatorsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstanceNumber
		{
			get
			{
				bool tagExists;
				string instanceNumber;
				_dicomImage.GetTag(Dcm.InstanceNumber, out instanceNumber, out tagExists);
				return instanceNumber;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientOrientationRows
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientOrientationColumns
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageType
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImagesInAcquisition
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string LossyImageCompressionRatio
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelSpacingX
		{
			get
			{
				bool tagExists;
				double pixelSpacingX;
                _dicomImage.GetTag(Dcm.PixelSpacing, out pixelSpacingX, 0, out tagExists);
				if (tagExists)
					return pixelSpacingX;
				return -1.0;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelSpacingY
		{
			get
			{
				bool tagExists;
				double pixelSpacingY;
                _dicomImage.GetTag(Dcm.PixelSpacing, out pixelSpacingY, 1, out tagExists);
				if (tagExists) 
					return pixelSpacingY;
				return -1.0;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientRowZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImageOrientationPatientColumnZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double ImagePositionPatientZ
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelAspectRatioX
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double PixelAspectRatioY
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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

		public override string PhotometricInterpretation
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
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double WindowCenter
		{
			get
			{
				bool tagExists;
				double windowCenter;
                _dicomImage.GetTag(Dcm.WindowCenter, out windowCenter, out tagExists);
				if (tagExists)
					return windowCenter;

				return double.NaN;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double WindowWidth
		{
			get
			{
				bool tagExists;
				double windowWidth;
                _dicomImage.GetTag(Dcm.WindowWidth, out windowWidth, out tagExists);
				if (tagExists)
					return windowWidth;
				
				return double.NaN;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double WindowCenterAndWidthExplanation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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
	}
}
