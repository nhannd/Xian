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
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Base class for DICOM Softcopy Presentation State objects, as defined in DICOM PS 3.3 A.33.
	/// </summary>
	/// <remarks>
	/// <para>
	/// At this time, the only supported softcopy presentation states are the following:
	/// </para>
	/// <list type="table">
	/// <listheader><dicom>Reference</dicom><pstate><see cref="PresentationState">Presentation State</see> Class</pstate><pimage><see cref="IPresentationImage">Presentation Image</see> Class</pimage></listheader>
	/// <item><dicom>PS 3.3 A.33.1</dicom><pstate><see cref="DicomGrayscaleSoftcopyPresentationState">Grayscale Softcopy Presentation State</see></pstate><pimage><see cref="DicomGrayscalePresentationImage"/></pimage></item>
	/// <item><dicom>PS 3.3 A.33.2</dicom><pstate><see cref="DicomColorSoftcopyPresentationState">Color Softcopy Presentation State</see></pstate><pimage><see cref="DicomColorPresentationImage"/></pimage></item>
	/// </list>
	/// </remarks>
	[Cloneable]
	public abstract class DicomSoftcopyPresentationState : PresentationState
	{
		private const string _messageAlreadySerialized = "This presentation state has already been serialized to a file.";
		private const string _messageNotYetSerialized = "This presentation state has not been serialized to a file.";

		[CloneCopyReference]
		private readonly SopClass _presentationSopClass;

		[CloneIgnore]
		private readonly DicomFile _dicomFile;

		private bool _serialized;
		private int _presentationInstanceNumber;
		private string _presentationSopInstanceUid;
		private DateTime? _presentationSeriesDateTime;
		private int? _presentationSeriesNumber;
		private string _presentationSeriesInstanceUid;
		private string _presentationLabel;
		private string _sourceAETitle;
		private string _stationName;
		private Institution _institution;
		private string _manufacturer;
		private string _manufacturersModelName;
		private string _deviceSerialNumber;
		private string _softwareVersions;

		/// <summary>
		/// Constructs a serialization-capable DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="presentationStateSopClass">The SOP class of this type of softcopy presentation state.</param>
		protected DicomSoftcopyPresentationState(SopClass presentationStateSopClass)
		{
			_presentationSopClass = presentationStateSopClass;
			_dicomFile = new DicomFile();

			_serialized = false;
			_sourceAETitle = string.Empty;
			_stationName = string.Empty;
			_institution = Institution.Empty;
			_manufacturer = "ClearCanvas";
			_manufacturersModelName = Application.Name;
			_deviceSerialNumber = string.Empty;
			_softwareVersions = Application.Version.ToString();
			_presentationInstanceNumber = 1;
			_presentationSopInstanceUid = string.Empty;
			_presentationSeriesDateTime = DateTime.Now;
			_presentationSeriesNumber = null;
			_presentationSeriesInstanceUid = string.Empty;
			_presentationLabel = "FOR_PRESENTATION";
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass">The SOP class of this type of softcopy presentation state.</param>
		/// <param name="dicomFile">The presentation state file.</param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomFile dicomFile)
		{
			if (dicomFile.MediaStorageSopClassUid != psSopClass.Uid)
			{
				string message = string.Format("Expected: {0}; Found: {1}", psSopClass, SopClass.GetSopClass(dicomFile.MediaStorageSopClassUid));
				throw new ArgumentException("The specified DICOM file is not of a compatible SOP Class. " + message, "dicomFile");
			}

			_presentationSopClass = psSopClass;
			_dicomFile = dicomFile;

			_serialized = true;
			_sourceAETitle = _dicomFile.SourceApplicationEntityTitle;
			_stationName = _dicomFile.DataSet[DicomTags.StationName].ToString();
			_institution = Institution.GetInstitution(_dicomFile);
			_manufacturer = _dicomFile.DataSet[DicomTags.Manufacturer].ToString();
			_manufacturersModelName = _dicomFile.DataSet[DicomTags.ManufacturersModelName].ToString();
			_deviceSerialNumber = _dicomFile.DataSet[DicomTags.DeviceSerialNumber].ToString();
			_softwareVersions = _dicomFile.DataSet[DicomTags.SoftwareVersions].ToString();
			_presentationInstanceNumber = _dicomFile.DataSet[DicomTags.InstanceNumber].GetInt32(0, 0);
			_presentationSopInstanceUid = _dicomFile.DataSet[DicomTags.SopInstanceUid].ToString();
			_presentationSeriesDateTime = DateTimeParser.ParseDateAndTime(_dicomFile.DataSet, 0, DicomTags.SeriesDate, DicomTags.SeriesTime);
			_presentationSeriesNumber = GetNullableInt32(_dicomFile.DataSet[DicomTags.SeriesNumber], 0);
			_presentationSeriesInstanceUid = _dicomFile.DataSet[DicomTags.SeriesInstanceUid].ToString();
			_presentationLabel = _dicomFile.DataSet[DicomTags.ContentLabel].ToString();
		}

		/// <summary>
		/// Constructs a deserialization-only DICOM softcopy presentation state object.
		/// </summary>
		/// <param name="psSopClass">The SOP class of this type of softcopy presentation state.</param>
		/// <param name="dataSource">An attribute collection containing the presentation state.</param>
		protected DicomSoftcopyPresentationState(SopClass psSopClass, DicomAttributeCollection dataSource)
			: this(psSopClass, new DicomFile("", CreateMetaInfo(dataSource), dataSource)) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomSoftcopyPresentationState(DicomSoftcopyPresentationState source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_dicomFile = new DicomFile("", source._dicomFile.MetaInfo.Copy(), source._dicomFile.DataSet.Copy());
		}

		/// <summary>
		/// Gets the SOP class of this type of softcopy presentation state.
		/// </summary>
		public SopClass PresentationSopClass
		{
			get { return _presentationSopClass; }
		}

		/// <summary>
		/// Gets the SOP class UID of this type of softcopy presentation state.
		/// </summary>
		public string PresentationSopClassUid
		{
			get { return _presentationSopClass.Uid; }
		}

		/// <summary>
		/// Gets or sets the presentation state series instance UID.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationSeriesInstanceUid
		{
			get { return _presentationSeriesInstanceUid; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationSeriesInstanceUid = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state SOP instance UID.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationSopInstanceUid
		{
			get { return _presentationSopInstanceUid; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationSopInstanceUid = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state instance number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public int PresentationInstanceNumber
		{
			get { return _presentationInstanceNumber; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationInstanceNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state series number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public int? PresentationSeriesNumber
		{
			get { return _presentationSeriesNumber; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationSeriesNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state series date/time.
		/// </summary>
		/// <remarks>
		/// <para>This property affects only the SeriesDate and SeriesTime attributes. The PresentationCreationDateTime is always the timestamp from when the call to <see cref="Serialize"/> is made.</para>
		/// <para>This property may only be set if the presentation state has not yet been serialized to a file.</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public DateTime? PresentationSeriesDateTime
		{
			get { return _presentationSeriesDateTime; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationSeriesDateTime = value;
			}
		}

		/// <summary>
		/// Gets or sets the presentation state content label.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string PresentationContentLabel
		{
			get { return _presentationLabel; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_presentationLabel = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's workstation AE title.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string SourceAETitle
		{
			get { return _sourceAETitle; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_sourceAETitle = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's workstation name.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string StationName
		{
			get { return _stationName; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_stationName = value;
			}
		}

		/// <summary>
		/// Gets or sets the instance creator's institution.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		internal Institution Institution
		{
			get { return _institution; }
			set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_institution = value;
			}
		}

		/// <summary>
		/// Gets or sets the workstation's manufacturer.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string Manufacturer
		{
			get { return _manufacturer; }
			protected set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_manufacturer = value;
			}
		}

		/// <summary>
		/// Gets or sets the workstation's model name.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string ManufacturersModelName
		{
			get { return _manufacturersModelName; }
			protected set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_manufacturersModelName = value;
			}
		}

		/// <summary>
		/// Gets or sets the workstation's serial number.
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string DeviceSerialNumber
		{
			get { return _deviceSerialNumber; }
			protected set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_deviceSerialNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the workstation's software version numbers (backslash-delimited for multiple values).
		/// </summary>
		/// <remarks>
		/// This property may only be set if the presentation state has not yet been serialized to a file.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public string SoftwareVersions
		{
			get { return _softwareVersions; }
			protected set
			{
				if (_serialized)
					throw new InvalidOperationException(_messageAlreadySerialized);
				_softwareVersions = value;
			}
		}

		/// <summary>
		/// Gets the DICOM file containing the presentation state after serialization.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public DicomFile DicomFile
		{
			get
			{
				if (!_serialized)
					throw new InvalidOperationException(_messageNotYetSerialized);

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

		/// <summary>
		/// Serializes the presentation state of the given images to the current state object.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has already been serialized to a file.</exception>
		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			if (_serialized)
				throw new InvalidOperationException(_messageAlreadySerialized);

			// create UIDs if needed now
			this.PresentationSeriesInstanceUid = CreateUid(this.PresentationSeriesInstanceUid);
			this.PresentationSopInstanceUid = CreateUid(this.PresentationSopInstanceUid);

			_serialized = true;

			GeneralEquipmentModuleIod generalEquipmentModule = new GeneralEquipmentModuleIod(this.DataSet);
			generalEquipmentModule.Manufacturer = this.Manufacturer ?? string.Empty; // this one is type 2 - all other GenEq attributes are type 3
			generalEquipmentModule.ManufacturersModelName = string.IsNullOrEmpty(this.ManufacturersModelName) ? null : this.ManufacturersModelName;
			generalEquipmentModule.DeviceSerialNumber = string.IsNullOrEmpty(this.DeviceSerialNumber) ? null : this.DeviceSerialNumber;
			generalEquipmentModule.SoftwareVersions = string.IsNullOrEmpty(this.SoftwareVersions) ? null : this.SoftwareVersions;
			generalEquipmentModule.InstitutionName = string.IsNullOrEmpty(this.Institution.Name) ? null : this.Institution.Name;
			generalEquipmentModule.InstitutionAddress = string.IsNullOrEmpty(this.Institution.Address) ? null : this.Institution.Address;
			generalEquipmentModule.InstitutionalDepartmentName = string.IsNullOrEmpty(this.Institution.DepartmentName) ? null : this.Institution.DepartmentName;
			generalEquipmentModule.StationName = string.IsNullOrEmpty(this.StationName) ? null : this.StationName;

			GeneralSeriesModuleIod generalSeriesModule = new GeneralSeriesModuleIod(this.DataSet);
			generalSeriesModule.InitializeAttributes();
			generalSeriesModule.SeriesDateTime = this.PresentationSeriesDateTime;
			generalSeriesModule.SeriesDescription = this.PresentationContentLabel;
			generalSeriesModule.SeriesInstanceUid = this.PresentationSeriesInstanceUid;
			generalSeriesModule.SeriesNumber = this.PresentationSeriesNumber;

			PresentationSeriesModuleIod presentationSeriesModule = new PresentationSeriesModuleIod(this.DataSet);
			presentationSeriesModule.InitializeAttributes();
			presentationSeriesModule.Modality = Modality.PR;

			SopCommonModuleIod sopCommonModule = new SopCommonModuleIod(this.DataSet);
			sopCommonModule.SopInstanceUid = this.PresentationSopInstanceUid;
			sopCommonModule.SopClassUid = this.PresentationSopClass.Uid;

			PresentationStateIdentificationModuleIod presentationStateIdentificationModule = new PresentationStateIdentificationModuleIod(this.DataSet);
			presentationStateIdentificationModule.InitializeAttributes();
			presentationStateIdentificationModule.ContentLabel = this.PresentationContentLabel;
			presentationStateIdentificationModule.InstanceNumber = this.PresentationInstanceNumber;
			presentationStateIdentificationModule.PresentationCreationDateTime = DateTime.Now;

			PerformSerialization(images);

			_dicomFile.SourceApplicationEntityTitle = this.SourceAETitle;
			_dicomFile.MediaStorageSopClassUid = this.PresentationSopClassUid;
			_dicomFile.MediaStorageSopInstanceUid = this.PresentationSopInstanceUid;
		}

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			if (!_serialized)
				throw new InvalidOperationException(_messageNotYetSerialized);

			PerformDeserialization(images);
		}

		/// <summary>
		/// Clears the presentation states of the given images.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The images whose presentation states are to be cleared.</param>
		/// <exception cref="InvalidOperationException">Thrown if the presentation state has not yet been serialized to a file.</exception>
		public override void Clear(IEnumerable<IPresentationImage> image)
		{
			if (!_serialized)
				throw new InvalidOperationException(_messageNotYetSerialized);
		}

		/// <summary>
		/// Called by the base <see cref="DicomSoftcopyPresentationState"/> to invoke presentation state serialization of the specified images.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		protected abstract void PerformSerialization(IEnumerable<IPresentationImage> images);

		/// <summary>
		/// Called by the base <see cref="DicomSoftcopyPresentationState"/> to invoke presentation state deserialization to the specified images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		protected abstract void PerformDeserialization(IEnumerable<IPresentationImage> images);

		#region Protected Helper Methods

		private static int? GetNullableInt32 (DicomAttribute attribute, int index)
		{
			int result;
			if (attribute.TryGetInt32(index, out result))
				return result;
			return null;
		}

		private static string CreateUid(string uidHint)
		{
			if (string.IsNullOrEmpty(uidHint))
				return DicomUid.GenerateUid().UID;
			return uidHint;
		}

		private static DicomAttributeCollection CreateMetaInfo(DicomAttributeCollection dataset)
		{
			DicomAttributeCollection metainfo = new DicomAttributeCollection();
			metainfo[DicomTags.MediaStorageSopClassUid].SetStringValue(dataset[DicomTags.SopClassUid].ToString());
			metainfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(dataset[DicomTags.SopInstanceUid].ToString());
			return metainfo;
		}

		/// <summary>
		/// Creates a <see cref="ImageSopInstanceReferenceMacro"/> to the given <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="sop">The image SOP to which a reference is to be constructed.</param>
		/// <returns>An image SOP instance reference macro item.</returns>
		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(ImageSop sop)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = sop.SopClassUid;
			imageReference.ReferencedSopInstanceUid = sop.SopInstanceUid;
			return imageReference;
		}

		/// <summary>
		/// Creates a <see cref="ImageSopInstanceReferenceMacro"/> to the given <see cref="Frame"/>.
		/// </summary>
		/// <param name="frame">The image SOP frame to which a reference is to be constructed.</param>
		/// <returns>An image SOP instance reference macro item.</returns>
		protected static ImageSopInstanceReferenceMacro CreateImageSopInstanceReference(Frame frame)
		{
			ImageSopInstanceReferenceMacro imageReference = new ImageSopInstanceReferenceMacro();
			imageReference.ReferencedSopClassUid = frame.ParentImageSop.SopClassUid;
			imageReference.ReferencedSopInstanceUid = frame.SopInstanceUid;
			imageReference.ReferencedFrameNumber.SetInt32(0, frame.FrameNumber);
			return imageReference;
		}

		#endregion

		#region Static Helpers

		/// <summary>
		/// Represents the callback method to initialize the instance properties of a <see cref="DicomSoftcopyPresentationState"/>.
		/// </summary>
		/// <param name="presentationState">A new, uninitialized presentation state SOP instance.</param>
		public delegate void InitializeDicomSoftcopyPresentationStateCallback(DicomSoftcopyPresentationState presentationState);

		private static void DefaultInitializeDicomSoftcopyPresentationStateCallback(DicomSoftcopyPresentationState presentationState) {}

		/// <summary>
		/// Creates a <see cref="DicomSoftcopyPresentationState"/> for a given image.
		/// </summary>
		/// <param name="image">The image for which the presentation state should be created.</param>
		/// <returns>One of the derived <see cref="DicomSoftcopyPresentationState"/> classes, depending on the type of the <paramref name="image"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states for the type of the given <paramref name="image"/> are not supported.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Create(IPresentationImage image)
		{
			return Create(image, null);
		}

		/// <summary>
		/// Creates a <see cref="DicomSoftcopyPresentationState"/> for a given image.
		/// </summary>
		/// <param name="image">The image for which the presentation state should be created.</param>
		/// <param name="callback">A callback method that initializes the instance properties of the created <see cref="DicomSoftcopyPresentationState"/>.</param>
		/// <returns>One of the derived <see cref="DicomSoftcopyPresentationState"/> classes, depending on the type of the <paramref name="image"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states for the type of the given <paramref name="image"/> are not supported.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static DicomSoftcopyPresentationState Create(IPresentationImage image, InitializeDicomSoftcopyPresentationStateCallback callback)
		{
			callback = callback ?? DefaultInitializeDicomSoftcopyPresentationStateCallback;

			if (image is DicomGrayscalePresentationImage)
			{
				DicomGrayscaleSoftcopyPresentationState grayscaleSoftcopyPresentationState = new DicomGrayscaleSoftcopyPresentationState();
				callback.Invoke(grayscaleSoftcopyPresentationState);
				grayscaleSoftcopyPresentationState.Serialize(image);
				return grayscaleSoftcopyPresentationState;
			}
			else if (image is DicomColorPresentationImage)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				callback.Invoke(colorSoftcopyPresentationState);
				colorSoftcopyPresentationState.Serialize(image);
				return colorSoftcopyPresentationState;
			}
			else
			{
				throw new ArgumentException("DICOM presentation state serialization is not supported for that type of image.");
			}
		}

		/// <summary>
		/// Creates a minimal number of <see cref="DicomSoftcopyPresentationState"/>s for the given images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Presentation state instances can contain information for multiple images, but the images must all be of the same type,
		/// and contain non-conflicting presentation state information. This method creates a minimal number of presentation
		/// state objects for the collection of given images.
		/// </para>
		/// </remarks>
		/// <param name="images">The images for which presentation states are to be created.</param>
		/// <returns>A dictionary mapping of presentation images to its associated presentation state instance.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states are not supported for the type of any one of the given <paramref name="images"/>.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IDictionary<IPresentationImage, DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images)
		{
			return Create(images, null);
		}

		/// <summary>
		/// Creates a minimal number of <see cref="DicomSoftcopyPresentationState"/>s for the given images.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Presentation state instances can contain information for multiple images, but the images must all be of the same type,
		/// and contain non-conflicting presentation state information. This method creates a minimal number of presentation
		/// state objects for the collection of given images.
		/// </para>
		/// </remarks>
		/// <param name="images">The images for which presentation states are to be created.</param>
		/// <param name="callback">A callback method that initializes the instance properties of the created <see cref="DicomSoftcopyPresentationState"/>s.</param>
		/// <returns>A dictionary mapping of presentation images to its associated presentation state instance.</returns>
		/// <exception cref="ArgumentException">Thrown if softcopy presentation states are not supported for the type of any one of the given <paramref name="images"/>.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static IDictionary<IPresentationImage, DicomSoftcopyPresentationState> Create(IEnumerable<IPresentationImage> images, InitializeDicomSoftcopyPresentationStateCallback callback)
		{
			callback = callback ?? DefaultInitializeDicomSoftcopyPresentationStateCallback;

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
				callback.Invoke(grayscaleSoftcopyPresentationState);
				grayscaleSoftcopyPresentationState.Serialize(grayscaleImages);
				foreach (IPresentationImage image in grayscaleImages)
				{
					presentationStates.Add(image, grayscaleSoftcopyPresentationState);
				}
			}
			if (colorImages.Count > 0)
			{
				DicomColorSoftcopyPresentationState colorSoftcopyPresentationState = new DicomColorSoftcopyPresentationState();
				callback.Invoke(colorSoftcopyPresentationState);
				colorSoftcopyPresentationState.Serialize(colorImages);
				foreach (IPresentationImage image in colorImages)
				{
					presentationStates.Add(image, colorSoftcopyPresentationState);
				}
			}
			return presentationStates;
		}

		/// <summary>
		/// Loads a presentation state from a file.
		/// </summary>
		/// <param name="presentationState">The DICOM file containing the presentation state SOP instance.</param>
		/// <returns>A <see cref="DicomSoftcopyPresentationState"/> object of the correct type.</returns>
		/// <exception cref="ArgumentException">Thrown if the given <paramref name="presentationState"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
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

		/// <summary>
		/// Loads a number of presentation states from multiple files.
		/// </summary>
		/// <param name="presentationStates">The DICOM files containing the presentation state SOP instances.</param>
		/// <returns>An enumeration of <see cref="DicomSoftcopyPresentationState"/> objects.</returns>
		/// <exception cref="ArgumentException">Thrown if one of the given <paramref name="presentationStates"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
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

		/// <summary>
		/// Loads a presentation state from a data set.
		/// </summary>
		/// <param name="presentationState">The data set containing the presentation state SOP instance.</param>
		/// <returns>A <see cref="DicomSoftcopyPresentationState"/> object of the correct type.</returns>
		/// <exception cref="ArgumentException">Thrown if the given <paramref name="presentationState"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
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

		/// <summary>
		/// Loads a number of presentation states from multiple data sets.
		/// </summary>
		/// <param name="presentationStates">The data sets containing the presentation state SOP instances.</param>
		/// <returns>An enumeration of <see cref="DicomSoftcopyPresentationState"/> objects.</returns>
		/// <exception cref="ArgumentException">Thrown if one of the given <paramref name="presentationStates"/> is not a supported presentation state SOP class.</exception>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
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

		/// <summary>
		/// Tests to see if softcopy presentation states are supported for the type of the given image.
		/// </summary>
		/// <param name="image">The image whose support for softcopy presentation states is to be checked.</param>
		/// <returns>True if softcopy presentation states are supported for the type of the given image; False otherwise.</returns>
		/// <seealso cref="DicomSoftcopyPresentationState"/>
		public static bool IsSupported(IPresentationImage image)
		{
			return (image is DicomGrayscalePresentationImage) || (image is DicomColorPresentationImage);
		}

		#endregion
	}
}