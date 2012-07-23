#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    static class DicomStatusTranslator
    {
        /// <summary>
        /// Returns a user-friendly error message for a specified DICOM error
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string TranslateError(DicomStatus status)
        {
            if (status == DicomStatuses.Success) // don't translate this
                return null;

            if (status == DicomStatuses.QueryRetrieveMoveDestinationUnknown)
                return SR.DicomError_QRMoveDestinationUnknown;

            return status.Description;
        }
    }
}