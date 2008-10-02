using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

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

            if (_context.History.DestStudyStorageKey == null)
            {
                StudyStorage storage = StudyStorage.Load(_context.WorkQueueItem.StudyStorageKey);
                MergeStudyCommandXmlParser parser = new MergeStudyCommandXmlParser();
                MergeStudyCommand command = new MergeStudyCommand();
                command.DestStudyStorage = StudyStorageLocation.FindStorageLocations(storage)[0];
                command.ImageLevelCommands.AddRange(
                    parser.ParseImageLevelCommands(_context.History.ChangeDescription.DocumentElement));

                command.UpdateDestination = true;
                command.SetContext(_context);

                AddCommand(command);
            }
            else
            {
                StudyStorage storage = StudyStorage.Load(_context.History.DestStudyStorageKey);
                MergeStudyCommandXmlParser parser = new MergeStudyCommandXmlParser();
                MergeStudyCommand command = new MergeStudyCommand();
                command.ImageLevelCommands.AddRange(
                    parser.ParseImageLevelCommands(_context.History.ChangeDescription.DocumentElement));

                IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(storage);
                command.DestStudyStorage = locations[0];
                command.UpdateDestination = false;
                command.SetContext(_context);

                AddCommand(command);
            }
        }

        #endregion
    }
    
}
