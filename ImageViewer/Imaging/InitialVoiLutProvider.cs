#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO (cr Oct 2009): deprecate and use something with a more appropriate name?
	/// <summary>
	/// A factory that provides the initial voi lut for a given <see cref="IPresentationImage"/>.
	/// </summary>
	public sealed class InitialVoiLutProvider : IInitialVoiLutProvider
	{
		#region Private Fields

		private static readonly InitialVoiLutProvider _instance = new InitialVoiLutProvider();

		private readonly IInitialVoiLutProvider _extensionProvider;

		#endregion

		private InitialVoiLutProvider()
		{
			try
			{
				_extensionProvider = new InitialVoiLutProviderExtensionPoint().CreateExtension() as IInitialVoiLutProvider;
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#region Public Members

		/// <summary>
		/// The single instance of the provider/factory.
		/// </summary>
		public static InitialVoiLutProvider Instance
		{
			get { return _instance; }
		}

		#region IInitialVoiLutProvider Members

		/// <summary>
		/// Determines and returns the initial Voi Lut that should be applied to the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The <see cref="IPresentationImage"/> whose intial Lut is to be determined.</param>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		public IVoiLut GetLut(IPresentationImage presentationImage)
		{
			IVoiLut lut = null;
			if (_extensionProvider != null)
				lut = _extensionProvider.GetLut(presentationImage);

			return lut;
		}

		#endregion
		#endregion
	}
}
