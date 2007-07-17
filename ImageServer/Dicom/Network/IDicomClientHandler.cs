using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public interface IDicomClientHandler
    {
        void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association);
        void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason);
        void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association);
        void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e);
        void OnDimseTimeout(DicomClient server, ClientAssociationParameters association);
    }
}
