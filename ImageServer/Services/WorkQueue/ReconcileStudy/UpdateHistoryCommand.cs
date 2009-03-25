using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Command to update the study history record
    /// </summary>
    class UpdateHistoryCommand : ServerDatabaseCommand<ReconcileStudyProcessorContext>
    {
        public UpdateHistoryCommand(ReconcileStudyProcessorContext context)
            : base("UpdateHistoryCommand", true, context)
        {
            
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            IStudyHistoryEntityBroker historyUpdateBroker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
            StudyHistoryUpdateColumns parms = new StudyHistoryUpdateColumns();
            parms.DestStudyStorageKey = Context.DestStorageLocation.GetKey();
            historyUpdateBroker.Update(Context.History.GetKey(), parms);

            ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
            LockStudyParameters lockParms = new LockStudyParameters();
            lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ProcessingScheduled;
            lockParms.StudyStorageKey = Context.WorkQueueItem.StudyHistoryKey;
            lockStudyBroker.Execute(lockParms);
        }

    }
}