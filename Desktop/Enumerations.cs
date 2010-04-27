#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration of (potentially) available mouse buttons.
	/// </summary>
	[Flags]
	public enum XMouseButtons
	{
		/// <summary>
		/// Default value
		/// </summary>
		None		= 0x00000000,
		/// <summary>
		/// The left mouse button.
		/// </summary>
		Left		= 0x00100000,
		/// <summary>
		/// The right mouse button.
		/// </summary>
		Right		= 0x00200000,
		/// <summary>
		/// The middle mouse button.
		/// </summary>
		Middle		= 0x00400000,
		/// <summary>
		/// The 'x1' button.
		/// </summary>
		XButton1	= 0x00800000,
		/// <summary>
		/// The 'x2' button.
		/// </summary>
		XButton2	= 0x01000000
	}

	/// <summary>
	/// Enumeration for keyboard modifiers.
	/// </summary>
	[Flags]
	public enum ModifierFlags
	{
		/// <summary>
		/// Default value.
		/// </summary>
		None = 0,
		/// <summary>
		/// Any one of the 'control' keys.
		/// </summary>
		Control = 1,
		/// <summary>
		/// Any one of the 'alt' keys.
		/// </summary>
		Alt = 2,
		/// <summary>
		/// Any one of the 'shift' keys.
		/// </summary>
		Shift = 4
	}
}
