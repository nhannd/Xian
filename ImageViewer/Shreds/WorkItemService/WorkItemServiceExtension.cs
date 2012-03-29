#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class WorkItemServiceExtension : WcfShred
    {
        private const string _workItemServiceEndpointName = "WorkItemService";
        private const string _workItemActivityMonitorServiceEndpointName = "WorkItemActivityMonitor";

        private bool _workItemServiceWCFInitialized;
        private bool _workItemActivityMonitorServiceWCFInitialized;

        private readonly WorkItemProcessorExtension _processor;

        public WorkItemServiceExtension()
        {
            _workItemServiceWCFInitialized = false;
            _workItemActivityMonitorServiceWCFInitialized = false;
            _processor = new WorkItemProcessorExtension();
        }

        public override void Start()
        {
            try
            {
                WorkItemService.Instance.Start();
                string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.WorkItemService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.WorkItemService));
                return;
            }

            try
            {
                StartNetPipeHost<WorkItemServiceType, IWorkItemService>(_workItemServiceEndpointName, SR.WorkItemService);
                _workItemServiceWCFInitialized = true;
                string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.WorkItemService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.WorkItemService));
            }

            try
            {
                StartNetPipeHost<WorkItemActivityMonitorServiceType, IWorkItemActivityMonitorService>(_workItemActivityMonitorServiceEndpointName, SR.WorkItemActivityMonitorService);
                _workItemActivityMonitorServiceWCFInitialized = true;
                string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.WorkItemActivityMonitorService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.WorkItemActivityMonitorService));
            }

            _processor.Start();
        }

        public override void Stop()
        {
            if (_workItemActivityMonitorServiceWCFInitialized)
            {
                try
                {
                    StopHost(_workItemActivityMonitorServiceEndpointName);
                    Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.WorkItemActivityMonitorService));
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }


            if (_workItemServiceWCFInitialized)
            {
                try
                {
                    StopHost(_workItemServiceEndpointName);
                    Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.WorkItemService));
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }

            try
            {
                WorkItemService.Instance.Stop();
                Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.WorkItemService));
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            _processor.Stop();
        }

        public override string GetDisplayName()
        {
            return SR.WorkItemService;
        }

        public override string GetDescription()
        {
            return SR.WorkItemServiceDescription;
        }
    }
}
