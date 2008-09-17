using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public enum GraphicsSource
	{
		Default = 0,
		Hdc = 1
	}

	public class TestImageRenderer
	{
		private Size _size;
		private bool _customBackBuffer;
		private PixelFormat _pixelFormat;
		private GraphicsSource _graphicsSource;
		private bool _useBufferedGraphics;
		private bool _bufferedGraphicsNew;

		private Bitmap _customImage;

		private Bitmap _image;
		private Bitmap _buffer;
		private BufferedGraphicsContext _context;
		private BufferedGraphics _bufferedGraphics;

		public TestImageRenderer()
		{
			_customBackBuffer = true;
			_bufferedGraphicsNew = true;

			Format = PixelFormat.Format24bppRgb;
			Source = GraphicsSource.Default;
		}

		public IList<PixelFormat> GetPixelFormats()
		{
			List<PixelFormat> formats = new List<PixelFormat>();
			formats.Add(PixelFormat.Format24bppRgb);
			formats.Add(PixelFormat.Format32bppPArgb);
			return formats;
		}

		public IList<GraphicsSource> GetGraphicsSources()
		{
			List<GraphicsSource> sources = new List<GraphicsSource>();
			Array values = Enum.GetValues(typeof(GraphicsSource));
			foreach (GraphicsSource value in values)
				sources.Add(value);
			return sources;
		}

		public Bitmap CustomImage
		{
			get { return _customImage; }
			set 
			{ 
				if (_customImage != null)
					_customImage.Dispose();

				_customImage = value;
				DisposeBuffer();
			}
		}

		public bool CustomBackBuffer
		{
			get { return _customBackBuffer; }	
			set
			{
				_customBackBuffer = value;
				DisposeBuffer();
			}
		}

		public bool UseBufferedGraphics
		{
			get { return _useBufferedGraphics; }
			set
			{
				_useBufferedGraphics = value;
				DisposeBuffer();
			}
		}

		public PixelFormat Format
		{
			get { return _pixelFormat; }
			set
			{
				_pixelFormat = value;
				DisposeBuffer();
				GenerateImage();
			}
		}

		public GraphicsSource Source
		{
			get { return _graphicsSource; }
			set
			{
				_graphicsSource = value;
				DisposeBuffer();
			}
		}

		public void RenderTo(Graphics graphics, Size size)
		{
			Graphics target = graphics;
			IntPtr hdc = IntPtr.Zero;
			if (_graphicsSource == GraphicsSource.Hdc)
			{
				hdc = graphics.GetHdc();
				target = Graphics.FromHdc(hdc);
			}

			if (_customBackBuffer)
			{
				AllocateBuffer(target, size);
				if (_buffer != null)
				{
					using (Graphics buffer = Graphics.FromImage(_buffer))
					{
						Render(buffer, size);
					}
					target.DrawImage(_buffer, 0, 0);
				}
				else
				{
					if (_bufferedGraphicsNew)
					{
						_bufferedGraphicsNew = false;
						Render(_bufferedGraphics.Graphics, size);
					}

					_bufferedGraphics.Render(target);
				}
			}
			else
			{
				Render(target, size);
			}

			if (hdc != IntPtr.Zero)
			{
				graphics.ReleaseHdc(hdc);
				target.Dispose();
			}

			_size = size;
		}

		private void DisposeBuffer()
		{
			if (_buffer != null)
			{
				_buffer.Dispose();
				_buffer = null;
			}
			
			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}

		private void AllocateBuffer(Graphics target, Size size)
		{
			if (!_useBufferedGraphics)
			{
				if (_buffer != null && size != _size)
					DisposeBuffer();

				if (_buffer == null)
					_buffer = new Bitmap(size.Width, size.Height, _pixelFormat);
			}
			else
			{
				if (_bufferedGraphics != null && size != _size)
					DisposeBuffer();

				if (_context == null)
					_context = BufferedGraphicsManager.Current;

				if (_bufferedGraphics == null)
				{
					_context.MaximumBuffer = new Size(size.Width + 1, size.Height + 1);
					_bufferedGraphics = _context.Allocate(target, new Rectangle(Point.Empty, size));
					_bufferedGraphicsNew = true;
				}
			}
		}

		private void Render(Graphics graphics, Size size)
		{
			graphics.Clear(Color.Black);
			if (_customImage != null)
				graphics.DrawImage(_customImage, 0, 0, size.Width, size.Height);
			else
				graphics.DrawImage(_image, 0, 0, size.Width, size.Height);

			_bufferedGraphicsNew = false;
		}

		private void GenerateImage()
		{
			if (_image != null)
				_image.Dispose();

			_image = new Bitmap(200, 200, _pixelFormat);

			Graphics graphics = Graphics.FromImage(_image);
			Pen pen = new Pen(Brushes.Red, 10);
			graphics.DrawRectangle(pen, 25, 25, 50, 50);
			pen.Dispose();

			pen = new Pen(Brushes.Green, 10);
			graphics.DrawRectangle(pen, 75, 75, 50, 50);
			pen.Dispose();

			pen = new Pen(Brushes.Blue, 10);
			graphics.DrawRectangle(pen, 125, 125, 50, 50);
			pen.Dispose();
			graphics.Dispose();
		}
	}
}
