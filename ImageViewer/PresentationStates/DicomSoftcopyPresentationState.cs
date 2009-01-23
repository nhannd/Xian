using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.PresentationStateRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	public abstract class DicomSoftcopyPresentationState
	{
		private readonly DicomAttributeCollection _dataSet;
		private readonly SopClass _psSopClass;
		private int _psInstanceNum = 1;
		private string _psSeriesUid;

		protected DicomSoftcopyPresentationState(SopClass psSopClass)
		{
			_psSopClass = psSopClass;
			_dataSet = new DicomAttributeCollection();
			InitializeDataset();
		}

		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomFile dicomFile)
		{
			if(dicomFile.MediaStorageSopClassUid != psSopClass.Uid)
			{
				string message = string.Format("Expected: {0}; Found: {1}", psSopClass, SopClass.GetSopClass(dicomFile.MediaStorageSopClassUid));
				throw new ArgumentException("The specified DICOM file is not of a compatible SOP Class. " + message, "dicomFile");
			}

			_psSopClass = psSopClass;
			_dataSet = dicomFile.DataSet;
		}

		#region Public Interface

		public SopClass PresentationSopClass
		{
			get { return _psSopClass; }
		}

		public string PresentationSeriesUid
		{
			get
			{
				if (string.IsNullOrEmpty(_psSeriesUid))
					_psSeriesUid = DicomUid.GenerateUid().UID;
				return _psSeriesUid;
			}
			set { _psSeriesUid = value; }
		}

		public int PresentationInstanceNumber
		{
			get { return _psInstanceNum; }
			set { _psInstanceNum = value; }
		}

		public void Apply(IPresentationImage image)
		{
			this.Deserialize(new IPresentationImage[] {image});
		}

		public void Apply(IEnumerable<IPresentationImage> images)
		{
			this.Deserialize(images);
		}

		public DicomFile Save()
		{
			DicomFile dcf = new DicomFile("", new DicomAttributeCollection(), _dataSet.Copy(true));
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(this.PresentationSeriesUid);
			dcf.DataSet[DicomTags.InstanceNumber].SetInt32(0, this.PresentationInstanceNumber);
			dcf.MediaStorageSopClassUid = dcf.DataSet[DicomTags.SopClassUid].ToString();
			dcf.MediaStorageSopInstanceUid = dcf.DataSet[DicomTags.SopInstanceUid].ToString();
			return dcf;
		}

		#endregion

		#region Public Static Helpers

		public static DicomSoftcopyPresentationState Create(IPresentationImage image)
		{
			if (image is DicomGrayscalePresentationImage)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				grayscaleSoftcopyPresentationState.Serialize(new IPresentationImage[] {image});
				return grayscaleSoftcopyPresentationState;
			}
			else if (image is DicomColorPresentationImage)
			{
				throw new NotImplementedException("DICOM presentation state serialization for color images has not yet been implemented.");
			}
			else
			{
				throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
			}
		}

		public static IEnumerable<DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images)
		{
			List<IPresentationImage> grayscaleImages = new List<IPresentationImage>();

			foreach (IPresentationImage image in images)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					grayscaleImages.Add(image);
				}
				else if (image is DicomColorPresentationImage)
				{
					throw new NotImplementedException("DICOM presentation state serialization for color images has not yet been implemented.");
				}
				else
				{
					throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
				}
			}

			List<DicomSoftcopyPresentationState> presentationStates = new List<DicomSoftcopyPresentationState>();

			if (grayscaleImages.Count > 0)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				grayscaleSoftcopyPresentationState.Serialize(grayscaleImages);
				presentationStates.Add(grayscaleSoftcopyPresentationState);
			}

			return presentationStates;
		}

		public static DicomSoftcopyPresentationState Load(DicomFile dicomFile)
		{
			if (dicomFile.MediaStorageSopClassUid == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomGrayscaleSoftcopyPresentationState(dicomFile);
				//} else if (image is DicomColorPresentationImage) {
				//    throw new NotImplementedException("DICOM presentation state deserialization for color images has not yet been implemented.");
			}
			else
			{
				throw new ArgumentException("DICOM presentation state deserialization is not supported for that SOP class.");
			}
		}

		public static IEnumerable<DicomSoftcopyPresentationState> Load(IEnumerable<DicomFile> dicomFiles)
		{
			List<DicomSoftcopyPresentationState> presentationStates = new List<DicomSoftcopyPresentationState>();

			foreach (DicomFile file in dicomFiles)
			{
				presentationStates.Add(Load(file));
			}

			return presentationStates;
		}

		#endregion

		#region Serialization - DataSet Initializer

		private void InitializeDataset()
		{
			GeneralEquipmentModuleIod generalEquipmentModule = new GeneralEquipmentModuleIod(this.DataSet);
			generalEquipmentModule.Manufacturer = "";

			GeneralSeriesModuleIod generalSeriesModule = new GeneralSeriesModuleIod(this.DataSet);
			generalSeriesModule.InitializeAttributes();
			generalSeriesModule.SeriesInstanceUid = this.PresentationSeriesUid;

			PresentationSeriesModuleIod presentationSeriesModule = new PresentationSeriesModuleIod(this.DataSet);
			presentationSeriesModule.InitializeAttributes();
			presentationSeriesModule.Modality = Modality.PR;

			SopCommonModuleIod sopCommonModule = new SopCommonModuleIod(this.DataSet);
			sopCommonModule.SopInstanceUid = DicomUid.GenerateUid().UID;
			sopCommonModule.SopClassUid = this.PresentationSopClass.Uid;

			PresentationStateIdentificationModuleIod presentationStateIdentificationModule = new PresentationStateIdentificationModuleIod(this.DataSet);
			presentationStateIdentificationModule.InitializeAttributes();
			presentationStateIdentificationModule.ContentLabel = "A";
			presentationStateIdentificationModule.InstanceNumber = 1;
			presentationStateIdentificationModule.PresentationCreationDateTime = DateTime.Now;
		}

		#endregion

		#region Protected Helper Methods

		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(ImageSop sop)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = sop.SopClassUID;
			imageReference.ReferencedSopInstanceUid = sop.SopInstanceUID;
			return imageReference;
		}

		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(Frame frame) {
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = frame.ParentImageSop.SopClassUID;
			imageReference.ReferencedSopInstanceUid = frame.SopInstanceUID;
			imageReference.ReferencedFrameNumber.SetInt32(0, frame.FrameNumber);
			return imageReference;
		}

		#endregion

		protected DicomAttributeCollection DataSet
		{
			get { return _dataSet; }
		}

		protected abstract void Serialize(IEnumerable<IPresentationImage> images);
		protected abstract void Deserialize(IEnumerable<IPresentationImage> images);
	}
}