#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;
using Cursor = ClearCanvas.ImageViewer.Web.Common.Entities.Cursor;
using Image = ClearCanvas.Web.Common.Image;
using Rectangle = System.Drawing.Rectangle;

namespace ClearCanvas.ImageViewer.Web.View
{
    internal static class CursorFactory
    {
        [ThreadStatic] private static Dictionary<CursorToken, Cursor> _cursors;

        private static Dictionary<CursorToken, Cursor> Cursors
        {
            get { return _cursors ?? (_cursors = new Dictionary<CursorToken, Cursor>()); }
        }

        public static Cursor CreateCursor(CursorToken cursorToken)
        {
            if (cursorToken == null)
                return null;

            Cursor cursor;
            if (!Cursors.TryGetValue(cursorToken, out cursor))
                cursor = Cursors[cursorToken] = CreateCursorIcon(cursorToken);

            return cursor;
        }

        private static Cursor CreateCursorIcon(CursorToken cursorToken)
        {
            Cursor webCursor = new Cursor();
            Bitmap bitmap;
            if (cursorToken.IsSystemCursor)
            {
                PropertyInfo propertyInfo = typeof(Cursors).GetProperty(cursorToken.ResourceName, BindingFlags.Static | BindingFlags.Public);
                var cursor = (System.Windows.Forms.Cursor)propertyInfo.GetValue(null, null);
                bitmap = new Bitmap(cursor.Size.Width, cursor.Size.Height, PixelFormat.Format32bppArgb);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    cursor.Draw(g, new Rectangle(Point.Empty, cursor.Size));

                webCursor.HotSpot = new Position(cursor.HotSpot);
            }
            else
            {
                bitmap = new Bitmap(cursorToken.Resolver.OpenResource(cursorToken.ResourceName));
                webCursor.HotSpot = new Position { X = bitmap.Width / 2, Y = bitmap.Height / 2 };
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                webCursor.Icon = new Image {Data = stream.GetBuffer(), MimeType = Image.MimeTypes.Png};
                stream.Close();
            }

            return webCursor;
        }
    }
}
