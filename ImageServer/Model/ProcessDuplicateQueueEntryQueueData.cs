#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents the state of the work queue processing.
    /// </summary>
    public class ProcessDuplicateQueueState
    {
        public bool ExistingStudyUpdated { get; set; }

        public bool HistoryLogged { get; set; }
        
    }

    /// <summary>
    /// Represents the contents in the Data column of the <see cref="WorkQueue"/> entry.
    /// </summary>
    public class ProcessDuplicateQueueEntryQueueData
    {

        public ProcessDuplicateQueueEntryQueueData()
        {
            State = new ProcessDuplicateQueueState();
        }

        public ProcessDuplicateAction Action { get; set; }

        public string DuplicateSopFolder { get; set; }

        public ProcessDuplicateQueueState State { get; set; }

        public string UserName { get; set; }
    }
}