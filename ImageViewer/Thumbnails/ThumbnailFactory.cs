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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    public interface IThumbnailFactory
    {
        Bitmap CreateDummy(string message, Size size);
        Bitmap CreateThumbnail(IPresentationImage image, Size size);
    }

    public class ThumbnailFactory : IThumbnailFactory
    {
        public Bitmap CreateDummy(string message, Size size)
        {
            return CreateDummy(message, size.Width, size.Height);
        }

        public Bitmap CreateThumbnail(IPresentationImage image, Size size)
        {
            return CreateThumbnail(image, size.Width, size.Height);
        }

        public static Bitmap CreateDummy(string message, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp);

            Brush brush = new SolidBrush(Color.WhiteSmoke);
            var font = new Font("Arial", 10.0f);

            var format = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

            graphics.DrawString(message, font, brush, new Rectangle(0, 0, width, height), format);
            DrawBorder(graphics, width, height);
            graphics.Dispose();

            format.Dispose();
            font.Dispose();
            brush.Dispose();

            return bmp;
        }

        public static Bitmap CreateThumbnail(IPresentationImage image, int width, int height)
        {
            var visibilityHelper = new TextOverlayVisibilityHelper(image);
            visibilityHelper.Hide();

            var bitmap = CreateThumbnailImage(image, width, height);

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                DrawBorder(graphics, width, height);

                image.Dispose();
                graphics.Dispose();
            }

            return bitmap;
        }

        private static Bitmap CreateThumbnailImage(IPresentationImage image, int width, int height)
        {
            //TODO (CR June 2011): Not sure why this is so complicated ... doesn't need to be, that's for sure.
            const int rasterResolution = 256;

            var bitmap = new Bitmap(width, height);
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                try
                {
                    var imageAspectRatio = 1f;
                    var thumbnailAspectRatio = (float)height / width;
                    var thumbnailBounds = new RectangleF(0, 0, width, height);

                    if (image is IImageGraphicProvider)
                    {
                        var imageGraphic = ((IImageGraphicProvider)image).ImageGraphic;
                        imageAspectRatio = (float)imageGraphic.Rows / imageGraphic.Columns;
                    }
                    if (image is IImageSopProvider)
                    {
                        var ig = ((IImageSopProvider)image).Frame;
                        if (!ig.PixelAspectRatio.IsNull)
                            imageAspectRatio *= ig.PixelAspectRatio.Value;
                        else if (!ig.PixelSpacing.IsNull)
                            imageAspectRatio *= (float)ig.PixelSpacing.AspectRatio;
                    }

                    if (thumbnailAspectRatio >= imageAspectRatio)
                    {
                        thumbnailBounds.Width = width;
                        thumbnailBounds.Height = width * imageAspectRatio;
                        thumbnailBounds.Y = (height - thumbnailBounds.Height) / 2;
                    }
                    else
                    {
                        thumbnailBounds.Width = height / imageAspectRatio;
                        thumbnailBounds.Height = height;
                        thumbnailBounds.X = (width - thumbnailBounds.Width) / 2;
                    }

                    // rasterize any invariant vector graphics at a semi-normal image box resolution first before rendering as a thumbnail
                    using (var raster = image.DrawToBitmap(rasterResolution, (int)(rasterResolution * imageAspectRatio)))
                    {
                        graphics.DrawImage(raster, thumbnailBounds);
                    }
                }
                catch (Exception ex)
                {
                    // rendering the error text to a 100x100 icon is useless, so we'll just paint a placeholder error icon and log the icon error
                    Platform.Log(LogLevel.Warn, ex, "Failed to render thumbnail with dimensions {0}x{1}", width, height);

                    graphics.FillRectangle(Brushes.Black, 0, 0, width, height);
                    graphics.DrawLine(Pens.WhiteSmoke, 0, 0, width, height);
                    graphics.DrawLine(Pens.WhiteSmoke, 0, height, width, 0);
                }
            }

            return bitmap;
        }

        private static void DrawBorder(System.Drawing.Graphics graphics, int width, int height)
        {
            using (var pen = new Pen(Color.DarkGray))
            {
                graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);
            }
        }
    }
}
