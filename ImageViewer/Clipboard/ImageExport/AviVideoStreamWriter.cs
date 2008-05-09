// Adapted from CodeProject: http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx

using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal class AviVideoStreamWriterException : Exception
	{
		internal AviVideoStreamWriterException(string message)
			: base(message)
		{
		}
	}

	internal class AviVideoStreamWriter : IDisposable
	{
		private string _fileName;
		private int _initCount = 0;
		private int _aviFileRef = 0;
		private IntPtr _aviStreamRef = IntPtr.Zero;

		private int _width = 256;
		private int _height = 256;
		private int _quality = -1;
		private int _frameRate = 20;

		private int _frameCount = 0;

		public AviVideoStreamWriter()
		{
		}

		~AviVideoStreamWriter()
		{
			Dispose(false);
		}

		#region Properties

		private int FrameSize
		{
			get { return _width * _height * 4; }
		}

		public int Width
		{
			get { return _width; }
			set
			{
				Platform.CheckPositive(value, "Width");
				CheckNotOpen();
				_width = value;
			}
		}

		public int Height
		{
			get { return _height; }
			set
			{
				Platform.CheckPositive(value, "Height");
				CheckNotOpen();
				_height = value;
			}
		}

		public int Quality
		{
			get { return _quality; }
			set
			{
				Platform.CheckPositive(value, "Quality");
				CheckNotOpen(); 
				_quality = value;
			}
		}

		public int FrameRate
		{
			get { return _frameRate; }
			set
			{
				Platform.CheckPositive(value, "FrameRate");
				CheckNotOpen();
				_frameRate = value;
			}
		}

		public bool IsOpen
		{
			get { return _aviFileRef != 0; }	
		}

		#endregion

		#region Methods

		private void CheckNotOpen()
		{
			if (_aviFileRef != 0)
				throw new AviVideoStreamWriterException("Cannot change the stream's properties once it has been opened.");
		}

		private void Open()
		{
			Avi.AVIFileInit();

			++_initCount;

			int result = Avi.AVIFileOpen(ref _aviFileRef, _fileName, Avi.OF_WRITE | Avi.OF_CREATE, 0);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi file could not be created.");

			CreateStream();

			SetFormat();
		}

		private void CreateStream()
		{
			Avi.AVISTREAMINFO strhdr = new Avi.AVISTREAMINFO();
			strhdr.fccType = Avi.mmioStringToFOURCC("vids", 0);
			//TODO: 
			strhdr.fccHandler = Avi.mmioStringToFOURCC("CVID", 0);
			strhdr.dwFlags = 0;
			strhdr.dwCaps = 0;
			strhdr.wPriority = 0;
			strhdr.wLanguage = 0;
			strhdr.dwScale = (int)1;
			strhdr.dwRate = (int)_frameRate;
			strhdr.dwStart = 0;
			strhdr.dwLength = 0;
			strhdr.dwInitialFrames = 0;
			strhdr.dwSuggestedBufferSize = FrameSize;
			strhdr.dwQuality = _quality;
			strhdr.dwSampleSize = 0;
			strhdr.rcFrame.top = 0;
			strhdr.rcFrame.left = 0;
			strhdr.rcFrame.bottom = (uint)_height;
			strhdr.rcFrame.right = (uint)_width;
			strhdr.dwEditCount = 0;
			strhdr.dwFormatChangeCount = 0;
			strhdr.szName = new UInt16[64];

			int result = Avi.AVIFileCreateStream(_aviFileRef, out _aviStreamRef, ref strhdr);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi stream could not be created.");
		}

		private void SetFormat()
		{
			Avi.BITMAPINFOHEADER info = new Avi.BITMAPINFOHEADER();
			info.biSize = Marshal.SizeOf(info);
			info.biWidth = _width;
			info.biHeight = _height;
			info.biPlanes = 1;
			info.biBitCount = 32;
			info.biSizeImage = FrameSize;

			int result = Avi.AVIStreamSetFormat(_aviStreamRef, 0, ref info, info.biSize);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi stream format could not be set.");
		}

		public void Open(string fileName)
		{
			if (this.IsOpen)
				throw new AviVideoStreamWriterException("The avi file is already open.");

			_fileName = fileName;
			Platform.CheckForEmptyString(_fileName, "fileName");

			Open();
		}

		public void AddBitmap(Bitmap bitmap)
		{
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				throw new AviVideoStreamWriterException("Bitmap must be standard 32 bit ARGB.");

			bool dispose = false;

			if (bitmap.Width != _width || bitmap.Height != _height)
			{
				bitmap = new Bitmap(bitmap, _width, _height);
				dispose = true;
			}

			try
			{
				AddBitmapInternal(bitmap);
			}
			finally
			{
				if (dispose)
					bitmap.Dispose();
			}
		}

		private void AddBitmapInternal(Bitmap bitmap)
		{
			//flip it.
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
										ImageLockMode.ReadOnly, bitmap.PixelFormat);


			int result = Avi.AVIStreamWrite(_aviStreamRef, _frameCount++, 1, data.Scan0,
													(Int32)(data.Stride * data.Height), 0, 0, 0);

			bitmap.UnlockBits(data);

			//reset it.
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			if (result != 0)
				throw new AviVideoStreamWriterException("Error adding bitmap to video stream.");

		}

		public void Close()
		{
			try
			{
				if (_aviStreamRef != IntPtr.Zero)
				{
					int result = Avi.AVIStreamRelease(_aviStreamRef);
					if (result != 0)
						throw new AviVideoStreamWriterException("Error releasing Avi stream.");
				}

				if (_aviFileRef != 0)
				{
					int result = Avi.AVIFileRelease(_aviFileRef);
					if (result != 0)
						throw new AviVideoStreamWriterException("Error releasing Avi file.");
				}

				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
			finally
			{
				_aviFileRef = 0;
				_aviStreamRef = IntPtr.Zero;
				if (_initCount > 0)
				{
					--_initCount;
					Avi.AVIFileExit();
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			Close();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
		#endregion
	}
}
