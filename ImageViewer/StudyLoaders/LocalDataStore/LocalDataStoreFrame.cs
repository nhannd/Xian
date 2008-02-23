using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
	public class LocalDataStoreFrame : LocalFrame
	{
		private readonly ImageSopInstance _dataStoreImageSopInstance;
		private Dicom.DataStore.Study _dataStoreStudy;
		private Dicom.DataStore.Series _dataStoreSeries;

		public LocalDataStoreFrame(ImageSopInstance dbSop, LocalDataStoreImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{
			_dataStoreImageSopInstance = dbSop;
			_dataStoreStudy = _dataStoreImageSopInstance.GetParentSeries().GetParentStudy() as Dicom.DataStore.Study;
			_dataStoreSeries = _dataStoreImageSopInstance.GetParentSeries() as Dicom.DataStore.Series;
		}

		public override string FrameOfReferenceUid
		{
			get
			{
				return _dataStoreSeries.FrameOfReferenceUid ?? "";
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				return this._dataStoreImageSopInstance.PixelSpacing ?? new PixelSpacing(0, 0);
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return this._dataStoreImageSopInstance.PixelAspectRatio ?? new PixelAspectRatio(1, 1);
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return this._dataStoreImageSopInstance.ImageOrientationPatient ?? new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return this._dataStoreImageSopInstance.ImagePositionPatient ?? new ImagePositionPatient(0, 0, 0);
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return this._dataStoreImageSopInstance.SamplesPerPixel;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return this._dataStoreImageSopInstance.PhotometricInterpretation;
			}
		}

		public override int Rows
		{
			get
			{
				return this._dataStoreImageSopInstance.Rows;
			}
		}

		public override int Columns
		{
			get
			{
				return this._dataStoreImageSopInstance.Columns;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return this._dataStoreImageSopInstance.BitsAllocated;
			}
		}

		public override int BitsStored
		{
			get
			{
				return this._dataStoreImageSopInstance.BitsStored;
			}
		}

		public override int HighBit
		{
			get
			{
				return this._dataStoreImageSopInstance.HighBit;
			}
		}

		public override int PixelRepresentation
		{
			get { return this._dataStoreImageSopInstance.PixelRepresentation; }
		}

		public override int PlanarConfiguration
		{
			get
			{
				return this._dataStoreImageSopInstance.PlanarConfiguration;
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				return this._dataStoreImageSopInstance.RescaleIntercept;
			}
		}

		public override double RescaleSlope
		{
			get
			{
				if (this._dataStoreImageSopInstance.RescaleSlope != 0.0)
					return this._dataStoreImageSopInstance.RescaleSlope;

				return 1.0;
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				if (this._dataStoreImageSopInstance.WindowValues == null || this._dataStoreImageSopInstance.WindowValues.Count == 0)
					return new Window[] { };

				List<Window> windowCentersAndWidths = new List<Window>();

				foreach (object existingWindow in this._dataStoreImageSopInstance.WindowValues)
				{
					Window window = (Window)existingWindow;
					windowCentersAndWidths.Add(new Window(window.Width, window.Center));
				}

				return windowCentersAndWidths.ToArray();
			}
		}
	}
}
