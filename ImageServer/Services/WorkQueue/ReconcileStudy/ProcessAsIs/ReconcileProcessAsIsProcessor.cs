using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.ProcessAsIs
{
    /// <summary>
    /// A processor implementing <see cref="IReconcileProcessor"/> to handle "MergeStudy" operation
    /// </summary>
    internal class ReconcileProcessAsIsProcessor : ServerCommandProcessor, IReconcileProcessor
    {
        private ReconcileStudyProcessorContext _context;
        public ReconcileProcessAsIsProcessor()
            : base("Process As Is")
        {

        }

        public string Name
        {
            get { return "Process As Is Processor"; }
        }

        #region IReconcileProcessor Members

        public void Initialize(ReconcileStudyProcessorContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
            _context.DestStorageLocation = context.WorkQueueItemStudyStorage;

            ProcessAsIsCommand.CommandParameters parms = new ProcessAsIsCommand.CommandParameters();
            ProcessAsIsCommand command = new ProcessAsIsCommand(_context, parms);

            AddCommand(command);
        }

        #endregion
    }
    
}
