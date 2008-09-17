using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands
{
    /// <summary>
    /// Command for updating content of an image for reconciliation.
    /// </summary>
    /// <remark>
    /// <see cref="ReconcileUpdateDicomFileCommand"/> is composed of one or more <see cref="IDicomFileUpdateCommandAction"/>
    /// which specify the action(s) to be applied to the image.
    /// DicomFileUpdateCommandActionList
    /// </remark>
    public class ReconcileUpdateDicomFileCommand : UpdateDicomFileCommand, IReconcileServerCommand
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ReconcileUpdateDicomFileCommand"/> with the specified actions.
        /// </summary>
        /// <param name="actions"></param>
        public ReconcileUpdateDicomFileCommand(DicomFileUpdateCommandActionList actions)
            : base(actions)
        {
        }
        #endregion


        public ReconcileStudyProcessorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        protected override void OnExecute()
        {
            base.DicomFile = Context.ReconcileImage;
            base.OnExecute();
        }
    }

    
}