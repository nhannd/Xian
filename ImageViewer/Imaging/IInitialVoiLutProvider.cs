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
	/// An extension point for custom <see cref="IInitialVoiLutProvider"/>s.
	/// </summary>
	/// <seealso cref="IInitialVoiLutProvider"/>
	public sealed class InitialVoiLutProviderExtensionPoint : ExtensionPoint<IInitialVoiLutProvider>
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public InitialVoiLutProviderExtensionPoint()
		{
		}
	}

	//TODO (cr Oct 2009): deprecate and use something with a more appropriate name?

	/// <summary>
	/// A provider of an image's Initial Voi Lut.
	/// </summary>
	/// <remarks>
	/// Implementors can apply logic based on the input <see cref="IPresentationImage"/> 
	/// to decide what type of Lut to return.
	/// </remarks>
	/// <seealso cref="InitialVoiLutProviderExtensionPoint"/>
	public interface IInitialVoiLutProvider
	{
		/// <summary>
		/// Determines and returns the initial Voi Lut that should be applied to the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The <see cref="IPresentationImage"/> whose intial Lut is to be determined.</param>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		IVoiLut GetLut(IPresentationImage presentationImage);
	}
}
