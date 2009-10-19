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
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Dicom;
using ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute
{
    /// <summary>
    /// Processor for 'AutoRoute <see cref="WorkQueue"/> entries
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class AutoRouteItemProcessor : BaseItemProcessor, ICancelable
    {
        private const int TEMP_BLACKOUT_DURATION = 5; // seconds
        // cache the list of device that has exceeded the limit
        // to prevent repeat db hits when a server processes 
        // multiple study move entries to the same device
        private static readonly ServerCache<ServerEntityKey, Device> _tempBlackoutDevices = new ServerCache<ServerEntityKey, Device>(TimeSpan.FromSeconds(TEMP_BLACKOUT_DURATION), TimeSpan.FromSeconds(TEMP_BLACKOUT_DURATION));

        #region Private Members
        private readonly object _syncLock = new object();
        private Dictionary<string, List<WorkQueueUid>> _uidMaps = null;
        private Device _device;
        private const short UNLIMITED = -1;
        #endregion

        #region Virtual Protected Methods


        /// <summary>
        /// Convert Uids into SopInstance
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<StorageInstance> GetStorageInstanceList()
        {
            string studyPath = StorageLocation.GetStudyPath();
            StudyXml studyXml = LoadStudyXml(StorageLocation);

            Dictionary<string, StorageInstance> list = new Dictionary<string, StorageInstance>();
            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                if (list.ContainsKey(uid.SopInstanceUid))
                {
                    Platform.Log(LogLevel.Warn, "WorkQueueUid {0} is a duplicate.", uid.Key);
                    continue; // duplicate;}
                }
                SeriesXml seriesXml = studyXml[uid.SeriesInstanceUid];
                InstanceXml instanceXml = seriesXml[uid.SopInstanceUid];
            
                string seriesPath = Path.Combine(studyPath, uid.SeriesInstanceUid);
                string instancePath = Path.Combine(seriesPath, uid.SopInstanceUid + ".dcm");
                StorageInstance instance = new StorageInstance(instancePath);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
        	    instance.StudyInstanceUid = studyXml.StudyInstanceUid;
        	    instance.PatientId = studyXml.PatientId;
			    instance.PatientsName = studyXml.PatientsName;

                list.Add(uid.SopInstanceUid, instance);
            }

            return list.Values;
        }

        /// <summary>
        /// Called when all instances have been sent
        /// </summary>
        protected virtual void OnComplete()
        {
            PostProcessing(WorkQueueItem,
                           WorkQueueProcessorStatus.Pending, // will go to Idle the next time around if there's no item left.
                           WorkQueueProcessorDatabaseUpdate.None);
        }

        protected virtual void OnInstanceSent(StorageInstance instance)
        {
            List<WorkQueueUid> foundUids = FindWorkQueueUids(instance);

            if (instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
            {
                WorkQueueItem.FailureDescription =
                    String.Format("SOP Class not supported by remote device: {0}",
                                  instance.SopClass.Name);
                Platform.Log(LogLevel.Warn,
                             "Unable to transfer SOP Instance, SOP Class is not supported by remote device: {0}",
                             instance.SopClass.Name);
            }

            if (instance.SendStatus.Status == DicomState.Failure)
            {
                WorkQueueItem.FailureDescription = instance.SendStatus.Description;
                foreach(WorkQueueUid uid in foundUids)
                {
                    uid.FailureCount++;
                    UpdateWorkQueueUid(uid);
                }
            }
            else if (foundUids!=null)
            {
                foreach (WorkQueueUid uid in foundUids)
                {
                    DeleteWorkQueueUid(uid);
                    WorkQueueUidList.Remove(uid);
                }
            }


        }

        #endregion

        #region Protected Properties

        protected IList<StorageInstance> InstanceList { get; set; }

        
        protected Device DestinationDevice
        {
            get
            {
                lock(_syncLock)
                {
                    if (_device==null)
                    {
                        _device = Device.Load(ReadContext, WorkQueueItem.DeviceKey);
                    }
                }

                return _device;
            }
        }
        #endregion

        #region Overridden Protected Method

        protected override void Initialize(Model.WorkQueue item)
        {
            base.Initialize(item);

            LoadStorageLocation(item);
            LoadUids(item);
            InstanceList = new List<StorageInstance>(GetStorageInstanceList());
        }

        public bool HasPendingItems
        {
            get { return InstanceList != null && InstanceList.Count > 0; }
        }

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item of type AutoRoute.
        /// </summary>
        protected override void ProcessItem(Model.WorkQueue item)
        {
            if (WorkQueueItem.ScheduledTime >= WorkQueueItem.ExpirationTime)
            {
                Platform.Log(LogLevel.Debug, "Removing Idle {0} entry : {1}", item.WorkQueueTypeEnum, item.GetKey().Key);
                base.PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.None);
                return;
            }

            if (!HasPendingItems)
            {
                // nothing to process, change to idle state
                PostProcessing(item, WorkQueueProcessorStatus.Idle, WorkQueueProcessorDatabaseUpdate.None);
                return;
            }

            Platform.Log(LogLevel.Info,
						 "Moving study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4} to {5}...",
						 Study.StudyInstanceUid, Study.PatientsName, Study.PatientId, Study.AccessionNumber,
						 ServerPartition.Description, DestinationDevice.AeTitle);

            // Load remote device information from the database.
            Device device = DestinationDevice;
            if (device == null)
            {
                item.FailureDescription = String.Format("Unknown auto-route destination \"{0}\"", item.DeviceKey);
                Platform.Log(LogLevel.Error, item.FailureDescription);

                PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal Error
                return;
            }

            if (device.Dhcp && device.IpAddress.Length == 0)
            {
                item.FailureDescription = String.Format("Auto-route destination is a DHCP device with no known IP address: \"{0}\"", device.AeTitle);
                Platform.Log(LogLevel.Error,
                             item.FailureDescription);

                PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal error
                return;
            }

            
            // Now setup the StorageSCU component
            int sendCounter = 0;
            ImageServerStorageScu scu = new ImageServerStorageScu(ServerPartition, device);

            // set the preferred syntax lists
            scu.LoadPreferredSyntaxes(ReadContext);

            // Load the Instances to Send into the SCU component
            scu.AddStorageInstanceList(InstanceList);

            // Set an event to be called when each image is transferred
            scu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                                    {
                                        if (instance.SendStatus.Status == DicomState.Success
                                            || instance.SendStatus.Status == DicomState.Warning
                                            || instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
                                        {
                                            sendCounter++;
                                            OnInstanceSent(instance);
                                        }

                                        if (instance.SendStatus.Status == DicomState.Failure)
                                        {
                                            scu.FailureDescription = instance.SendStatus.Description;
                                            if (false == String.IsNullOrEmpty(instance.ExtendedFailureDescription))
                                            {
                                                scu.FailureDescription = String.Format("{0} [{1}]", scu.FailureDescription, instance.ExtendedFailureDescription);
                                            }
                                        }
                                            

                                        if (CancelPending && !(this is WebMoveStudyItemProcessor) && !scu.Canceled)
                                        {
                                            Platform.Log(LogLevel.Info, "Auto-route canceled due to shutdown for study: {0}", StorageLocation.StudyInstanceUid);
                                            item.FailureDescription = "Operation was canceled due to server shutdown request.";
                                            scu.Cancel();
                                        }
                                    };

            try
            {
                // Block until send is complete
                scu.Send();

                // Join for the thread to exit
                scu.Join();

                // Dispose to cleanup properly
                scu.Dispose();
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error occurs while sending images to {0} : {1}", device.AeTitle, ex.Message);
            }
            finally
            {
                if (scu.FailureDescription.Length > 0)
                {
                    item.FailureDescription = scu.FailureDescription;
                    scu.Status = ScuOperationStatus.Failed;
                }

                // Reset the WorkQueue entry status
                if ((InstanceList.Count > 0 && sendCounter != InstanceList.Count) // not all sop were sent
                    || scu.Status == ScuOperationStatus.Failed
                    || scu.Status == ScuOperationStatus.ConnectFailed)
                {
                    PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal); // failures occurred}
                }
                else
                {
                    OnComplete();
                }
            }
            
        }

        protected override bool CanStart()
        {
            if (InstanceList == null || InstanceList.Count == 0)
                return true;

            if (DeviceIsBusy(DestinationDevice))
            {
                DateTime newScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.ProcessDelaySeconds);
                PostponeItem(WorkQueueItem, newScheduledTime, newScheduledTime.AddSeconds(WorkQueueProperties.ExpireDelaySeconds),
                             "Devices is busy. Max connection limit has been reached for the device");
                return false;
            }

            return true;
        }

        private bool DeviceIsBusy(Device device)
        {
            bool busy = false;
            if (device.ThrottleMaxConnections != UNLIMITED)
            {
                if (_tempBlackoutDevices.ContainsKey(device.Key))
                {
                    busy = true;
                }
                else
                {
                    List<Model.WorkQueue> currentMoves;

                    using(IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        currentMoves = device.GetAllCurrentMoveEntries(context);
                    }

                    if (currentMoves!=null && currentMoves.Count > device.ThrottleMaxConnections)
                    {
                        // sort the list to see where this entry is
                        // and postpone it if its position is greater than the ThrottleMaxConnections 
                        currentMoves.Sort(delegate(Model.WorkQueue item1, Model.WorkQueue item2)
                                              {
                                                  return item1.ScheduledTime.CompareTo(item2.ScheduledTime);
                                              });
                        int index = currentMoves.FindIndex(delegate(Model.WorkQueue item) { return item.Key.Equals(WorkQueueItem.Key); });

                        if (index >= device.ThrottleMaxConnections)
                        {
                            Platform.Log(LogLevel.Warn, "Connection limit on device {0} has been reached. Max = {1}.", device.AeTitle, device.ThrottleMaxConnections);
                            
                            // blackout for 5 seconds
                            _tempBlackoutDevices.Add(device.Key, device);
                            busy = true;
                        }
                    }
                }

            }

            return busy;
        }

        #endregion

        #region Private Methods

        private List<WorkQueueUid> FindWorkQueueUids(StorageInstance instance)
        {
            if (_uidMaps == null)
            {
                if (WorkQueueUidList != null)
                {
                    _uidMaps = new Dictionary<string, List<WorkQueueUid>>();
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        if (!_uidMaps.ContainsKey(uid.SopInstanceUid))
                            _uidMaps.Add(uid.SopInstanceUid, new List<WorkQueueUid>());

                        _uidMaps[uid.SopInstanceUid].Add(uid);
                    }
                }
            }

            if (_uidMaps!=null)
            {
                List<WorkQueueUid> foundUids;
                if (_uidMaps.TryGetValue(instance.SopInstanceUid, out foundUids))
                {
                    return foundUids;
                }
            }

            return null;
        }

        #endregion
    }
}
