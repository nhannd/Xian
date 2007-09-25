using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using Iesi.Collections;
using NHibernate;
using NHibernate.Collection;

namespace ClearCanvas.Dicom.DataStore
{
	public class Series : PersistentDicomObject, ISeries, IEquatable<Series>
    {
		public delegate void InitializeAssociatedCollectionCallback(object domainObject, PersistentCollection associatedCollection);
        public InitializeAssociatedCollectionCallback InitializeAssociatedCollection;

		#region Private Fields

    	private Guid _seriesOid;
    	private string _seriesInstanceUid;
    	private string _modality;
    	private string _laterality;
    	private int _seriesNumber;
    	private string _seriesDescription;
		private string _specificCharacterSet;
		private Study _parentStudy;
		private readonly ISet _internalSopInstances;
		
		#endregion

		public Series()
        {
            _internalSopInstances = new HybridSet();
        }

		#region NHibernate Persistent Properties

		protected virtual Guid SeriesOid
        {
            get { return _seriesOid; }
			set { _seriesOid = value; }
        }

        public virtual string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
			set { SetClassMember(ref _seriesInstanceUid, value); }
        }

        public virtual string Modality
        {
            get { return _modality; }
			set { SetClassMember(ref _modality, value); }
        }

    	public virtual string Laterality
    	{
    		get { return _laterality; }
			set { SetClassMember(ref _laterality, value); }
    	}

    	public virtual int SeriesNumber
        {
            get { return _seriesNumber; }
			set { SetValueTypeMember(ref _seriesNumber, value); }
        }

    	public virtual string SeriesDescription
        {
            get { return _seriesDescription; }
			set { SetClassMember(ref _seriesDescription, value); }
        }

        public virtual string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
			set { SetClassMember(ref _specificCharacterSet, value); }
        }

        public virtual Study Study
        {
            get { return _parentStudy; }
			set { SetClassMember(ref _parentStudy, value); }
		}

        protected virtual ISet InternalSopInstances
        {
            get { return _internalSopInstances; }
		}

		#endregion

		public virtual ISet SopInstances
        {
            get
            {
                if (null != InitializeAssociatedCollection)
                    InitializeAssociatedCollection(this, _internalSopInstances as PersistentCollection);

                return _internalSopInstances;
            }
        }

    	#region IEquatable<Series> Members

    	public bool Equals(Series other)
    	{
			return (this.SeriesInstanceUid == other.SeriesInstanceUid);
    	}

    	#endregion

    	public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

			if (obj is Series)
				return this.Equals((Series) obj);

			return false;
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

		#region ISeries Members

		public Uid GetSeriesInstanceUid()
		{
			return new Uid(this.SeriesInstanceUid);
		}

		public IStudy GetParentStudy()
		{
			return this.Study;
		}

		public int GetNumberOfSopInstances()
		{
			return this.SopInstances.Count;
		}

    	public IEnumerable<ISopInstance> GetSopInstances()
        {
            foreach (ImageSopInstance sop in this.SopInstances)
            {
            	yield return sop;
            }
        }

        #endregion

		#region Helper Methods

		public void Update(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
		{
			DicomAttribute attribute = sopInstanceDataset[DicomTags.SeriesInstanceUid];
			if (!String.IsNullOrEmpty(SeriesInstanceUid) && SeriesInstanceUid != attribute.ToString())
			{
				throw new InvalidOperationException(SR.ExceptionCanOnlyUpdateExistingSeriesWithSameSeriesUid);
			}
			else
			{
				this.SeriesInstanceUid = attribute.ToString();
			}

			Platform.CheckForEmptyString(SeriesInstanceUid, "SeriesInstanceUid");

			attribute = sopInstanceDataset[DicomTags.Modality];
			Modality = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.Laterality];
			Laterality = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SeriesNumber];
			int intValue;
			attribute.TryGetInt32(0, out intValue);
			SeriesNumber = intValue;

			attribute = sopInstanceDataset[DicomTags.SeriesDescription];
			SeriesDescription = attribute.ToString();

			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			SpecificCharacterSet = attribute.ToString();
		}

    	#endregion
	}
}
