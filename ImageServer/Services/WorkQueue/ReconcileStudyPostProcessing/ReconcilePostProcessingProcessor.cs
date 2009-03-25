using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
    class ReconcilePostProcessingProcessor:StudyProcessItemProcessor
    {
        protected override void ProcessFile(ClearCanvas.ImageServer.Model.WorkQueueUid queueUid, string path, ClearCanvas.Dicom.Utilities.Xml.StudyXml stream)
        {
            DicomFile file = LoadDicomFile(path);
            if (StorageLocation.Study!=null)
            {
                DifferenceCollection list = StudyHelper.Compare(file, StorageLocation);
                if (list != null && list.Count > 0)
                {
                    Platform.Log(LogLevel.Warn, "Dicom file contains information inconsistent with the study in the system");
                }

            }
            
            InsertInstance(file, stream, queueUid);
        }
    }
}
