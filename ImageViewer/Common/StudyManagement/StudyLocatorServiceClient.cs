#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	/// <summary>
	/// WCF client proxy for <see cref="IStudyLocator"/> services.
	/// </summary>
	public class StudyLocatorServiceClient : ClientBase<IStudyLocator>, IStudyLocator
	{
		/// <summary>
		/// Constructor - uses default configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyLocatorServiceClient() {}

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyLocatorServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName) {}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public StudyLocatorServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress) {}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
		public StudyLocatorServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress) {}

		#region Implementation of IStudyLocator

		public LocateStudiesResult LocateStudies(LocateStudiesRequest request)
		{
			return Channel.LocateStudies(request);
		}

		public LocateSeriesResult LocateSeries(LocateSeriesRequest request)
		{
			return Channel.LocateSeries(request);
		}

		public LocateImagesResult LocateImages(LocateImagesRequest request)
		{
			return Channel.LocateImages(request);
		}

		#endregion
	}
}