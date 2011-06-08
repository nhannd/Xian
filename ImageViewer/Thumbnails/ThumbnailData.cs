#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailData
    {
        int Width { get; }
        int Height { get; }
        int Stride { get; }
        PixelFormat PixelFormat { get; }
        byte[] PixelData { get; }

        Bitmap ToBitmap();
    }

    public class ThumbnailData : IThumbnailData
    {
        public ThumbnailData(int width, int height, int stride, PixelFormat pixelFormat, byte[] pixelData)
        {
            Width = width;
            Height = height;
            Stride = stride;
            PixelFormat = pixelFormat;
            PixelData = pixelData;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public PixelFormat PixelFormat { get; private set; }
        public byte[] PixelData { get; private set; }

        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(Width, Height, PixelFormat);
            var bounds = new Rectangle(Point.Empty, bitmap.Size);
            var bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                Marshal.Copy(PixelData, 0, bitmapData.Scan0, PixelData.Length);
                return bitmap;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public static IThumbnailData FromBitmap(Bitmap bitmap)
        {
            var bounds = new Rectangle(Point.Empty, bitmap.Size);
            var bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                var pixelData = new byte[bitmapData.Stride * bitmapData.Height];
                Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);
                return new ThumbnailData(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmap.PixelFormat, pixelData);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}