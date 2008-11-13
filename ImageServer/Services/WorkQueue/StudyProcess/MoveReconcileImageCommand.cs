#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    class MoveReconcileImageCommand : ServerCommand
    {
        private readonly ReconcileImageContext _context;
        private ServerCommandProcessor _processor;
    	private readonly bool _duplicate;

        public MoveReconcileImageCommand(ReconcileImageContext context, bool duplicate)
            : base("MoveReconcileImageCommand", true)
        {
            _context = context;
			_duplicate = duplicate;
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
            _processor.AddCommand(mkdirCommand);
			if (_duplicate)
			{
				CopyFileCommand copyCommand = new CopyFileCommand(src, dest);
				_processor.AddCommand(copyCommand);
			}
			else
			{
				RenameFileCommand moveCommand = new RenameFileCommand(src, dest);
				_processor.AddCommand(moveCommand);
			}
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
