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
        IParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, string parcelDescription);
        IEnumerable<IParcel> GetParcels();
        IEnumerable<IParcel> GetSendIncompleteParcels();
        void Add(IParcel aParcel);
        void Remove(IParcel aParcel);
        void UpdateParcel(IParcel aParcel);
    }
}
