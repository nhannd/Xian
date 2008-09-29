using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{

    class DiscardImageCommandProcessor : ServerCommandProcessor, IReconcileProcessor
    {
        private ReconcileStudyProcessorContext _context;
        public DiscardImageCommandProcessor()
            : base("Discard Image")
        {

        }
        public string Name
        {
            get { return "Discard Image Processor"; }
        }
        #region IReconcileProcessor Members

        public void Initialize(ReconcileStudyProcessorContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;

            DiscardImagesCommand discard = new DiscardImagesCommand();
            discard.SetContext(_context);

            DeleteDirectoryCommand deleteDir = new DeleteDirectoryCommand(context.ReconcileWorkQueueData.StoragePath, true);
            AddCommand(discard);
            AddCommand(deleteDir);
        }

        #endregion
    }
}
