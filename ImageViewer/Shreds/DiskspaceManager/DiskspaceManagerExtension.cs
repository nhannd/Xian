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
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.DiskspaceManager;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DiskspaceManagerExtension : WcfShred
    {
		private bool _wcfInitialized;
		private readonly string _endpointName;

        public DiskspaceManagerExtension()
        {
        	_wcfInitialized = false;
            _endpointName = "DiskspaceManager";
        }

        public override void Start()
        {
			try
			{
				DiskspaceManagerProcessor.Instance.StartProcessor();
				string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.DiskspaceManager);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.DiskspaceManager));
				return;
			}

			try
			{
				StartNetPipeHost<DiskspaceManagerServiceType, IDiskspaceManagerService>(_endpointName, SR.DiskspaceManager);
				_wcfInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.DiskspaceManager);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.DiskspaceManager));
			}
		}

        public override void Stop()
        {
			if (_wcfInitialized)
			{
				try
				{
					StopHost(_endpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.DiskspaceManager));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			try
			{
				DiskspaceManagerProcessor.Instance.StopProcessor();
				Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.DiskspaceManager));
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
        }

        public override string GetDisplayName()
        {
			return SR.DiskspaceManager;
        }

        public override string GetDescription()
        {
			return SR.DiskspaceManagerDescription;
        }
   }
}