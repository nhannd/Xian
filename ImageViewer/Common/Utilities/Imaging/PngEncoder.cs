using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.Utilities.Imaging
{
    public interface IPngEncoder
    {
        MemoryStream Encode(Bitmap bitmap);
        void Encode(Bitmap bitmap, MemoryStream memoryStream);
    }

    [ExtensionPoint]
    public class PngEncoderExtensionPoint : ExtensionPoint<IPngEncoder>
    {
    }

    public abstract class PngEncoder : IPngEncoder
    {
        private class DefaultEncoder : IPngEncoder
        {
            #region IPngEncoder Members

            public MemoryStream Encode(Bitmap bitmap)
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return stream;
            }

            public void Encode(Bitmap bitmap, MemoryStream memoryStream)
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
            }

            #endregion
        }

        public abstract MemoryStream Encode(Bitmap bitmap);
        public abstract void Encode(Bitmap bitmap, MemoryStream memoryStream);

        public static IPngEncoder Create()
        {
            try
            {
                return (IPngEncoder)new PngEncoderExtensionPoint().CreateExtension();
            }
            catch (NotSupportedException)
            {
            }

            return new DefaultEncoder();
        }
    }
}
