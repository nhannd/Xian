using System.IO;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    class TierMigrateMoveStudyFolderCommand:ServerCommand
    {
        #region Private Memeber
        private TierMigrationContext _context = null;
        private ServerCommandProcessor _processor;
        private bool _undo = false;
        #endregion

        public TierMigrateMoveStudyFolderCommand(TierMigrationContext context)
            : base("TierMigrateMoveStudyFolderCommand", true)
        {
            _context = context;
            _processor = new ServerCommandProcessor("TierMigrateMoveStudyFolderCommand");
        }

        public TierMigrationContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        public override void Dispose()
        {
            if (_processor != null)
                _processor.Dispose();

            if (!_undo)
            {
                DirectoryUtility.DeleteIfExists(Context.OriginalStudyLocation.GetStudyPath(), true);
            }
            base.Dispose();
        }

        protected override void OnExecute()
        {
            if (Context!=null)
            {
                string origFolder = Context.OriginalStudyLocation.GetStudyPath();
                string newFolder = Path.Combine(Context.Destination.Filesystem.FilesystemPath, Context.OriginalStudyLocation.PartitionFolder);
                newFolder = Path.Combine(newFolder, Context.OriginalStudyLocation.StudyFolder);
                newFolder = Path.Combine(newFolder, Context.OriginalStudyLocation.StudyInstanceUid);

                CreateDirectoryCommand mkdir = new CreateDirectoryCommand(newFolder);
                CopyDirectoryCommand copy = new CopyDirectoryCommand(origFolder, newFolder);
                _processor.AddCommand(mkdir);
                _processor.AddCommand(copy);

                _processor.Execute();
            }
        }

        protected override void OnUndo()
        {
            if (_processor != null)
            {
                _processor.Rollback();
                _processor = null;
            }

            _undo = true;
        }
    }
}
