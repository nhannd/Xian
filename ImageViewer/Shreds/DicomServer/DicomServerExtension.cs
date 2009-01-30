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
		private static readonly string _studyLocatorEndpointName = "StudyLocator";

		private bool _studyLocatorWCFInitialized;
		private bool _dicomServerWcfInitialized;
		private bool _dicomSendServiceWCFInitialized;

        public DicomServerExtension()
        {
			_studyLocatorWCFInitialized = false;
			_dicomServerWcfInitialized = false;
        	_dicomSendServiceWCFInitialized = false;
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

			try
			{
				ServiceEndpointDescription sed = 
					StartBasicHttpHost<StudyLocator.StudyLocator, IStudyRootQuery>(_studyLocatorEndpointName, SR.StudyLocator);
				sed.Binding.Namespace = QueryNamespace.Value;

				_studyLocatorWCFInitialized = true;

				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.StudyLocator);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.StudyLocator));
			}
        }

        public override void Stop()
        {
			if (_studyLocatorWCFInitialized)
			{
				try
				{
					StopHost(_studyLocatorEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.StudyLocator));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

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
   }
}