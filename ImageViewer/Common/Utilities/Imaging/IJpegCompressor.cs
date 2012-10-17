#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.Utilities.Imaging
{
    [ExtensionPoint]
    public class JpegCompressorExtensionPoint : ExtensionPoint<IJpegCompressor>
    {
    }

    public interface IJpegCompressor
    {
        MemoryStream Compress(Bitmap image, int quality);
    }

    public abstract class JpegCompressor : IJpegCompressor
    {
        private class DefaultCompressor : IJpegCompressor
        {
            private const string _mimeType = "image/jpeg";
            private readonly ImageCodecInfo _codec;

            public DefaultCompressor()
            {
                _codec = GetEncoderInfo();
            }

            public MemoryStream Compress(Bitmap image, int quality)
            {
                var eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                ImageCodecInfo ici = _codec;
                
                var ms = new MemoryStream();
                image.Save(ms, ici, eps);
                return ms;
            }

            private static ImageCodecInfo GetEncoderInfo()
            {
                int j;
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType.Equals(_mimeType))
                        return encoders[j];
                }
                return null;
            }
        }

        public static IJpegCompressor Create()
        {
            try
            {
                return (IJpegCompressor)new JpegCompressorExtensionPoint().CreateExtension();

            }
            catch (NotSupportedException)
            {
            }
            
            return new DefaultCompressor();
        }

        public abstract MemoryStream Compress(Bitmap image, int quality);
    }
}
