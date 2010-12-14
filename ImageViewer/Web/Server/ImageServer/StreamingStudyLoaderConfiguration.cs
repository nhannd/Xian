#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyLoaders.Streaming;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(StreamingStudyLoaderConfigurationExtensionPoint))]
    public class StreamingStudyLoaderConfiguration:IStreamingStudyLoaderConfiguration
    {
        public string GetClientAETitle()
        {
            return WebViewerServices.Default.AETitle;
        }
    }
}
