using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    public interface IDicomServerHandler
    {
        void OnReceiveAssociateRequest(DicomServer server, AssociationParameters association);
        void OnReceiveRequestMessage(DicomServer server, byte presentationID, DicomMessage message);
        void OnReceiveResponseMessage(DicomServer server, byte presentationID, DicomMessage message);
        void OnReceiveReleaseRequest(DicomServer server);
        void OnReceiveAbort(DicomServer server, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomServer server, Exception e);
        void OnDimseTimeout(DicomServer server);
    }
}
