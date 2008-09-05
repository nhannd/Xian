using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class MoveReconcileImageCommand : ServerCommand
    {
        private ReconcileImageContext _context;
        private ServerCommandProcessor _processor;

        public MoveReconcileImageCommand(ReconcileImageContext context)
            : base("MoveReconcileImageCommand", true)
        {
            _context = context;
            
        }

        private string GetReconcileImageTempPath(DicomFile file)
        {
            ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(_context.StudyLocation.FilesystemKey);

            if (!fs.Writeable)
            {
                throw new ApplicationException(String.Format("Unexpected error: temporary filesystem '{0}' is not writable", fs.Filesystem.Description));
            }

            string path = Path.Combine(fs.Filesystem.FilesystemPath, _context.StudyLocation.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, _context.ReconcileQueue.GetKey().Key.ToString());
            path = Path.Combine(path, _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty) + ".dcm");
            return path;
        }


        protected override void OnExecute()
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.ReconcileQueue, "_context.ReconcileQueue");

            _processor = new ServerCommandProcessor("Move Reconcile Image Processor");

            string src = _context.File.Filename;
            string dest = GetReconcileImageTempPath(_context.File);
            CreateDirectoryCommand mkdirCommand = new CreateDirectoryCommand(Directory.GetParent(dest).FullName);
            RenameFileCommand moveCommand = new RenameFileCommand(src, dest);
            _processor.AddCommand(mkdirCommand);
            _processor.AddCommand(moveCommand);

            if (!_processor.Execute())
            {
                throw new ApplicationException(
                    String.Format("Unable to move reconcile image to temporary folder: {0}", _processor.FailureReason));

            }
        }

        protected override void OnUndo()
        {
            if (_processor!=null)
            {
                _processor.Rollback();
                _processor = null;
            }
        }
    }
}
