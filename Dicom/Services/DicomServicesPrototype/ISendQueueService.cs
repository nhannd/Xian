namespace ClearCanvas.Dicom.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.Network;

    public interface ISendQueueService
    {
        IParcel CreateNewParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE);
        IEnumerable<IParcel> GetParcels();
        IEnumerable<IParcel> GetSendIncompleteParcels();
        void Add(IParcel aParcel);
        void Remove(IParcel aParcel);
        void UpdateParcel(IParcel aParcel);
        void LoadAllReferences(IParcel aParcel);
    }
}
