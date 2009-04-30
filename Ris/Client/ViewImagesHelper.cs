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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class ViewerIntegrationExtensionPoint : ExtensionPoint<IViewerIntegration>
    {
    }

    public interface IViewerIntegration
    {
        void Open(string accessionNumber);
    	void Close(string accessionNumber);
    	void Activate(string accessionNumber);
    }

	public static class ViewImagesHelper
	{
		private static readonly IViewerIntegration _viewer;

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

		private static void CheckSupported()
		{
			if (_viewer == null)
				throw new NotSupportedException("No viewer integration extension found.");
		}

		public static bool IsSupported
		{
			get { return _viewer != null; }
		}

		public static void Open(string accession)
		{
			CheckSupported();
			_viewer.Open(accession);
		}

		public static bool Close(string accessionNumber)
		{
			CheckSupported();

			try
			{
				_viewer.Close(accessionNumber);
				return true;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to close the viewer for Accession# {0}.", accessionNumber));
			}

			return false;
		}

		public static bool Activate(string accessionNumber)
		{
			CheckSupported();

			try
			{
				_viewer.Activate(accessionNumber);
				return true;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to activate the viewer for Accession# {0}.", accessionNumber));
			}

			return false;
		}
	}
}
