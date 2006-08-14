using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{

	public class LocalDataStoreImageSop : ImageSop
	{
		private FileDicomImage _dicomImage;
        private ImageSopInstance _dataStoreImageSopInstance;
        private ClearCanvas.Dicom.DataStore.Study _dataStoreStudy;
        private ClearCanvas.Dicom.DataStore.Series _dataStoreSeries;
        static private Dictionary<ClearCanvas.Dicom.DataStore.PhotometricInterpretation, string> _photometricInterpretationMap;

        static LocalDataStoreImageSop()
        {
            _photometricInterpretationMap = new Dictionary<ClearCanvas.Dicom.DataStore.PhotometricInterpretation, string>();
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Argb, "ARGB");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Cmyk, "CMYK");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Hsv, "HSV");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Monochrome1, "MONOCHROME1");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Monochrome2, "MONOCHROME2");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.PaletteColor, "PALETTE_COLOR");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Rgb, "RGB");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrFull, "YBR_FULL");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrFull422, "YBR_FULL_422");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrIct, "YBR_ICT");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrPartial420, "YBR_PARTIAL_420");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrPartial422, "YBR_PARTIAL_422");
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.YbrRct, "YBR_RCT");
        }

        static private Dictionary<ClearCanvas.Dicom.DataStore.PhotometricInterpretation, string> PhotometricInterpretationMap
        {
            get { return LocalDataStoreImageSop._photometricInterpretationMap; }
        }

		public LocalDataStoreImageSop(ImageSopInstance sop)
		{
            _dataStoreImageSopInstance = sop;
            _dataStoreStudy = _dataStoreImageSopInstance.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Study;
            _dataStoreSeries = _dataStoreImageSopInstance.GetParentSeries() as ClearCanvas.Dicom.DataStore.Series;
			_dicomImage = new FileDicomImage(sop.LocationUri.LocalPath.Substring(12)); // remove the "\\localhost\" part
		}

        /// <summary>
        /// Disallow default constructor to be called.
        /// </summary>
        private LocalDataStoreImageSop()
        {
        }

        private ImageSopInstance DataStoreImageSopInstance
        {
            get { return _dataStoreImageSopInstance; }
            set { throw new Exception("This is not yet implemented."); }
        }

        private ClearCanvas.Dicom.DataStore.Study DataStoreStudy
        {
            get { return _dataStoreStudy; }
            set { throw new Exception("This is not yet implemented."); }
        }

        private ClearCanvas.Dicom.DataStore.Series DataStoreSeries
        {
            get { return _dataStoreSeries; }
            set { throw new Exception("This is not yet implemented."); }
        }

		public override string PatientsName
		{
			get { return this.DataStoreStudy.PatientsName; }
            set { throw new Exception("This is not yet implemented."); }
		}

        public override string PatientId
        {
            get { return this.DataStoreStudy.PatientId; }
            set { throw new Exception("This is not yet implemented."); }
        }

		public override string PatientsBirthDate
		{
            get { return this.DataStoreStudy.PatientsBirthDate; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string PatientsSex
		{
            get { return this.DataStoreStudy.PatientsSex; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyInstanceUID
		{
            get { return this.DataStoreStudy.StudyInstanceUid; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyDate
		{
            get { return this.DataStoreStudy.StudyDate; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyTime
		{
            get { return this.DataStoreStudy.StudyTime; }
            set { throw new Exception("This is not yet implemented."); }
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
            get { return this.DataStoreStudy.AccessionNumber; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyDescription
		{
            get { return this.DataStoreStudy.StudyDescription; }
            set { throw new Exception("This is not yet implemented."); }
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
            get { return this.DataStoreSeries.Modality; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string SeriesInstanceUID
		{
            get { return this.DataStoreSeries.SeriesInstanceUid; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string SeriesNumber
		{
            get { return Convert.ToString(this.DataStoreSeries.SeriesNumber); }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string Laterality
		{
            get { return this.DataStoreSeries.Laterality; }
            set { throw new Exception("This is not yet implemented."); }
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
				throw new Exception("The method or operation is not implemented.");
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
            get { return this.DataStoreImageSopInstance.PixelSpacing.Column; }
            set { throw new Exception("This is not yet implemented."); }
       
		}

		public override double PixelSpacingY
		{
            get { return this.DataStoreImageSopInstance.PixelSpacing.Row; }
            set { throw new Exception("This is not yet implemented."); }
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
            get { return this.DataStoreImageSopInstance.SamplesPerPixel; }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string PhotometricInterpretation
		{
			get
			{
                if (LocalDataStoreImageSop.PhotometricInterpretationMap.ContainsKey(this.DataStoreImageSopInstance.PhotometricInterpretation))
                    return LocalDataStoreImageSop.PhotometricInterpretationMap[this.DataStoreImageSopInstance.PhotometricInterpretation];
                else
                    return null;
			}
            set { throw new Exception("This is not yet implemented."); }
		}

		public override int Rows
		{
            get { return Convert.ToInt32(this.DataStoreImageSopInstance.Rows); }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Columns
		{
            get { return Convert.ToInt32(this.DataStoreImageSopInstance.Columns); }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int BitsAllocated
		{
            get { return this.DataStoreImageSopInstance.BitsAllocated; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int BitsStored
		{
            get { return this.DataStoreImageSopInstance.BitsStored; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int HighBit
		{
            get { return this.DataStoreImageSopInstance.HighBit; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int PixelRepresentation
		{
            get { return this.DataStoreImageSopInstance.PixelRepresentation; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int PlanarConfiguration
		{
            get { return this.DataStoreImageSopInstance.PlanarConfiguration; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleIntercept
		{
            get { return this.DataStoreImageSopInstance.RescaleIntercept; }
            set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleSlope
		{
            get { return this.DataStoreImageSopInstance.RescaleSlope; }
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
                if (this.DataStoreImageSopInstance.WindowValues != null)
                {
                    return (this.DataStoreImageSopInstance.WindowValues[0] as Window).Center;
                }
                else
                    return 0.0;
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
                if (this.DataStoreImageSopInstance.WindowValues != null)
                {
                    return (this.DataStoreImageSopInstance.WindowValues[0] as Window).Width;
                }
                else
                    return 0.0;
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
            get { return this.DataStoreImageSopInstance.SopInstanceUid; }
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string TransferSyntaxUID
		{
            get { return this.DataStoreImageSopInstance.TransferSyntaxUid; }
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] GetPixelData()
		{
			return _dicomImage.GetPixelData(
				this.Rows,
				this.Columns,
				this.BitsAllocated,
				this.BitsStored,
				this.PixelRepresentation,
				this.PhotometricInterpretation,
				this.SamplesPerPixel,
				this.PlanarConfiguration,
                this.TransferSyntaxUID);
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

		// MetaData
		private string _transferSyntaxUID;

        // Patient Module
		//private string _patientsName;
		private string _patientId;
		//private string _patientsBirthDate;
		//private string _patientsSex;

		//// General Study Module
		private string _studyInstanceUid;
		//private string _studyDate;
		//private string _studyTime;
		//private string _referringPhysiciansName;
		//private string _accessionNumber;
		//private string _studyDescription;
		//private string _nameOfPhysiciansReadyStudy;

		//// Patient Study Module
		//private string _admittingDiagnosesDescription;
		//private string _patientsAge;
		//private string _additionalPatientsHistory;

		//// General Series Module
		//private string _modality;
		private string _seriesInstanceUid;
		//private string _seriesNumber;
		//private string _laterality;
		//private string _seriesDate;
		//private string _seriesTime;
		//private string _performingPhysiciansName;
		//private string _operatorsName;
		//private string _bodyPartExamined;
		//private string _patientPosition;

		//// General Equipment Module
		//private string _manufacturer;
		//private string _institutionName;
		//private string _stationName;
		//private string _institutionalDepartmentName;
		//private string _manufacturersModelName;
    	
		//// General Image Module
		//private string _instanceNumber;
		//private string _patientOrientationRows;
		//private string _patientOrientationColumns;
		//private string _imageType;
		//private string _acquisitionNumber;
		//private string _acquisitionDate;
		//private string _acquisitionTime;
		//private string _acquisitionDateTime;
		//private string _imagesInAcquisition;
		//private string _imageComments;
		//private string _lossyImageCompression;
		//private string _lossyImageCompressionRatio;
		//private string _presentationLutShape;
        private string _sopInstanceUid;

	    // Image Plane Module
        private Nullable<double> _pixelSpacingX;
        private Nullable<double> _pixelSpacingY;
		//private Nullable<double> _imageOrientationPatientRowX;
		//private Nullable<double> _imageOrientationPatientRowY;
		//private Nullable<double> _imageOrientationPatientRowZ;
		//private Nullable<double> _imageOrientationPatientColumnX;
		//private Nullable<double> _imageOrientationPatientColumnY;
		//private Nullable<double> _imageOrientationPatientColumnZ;
		//private Nullable<double> _imagePositionPatientX;
		//private Nullable<double> _imagePositionPatientY;
		//private Nullable<double> _imagePositionPatientZ;
		//private Nullable<double> _sliceThickness;
		//private Nullable<double> _sliceLocation;
		//private Nullable<double> _pixelAspectRatioX;
		//private Nullable<double> _pixelAspectRatioY;

	    // Image Pixel Module
        private Nullable<int> _samplesPerPixel;
        private string _photometricInterpretation;
        private Nullable<int> _rows;
        private Nullable<int> _columns;
        private Nullable<int> _bitsAllocated;
        private Nullable<int> _bitsStored;
        private Nullable<int> _highBit;
        private Nullable<int> _pixelRepresentation;
        private Nullable<int> _planarConfiguration;

	    // Modality LUT Module
        private Nullable<double> _rescaleIntercept;
        private Nullable<double> _rescaleSlope;
        //private string _rescaleType;

	    // VOI LUT Module
        private Nullable<double> _windowCenter;
        private Nullable<double> _windowWidth;
        //private Nullable<double> _windowCenterAndWidthExplanation;

    }
}
