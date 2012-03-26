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
    // TODO (Marmot): remove. Add bridge for equivalent work queue stuff.

    [ServiceContract(ConfigurationName = "IDicomSendService")]
	public interface IDicomSendService
	{
		[OperationContract]
		SendOperationReference SendStudies(SendStudiesRequest request);
		
		[OperationContract]
		SendOperationReference SendSeries(SendSeriesRequest request);
		
		[OperationContract]
		SendOperationReference SendSopInstances(SendSopInstancesRequest request);

		[OperationContract]
		SendOperationReference SendFiles(SendFilesRequest request);

		//TODO: add cancel
	}
}
