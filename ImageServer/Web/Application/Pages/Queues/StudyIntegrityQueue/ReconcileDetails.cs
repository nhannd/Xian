using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public class ReconcileDetails
    {
        public class SeriesDetails
        {
            private string _description;
            private int _numberOfInstances;
            private string _modality;

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public string Modalitiy
            {
                get { return _modality; }
                set { _modality = value; }
            }

            public int NumberOfInstances
            {
                get { return _numberOfInstances;  }
                set { _numberOfInstances = value; }
            }
        }
        
        public class PatientInfo
        {
            private string _name;
            private string _accessionNumber;
            private string _sex;
            private string _birthDate;
            private string _patientID;
            private string _issuerOfPatientID;
            
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string AccessionNumber
            {
                get { return _accessionNumber; }
                set { _accessionNumber = value; }
            }

            public string Sex
            {
                get { return _sex; }
                set { _sex = value; }
            }

            public string BirthDate
            {
                get { return _birthDate; }
                set { _birthDate = value; }
            }

            public string PatientID
            {
                get { return _patientID; }
                set { _patientID = value; }
            }

            public string IssuerOfPatientID
            {
                get { return _issuerOfPatientID; }
                set { _issuerOfPatientID = value; }
            }

           
        }

        public class StudyInfo
        {
            private IEnumerable<SeriesDetails> _series;
            private string _studyDate;

            public IEnumerable<SeriesDetails> Series
            {
                get { return _series; }
                set { _series = value; }
            }

            public string StudyDate
            {
                get { return _studyDate; }
                set { _studyDate = value; }
            }
        }

        private string _studyInstanceUID;
        private PatientInfo _existingPatient = new PatientInfo();
        private PatientInfo _conflictingPatient = new PatientInfo();
        private Model.StudyIntegrityQueue _item = new Model.StudyIntegrityQueue();
        private StudyInfo _existingStudy;
        private ImageSetDetails _conflictingImages;

        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        public PatientInfo ExistingPatient
        {
            get { return _existingPatient; }
            set { _existingPatient = value; }
        }

        public StudyInfo ExistingStudy
        {
            get { return _existingStudy; }
            set { _existingStudy = value; }
        }

        public PatientInfo ConflictingPatient
        {
            get { return _conflictingPatient; }
            set { _conflictingPatient = value; }
        }

        public ImageSetDetails ConflictingImageSet
        {
            get { return _conflictingImages; }
            set { _conflictingImages = value; }
        }

        public Model.StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}
