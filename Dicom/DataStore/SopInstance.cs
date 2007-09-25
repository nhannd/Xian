using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using Iesi.Collections;
using NHibernate;

namespace ClearCanvas.Dicom.DataStore
{
    public abstract class SopInstance : PersistentDicomObject, ISopInstance, IEquatable<SopInstance>
    {
    	private Guid _oid;
		private string _sopInstanceUid;
		private string _sopClassUid;
    	private string _transferSyntaxUid;
    	private int _instanceNumber;
    	private string _specificCharacterSet;
    	private DicomUri _locationUri;
    	private Series _parentSeries;

    	protected SopInstance()
		{
		}

		#region NHibernate Persistent Properties

		protected virtual Guid Oid
        {
            get { return _oid; }
			set { _oid = value; }
        }

        public virtual string SopInstanceUid
        {
            get { return _sopInstanceUid; }
			set { SetClassMember(ref _sopInstanceUid, value); }
        }

        public virtual string SopClassUid
        {
            get { return _sopClassUid; }
			set { SetClassMember(ref _sopClassUid, value); }
        }

    	public virtual string TransferSyntaxUid
    	{
    		get { return _transferSyntaxUid; }
			set { SetClassMember(ref _transferSyntaxUid, value); }
    	}

    	public virtual int InstanceNumber
        {
            get { return _instanceNumber; }
			set { SetValueTypeMember(ref _instanceNumber, value); }
        }

        public virtual string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
			set { SetClassMember(ref _specificCharacterSet, value); }
        }

    	public virtual DicomUri LocationUri
    	{
    		get { return _locationUri; }
			set { SetClassMember(ref _locationUri, value); }
    	}

    	public virtual Series Series
		{
			get { return _parentSeries; }
			set { SetClassMember(ref _parentSeries, value); }
		}

		#endregion

		#region IEquatable<SopInstance> Members

		public bool Equals(SopInstance other)
		{
			return (this.SopInstanceUid == other.SopInstanceUid);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			if (obj is SopInstance)
				return this.Equals((SopInstance) obj);

			return false; // null or not a sop
		}

		public override int GetHashCode()
		{
			int accumulator = 0;
			foreach (char character in this.SopInstanceUid)
			{
				if ('.' != character)
					accumulator += Convert.ToInt32(character);
				else
					accumulator -= 19;
			}
			return accumulator;
		}
		
        #region ISopInstance Members

    	public Uid GetSopInstanceUid()
    	{
    		return new Uid(this.SopInstanceUid);
    	}

    	public Uid GetSopClassUid()
    	{
    		return new Uid(this.SopClassUid);
    	}

    	public Uid GetTransferSyntaxUid()
        {
            return new Uid(this.TransferSyntaxUid);
        }

    	public ISeries GetParentSeries()
    	{
    		return this.Series;
    	}

    	public DicomUri GetLocationUri()
        {
            return this.LocationUri;
        }

		#endregion

		#region Helper Methods

		public virtual void Update(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
		{
			DicomAttribute attribute = sopInstanceDataset[DicomTags.SopInstanceUid];
			if (!String.IsNullOrEmpty(SopInstanceUid) && SopInstanceUid != attribute.ToString())
			{
				throw new InvalidOperationException(SR.ExceptionCanOnlyUpdateExistingSopWithSameSopInstanceUid);
			}
			else
			{
				this.SopInstanceUid = attribute.ToString();
			}

			Platform.CheckForEmptyString(SopInstanceUid, "SopInstanceUid");

			attribute = sopInstanceDataset[DicomTags.SopClassUid];
			SopClassUid = attribute.ToString();

			attribute = metaInfo[DicomTags.TransferSyntaxUid];
			TransferSyntaxUid = attribute.ToString();

			int intValue;
			attribute = sopInstanceDataset[DicomTags.InstanceNumber];
			attribute.TryGetInt32(0, out intValue);
			InstanceNumber = intValue;
			
			attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
			SpecificCharacterSet = attribute.ToString();
		}

    	#endregion
	}
}
