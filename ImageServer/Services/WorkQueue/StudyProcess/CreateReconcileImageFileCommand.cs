using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class CreateReconcileImageFileCommand : ServerCommand
    {
        private readonly ReconcileImageContext _context;
        private ServerCommandProcessor _processor;

        public CreateReconcileImageFileCommand(ReconcileImageContext context)
            : base("CreateReconcileImageFile", false)
        {
            _context = context;
            _processor = new ServerCommandProcessor("CreateReconcileImageFile");
        }


        protected override void OnExecute()
        {
            string root = _context.FilesystemInfo.Filesystem.FilesystemPath;
            string path = Path.Combine(root, _context.StudyLocation.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, _context.ReconcileRecord.GetKey().Key.ToString());
            path = Path.Combine(path, _context.SopInstanceUid + ".dcm");
            _context.DestImagePath = path;

            CreateDirectoryCommand mkdir = new CreateDirectoryCommand(Directory.GetParent(_context.DestImagePath).FullName);
            RenameFileCommand moveFile = new RenameFileCommand(_context.SourceImagePath, _context.DestImagePath);
            _processor.AddCommand(mkdir);
            _processor.AddCommand(moveFile);

            if (!_processor.Execute())
            {
                throw new ApplicationException(_processor.FailureReason);
            }
        }

        protected override void OnUndo()
        {
            if (_processor!=null)
            {
                _processor.Rollback();
                _processor.Dispose();
                _processor = null;
            }
        }
    }
}
