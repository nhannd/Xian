#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.ImageViewer.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [SettingsGroupDescription("Application settings for DICOM Sends.")]
    [SettingsProvider(typeof(SystemConfigurationSettingsProvider))]
    public partial class DicomSendSettings
    {
    }
}
