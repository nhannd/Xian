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

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A Voi Lut Manager, which is responsible for managing installation and restoration
	/// of Voi Luts via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// Implementors must not return null from the <see cref="GetLut"/> method.
	/// </remarks>
	/// <seealso cref="IVoiLutProvider"/>
	/// <seealso cref="IComposableLut"/>
	public interface IVoiLutManager : IMemorable
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IComposableLut"/>.</returns>
		IComposableLut GetLut();

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">The Lut to be installed.</param>
		void InstallLut(IComposableLut lut);

		/// <summary>
		/// Gets or sets whether the output of the Voi Lut should be inverted for display.
		/// </summary>
		bool Invert { get; set; }

		/// <summary>
		/// Toggles the state of the <see cref="Invert"/> property.
		/// </summary>
		void ToggleInvert();
	}
}
