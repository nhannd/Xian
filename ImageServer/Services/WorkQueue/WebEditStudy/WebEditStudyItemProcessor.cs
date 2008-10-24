using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{

    class WebEditStudyItemProcessor : BaseItemProcessor
    {
        protected override void ProcessItem(Model.WorkQueue item)
        {
            if (!LoadStorageLocation(item))
            {
                WorkQueueSettings settings = WorkQueueSettings.Instance;

                DateTime newScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                DateTime expire = newScheduledTime.AddSeconds(settings.WorkQueueExpireDelaySeconds);
                Platform.Log(LogLevel.Info, "Storage is not ready. Postponing {0} work queue entry (GUID={1}) to {2}",item.WorkQueueTypeEnum, item.GetKey(), newScheduledTime);
                PostponeItem(item, newScheduledTime, expire);
            }
            else
            {
                ServerFilesystemInfo filesystem =
                    FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);

                Debug.Assert(filesystem != null);

                if (!filesystem.Readable && !filesystem.Writeable)
                {
                    String reason = String.Format("Filesystem {0} is not readable and writable.", filesystem.Filesystem.Description);
                    FailQueueItem(item, reason);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Study Edit started. GUID={0}. Storage={1}", item.GetKey(), item.StudyStorageKey);
                    ServerPartition partition = ServerPartition.Load(item.ServerPartitionKey);
                    StudyStorage storage = StudyStorage.Load(item.StudyStorageKey);
                    StudyStorageLocation location = StudyStorageLocation.FindStorageLocations(storage)[0];

                    WebEditStudyCommandXmlParser parser = new WebEditStudyCommandXmlParser();

                    ServerCommandProcessor processor = new ServerCommandProcessor("Web Edit Study");
                    StatisticsSet statistics = null;
                    using (processor)
                    {
                        IList<IImageLevelUpdateCommand> updates = parser.ParseImageLevelCommands(item.Data.DocumentElement);
                        UpdateStudyCommand updateStudyCommand = new UpdateStudyCommand(partition, location, updates);
                        processor.AddCommand(updateStudyCommand);
                        if (processor.Execute())
                        {
                            Complete();
                            statistics = updateStudyCommand.Statistics;
                        }
                        else
                        {
                            FailQueueItem(WorkQueueItem, processor.FailureReason);
                            Platform.Log(LogLevel.Info, "Study Edit failed. GUID={0}. Reason={1}", WorkQueueItem.GetKey(), processor.FailureReason);
                        }
                    }

                    if (statistics != null)
                    {
                        StatisticsLogger.Log(LogLevel.Info, statistics);
                    }
                }
            }
        }


        private void Complete()
        {
            PostProcessing(WorkQueueItem, true, true, true);
            Platform.Log(LogLevel.Info, "Study Edit completed. GUID={0}", WorkQueueItem.GetKey());
        }

        protected override bool CanStart()
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
            return (relatedItems == null || relatedItems.Count == 0);
        }
    }
}
