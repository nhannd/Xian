using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public interface IDicomServerHandler
    {
        void OnClientConnected(DicomServer server);
        void OnClientClosed(DicomServer server);
        void OnNetworkError(DicomServer server, Exception e);
        void OnReceiveAssociateRequest(DicomServer server, AssociationParameters association);
        void OnReceiveRequestMessage(DicomServer server, byte presentationID, ushort messageID, DicomMessage message);
        void OnReceiveResponseMessage(DicomServer server, byte presentationID, ushort messageID, DcmStatus status, DicomMessage message);
        void OnReceiveReleaseRequest(DicomServer server);
        void OnReceiveAbort(DicomServer server, DcmAbortSource source, DcmAbortReason reason);
    }
}
