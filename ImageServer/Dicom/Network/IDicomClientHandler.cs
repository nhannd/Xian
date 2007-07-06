using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public interface IDicomClientHandler
    {
        void OnClientClosed(DicomClient client);
        void OnNetworkError(DicomClient client, Exception e);
        void OnReceiveAssociateAccept(DicomClient client, AssociationParameters association);
        void OnReceiveRequestMessage(DicomClient client, byte presentationID, ushort messageID, DicomMessage message);
        void OnReceiveResponseMessage(DicomClient client, byte presentationID, ushort messageID, DcmStatus status, DicomMessage message);
        void OnReceiveReleaseResponse(DicomClient client);
        void OnReceiveAssociateReject(DicomClient client, DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason);
        void OnReceiveAbort(DicomClient client, DcmAbortSource source, DcmAbortReason reason);
    }
}
