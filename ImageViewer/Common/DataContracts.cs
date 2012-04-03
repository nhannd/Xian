#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common
{
    public static class ImageViewerNamespace
    {
        public const string Value = "http://www.clearcanvas.ca/imageviewer";
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    public enum ServiceState
    {
        Stopped = 0,
        Starting,
        Started,
        Stopping
    }
}
