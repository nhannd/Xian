using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    /// <summary>
    /// A processor implementing <see cref="IReconcileProcessor"/> to handle "MergeStudy" operation
    /// </summary>
    class MergeStudyCommandProcessor : ServerCommandProcessor, IReconcileProcessor
    {
        private ReconcileStudyProcessorContext _context;
        public MergeStudyCommandProcessor()
            : base("Merge Study")
        {

        }

        public string Name
        {
            get { return "Merge Study Processor"; }
        }
    
        #region IReconcileProcessor Members

        public void Initialize(ReconcileStudyProcessorContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
            MergeStudyCommandXmlParser parser = new MergeStudyCommandXmlParser();
            ReconcileMergeToExistingStudyDescriptor desc = parser.Parse(_context.History.ChangeDescription);
                
            if (_context.History.DestStudyStorageKey == null)
            {
                ReconcileMergeStudyCommandParameters parameters = new ReconcileMergeStudyCommandParameters();
                parameters.UpdateDestination = true;
                parameters.Commands = desc.Commands;
                MergeStudyCommand command = new MergeStudyCommand(_context, parameters);
                AddCommand(command);
            }
            else
            {
                ReconcileMergeStudyCommandParameters parameters = new ReconcileMergeStudyCommandParameters();
                parameters.UpdateDestination = false; // the target study has been assigned (ie, this entry has been excecuted at least once), we don't need to update the study again (for performance reason).
                parameters.Commands = desc.Commands;
                MergeStudyCommand command = new MergeStudyCommand(_context, parameters);
                AddCommand(command);
            }
        }

        #endregion
    }
    
}
