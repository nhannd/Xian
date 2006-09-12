using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Iesi.Collections;

namespace ClearCanvas.Dicom.DataStore
{
    public class Study : IStudy
    {
        #region Handcoded Members
        public Study()
        {
            _series = new HybridSet();
        }

        protected virtual long StudyOid
        {
            get { return _studyOid; }
            set { _studyOid = value; }
        }

        public virtual string ProcedureCodeSequenceCodingSchemeDesignator
        {
            get { return _procedureCodeSequenceCodingSchemeDesignator; }
            set { _procedureCodeSequenceCodingSchemeDesignator = value; }
        }

        public virtual string StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }

        public virtual string StudyTime
        {
            get { return _studyTime; }
            set { _studyTime = value; }
        }

        public virtual string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public virtual string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public virtual string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public virtual string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public virtual string ProcedureCodeSequenceCodeValue
        {
            get { return _procedureCodeSequenceCodeValue; }
            set { _procedureCodeSequenceCodeValue = value; }
        }

        public virtual string AdmittingDiagnosesDescription
        {
            get { return _admittingDiagnosesDescription; }
            set { _admittingDiagnosesDescription = value; }
        }

        public virtual PatientId PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public virtual PatientsName PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public virtual string PatientsSex
        {
            get { return _patientsSex; }
            set { _patientsSex = value; }
        }

        public virtual string PatientsBirthDate
        {
            get { return _patientsBirthDate; }
            set { _patientsBirthDate = value; }
        }

        public virtual ISet Series
        {
            get { return _series; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            Study study = obj as Study;
            if (null == study)
                return false; // null or not a sop

            if (this.StudyOid != 0 && study.StudyOid != 0)
            {
                if (this.StudyOid != study.StudyOid)
                    return false;
            }

            if (this.StudyInstanceUid != study.StudyInstanceUid)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int accumulator = 0;
            foreach (char character in this.StudyInstanceUid)
            {
                if ('.' != character)
                    accumulator += Convert.ToInt32(character);
                else
                    accumulator -= 23;
            }
            return accumulator;
        }
        #region Private Members
        long _studyOid;
        string _procedureCodeSequenceCodingSchemeDesignator;
        string _studyId;
        string _studyTime;
        string _studyDate;
        string _accessionNumber;
        string _studyInstanceUid;
        string _studyDescription;
        string _procedureCodeSequenceCodeValue;
        string _admittingDiagnosesDescription;
        PatientId _patientId;
        PatientsName _patientsName;
        string _patientsSex;
        string _patientsBirthDate;
        ISet _series;
        #endregion
        #endregion
        #region IStudy Members

        public IEnumerable<ISopInstance> GetSopInstances()
        {
            DataAccessLayer.GetIDataStoreReader().InitializeAssociatedObject(this, this.Series);
            List<ISopInstance> sopList = new List<ISopInstance>();
            foreach (ISeries series in this.Series)
            {
                foreach (ISopInstance sopInstance in series.GetSopInstances())
                {
                    sopList.Add(sopInstance);
                }
            }
            return sopList.AsReadOnly();
        }

        public Uid GetStudyInstanceUid()
        {
            return new Uid(this.StudyInstanceUid);
        }

        public void AddSeries(ISeries series)
        {
            series.SetParentStudy(this);
            this.Series.Add(series);
        }

        public void RemoveSeries(ISeries series)
        {
            this.Series.Remove(series);
        }

        public IEnumerable<ISeries> GetSeries()
        {
            List<ISeries> seriesList = new List<ISeries>();
            foreach (ISeries series in this.Series)
            {
                seriesList.Add(series);
            }

            return seriesList.AsReadOnly();
        }

        #endregion
    }
}
