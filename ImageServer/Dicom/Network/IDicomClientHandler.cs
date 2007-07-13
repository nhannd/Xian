using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public interface IDicomClientHandler
    {
        void OnReceiveAssociateAccept(DicomClient client, AssociationParameters association);
        void OnReceiveAssociateReject(DicomClient client, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason);
        void OnReceiveRequestMessage(DicomClient client, byte presentationID, DicomMessage message);
        void OnReceiveResponseMessage(DicomClient client, byte presentationID, DicomMessage message);
        void OnReceiveReleaseResponse(DicomClient client);
        void OnReceiveAbort(DicomClient client, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomClient client, Exception e);
        void OnDimseTimeout(DicomClient server);
    }
}
