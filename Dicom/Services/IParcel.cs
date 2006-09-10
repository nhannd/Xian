using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Services
{
    public interface IParcel
    {
        int Include(Uid referencedUid);
        ParcelTransferState GetState();
        void StartSend(IDicomSender dicomSender);
        void StopSend();
        int GetToSendObjectCount();
        int SentObjectCount();
        IEnumerable<string> GetReferencedSopInstanceFileNames();
    }
}
