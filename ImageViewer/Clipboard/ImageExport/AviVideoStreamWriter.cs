#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

/* Adapted from CodeProject: http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * Thanks to:
 * 
 * Corinna John (Hannover, Germany)
 * cj@binary-universe.net
 * 
 * for making her code publicly available.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal partial class Avi
	{
		public class NoCodecFoundException : VideoStreamWriterException
		{
			internal NoCodecFoundException(string message)
				: base(message)
			{
			}
		}

		public class VideoStreamWriterException : Exception
		{
			internal VideoStreamWriterException(string message)
				: base(message)
			{
			}
		}

		public class VideoStreamWriter : IDisposable
		{
			private readonly Codec _codec;

			private int _aviFileRef = 0;
			private IntPtr _aviStreamRef = IntPtr.Zero;
			private IntPtr _aviCompressedStreamRef = IntPtr.Zero;

			private bool _initialized = false;
			private string _fileName;

			private int _width = 512;
			private int _height = 512;
			private int _quality = -1;
			private int _frameRate = 20;

			private int _frameCount = 0;

			public VideoStreamWriter(Codec codec)
			{
				Platform.CheckForNullReference(codec, "codec");
				_codec = codec;
			}

			~VideoStreamWriter()
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
				get { return _width * _height * 3; }
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
					Platform.CheckArgumentRange(value, -1, 100, "Quality");
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

			public Codec Codec
			{
				get { return _codec; }
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
					throw new VideoStreamWriterException("The avi file is already open.");

				_fileName = fileName;
				Platform.CheckForEmptyString(_fileName, "fileName");

				Open();
			}

			public void AddBitmap(Bitmap bitmap)
			{
				bool convert = false;
				if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
				{
					convert = true;
				}
				else if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
				{
					throw new VideoStreamWriterException("Bitmap must be standard 24 bit RGB or 32 bit ARGB.");
				}

				if (bitmap.Width != Width || bitmap.Height != Height)
					convert = true;

				bool dispose = false;
				if (convert)
				{
					dispose = true;
					Bitmap old = bitmap;
					bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
					System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.DrawImage(old, 0, 0, Width, Height);
					g.Dispose();
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
				catch (Exception e)
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
					throw new VideoStreamWriterException("Cannot change the stream's properties once it has been opened.");
			}

			private void Open()
			{
				Avi.AVIFileInit();
				_initialized = true;

				int result = Avi.AVIFileOpen(ref _aviFileRef, _fileName, Avi.OF_WRITE | Avi.OF_CREATE, 0);
				if (result != 0)
					throw new VideoStreamWriterException("Avi file could not be created.");

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
					throw new VideoStreamWriterException("Error adding bitmap to video stream.");

			}

			private void CreateStream()
			{
				Avi.AVISTREAMINFO streamInfo = GetStreamInfo();

				int result = Avi.AVIFileCreateStream(_aviFileRef, out _aviStreamRef, ref streamInfo);
				if (result != 0)
					throw new VideoStreamWriterException("Avi stream could not be created.");

				GC.KeepAlive(streamInfo);

				if (!Codec.SupportsQuality)
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

				GC.KeepAlive(compressOptions);

				SetFormat(_aviCompressedStreamRef);
			}

			private Avi.AVISTREAMINFO GetStreamInfo()
			{
				Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
				streamInfo.fccType = Avi.streamtypeVIDEO;
				streamInfo.fccHandler = this.Codec.FourCCHandler;
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
				streamInfo.dwQuality = ComputeQuality();
				streamInfo.dwSampleSize = 0;
				streamInfo.rcFrame.top = 0;
				streamInfo.rcFrame.left = 0;
				streamInfo.rcFrame.bottom = Height;
				streamInfo.rcFrame.right = Width;
				streamInfo.dwEditCount = 0;
				streamInfo.dwFormatChangeCount = 0;
				streamInfo.szName = new UInt16[64];

				return streamInfo;
			}

			private Avi.AVICOMPRESSOPTIONS GetCompressOptions()
			{
				Avi.AVICOMPRESSOPTIONS compressOptions = new Avi.AVICOMPRESSOPTIONS();

				compressOptions.fccType = Avi.streamtypeVIDEO;
				compressOptions.fccHandler = this.Codec.FourCCHandler;
				compressOptions.dwKeyFrameEvery = 0;
				compressOptions.dwQuality = ComputeQuality();
				compressOptions.dwFlags = 0;
				compressOptions.dwBytesPerSecond = 0;
				compressOptions.lpFormat = IntPtr.Zero;
				compressOptions.cbFormat = 0;
				compressOptions.lpParms = IntPtr.Zero;
				compressOptions.cbParms = 0;
				compressOptions.dwInterleaveEvery = 0;

				return compressOptions;
			}

			private Avi.BITMAPINFOHEADER GetImageFormat()
			{
				Avi.BITMAPINFOHEADER format = new Avi.BITMAPINFOHEADER();
				format.biSize = Marshal.SizeOf(format);
				format.biPlanes = 1;
				format.biBitCount = 24;
				format.biCompression = 0; //RGB
				format.biHeight = Height;
				format.biWidth = Width;
				format.biSizeImage = FrameSize;
				return format;
			}

			private void SetFormat(IntPtr aviStream)
			{
				Avi.BITMAPINFOHEADER info = GetImageFormat();

				int result = Avi.AVIStreamSetFormat(aviStream, 0, ref info, info.biSize);
				if (result != 0)
					throw new VideoStreamWriterException("Avi stream format could not be set.");

				GC.KeepAlive(info);
			}

			private int ComputeQuality()
			{
				if (!Codec.SupportsQuality)
					return -1;

				if (_quality == -1)
					return _quality;

				return _quality * 100;
			}

			private void ReleaseCompressedStream()
			{
				if (_aviCompressedStreamRef != IntPtr.Zero)
				{
					int result = Avi.AVIStreamRelease(_aviCompressedStreamRef);
					if (result != 0)
						throw new VideoStreamWriterException("Error releasing compressed Avi stream.");
				}
			}

			private void ReleaseStream()
			{
				if (_aviStreamRef != IntPtr.Zero)
				{
					int result = Avi.AVIStreamRelease(_aviStreamRef);
					if (result != 0)
						throw new VideoStreamWriterException("Error releasing Avi stream.");
				}
			}

			private void ReleaseFile()
			{
				if (_aviFileRef != 0)
				{
					int result = Avi.AVIFileRelease(_aviFileRef);
					if (result != 0)
						throw new VideoStreamWriterException("Error releasing Avi file.");
				}
			}

			#endregion
		}
	}
}