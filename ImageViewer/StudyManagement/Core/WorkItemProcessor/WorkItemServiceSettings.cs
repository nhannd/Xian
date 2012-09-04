#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// TODO (CR Jun 2012): Move to Common and/or use the new settings provider?
    // (SW) didn't make it the new settings provider because there'd be no easy way for the user to edit.
    [SettingsGroupDescription("Settings for the WorkItemService.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    public partial class WorkItemServiceSettings 
	{
	}
}
