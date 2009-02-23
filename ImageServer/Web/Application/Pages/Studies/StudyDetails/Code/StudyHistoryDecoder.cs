using System;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Provides methods to decode the information in a <see cref="StudyHistory"/> record.
    /// </summary>
    static class StudyHistoryRecordDecoder
    {
        public static ReconcileHistoryRecord ReadReconcileRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled,
                               "History record has invalid history record type");

            ReconcileHistoryRecord record = new ReconcileHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            record.UpdateDescription = new ReconcileHistoryChangeDescription();

            if (historyRecord.ChangeDescription.DocumentElement.Name == "MergeStudy")
            {
                record.UpdateDescription.ReconcileAction = ReconcileAction.Merge;
            }
            else if (historyRecord.ChangeDescription.DocumentElement.Name == "CreateStudy")
            {
                record.UpdateDescription.ReconcileAction = ReconcileAction.SplitStudies;
            }
                
            
            switch(record.UpdateDescription.ReconcileAction )
            {
                case ReconcileAction.Merge:
                    {
                        MergeStudyCommandXmlParser parser = new MergeStudyCommandXmlParser();
                        record.UpdateDescription.UpdateCommands = parser.ParseImageLevelCommands(historyRecord.ChangeDescription.DocumentElement);
                        break;
                    }

                case ReconcileAction.SplitStudies:
                    {
                        CreateStudyCommandXmlParser parser = new CreateStudyCommandXmlParser();
                        record.UpdateDescription.UpdateCommands = parser.ParseImageLevelCommands(historyRecord.ChangeDescription.DocumentElement);
                        break;
                    }
                case ReconcileAction.Discard:
                    {
                        break;
                    }
            }

            return record;
        }

        public static WebEditStudyHistoryRecord ReadEditRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited,
                               "History record has invalid history record type");

            WebEditStudyHistoryRecord record = new WebEditStudyHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            record.UpdateDescription = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(historyRecord.ChangeDescription);
            return record;
        }
    }
    
    internal class StudyHistoryRecordBase
    {
        public DateTime InsertTime;
        public StudyStorageLocation StudyStorageLocation;

    }
}
