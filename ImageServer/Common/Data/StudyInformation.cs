using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents serializable study information.
    /// </summary>
    [XmlRoot("StudyInformation")]
    public class StudyInformation 
    {
        #region Private Fields
        private string _studyId;
        private string _accessionNumber;
        private string _studyDate;
        private string _studyTime;
        private string _modalities;
        private string _studyInstanceUid;
        private string _studyDescription;
        private string _referringPhysician;
        private PatientInformation _patientInfo = new PatientInformation();
        private List<SeriesInformation> _series= new List<SeriesInformation> ();

        #endregion

        #region Constructors
        public StudyInformation()
        {
        }

        public StudyInformation(IDicomAttributeProvider attributeProvider)
        {
            if (attributeProvider[DicomTags.StudyId]!=null)
                StudyId = attributeProvider[DicomTags.StudyId].ToString();
            
            if (attributeProvider[DicomTags.AccessionNumber]!=null)
                AccessionNumber = attributeProvider[DicomTags.AccessionNumber].ToString();

            if (attributeProvider[DicomTags.StudyDate] != null )
                StudyDate = attributeProvider[DicomTags.StudyDate].ToString();

            if (attributeProvider[DicomTags.ModalitiesInStudy] != null)
                Modalities = attributeProvider[DicomTags.ModalitiesInStudy].ToString();

            if (attributeProvider[DicomTags.StudyInstanceUid] != null)
                StudyInstanceUid = attributeProvider[DicomTags.StudyInstanceUid].ToString();

            if (attributeProvider[DicomTags.StudyDescription] != null)
                StudyDescription = attributeProvider[DicomTags.StudyDescription].ToString();


            if (attributeProvider[DicomTags.ReferringPhysiciansName] != null)
                ReferringPhysician = attributeProvider[DicomTags.ReferringPhysiciansName].ToString();

            PatientInfo = new PatientInformation(attributeProvider);
            SeriesInformation series = new SeriesInformation(attributeProvider);
            if (!String.IsNullOrEmpty(series.SeriesInstanceUid))
                Add(series);
        }

        #endregion

        #region Public Properties
        public string StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }
        
        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string StudyTime
        {
            get { return _studyTime; }
            set { _studyTime = value; }
        }

        public string Modalities
        {
            get { return _modalities; }
            set { _modalities = value; }
        }

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }


        public string ReferringPhysician
        {
            get { return _referringPhysician; }
            set { _referringPhysician = value; }
        }

        public PatientInformation PatientInfo
        {
            get { return _patientInfo; }
            set { _patientInfo = value; }
        }

        [XmlArray("Series")]
        public List<SeriesInformation> Series
        {
            get { return _series;}
            set { _series = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a <see cref="SeriesInformation"/> data
        /// </summary>
        /// <param name="series"></param>
        public void Add(SeriesInformation series)
        {
            SeriesInformation theSeries = Series.Find(delegate(SeriesInformation ser) { return ser.SeriesInstanceUid == series.SeriesInstanceUid; });
            if (theSeries==null)
            {
                this.Series.Add(series);
            }
            else
            {

                theSeries.NumberOfInstances++;
            }
        }

        /// <summary>
        /// Adds a list of <see cref="SeriesInformation"/> data
        /// </summary>
        /// <param name="series"></param>
        public void Add(IEnumerable<SeriesInformation> series)
        {
           foreach(SeriesInformation ser in series)
           {
               Add(ser);
           }
       }
       #endregion

       #region Public Static Methods
       public static StudyInformation CreateFrom(Study study)
        {
            ServerEntityAttributeProvider studyWrapper = new ServerEntityAttributeProvider(study);
            StudyInformation studyInfo = new StudyInformation(studyWrapper);

            foreach(Model.Series series in study.Series)
            {
                ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(series);
                SeriesInformation seriesInfo = new SeriesInformation(seriesWrapper);
                studyInfo.Add(seriesInfo);
            }

            return studyInfo;
        }
        #endregion
    }
}