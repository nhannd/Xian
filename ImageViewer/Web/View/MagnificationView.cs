using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.Utilities.Imaging;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Tools.Standard;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Common.Messages;
using ClearCanvas.Web.Services.View;
using Image = ClearCanvas.Web.Common.Image;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace ClearCanvas.ImageViewer.Web.View
{
    [ExtensionOf(typeof(MagnificationViewExtensionPoint))]
    internal class MagnificationView : WebView<Magnifier>, IMagnificationView
    {
        private IJpegCompressor _jpegCompressor;
        private IPngEncoder _pngEncoder;

        private RenderMagnifiedImage _render;

        private Guid _tileId;
        private PresentationImage _image;
        private IRenderingSurface _surface;

        private Size _size;
        private Bitmap _bitmap;

        private Point _locationTile;
        private MemoryStream _memoryStream;

        private MemoryStream MemoryStream
        {
            get { return _memoryStream ?? (_memoryStream = new MemoryStream()); }
        }

        public override void SetModelObject(object modelObject)
        {
            //Custom view - not called.
        }

        public override void ProcessMessage(Message message)
        {
            var update = message as UpdatePropertyMessage;
            if (update == null)
                return;

            if (update.PropertyName == "Size")
            {
                _size = (ClearCanvas.Web.Common.Size)update.Value;
                _bitmap = new Bitmap(_size.Width, _size.Height, PixelFormat.Format24bppRgb);
                _surface = _image.ImageRenderer.GetRenderingSurface(IntPtr.Zero, _size.Width, _size.Height);

                _jpegCompressor = JpegCompressor.Create();
                _pngEncoder = PngEncoder.Create();

                FireEvent(new PropertyChangedEvent{PropertyName = "Image", Value = CreateImage()});
            }
        }

        protected override void Initialize()
        {
        }

        protected override void UpdateEntity(Magnifier entity)
        {
            entity.Size = _size;
            entity.Location = _locationTile;
            if (!_size.IsEmpty && _bitmap != null)
                entity.Image = CreateImage();
        }

        private Image CreateImage()
        {
            using (var graphics = System.Drawing.Graphics.FromImage(_bitmap))
            {
                _surface.ContextID = graphics.GetHdc();
                _surface.ClipRectangle = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

                var drawArgs = new DrawArgs(_surface, null, DrawMode.Render);
                _render(drawArgs);
                drawArgs = new DrawArgs(_surface, null, DrawMode.Refresh);
                _render(drawArgs);
                graphics.ReleaseHdc(_surface.ContextID);
            }

            string mimeType = Image.MimeTypes.Png;
            int jpegQ = 0;
            //ImageQualityManager.Instance.GetImageQualityParameters(out mimeType, out jpegQ);

            if (mimeType.Equals(Image.MimeTypes.Jpeg))
            {
                MemoryStream.SetLength(0);
                _jpegCompressor.Compress(_bitmap, jpegQ, MemoryStream);
            }
            else if (mimeType.Equals(Image.MimeTypes.Png))
            {
                MemoryStream.SetLength(0);
                _pngEncoder.Encode(_bitmap, MemoryStream);
            }
            else if (mimeType.Equals(Image.MimeTypes.Bmp))
            {
                MemoryStream.SetLength(0);
                _bitmap.Save(MemoryStream, ImageFormat.Bmp);
            }

            byte[] imageBuffer = MemoryStream.ToArray();
            if (!ApplicationContext.BlobsSupported)
                imageBuffer = Encoding.UTF8.GetBytes(Convert.ToBase64String(imageBuffer));

            return new Image { Data = imageBuffer, MimeType = mimeType, IsBase64 = !ApplicationContext.BlobsSupported };
        }

        #region IMagnificationView Members

        public void Open(IPresentationImage image, Point locationTile, RenderMagnifiedImage render)
        {
            _image = (PresentationImage)image;
            _render = render;
            _locationTile = locationTile;

            FireEvent(new EntityEvent
                          {
                              // TODO (Phoenix5): Seen this a few times where the "sender" is 
                              // not technically right - targetId? Or something wrong with design>
                              SenderId = _tileId = image.Tile.GetViewId(),
                              Entity = GetEntity(),
                              EventType = EntityEventType.Created
                          });
        }

        public void UpdateMouseLocation(Point locationTile)
        {
            _locationTile = locationTile;
            FireEvent(new PropertyChangedEvent { PropertyName = "Location", Value = (Position)_locationTile });
            if (_bitmap != null)
                FireEvent(new PropertyChangedEvent { PropertyName = "Image", Value = CreateImage() });
        }

        public void Close()
        {
            FireEvent(new EntityEvent
            {
                // TODO (Phoenix5): Seen this a few times where the "sender" is 
                // not technically right - targetId? Or something wrong with design>
                SenderId = _tileId,
                EventType = EntityEventType.Destroyed,
                Entity = new Magnifier{Identifier = Identifier}
            });

            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }

            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        #endregion
    }
}
