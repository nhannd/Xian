#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy
{
    public class DeleteStudyItemProcessor : BaseItemProcessor, IWorkQueueItemProcessor
    {
        #region Private Members
        private string _processorId;
        #endregion

        #region Private Methods
        private void RemoveFilesystem()
        {
            foreach (StudyStorageLocation location in StorageLocationList )
            {
                string path = location.GetStudyPath();
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when trying to delete directory: {0}", path);
                }
            }
        }

        private void RemoveDatabase(Model.WorkQueue item)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // Setup the delete parameters
                StudyStorageDeleteParameters parms = new StudyStorageDeleteParameters();

                parms.ServerPartitionKey = item.ServerPartitionKey;
                parms.StudyStorageKey = item.StudyStorageKey;

                // Get the Insert Instance broker and do the insert
                IDeleteStudyStorage delete = updateContext.GetBroker<IDeleteStudyStorage>();

                if (false == delete.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0}",
                                 item.StudyStorageKey);
                }
                else
                    updateContext.Commit();
            }
        }
        #endregion

        #region IWorkQueueItemProcessor Members

        public string ProcessorID
        {
            get { return _processorId; }
            set { _processorId = value; }
        }

        public void Process(Model.WorkQueue item)
        {
            //Load the storage location.
            LoadStorageLocation(item);

            RemoveFilesystem();

            RemoveDatabase(item);

            // No need to remove / update the Queue entry, it was deleted as part of the delete process.
        }

        #endregion
    }
}
