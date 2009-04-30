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
using System.ComponentModel;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public sealed class KeyImageInformation : IDisposable
	{
		private string _description;
		private string _seriesDescription;
		private KeyObjectSelectionDocumentTitle _docTitle;

		internal readonly BindingList<IClipboardItem> ClipboardItems;

		internal KeyImageInformation()
		{
			_description = "";
			_seriesDescription = "KEY IMAGES";
			_docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

			ClipboardItems = new BindingList<IClipboardItem>();
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (IClipboardItem item in ClipboardItems)
				((IDisposable) item).Dispose();

			ClipboardItems.Clear();
		}

		#endregion
	}
}
