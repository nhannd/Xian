#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

//using System.Configuration;
//using ClearCanvas.Server.ShredHost;

using System.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
	// TODO : this is temporarliy broken - but needs to be converted to app settings anyhow
    [SettingsGroupDescription("Settings for the WorkItemService.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    public partial class WorkItemServiceSettings 
	{
	}
}
