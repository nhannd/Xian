#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: change how this manager/provider relationship works ... it just doesn't feel right.

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
	/// Implementors must not return null from the <see cref="IColorMapInstaller.ColorMap"/> property.
	/// </para>
	/// </remarks>
	public interface IColorMapManager : IColorMapInstaller, IMemorable
	{
		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		[Obsolete("Use the ColorMap property instead.")]
		IColorMap GetColorMap();
	}
}
