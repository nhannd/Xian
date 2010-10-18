#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Security;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an extension point for integrating image viewing capabilities.
	/// </summary>
	[ExtensionPoint]
	public class ViewerIntegrationExtensionPoint : ExtensionPoint<IViewerIntegration>
	{
	}

	/// <summary>
	/// Defines the interface to an integrated image viewer.
	/// </summary>
	public interface IViewerIntegration
	{
		/// <summary>
		/// Opens images associated with the specified accession number.
		/// </summary>
		/// <param name="accessionNumber"></param>
		void Open(string accessionNumber);

		/// <summary>
		/// Closes images associated with the specified accession number.
		/// </summary>
		/// <param name="accessionNumber"></param>
		void Close(string accessionNumber);

		/// <summary>
		/// Ensures that the images associated with the specified accession number are active,
		/// assuming they have already been opened.
		/// </summary>
		/// <param name="accessionNumber"></param>
		void Activate(string accessionNumber);
	}

	/// <summary>
	/// Helper class for interacting with an integrated image viewer.
	/// </summary>
	public static class ViewImagesHelper
	{
		private static readonly IViewerIntegration _viewer;

		/// <summary>
		/// Class initializer.
		/// </summary>
		static ViewImagesHelper()
		{
			try
			{
				_viewer = (IViewerIntegration)(new ViewerIntegrationExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No viewer integration extension found.");
			}
		}

		#region Public API

		/// <summary>
		/// Gets a value indicating if image viewing is supported.
		/// </summary>
		/// <remarks>
		/// Image viewing is supported if a viewer integration extension was successfully instantiated.
		/// </remarks>
		public static bool IsSupported
		{
			get { return _viewer != null; }
		}

		/// <summary>
		/// Gets a value indicating whether the current user has permission to view images.
		/// </summary>
		public static bool UserHasAccessToViewImages
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Images.View); }
		}

		/// <summary>
		/// Attempts to open images for the specified accession number, returning false if the images cannot be opened.
		/// </summary>
		/// <param name="accession"></param>
		/// <returns></returns>
		public static bool TryOpen(string accession)
		{
			try
			{
				Open(accession);
				return true;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e);
				return false;
			}
		}

		/// <summary>
		/// Opens the images associated with the specified accession number.
		/// </summary>
		/// <param name="accession"></param>
		public static void Open(string accession)
		{
			CheckSupported();
			CheckAccess();

			_viewer.Open(accession);
		}

		/// <summary>
		/// Activates the images associated with the specified accession number, assuming they are already open.
		/// </summary>
		/// <param name="accession"></param>
		public static bool Activate(string accession)
		{
			CheckSupported();
			CheckAccess();

			try
			{
				_viewer.Activate(accession);
				return true;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to activate the viewer for Accession# {0}.", accession));
			}

			return false;
		}

		/// <summary>
		/// Closes the images associated with the specified accession number.
		/// </summary>
		/// <param name="accession"></param>
		public static bool Close(string accession)
		{
			CheckSupported();

			// no need to check access on close!

			try
			{
				_viewer.Close(accession);
				return true;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to close the viewer for Accession# {0}.", accession));
			}

			return false;
		}

		#endregion


		private static void CheckSupported()
		{
			if (_viewer == null)
				throw new NotSupportedException("No viewer integration extension found.");
		}

		private static void CheckAccess()
		{
			if (!UserHasAccessToViewImages)
				throw new SecurityException("Access to images denied.");
		}

	}
}
