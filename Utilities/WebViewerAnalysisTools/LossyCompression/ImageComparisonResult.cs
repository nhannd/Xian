#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    /// <summary>
    /// Encapsulates the bitmap comparison result.
    /// </summary>
    public class ImageComparisonResult
    {
        public string Description { get; set; }
        public ChannelComparisonResult[] Channels { get; set; }

        public Bitmap Image1 { get; set; }
        public Bitmap Image2 { get; set; }

        public float? CompressionRatio { get;set;}
    }
}