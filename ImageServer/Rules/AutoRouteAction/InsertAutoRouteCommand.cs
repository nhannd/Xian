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
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Rules.AutoRouteAction
{
    
    /// <summary>
    /// <see cref="ServerDatabaseCommand"/> derived class for use with <see cref="ServerCommandProcessor"/> for inserting AutoRoute WorkQueue entries into the Persistent Store.
    /// </summary>
    public class InsertAutoRouteCommand : ServerDatabaseCommand
    {
        private readonly ServerActionContext _context;
        private readonly string _deviceAe;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">A contentxt in which to apply the AutoRoute request.</param>
        /// <param name="device">The AE Title of the device to AutoRoute to.</param>
        public InsertAutoRouteCommand(ServerActionContext context, string device)
            : base("Update/Insert an AutoRoute WorkQueue Entry", false)
        {
            Platform.CheckForNullReference(context, "ServerActionContext");

            _context = context;
            _deviceAe = device;
        }

        /// <summary>
        /// Do the insertion of the AutoRoute.
        /// </summary>
        protected override void OnExecute(IUpdateContext updateContext)
        {
            DeviceSelectCriteria deviceSelectCriteria = new DeviceSelectCriteria();
            deviceSelectCriteria.AeTitle.EqualTo(_deviceAe);
            deviceSelectCriteria.ServerPartitionKey.EqualTo(_context.ServerPartitionKey);

            IDeviceEntityBroker selectDevice = updateContext.GetBroker<IDeviceEntityBroker>();

            Device dev = selectDevice.FindOne(deviceSelectCriteria);
			if (dev == null)
			{
				Platform.Log(LogLevel.Warn,
				             "Device '{0}' on partition {1} not in database for autoroute request!  Ignoring request.", _deviceAe,
				             _context.ServerPartition.AeTitle);

                ServerPlatform.Alert(
                                AlertCategory.Application, AlertLevel.Warning,
                                SR.AlertComponentAutorouteRule, AlertTypeCodes.UnableToProcess, TimeSpan.FromMinutes(5),
                                SR.AlertAutoRouteUnknownDestination, _deviceAe, _context.ServerPartition.AeTitle);

                return;
			}
        	if (!dev.AllowAutoRoute)
            {
                Platform.Log(LogLevel.Warn,
                             "Auto-route attempted to device {0} on partition {1} with autoroute support disabled.  Ignoring request.",
                             dev.AeTitle, _context.ServerPartition.AeTitle);

                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, SR.AlertComponentAutorouteRule, AlertTypeCodes.UnableToProcess, TimeSpan.FromMinutes(5),
                            SR.AlertAutoRouteDestinationAEDisabled, dev.AeTitle, _context.ServerPartition.AeTitle);
                
                return;
            }

			InsertWorkQueueParameters parms = new InsertWorkQueueParameters();
			parms.WorkQueueTypeEnum = WorkQueueTypeEnum.AutoRoute;
            parms.ScheduledTime = Platform.Time.AddSeconds(30);
            parms.ExpirationTime = Platform.Time.AddMinutes(4);
            parms.StudyStorageKey = _context.StudyLocationKey;
            parms.ServerPartitionKey = _context.ServerPartitionKey;
            parms.DeviceKey = dev.GetKey();
            parms.SeriesInstanceUid = _context.Message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            parms.SopInstanceUid = _context.Message.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
        	parms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.Medium;
			IInsertWorkQueue broker = updateContext.GetBroker<IInsertWorkQueue>();

            broker.Execute(parms);
        }
    }
}
