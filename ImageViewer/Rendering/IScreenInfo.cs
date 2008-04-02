#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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


using System.Drawing;
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Information about a display screen.
	/// </summary>
	public interface IScreenInfo
	{
		/// <summary>
		/// Gets the number of bits of memory, associated with one pixel of data.
		/// </summary>
		int BitsPerPixel { get; }

		/// <summary>
		/// Gets the bounds of the display.
		/// </summary>
		Rectangle Bounds { get; }

		/// <summary>
		/// Gets the device name associated with a display.
		/// </summary>
		string DeviceName { get; }

		/// <summary>
		/// Gets a value indicating whether a particular display is the primary device.
		/// </summary>
		bool Primary { get; }

		/// <summary>
		/// Gets the working area of the display. The working area is the desktop area 
		/// of the display, excluding taskbars, docked windows, and docked tool bars.
		/// </summary>
		Rectangle WorkingArea { get; }
	}
}
