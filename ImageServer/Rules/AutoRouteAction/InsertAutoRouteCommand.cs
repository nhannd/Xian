#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
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
    	private readonly DateTime? _scheduledTime;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">A contentxt in which to apply the AutoRoute request.</param>
        /// <param name="device">The AE Title of the device to AutoRoute to.</param>
        public InsertAutoRouteCommand(ServerActionContext context, string device)
            : base("Update/Insert an AutoRoute WorkQueue Entry")
        {
            Platform.CheckForNullReference(context, "ServerActionContext");

            _context = context;
            _deviceAe = device;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context">A contentxt in which to apply the AutoRoute request.</param>
		/// <param name="device">The AE Title of the device to AutoRoute to.</param>
		/// <param name="scheduledTime">The scheduled time for the AutoRoute.</param>
		public InsertAutoRouteCommand(ServerActionContext context, string device, DateTime scheduledTime)
			: base("Update/Insert an AutoRoute WorkQueue Entry")
		{
			Platform.CheckForNullReference(context, "ServerActionContext");

			_context = context;
			_deviceAe = device;
			_scheduledTime = scheduledTime;
		}

        /// <summary>
        /// Do the insertion of the AutoRoute.
        /// </summary>
        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
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
                                SR.AlertComponentAutorouteRule, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
                                SR.AlertAutoRouteUnknownDestination, _deviceAe, _context.ServerPartition.AeTitle);

                return;
			}
        	if (!dev.AllowAutoRoute)
            {
                Platform.Log(LogLevel.Warn,
                             "Auto-route attempted to device {0} on partition {1} with autoroute support disabled.  Ignoring request.",
                             dev.AeTitle, _context.ServerPartition.AeTitle);

                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, SR.AlertComponentAutorouteRule, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
                            SR.AlertAutoRouteDestinationAEDisabled, dev.AeTitle, _context.ServerPartition.AeTitle);
                
                return;
            }

			InsertWorkQueueParameters parms = new InsertWorkQueueParameters
			                                  	{
			                                  		WorkQueueTypeEnum = WorkQueueTypeEnum.AutoRoute,
			                                  		ScheduledTime = _scheduledTime.HasValue
			                                  		                	? _scheduledTime.Value
			                                  		                	: Platform.Time.AddSeconds(10),
			                                  		StudyStorageKey = _context.StudyLocationKey,
			                                  		ServerPartitionKey = _context.ServerPartitionKey,
			                                  		DeviceKey = dev.GetKey(),
			                                  		SeriesInstanceUid =
			                                  			_context.Message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty),
			                                  		SopInstanceUid =
			                                  			_context.Message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty)
			                                  	};
        	IInsertWorkQueue broker = updateContext.GetBroker<IInsertWorkQueue>();

            if (broker.FindOne(parms)==null)
            {
                throw new ApplicationException("InsertAutoRouteCommand failed");    
            }
        }
    }
}
