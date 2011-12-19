#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Model
{

    /// <summary>
    /// Contains the info of the study associated with an <see cref="Alert"/>
    /// </summary>
    public class StudyAlertContextInfo
    {
        public string StudyInstanceUid { get; set; }
        public string ServerPartitionAE { get; set; }

        public StudyAlertContextInfo(){}

        public StudyAlertContextInfo(string serverPartitionAE, string studyInstanceUid)
        {
            ServerPartitionAE = serverPartitionAE;
            StudyInstanceUid = studyInstanceUid;
        }
    }
}
