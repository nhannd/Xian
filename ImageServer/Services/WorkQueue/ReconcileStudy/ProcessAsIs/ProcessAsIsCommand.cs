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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.ProcessAsIs
{
    internal class ProcessAsIsCommand : ReconcileCommandBase
    {
        private readonly CommandParameters _parameters;

        /// <summary>
        /// Represents parameters passed to <see cref="ProcessAsIsCommand"/>
        /// </summary>
        internal class CommandParameters
        {
        }

    
        /// <summary>
        /// Creates an instance of <see cref="ProcessAsIsCommand"/>
        /// </summary>
        public ProcessAsIsCommand(ReconcileStudyProcessorContext context, CommandParameters parms)
            : base("Process As-is Command", true, context)
        {
            _parameters = parms;
        }

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(_parameters, "_parameters");

            if (Context.DestStorageLocation==null)
            {
                DetermineTargetLocation();
            }

            ProcessUidList();
        }

        private void DetermineTargetLocation()
        {
            if (Context.History.DestStudyStorageKey!=null)
            {
                Context.DestStorageLocation =
                    StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.StudyStorageKey))[0];

            }
            else
            {
                Context.DestStorageLocation = Context.WorkQueueItemStudyStorage;
            }
        }

        protected override void OnUndo()
        {
            // undo is done  in SaveFile()
        }

        private void ProcessUidList()
        {
            int counter = 0;
            Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
                {
                    string imagePath = GetReconcileUidPath(uid);
                    DicomFile file = new DicomFile(imagePath);
                    file.Load();

                    processor.AddCommand(new SaveFileCommand(Context, file));
                    UpdateWorkQueueCommand.CommandParameters parameters = new UpdateWorkQueueCommand.CommandParameters();
                    parameters.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                    parameters.SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
                    parameters.Extension = "dcm";
                    parameters.IsDuplicate = false;
                    processor.AddCommand(new UpdateWorkQueueCommand(Context, parameters));
                    processor.AddCommand(new FileDeleteCommand(GetReconcileUidPath(uid), true));
                    processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                    if (counter == 0)
                    {
                        processor.AddCommand(new UpdateHistoryCommand(Context));
                    }
                    
                    if (!processor.Execute())
                    {
                        FailUid(uid, true);
                        throw new ApplicationException(String.Format("Unable to reconcile image {0} : {1}", file.Filename, processor.FailureReason));
                    }

                }

                counter++;
                Platform.Log(LogLevel.Info, "Reconciled SOP {0} (not yet processed) [{1} of {2}]", uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
            }
        }
    }


}
