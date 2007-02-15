using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

namespace ClearCanvas.Dicom.DataStore
{
    public abstract class SopInstance : ISopInstance
    {
        #region Handcoded Members
        public virtual Guid Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }

        public virtual string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }

        public virtual string SopClassUid
        {
            get { return _sopClassUid; }
            set { _sopClassUid = value; }
        }

        public virtual DicomUri LocationUri
        {
            get { return _locationUri; }
            set { _locationUri = value; }
        }

        public virtual string TransferSyntaxUid
        {
            get { return _transferSyntaxUid; }
            set { _transferSyntaxUid = value; }
        }

        public virtual int InstanceNumber
        {
            get { return _instanceNumber; }
            set { _instanceNumber = value; }
        }

        public virtual Series Series
        {
            get { return _parentSeries; }
            set { _parentSeries = value; }
        }

        public virtual string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

        #region Private members
        Guid _oid;
        string _sopInstanceUid;
        string _sopClassUid;
        DicomUri _locationUri;
        string _transferSyntaxUid;
        int _instanceNumber;
        string _specificCharacterSet;
        Series _parentSeries;
        #endregion
        #endregion

        #region ISopInstance Members

        public Uid GetTransferSyntaxUid()
        {
            return new Uid(this.TransferSyntaxUid);
        }

        public Uid GetSopClassUid()
        {
            return new Uid(this.SopClassUid);
        }

        public bool IsIdenticalTo(ISopInstance sop)
        {
            return this.SopInstanceUid == sop.GetSopInstanceUid();
        }

        public Uid GetSopInstanceUid()
        {
            return new Uid(this.SopInstanceUid);
        }

        public void SetParentSeries(ISeries series)
        {
            this.Series = series as Series;
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
    }
}
