#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public interface IPublishFiles
    {
        void PublishLocal(ICollection<DicomFile> files);
        void PublishRemote(ICollection<DicomFile> files, IDicomServiceNode destinationServer);
    }
}
