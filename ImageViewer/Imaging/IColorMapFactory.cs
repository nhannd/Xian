#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An extension point for <see cref="IColorMapFactory"/>s.
	/// </summary>
	/// <seealso cref="IColorMapFactory"/>
	public sealed class ColorMapFactoryExtensionPoint : ExtensionPoint<IColorMapFactory>
	{
	}

	/// <summary>
	/// A factory for color maps.
	/// </summary>
	/// <seealso cref="ColorMapFactoryExtensionPoint"/>
	/// <seealso cref="ColorMap"/>
	public interface IColorMapFactory
	{
		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should <b>not</b> be a resource string, as it should be language-independent.
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Creates a color map.
		/// </summary>
		IColorMap Create();
	}
}
