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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{

    

    /// <summary>
    /// Command for discarding images that need to be reconciled.
    /// </summary>
    class DiscardImagesCommand : ReconcileCommandBase
    {

        #region Constructors
        public DiscardImagesCommand(ReconcileStudyProcessorContext context)
            : base("Discard image", false, context)
        {
        }
        #endregion

        #region Overidden Protected Methods

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(Context, "Context");
            Platform.CheckForNullReference(Context.ReconcileWorkQueueData, "Context.ReconcileWorkQueueData");

            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
            {
                string imagePath = Path.Combine(Context.ReconcileWorkQueueData.StoragePath, uid.SopInstanceUid + ".dcm");
                using(ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
                {
                    FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
                    DeleteWorkQueueUidCommand deleteUid = new DeleteWorkQueueUidCommand(uid);
                    processor.AddCommand(deleteFile);
                    processor.AddCommand(deleteUid);
                    Platform.Log(LogLevel.Info, deleteFile.ToString());
                    if (!processor.Execute())
                    {
                        throw new Exception(String.Format("Unable to discard image {0}", uid.SopInstanceUid));
                    } 
                }
                
            }
        }

       
        protected override void OnUndo()
        {

        }
        #endregion
    }
}