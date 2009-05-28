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

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Enumerated values defining the units of measurement used in various calculations in the <see cref="ClearCanvas.ImageViewer.RoiGraphics"/> namespace.
	/// </summary>
	/// <remarks>
	/// Depending on the specific context, the enumerated values can also represent areas or volumes. For example, if a method that computes area
	/// is given an argument of <see cref="Centimeters"/>, then the output should be interpreted to be in square centimetres. Similarly, if a
	/// method that computes volume is given <see cref="Pixels"/>, then the output should be interpreted to be in cubic pixels.
	/// </remarks>
	public enum Units
	{
		/// <summary>
		/// Indicates that the measurement is in units of image pixels (or square pixels, or cubic pixels).
		/// </summary>
		Pixels,

		/// <summary>
		/// Indicates that the measurement is in units of millimetres (or square millimetres, or cubic millimetres).
		/// </summary>
		Millimeters,

		/// <summary>
		/// Indicates that the measurement is int units of centimetres (or square centimetres, or cubic centimetres).
		/// </summary>
		Centimeters
	}
}