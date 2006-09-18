using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.Dicom.Services
{
    public class Sender : ISender
    {
        public Sender(ApplicationEntity ae)
        {
            _myAEParameters = ae;
        }

        public virtual ISendQueue SendQueue
        {
            get { return DicomServicesLayer.GetISendQueue(); }
        }

        protected ApplicationEntity MyAE
        {
            get { return _myAEParameters; }
        }

        #region ISendService Members

        public void Send(Uid referencedUid, ApplicationEntity destinationAE, string parcelDescription)
        {
            // create the parcel
            IParcel aParcel = this.SendQueue.CreateNewParcel(this.MyAE, destinationAE, parcelDescription);

            // put the object into the parcel
            if (aParcel.Include(referencedUid) > 0)
            {
                // put the parcel into the SendQueue
                this.SendQueue.Add(aParcel);
            }
        }

        #endregion

        private ApplicationEntity _myAEParameters;
    }
}
