using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
    /// <summary>
    /// A processor implementing <see cref="IReconcileProcessor"/> to handle "CreateStudy" operation
    /// </summary>
    class ReconcileCreateStudyProcessor : ServerCommandProcessor, IReconcileProcessor
    {
        #region Private Members
        private ReconcileStudyProcessorContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an instance of <see cref="ReconcileCreateStudyProcessor"/>
        /// </summary>
        public ReconcileCreateStudyProcessor()
            : base("Create Study")
        {

        }

        #endregion

        #region IReconcileProcessor Members


        public string Name
        {
            get { return "Create Study Processor"; }
        }

        public void Initialize(ReconcileStudyProcessorContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
            CreateStudyCommandXmlParser parser = new CreateStudyCommandXmlParser();
            ReconcileCreateStudyDescriptor desc = parser.Parse(_context.History.ChangeDescription);

            if (_context.History.DestStudyStorageKey == null)
            {
                CreateStudyCommand.CommandParameters paramaters = new CreateStudyCommand.CommandParameters();
                paramaters.Commands = desc.Commands;
                CreateStudyCommand command = new CreateStudyCommand(context, paramaters);
                AddCommand(command);
            }
            else
            {
                ReconcileMergeStudyCommandParameters parameters = new ReconcileMergeStudyCommandParameters();
                parameters.Commands = desc.Commands;
                parameters.UpdateDestination = false;
                MergeStudyCommand command = new MergeStudyCommand(_context, parameters);
                AddCommand(command);
            }
        }

        #endregion

        
    }
    
}
