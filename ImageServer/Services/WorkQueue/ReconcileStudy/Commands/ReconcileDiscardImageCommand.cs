using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands
{
    /// <summary>
    /// Command for discarding images that need to be reconciled.
    /// </summary>
    class ReconcileDiscardImageCommand : ServerCommand, IReconcileServerCommand
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        #endregion

        #region Constructors
        public ReconcileDiscardImageCommand()
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
            Platform.Log(LogLevel.Debug, "Discarding image {0}", Context.ReconcileImage.Filename);
            File.Delete(Context.ReconcileImage.Filename);
        }

        protected override void OnUndo()
        {

        }
        #endregion
    }

    
}