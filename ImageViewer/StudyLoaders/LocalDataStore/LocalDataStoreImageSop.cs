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

	public class LocalDataStoreImageSop : LocalImageSop
	{
        private ImageSopInstance _dataStoreImageSopInstance;
        private ClearCanvas.Dicom.DataStore.Study _dataStoreStudy;
        private ClearCanvas.Dicom.DataStore.Series _dataStoreSeries;

		public LocalDataStoreImageSop(ImageSopInstance sop)
			: base(sop.LocationUri.LocalDiskPath)
		{
            _dataStoreImageSopInstance = sop;
            _dataStoreStudy = _dataStoreImageSopInstance.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Study;
            _dataStoreSeries = _dataStoreImageSopInstance.GetParentSeries() as ClearCanvas.Dicom.DataStore.Series;
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
            set { throw new Exception("The method or operation is not implemented.");; }
        }

        private ClearCanvas.Dicom.DataStore.Study DataStoreStudy
        {
            get { return _dataStoreStudy; }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        private ClearCanvas.Dicom.DataStore.Series DataStoreSeries
        {
            get { return _dataStoreSeries; }
            set { throw new Exception("The method or operation is not implemented."); }
        }

		public override PersonName PatientsName
		{
			get 
            {
                if (this.DataStoreStudy.PatientsName != null)
                    return this.DataStoreStudy.PatientsName;
                else
                    return new PersonName("");
            }
            set { throw new Exception("The method or operation is not implemented."); }
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

            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
		}

		public override string StudyTime
		{
            get 
            {
                if (this.DataStoreStudy.StudyTime != null)
                    return this.DataStoreStudy.StudyTime;
                else
					return "";
            }
            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
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

            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
		}

		public override int SeriesNumber
		{
            get 
            {
                return DataStoreSeries.SeriesNumber;
            }
            set { throw new Exception("The method or operation is not implemented."); }
		}

		public override string SeriesDescription
		{
			get
			{
				if (this.DataStoreSeries.SeriesDescription != null)
					return this.DataStoreSeries.SeriesDescription;
				else
					return "";
			}
			set { throw new Exception("The method or operation is not implemented."); }
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
            set { throw new Exception("The method or operation is not implemented."); }
		}


		public override int InstanceNumber
		{
			get
			{
    			return DataStoreImageSopInstance.InstanceNumber;
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
				if (this.DataStoreImageSopInstance.PixelSpacing != null)
					return this.DataStoreImageSopInstance.PixelSpacing;
				else
					return new PixelSpacing(-1.0, -1.0);
            }
            set { throw new Exception("The method or operation is not implemented."); }
       
		}

        public override PixelAspectRatio PixelAspectRatio
        {
            get
            {
                if (this.DataStoreImageSopInstance.PixelAspectRatio != null)
                    return this.DataStoreImageSopInstance.PixelAspectRatio;
                else
                    return new PixelAspectRatio(1.0, 1.0);
            }
            set { throw new Exception("The method or operation is not implemented."); }

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
            set { throw new Exception("The method or operation is not implemented."); }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
                return this.DataStoreImageSopInstance.PhotometricInterpretation;
			}
            set { throw new Exception("The method or operation is not implemented."); }
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
                return this.DataStoreImageSopInstance.RescaleIntercept;
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

		public override Window[]  WindowCenterAndWidth
		{
            get
            {
				if (this.DataStoreImageSopInstance.WindowValues == null || this.DataStoreImageSopInstance.WindowValues.Count == 0)
					return new Window[] { new Window(double.NaN, double.NaN) };

				List<Window> windowCentersAndWidths = new List<Window>();
				
				foreach(object existingWindow in this.DataStoreImageSopInstance.WindowValues)
					windowCentersAndWidths.Add(new Window((Window)existingWindow));
				
				return windowCentersAndWidths.ToArray();
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


	}
}
