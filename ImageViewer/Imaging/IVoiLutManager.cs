#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: change how this manager/provider relationship works ... it just doesn't feel right.

	/// <summary>
	/// A VOI LUT Manager, which is responsible for managing installation and restoration
	/// of VOI LUTs via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// Implementors must not return null from the <see cref="IVoiLutInstaller.VoiLut"/> method.
	/// </remarks>
	/// <seealso cref="IVoiLutProvider"/>
	/// <seealso cref="IVoiLut"/>
	public interface IVoiLutManager : IVoiLutInstaller, IMemorable
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		[Obsolete("Use the VoiLut property instead.")]
		IVoiLut GetLut();

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="voiLut">The Lut to be installed.</param>
		[Obsolete("Use the InstallVoiLut method instead.")]
		void InstallLut(IVoiLut voiLut);

		/// <summary>
		/// Toggles the state of the <see cref="IVoiLutInstaller.Invert"/> property.
		/// </summary>
		[Obsolete("Use the Invert property instead.")]
		void ToggleInvert();

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering the parent object.
		/// </summary>
		bool Enabled { get; set; }
	}
}
