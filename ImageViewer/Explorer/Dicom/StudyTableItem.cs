using System;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class StudyTableItem : IStudyRootStudyIdentifier, IStudyEntryData
    {
        private StudyEntry _entry;

        public StudyTableItem(StudyEntry entry)
        {
            Entry = entry;
        }

        public StudyEntry Entry
        {
            get { return _entry; }
            set
            {
                _entry = value;
                _entry.Study.ResolveServer(true);
                if (_entry.Data == null)
                    _entry.Data = new StudyEntryData();
            }
        }

        private IStudyRootStudyIdentifier Identifier
        {
            get { return Entry.Study; }
        }

        private IStudyEntryData EntryData
        {
            get { return Entry.Data; }
        }

        #region IStudyEntryData Members

        public string[] InstitutionNamesInStudy
        {
            get { return EntryData.InstitutionNamesInStudy; }
            set { EntryData.InstitutionNamesInStudy = value; }
        }

        public string[] StationNamesInStudy
        {
            get { return EntryData.StationNamesInStudy; }
            set { EntryData.StationNamesInStudy = value; }
        }

        public string[] SourceAETitlesInStudy
        {
            get { return EntryData.SourceAETitlesInStudy; }
            set { EntryData.SourceAETitlesInStudy = value; }
        }

        public DateTime? DeleteTime
        {
            get { return EntryData.DeleteTime; }
            set { EntryData.DeleteTime = value; }
        }

        public DateTime? StoreTime
        {
            get { return EntryData.StoreTime; }
            set { EntryData.StoreTime = value; }
        }

        #endregion

        #region IStudyRootStudyIdentifier Members

        public string ResponsibleOrganization
        {
            get { return Identifier.ResponsibleOrganization; }
        }

        public string SpecificCharacterSet
        {
            get { return Identifier.SpecificCharacterSet; }
        }

        string IIdentifier.RetrieveAeTitle
        {
            get { return Identifier.RetrieveAeTitle; }
        }

        IApplicationEntity IIdentifier.RetrieveAE
        {
            get { return Identifier.RetrieveAE; }
        }

        public string InstanceAvailability
        {
            get { return Identifier.InstanceAvailability; }
        }

        public string ResponsiblePersonRole
        {
            get { return Identifier.ResponsiblePersonRole; }
        }

        public string ResponsiblePerson
        {
            get { return Identifier.ResponsiblePerson; }
        }

        public string PatientBreedCodeSequenceCodeMeaning
        {
            get { return Identifier.PatientBreedCodeSequenceCodeMeaning; }
        }

        public string PatientBreedCodeSequenceCodeValue
        {
            get { return Identifier.PatientBreedCodeSequenceCodeValue; }
        }

        public string PatientBreedCodeSequenceCodingSchemeDesignator
        {
            get { return Identifier.PatientBreedCodeSequenceCodingSchemeDesignator; }
        }

        public string PatientBreedDescription
        {
            get { return Identifier.PatientBreedDescription; }
        }

        public string PatientSpeciesCodeSequenceCodeMeaning
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodeMeaning; }
        }

        public string PatientSpeciesCodeSequenceCodeValue
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodeValue; }
        }

        public string PatientSpeciesCodeSequenceCodingSchemeDesignator
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodingSchemeDesignator; }
        }

        public string PatientSpeciesDescription
        {
            get { return Identifier.PatientSpeciesDescription; }
        }

        public string PatientsSex
        {
            get { return Identifier.PatientsSex; }
        }

        public string PatientsBirthTime
        {
            get { return Identifier.PatientsBirthTime; }
        }

        public string PatientsBirthDate
        {
            get { return Identifier.PatientsBirthDate; }
        }

        //TODO (Marmot): Make it a PersonName. Too much of a pain to keep doing new PersonName(...).
        public string PatientsName
        {
            get { return Identifier.PatientsName; }
        }

        public string PatientId
        {
            get { return Identifier.PatientId; }
        }

        public int? NumberOfStudyRelatedInstances
        {
            get { return Identifier.NumberOfStudyRelatedInstances; }
        }

        public int? NumberOfStudyRelatedSeries
        {
            get { return Identifier.NumberOfStudyRelatedSeries; }
        }

        public string ReferringPhysiciansName
        {
            get { return Identifier.ReferringPhysiciansName; }
        }

        public string AccessionNumber
        {
            get { return Identifier.AccessionNumber; }
        }

        public string StudyTime
        {
            get { return Identifier.StudyTime; }
        }

        public string StudyDate
        {
            get { return Identifier.StudyDate; }
        }

        public string StudyId
        {
            get { return Identifier.StudyId; }
        }

        public string StudyDescription
        {
            get { return Identifier.StudyDescription; }
        }

        public string[] ModalitiesInStudy
        {
            get { return Identifier.ModalitiesInStudy; }
        }

        public string[] SopClassesInStudy
        {
            get { return Identifier.SopClassesInStudy; }
        }

        public string StudyInstanceUid
        {
            get { return Identifier.StudyInstanceUid; }
        }

        public IDicomServiceNode Server
        {
            get { return (IDicomServiceNode)((IIdentifier)this).RetrieveAE; }
        }

        #endregion
    }
}