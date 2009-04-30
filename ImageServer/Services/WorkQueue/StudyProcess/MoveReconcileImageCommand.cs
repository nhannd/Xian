#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Diagnostics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class OpValidationCommand:ServerCommand
    {
        private readonly ReconcileImageContext _context;
        public OpValidationCommand(ReconcileImageContext context)
            : base("Validating result of the operation", true)
        {
            _context = context;
        }

        protected override void OnExecute()
        {
            string reconcileImagePath =
                Path.Combine(_context.StoragePath, _context.File.DataSet[DicomTags.SopInstanceUid].ToString() + ".dcm");

            Platform.CheckTrue(File.Exists(reconcileImagePath), "File was not copied to Reconcile folder properly");

        }

        protected override void OnUndo()
        {
            
        }
    }


    class MoveReconcileImageCommand : ServerCommand, IDisposable
    {
        private readonly ReconcileImageContext _context;
        private ServerCommandProcessor _processor;
        private string _src;
        private string _dest;

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
            
            _src = _context.File.Filename;
            _dest = GetReconcileImageTempPath(_context.File);
            
            CreateDirectoryCommand mkdirCommand = new CreateDirectoryCommand(Directory.GetParent(_dest).FullName);
            _processor.AddCommand(mkdirCommand);
            
            RenameFileCommand moveCommand = new RenameFileCommand(_src, _dest);
            _processor.AddCommand(moveCommand);
            
            if (!_processor.Execute())
            {
                throw new ApplicationException(
                    String.Format("Unable to process image. Unable to move image to Reconcile folder: {0}", _processor.FailureReason));
            }

            SimulatePostOperationError();
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

        [Conditional("DEBUG_SIM_ERRORS")]
        private void SimulatePostOperationError()
        {
            RandomError.Generate(Settings.SimulateTierMigrationError, "Post MoveReconcileImageCommand error : file is deleted by another process", delegate { File.Delete(_dest); });
            RandomError.Generate(Settings.SimulateTierMigrationError, "Post MoveReconcileImageCommand Exception");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_processor != null)
            {
                _processor.Dispose();
                _processor = null;
            }
        }

        #endregion
    }
}
