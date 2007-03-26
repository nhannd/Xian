using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    internal abstract class Parcel
    {
        private ApplicationEntity _destinationAE;
        private ApplicationEntity _sourceAE;
        private Guid _parcelOid;
        private ParcelTransferState _parcelTransferState = ParcelTransferState.Pending;
        private string _description;
        private int _currentProgressStep;
        private int _totalProgressSteps;

        public Parcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
        {
            _sourceAE = sourceAE;
            _destinationAE = destinationAE;
            _description = parcelDescription;
        }

        protected Parcel()
        {
        }

        #region Properties

        public ParcelTransferState ParcelTransferState
        {
            get { return _parcelTransferState; }
            protected set { _parcelTransferState = value; }
        }

        protected virtual Guid ParcelOid
        {
            get { return _parcelOid; }
            set { _parcelOid = value; }
        }

        public virtual ApplicationEntity DestinationAE
        {
            get { return _destinationAE; }
            protected set { _destinationAE = value; }
        }

        public virtual ApplicationEntity SourceAE
        {
            get { return _sourceAE; }
            protected set { _sourceAE = value; }
        }

        public virtual int TotalProgressSteps
        {
            get { return _totalProgressSteps; }
            set { _totalProgressSteps = value; }
        }

        public virtual int CurrentProgressStep
        {
            get { return _currentProgressStep; }
            set { _currentProgressStep = value; }
        }

        public virtual string Description
        {
            get { return _description; }
            private set { _description = value; }
        }

        #endregion
    }
}
