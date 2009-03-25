using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    internal abstract class ReconcileCommandBase : ServerCommand<ReconcileStudyProcessorContext>, IReconcileServerCommand, IDisposable
    {
        
        /// <summary>
        /// Creates an instance of <see cref="ServerCommand"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="requiresRollback"></param>
        /// <param name="context"></param>
        public ReconcileCommandBase(string description, bool requiresRollback, ReconcileStudyProcessorContext context)
            : base(description, requiresRollback, context)
        {
        }

        protected string GetReconcileUidPath(WorkQueueUid sop)
        {
            string imagePath = Path.Combine(Context.ReconcileWorkQueueData.StoragePath, sop.SopInstanceUid + ".dcm");
            return imagePath;
        }

        
        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                DirectoryUtility.DeleteIfEmpty(Context.ReconcileWorkQueueData.StoragePath);
            }
            catch(IOException ex)
            {
                Platform.Log(LogLevel.Warn, ex, "Unable to cleanup {0}", Context.ReconcileWorkQueueData.StoragePath);
            }
        }

        #endregion

        protected static void FailUid(WorkQueueUid sop, bool retry)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
                if (!retry)
                    columns.Failed = true;
                else
                {
                    if (sop.FailureCount >= WorkQueueSettings.Instance.WorkQueueMaxFailureCount)
                    {
                        columns.Failed = true;
                    }
                    else
                    {
                        columns.FailureCount = sop.FailureCount++;
                    }
                }

                uidUpdateBroker.Update(sop.GetKey(), columns);
                updateContext.Commit();
            }
        }
    }
}