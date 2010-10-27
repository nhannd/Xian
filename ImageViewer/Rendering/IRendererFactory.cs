#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Thrown when an <see cref="IRendererFactory"/> cannot initialize, 
	/// for example when the required hardware is not present.
	/// </summary>
	public class RendererFactoryInitializationException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public RendererFactoryInitializationException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// A factory for <see cref="IRenderer"/>s.
	/// </summary>
	public interface IRendererFactory
	{
		/// <summary>
		/// Initializes a <see cref="IRendererFactory"/>.
		/// </summary>
		/// <exception cref="RendererFactoryInitializationException">
		/// Thrown when the <see cref="IRendererFactory"/> cannot initialize, for example
		/// when the required hardware is not present.
		/// </exception>
		void Initialize();

		/// <summary>
		/// Gets an <see cref="IRenderer"/>.
		/// </summary>
		IRenderer GetRenderer();
	}
}
