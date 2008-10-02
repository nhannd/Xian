using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
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

            if (_context.History.DestStudyStorageKey == null)
            {
                CreateStudyCommandXmlParser parser = new CreateStudyCommandXmlParser();
                CreateStudyCommand command = new CreateStudyCommand();
                command.SetContext(_context);
                command.ImageLevelCommands.AddRange(
                    parser.ParseImageLevelCommands(_context.History.ChangeDescription.DocumentElement));

                command.DestStudyStorage = null; // to be created
                
                AddCommand(command);
            }
            else
            {

                StudyStorage storage = StudyStorage.Load(_context.History.DestStudyStorageKey);
                MergeStudyCommand command = new MergeStudyCommand();
                command.SetContext(_context);
                
                IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(storage);
                command.DestStudyStorage = locations[0];
                command.UpdateDestination = false;

                AddCommand(command);
            }
        }

        #endregion

        
    }
    
}
