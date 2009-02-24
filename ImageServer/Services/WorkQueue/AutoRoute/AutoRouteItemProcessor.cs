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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Dicom;
using ClearCanvas.ImageServer.Services.WorkQueue.WebMoveStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute
{
    /// <summary>
    /// Processor for 'AutoRoute <see cref="WorkQueue"/> entries
    /// </summary>
    public class AutoRouteItemProcessor : BaseItemProcessor, ICancelable
    {
        #region Protected Methods
        /// <summary>
        /// Add the UIDs scheduled to be transfered to the SCU
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="scu">The Storage SCU component doing an autoroute.</param>
        protected virtual void AddWorkQueueUidsToSendList(Model.WorkQueue item, ImageServerStorageScu scu)
        {
			if (!LoadStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing AutoRoute WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
				return;
			}

			string studyPath = StorageLocation.GetStudyPath();

            StudyXml studyXml = LoadStudyXml(StorageLocation);

            foreach (WorkQueueUid uid in WorkQueueUidList)
            {
                scu.LoadInstanceFromStudyXml(studyPath, uid.SeriesInstanceUid, uid.SopInstanceUid, studyXml);
            }
        }

        #endregion

        #region Overridden Protected Method

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item of type AutoRoute.
        /// </summary>
        protected override void ProcessItem(Model.WorkQueue item)
        {
            // Load related rows form the WorkQueueUid table
            LoadUids(item);

            // Intercept entries that don't have any UIDs associated with them, and just
            // set them back to pending if its an AutoRoute request.
            if (WorkQueueUidList.Count == 0 
                && item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.AutoRoute))
            {
				PostProcessing(item, 
					WorkQueueProcessorStatus.Idle, 
					WorkQueueProcessorDatabaseUpdate.None);
                return;
            }

            // Load remote device information from the database.
            Device device = Device.Load(ReadContext, item.DeviceKey);
            if (device == null)
            {
                item.FailureDescription = String.Format("Unknown auto-route destination \"{0}\"", item.DeviceKey);
                Platform.Log(LogLevel.Error,item.FailureDescription);

                PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal Error
                return ;
            }

            if (device.Dhcp && device.IpAddress.Length == 0)
            {
                item.FailureDescription = String.Format("Auto-route destination is a DHCP device with no known IP address: \"{0}\"", device.AeTitle);
                Platform.Log(LogLevel.Error,
                             item.FailureDescription);

                PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal); // Fatal error
                return;
            }

            ServerPartition partition = ServerPartition.Load(ReadContext, item.ServerPartitionKey);

            // Now setup the StorageSCU component
            ImageServerStorageScu scu = new ImageServerStorageScu(partition, device);

            // set the preferred syntax lists
            scu.LoadPreferredSyntaxes(ReadContext);

            // Load the Instances to Send into the SCU component
            AddWorkQueueUidsToSendList(item, scu);

            // Set an event to be called when each image is transferred
            scu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                                    {
                                        if (instance.SendStatus.Status == DicomState.Success
                                            || instance.SendStatus.Status == DicomState.Warning
                                            || instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
                                        {
                                        	WorkQueueUid foundUid = null;
                                        	foreach (WorkQueueUid uid in WorkQueueUidList)
                                        	{
                                        		if (uid.SopInstanceUid.Equals(instance.SopInstanceUid))
                                        		{
                                        			foundUid = uid;
                                        			break;
                                        		}
                                        	}

                                        	if (instance.SendStatus.Status == DicomState.Failure)
                                        	{
                                        		item.FailureDescription =
                                        			scu.FailureDescription = instance.SendStatus.Description;
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

										if (instance.SendStatus.Equals(DicomStatuses.SOPClassNotSupported))
										{
											item.FailureDescription =
												String.Format("SOP Class not supported by remote device: {0}",
												              instance.SopClass.Name);
											Platform.Log(LogLevel.Warn,
											             "Unable to transfer SOP Instance, SOP Class is not supported by remote device: {0}",
											             instance.SopClass.Name);
										}

										if (CancelPending && !(this is WebMoveStudyItemProcessor) && !scu.Canceled)
										{
											Platform.Log(LogLevel.Info,"Auto-route canceled due to shutdown for study: {0}",StorageLocation.StudyInstanceUid);
											item.FailureDescription = "Operation was canceled due to server shutdown request.";
											scu.Cancel();
										}
                                    };

            // Block until send is complete
            scu.Send();

            // Join for the thread to exit
            scu.Join();

            // Dispose to cleanup properly
            scu.Dispose();

			if (scu.FailureDescription.Length > 0)
			{
				item.FailureDescription = scu.FailureDescription;
				scu.Status = ScuOperationStatus.Failed;
			}

        	// Reset the WorkQueue entry status
            if ((WorkQueueUidList.Count > 0) || scu.Status == ScuOperationStatus.Failed || scu.Status == ScuOperationStatus.ConnectFailed)
                PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal); // failures occurred
			else if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.AutoRoute))
				PostProcessing(item,
					WorkQueueProcessorStatus.Pending,
					WorkQueueProcessorDatabaseUpdate.None); // no failures
			else
			{
				item.ScheduledTime = item.ExpirationTime;
				PostProcessing(item,
				               WorkQueueProcessorStatus.Idle, // Will force the entry to idle.
				               WorkQueueProcessorDatabaseUpdate.None); 
			}
        }
        #endregion

        protected override bool CanStart()
        {
            return true; // can start anytime
        }
    }
}
