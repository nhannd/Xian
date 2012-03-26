#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDicomServerService))
                return new DicomServerServiceClient();
            return null;
        }

        #endregion
    }

	internal class DicomServerServiceClient : ClientBase<IDicomServerService>, IDicomServerService
	{
		public DicomServerServiceClient()
		{
		}

		[Obsolete("Use the SendStudies method instead.")]
        public void Send(ApplicationEntity destinationAEInformation, IEnumerable<string> studyInstanceUids)
		{
			base.Channel.Send(destinationAEInformation, studyInstanceUids);
		}

        public void RetrieveStudies(ApplicationEntity sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			base.Channel.RetrieveStudies(sourceAEInformation, studiesToRetrieve);
		}

        public void RetrieveSeries(ApplicationEntity sourceAEInformation, StudyInformation studyInformation, IEnumerable<string> seriesInstanceUids)
		{
			base.Channel.RetrieveSeries(sourceAEInformation, studyInformation, seriesInstanceUids);
		}

        public RestartListenerResult RestartListener(RestartListenerRequest request)
        {
            return base.Channel.RestartListener(request);
        }
    }
}
