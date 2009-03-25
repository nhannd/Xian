using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{

    

    /// <summary>
    /// Command for discarding images that need to be reconciled.
    /// </summary>
    class DiscardImagesCommand : ReconcileCommandBase
    {

        #region Constructors
        public DiscardImagesCommand(ReconcileStudyProcessorContext context)
            : base("Discard image", false, context)
        {
        }
        #endregion

        #region Overidden Protected Methods

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(Context.ReconcileWorkQueueData, "Context.ReconcileWorkQueueData");

            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                string imagePath = Path.Combine(Context.ReconcileWorkQueueData.StoragePath, uid.SopInstanceUid + ".dcm");
                using(ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
                {
                    FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
                    DeleteWorkQueueUidCommand deleteUid = new DeleteWorkQueueUidCommand(uid);
                    processor.AddCommand(deleteFile);
                    processor.AddCommand(deleteUid);
                    Platform.Log(LogLevel.Info, deleteFile.ToString());
                    if (!processor.Execute())
                    {
                        throw new Exception(String.Format("Unable to discard image {0}", uid.SopInstanceUid));
                    } 
                }
                
            }
        }

       
        protected override void OnUndo()
        {

        }
        #endregion
    }
}