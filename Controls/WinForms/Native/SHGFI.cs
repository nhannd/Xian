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
	partial class Native
	{
		[Flags]
		public enum SHGFI
		{
			SHGFI_ICON = 0x000000100,
			SHGFI_DISPLAYNAME = 0x000000200,
			SHGFI_TYPENAME = 0x000000400,
			SHGFI_ATTRIBUTES = 0x000000800,
			SHGFI_ICONLOCATION = 0x000001000,
			SHGFI_EXETYPE = 0x000002000,
			SHGFI_SYSICONINDEX = 0x000004000,
			SHGFI_LINKOVERLAY = 0x000008000,
			SHGFI_SELECTED = 0x000010000,
			SHGFI_ATTR_SPECIFIED = 0x000020000,
			SHGFI_LARGEICON = 0x000000000,
			SHGFI_SMALLICON = 0x000000001,
			SHGFI_OPENICON = 0x000000002,
			SHGFI_SHELLICONSIZE = 0x000000004,
			SHGFI_PIDL = 0x000000008,
			SHGFI_USEFILEATTRIBUTES = 0x000000010,
			SHGFI_ADDOVERLAYS = 0x000000020,
			SHGFI_OVERLAYINDEX = 0x000000040
		}
	}
}