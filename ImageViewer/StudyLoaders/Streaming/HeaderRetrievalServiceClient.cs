#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
	{
		public HeaderStreamingServiceClient()
		{

		}

		public HeaderStreamingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			
		}

		public Stream GetStudyHeader(string callingAeTitle, HeaderStreamingParameters parameters)
		{
			return base.Channel.GetStudyHeader(callingAeTitle, parameters);
		}
	}
}
