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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal partial class Avi
	{
		public class Codec
		{
			private static readonly Codec[] _installedCodecs = LoadInstalledCodecs();

			private ICINFO _icInfo;
			private string _description = null;
			private string _name = null;
			private string _driver = null;
			private readonly string _fourCCCode;

			private Codec(ICINFO icInfo)
			{
				_icInfo = icInfo;
				_fourCCCode = mmioFourCCToString(FourCCHandler); 
			}

			public string FourCCCode
			{
				get { return _fourCCCode; }	
			}

			public int FourCCHandler
			{
				get { return _icInfo.fccHandler; }
			}

			public string Name
			{
				get
				{
					if (_name == null)
						_name = StringFromShortArray(_icInfo.szName);

					return _name;
				}
			}

			public string Description
			{
				get
				{
					if (_description == null)
						_description = StringFromShortArray(_icInfo.szDescription);

					return _description;
				}
			}

			public string Driver
			{
				get
				{
					if (_driver == null)
						_driver = StringFromShortArray(_icInfo.szDriver);

					return _driver;
				}
			}

			public bool SupportsQuality
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_QUALITY); }
			}

			public bool SupportsCrunching
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_CRUNCH); }
			}

			public bool SupportsDraw
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_DRAW); }
			}

			public bool WantsCompressAllFramesMessage
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_COMPRESSFRAMES); }
			}

			public bool SupportsTemporalCompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_TEMPORAL); }
			}

			public bool SupportsFastTemporalCompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_FASTTEMPORALC); }
			}

			public bool SupportsFastTemporalDecompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_FASTTEMPORALD); }
			}

			public bool CanCompress(BITMAPINFOHEADER format)
			{
				return this == Find(format, this);
			}

			public static Codec Find(BITMAPINFOHEADER format, Codec preferredCodec)
			{
				ICModeFlags flags = ICModeFlags.ICMODE_COMPRESS | ICModeFlags.ICMODE_FASTCOMPRESS;
				Codec codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				flags = ICModeFlags.ICMODE_COMPRESS;
				codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				flags = 0;
				codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				return null;
			}

			public static Codec[] GetInstalledCodecs()
			{
				return _installedCodecs;
			}

			public static Codec GetInstalledCodec(string fccHandlerCode)
			{
				return CollectionUtils.SelectFirst(_installedCodecs,
					delegate(Codec codec)
					{
						return codec.FourCCCode == fccHandlerCode;
					});
			}

			public override string ToString()
			{
				return String.Format("{0} | {1} | {2}", Name, Description, Driver);
			}

			private static unsafe string StringFromShortArray(ushort[] source)
			{
				string value = "";
				if (source.Length == 0)
					return value;

				fixed (ushort* shortString = source)
				{
					char* charPtr = (char*)shortString;

					StringBuilder builder = new StringBuilder();
					for (int i = 0; i < source.Length; ++i)
					{
						char character = *charPtr++;
						if (character != '\0')
							builder.Append(character);
						else
							builder.Append("");
					}

					value = builder.ToString().Trim();
				}

				return value;
			}

			private static Codec Find(BITMAPINFOHEADER format, ICModeFlags flags, Codec preferredCodec)
			{
				int fccType = mmioStringToFOURCC("VIDC", 0);

				int preferredFccHandler = 0;
				if (preferredCodec != null)
					preferredFccHandler = preferredCodec.FourCCHandler;

				IntPtr handle = ICLocate(fccType, preferredFccHandler, ref format, IntPtr.Zero, (short)flags);
				GC.KeepAlive(format);

				if (handle != IntPtr.Zero)
				{
					ICINFO icInfo = new ICINFO();
					icInfo.dwSize = Marshal.SizeOf(icInfo);

					if (0 == ICGetInfo(handle, ref icInfo, icInfo.dwSize))
					{
						ICClose(handle);
						return null;
					}

					ICClose(handle);

					return CollectionUtils.SelectFirst(
						_installedCodecs, 
						delegate(Codec codec) { return codec.FourCCHandler == icInfo.fccHandler; });
				}

				return null;
			}

			private static Codec[] LoadInstalledCodecs()
			{
				List<Codec> codecs = new List<Codec>();

				int fccType = mmioStringToFOURCC("VIDC", 0);

				int i = 0;
				ICINFO handlerInfo = new ICINFO();
				handlerInfo.dwSize = Marshal.SizeOf(handlerInfo);

				while (0 != ICInfo(fccType, i++, ref handlerInfo))
				{
					IntPtr handle = ICOpen(fccType, handlerInfo.fccHandler, (Int32)ICModeFlags.ICMODE_QUERY);
					if (handle == IntPtr.Zero)
						continue;

					ICINFO queryHandlerInfo = new ICINFO();
					queryHandlerInfo.dwSize = Marshal.SizeOf(queryHandlerInfo);
					if (0 != ICGetInfo(handle, ref queryHandlerInfo, queryHandlerInfo.dwSize))
					{
						Codec codec = new Codec(queryHandlerInfo);
						if (!String.IsNullOrEmpty(codec.Name) &&
							null == codecs.Find(delegate(Codec test) { return test.Name == codec.Name; }))
						{
							codecs.Add(codec);
						}
					}

					ICClose(handle);
				}

				return codecs.ToArray();
			}
		}
	}
}