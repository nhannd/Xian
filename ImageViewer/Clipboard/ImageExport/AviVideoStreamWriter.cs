/* Adapted from CodeProject: http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * Thanks to:
 * 
 * Corinna John (Hannover, Germany)
 * cj@binary-universe.net
 * 
 * for making her code publicly available.
 */

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
		private int _aviFileRef = 0;
		private IntPtr _aviStreamRef = IntPtr.Zero;
		private IntPtr _aviCompressedStreamRef = IntPtr.Zero;

		private bool _initialized = false;
		private string _fileName;

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
			try
			{
				Close();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		private int FrameSize
		{
			get { return _width * _height * 4; }
		}

		#region Public Properties

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

		#region Public Methods

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

		public void Close()
		{
			try
			{
				ReleaseCompressedStream();
				ReleaseStream();
				ReleaseFile();
			}
			finally
			{
				_aviCompressedStreamRef = IntPtr.Zero;
				_aviStreamRef = IntPtr.Zero;
				_aviFileRef = 0;

				if (_initialized)
				{
					_initialized = false;
					Avi.AVIFileExit();
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Close();
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
		#endregion

		#region Private Methods

		private void CheckNotOpen()
		{
			if (IsOpen)
				throw new AviVideoStreamWriterException("Cannot change the stream's properties once it has been opened.");
		}

		private void Open()
		{
			Avi.AVIFileInit();
			_initialized = true;

			int result = Avi.AVIFileOpen(ref _aviFileRef, _fileName, Avi.OF_WRITE | Avi.OF_CREATE, 0);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi file could not be created.");

			CreateStream();
		}

		private void AddBitmapInternal(Bitmap bitmap)
		{
			//flip it.
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
			                                  ImageLockMode.ReadOnly, bitmap.PixelFormat);


			IntPtr outputStream = _aviStreamRef;
			if (_aviCompressedStreamRef != IntPtr.Zero)
				outputStream = _aviCompressedStreamRef;

			int result = Avi.AVIStreamWrite(outputStream, _frameCount++, 1, data.Scan0,
			                                (Int32)(data.Stride * data.Height), 0, 0, 0);

			bitmap.UnlockBits(data);

			//reset it.
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

			if (result != 0)
				throw new AviVideoStreamWriterException("Error adding bitmap to video stream.");

		}

		private static void GetHandlers(out int uncompressedHandler, out int compressedHandler)
		{
			int fccType = Avi.mmioStringToFOURCC("VIDC", 0);

			uncompressedHandler = 0;
			compressedHandler = 0;
			int compressedHandlerScore = 0;

			int i = 0;
			Avi.ICINFO handlerInfo = new Avi.ICINFO();
			while (0 != Avi.ICInfo(fccType, i++, ref handlerInfo))
			{
				IntPtr handle = Avi.ICOpen(fccType, handlerInfo.fccHandler, (Int32)Avi.ICModeFlags.ICMODE_QUERY);
				if (handle == IntPtr.Zero)
					continue;

				try
				{
					Avi.ICINFO queryHandlerInfo = new Avi.ICINFO();
					if (0 != Avi.ICGetInfo(handle, ref queryHandlerInfo, Marshal.SizeOf(queryHandlerInfo)))
					{
						int score = ScoreHandler(queryHandlerInfo);

						//take the first uncompressed handler
						if (uncompressedHandler == 0 && score == 0)
							uncompressedHandler = handlerInfo.fccHandler;

						//take the most flexible compressed handler
						if (score > compressedHandlerScore)
						{
							compressedHandlerScore = score;
							compressedHandler = handlerInfo.fccHandler;
						}
					}
				}
				finally
				{
					Avi.ICClose(handle);
				}
			}
		}

		private static int ScoreHandler(Avi.ICINFO compressorInfo)
		{
			int score = -1;

			Avi.ICInfoFlags flags = (Avi.ICInfoFlags)compressorInfo.dwFlags;
			if (0 == (flags & Avi.ICInfoFlags.VIDCF_COMPRESSFRAMES))
			{
				score = 0;
				score += (int)(flags & Avi.ICInfoFlags.VIDCF_QUALITY);
				score += (int)(flags & Avi.ICInfoFlags.VIDCF_TEMPORAL);
				score += (int)(flags & Avi.ICInfoFlags.VIDCF_FASTTEMPORALC);
			}

			return score;
		}

		private void CreateStream()
		{
			int uncompressedHandler, compressedHandler;
			GetHandlers(out uncompressedHandler, out compressedHandler);

			if (uncompressedHandler == 0 && compressedHandler == 0)
				throw new AviVideoStreamWriterException("Unable to locate acceptable video codec.");

			int handler = uncompressedHandler;
			if (compressedHandler != 0)
				handler = compressedHandler;

			Avi.AVISTREAMINFO streamInfo = GetStreamInfo(handler);

			int result = Avi.AVIFileCreateStream(_aviFileRef, out _aviStreamRef, ref streamInfo);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi stream could not be created.");

			if (compressedHandler <= 0)
				SetFormat(_aviStreamRef);
			else
				MakeStreamCompressed();
		}

		private void MakeStreamCompressed()
		{
			Avi.AVICOMPRESSOPTIONS compressOptions = GetCompressOptions();

			int result = Avi.AVIMakeCompressedStream(out _aviCompressedStreamRef, _aviStreamRef, ref compressOptions, 0);
			if (result != 0)
				throw new Exception("Failed to create compressed stream.");

			SetFormat(_aviCompressedStreamRef);
		}

		private Avi.AVISTREAMINFO GetStreamInfo(int handler)
		{
			Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
			
			streamInfo.fccType = Avi.streamtypeVIDEO;
			streamInfo.fccHandler = handler;
			streamInfo.dwFlags = 0;
			streamInfo.dwCaps = 0;
			streamInfo.wPriority = 0;
			streamInfo.wLanguage = 0;
			streamInfo.dwScale = (int)1;
			streamInfo.dwRate = (int)_frameRate;
			streamInfo.dwStart = 0;
			streamInfo.dwLength = 0;
			streamInfo.dwInitialFrames = 0;
			streamInfo.dwSuggestedBufferSize = FrameSize;
			streamInfo.dwQuality = _quality;
			streamInfo.dwSampleSize = 0;
			streamInfo.rcFrame.top = 0;
			streamInfo.rcFrame.left = 0;
			streamInfo.rcFrame.bottom = _height;
			streamInfo.rcFrame.right = _width;
			streamInfo.dwEditCount = 0;
			streamInfo.dwFormatChangeCount = 0;
			streamInfo.szName = new UInt16[64];

			return streamInfo;
		}

		private Avi.AVICOMPRESSOPTIONS GetCompressOptions()
		{
			Avi.AVICOMPRESSOPTIONS compressOptions = new Avi.AVICOMPRESSOPTIONS();

			compressOptions.fccType = Avi.streamtypeVIDEO;
			compressOptions.fccHandler = Avi.mmioStringToFOURCC("CVID", 0);
			compressOptions.dwKeyFrameEvery = 0;
			compressOptions.dwQuality = _quality;
			compressOptions.dwFlags = 0;
			compressOptions.dwBytesPerSecond = 0;
			compressOptions.lpFormat = IntPtr.Zero;
			compressOptions.cbFormat = 0;
			compressOptions.lpParms = IntPtr.Zero;
			compressOptions.cbParms = 0;
			compressOptions.dwInterleaveEvery = 0;

			return compressOptions;
		}

		private void SetFormat(IntPtr aviStream)
		{
			Avi.BITMAPINFOHEADER info = new Avi.BITMAPINFOHEADER();
			info.biSize = Marshal.SizeOf(info);
			info.biWidth = _width;
			info.biHeight = _height;
			info.biPlanes = 1;
			info.biBitCount = 32;
			info.biSizeImage = FrameSize;

			int result = Avi.AVIStreamSetFormat(aviStream, 0, ref info, info.biSize);
			if (result != 0)
				throw new AviVideoStreamWriterException("Avi stream format could not be set.");
		}

		private void ReleaseCompressedStream()
		{
			if (_aviCompressedStreamRef != IntPtr.Zero)
			{
				int result = Avi.AVIStreamRelease(_aviCompressedStreamRef);
				if (result != 0)
					throw new AviVideoStreamWriterException("Error releasing compressed Avi stream.");
			}
		}

		private void ReleaseStream()
		{
			if (_aviStreamRef != IntPtr.Zero)
			{
				int result = Avi.AVIStreamRelease(_aviStreamRef);
				if (result != 0)
					throw new AviVideoStreamWriterException("Error releasing Avi stream.");
			}
		}

		private void ReleaseFile()
		{
			if (_aviFileRef != 0)
			{
				int result = Avi.AVIFileRelease(_aviFileRef);
				if (result != 0)
					throw new AviVideoStreamWriterException("Error releasing Avi file.");
			}
		}

		#endregion
	}
}
