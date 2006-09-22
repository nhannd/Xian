using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Allows access to the DataStore in a way that's not specific to Study, Series or SopInstance
    /// </summary>
    public class RetrieveParcel : Parcel
    {
        public RetrieveParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription)
            : base(sourceAE, destinationAE, parcelDescription)
        {

        }

        private RetrieveParcel() : base()
        {
        }

        public virtual IDataStoreReader DataStoreReader
        {
            get { return DataAccessLayer.GetIDataStoreReader(); }
        }

        public bool IsRetrieveCompleted
        {
            get { return this.ParcelTransferState == ParcelTransferState.Completed; }
        }

        private string _retrieveObjectUid;

        public string RetrieveObjectUid
        {
            get { return _retrieveObjectUid; }
            set { _retrieveObjectUid = value; }
        }
	
    }
}
