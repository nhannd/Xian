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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute
{
    public class AutoRouteItemProcessor : BaseItemProcessor, IWorkQueueItemProcessor
    {
        private string _processorId;
        public string ProcessorID
        {
            get { return _processorId; }
            set { _processorId = value; }
        }

        public void ProcessWorkQueueUids(Model.WorkQueue item, ImageServerStorageScu _theScu)
        {
            LoadStorageLocation(item);
            string studyPath = _storageLocation.GetStudyPath();

            StudyXml studyXml = BaseScp.LoadStudyStream(_storageLocation);
            
            foreach (WorkQueueUid uid in _uidList)
            {
                _theScu.LoadInstanceFromStudyXml(studyPath, uid.SeriesInstanceUid, uid.SopInstanceUid, studyXml);
            }
        }

        public void Process(Model.WorkQueue item)
        {
            LoadUids(item);

            if(_uidList.Count == 0)
            {
                SetWorkQueueItemPending(item, false);
                return;
            }

            // Load remote device information fromt he database.
            Device device = Device.Load(item.DeviceKey);
            if (device == null)
            {
                Platform.Log(LogLevel.Error,
                             "Unknown auto-route destination \"{0}\"", item.DeviceKey);

                SetWorkQueueItemPending(item, true);
                return ;
            }

            ServerPartition partition = ServerPartition.Load(item.ServerPartitionKey);

            // Now setup the StorageSCU component
            ImageServerStorageScu scu = new ImageServerStorageScu(partition.AeTitle, device.AeTitle,
                                                                  device.IpAddress, device.Port);

            // set the preferred syntax lists
            scu.LoadPreferredSyntaxes(_readContext,device);

            // Load the Instances to Send into the SCU component
            ProcessWorkQueueUids(item, scu);

            scu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                                    {
                                        if (instance.SendStatus.Status == DicomState.Success
                                            || instance.SendStatus.Status == DicomState.Warning)
                                        {
                                            WorkQueueUid foundUid = null;
                                            foreach (WorkQueueUid uid in _uidList)
                                            {
                                                if (uid.SopInstanceUid.Equals(instance.SopInstanceUid))
                                                {
                                                    foundUid = uid;
                                                    break;
                                                }
                                            }
                                            if (foundUid != null)
                                            {
                                                DeleteWorkQueueUid(foundUid);
                                                _uidList.Remove(foundUid);
                                            }
                                        }
                                    };

            // Block until send is complete
            scu.Send();

            scu.Join();

            if (_uidList.Count > 0)
                SetWorkQueueItemPending(item, true); // failures occurred
            else
                SetWorkQueueItemPending(item, false); // no failures
        }

    }
}
