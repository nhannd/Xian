using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Manipulate the queue directly with operations that create, add and remove parcels from
    /// the queue
    /// </summary>
    public interface ISendQueue
    {
        ISendParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription);
        IEnumerable<ISendParcel> GetParcels();
        IEnumerable<ISendParcel> GetSendIncompleteParcels();
        void Add(ISendParcel aParcel);
        void Remove(ISendParcel aParcel);
        void UpdateParcel(ISendParcel aParcel);
    }
}
