#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Interface to an entity responsible for managing installation of color maps.
	/// </summary>
	public interface IColorMapInstaller
	{
		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		IColorMap ColorMap { get; }

		/// <summary>
		/// Installs a color map by name.
		/// </summary>
		void InstallColorMap(string name);

		/// <summary>
		/// Installs a color map by <see cref="ColorMapDescriptor">descriptor</see>.
		/// </summary>
		void InstallColorMap(ColorMapDescriptor descriptor);

		/// <summary>
		/// Installs a color map.
		/// </summary>
		void InstallColorMap(IColorMap colorMap);

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s for all the different types of available color maps.
		/// </summary>
		IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }
	}
}
