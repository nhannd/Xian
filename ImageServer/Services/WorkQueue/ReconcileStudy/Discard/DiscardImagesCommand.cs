using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{
    class DeleteReconcileQueueUidCommand : ServerDatabaseCommand
    {
        private WorkQueueUid _uid;
        #region Constructors
        public DeleteReconcileQueueUidCommand(WorkQueueUid uid)
            : base("Delete uid", true)
        {
            _uid = uid;
        }
        #endregion

        protected override void OnExecute(IUpdateContext updateContext)
        {
            if (_uid!=null)
            {
                IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                delete.Delete(_uid.GetKey());
            }   
        }

        public override string ToString()
        {
            return String.Format("Delete Reconcile Uid. Uid={0}", _uid != null ? _uid.SopInstanceUid:"Unknown");
        }
    }
    /// <summary>
    /// Command for discarding images that need to be reconciled.
    /// </summary>
    class DiscardImagesCommand : ServerCommand, IReconcileServerCommand
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        #endregion

        #region Constructors
        public DiscardImagesCommand()
            : base("Discard image", false)
        {
        }
        #endregion


        #region IReconcileServerCommand Members

        public ReconcileStudyProcessorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion

        #region Overidden Protected Methods

        protected override void OnExecute()
        {
            foreach (WorkQueueUid uid in _context.WorkQueueUidList)
            {
                string imagePath = Path.Combine(_context.ReconcileWorkQueueData.StoragePath, uid.SopInstanceUid + ".dcm");
                ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid));
                FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
                DeleteReconcileQueueUidCommand deleteUid = new DeleteReconcileQueueUidCommand(uid);
                processor.AddCommand(deleteFile);
                processor.AddCommand(deleteUid);
                Platform.Log(LogLevel.Info, deleteFile.ToString());
                if (!processor.Execute())
                {
                    throw new Exception(String.Format("Unable to discard image {0}", uid.SopInstanceUid));
                }
            }
        }

       
        protected override void OnUndo()
        {

        }
        #endregion

        public void SetContext(ReconcileStudyProcessorContext context)
        {
            _context = context;
        }
    }
}