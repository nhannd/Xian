#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to an object that is responsible for configuring a service channel.
	/// </summary>
    public interface IServiceChannelConfiguration
    {
		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
        ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args);
    }
}
