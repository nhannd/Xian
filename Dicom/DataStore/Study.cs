using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Collection;
using Iesi.Collections;
using System.Collections.ObjectModel;

namespace ClearCanvas.Dicom.DataStore
{
    public class Study : IStudy
    {
        #region Handcoded Members

        public delegate void InitializeAssociatedCollectionCallback(object domainObject, PersistentCollection associatedCollection);
        public InitializeAssociatedCollectionCallback InitializeAssociatedCollection;

        public Study()
        {
            _internalSeries = new HybridSet();
        }

        protected virtual Guid StudyOid
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

        public virtual PatientId PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public virtual PersonName PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public virtual string PatientsNameRaw
        {
            get { return _patientsNameRaw; }
            set { _patientsNameRaw = value; }
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

        public virtual string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

        public virtual DateTime StoreTime
        {
            get { return _storeTime; }
            set { _storeTime = value; }
        }

        protected virtual ISet InternalSeries
        {
            get { return _internalSeries; }
        }

        public virtual ISet Series
        {
            get
            {
                if (null != InitializeAssociatedCollection)
                    InitializeAssociatedCollection(this, _internalSeries as PersistentCollection);

                return _internalSeries;
            }
        }

		public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            Study other = obj as Study;
			if (null == other)
                return false; // null or not a study

			return (this.StudyInstanceUid == other.StudyInstanceUid);
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

        #region Private fields

		Guid _studyOid;
        string _procedureCodeSequenceCodingSchemeDesignator;
        string _studyId;
        string _studyTime;
        string _studyDate;
        string _accessionNumber;
        string _studyInstanceUid;
        string _studyDescription;
        string _specificCharacterSet;
        DateTime _storeTime;
        string _procedureCodeSequenceCodeValue;
        PatientId _patientId;
        PersonName _patientsName;
        string _patientsNameRaw;
        string _patientsSex;
        string _patientsBirthDate;
        ISet _internalSeries;

		#endregion //Private Fields
        #endregion //Handcoded Members

		#region IStudy Members

        public ReadOnlyCollection<ISopInstance> GetSopInstances()
        {
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

        public ReadOnlyCollection<ISeries> GetSeries()
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
