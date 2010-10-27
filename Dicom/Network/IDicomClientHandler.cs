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
