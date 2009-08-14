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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
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
        #region Private Members
        private readonly object _syncLock = new object();
        private Dictionary<string, WorkQueueUid> _uidMaps = null;
        private Device _device;
        private const short UNLIMITED = -1;
        #endregion

        #region Virtual Protected Methods


        /// <summary>
        /// Convert Uids into SopInstance
        /// </summary>
        /// <returns></returns>
        protected virtual IList<StorageInstance> GetStorageInstanceList()
        {
            LoadUids(WorkQueueItem);
            
            string studyPath = StorageLocation.GetStudyPath();
            StudyXml studyXml = LoadStudyXml(StorageLocation);
            
            List<StorageInstance> list = new List<StorageInstance>();
            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
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

                list.Add(instance);
            }

            return list;
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
            WorkQueueUid foundUid = FindWorkQueueUid(instance);

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
                if (foundUid != null)
                {
                    foundUid.FailureCount++;
                    UpdateWorkQueueUid(foundUid);
                }
            }
            else
            {
                if (foundUid != null)
                {
                    DeleteWorkQueueUid(foundUid);
                    WorkQueueUidList.Remove(foundUid);
                }
            }


        }

        #endregion

        #region Protected Properties
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

            // Load the list of instances to be sent
            IList<StorageInstance> instanceList = GetStorageInstanceList();
            
            if (instanceList == null || instanceList.Count == 0)
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
            scu.AddStorageInstanceList(instanceList);

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
                if ((instanceList.Count > 0 && sendCounter != instanceList.Count) // not all sop were sent
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
            int currentConnectionCounter = DestinationDevice.GetConcurrentMoveCount(ReadContext);
            if (DestinationDevice.ThrottleMaxConnections == UNLIMITED)
            {
                if (currentConnectionCounter >= ImageServerCommonConfiguration.TooManyStudyMoveWarningThreshold)
                {
                    RaisePotentialTooManyConnectionAlert(currentConnectionCounter);
                }
            }
            else if (currentConnectionCounter > DestinationDevice.ThrottleMaxConnections)
            {
                RaiseConnectionLimitReachedAlert();

                PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Pending,WorkQueueProcessorDatabaseUpdate.None);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private WorkQueueUid FindWorkQueueUid(StorageInstance instance)
        {
            if (_uidMaps == null)
            {
                if (WorkQueueUidList != null)
                {
                    _uidMaps = new Dictionary<string, WorkQueueUid>();
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        _uidMaps.Add(uid.SopInstanceUid, uid);
                    }
                }
            }

            WorkQueueUid foundUid;
            if (_uidMaps.TryGetValue(instance.SopInstanceUid, out foundUid))
            {
                return foundUid;
            }
            else
                return null;

        }

        

        private void RaiseConnectionLimitReachedAlert()
        {
            Platform.Log(LogLevel.Warn, "Connection limit on device {0} has been reached. Max = {1}.", DestinationDevice.AeTitle, DestinationDevice.ThrottleMaxConnections);
        }

        private void RaisePotentialTooManyConnectionAlert(int currentConnectionCounter)
        {
        	ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, "Auto-route/Move",
                                 AlertTypeCodes.LowResources, null, TimeSpan.Zero, "Number of current connections to {0} : {1}",
        	                     DestinationDevice.AeTitle, currentConnectionCounter);
        }

        #endregion
    }
}
