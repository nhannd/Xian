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
using System.Runtime.InteropServices;


namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{

	internal partial class Avi{

		public static int PALETTE_SIZE = 4*256; //RGBQUAD * 256 colours

		public static readonly int streamtypeVIDEO = mmioFOURCC('v', 'i', 'd', 's');
		public static readonly int streamtypeAUDIO = mmioFOURCC('a', 'u', 'd', 's');
		public static readonly int streamtypeMIDI = mmioFOURCC('m', 'i', 'd', 's');
		public static readonly int streamtypeTEXT = mmioFOURCC('t', 'x', 't', 's');
		
		public const int OF_SHARE_DENY_WRITE = 32;
		public const int OF_WRITE			 = 1;
		public const int OF_READWRITE		 = 2;
		public const int OF_CREATE			 = 4096;

		public const int BMP_MAGIC_COOKIE = 19778; //ascii string "BM"

		public const int AVICOMPRESSF_INTERLEAVE = 0x00000001;    // interleave
		public const int AVICOMPRESSF_DATARATE = 0x00000002;    // use a data rate
		public const int AVICOMPRESSF_KEYFRAMES = 0x00000004;    // use keyframes
		public const int AVICOMPRESSF_VALID = 0x00000008;    // has valid data
		public const int AVIIF_KEYFRAME = 0x00000010;

		public const UInt32 ICMF_CHOOSE_KEYFRAME = 0x0001;	// show KeyFrame Every box
		public const UInt32 ICMF_CHOOSE_DATARATE = 0x0002;	// show DataRate box
		public const UInt32 ICMF_CHOOSE_PREVIEW  = 0x0004;	// allow expanded preview dialog

        //macro mmioFOURCC
        public static Int32 mmioFOURCC(char ch0, char ch1, char ch2, char ch3) {
            return ((Int32)(byte)(ch0) | ((byte)(ch1) << 8) |
                ((byte)(ch2) << 16) | ((byte)(ch3) << 24));
        }

		public static string mmioFourCCToString(Int32 code)
		{
			char[] values = new char[4];
			values[0] = (char)((code & 0x000000ff));
			values[1] = (char)((code & 0x0000ff00) >> 8);
			values[2] = (char)((code & 0x00ff0000) >> 16);
			values[3] = (char)((code & 0xff000000) >> 24);

			return new String(values);
		}

		#region structure declarations

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct RECT{ 
			public Int32 left; 
			public Int32 top; 
			public Int32 right; 
			public Int32 bottom; 
		} 		

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct BITMAPINFOHEADER {
			public Int32 biSize;
			public Int32 biWidth;
			public Int32 biHeight;
			public Int16 biPlanes;
			public Int16 biBitCount;
			public Int32 biCompression;
			public Int32 biSizeImage;
			public Int32 biXPelsPerMeter;
			public Int32 biYPelsPerMeter;
			public Int32 biClrUsed;
			public Int32 biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential)] 
		public struct PCMWAVEFORMAT {
			public short wFormatTag;
			public short nChannels;
			public int nSamplesPerSec;
			public int nAvgBytesPerSec;
			public short nBlockAlign;
			public short wBitsPerSample;
			public short cbSize;
		}

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVISTREAMINFO {
			public Int32    fccType;
			public Int32    fccHandler;
			public Int32    dwFlags;
			public Int32    dwCaps;
			public Int16    wPriority;
			public Int16    wLanguage;
			public Int32    dwScale;
			public Int32    dwRate;
			public Int32    dwStart;
			public Int32    dwLength;
			public Int32    dwInitialFrames;
			public Int32    dwSuggestedBufferSize;
			public Int32    dwQuality;
			public Int32    dwSampleSize;
			public RECT		rcFrame;
			public Int32    dwEditCount;
			public Int32    dwFormatChangeCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public UInt16[]    szName;
		}
		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct BITMAPFILEHEADER{
			public Int16 bfType; //"magic cookie" - must be "BM"
			public Int32 bfSize;
			public Int16 bfReserved1;
			public Int16 bfReserved2;
			public Int32 bfOffBits;
		}

				
		[StructLayout(LayoutKind.Sequential, Pack=1)]
			public struct AVIFILEINFO{
			public Int32 dwMaxBytesPerSecond;
			public Int32 dwFlags;
			public Int32 dwCaps;
			public Int32 dwStreams;
			public Int32 dwSuggestedBufferSize;
			public Int32 dwWidth;
			public Int32 dwHeight;
			public Int32 dwScale;
			public Int32 dwRate;
			public Int32 dwLength;
			public Int32 dwEditCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public char[] szFileType;
		}

		[StructLayout(LayoutKind.Sequential, Pack=1)]
			public struct AVICOMPRESSOPTIONS {
			public Int32   fccType;
			public Int32   fccHandler;
			public Int32   dwKeyFrameEvery;  // only used with AVICOMRPESSF_KEYFRAMES
			public Int32   dwQuality;
			public Int32   dwBytesPerSecond; // only used with AVICOMPRESSF_DATARATE
			public Int32   dwFlags;
			public IntPtr   lpFormat;
			public Int32   cbFormat;
			public IntPtr   lpParms;
			public Int32   cbParms;
			public Int32   dwInterleaveEvery;
		}

		/// <summary>AviSaveV needs a pointer to a pointer to an AVICOMPRESSOPTIONS structure</summary>
		[StructLayout(LayoutKind.Sequential, Pack=1)]
			public class AVICOMPRESSOPTIONS_CLASS {
			public Int32   fccType;
			public Int32   fccHandler;
			public Int32   dwKeyFrameEvery;  // only used with AVICOMRPESSF_KEYFRAMES
			public Int32   dwQuality;
			public Int32   dwBytesPerSecond; // only used with AVICOMPRESSF_DATARATE
			public Int32   dwFlags;
			public IntPtr   lpFormat;
			public Int32   cbFormat;
			public IntPtr   lpParms;
			public Int32   cbParms;
			public Int32   dwInterleaveEvery;

			public AVICOMPRESSOPTIONS ToStruct(){
				AVICOMPRESSOPTIONS returnVar = new AVICOMPRESSOPTIONS();
				returnVar.fccType = this.fccType;
				returnVar.fccHandler = this.fccHandler;
				returnVar.dwKeyFrameEvery = this.dwKeyFrameEvery;
				returnVar.dwQuality = this.dwQuality;
				returnVar.dwBytesPerSecond = this.dwBytesPerSecond;
				returnVar.dwFlags = this.dwFlags;
				returnVar.lpFormat = this.lpFormat;
				returnVar.cbFormat = this.cbFormat;
				returnVar.lpParms = this.lpParms;
				returnVar.cbParms = this.cbParms;
				returnVar.dwInterleaveEvery = this.dwInterleaveEvery;
				return returnVar;
			}
		}

		#endregion structure declarations

		#region method declarations
	
		//Initialize the AVI library
		[DllImport("avifil32.dll")]
		public static extern void AVIFileInit();

		//Open an AVI file
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIFileOpen(
			ref int ppfile,
			String szFile,
			int uMode,
			int pclsidHandler);

		//Get a stream from an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileGetStream(
			int pfile,
			out IntPtr ppavi,  
			int fccType,       
			int lParam);

		//Get the start position of a stream
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIStreamStart(int pavi);

		//Get the length of a stream in frames
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIStreamLength(int pavi);

		//Get information about an open stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamInfo(
			IntPtr pAVIStream,
			ref AVISTREAMINFO psi,
			int lSize);

		//Get a pointer to a GETFRAME object (returns 0 on error)
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameOpen(
			IntPtr pAVIStream,
			ref BITMAPINFOHEADER bih);

		//Get a pointer to a packed DIB (returns 0 on error)
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrame(
			int pGetFrameObj,
			int lPos);

		//Create a new stream in an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileCreateStream(
			int pfile,
			out IntPtr ppavi, 
			ref AVISTREAMINFO ptr_streaminfo);

        //Create an editable stream
        [DllImport("avifil32.dll")]
        public static extern int CreateEditableStream(
            ref IntPtr ppsEditable,
            IntPtr psSource
        );

        //Cut samples from an editable stream
        [DllImport("avifil32.dll")]
        public static extern int EditStreamCut(
            IntPtr pStream,
            ref Int32 plStart,
            ref Int32 plLength,
            ref IntPtr ppResult
        );

        //Copy a part of an editable stream
        [DllImport("avifil32.dll")]
        public static extern int EditStreamCopy(
            IntPtr pStream,
            ref Int32 plStart,
            ref Int32 plLength,
            ref IntPtr ppResult
        );

        //Paste an editable stream into another editable stream
        [DllImport("avifil32.dll")]
        public static extern int EditStreamPaste(
            IntPtr pStream,
            ref Int32 plPos,
            ref Int32 plLength,
            IntPtr pstream,
            Int32 lStart,
            Int32 lLength
        );

        //Change a stream's header values
        [DllImport("avifil32.dll")]
        public static extern int EditStreamSetInfo(
            IntPtr pStream,
            ref AVISTREAMINFO lpInfo,
            Int32 cbInfo
        );

        [DllImport("avifil32.dll")]
        public static extern int AVIMakeFileFromStreams(
            ref IntPtr ppfile,
            int nStreams,
            ref IntPtr papStreams
        );

        //Set the format for a new stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamSetFormat(
			IntPtr aviStream, Int32 lPos, 
			ref BITMAPINFOHEADER lpFormat, Int32 cbFormat);
		
		//Set the format for a new stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamSetFormat(
			IntPtr aviStream, Int32 lPos, 
			ref PCMWAVEFORMAT lpFormat, Int32 cbFormat);
		
		//Read the format for a stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamReadFormat(
			IntPtr aviStream, Int32 lPos,
			ref BITMAPINFOHEADER lpFormat, ref Int32 cbFormat
			);

		//Read the size of the format for a stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamReadFormat(
			IntPtr aviStream, Int32 lPos,
			int empty, ref Int32 cbFormat
			);
		
		//Read the format for a stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamReadFormat(
			IntPtr aviStream, Int32 lPos,
			ref PCMWAVEFORMAT lpFormat, ref Int32 cbFormat
			);

		//Write a sample to a stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamWrite(
			IntPtr aviStream, Int32 lStart, Int32 lSamples, 
			IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags, 
			Int32 dummy1, Int32 dummy2);

		//Release the GETFRAME object
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameClose(
			int pGetFrameObj);

		//Release an open AVI stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamRelease(IntPtr aviStream);

		//Release an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileRelease(int pfile);

		//Close the AVI library
		[DllImport("avifil32.dll")]
		public static extern void AVIFileExit();

		[DllImport("avifil32.dll")]
		public static extern int AVIMakeCompressedStream(
			out IntPtr ppsCompressed, IntPtr aviStream, 
			ref AVICOMPRESSOPTIONS ao, int dummy);

		[DllImport("avifil32.dll")]
		public static extern bool AVISaveOptions(
			IntPtr hwnd,
			UInt32 uiFlags,              
			Int32 nStreams,                      
			ref IntPtr ppavi,
			ref AVICOMPRESSOPTIONS_CLASS plpOptions  
			);

		[DllImport("avifil32.dll")]
		public static extern long AVISaveOptionsFree(
			int nStreams,
			ref AVICOMPRESSOPTIONS_CLASS plpOptions  
			);

		[DllImport("avifil32.dll")]
		public static extern int AVIFileInfo(
			int pfile, 
			ref AVIFILEINFO pfi,
			int lSize);

		[DllImport("winmm.dll", EntryPoint="mmioStringToFOURCCA")]
		public static extern int mmioStringToFOURCC(String sz, int uFlags);

		[DllImport("avifil32.dll")]
		public static extern int AVIStreamRead(
			IntPtr pavi, 
			Int32 lStart,     
			Int32 lSamples,   
			IntPtr lpBuffer, 
			Int32 cbBuffer,   
			Int32  plBytes,  
			Int32  plSamples 
			);

		[DllImport("avifil32.dll")]
		public static extern int AVISaveV(
			String szFile,
			Int16 empty,
			Int16 lpfnCallback,
			Int16 nStreams,
			ref IntPtr ppavi,
			ref AVICOMPRESSOPTIONS_CLASS plpOptions
			);

		#endregion method declarations

		#region Video Compression Manager

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ICINFO
		{
			public Int32 dwSize;
			public Int32 fccType;
			public Int32 fccHandler;
			public Int32 dwFlags;
			public Int32 dwVersion;
			public Int32 dwVersionICM;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public UInt16[] szName;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public UInt16[] szDescription;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public UInt16[] szDriver;
		}; 

		[Flags]
		public enum ICInfoFlags
		{
			VIDCF_QUALITY = 0x0001, // supports quality
			VIDCF_CRUNCH = 0x0002, // supports crunching to a frame size
			VIDCF_TEMPORAL = 0x0004, // supports inter-frame compress
			VIDCF_COMPRESSFRAMES = 0x0008, // wants the compress all frames message
			VIDCF_DRAW = 0x0010, // supports drawing
			VIDCF_FASTTEMPORALC = 0x0020, // does not need prev frame on compress
			VIDCF_FASTTEMPORALD = 0x0080 // does not need prev frame on decompress
		}

		[DllImport("msvfw32.dll")]
		public static extern int ICInfo(
			Int32 fccType,
			Int32 fccHandler,
			ref ICINFO icInfo);

		[DllImport("msvfw32.dll")]
		public static extern IntPtr ICOpen(
			Int32 fccType,
			Int32 fccHandler,
			Int32 wMode
			);

		[DllImport("msvfw32.dll")]
		public static extern Int32 ICClose(IntPtr hic);

		[Flags]
		public enum ICModeFlags
		{
			ICMODE_COMPRESS = 1,
			ICMODE_DECOMPRESS = 2,
			ICMODE_FASTDECOMPRESS = 3,
			ICMODE_QUERY = 4,
			ICMODE_FASTCOMPRESS = 5,
			ICMODE_DRAW = 8
		}

		[DllImport("msvfw32.dll")]
		public static extern Int32 ICGetInfo(IntPtr hic, ref ICINFO picinfo, Int32 size);

		[DllImport("msvfw32.dll")]
		public static extern IntPtr ICLocate(
		  Int32 fccType,
		  Int32 fccHandler,
		  ref BITMAPINFOHEADER lpbiIn,
		  IntPtr lpbiOut,
		  Int16 wFlags);

		#endregion
	}
}
