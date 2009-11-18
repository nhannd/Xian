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

namespace ClearCanvas.Controls.WinForms
{
	// ReSharper disable InconsistentNaming

	/// <summary>
	/// CSIDL values provide a unique system-independent way to identify special folders used frequently by applications, but which may not have the same name or location on any given system.
	/// </summary>
	internal partial class Native
	{
		[Flags]
		public enum CSIDL : uint
		{
			/// <summary>
			/// The virtual folder containing the objects in the user's Recycle Bin.
			/// </summary>
			CSIDL_BITBUCKET = 0x000A,

			/// <summary>
			/// The virtual folder containing icons for the Control Panel applications.
			/// </summary>
			CSIDL_CONTROLS = 0x0003,

			/// <summary>
			/// The virtual folder representing the Windows desktop, the root of the namespace.
			/// </summary>
			CSIDL_DESKTOP = 0x0000,

			/// <summary>
			/// The file system directory used to physically store file objects on the desktop (not to be confused with the desktop folder itself). A typical path is C:\Documents and Settings\username\Desktop.
			/// </summary>
			CSIDL_DESKTOPDIRECTORY = 0x0010,

			/// <summary>
			/// The virtual folder representing My Computer, containing everything on the local computer: storage devices, printers, and Control Panel. The folder may also contain mapped network drives.
			/// </summary>
			CSIDL_DRIVES = 0x0011,

			/// <summary>
			/// The virtual folder representing the My Documents desktop item.
			/// </summary>
			CSIDL_MYDOCUMENTS = 0x000C,

			/// <summary>
			/// The file system directory that serves as a common repository for music files. A typical path is C:\Documents and Settings\User\My Documents\My Music.
			/// </summary>
			CSIDL_MYMUSIC = 0x000D,

			/// <summary>
			/// The file system directory that serves as a common repository for image files. A typical path is C:\Documents and Settings\username\My Documents\My Pictures.
			/// </summary>
			CSIDL_MYPICTURES = 0x0027,

			/// <summary>
			/// The file system directory that serves as a common repository for video files. A typical path is C:\Documents and Settings\username\My Documents\My Videos. 
			/// </summary>
			CSIDL_MYVIDEO = 0x000E,

			/// <summary>
			/// A file system directory containing the link objects that may exist in the My Network Places virtual folder. It is not the same as CSIDL_NETWORK, which represents the network namespace root. A typical path is C:\Documents and Settings\username\NetHood.
			/// </summary>
			CSIDL_NETHOOD = 0x0013,

			/// <summary>
			/// A virtual folder representing Network Neighborhood, the root of the network namespace hierarchy. 
			/// </summary>
			CSIDL_NETWORK = 0x0012,

			/// <summary>
			/// The virtual folder representing the My Documents desktop item. This is equivalent to CSIDL_MYDOCUMENTS. 
			/// </summary>
			CSIDL_PERSONAL = 0x0005,

			/// <summary>
			/// The virtual folder containing installed printers. 
			/// </summary>
			CSIDL_PRINTERS = 0x0004,

			/// <summary>
			/// The file system directory that contains the link objects that can exist in the Printers virtual folder. A typical path is C:\Documents and Settings\username\PrintHood. 
			/// </summary>
			CSIDL_PRINTHOOD = 0x001B,

			/// <summary>
			/// The Windows directory or SYSROOT. This corresponds to the %windir% or %SYSTEMROOT% environment variables. A typical path is C:\Windows.
			/// </summary>
			CSIDL_WINDOWS = 0x0024,
		}
	}

	// ReSharper restore InconsistentNaming
}