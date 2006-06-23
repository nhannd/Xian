namespace ClearCanvas.Workstation.StudyLoaders.LocalDataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.OffisWrapper;
    using ClearCanvas.Workstation.Model;
    using ClearCanvas.Workstation.Model.StudyManagement;

	public class LocalDataStoreImageSop : ImageSop, IDicomPropertySettable
	{
		private FileDicomImage _dicomImage;

		public LocalDataStoreImageSop(string filename)
		{
			_dicomImage = new FileDicomImage(filename);
		}

		public override string PatientsName
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

		public override string PatientId
		{
			get
			{
                if (null != _patientId)
                    return _patientId;
                else
                {
                    bool tagExists;
                    _dicomImage.GetTag(Dcm.PatientId, out _patientId, out tagExists);
                    return _patientId;
                }
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
                if (null != _studyInstanceUid)
                    return _studyInstanceUid;
                else
                {
                    bool tagExists;
                    _dicomImage.GetTag(Dcm.StudyInstanceUID, out _studyInstanceUid, out tagExists);
                    return _studyInstanceUid;
                }
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
                if (null != _seriesInstanceUid)
                    return _seriesInstanceUid;
                else
                {
                    bool tagExists;
                    _dicomImage.GetTag(Dcm.SeriesInstanceUID, out _seriesInstanceUid, out tagExists);
                    return _seriesInstanceUid;
                }
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
				throw new Exception("The method or operation is not implemented.");
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
				throw new Exception("The method or operation is not implemented.");
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
                if (null != _pixelSpacingX)
                    return (double) _pixelSpacingX;
                else
                {
                    bool tagExists;
                    double pixelSpacingValueX;
                    _dicomImage.GetTag(Dcm.PixelSpacing, out pixelSpacingValueX, 0, out tagExists);

                    if (tagExists)
                    {
                        _pixelSpacingX = pixelSpacingValueX;
                        return (double)_pixelSpacingX;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
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
                if (null != _pixelSpacingY)
                    return (double) _pixelSpacingY;
                else
                {
                    bool tagExists;
                    double pixelSpacingY;
                    _dicomImage.GetTag(Dcm.PixelSpacing, out pixelSpacingY, 1, out tagExists);

                    if (tagExists)
                    {
                        _pixelSpacingY = pixelSpacingY;
                        return (double)_pixelSpacingY;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
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
                if (null != _samplesPerPixel)
                    return (int) _samplesPerPixel;
                else
                    return -1;
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
                if (null != _photometricInterpretation)
                    return _photometricInterpretation;
                else
                    return null;
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
                if (null != _rows)
                    return (int)_rows;
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
                if (null != _columns)
                    return (int)_columns;
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
                if (null != _bitsAllocated)
                    return (int)_bitsAllocated;
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
                if (null != _bitsStored)
                    return (int)_bitsStored;
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
                if (null != _highBit)
                    return (int)_highBit;
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
			get
			{
                if (null != _pixelRepresentation)
                    return (int)_pixelRepresentation;
                else
                    return -1;
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
                if (null != _planarConfiguration)
                    return (int)_planarConfiguration;
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
                if (null != _rescaleIntercept)
                    return (double) _rescaleIntercept;
                else
                {
                    bool tagExists;
                    double rescaleIntercept;
                    _dicomImage.GetTag(Dcm.RescaleIntercept, out rescaleIntercept, out tagExists);
                    if (tagExists)
                    {
                        _rescaleIntercept = rescaleIntercept;
                        return (double)_rescaleIntercept;
                    }
                    else
                        return 0.0;
                }
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
                if (null != _rescaleSlope)
                    return (double) _rescaleSlope;
                else
                {
                    bool tagExists;
                    double rescaleSlope;
                    _dicomImage.GetTag(Dcm.RescaleSlope, out rescaleSlope, out tagExists);
                    if (tagExists)
                    {
                        _rescaleSlope = rescaleSlope;
                        return (double)_rescaleSlope;
                    }
                    else
                        return 1.0;
                }
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
                if (null != _windowCenter)
                    return (double) _windowCenter;
                else
                {
                    bool tagExists;
                    double windowCenter;
                    _dicomImage.GetTag(Dcm.WindowCenter, out windowCenter, out tagExists);
                    if (tagExists)
                    {
                        _windowCenter = windowCenter;
                        return (double)_windowCenter;
                    }
                    else
                        return 0.0;
                }
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
                if (null != _windowWidth)
                    return (double) _windowWidth;
                else
                {
                    bool tagExists;
                    double windowWidth;
                    _dicomImage.GetTag(Dcm.WindowWidth, out windowWidth, out tagExists);
                    if (tagExists)
                    {
                        _windowWidth = windowWidth;
                        return (double)_windowWidth;
                    }
                    else
                        return 0.0;
                }
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
                if (null != _sopInstanceUid)
                    return _sopInstanceUid;
                else
                {
                    bool tagExists;
                    _dicomImage.GetTag(Dcm.SOPInstanceUID, out _sopInstanceUid, out tagExists);
                    return _sopInstanceUid;
                }
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
				if (null != _transferSyntaxUID)
					return _transferSyntaxUID;
				else
				{
					bool tagExists;
					_dicomImage.GetTag(Dcm.TransferSyntaxUID, out _transferSyntaxUID, out tagExists);
					return _transferSyntaxUID;
				}
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] GetPixelData()
		{
			return _dicomImage.GetPixelData(BitsAllocated, Rows, Columns, SamplesPerPixel);
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

        #region IDicomPropertySettable Members

        public void SetStringProperty(string propertyName, string value)
        {
            char[] vmSeparator = { '\\' };

            switch (propertyName)
            {
                case "PatientId":
                    _patientId = value;
                    break;
                case "StudyInstanceUid":
                    _studyInstanceUid = value;
                    break;
                case "SeriesInstanceUid":
                    _seriesInstanceUid = value;
                    break;
				case "TransferSyntaxUid":
					_transferSyntaxUID = value;
					break;
				case "PixelSpacing":
                    if ("" == value)
                    {
                        _pixelSpacingX = 0.0;
                        _pixelSpacingY = 0.0;
                    }
                    else
                    {
                        string[] vms = value.Split(vmSeparator);
                        _pixelSpacingY = Convert.ToDouble(vms[0]);  // assuming Y is Row spacing
                        _pixelSpacingX = Convert.ToDouble(vms[1]);  // assuming X is Column spacing;
                    }
                    break;
                case "SamplesPerPixel":
                    if ("" == value)
                        _samplesPerPixel = 1;
                    else
                        _samplesPerPixel = Convert.ToInt32(value);
                    break;
                case "PhotometricInterpretation":
                    _photometricInterpretation = value;
                    break;
                case "Rows":
                    if ("" == value)
                        _rows = 0;
                    else
                        _rows = Convert.ToInt32(value);
                    break;
                case "Columns":
                    if ("" == value)
                        _columns = 0;
                    else
                        _columns = Convert.ToInt32(value);
                    break;
                case "BitsAllocated":
                    if ("" == value)
                        _bitsAllocated = 0;
                    else
                        _bitsAllocated = Convert.ToInt32(value);
                    break;
                case "BitsStored":
                    if ("" == value)
                        _bitsStored = 0;
                    else
                        _bitsStored = Convert.ToInt32(value);
                    break;
                case "HighBit":
                    if ("" == value)
                        _highBit = 0;
                    else
                        _highBit = Convert.ToInt32(value);
                    break;
                case "PixelRepresentation":
                    if ("" == value)
                        _pixelRepresentation = 0;
                    else
                        _pixelRepresentation = Convert.ToInt32(value);
                    break;
                case "PlanarConfiguration":
                    if ("" == value)
                        _planarConfiguration = 0;
                    else 
                        _planarConfiguration = Convert.ToInt32(value);
                    break;
                case "RescaleIntercept":
                    if ("" == value)
                        _rescaleIntercept = 0;
                    else
                        _rescaleIntercept = Convert.ToDouble(value);
                    break;
                case "RescaleSlope":
                    if ("" == value)
                        _rescaleSlope = 0;
                    else
                        _rescaleSlope = Convert.ToDouble(value);
                    break;
                case "WindowCenter":
                    if ("" == value)
                        _windowCenter = 0;
                    else
                    {
                        string[] vms = value.Split(vmSeparator);
                        _windowCenter = Convert.ToDouble(vms[0]);
                    }
                    break;
                case "WindowWidth":
                    if ("" == value)
                        _windowWidth = 0;
                    else
                    {
                        string[] vms = value.Split(vmSeparator);
                        _windowWidth = Convert.ToDouble(vms[0]);
                    }
                    break;
                case "SopInstanceUid":
                    _sopInstanceUid = value;
                    break;
                default:
                    break;
            }
        }

        public void SetInt32Property(string propertyName, int value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetUInt32Property(string propertyName, uint value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetDoubleProperty(string propertyName, double value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

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
