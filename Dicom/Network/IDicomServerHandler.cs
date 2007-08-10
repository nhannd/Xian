using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public interface IDicomServerHandler
    {
        void OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association);
        void OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association);
        void OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e);
        void OnDimseTimeout(DicomServer server, ServerAssociationParameters association);
    }
}
