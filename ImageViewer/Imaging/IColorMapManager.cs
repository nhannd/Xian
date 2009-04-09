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

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: the color map shouldn't *have* to be a data lut - it could be calculated.

	/// <summary>
	/// A Color Map Manager, which is responsible for managing installation and restoration
	/// of color maps via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementors can maintain the named color maps any way they choose.
	/// However, the <see cref="ColorMapFactoryExtensionPoint"/> is the preferred method of 
	/// creating new color maps.
	/// </para>
	/// <para>
	/// Implementors must not return null from the <see cref="GetColorMap"/> method.
	/// </para>
	/// </remarks>
	public interface IColorMapManager : IMemorable
	{
		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		IDataLut GetColorMap();

		/// <summary>
		/// Installs a color map by name.
		/// </summary>
		/// <param name="name">the name of the color map to install.</param>
		void InstallColorMap(string name);

		/// <summary>
		/// Installs a color map by name, given the input <see cref="ColorMapDescriptor"/>.
		/// </summary>
		/// <param name="descriptor">a <see cref="ColorMapDescriptor"/> describing the color map to be installed.</param>
		void InstallColorMap(ColorMapDescriptor descriptor);

		/// <summary>
		/// Installs a color map.
		/// </summary>
		/// <param name="colorMap">The color map to install.</param>
		void InstallColorMap(IDataLut colorMap);

		/// <summary>
		/// Gets all available color maps in the form of <see cref="ColorMapDescriptor"/>s.
		/// </summary>
		IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }
	}
}
