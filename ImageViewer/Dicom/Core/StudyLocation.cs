#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    public class StudyLocation
    {
        public StudyLocation(DicomMessageBase message)
        {
            Study = new StudyIdentifier(message.DataSet);

            StudyFolder = Path.Combine("", Study.StudyInstanceUid);            
        }

        public string StudyFolder
        {
            get;private set;
        }

        public StudyIdentifier Study { get; set; }

        public string GetSopInstancePath(string seriesInstanceUid, string sopInstanceUid)
        {
            return Path.Combine(StudyFolder, 
                string.Format("{0}.{1}", sopInstanceUid, "dcm"));
        }
    }
}
