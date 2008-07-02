using System.Drawing;
using ClearCanvas.Desktop;
using System;
using System.Threading;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	public partial class ThumbnailComponent
	{
		private delegate void ThumbnailLoadedCallback(Thumbnail thumbnail);

		private class Thumbnail : IGalleryItem, IDisposable
		{
			private static readonly int _iconWidth = 100;
			private static readonly int _iconHeight = 100;

			private readonly IDisplaySet _displaySet;

			private volatile IPresentationImage _image;
			private volatile Image _icon;

			private volatile SynchronizationContext _uiThreadContext;
			private volatile bool _loading = false;
			private volatile bool _disposed = false;

			private ThumbnailLoadedCallback _loadedCallback;

			public Thumbnail(IDisplaySet displaySet, ThumbnailLoadedCallback loadedCallback)
			{
				_displaySet = displaySet;

				_image = GetMiddlePresentationImage(displaySet);
				if (_image != null)
				{
					_image = _image.CreateFreshCopy();
					_icon = CreateDummyBitmap(SR.MessageLoading);
				}
				else
				{
					_icon = CreateDummyBitmap(SR.MessageNoImages);
				}

				_uiThreadContext = SynchronizationContext.Current;
				_loadedCallback = loadedCallback;
				_loading = false;
				_disposed = false;
			}

			#region IGalleryItem Members

			public Image Image
			{
				get { return _icon; }
			}

			public string Description
			{
				get { return _displaySet.Name; }
			}

			public object Item
			{
				get { return _displaySet; }
			}

			#endregion

			public void LoadAsync()
			{
				if (_image == null)
					return;

				_loading = true;
				ThreadPool.QueueUserWorkItem(LoadAsync);
			}

			#region IDisposable Members

			public void Dispose()
			{
				_disposed = true;
				if (_loading)
					return;

				DisposeInternal();
			}

			#endregion

			#region Private Methods

			private void DisposeInternal()
			{
				_disposed = true;

				_uiThreadContext = null;
				_loadedCallback = null;

				if (_image != null)
				{
					_image.Dispose();
					_image = null;
				}

				if (_icon != null)
				{
					_icon.Dispose();
					_icon = null;
				}
			}

			private void LoadAsync(object ignored)
			{
				Bitmap icon;
				try
				{
					icon = CreateBitmap(_image);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					icon = CreateDummyBitmap(SR.MessageLoadFailed);
				}

				_uiThreadContext.Post(this.OnLoaded, icon);
			}

			private void OnLoaded(object icon)
			{
				_icon.Dispose();
				_icon = (Bitmap)icon;

				_loading = false;

				if (!_disposed)
					_loadedCallback(this);
				else
					DisposeInternal();
			}

			private static Bitmap CreateDummyBitmap(string message)
			{
				Bitmap bmp = new Bitmap(_iconWidth, _iconHeight);
				System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp);

				Brush brush = new SolidBrush(Color.WhiteSmoke);
				Font font = new Font("Arial", 10.0f);

				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;

				graphics.DrawString(message, font, brush, new Rectangle(0, 0, _iconWidth, _iconHeight), format);
				DrawBorder(graphics);
				graphics.Dispose();

				format.Dispose();
				font.Dispose();
				brush.Dispose();

				return bmp;
			}

			private static Bitmap CreateBitmap(IPresentationImage image)
			{
				image = image.CreateFreshCopy();

				if (image is IAnnotationLayoutProvider)
					((IAnnotationLayoutProvider)image).AnnotationLayout.Visible = false;

				Bitmap bmp = image.DrawToBitmap(_iconWidth, _iconHeight);
				System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp);

				DrawBorder(graphics);

				image.Dispose();
				graphics.Dispose();

				return bmp;
			}

			private static void DrawBorder(System.Drawing.Graphics graphics)
			{
				using (Pen pen = new Pen(Color.DarkGray))
				{
					graphics.DrawRectangle(pen, 0, 0, _iconWidth - 1, _iconHeight - 1);
				}
			}

			private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
			{
				if (displaySet.PresentationImages.Count == 0)
					return null;

				if (displaySet.PresentationImages.Count <= 2)
					return displaySet.PresentationImages[0];

				return displaySet.PresentationImages[displaySet.PresentationImages.Count / 2];
			}

			#endregion
		}
	}
}