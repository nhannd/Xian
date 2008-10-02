using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    class WebEditStudyItemProcessor : BaseItemProcessor
    {
        protected override void ProcessItem(Model.WorkQueue item)
        {
            LoadStorageLocation(item);

        
            Platform.Log(LogLevel.Info, "Study Edit started. GUID={0}. Storage={1}", item.GetKey(), item.StudyStorageKey);
            ServerPartition partition = ServerPartition.Load(item.ServerPartitionKey);
            StudyStorage storage = StudyStorage.Load(item.StudyStorageKey);
            StudyStorageLocation location = StudyStorageLocation.FindStorageLocations(storage)[0];

            WebEditStudyCommandXmlParser parser = new WebEditStudyCommandXmlParser();

            ServerCommandProcessor processor = new ServerCommandProcessor("Web Edit Study");
            using (processor)
            {
                IList<IImageLevelUpdateCommand> updates = parser.ParseImageLevelCommands(item.Data.DocumentElement);
                UpdateStudyCommand updateStudyCommand = new UpdateStudyCommand(partition, location, updates);
                processor.AddCommand(updateStudyCommand);
                if (processor.Execute())
                {
                    Complete();
                }
                else
                {
                    FailQueueItem(WorkQueueItem, processor.FailureReason);
                    Platform.Log(LogLevel.Info, "Study Edit failed. GUID={0}. Reason={1}", WorkQueueItem.GetKey(), processor.FailureReason);
                }
            }
        }


        private void Complete()
        {
            PostProcessing(WorkQueueItem, true, true);
            Platform.Log(LogLevel.Info, "Study Edit completed. GUID={0}", WorkQueueItem.GetKey());
        }

        protected override bool CannotStart()
        {
            Model.WorkQueue item = WorkQueueItem;

            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(item.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(
                new WorkQueueTypeEnum[]
                {
                    WorkQueueTypeEnum.StudyProcess,
                    WorkQueueTypeEnum.ReconcileStudy
                });
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending });

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item, workQueueCriteria);
            return (relatedItems != null && relatedItems.Count > 0);
        }
    }
}
