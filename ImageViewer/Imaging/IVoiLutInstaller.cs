#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Interface to an entity responsible for managing installation of VOI LUTs.
	/// </summary>
	public interface IVoiLutInstaller
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		IVoiLut VoiLut { get; }

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="voiLut">The Lut to be installed.</param>
		void InstallVoiLut(IVoiLut voiLut);

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		bool Invert { get; set; }

        /// <summary>
        /// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
        /// for all MONOCHROME1 images.
        /// </summary>
        bool DefaultInvert { get; }
	}
}
