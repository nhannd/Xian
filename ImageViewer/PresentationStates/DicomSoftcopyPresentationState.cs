#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// Base class for DICOM Softcopy Presentation State objects.
	/// </summary>
	[Cloneable]
	public abstract class DicomSoftcopyPresentationState
	{
		[CloneCopyReference]
		private readonly SopClass _psSopClass;

		[CloneIgnore]
		private readonly DicomFile _dicomFile;

		private bool _serialized;
		private int _psInstanceNum;
		private string _psInstanceUid;
		private string _psSeriesUid;
		private string _psLabel;

		/// <summary>
		/// Constructs a serialization-capable DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass"></param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass)
		{
			_psSopClass = psSopClass;
			_dicomFile = new DicomFile();

			_serialized = false;
			_psInstanceNum = 1;
			_psInstanceUid = DicomUid.GenerateUid().UID;
			_psSeriesUid = DicomUid.GenerateUid().UID;
			_psLabel = "FOR_PRESENTATION";
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass"></param>
		/// <param name="dicomFile"></param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomFile dicomFile)
		{
			if (dicomFile.MediaStorageSopClassUid != psSopClass.Uid)
			{
				string message = string.Format("Expected: {0}; Found: {1}", psSopClass, SopClass.GetSopClass(dicomFile.MediaStorageSopClassUid));
				throw new ArgumentException("The specified DICOM file is not of a compatible SOP Class. " + message, "dicomFile");
			}

			_psSopClass = psSopClass;
			_dicomFile = dicomFile;

			_serialized = true;
			_psInstanceNum = _dicomFile.DataSet[DicomTags.InstanceNumber].GetInt32(0, 0);
			_psInstanceUid = _dicomFile.DataSet[DicomTags.SopInstanceUid].ToString();
			_psSeriesUid = _dicomFile.DataSet[DicomTags.SeriesInstanceUid].ToString();
			_psLabel = _dicomFile.DataSet[DicomTags.ContentLabel].ToString();
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass"></param>
		/// <param name="dataSource"></param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomAttributeCollection dataSource)
			: this(psSopClass, new DicomFile("", CreateMetaInfo(dataSource), dataSource)) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="context"></param>
		protected DicomSoftcopyPresentationState(DicomSoftcopyPresentationState source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_dicomFile = new DicomFile("", source._dicomFile.MetaInfo.Copy(true), source._dicomFile.DataSet.Copy(true));
		}

		public SopClass PresentationSopClass
		{
			get { return _psSopClass; }
		}

		public string PresentationSopClassUid
		{
			get { return _psSopClass.Uid; }
		}

		public string PresentationSeriesUid
		{
			get { return _psSeriesUid; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException("This presentation state has already been serialized.");
				_psSeriesUid = value;
			}
		}

		public string PresentationInstanceUid
		{
			get { return _psInstanceUid; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException("This presentation state has already been serialized.");
				_psInstanceUid = value;
			}
		}

		public int PresentationInstanceNumber
		{
			get { return _psInstanceNum; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException("This presentation state has already been serialized.");
				_psInstanceNum = value;
			}
		}

		public string PresentationContentLabel
		{
			get { return _psLabel; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException("This presentation state has already been serialized.");
				_psLabel = value;
			}
		}

		public DicomFile DicomFile
		{
			get
			{
				if (!_serialized)
					throw new InvalidOperationException("This presentation state has not been serialized.");

				return _dicomFile;
			}
		}

		/// <summary>
		/// Gets the underlying DICOM data set.
		/// </summary>
		protected IDicomAttributeProvider DataSet
		{
			get { return _dicomFile.DataSet; }
		}

		public void Serialize(IPresentationImage image)
		{
			this.Serialize(new IPresentationImage[] {image});
		}

		public void Serialize(IEnumerable<IPresentationImage> images)
		{
			if (_serialized)
				throw new InvalidOperationException("This presentation state has already been serialized.");

			_serialized = true;

			GeneralEquipmentModuleIod generalEquipmentModule = new GeneralEquipmentModuleIod(this.DataSet);
			generalEquipmentModule.Manufacturer = "";

			GeneralSeriesModuleIod generalSeriesModule = new GeneralSeriesModuleIod(this.DataSet);
			generalSeriesModule.InitializeAttributes();
			generalSeriesModule.SeriesInstanceUid = this.PresentationSeriesUid;

			PresentationSeriesModuleIod presentationSeriesModule = new PresentationSeriesModuleIod(this.DataSet);
			presentationSeriesModule.InitializeAttributes();
			presentationSeriesModule.Modality = Modality.PR;

			SopCommonModuleIod sopCommonModule = new SopCommonModuleIod(this.DataSet);
			sopCommonModule.SopInstanceUid = this.PresentationInstanceUid;
			sopCommonModule.SopClassUid = this.PresentationSopClass.Uid;

			PresentationStateIdentificationModuleIod presentationStateIdentificationModule = new PresentationStateIdentificationModuleIod(this.DataSet);
			presentationStateIdentificationModule.InitializeAttributes();
			presentationStateIdentificationModule.ContentLabel = this.PresentationContentLabel;
			presentationStateIdentificationModule.InstanceNumber = this.PresentationInstanceNumber;
			presentationStateIdentificationModule.PresentationCreationDateTime = DateTime.Now;

			PerformSerialization(images);

			_dicomFile.MediaStorageSopClassUid = this.PresentationSopClassUid;
			_dicomFile.MediaStorageSopInstanceUid = this.PresentationInstanceUid;
		}

		public void Deserialize(IPresentationImage image)
		{
			this.Deserialize(new IPresentationImage[] {image});
		}

		public void Deserialize(IEnumerable<IPresentationImage> images)
		{
			if (!_serialized)
				throw new InvalidOperationException("This presentation state has not been serialized.");

			PerformDeserialization(images);
		}

		protected abstract void PerformSerialization(IEnumerable<IPresentationImage> images);
		protected abstract void PerformDeserialization(IEnumerable<IPresentationImage> images);

		#region Protected Helper Methods

		private static DicomAttributeCollection CreateMetaInfo(DicomAttributeCollection dataset)
		{
			DicomAttributeCollection metainfo = new DicomAttributeCollection();
			metainfo[DicomTags.MediaStorageSopClassUid].SetStringValue(dataset[DicomTags.SopClassUid].ToString());
			metainfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(dataset[DicomTags.SopInstanceUid].ToString());
			return metainfo;
		}

		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(ImageSop sop)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = sop.SopClassUID;
			imageReference.ReferencedSopInstanceUid = sop.SopInstanceUID;
			return imageReference;
		}

		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(Frame frame)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = frame.ParentImageSop.SopClassUID;
			imageReference.ReferencedSopInstanceUid = frame.SopInstanceUID;
			imageReference.ReferencedFrameNumber.SetInt32(0, frame.FrameNumber);
			return imageReference;
		}

		#endregion

		#region Static Helpers

		public static DicomSoftcopyPresentationState Create(IPresentationImage image)
		{
			if (image is DicomGrayscalePresentationImage)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				grayscaleSoftcopyPresentationState.Serialize(image);
				return grayscaleSoftcopyPresentationState;
			}
			else if (image is DicomColorPresentationImage)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				colorSoftcopyPresentationState.Serialize(image);
				return colorSoftcopyPresentationState;
			}
			else
			{
				throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
			}
		}

		public static IDictionary<IPresentationImage, DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images)
		{
			List<IPresentationImage> grayscaleImages = new List<IPresentationImage>();
			List<IPresentationImage> colorImages = new List<IPresentationImage>();

			foreach (IPresentationImage image in images)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					grayscaleImages.Add(image);
				}
				else if (image is DicomColorPresentationImage)
				{
					colorImages.Add(image);
				}
				else
				{
					throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
				}
			}

			Dictionary<IPresentationImage, DicomSoftcopyPresentationState> presentationStates = new Dictionary<IPresentationImage, DicomSoftcopyPresentationState>();
			if (grayscaleImages.Count > 0)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				grayscaleSoftcopyPresentationState.Serialize(grayscaleImages);
				foreach (IPresentationImage image in grayscaleImages)
				{
					presentationStates.Add(image, grayscaleSoftcopyPresentationState);
				}
			}
			if (colorImages.Count > 0)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				colorSoftcopyPresentationState.Serialize(colorImages);
				foreach (IPresentationImage image in colorImages)
				{
					presentationStates.Add(image, colorSoftcopyPresentationState);
				}
			}
			return presentationStates;
		}

		public static DicomSoftcopyPresentationState Load(DicomFile presentationState)
		{
			if (presentationState.MediaStorageSopClassUid == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomGrayscaleSoftcopyPresentationState(presentationState);
			}
			else if(presentationState.MediaStorageSopClassUid == DicomColorSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomColorSoftcopyPresentationState(presentationState);
			}
			else
			{
				throw new ArgumentException("DICOM presentation state deserialization is not supported for that SOP class.");
			}
		}

		public static IEnumerable<DicomSoftcopyPresentationState> Load(IEnumerable<DicomFile> presentationStates)
		{
			foreach (DicomFile presentationState in presentationStates)
			{
				if (presentationState.MediaStorageSopClassUid == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomGrayscaleSoftcopyPresentationState(presentationState);
				}
				else if (presentationState.MediaStorageSopClassUid == DicomColorSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomColorSoftcopyPresentationState(presentationState);
				}
			}
		}

		public static DicomSoftcopyPresentationState Load(IDicomAttributeProvider presentationState)
		{
			if (presentationState[DicomTags.SopClassUid].ToString() == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomGrayscaleSoftcopyPresentationState(presentationState);
			}
			else if (presentationState[DicomTags.SopClassUid].ToString() == DicomColorSoftcopyPresentationState.SopClass.Uid)
			{
				return new DicomColorSoftcopyPresentationState(presentationState);
			}
			else
			{
				throw new ArgumentException("DICOM presentation state deserialization is not supported for that SOP class.");
			}
		}

		public static IEnumerable<DicomSoftcopyPresentationState> Load(IEnumerable<IDicomAttributeProvider> presentationStates)
		{
			foreach (IDicomAttributeProvider presentationState in presentationStates)
			{
				if (presentationState[DicomTags.SopClassUid].ToString() == DicomGrayscaleSoftcopyPresentationState.SopClass.Uid)
				{
					yield return new DicomGrayscaleSoftcopyPresentationState(presentationState);
				} 
				else if (presentationState[DicomTags.SopClassUid].ToString() == DicomColorSoftcopyPresentationState.SopClass.Uid) 
				{
					yield return new DicomColorSoftcopyPresentationState(presentationState);
				}
			}
		}

		public static bool IsSupported(IPresentationImage image)
		{
			return (image is DicomGrayscalePresentationImage) || (image is DicomColorPresentationImage);
		}

		#endregion
	}
}