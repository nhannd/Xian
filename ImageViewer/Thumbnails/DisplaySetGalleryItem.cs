using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	public partial class ThumbnailComponent
	{
		#region Drawing

		private static readonly int _iconWidth = 100;
		private static readonly int _iconHeight = 100;

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

		private static Bitmap CreateBitmap(IDisplaySet displaySet)
		{
			IPresentationImage image = GetMiddlePresentationImage(displaySet);
			if (image == null)
				return CreateDummyBitmap(SR.MessageNoImages);

			image = image.CreateFreshCopy();
			if (image is IAnnotationLayoutProvider)
				((IAnnotationLayoutProvider) image).AnnotationLayout.Visible = false;

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

		private class DisplaySetGalleryItemLoader : BlockingThreadPool<DisplaySetGalleryItem>
		{
			private readonly SynchronizationContext _uiThreadContext;
			private readonly ThumbnailComponent _parent;

			public DisplaySetGalleryItemLoader(ThumbnailComponent parent)
				: base(1, false)
			{
				_uiThreadContext = SynchronizationContext.Current;
				_parent = parent;
			}

			protected override void ProcessItem(DisplaySetGalleryItem item)
			{
				try
				{
					if (item.Valid)
					{
						Bitmap icon = CreateBitmap(item.DisplaySet);

						_uiThreadContext.Post(
							delegate
							{
								item.Image.Dispose();
								item.Image = icon;
								item.Complete();

								//only ever dispose on the main thread.
								if (!item.Valid)
									((IDisposable)item).Dispose();
								else 
									_parent.RefreshGalleryItem(item);

							}, null);
					}
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		private class DisplaySetGalleryItem : IGalleryItem, IDisposable
		{
			private volatile bool _valid;
			private volatile bool _complete;

			private IDisplaySet _displaySet;
			private Image _image;

			public DisplaySetGalleryItem(IDisplaySet displaySet)
			{
				_displaySet = displaySet.CreateFreshCopy();
				_image = CreateDummyBitmap(SR.MessageLoading);
				_valid = true;
				_complete = false;
			}

			#region Threaded Loader Properties / Methods

			public IDisplaySet DisplaySet
			{
				get { return _displaySet; }
			}

			public Image Image
			{
				get { return _image; }
				set { _image = value; }
			}

			public void Complete()
			{
				_complete = true;
			}

			#endregion

			public bool Valid
			{
				get { return _valid; }	
			}

			#region IGalleryItem Members

			Image IGalleryItem.Image
			{
				get { return _image; }
			}

			string IGalleryItem.Description
			{
				get { return _displaySet.Name; }
			}

			object IGalleryItem.Item
			{
				get { return _displaySet; }
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				_valid = false;

				//the threaded loader is still using the object and will dispose it when it's finished.
				if (!_complete)
					return;

				if (_displaySet != null)
				{
					_displaySet.Dispose();
					_displaySet = null;
				}

				if (_image != null)
				{
					_image.Dispose();
					_image = null;
				}
			}

			#endregion
		}
	}
}
