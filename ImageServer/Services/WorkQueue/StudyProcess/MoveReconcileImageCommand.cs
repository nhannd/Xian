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
            string path = Path.Combine(_context.StoragePath, file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty) + ".dcm");
            return path;
        }


        protected override void OnExecute()
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.StoragePath, "_context.StoragePath");

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
