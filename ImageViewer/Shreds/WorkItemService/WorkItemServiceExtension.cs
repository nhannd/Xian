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

        private bool _workItemServiceWCFInitialized;

        public WorkItemServiceExtension()
        {
            _workItemServiceWCFInitialized = false;            
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
        }

        public override void Stop()
        {
            if (_workItemServiceWCFInitialized)
            {
                try
                {
                    StopHost(_workItemServiceEndpointName);
                    Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.LocalDataStore));
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
