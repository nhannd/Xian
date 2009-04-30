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

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Imaging
{
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
		/// <returns>The Voi Lut as an <see cref="IComposableLut"/>.</returns>
		public IComposableLut GetLut(IPresentationImage presentationImage)
		{
			IComposableLut lut = null;
			if (_extensionProvider != null)
				lut = _extensionProvider.GetLut(presentationImage);

			return lut;
		}

		#endregion
		#endregion
	}
}
