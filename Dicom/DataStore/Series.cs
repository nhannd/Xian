using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;
using NHibernate;
using NHibernate.Collection;

namespace ClearCanvas.Dicom.DataStore
{
    public class Series : ISeries
    {
        #region Handcoded Members
        
		public delegate void InitializeAssociatedCollectionCallback(object domainObject, PersistentCollection associatedCollection);
        public InitializeAssociatedCollectionCallback InitializeAssociatedCollection;

        public Series()
        {
            _internalSopInstances = new HybridSet();
        }

        protected virtual Guid SeriesOid
        {
            get { return _seriesOid; }
            set { _seriesOid = value; }
        }

        public virtual string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public virtual string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public virtual int SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public virtual string Laterality
        {
            get { return _laterality; }
            set { _laterality = value; }
        }

        public virtual string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        public virtual string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

        public virtual Study Study
        {
            get { return _parentStudy; }
            set { _parentStudy = value; }
        }

        protected virtual ISet InternalSopInstances
        {
            get { return _internalSopInstances; }
        }

        public virtual ISet SopInstances
        {
            get
            {
                if (null != InitializeAssociatedCollection)
                    InitializeAssociatedCollection(this, _internalSopInstances as PersistentCollection);

                return _internalSopInstances;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			Series other = obj as Series;
            if (null == other)
                return false; // null or not a series

			return (this.SeriesInstanceUid == other.SeriesInstanceUid);
        }

        public override int GetHashCode()
        {
            int accumulator = 0;
            foreach (char character in this.SeriesInstanceUid)
            {
                if ('.' != character)
                    accumulator += Convert.ToInt32(character);
                else
                    accumulator -= 17;
            }
            return accumulator;
        }

        #region Private Members

        Guid _seriesOid;
        string _seriesInstanceUid;
        string _modality;
        int _seriesNumber;
        string _laterality;
        string _seriesDescription;
        string _specificCharacterSet;
        ISet _internalSopInstances;
        Study _parentStudy;

		#endregion //Private Members
		#endregion //Handcoded Members

		#region ISeries Members

        public IEnumerable<ISopInstance> GetSopInstances()
        {
            List<ISopInstance> sops = new List<ISopInstance>();
            foreach (ImageSopInstance sop in this.SopInstances)
            {
                sops.Add(sop);
            }
            return sops.AsReadOnly();
        }

        public void AddSopInstance(ISopInstance sop)
        {
            sop.SetParentSeries(this);
            this.SopInstances.Add(sop);
        }
        
        public void RemoveSopInstance(ISopInstance sop)
        {
            this.SopInstances.Remove(sop);
        }

        public Uid GetSeriesInstanceUid()
        {
            return new Uid(this.SeriesInstanceUid);
        }

        public void SetParentStudy(IStudy study)
        {
            this.Study = study as Study;
        }

        public IStudy GetParentStudy()
        {
            return this.Study;
        }

        #endregion
    }
}
