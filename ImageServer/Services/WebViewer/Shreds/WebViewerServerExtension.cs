#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;


namespace ClearCanvas.ImageServer.Services.WebViewer.Shreds
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    class WebViewerServerExtension : Shred
    {
        public override void Start()
        {
            Platform.Log(LogLevel.Info, "Starting {0}...", GetDisplayName());
            WebViewerServerManager.Instance.Start();
            Platform.Log(LogLevel.Info, "{0} is started", GetDisplayName());
        }

        public override void Stop()
        {
            WebViewerServerManager.Instance.Stop();
        }

        public override string GetDisplayName()
        {
            return SR.WebViewerServerName;
        }

        public override string GetDescription()
        {
            return SR.WebViewerServerDescription;
        }
    }
}
