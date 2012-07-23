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
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerExtension : WcfShred
    {
		private readonly string _dicomServerEndpointName = "DicomServer";
		private bool _dicomServerWcfInitialized;

        public DicomServerExtension()
        {
			_dicomServerWcfInitialized = false;

			LicenseInformation.LicenseChanged += OnLicenseInformationChanged;
		}

        public override void Start()
        {
			try
			{                
				StartNetPipeHost<DicomServerServiceType, IDicomServer>(_dicomServerEndpointName, SR.DicomServer);
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

            //NOTE: in a lot of cases, we start all the internal services before the WCF service,
            //but in this case, the (shared/offline) DICOM service configuration will call RestartListener
            //right after a change is made in the database, so we want to start the internal services
            //after the WCF service us up and running. That way, although unlikely, if the server configuration
            //were changed just as this service were starting up, we will always start up the listener
            //with the right AE title and Port, even if the listener starts then restarts in quick succession.
            //These internal services all nicely handle the possibility of a service calling into them when
            //they're not running yet, anyway.
            try
            {
                DicomServerManager.Instance.Start();

                string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.DicomServer);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.DicomServer));
            }
        }

        public override void Stop()
        {
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
    		DicomServerManager.Instance.Restart();
    	}
   }
}