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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// A class for serializing a key image series from a number of images with associated presentation states.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public class KeyImageSerializer
	{
		private readonly FramePresentationList _framePresentationStates;
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private KeyObjectSelectionDocumentTitle _docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageSerializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageSerializer()
		{
			_framePresentationStates = new FramePresentationList();
			_datetime = Platform.Time;
		}

		/// <summary>
		/// Gets or sets the series date time to use for the key object selection document.
		/// </summary>
		public DateTime DateTime
		{
			get { return _datetime; }
			set { _datetime = value; }
		}

		/// <summary>
		/// Gets or sets the description of the key object selection.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		/// <summary>
		/// Gets or sets the key object selection document title.
		/// </summary>
		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

		/// <summary>
		/// Adds a frame and associated presentation state to the serialization queue.
		/// </summary>
		public void AddImage(Frame frame, DicomSoftcopyPresentationState presentationState)
		{
			_framePresentationStates.Add(new KeyValuePair<Frame, DicomSoftcopyPresentationState>(frame, presentationState));
		}

		/// <summary>
		/// Serializes the current contents into a number of key object selection document SOP instances.
		/// </summary>
		public List<DicomFile> Serialize()
		{
			if (_framePresentationStates.Count == 0)
				throw new InvalidOperationException("Key object selection cannot be empty.");

			List<DicomFile> keyObjectDocuments = new List<DicomFile>();
			List<IHierarchicalSopInstanceReferenceMacro> identicalDocuments = new List<IHierarchicalSopInstanceReferenceMacro>();
			Dictionary<string, KeyObjectSelectionDocumentIod> koDocumentsByStudy = new Dictionary<string, KeyObjectSelectionDocumentIod>();
			foreach (Frame frame in (IEnumerable<Frame>)_framePresentationStates)
			{
				string studyInstanceUid = frame.StudyInstanceUID;
				if (!koDocumentsByStudy.ContainsKey(studyInstanceUid))
				{
					DicomFile keyObjectDocument = new DicomFile();
					KeyObjectSelectionDocumentIod iod = CreatePrototypeDocument(frame.ParentImageSop.DataSource, keyObjectDocument.DataSet);

					iod.KeyObjectDocumentSeries.InitializeAttributes();
					iod.KeyObjectDocumentSeries.Modality = Modality.KO;
					iod.KeyObjectDocumentSeries.SeriesDateTime = _datetime;
					iod.KeyObjectDocumentSeries.SeriesDescription = _seriesDescription;
					iod.KeyObjectDocumentSeries.SeriesInstanceUid = DicomUid.GenerateUid().UID;
					iod.KeyObjectDocumentSeries.SeriesNumber = CalculateSeriesNumber(frame);
					iod.KeyObjectDocumentSeries.ReferencedPerformedProcedureStepSequence = null;
					iod.SopCommon.SopClass = SopClass.KeyObjectSelectionDocumentStorage;
					iod.SopCommon.SopInstanceUid = DicomUid.GenerateUid().UID;

					identicalDocuments.Add(iod.KeyObjectDocument.CreateIdenticalDocumentsSequence(
						studyInstanceUid,
						iod.KeyObjectDocumentSeries.SeriesInstanceUid,
						iod.SopCommon.SopClassUid,
						iod.SopCommon.SopInstanceUid));

					koDocumentsByStudy.Add(studyInstanceUid, iod);
					keyObjectDocuments.Add(keyObjectDocument);
				}
			}

			foreach (KeyObjectSelectionDocumentIod iod in koDocumentsByStudy.Values)
			{
				iod.KeyObjectDocument.InitializeAttributes();
				iod.KeyObjectDocument.InstanceNumber = 1;
				iod.KeyObjectDocument.ContentDateTime = _datetime;
				iod.KeyObjectDocument.ReferencedRequestSequence = null;

				iod.KeyObjectDocument.IdenticalDocumentsSequence = identicalDocuments.ToArray();

				iod.SrDocumentContent.InitializeContainerAttributes();
				iod.SrDocumentContent.ConceptNameCodeSequence = _docTitle;

				List<IContentSequence> contentList = new List<IContentSequence>();
				EvidenceDictionary currentRequestedProcedureEvidenceList = new EvidenceDictionary();

				Dictionary<ImageSop, List<int>> frameMap = new Dictionary<ImageSop, List<int>>();
				foreach (KeyValuePair<Frame,DicomSoftcopyPresentationState> frameAndPresentationState in _framePresentationStates)
				{
					Frame frame = frameAndPresentationState.Key;
					ImageSop sop = frame.ParentImageSop;

					// build frame map by unique sop - used to make the evidence sequence less verbose
					if (!frameMap.ContainsKey(frame.ParentImageSop))
						frameMap.Add(frame.ParentImageSop, new List<int>());
					List<int> frames = frameMap[frame.ParentImageSop];
					if (!frames.Contains(frame.FrameNumber))
						frames.Add(frame.FrameNumber);

					// content sequence must still list all content as it was given, including any repeats
					IContentSequence content = iod.SrDocumentContent.CreateContentSequence();
					{
						content.RelationshipType = RelationshipType.Contains;
						content.ReferencedContentItemIdentifier = new uint[] { 1 };

						IImageReferenceMacro imageReferenceMacro = content.InitializeImageReferenceAttributes();
						imageReferenceMacro.ReferencedSopSequence.InitializeAttributes();
						imageReferenceMacro.ReferencedSopSequence.ReferencedSopClassUid = sop.SopClassUID;
						imageReferenceMacro.ReferencedSopSequence.ReferencedSopInstanceUid = sop.SopInstanceUID;
						if (sop.NumberOfFrames > 1)
							imageReferenceMacro.ReferencedSopSequence.ReferencedFrameNumber = frame.FrameNumber.ToString();
						else
							imageReferenceMacro.ReferencedSopSequence.ReferencedFrameNumber = null;

						// save the presentation state
						if(frameAndPresentationState.Value!=null)
						{
							DicomSoftcopyPresentationState presentationState = frameAndPresentationState.Value;
							imageReferenceMacro.ReferencedSopSequence.CreateReferencedSopSequence();
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.InitializeAttributes();
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopClassUid = presentationState.PresentationSopClass.Uid;
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid = presentationState.PresentationSopInstanceUid;
						}
					}
					contentList.Add(content);
				}

				// add the description
				if (!string.IsNullOrEmpty(_description))
				{
					IContentSequence koDescription = iod.SrDocumentContent.CreateContentSequence();
					koDescription.InitializeAttributes();
					koDescription.ConceptNameCodeSequence = KeyObjectSelectionCodeSequences.DocumentTitleModifier;
					koDescription.TextValue = _description;
					koDescription.RelationshipType = RelationshipType.Contains;
					koDescription.ReferencedContentItemIdentifier = new uint[] {1};
					contentList.Add(koDescription);
				}

				// add each unique sop to the evidence list using the map built earlier
				foreach (ImageSop sop in frameMap.Keys)
					currentRequestedProcedureEvidenceList.Add(sop);

				// add each referenced presentation state to the evidence list as well
				foreach (DicomSoftcopyPresentationState state in (IEnumerable<DicomSoftcopyPresentationState>) _framePresentationStates)
					currentRequestedProcedureEvidenceList.Add(state);

				// set the content and the evidence sequences
				iod.SrDocumentContent.ContentSequence = contentList.ToArray();
				iod.KeyObjectDocument.CurrentRequestedProcedureEvidenceSequence = currentRequestedProcedureEvidenceList.ToArray();
			}

			// set meta for the files
			foreach (DicomFile keyObjectDocument in keyObjectDocuments)
			{
				keyObjectDocument.MediaStorageSopClassUid = keyObjectDocument.DataSet[DicomTags.SopClassUid].ToString();
				keyObjectDocument.MediaStorageSopInstanceUid = keyObjectDocument.DataSet[DicomTags.SopInstanceUid].ToString();
			}

			return keyObjectDocuments;
		}

		private int CalculateSeriesNumber(Frame frame)
		{
			if (frame.ParentImageSop == null || frame.ParentImageSop.ParentSeries == null || frame.ParentImageSop.ParentSeries.ParentStudy == null)
				return 1;

			int maxValue = 0;
			foreach (Series series in frame.ParentImageSop.ParentSeries.ParentStudy.Series)
			{
				if (series.SeriesNumber > maxValue)
					maxValue = series.SeriesNumber;
			}

			return maxValue + 1;
		}

		private static KeyObjectSelectionDocumentIod CreatePrototypeDocument(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			KeyObjectSelectionDocumentIod iod = new KeyObjectSelectionDocumentIod(target);

			PatientModuleIod sourcePatient = new PatientModuleIod(source);
			if (true) // patient module is always required
			{
				iod.Patient.BreedRegistrationSequence = sourcePatient.BreedRegistrationSequence;
				iod.Patient.DeIdentificationMethod = sourcePatient.DeIdentificationMethod;
				iod.Patient.DeIdentificationMethodCodeSequence = sourcePatient.DeIdentificationMethodCodeSequence;
				iod.Patient.EthnicGroup = sourcePatient.EthnicGroup;
				iod.Patient.IssuerOfPatientId = sourcePatient.IssuerOfPatientId;
				iod.Patient.OtherPatientIds = sourcePatient.OtherPatientIds;
				iod.Patient.OtherPatientIdsSequence = sourcePatient.OtherPatientIdsSequence;
				iod.Patient.OtherPatientNames = sourcePatient.OtherPatientNames;
				iod.Patient.PatientBreedCodeSequence = sourcePatient.PatientBreedCodeSequence;
				iod.Patient.PatientBreedDescription = sourcePatient.PatientBreedDescription;
				iod.Patient.PatientComments = sourcePatient.PatientComments;
				iod.Patient.PatientId = sourcePatient.PatientId;
				iod.Patient.PatientIdentityRemoved = sourcePatient.PatientIdentityRemoved;
				iod.Patient.PatientsBirthDateTime = sourcePatient.PatientsBirthDateTime;
				iod.Patient.PatientsName = sourcePatient.PatientsName;
				iod.Patient.PatientSpeciesCodeSequence = sourcePatient.PatientSpeciesCodeSequence;
				iod.Patient.PatientSpeciesDescription = sourcePatient.PatientSpeciesDescription;
				iod.Patient.PatientsSex = sourcePatient.PatientsSex;
				iod.Patient.ReferencedPatientSequence = sourcePatient.ReferencedPatientSequence;
				iod.Patient.ResponsibleOrganization = sourcePatient.ResponsibleOrganization;
				iod.Patient.ResponsiblePerson = sourcePatient.ResponsiblePerson;
				iod.Patient.ResponsiblePersonRole = sourcePatient.ResponsiblePersonRole;
			}

			SpecimenIdentificationModuleIod sourceSpecimen = new SpecimenIdentificationModuleIod(source);
			if (sourceSpecimen.HasValues()) // specimen module is required only if subject is a specimen
			{
				iod.SpecimenIdentification.SpecimenAccessionNumber = sourceSpecimen.SpecimenAccessionNumber;
				iod.SpecimenIdentification.SpecimenSequence = sourceSpecimen.SpecimenSequence;
			}

			ClinicalTrialSubjectModuleIod sourceTrialSubject = new ClinicalTrialSubjectModuleIod(source);
			if (sourceTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				iod.ClinicalTrialSubject.ClinicalTrialProtocolId = sourceTrialSubject.ClinicalTrialProtocolId;
				iod.ClinicalTrialSubject.ClinicalTrialProtocolName = sourceTrialSubject.ClinicalTrialProtocolName;
				iod.ClinicalTrialSubject.ClinicalTrialSiteId = sourceTrialSubject.ClinicalTrialSiteId;
				iod.ClinicalTrialSubject.ClinicalTrialSiteName = sourceTrialSubject.ClinicalTrialSiteName;
				iod.ClinicalTrialSubject.ClinicalTrialSponsorName = sourceTrialSubject.ClinicalTrialSponsorName;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectId = sourceTrialSubject.ClinicalTrialSubjectId;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectReadingId = sourceTrialSubject.ClinicalTrialSubjectReadingId;
			}

			GeneralStudyModuleIod sourceGeneralStudy = new GeneralStudyModuleIod(source);
			if (true) // general study module is always required
			{
				iod.GeneralStudy.AccessionNumber = sourceGeneralStudy.AccessionNumber;
				iod.GeneralStudy.NameOfPhysiciansReadingStudy = sourceGeneralStudy.NameOfPhysiciansReadingStudy;
				iod.GeneralStudy.PhysiciansOfRecord = sourceGeneralStudy.PhysiciansOfRecord;
				iod.GeneralStudy.PhysiciansOfRecordIdentificationSequence = sourceGeneralStudy.PhysiciansOfRecordIdentificationSequence;
				iod.GeneralStudy.PhysiciansReadingStudyIdentificationSequence = sourceGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
				iod.GeneralStudy.ProcedureCodeSequence = sourceGeneralStudy.ProcedureCodeSequence;
				iod.GeneralStudy.ReferencedStudySequence = sourceGeneralStudy.ReferencedStudySequence;
				iod.GeneralStudy.ReferringPhysicianIdentificationSequence = sourceGeneralStudy.ReferringPhysicianIdentificationSequence;
				iod.GeneralStudy.ReferringPhysiciansName = sourceGeneralStudy.ReferringPhysiciansName;
				iod.GeneralStudy.StudyDateTime = sourceGeneralStudy.StudyDateTime;
				iod.GeneralStudy.StudyDescription = sourceGeneralStudy.StudyDescription;
				iod.GeneralStudy.StudyId = sourceGeneralStudy.StudyId;
				iod.GeneralStudy.StudyInstanceUid = sourceGeneralStudy.StudyInstanceUid;
			}

			PatientStudyModuleIod sourcePatientStudy = new PatientStudyModuleIod(source);
			if (sourcePatientStudy.HasValues()) // patient study module is user optional
			{
				iod.PatientStudy.AdditionalPatientHistory = sourcePatientStudy.AdditionalPatientHistory;
				iod.PatientStudy.AdmissionId = sourcePatientStudy.AdmissionId;
				iod.PatientStudy.AdmittingDiagnosesCodeSequence = sourcePatientStudy.AdmittingDiagnosesCodeSequence;
				iod.PatientStudy.AdmittingDiagnosesDescription = sourcePatientStudy.AdmittingDiagnosesDescription;
				iod.PatientStudy.IssuerOfAdmissionId = sourcePatientStudy.IssuerOfAdmissionId;
				iod.PatientStudy.IssuerOfServiceEpisodeId = sourcePatientStudy.IssuerOfServiceEpisodeId;
				iod.PatientStudy.Occupation = sourcePatientStudy.Occupation;
				iod.PatientStudy.PatientsAge = sourcePatientStudy.PatientsAge;
				iod.PatientStudy.PatientsSexNeutered = sourcePatientStudy.PatientsSexNeutered;
				iod.PatientStudy.PatientsSize = sourcePatientStudy.PatientsSize;
				iod.PatientStudy.PatientsWeight = sourcePatientStudy.PatientsWeight;
				iod.PatientStudy.ServiceEpisodeDescription = sourcePatientStudy.ServiceEpisodeDescription;
				iod.PatientStudy.ServiceEpisodeId = sourcePatientStudy.ServiceEpisodeId;
			}

			ClinicalTrialStudyModuleIod sourceTrialStudy = new ClinicalTrialStudyModuleIod(source);
			if (sourceTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				iod.ClinicalTrialStudy.ClinicalTrialTimePointDescription = sourceTrialStudy.ClinicalTrialTimePointDescription;
				iod.ClinicalTrialStudy.ClinicalTrialTimePointId = sourceTrialStudy.ClinicalTrialTimePointId;
			}

			ClinicalTrialSeriesModuleIod sourceTrialSeries = new ClinicalTrialSeriesModuleIod(source);
			if (sourceTrialSeries.HasValues()) // clinical trial series module is user optional
			{
				iod.ClinicalTrialSeries.ClinicalTrialCoordinatingCenterName = sourceTrialSeries.ClinicalTrialCoordinatingCenterName;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesDescription = sourceTrialSeries.ClinicalTrialSeriesDescription;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesId = sourceTrialSeries.ClinicalTrialSeriesId;
			}

			GeneralEquipmentModuleIod sourceGeneralEquipment = new GeneralEquipmentModuleIod(source);
			if (true) // general equipment module is always required
			{
				iod.GeneralEquipment.DateTimeOfLastCalibrationDateTime = sourceGeneralEquipment.DateTimeOfLastCalibrationDateTime;
				iod.GeneralEquipment.DeviceSerialNumber = sourceGeneralEquipment.DeviceSerialNumber;
				iod.GeneralEquipment.GantryId = sourceGeneralEquipment.GantryId;
				iod.GeneralEquipment.InstitutionAddress = sourceGeneralEquipment.InstitutionAddress;
				iod.GeneralEquipment.InstitutionalDepartmentName = sourceGeneralEquipment.InstitutionalDepartmentName;
				iod.GeneralEquipment.InstitutionName = sourceGeneralEquipment.InstitutionName;
				iod.GeneralEquipment.Manufacturer = sourceGeneralEquipment.Manufacturer;
				iod.GeneralEquipment.ManufacturersModelName = sourceGeneralEquipment.ManufacturersModelName;
				iod.GeneralEquipment.PixelPaddingValue = sourceGeneralEquipment.PixelPaddingValue;
				iod.GeneralEquipment.SoftwareVersions = sourceGeneralEquipment.SoftwareVersions;
				iod.GeneralEquipment.SpatialResolution = sourceGeneralEquipment.SpatialResolution;
				iod.GeneralEquipment.StationName = sourceGeneralEquipment.StationName;
			}

			return iod;
		}

		#region FramePresentationList Class

		private class FramePresentationList : IList<KeyValuePair<Frame, DicomSoftcopyPresentationState>>, IEnumerable<Frame>, IEnumerable<DicomSoftcopyPresentationState>
		{
			private readonly List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> _list = new List<KeyValuePair<Frame, DicomSoftcopyPresentationState>>();

			public int IndexOf(KeyValuePair<Frame, DicomSoftcopyPresentationState> item)
			{
				return _list.IndexOf(item);
			}

			public void Insert(int index, KeyValuePair<Frame, DicomSoftcopyPresentationState> item)
			{
				Platform.CheckForNullReference(item, "item");
				Platform.CheckForNullReference(item.Key, "item");
				_list.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				_list.RemoveAt(index);
			}

			public KeyValuePair<Frame, DicomSoftcopyPresentationState> this[int index]
			{
				get { return _list[index]; }
				set
				{
					Platform.CheckForNullReference(value, "value");
					Platform.CheckForNullReference(value.Key, "value");
					_list[index] = value;
				}
			}

			public void Add(KeyValuePair<Frame, DicomSoftcopyPresentationState> item)
			{
				Platform.CheckForNullReference(item, "item");
				Platform.CheckForNullReference(item.Key, "item");
				_list.Add(item);
			}

			public void Clear()
			{
				_list.Clear();
			}

			public bool Contains(KeyValuePair<Frame, DicomSoftcopyPresentationState> item)
			{
				return _list.Contains(item);
			}

			public void CopyTo(KeyValuePair<Frame, DicomSoftcopyPresentationState>[] array, int arrayIndex)
			{
				_list.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _list.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(KeyValuePair<Frame, DicomSoftcopyPresentationState> item)
			{
				return _list.Remove(item);
			}

			public IEnumerator<KeyValuePair<Frame, DicomSoftcopyPresentationState>> GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			IEnumerator<Frame> IEnumerable<Frame>.GetEnumerator()
			{
				foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> pair in _list)
					yield return pair.Key;
			}

			IEnumerator<DicomSoftcopyPresentationState> IEnumerable<DicomSoftcopyPresentationState>.GetEnumerator()
			{
				foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> pair in _list)
					yield return pair.Value;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		#endregion

		#region EvidenceDictionary Class

		private class EvidenceDictionary : HierarchicalSopInstanceReferenceDictionary
		{
			private readonly Dictionary<string, SeriesInfo> _seriesInfo = new Dictionary<string, SeriesInfo>();

			public void Add(Sop sop)
			{
				bool result = base.TryAddReference(sop.StudyInstanceUID, sop.SeriesInstanceUID, sop.SopClassUID, sop.SopInstanceUID);
				if (result && !_seriesInfo.ContainsKey(sop.SeriesInstanceUID))
				{
					SeriesInfo seriesInfo = new SeriesInfo();
					seriesInfo.RetrieveAeTitle = sop[DicomTags.RetrieveAeTitle].ToString();
					seriesInfo.StorageMediaFileSetId = sop[DicomTags.StorageMediaFileSetId].GetString(0, string.Empty);
					seriesInfo.StorageMediaFileSetUid = sop[DicomTags.StorageMediaFileSetUid].GetString(0, string.Empty);
					_seriesInfo.Add(sop.SeriesInstanceUID, seriesInfo);
				}
			}

			public void Add(DicomSoftcopyPresentationState state)
			{
				IDicomAttributeProvider dataset = state.DicomFile.DataSet;
				bool result = base.TryAddReference(dataset[DicomTags.StudyInstanceUid], state.PresentationSeriesInstanceUid, state.PresentationSopClassUid, state.PresentationSopInstanceUid);
				if (result && !_seriesInfo.ContainsKey(state.PresentationSeriesInstanceUid))
				{
					SeriesInfo seriesInfo = new SeriesInfo();
					seriesInfo.RetrieveAeTitle = dataset[DicomTags.RetrieveAeTitle].ToString();
					seriesInfo.StorageMediaFileSetId = dataset[DicomTags.StorageMediaFileSetId].GetString(0, string.Empty);
					seriesInfo.StorageMediaFileSetUid = dataset[DicomTags.StorageMediaFileSetUid].GetString(0, string.Empty);
					_seriesInfo.Add(state.PresentationSeriesInstanceUid, seriesInfo);
				}
			}

			protected override IHierarchicalSeriesInstanceReferenceMacro CreateSeriesReference(string seriesInstanceUid)
			{
				IHierarchicalSeriesInstanceReferenceMacro reference = base.CreateSeriesReference(seriesInstanceUid);
				if (_seriesInfo.ContainsKey(seriesInstanceUid))
				{
					SeriesInfo seriesInfo = _seriesInfo[seriesInstanceUid];
					reference.RetrieveAeTitle = seriesInfo.RetrieveAeTitle;
					reference.StorageMediaFileSetId = seriesInfo.StorageMediaFileSetId;
					reference.StorageMediaFileSetUid = seriesInfo.StorageMediaFileSetUid;
				}
				return reference;
			}

			private class SeriesInfo
			{
				public string RetrieveAeTitle = "";
				public string StorageMediaFileSetId = "";
				public string StorageMediaFileSetUid = "";
			}
		}

		#endregion
	}
}