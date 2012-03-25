#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    // TODO (Marmot): remove this stuff.


	public class DicomSendServiceClient : ClientBase<IDicomSendService>, IDicomSendService
	{
		public DicomSendServiceClient()
		{
		}

		#region IDicomSendService Members

		public SendOperationReference SendStudies(SendStudiesRequest request)
		{
			return base.Channel.SendStudies(request);
		}

		public SendOperationReference SendSeries(SendSeriesRequest request)
		{
			return base.Channel.SendSeries(request);
		}

		public SendOperationReference SendSopInstances(SendSopInstancesRequest request)
		{
			return base.Channel.SendSopInstances(request);
		}

		public SendOperationReference SendFiles(SendFilesRequest request)
		{
			return base.Channel.SendFiles(request);
		}

		#endregion
	}
}
