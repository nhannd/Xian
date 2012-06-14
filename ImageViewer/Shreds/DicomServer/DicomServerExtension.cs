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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerExtension : WcfShred
    {
		private readonly string _dicomServerEndpointName = "DicomServer";
		private readonly string _dicomSendServiceEndpointName = "DicomSend";

		private bool _dicomServerWcfInitialized;
		private bool _dicomSendServiceWCFInitialized;

        public DicomServerExtension()
        {
			_dicomServerWcfInitialized = false;
        	_dicomSendServiceWCFInitialized = false;

			LicenseInformation.LicenseChanged += OnLicenseInformationChanged;
		}

        public override void Start()
        {
			try
			{
				LocalDataStoreEventPublisher.Instance.Start();
				DicomSendManager.Instance.Start();
				DicomRetrieveManager.Instance.Start();
				DicomServerManager.Instance.Start();

				string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.DicomServer);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.DicomServer));
				return;
			}

			try
			{
				StartNetPipeHost<DicomServerServiceType, IDicomServerService>(_dicomServerEndpointName, SR.DicomServer);
				_dicomServerWcfInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.DicomServer);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.DicomServer));
			}

			try
			{
				StartNetPipeHost<DicomSendServiceType, IDicomSendService>(_dicomSendServiceEndpointName, SR.DicomSendService);
				_dicomSendServiceWCFInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.DicomSendService);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.DicomSendService));
			}
        }

        public override void Stop()
        {
			if (_dicomSendServiceWCFInitialized)
			{
				try
				{
					StopHost(_dicomSendServiceEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.DicomSendService));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			if (_dicomServerWcfInitialized)
        	{
        		try
        		{
        			StopHost(_dicomServerEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.DicomServer));
        		}
        		catch (Exception e)
        		{
        			Platform.Log(LogLevel.Error, e);
        		}
        	}

			try
			{
				DicomServerManager.Instance.Stop();
				DicomSendManager.Instance.Stop();
				DicomRetrieveManager.Instance.Stop();
				LocalDataStoreEventPublisher.Instance.Stop();

				Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.DicomServer));
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

        public override string GetDisplayName()
        {
			return SR.DicomServer;
        }

        public override string GetDescription()
        {
			return SR.DicomServerDescription;
        }

    	private void OnLicenseInformationChanged(object sender, EventArgs e)
    	{
    		Platform.Log(LogLevel.Info, @"Restarting {0} due to application licensing status change.", SR.DicomServer);
    		DicomServerManager.Instance.UpdateApplicationLicensingStatus();
    	}
   }
}