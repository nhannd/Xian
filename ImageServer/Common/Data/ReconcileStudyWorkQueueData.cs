using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents the information encoded in the <see cref="StudyIntegrityQueue.QueueData"/> column of a <see cref="StudyIntegrityQueue"/> record.
    /// </summary>
    public class ReconcileStudyWorkQueueData
    {
        private string _storagePath;
        private ImageSetDetails _details;

        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        public ImageSetDetails Details
        {
            get { return _details; }
            set { _details = value; }
        }
    }
}
