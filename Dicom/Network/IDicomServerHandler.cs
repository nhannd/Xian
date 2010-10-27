#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

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
