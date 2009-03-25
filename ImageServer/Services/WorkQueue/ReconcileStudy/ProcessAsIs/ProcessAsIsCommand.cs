using System;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.ProcessAsIs
{
    internal class ProcessAsIsCommand : ReconcileCommandBase
    {
        private readonly CommandParameters _parameters;

        /// <summary>
        /// Represents parameters passed to <see cref="ProcessAsIsCommand"/>
        /// </summary>
        internal class CommandParameters
        {
        }

    
        /// <summary>
        /// Creates an instance of <see cref="ProcessAsIsCommand"/>
        /// </summary>
        public ProcessAsIsCommand(ReconcileStudyProcessorContext context, CommandParameters parms)
            : base("Process As-is Command", true, context)
        {
            _parameters = parms;
        }

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(_parameters, "_parameters");

            if (Context.DestStorageLocation==null)
            {
                DetermineTargetLocation();
            }

            ProcessUidList();
        }

        private void DetermineTargetLocation()
        {
            if (Context.History.DestStudyStorageKey!=null)
            {
                Context.DestStorageLocation =
                    StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.StudyStorageKey))[0];

            }
            else
            {
                Context.DestStorageLocation = Context.WorkQueueItemStudyStorage;
            }
        }

        protected override void OnUndo()
        {
            // undo is done  in SaveFile()
        }

        private void ProcessUidList()
        {
            int counter = 0;
            Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
                {
                    string imagePath = GetReconcileUidPath(uid);
                    DicomFile file = new DicomFile(imagePath);
                    file.Load();

                    processor.AddCommand(new SaveFileCommand(Context, file));
                    UpdateWorkQueueCommand.CommandParameters parameters = new UpdateWorkQueueCommand.CommandParameters();
                    parameters.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    parameters.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    parameters.Extension = "dcm";
                    parameters.IsDuplicate = false;
                    processor.AddCommand(new UpdateWorkQueueCommand(Context, parameters));
                    processor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
                    processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                    if (counter == 0)
                    {
                        processor.AddCommand(new UpdateHistoryCommand(Context));
                    }
                    
                    if (!processor.Execute())
                    {
                        FailUid(uid, true);
                        throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                    }

                }

                counter++;
                Platform.Log(LogLevel.Info, "Reconciled SOP {0} (not yet processed) [{1} of {2}]", uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
            }
        }
    }


}
