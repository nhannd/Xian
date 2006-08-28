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
            _photometricInterpretationMap.Add(ClearCanvas.Dicom.DataStore.PhotometricInterpretation.Unknown, "UNKNOWN");
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
			get 
            {
                if (this.DataStoreStudy.PatientsName != null)
                    return this.DataStoreStudy.PatientsName;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

        public override string PatientId
        {
            get 
            {
                if (this.DataStoreStudy.PatientId != null)
                    return this.DataStoreStudy.PatientId;
                else
                    return "";
            }

            set { throw new Exception("This is not yet implemented."); }
        }

		public override string PatientsBirthDate
		{
            get 
            {
                if (this.DataStoreStudy.PatientsBirthDate != null)
                    return this.DataStoreStudy.PatientsBirthDate;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string PatientsSex
		{
            get 
            {
                if (this.DataStoreStudy.PatientsSex != null)
                    return this.DataStoreStudy.PatientsSex;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyInstanceUID
		{
            get 
            {
                if (this.DataStoreStudy.StudyInstanceUid != null)
                    return this.DataStoreStudy.StudyInstanceUid;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyDate
		{
            get 
            {
                if (this.DataStoreStudy.StudyDate != null)
                    return this.DataStoreStudy.StudyDate;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyTime
		{
            get 
            {
                if (this.DataStoreStudy.StudyTime != null)
                    return this.DataStoreStudy.StudyTime;
                else return "";
            }
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
            get 
            {
                if (this.DataStoreStudy.AccessionNumber != null)
                    return this.DataStoreStudy.AccessionNumber;
                else 
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string StudyDescription
		{
            get 
            {
                if (this.DataStoreStudy.StudyDescription != null)
                    return this.DataStoreStudy.StudyDescription;
                else
                    return "";
            }

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
            get 
            {
                if (this.DataStoreSeries.Modality != null)
                    return this.DataStoreSeries.Modality;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string SeriesInstanceUID
		{
            get 
            {
                if (this.DataStoreSeries.SeriesInstanceUid != null)
                    return this.DataStoreSeries.SeriesInstanceUid;
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string SeriesNumber
		{
            get 
            {
                if (this.DataStoreSeries.SeriesNumber != 0)
                    return Convert.ToString(this.DataStoreSeries.SeriesNumber);
                else
                    return "";
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string Laterality
		{
            get 
            {
                if (this.DataStoreSeries.Laterality != null)
                    return this.DataStoreSeries.Laterality;
                else
                    return "";
            }
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
            get 
            {
                if (this.DataStoreImageSopInstance.PixelSpacing != null)
                    return this.DataStoreImageSopInstance.PixelSpacing.Column;
                else
                    return -1.0;
            }
            set { throw new Exception("This is not yet implemented."); }
       
		}

		public override double PixelSpacingY
		{
            get 
            {
                if (this.DataStoreImageSopInstance.PixelSpacing != null)
                    return this.DataStoreImageSopInstance.PixelSpacing.Row;
                else
                    return -1.0;
            }
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
            get 
            {
                if (this.DataStoreImageSopInstance.SamplesPerPixel != 0)
                    return this.DataStoreImageSopInstance.SamplesPerPixel;
                else
                    return -1;
            }
            set { throw new Exception("This is not yet implemented."); }
		}

		public override string PhotometricInterpretation
		{
			get
			{
                return LocalDataStoreImageSop.PhotometricInterpretationMap[this.DataStoreImageSopInstance.PhotometricInterpretation];
			}
            set { throw new Exception("This is not yet implemented."); }
		}

		public override int Rows
		{
            get 
            {
                if (this.DataStoreImageSopInstance.Rows != 0)
                    return Convert.ToInt32(this.DataStoreImageSopInstance.Rows);
                else
                    return -1;
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
                if (this.DataStoreImageSopInstance.Columns != 0)
                    return Convert.ToInt32(this.DataStoreImageSopInstance.Columns);
                else
                    return -1;
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
                if (this.DataStoreImageSopInstance.BitsAllocated != 0)
                    return this.DataStoreImageSopInstance.BitsAllocated;
                else
                    return -1;
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
                if (this.DataStoreImageSopInstance.BitsStored != 0)
                    return this.DataStoreImageSopInstance.BitsStored;
                else
                    return -1;
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
                if (this.DataStoreImageSopInstance.HighBit != 0)
                    return this.DataStoreImageSopInstance.HighBit;
                else
                    return -1;
            }
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
            get 
            {
                if (this.DataStoreImageSopInstance.PlanarConfiguration == 0 ||
                    this.DataStoreImageSopInstance.PlanarConfiguration == 1)
                    return this.DataStoreImageSopInstance.PlanarConfiguration;
                else
                    return -1;
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
                if (this.DataStoreImageSopInstance.RescaleIntercept != 0)
                    return this.DataStoreImageSopInstance.RescaleIntercept;
                else
                    return double.NaN;
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
                if (this.DataStoreImageSopInstance.RescaleSlope != 0.0)
                    return this.DataStoreImageSopInstance.RescaleSlope;
                else
                    return double.NaN;
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
                if (this.DataStoreImageSopInstance.WindowValues != null && this.DataStoreImageSopInstance.WindowValues.Count > 0)
                {
                    return (this.DataStoreImageSopInstance.WindowValues[0] as Window).Center;
                }
                else
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
                if (this.DataStoreImageSopInstance.WindowValues != null && this.DataStoreImageSopInstance.WindowValues.Count > 0)
                {
                    return (this.DataStoreImageSopInstance.WindowValues[0] as Window).Width;
                }
                else
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
                if (this.DataStoreImageSopInstance.SopInstanceUid != null)
                    return this.DataStoreImageSopInstance.SopInstanceUid;
                else
                    return "";
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
                if (this.DataStoreImageSopInstance.TransferSyntaxUid != null)
                    return this.DataStoreImageSopInstance.TransferSyntaxUid;
                else
                    return "";
            }
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
    }
}
