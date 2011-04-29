#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Core.Ping
{
	[SettingsGroupDescription("Client side caching settings for ping service.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
    internal sealed partial class PingServiceSettings 
	{
    }
}
