#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.Automation
{
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IDicomExplorerAutomation", Namespace = AutomationNamespace.Value)]
	public interface IDicomExplorerAutomation
	{
		[FaultContract(typeof(NoLocalStoreFault))]
		[FaultContract(typeof(DicomExplorerNotFoundFault))]
		[OperationContract(IsOneWay = false)]
		SearchLocalStudiesResult SearchLocalStudies(SearchLocalStudiesRequest request);

		[FaultContract(typeof(ServerNotFoundFault))]
		[FaultContract(typeof(DicomExplorerNotFoundFault))]
		[OperationContract(IsOneWay = false)]
		SearchRemoteStudiesResult SearchRemoteStudies(SearchRemoteStudiesRequest request);
	}
}
