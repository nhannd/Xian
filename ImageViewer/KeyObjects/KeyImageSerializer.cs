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
using ClearCanvas.Dicom.Iod.Macros.HierarchicalSeriesInstanceReference;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public class KeyImageSerializer
	{
		private readonly FramePresentationList _framePresentationStates;
		//private readonly List<string> _docTitleMods;
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private KeyObjectSelectionDocumentTitle _docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

		public KeyImageSerializer()
		{
			_framePresentationStates = new FramePresentationList();
			_datetime = Platform.Time;
		}

		public IList<KeyValuePair<Frame, DicomSoftcopyPresentationState>> FramePresentationStates
		{
			get { return _framePresentationStates; }
		}

		//public IList<string> DocumentTitleModifiers
		//{
		//    get { return _docTitleMods; }
		//}

		public DateTime DateTime
		{
			get { return _datetime; }
			set { _datetime = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

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
				foreach (KeyValuePair<Frame,DicomSoftcopyPresentationState> framePRPair in _framePresentationStates)
				{
					Frame frame = framePRPair.Key;
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

						IImageReferenceMacro imgMac = content.InitializeImageReferenceAttributes();
						imgMac.ReferencedSopSequence.InitializeAttributes();
						imgMac.ReferencedSopSequence.ReferencedSopClassUid = sop.SopClassUID;
						imgMac.ReferencedSopSequence.ReferencedSopInstanceUid = sop.SopInstanceUID;
						if (sop.NumberOfFrames > 1)
							imgMac.ReferencedSopSequence.ReferencedFrameNumber = frame.FrameNumber.ToString();
						else
							imgMac.ReferencedSopSequence.ReferencedFrameNumber = null;

						// save the presentation state
						if(framePRPair.Value!=null)
						{
							DicomSoftcopyPresentationState presentationState = framePRPair.Value;
							imgMac.ReferencedSopSequence.CreateReferencedSopSequence();
							imgMac.ReferencedSopSequence.ReferencedSopSequence.InitializeAttributes();
							imgMac.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopClassUid = presentationState.PresentationSopClass.Uid;
							imgMac.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid = presentationState.PresentationInstanceUid;
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

			PatientModuleIod srcPatient = new PatientModuleIod(source);
			if (true) // patient module is always required
			{
				iod.Patient.BreedRegistrationSequence = srcPatient.BreedRegistrationSequence;
				iod.Patient.DeIdentificationMethod = srcPatient.DeIdentificationMethod;
				iod.Patient.DeIdentificationMethodCodeSequence = srcPatient.DeIdentificationMethodCodeSequence;
				iod.Patient.EthnicGroup = srcPatient.EthnicGroup;
				iod.Patient.IssuerOfPatientId = srcPatient.IssuerOfPatientId;
				iod.Patient.OtherPatientIds = srcPatient.OtherPatientIds;
				iod.Patient.OtherPatientIdsSequence = srcPatient.OtherPatientIdsSequence;
				iod.Patient.OtherPatientNames = srcPatient.OtherPatientNames;
				iod.Patient.PatientBreedCodeSequence = srcPatient.PatientBreedCodeSequence;
				iod.Patient.PatientBreedDescription = srcPatient.PatientBreedDescription;
				iod.Patient.PatientComments = srcPatient.PatientComments;
				iod.Patient.PatientId = srcPatient.PatientId;
				iod.Patient.PatientIdentityRemoved = srcPatient.PatientIdentityRemoved;
				iod.Patient.PatientsBirthDateTime = srcPatient.PatientsBirthDateTime;
				iod.Patient.PatientsName = srcPatient.PatientsName;
				iod.Patient.PatientSpeciesCodeSequence = srcPatient.PatientSpeciesCodeSequence;
				iod.Patient.PatientSpeciesDescription = srcPatient.PatientSpeciesDescription;
				iod.Patient.PatientsSex = srcPatient.PatientsSex;
				iod.Patient.ReferencedPatientSequence = srcPatient.ReferencedPatientSequence;
				iod.Patient.ResponsibleOrganization = srcPatient.ResponsibleOrganization;
				iod.Patient.ResponsiblePerson = srcPatient.ResponsiblePerson;
				iod.Patient.ResponsiblePersonRole = srcPatient.ResponsiblePersonRole;
			}

			SpecimenIdentificationModuleIod srcSpecimen = new SpecimenIdentificationModuleIod(source);
			if (srcSpecimen.HasValues()) // specimen module is required only if subject is a specimen
			{
				iod.SpecimenIdentification.SpecimenAccessionNumber = srcSpecimen.SpecimenAccessionNumber;
				iod.SpecimenIdentification.SpecimenSequence = srcSpecimen.SpecimenSequence;
			}

			ClinicalTrialSubjectModuleIod srcTrialSubject = new ClinicalTrialSubjectModuleIod(source);
			if (srcTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				iod.ClinicalTrialSubject.ClinicalTrialProtocolId = srcTrialSubject.ClinicalTrialProtocolId;
				iod.ClinicalTrialSubject.ClinicalTrialProtocolName = srcTrialSubject.ClinicalTrialProtocolName;
				iod.ClinicalTrialSubject.ClinicalTrialSiteId = srcTrialSubject.ClinicalTrialSiteId;
				iod.ClinicalTrialSubject.ClinicalTrialSiteName = srcTrialSubject.ClinicalTrialSiteName;
				iod.ClinicalTrialSubject.ClinicalTrialSponsorName = srcTrialSubject.ClinicalTrialSponsorName;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectId = srcTrialSubject.ClinicalTrialSubjectId;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectReadingId = srcTrialSubject.ClinicalTrialSubjectReadingId;
			}

			GeneralStudyModuleIod srcGeneralStudy = new GeneralStudyModuleIod(source);
			if (true) // general study module is always required
			{
				iod.GeneralStudy.AccessionNumber = srcGeneralStudy.AccessionNumber;
				iod.GeneralStudy.NameOfPhysiciansReadingStudy = srcGeneralStudy.NameOfPhysiciansReadingStudy;
				iod.GeneralStudy.PhysiciansOfRecord = srcGeneralStudy.PhysiciansOfRecord;
				iod.GeneralStudy.PhysiciansOfRecordIdentificationSequence = srcGeneralStudy.PhysiciansOfRecordIdentificationSequence;
				iod.GeneralStudy.PhysiciansReadingStudyIdentificationSequence = srcGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
				iod.GeneralStudy.ProcedureCodeSequence = srcGeneralStudy.ProcedureCodeSequence;
				iod.GeneralStudy.ReferencedStudySequence = srcGeneralStudy.ReferencedStudySequence;
				iod.GeneralStudy.ReferringPhysicianIdentificationSequence = srcGeneralStudy.ReferringPhysicianIdentificationSequence;
				iod.GeneralStudy.ReferringPhysiciansName = srcGeneralStudy.ReferringPhysiciansName;
				iod.GeneralStudy.StudyDateTime = srcGeneralStudy.StudyDateTime;
				iod.GeneralStudy.StudyDescription = srcGeneralStudy.StudyDescription;
				iod.GeneralStudy.StudyId = srcGeneralStudy.StudyId;
				iod.GeneralStudy.StudyInstanceUid = srcGeneralStudy.StudyInstanceUid;
			}

			PatientStudyModuleIod srcPatientStudy = new PatientStudyModuleIod(source);
			if (srcPatientStudy.HasValues()) // patient study module is user optional
			{
				iod.PatientStudy.AdditionalPatientHistory = srcPatientStudy.AdditionalPatientHistory;
				iod.PatientStudy.AdmissionId = srcPatientStudy.AdmissionId;
				iod.PatientStudy.AdmittingDiagnosesCodeSequence = srcPatientStudy.AdmittingDiagnosesCodeSequence;
				iod.PatientStudy.AdmittingDiagnosesDescription = srcPatientStudy.AdmittingDiagnosesDescription;
				iod.PatientStudy.IssuerOfAdmissionId = srcPatientStudy.IssuerOfAdmissionId;
				iod.PatientStudy.IssuerOfServiceEpisodeId = srcPatientStudy.IssuerOfServiceEpisodeId;
				iod.PatientStudy.Occupation = srcPatientStudy.Occupation;
				iod.PatientStudy.PatientsAge = srcPatientStudy.PatientsAge;
				iod.PatientStudy.PatientsSexNeutered = srcPatientStudy.PatientsSexNeutered;
				iod.PatientStudy.PatientsSize = srcPatientStudy.PatientsSize;
				iod.PatientStudy.PatientsWeight = srcPatientStudy.PatientsWeight;
				iod.PatientStudy.ServiceEpisodeDescription = srcPatientStudy.ServiceEpisodeDescription;
				iod.PatientStudy.ServiceEpisodeId = srcPatientStudy.ServiceEpisodeId;
			}

			ClinicalTrialStudyModuleIod srcTrialStudy = new ClinicalTrialStudyModuleIod(source);
			if (srcTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				iod.ClinicalTrialStudy.ClinicalTrialTimePointDescription = srcTrialStudy.ClinicalTrialTimePointDescription;
				iod.ClinicalTrialStudy.ClinicalTrialTimePointId = srcTrialStudy.ClinicalTrialTimePointId;
			}

			ClinicalTrialSeriesModuleIod srcTrialSeries = new ClinicalTrialSeriesModuleIod(source);
			if (srcTrialSeries.HasValues()) // clinical trial series module is user optional
			{
				iod.ClinicalTrialSeries.ClinicalTrialCoordinatingCenterName = srcTrialSeries.ClinicalTrialCoordinatingCenterName;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesDescription = srcTrialSeries.ClinicalTrialSeriesDescription;
				iod.ClinicalTrialSeries.ClinicalTrialSeriesId = srcTrialSeries.ClinicalTrialSeriesId;
			}

			GeneralEquipmentModuleIod srcGeneralEquipment = new GeneralEquipmentModuleIod(source);
			if (true) // general equipment module is always required
			{
				iod.GeneralEquipment.DateTimeOfLastCalibrationDateTime = srcGeneralEquipment.DateTimeOfLastCalibrationDateTime;
				iod.GeneralEquipment.DeviceSerialNumber = srcGeneralEquipment.DeviceSerialNumber;
				iod.GeneralEquipment.GantryId = srcGeneralEquipment.GantryId;
				iod.GeneralEquipment.InstitutionAddress = srcGeneralEquipment.InstitutionAddress;
				iod.GeneralEquipment.InstitutionalDepartmentName = srcGeneralEquipment.InstitutionalDepartmentName;
				iod.GeneralEquipment.InstitutionName = srcGeneralEquipment.InstitutionName;
				iod.GeneralEquipment.Manufacturer = srcGeneralEquipment.Manufacturer;
				iod.GeneralEquipment.ManufacturersModelName = srcGeneralEquipment.ManufacturersModelName;
				iod.GeneralEquipment.PixelPaddingValue = srcGeneralEquipment.PixelPaddingValue;
				iod.GeneralEquipment.SoftwareVersions = srcGeneralEquipment.SoftwareVersions;
				iod.GeneralEquipment.SpatialResolution = srcGeneralEquipment.SpatialResolution;
				iod.GeneralEquipment.StationName = srcGeneralEquipment.StationName;
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
				bool result = base.TryAddReference(dataset[DicomTags.StudyInstanceUid], state.PresentationSeriesUid, state.PresentationSopClassUid, state.PresentationInstanceUid);
				if (result && !_seriesInfo.ContainsKey(state.PresentationSeriesUid))
				{
					SeriesInfo seriesInfo = new SeriesInfo();
					seriesInfo.RetrieveAeTitle = dataset[DicomTags.RetrieveAeTitle].ToString();
					seriesInfo.StorageMediaFileSetId = dataset[DicomTags.StorageMediaFileSetId].GetString(0, string.Empty);
					seriesInfo.StorageMediaFileSetUid = dataset[DicomTags.StorageMediaFileSetUid].GetString(0, string.Empty);
					_seriesInfo.Add(state.PresentationSeriesUid, seriesInfo);
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